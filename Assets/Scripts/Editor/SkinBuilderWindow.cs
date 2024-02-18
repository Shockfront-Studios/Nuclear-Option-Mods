using UnityEngine;

public abstract class ModBuilder : ScriptableObject
{
    [Header("Bundle Settings")]
    public string BundleName;
    public bool CopyToAppData = true;

    public abstract void AddToBuilder(BundleBuilder builder);
}

public abstract class ModBuilder<TData, TMeta> : ModBuilder where TData : ScriptableObject where TMeta : struct
{
    public override void AddToBuilder(BundleBuilder builder)
    {
        builder.AddAsset(CreateObject(), Label, true);
        builder.AddMetaData("meta.json", CreateMeta());
    }

    protected abstract string Label { get; }
    public abstract TData CreateObject();
    public abstract TMeta CreateMeta();
}
