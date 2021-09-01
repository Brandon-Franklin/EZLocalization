using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace EZLocalization
{
    [CreateAssetMenu(fileName = "Localization Database", menuName = "EZLoc/Loc DB", order = 0)]
    public class LocalizationDatabase : ScriptableObject
    {
        [HideInInspector]
        public CountObject count_so;
        //[HideInInspector]
        public LocalizationLanguages localizedLanguages;

        [HideInInspector]
        public List<ID> stringID_keys;
        [HideInInspector]
        public List<LanguageStrings> strings;


        public LanguageStrings GetDBElement(ID targetID)
        {
            if (stringID_keys.Contains(targetID))
            {
                int index = stringID_keys.IndexOf(targetID);
                return strings[index];
            }
            else
            {
                Debug.LogError($"{targetID} is not a valid Loc ID");
                return null;
            }
        }

        public string GetLocString(ID targetID)
        {
            if (stringID_keys.Contains(targetID))
            {
                int index = stringID_keys.IndexOf(targetID);
                return strings[index].languageStrings[(int)localizedLanguages.currentLanguage];
            }
            else
            {
                Debug.LogError($"{targetID} is not a valid Loc ID");
                return null;
            }
        }
        public string GetLocString(string targetID_string)
        {
            ID targetID = new ID(targetID_string);
            return GetLocString(targetID);
        }

        public AudioClip GetLocClip(ID targetID)
        {
            if (stringID_keys.Contains(targetID))
            {
                int index = stringID_keys.IndexOf(targetID);
                return strings[index].languageClips[(int)localizedLanguages.currentLanguage];
            }
            else
            {
                Debug.LogError($"{targetID} is not a valid Loc ID");
                return null;
            }
        }

        public AudioClip GetLocClip(string targetID_string)
        {
            ID targetID = new ID(targetID_string);
            return GetLocClip(targetID);
        }

        public bool TryGetLocLanguagesObject()
        {
            localizedLanguages = Resources.Load("Localization Languages") as LocalizationLanguages;

            if (localizedLanguages == null)
            {
                Debug.LogError("Missing a \"Localization Languages\" LocalizationLanguages object in a resources folder. Please create one with Assets/Create/Speech#/Loc Languages and move it to a Resources folder.");
                return false;
            }
            else
            {
                return true;
            }
        }
#if UNITY_EDITOR
        string errorText = "<Missing Text>";

        public void AddNewLanguageEntry()
        {
            for (int j = 0; j < strings.Count; j++)
            {
                strings[j].languageStrings.Add(errorText);
                strings[j].languageClips.Add(null);
            }
        }

        public ID AddNewLocEntry(string text = "")
        {
            ID id = GUIDGenerator.ConvertIntToGUID(GetNextCount());

            if (TryGetLocLanguagesObject() && !stringID_keys.Contains(id) && localizedLanguages.languages.Count > 0)
            {
                stringID_keys.Add(id);
                LocalizationDatabase.LanguageStrings newStrings = new LocalizationDatabase.LanguageStrings();
                strings.Add(new LocalizationDatabase.LanguageStrings());
                int addedIndex = strings.Count - 1;
                strings[addedIndex].languageStrings = new List<string>();
                strings[addedIndex].languageClips = new List<AudioClip>();

                for (int i = 0; i < localizedLanguages.languages.Count; i++)
                {
                    if (i == (int)localizedLanguages.currentLanguage)
                    {
                        strings[addedIndex].languageStrings.Add(text != string.Empty ? text : errorText);
                    }
                    else
                    {
                        strings[addedIndex].languageStrings.Add(errorText);
                    }
                    strings[addedIndex].languageClips.Add(null);
                }
            }
            return id;
        }

        public void ClearDatabase()
        {
            stringID_keys = new List<ID>();
            strings = new List<LocalizationDatabase.LanguageStrings>();

            if (count_so != null)
            {
                count_so.count = 0;
            }
        }

        public void Initialize()
        {
            if (count_so == null)
            {
                CreateCountScriptableObjectAndAddToThisAsset();
            }
            if (stringID_keys == null)
            {
                stringID_keys = new List<ID>();
            }
            if (strings == null)
            {
                strings = new List<LanguageStrings>();
            }
        }

        public int GetNextCount()
        {
            if (count_so != null)
            {
                int nextInt = count_so.count;
                count_so.count += 1;
                return nextInt;
            }
            else
            {
                Debug.LogError("count_so is Null for the Localization Database asset");
                return -1;
            }
        }

        public void CreateCountScriptableObjectAndAddToThisAsset()
        {          
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
				CountObject counter = CreateInstance(typeof(CountObject)) as CountObject;
				counter.name = "Loc ID Counter";
				AssetDatabase.AddObjectToAsset(counter, this);
				count_so = counter;
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
        }

        private void OnValidate()
        {
            AddMissingEntries();
            EditorSave();
        }

        void AddMissingEntries()
        {
            for (int i = 0; i < strings.Count; i++)
            {
                if (strings[i].languageStrings.Count < localizedLanguages.languages.Count)
                {
                    int missingCount = localizedLanguages.languages.Count - strings[i].languageStrings.Count;
                    for (int j = 0; j < missingCount; j++)
                    {
                        strings[i].languageStrings.Add(errorText);
                    }
                }
                if (strings[i].languageClips.Count < localizedLanguages.languages.Count)
                {
                    int missingCount = localizedLanguages.languages.Count - strings[i].languageClips.Count;
                    for (int j = 0; j < missingCount; j++)
                    {
                        strings[i].languageClips.Add(null);
                    }
                }
            }
        }

        void EditorSave()
        {          
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
        }

        [MenuItem("CONTEXT/LocalizationDatabase/Export CSVs")]
        public static void ExportCSV(MenuCommand command)
        {
            string exportRootPath = EditorUtility.SaveFolderPanel("CSV Export", "", "CSV Export Folder");

            LocalizationDatabase locBD = (LocalizationDatabase)command.context;
            for (int j = 0; j < locBD.localizedLanguages.languages.Count; j++)
            {
                string folderPath = exportRootPath + "/" + locBD.localizedLanguages.languages[j] + "/";
                string filePath = exportRootPath + "/" + locBD.localizedLanguages.languages[j] + "/" + $"loc_{locBD.localizedLanguages.languages[j]}.csv";
                string fileString = string.Empty;

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                for (int i = 0; i < locBD.stringID_keys.Count; i++)
                {
                    fileString += locBD.stringID_keys[i].hex + ",";
                    fileString += locBD.strings[i].languageStrings[j];
                    if (i < locBD.stringID_keys.Count - 1)
                    {
                        fileString += "\n";
                    }
                }

                File.WriteAllText(filePath, fileString);
            }
        }

        [MenuItem("CONTEXT/LocalizationDatabase/Import CSVs")]
        public static void ImportCSV(MenuCommand command)
        {
            string importRootPath = EditorUtility.SaveFolderPanel("CSV Import", "", "CSV Import Folder");

            LocalizationDatabase locDB = (LocalizationDatabase)command.context;

            var info = new DirectoryInfo(importRootPath);
            DirectoryInfo[] di = info.GetDirectories();
            for (int j = 0; j < locDB.localizedLanguages.languages.Count; j++)
            {
                for (int i = 0; i < di.Length; i++)
                {
                    if (di[i].Name == locDB.localizedLanguages.languages[j])
                    {
                        string[] files = System.IO.Directory.GetFiles(di[i].FullName, "*.csv");
                        string filePath = string.Empty;
                        for (int x = 0; x < files.Length; x++)
                        {
                            if (Path.GetFileName(files[x]) == $"loc_{locDB.localizedLanguages.languages[j]}.csv")
                            {
                                filePath = files[x];
                                break;
                            }
                        }

                        if (filePath != string.Empty)
                        {
                            string fileContent = File.ReadAllText(filePath);
                            string[] lines = fileContent.Split('\n');
                            for (int a = 0; a < lines.Length; a++)
                            {
                                string[] elements = lines[a].Split(',');
                                LanguageStrings ls = locDB.GetDBElement(new ID(elements[0]));

                                ls.languageStrings[j] = elements[1];
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"No folder for {locDB.localizedLanguages.languages[j]}");
                        }
                    }
                }
            }
        }
#endif
        [System.Serializable]
        public class LanguageStrings
        {
            public List<string> languageStrings;
            public List<AudioClip> languageClips;
        }
    }
}
