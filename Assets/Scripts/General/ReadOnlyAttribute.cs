using UnityEditor;
using UnityEngine;

// Custom UnityEditor inspector property.
// Disables built-in type modification via inspector.
// Ideal for displaying serialized members for debugging.
public class ReadOnlyAttribute : PropertyAttribute
{}

[CustomPropertyDrawer(typeof (ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
        GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
