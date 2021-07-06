﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace EZLocalization
{
    [CreateAssetMenu(fileName = "Localization Languages", menuName = "EZLoc/Loc Languages", order = 0)]
    public class LocalizationLanguages : ScriptableObject
    {
        public LanguagesEnum.LocalizedLanguages currentLanguage;
        [HideInInspector]
        public List<string> languages;
        static List<string> languages_static;
        public void UpdateStaticLanguagesList()
        {
            languages_static = languages;
        }
#if UNITY_EDITOR
        public void AddLanguageID(string languageID)
        {
            if (!languages.Contains(languageID))
            {
                languages.Add(languageID);
                UpdateStaticLanguagesList();
                UpdateLocLanguages();
            }
        }
        public void Initialize()
        {
            if (languages == null)
            {
                languages = new List<string>();
                languages.Add("en");
            }
            if (!languages.Contains("en"))
            {
                languages.Add("en");
            }
            languages_static = languages;
        }

        public void ClearLanguages()
        {
            languages = new List<string>();
            languages.Add("en");
            UpdateStaticLanguagesList();
            UpdateLocLanguages();
        }

        [MenuItem("CONTEXT/LocalizationDatabase/Update Language Enum")]
        public static void UpdateLocLanguages()
        {
            TextAsset template = Resources.Load("Templates/LanguagesEnumTemplate") as TextAsset;
            string templateText = template.text;

            string path = "Assets/LanguagesEnum.cs";

            //try and see if this file already exists in the project to update rather than make a new one
            string[] results = AssetDatabase.FindAssets("LanguagesEnum");
            foreach (string guid in results)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if ("LanguagesEnum" == Path.GetFileNameWithoutExtension(assetPath))
                {
                    path = assetPath;
                }
            }

            StreamWriter outfile = new StreamWriter(path);
            using (outfile)
            {
                templateText = templateText.Replace("LanguagesEnumTemplate", "LanguagesEnum");

                templateText = templateText.Replace("//Enum Location", CreateEnumTextFromLanugageList());
                outfile.WriteLine(templateText);
            }
            //File written
            AssetDatabase.Refresh();
            Debug.Log($"Updating the languages enum at path: {path}");
        }
#endif
        static string CreateEnumTextFromLanugageList()
        {
            string enumString = "public enum LocalizedLanguages {";
            for (int i = 0; i < languages_static.Count; i++)
            {
                if (i > 0)
                {
                    enumString += ",";
                }
                enumString += "\n\t\t" + languages_static[i];
            }
            enumString += "\n\t}";
            return enumString;
        }
    }
}
