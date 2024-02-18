using UnityEditor;
using UnityEngine;

public class CreateModWindow : EditorWindow
{
    [MenuItem("Nuclear Option/Create Mod Window")]
    private static void CreateWindow()
    {
        CreateWindow<CreateModWindow>();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Create Livery Builder"))
        {
            var builder = ScriptableObjectUtility.CreateAsset<LiveryBuilder>("Livery Builder", "Assets", true, true);
            Selection.activeObject = builder;
            Close();
        }
    }
}
