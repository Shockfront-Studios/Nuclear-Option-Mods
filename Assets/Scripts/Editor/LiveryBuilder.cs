using System;
using UnityEngine;

public class LiveryBuilder : ModBuilder<LiveryData, LiveryMetaData>
{
    [Header("Livery Settings")]
    public Texture2D Texture;
    [Range(0, 1)] public float Glossiness;
    public string DisplayName;
    public string Faction;
    public string Aircraft;

    protected override string Label => "Skin";

    public override LiveryData CreateObject()
    {
        var asset = ScriptableObject.CreateInstance<LiveryData>();
        asset.Texture = Texture;
        asset.Glossiness = Glossiness;
        return asset;
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
}
