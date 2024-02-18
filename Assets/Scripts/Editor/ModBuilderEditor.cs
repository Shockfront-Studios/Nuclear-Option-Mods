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

        GUILayout.Space(20);
        GUIHelper.HorizontalLine();
        GUILayout.Space(10);

        if (GUILayout.Button("Build"))
        {
            Build();
        }
    }

    private void Build()
    {
        var builder = new BundleBuilder(Target.BundleName);
        Target.AddToBuilder(builder);
        builder.Build(copyToPersistentDataPath: Target.CopyToAppData);
    }
}
