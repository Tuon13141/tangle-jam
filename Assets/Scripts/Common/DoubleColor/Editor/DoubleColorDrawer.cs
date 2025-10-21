using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DoubleColor))]
public class DoubleColorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Start the property
        EditorGUI.BeginProperty(position, label, property);

        // Get the properties
        SerializedProperty minValueProperty = property.FindPropertyRelative("color1");
        SerializedProperty maxValueProperty = property.FindPropertyRelative("color2");

        // Calculate rects
        Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
        Rect minMaxRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height);

        // Draw the main label
        EditorGUI.LabelField(labelRect, label);

        // Set up the indent level
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects for min and max labels and fields
        float labelWidth = 50f;
        float fieldWidth = (minMaxRect.width - 4 - 2 * labelWidth) / 2;

        Rect minLabelRect = new Rect(minMaxRect.x, minMaxRect.y, labelWidth, minMaxRect.height);
        Rect minRect = new Rect(minMaxRect.x + labelWidth, minMaxRect.y, fieldWidth, minMaxRect.height);

        Rect maxLabelRect = new Rect(minMaxRect.x + labelWidth + fieldWidth + 4, minMaxRect.y, labelWidth, minMaxRect.height);
        Rect maxRect = new Rect(minMaxRect.x + labelWidth + fieldWidth + 4 + labelWidth, minMaxRect.y, fieldWidth, minMaxRect.height);

        // Draw labels and fields for min and max values
        EditorGUI.LabelField(minLabelRect, "Color 1");
        EditorGUI.PropertyField(minRect, minValueProperty, GUIContent.none);

        EditorGUI.LabelField(maxLabelRect, "Color 2");
        EditorGUI.PropertyField(maxRect, maxValueProperty, GUIContent.none);

        // Restore the indent level
        EditorGUI.indentLevel = indent;

        // End the property
        EditorGUI.EndProperty();
    }
}
