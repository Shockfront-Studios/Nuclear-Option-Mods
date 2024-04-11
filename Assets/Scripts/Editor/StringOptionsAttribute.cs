using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

public class StringOptionsAttribute : PropertyAttribute
{
    public string[] Options { get; private set; }

    public StringOptionsAttribute(params string[] options)
    {
        Options = options;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(StringOptionsAttribute))]
public class StringOptionsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var stringOptionsAttribute = (StringOptionsAttribute)attribute;
        var options = stringOptionsAttribute.Options;

        if (property.propertyType == SerializedPropertyType.String)
        {
            var selectedIndex = 0;
            for (var i = 0; i < options.Length; i++)
            {
                if (options[i] == property.stringValue)
                {
                    selectedIndex = i;
                    break;
                }
            }

            var displayOptions = options.Select(x =>
            {
                if (string.IsNullOrEmpty(x))
                    return "<none>";

                return x.Replace("/", "");
            }).ToArray();
            selectedIndex = EditorGUI.Popup(position, property.displayName, selectedIndex, displayOptions);
            property.stringValue = options[selectedIndex];
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use StringOptions with string.");
        }
    }
}
#endif
