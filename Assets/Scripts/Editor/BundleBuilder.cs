using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using Object = UnityEngine.Object;

public class BundleBuilder
{
    private readonly AddressableAssetSettings settings;
    private readonly BundledAssetGroupSchema schema;
    private readonly AddressableAssetGroup group;
    private string bundleName;
    private readonly List<(string name, string json)> metaData = new();

    public BundleBuilder(string bundleName)
    {
        if (string.IsNullOrEmpty(bundleName))
            throw new ArgumentNullException(nameof(bundleName));

        this.bundleName = bundleName;
        settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
        settings.BuildRemoteCatalog = true;
        //settings.OverridePlayerVersion = ""; // empty string uses DateTime instead of version
        settings.OverridePlayerVersion = "1";

        DeleteAllGroups(settings);

        // Add a new group
        group = settings.CreateGroup(bundleName, false, false, true, null);
        schema = group.AddSchema<BundledAssetGroupSchema>();
        schema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
        schema.IncludeAddressInCatalog = false;
        schema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;

        // set paths
        SetPaths();
    }

    private static void DeleteAllGroups(AddressableAssetSettings settings)
    {
        foreach (var g in settings.groups.ToArray())
            settings.RemoveGroup(g);
        Debug.Assert(settings.groups.Count == 0);
    }

    private void SetPaths()
    {
        var buildPath = $"Bundles/{bundleName}/";
        // note: this will be evaluated at runtime
        // see: https://forum.unity.com/threads/change-remote-catalog-load-path-at-runtime.712052/#post-4761680
        var loadPath = $"{{NuclearOption.AddressablePaths.AppDataSkinPath}}/{bundleName}/";

        settings.profileSettings.SetValue(settings.activeProfileId, "Remote.BuildPath", buildPath);
        settings.profileSettings.SetValue(settings.activeProfileId, "Remote.LoadPath", loadPath);

        schema.BuildPath.SetVariableByName(settings, "Remote.BuildPath");
        schema.LoadPath.SetVariableByName(settings, "Remote.LoadPath");
    }

    public void AddAsset(Object asset, string label, bool createAsset)
    {
        if (createAsset)
            CreateAsset(asset);

        if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out var guid, out long _))
        {
            Debug.LogError("Failed to get GUID for livery");
            return;
        }

        var entry = settings.CreateOrMoveEntry(guid, group);
        entry.SetLabel(label, true, true, false);
    }
    private static void CreateAsset(UnityEngine.Object asset)
    {
        const string TEMP_FOLDER = "Assets/TempData/";
        const string TEMP_LIVERY_DATA = "Assets/TempData/Data.asset";

        if (!Directory.Exists(TEMP_FOLDER))
            Directory.CreateDirectory(TEMP_FOLDER);

        AssetDatabase.DeleteAsset(TEMP_LIVERY_DATA);
        // unity is dumb and will use same GUID if we use the same path.
        // so we have to first create random path. create asset. then copy to temp path
        var rndPath = $"Assets/{Path.GetRandomFileName()}.asset";
        AssetDatabase.CreateAsset(asset, rndPath);
        var name = AssetDatabase.GenerateUniqueAssetPath(TEMP_LIVERY_DATA);
        AssetDatabase.MoveAsset(rndPath, name);
    }

    public void AddMetaData<T>(string fileName, T data)
    {
        var json = JsonUtility.ToJson(data);
        metaData.Add((fileName, json));
    }

    public void Build(bool copyToPersistentDataPath)
    {
        // Build content
        var buildPath = $"Bundles/{bundleName}/";
        if (Directory.Exists(buildPath))
            Directory.Delete(buildPath, true);
        AddressableAssetSettings.CleanPlayerContent(settings.ActivePlayerDataBuilder);
        AddressableAssetSettings.BuildPlayerContent();

        // Log the path where the bundle was built to
        var dir = schema.BuildPath.GetValue(settings);
        Debug.Log($"Bundle was built to: {dir}");

        // write metadata
        foreach (var (name, json) in metaData)
        {
            var path = Path.Join(dir, name);
            File.WriteAllText(path, json);
            Debug.Log($"Writing meta data to {path}");
        }

        if (copyToPersistentDataPath)
        {
            var dataDir = Path.Combine(Application.persistentDataPath, "Skins", bundleName);
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            foreach (var filePath in Directory.GetFiles(dir))
            {
                var fileName = Path.GetFileName(filePath);
                var copyTo = Path.Combine(dataDir, fileName);
                File.Copy(filePath, copyTo, true);
                Debug.Log($"Copy File\n  from:{filePath}\n    to:{copyTo}");
            }
        }
    }
}
