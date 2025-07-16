using UnityEngine;
using UnityEditor;

public class MergeMeshTool
{
    [MenuItem("Tools/MergeMesh")]
    static void MergeSelectedMeshes()
    {
        var selected = Selection.gameObjects;

        if (selected.Length == 0)
        {
            Debug.LogWarning("No GameObjects selected.");
            return;
        }

        var combineInstances = new System.Collections.Generic.List<CombineInstance>();
        var materials = new System.Collections.Generic.List<Material>();
        var hasMeshes = false;

        foreach (var obj in selected)
        {
            var meshFilter = obj.GetComponent<MeshFilter>();
            var meshRenderer = obj.GetComponent<MeshRenderer>();

            if (meshFilter == null || meshRenderer == null || meshFilter.sharedMesh == null)
                continue;

            var combine = new CombineInstance
            {
                mesh = meshFilter.sharedMesh,
                transform = meshFilter.transform.localToWorldMatrix
            };
            combineInstances.Add(combine);
            materials.AddRange(meshRenderer.sharedMaterials);
            hasMeshes = true;
        }

        if (!hasMeshes)
        {
            Debug.LogWarning("No meshes found in selected GameObjects.");
            return;
        }

        // Create the new GameObject and set transform to identity
        var newObject = new GameObject("MergedMesh")
        {
            transform =
            {
                position = Vector3.zero,
                rotation = Quaternion.identity,
                localScale = Vector3.one
            }
        };

        var filter = newObject.AddComponent<MeshFilter>();
        var renderer = newObject.AddComponent<MeshRenderer>();

        // Combine meshes
        var combinedMesh = new Mesh { name = "CombinedMesh" };
        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
        filter.sharedMesh = combinedMesh;

        // Assign material
        if (materials.Count > 0)
            renderer.sharedMaterial = materials[0];

        // Set parent after mesh assignment and preserve world transform
        var lastSelected = selected[^1];
        newObject.transform.SetParent(lastSelected.transform.parent, false); // false = maintain local transform
        newObject.transform.position = Vector3.zero;
        newObject.transform.rotation = Quaternion.identity;
        newObject.transform.localScale = Vector3.one;

        Undo.RegisterCreatedObjectUndo(newObject, "Merge Mesh");
        Debug.Log("Mesh merge complete.");
    }
}
