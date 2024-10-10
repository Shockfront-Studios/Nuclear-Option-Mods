using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LiveryBuilder : ModBuilder<LiveryData, LiveryMetaData>
{
    [Header("Livery Settings")]
    public Texture2D Texture;
    [Range(0, 1)] public float Glossiness;
    public string DisplayName;

    [StringOptions(
        "",
        "Boscali",
        "Primeva"
    )]
    public string Faction;

    [StringOptions(
        "",
        "CI-22 Cricket",
        "T/A-30 Compass",
        "SAH-46 Chicane",
        "FS-12 Revoker",
        "KR-67 Ifrit",
        "EW-25 Medusa",
        "SFB-81 Darkreach"
    )]
    public string Aircraft;

    public bool UseCustomColors;
    public LiveryData.TextureColor[] Colors = new LiveryData.TextureColor[5];

    protected override string Label => "Skin";

    public override LiveryData CreateObject()
    {
        var asset = ScriptableObject.CreateInstance<LiveryData>();
        asset.Texture = Texture;
        asset.Glossiness = Glossiness;
        asset.Colors = UseCustomColors ? Colors : GetColorsFromTexture();
        return asset;
    }

    public LiveryData.TextureColor[] GetColorsFromTexture()
    {
        return TextureColorCalculator.GetPallet(Texture, 5)
            .Select(x => new LiveryData.TextureColor { Color = x.color, Count = x.count })
            .ToArray();
    }


    public override LiveryMetaData CreateMeta()
    {
        return new LiveryMetaData
        {
            DisplayName = DisplayName,
            Faction = Faction,
            Aircraft = Aircraft,
        };
    }

    private void OnValidate()
    {
        if (Colors == null)
            Colors = new LiveryData.TextureColor[5];
        if (Colors.Length != 5)
            Array.Resize(ref Colors, 5);
    }
}

internal static class TextureColorCalculator
{
    public static (Color32 color, int count)[] GetPallet(Texture2D texture, int size)
    {
        var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));
        var readable = importer.isReadable;
        importer.isReadable = true;
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texture), ImportAssetOptions.ForceUpdate);
        try
        {
            return MostCommonColors_4Bit(texture, size);
        }
        finally
        {
            importer.isReadable = readable;
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texture), ImportAssetOptions.ForceUpdate);
        }
    }

    private static (Color32, int)[] MostCommonColors_4Bit(Texture2D tex, int paletteSize)
    {
        var texColors = tex.GetPixels32();
        var total = texColors.Length;

        const int bitDepth = 4;
        const int range = 1 << bitDepth;
        var shiftAmount = 8 - bitDepth;

        var counts = new int[range * range * range];

        for (var i = 0; i < total; i++)
        {
            var r = (byte)(texColors[i].r >> shiftAmount);
            var g = (byte)(texColors[i].g >> shiftAmount);
            var b = (byte)(texColors[i].b >> shiftAmount);

            counts[r + (range * (g + (range * b)))]++;
        }

        var colors = new (Color32 color, int count)[range * range * range];
        for (var i = 0; i < colors.Length; i++)
        {
            var r = (byte)((i % range) << shiftAmount);
            var g = (byte)((i / range % range) << shiftAmount);
            var b = (byte)((i / (range * range)) << shiftAmount);

            var color = new Color32(r, g, b, 255);
            colors[i] = (color, counts[i]);
        }

        var palette = colors
            .OrderByDescending(x => x.count)
            .Take(paletteSize)
            .ToArray();

        return palette;
    }
}
