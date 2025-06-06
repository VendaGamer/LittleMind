using UnityEngine;
using UnityEditor;
using System.Text;

public class SelectedHierarchyPrinter
{
    [MenuItem("Tools/Print Selected Hierarchy %#h")] // Ctrl+Shift+H
    public static void PrintSelectedHierarchy()
    {
        if (Selection.activeTransform == null)
        {
            Debug.LogWarning("No GameObject selected.");
            return;
        }

        StringBuilder sb = new StringBuilder();
        PrintRecursive(Selection.activeTransform, 0, sb);
        Debug.Log(sb.ToString());
    }

    static void PrintRecursive(Transform transform, int indent, StringBuilder sb)
    {
        sb.AppendLine(new string(' ', indent * 2) + "- " + transform.name);
        foreach (Transform child in transform)
        {
            PrintRecursive(child, indent + 1, sb);
        }
    }
}