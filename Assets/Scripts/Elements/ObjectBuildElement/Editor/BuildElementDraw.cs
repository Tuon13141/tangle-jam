using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BuildElement))]
public class BuildElementDraw : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Start the property
        EditorGUI.BeginProperty(position, label, property);

        // Get the properties
        SerializedProperty objectFill = property.FindPropertyRelative("objectFill");
        SerializedProperty countFill = property.FindPropertyRelative("countFill");

        // Calculate rects
        Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
        Rect minMaxRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height);

        // Draw the main label
        EditorGUI.LabelField(labelRect, label);

        // Set up the indent level
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects for min and max labels and fields
        float offsetField = 5f;
        float fieldWidth = (minMaxRect.width - offsetField * 2) / 2;

        Rect rect1 = new Rect(minMaxRect.x, minMaxRect.y, fieldWidth, minMaxRect.height);
        Rect rect2 = new Rect(minMaxRect.x + fieldWidth + offsetField, minMaxRect.y, fieldWidth, minMaxRect.height);

        // Draw labels and fields for min and max values
        EditorGUI.PropertyField(rect1, objectFill, GUIContent.none);
        EditorGUI.PropertyField(rect2, countFill, GUIContent.none);

        // Restore the indent level
        EditorGUI.indentLevel = indent;

        // End the property
        EditorGUI.EndProperty();
    }
}
