using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModBuilder), true)]
public class ModBuilderEditor : Editor
{
    private ModBuilder Target => (ModBuilder)target;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();

        ExtraGui();

        GUILayout.Space(20);
        GUIHelper.HorizontalLine();
        GUILayout.Space(10);

        if (GUILayout.Button("Build"))
        {
            Build();
        }
    }

    protected virtual void ExtraGui() { }


    private void Build()
    {
        var builder = new BundleBuilder(Target.BundleName);
        Target.AddToBuilder(builder);
        builder.Build(copyToPersistentDataPath: Target.CopyToAppData);
    }
}

[CustomEditor(typeof(LiveryBuilder), true)]
public class LiveryBuilderEditor : ModBuilderEditor
{
    private LiveryBuilder Target => (LiveryBuilder)target;

    protected override void ExtraGui()
    {
        if (GUILayout.Button("Get Colors from Texture"))
        {
            Target.Colors = Target.GetColorsFromTexture();
            EditorUtility.SetDirty(Target);
        }
    }
}
