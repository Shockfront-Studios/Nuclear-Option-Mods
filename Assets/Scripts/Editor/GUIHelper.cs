using UnityEditor;
using UnityEngine;

public static class GUIHelper
{
    public static void HorizontalLine()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    public static string GetShortPath(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath))
            return fullPath;

        var shortPath = fullPath;
        if (shortPath.StartsWith(Application.dataPath))
        {
            shortPath = "Assets" + shortPath.Substring(Application.dataPath.Length);
        }

        return shortPath;
    }
}
