using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ScriptableObjectUtility
{
    private static string lastPath = "Assets/GameData/";

    /// <summary>
    ///	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static T CreateAsset<T>(string defaultName = null, string defaultPath = null, bool errorIfExists = false, bool showSavePanel = true) where T : ScriptableObject
    {
        var asset = CreateAssetInternal(typeof(T), defaultName, defaultPath, errorIfExists, showSavePanel);
        return asset != null ? (T)asset : null;
    }

    /// <summary>
    ///	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    private static ScriptableObject CreateAssetInternal(Type type, string defaultName, string defaultPath, bool errorIfExists = false, bool showSavePanel = true)
    {
        if (string.IsNullOrEmpty(defaultName))
            defaultName = "New" + type.Name;

        // if no folder always show save panel
        if (string.IsNullOrEmpty(defaultPath))
        {
            if (!showSavePanel)
                Debug.LogWarning($"showSavePanel must be true when there was no default folder.");
            showSavePanel = true;
        }

        var path = GetPath(defaultName, defaultPath, showSavePanel);

        // user click cancel
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning($"No Path");
            return null;
        }

        Debug.Log($"Creating {type.FullName} at {path}");

        var asset = ScriptableObject.CreateInstance(type);

        SaveAsset(path, asset, errorIfExists);

        return asset;
    }

    private static string GetPath(string defaultName, string defaultPath, bool showSavePanel)
    {
        string path;
        if (showSavePanel)
        {
            path = SavePanel(defaultName, defaultPath);
        }
        else
        {
            path = Path.Combine(defaultPath, defaultName);
            if (!path.EndsWith(".asset"))
                path += ".asset";
        }

        return GUIHelper.GetShortPath(path);
    }

    private static string SavePanel(string name, string defaultPath)
    {
        // if no path, then use last selected path
        if (string.IsNullOrEmpty(defaultPath))
        {
            defaultPath = lastPath;
        }

        if (!name.EndsWith(".asset"))
            name += ".asset";

        return EditorUtility.SaveFilePanel(
            "Save ScriptableObject",
            defaultPath,
            name,
            "asset");
    }

    private static void SaveAsset(string path, ScriptableObject asset, bool errorIfExists)
    {
        if (errorIfExists)
        {
            if (File.Exists(path))
            {
                Debug.LogError("File Exists");
                return;
            }
        }

        CheckDirectoryExists(path);

        var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CheckDirectoryExists(string path)
    {
        var folders = path.Split('/');
        var parent = folders[0];
        // skip First (asset/) and last (file)
        for (var i = 1; i < folders.Length - 1; i++)
        {
            if (!AssetDatabase.IsValidFolder(parent + "/" + folders[i]))
            {
                Debug.LogFormat("creating '{0}'", parent + "/" + folders[i]);
                AssetDatabase.CreateFolder(parent, folders[i]);
            }
            parent += "/" + folders[i];
        }
    }
}
