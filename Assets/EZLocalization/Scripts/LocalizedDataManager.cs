using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EZLocalization
{
    public class LocalizedDataManager : MonoBehaviour
    {
        public LocalizationDatabase centralizedLocDB;

        public static LocalizedDataManager localizedData;

        private void Awake()
        {
            if (centralizedLocDB == null)
            {
                LocalizationDatabase db = Resources.Load("Localization Database") as LocalizationDatabase;
                if (db != null)
                {
                    centralizedLocDB = db;
                }
                else
                {
                    Debug.LogError("No database named \"Localization Database\" found in a resources folder. Please create one from Assets/Create/Speech#/Loc DB and move it to a resources folder");
                }
            }

            localizedData = this;
        }

        public static string GetLocString(ID targetID)
        {
            return localizedData.centralizedLocDB.GetLocString(targetID);
        }
        public static string GetLocString(string targetID_string)
        {
            ID targetID = new ID(targetID_string);
            return GetLocString(targetID);
        }


        public static AudioClip GetLocClip(ID targetID)
        {
            return localizedData.centralizedLocDB.GetLocClip(targetID);
        }

        public static AudioClip GetLocClip(string targetID_string)
        {
            ID targetID = new ID(targetID_string);
            return GetLocClip(targetID);
        }
    }
}
