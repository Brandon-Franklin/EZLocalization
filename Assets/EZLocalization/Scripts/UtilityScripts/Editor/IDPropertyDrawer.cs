using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ID))]
public class IDPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //property.stringValue= EditorGUI.TextField(position, label, property.stringValue);
        EditorGUI.PropertyField(position, property.FindPropertyRelative("Hex"), new GUIContent("ID"), true);

        //EditorGUILayout.LabelField(property.FindPropertyRelative("Hex").stringValue);
        //property.FindPropertyRelative("Hex").stringValue = EditorGUILayout.TextField(property.FindPropertyRelative("Hex").stringValue, GUILayout.ExpandHeight(false));
    }
}
