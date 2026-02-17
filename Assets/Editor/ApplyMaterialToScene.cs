using UnityEditor;
using UnityEngine;

public static class ApplyMaterialToScene
{
    [MenuItem("Tools/Apply Selected Material To Scene")]
    static void Apply()
    {
        Material mat = Selection.activeObject as Material;

        if (mat == null)
        {
            Debug.LogWarning("Select a Material first.");
            return;
        }

        Renderer[] renderers = Object.FindObjectsOfType<Renderer>();

        foreach (Renderer r in renderers)
        {
            Undo.RecordObject(r, "Apply Material To Scene");
            r.sharedMaterial = mat;
            EditorUtility.SetDirty(r);
        }

        Debug.Log($"Applied material '{mat.name}' to {renderers.Length} renderers.");
    }
}
