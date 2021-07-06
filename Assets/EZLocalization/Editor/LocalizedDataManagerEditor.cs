using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EZLocalization;

[CustomEditor(typeof(LocalizedDataManager))]
public class LocalizedDataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LocalizedDataManager locManager = target as LocalizedDataManager;

        if (locManager.centralizedLocDB != null)
        {
            SerializedObject so = new SerializedObject(locManager.centralizedLocDB.localizedLanguages);
            so.Update();
            SerializedProperty languageEnum = so.FindProperty("currentLanguage");
            EditorGUILayout.PropertyField(languageEnum);
            so.ApplyModifiedProperties();
        }
        base.OnInspectorGUI();

    }
}
