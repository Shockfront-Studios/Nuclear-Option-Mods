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
            var selectedIndex = -1;
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

            position = EditorGUI.PrefixLabel(position, label);

            float totalWidth = position.width;
            float textFieldWidth = totalWidth * 0.6f;
            float popupWidth = totalWidth * 0.4f;

            Rect textFieldRect = new Rect(position.x, position.y, textFieldWidth - 2, position.height);
            Rect popupRect = new Rect(position.x + textFieldWidth, position.y, popupWidth, position.height);

            property.stringValue = EditorGUI.TextField(textFieldRect, property.stringValue);

            int newIndex = EditorGUI.Popup(popupRect, selectedIndex, displayOptions);
            if (newIndex >= 0 && newIndex != selectedIndex)
            {
                property.stringValue = options[newIndex];
            }
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use StringOptions with string.");
        }
    }
}
#endif