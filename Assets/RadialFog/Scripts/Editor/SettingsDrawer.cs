using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(Settings))]
public class SettingsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty fogMode = property.FindPropertyRelative("FogMode");
        SerializedProperty startDistance = property.FindPropertyRelative("StartDistance");
        SerializedProperty endDistance = property.FindPropertyRelative("EndDistance");
        SerializedProperty density = property.FindPropertyRelative("Density");
        SerializedProperty color = property.FindPropertyRelative("Color");

  
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), fogMode);

        FogMode mode = (FogMode)fogMode.enumValueIndex;

        switch (mode)
        {
            case FogMode.Linear:
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), startDistance);
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight), endDistance);
                break;
            case FogMode.Exponential:
            case FogMode.Exponential_Squared:
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), density);
                break;
        }

        EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * (mode == FogMode.Linear ? 3 : 2), position.width, EditorGUIUtility.singleLineHeight), color);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty fogMode = property.FindPropertyRelative("FogMode");
        int lines = (FogMode)fogMode.enumValueIndex == FogMode.Linear ? 4 : 3;
        return EditorGUIUtility.singleLineHeight * lines;
    }
}
