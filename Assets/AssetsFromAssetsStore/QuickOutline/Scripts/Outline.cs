//
//  Outline.cs
//  QuickOutline Optimized (No Update Loop)
//
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class Outline : MonoBehaviour
{
    // Cache meshes whose UV3 (smooth normals) have been set.
    private static readonly HashSet<Mesh> RegisteredMeshes = new();

    // Cached shader property IDs.
    private static readonly int PropZTest = Shader.PropertyToID("_ZTest");
    private static readonly int PropWidth = Shader.PropertyToID("_OutlineWidth");
    private static readonly int PropOutlineColor = Shader.PropertyToID("_OutlineColor");

    public enum Mode
    {
        OutlineAll,
        OutlineVisible,
        OutlineHidden,
        OutlineAndSilhouette,
        SilhouetteOnly
    }

    [SerializeField]
    private Mode outlineMode = OutlineSettings.OutlineMode;
    public Mode OutlineMode
    {
        get => outlineMode;
        set
        {
            if (outlineMode == value) return;
            
            outlineMode = value;
            UpdateMaterialProperties();
        }
    }

    [SerializeField]
    private Color outlineColor = OutlineSettings.OutlineColor;
    public Color OutlineColor
    {
        get => outlineColor;
        set
        {
            if (outlineColor == value) return;
            
            outlineColor = value;
            UpdateMaterialProperties();
        }
    }

    [SerializeField, Range(0f, 10f)]
    private float outlineWidth = OutlineSettings.OutlineThickness;
    public float OutlineWidth
    {
        get => outlineWidth;
        set
        {
            if (Mathf.Approximately(outlineWidth, value)) return;
            
            outlineWidth = value;
            UpdateMaterialProperties();
        }
    }

    [Header("Optional")]
    [SerializeField, Tooltip("When enabled, smooth normals are precomputed in the editor and serialized. When disabled, they are computed at runtime.")]
    private bool precomputeOutline;
    
    private readonly Dictionary<Mesh, Vector3[]> bakedSmoothNormals = new();

    // Cached renderers and material instances.
    private Renderer[] renderers;
    private static Material outlineMaskMaterial;
    private static Material outlineFillMaterial;

    private void Awake()
    {
        // Cache renderers.
        renderers = GetComponentsInChildren<Renderer>();
        if (!outlineMaskMaterial)
        {
            outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
        }

        if (!outlineFillMaterial)
        {
            outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));
        }

        // Set up smooth normals.
        LoadSmoothNormals();

        // Immediately update shader properties.
        UpdateMaterialProperties();
    }

    private void OnEnable()
    {
        // Add outline materials without using temporary Lists.
        foreach (var rend in renderers)
        {
            Material[] mats = rend.sharedMaterials;
            bool hasMask = false;
            bool hasFill = false;
            foreach (var t in mats)
            {
                if (t == outlineMaskMaterial)
                {
                    hasMask = true;
                }else if (t == outlineFillMaterial)
                    hasFill = true;
            }
            if (hasMask && hasFill)
                continue;

            int extra = (!hasMask ? 1 : 0) + (!hasFill ? 1 : 0);
            Material[] newMats = new Material[mats.Length + extra];
            Array.Copy(mats, newMats, mats.Length);
            int index = mats.Length;
            if (!hasMask)
                newMats[index++] = outlineMaskMaterial;
            if (!hasFill)
                newMats[index++] = outlineFillMaterial;
            rend.materials = newMats;
        }
    }

    private void OnValidate()
    {
        renderers = GetComponentsInChildren<Renderer>();
        if (!outlineMaskMaterial)
        {
            outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
        }

        if (!outlineFillMaterial)
        {
            outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));
        }
        
        // Update shader properties immediately in the editor.
        UpdateMaterialProperties();

        // Clear precomputed data if baking is disabled.
        if (!precomputeOutline && bakedSmoothNormals.Count > 0)
        {
            bakedSmoothNormals.Clear();
        }
        // Bake smooth normals if enabled and not already done.
        if (precomputeOutline && bakedSmoothNormals.Count == 0)
        {
            Bake();
        }
    }

    private void OnDisable()
    {
        // Remove outline materials without using temporary Lists.
        foreach (var rend in renderers)
        {
            Material[] mats = rend.sharedMaterials;
            int removeCount = mats.Count(t => t == outlineMaskMaterial || t == outlineFillMaterial);
            if (removeCount == 0)
                continue;

            Material[] newMats = new Material[mats.Length - removeCount];
            int newIndex = 0;
            foreach (var t in mats)
            {
                if (t != outlineMaskMaterial && t != outlineFillMaterial)
                {
                    newMats[newIndex++] = t;
                }
            }
            rend.materials = newMats;
        }
    }

    private void OnDestroy()
    {
        // Clean up instantiated materials.
        Destroy(outlineMaskMaterial);
        Destroy(outlineFillMaterial);
    }

    /// <summary>
    /// Precomputes and stores smooth normals for all meshes in children.
    /// </summary>
    private void Bake()
    {
        var processed = new HashSet<Mesh>();
        foreach (var mf in GetComponentsInChildren<MeshFilter>())
        {
            Mesh mesh = mf.sharedMesh;
            if (mesh == null || !processed.Add(mesh))
                continue;
            Vector3[] smoothNormals = ComputeSmoothNormals(mesh);
            bakedSmoothNormals[mesh] = smoothNormals;
        }
    }

    /// <summary>
    /// Loads smooth normals into UV3 (or clears for skinned meshes).
    /// </summary>
    private void LoadSmoothNormals()
    {
        foreach (var mf in GetComponentsInChildren<MeshFilter>())
        {
            Mesh mesh = mf.sharedMesh;
            if (mesh == null || !RegisteredMeshes.Add(mesh))
                continue;

            if (!bakedSmoothNormals.TryGetValue(mesh, out var smoothNormals))
            {
                smoothNormals = ComputeSmoothNormals(mesh);
            }
            mesh.SetUVs(3, smoothNormals);

            var rend = mf.GetComponent<Renderer>();
            if (rend != null)
                CombineSubmeshes(mesh, rend.sharedMaterials);
        }

        // For SkinnedMeshRenderers, clear UV3 and combine submeshes.
        foreach (var smr in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            Mesh mesh = smr.sharedMesh;
            if (mesh == null || !RegisteredMeshes.Add(mesh))
                continue;

            mesh.uv4 = new Vector2[mesh.vertexCount];
            CombineSubmeshes(mesh, smr.sharedMaterials);
        }
    }

    /// <summary>
    /// Computes smooth normals by grouping vertices by position and averaging normals.
    /// </summary>
    private static Vector3[] ComputeSmoothNormals(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector3[] smoothNormals = (Vector3[])normals.Clone();

        // Group vertex indices by their position.
        var counts = new Dictionary<Vector3, int>();
        foreach (var t in vertices)
        {
            counts.TryAdd(t, 0);
            counts[t]++;
        }

        var indicesPerVertex = new Dictionary<Vector3, int[]>(counts.Count);
        foreach (var pair in counts)
        {
            indicesPerVertex[pair.Key] = new int[pair.Value];
        }

        var currentCount = new Dictionary<Vector3, int>();
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 key = vertices[i];
            currentCount.TryAdd(key, 0);
            indicesPerVertex[key][currentCount[key]++] = i;
        }

        foreach (var indices in indicesPerVertex.Values)
        {
            if (indices.Length <= 1)
                continue;
            Vector3 avgNormal = indices.Aggregate(Vector3.zero, (current, t) => current + smoothNormals[t]);
            avgNormal.Normalize();
            foreach (var t in indices)
                smoothNormals[t] = avgNormal;
        }

        return smoothNormals;
    }

    /// <summary>
    /// If a mesh has multiple submeshes and the material count allows it, adds an extra submesh combining all triangles.
    /// </summary>
    private static void CombineSubmeshes(Mesh mesh, Material[] materials)
    {
        if (mesh.subMeshCount == 1 || mesh.subMeshCount > materials.Length)
            return;
        mesh.subMeshCount++;
        mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
    }

    /// <summary>
    /// Updates the outline shader material properties based on the current mode.
    /// </summary>
    private void UpdateMaterialProperties()
    {
        // Update the outline fill material's color.
        outlineFillMaterial.SetColor(PropOutlineColor, outlineColor);

        switch (outlineMode)
        {
            case Mode.OutlineAll:
                SetMaterialProperties(UnityEngine.Rendering.CompareFunction.Always,
                                      UnityEngine.Rendering.CompareFunction.Always, outlineWidth);
                break;
            case Mode.OutlineVisible:
                SetMaterialProperties(UnityEngine.Rendering.CompareFunction.Always,
                                      UnityEngine.Rendering.CompareFunction.LessEqual, outlineWidth);
                break;
            case Mode.OutlineHidden:
                SetMaterialProperties(UnityEngine.Rendering.CompareFunction.Always,
                                      UnityEngine.Rendering.CompareFunction.Greater, outlineWidth);
                break;
            case Mode.OutlineAndSilhouette:
                SetMaterialProperties(UnityEngine.Rendering.CompareFunction.LessEqual,
                                      UnityEngine.Rendering.CompareFunction.Always, outlineWidth);
                break;
            case Mode.SilhouetteOnly:
                SetMaterialProperties(UnityEngine.Rendering.CompareFunction.LessEqual,
                                      UnityEngine.Rendering.CompareFunction.Greater, 0f);
                break;
        }
    }

    /// <summary>
    /// Helper method to set the ZTest and outline width on both outline materials.
    /// </summary>
    private void SetMaterialProperties(UnityEngine.Rendering.CompareFunction maskTest,
                               UnityEngine.Rendering.CompareFunction fillTest, float width)
    {
        outlineMaskMaterial.SetFloat(PropZTest, (float)maskTest);
        outlineFillMaterial.SetFloat(PropZTest, (float)fillTest);
        outlineFillMaterial.SetFloat(PropWidth, width);
    }
}
