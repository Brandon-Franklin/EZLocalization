using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using System.Linq;
using EZLocalization;

public class LocalizationWindow : EditorWindow
{
    string languageID = "en";
    Vector2 scrollPos;
    private readonly Color _lighterColor = Color.white * 0.2f;
    private readonly Color _darkerColor = Color.white * 0.05f;
    MultiColumnHeader columnHeader;
    private MultiColumnHeaderState multiColumnHeaderState;

    private MultiColumnHeaderState.Column[] columns;

    //options display variables
    public int toolbarInt = 0;
    List<string> toolbarStrings_List;
    List<LocalizationDatabase> locDBs;

    [MenuItem("Window/EZLoc/Localization")]
    public static void ShowWindow()
    {
        LocalizationWindow window = CreateInstance<LocalizationWindow>();
        window.titleContent = new GUIContent("EZ Localization");
        window.Show();
        //EditorWindow.GetWindow(typeof(LocalizationWindow), false, "EZ Localization");
    }

    void OnEnable()
    {
        Initialize();
    }

    void GetPossibleDBs()
    {
        toolbarStrings_List = new List<string>();
        locDBs = new List<LocalizationDatabase>();

        if (Selection.activeObject != null && Selection.activeObject.GetType() == typeof(LocalizationDatabase))
        {
            LocalizationDatabase locDB = Selection.activeObject as LocalizationDatabase;
            toolbarStrings_List.Add(Selection.activeObject.name);
            locDBs.Add(locDB);
        }
        
        LocalizationDatabase defaultDB = Resources.Load("Localization Database") as LocalizationDatabase;
        if (!locDBs.Contains(defaultDB))
        {
            toolbarStrings_List.Add("Localization Database");
            locDBs.Add(defaultDB);
        }
    }

    void Initialize()
    {
        LocalizationLanguages localizedLanguages = Resources.Load("Localization Languages") as LocalizationLanguages;

        if (localizedLanguages == null)
        {
            Debug.LogError("Missing a \"Localization Languages\" LocalizationLanguages object in a resources folder. Please create one with Assets/Create/Speech#/Loc Languages and move it to a Resources folder.");
        }

        List<MultiColumnHeaderState.Column> columns_Dynamic = new List<MultiColumnHeaderState.Column>();
        columns_Dynamic.Add(
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("Loc ID"),
                    width = 75,
                    minWidth = 75,
                    maxWidth = 150,
                    autoResize = false,
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false
                }
            );
        for (int i = 0; i < localizedLanguages.languages.Count; i++)
        {
            columns_Dynamic.Add(
                    new MultiColumnHeaderState.Column()
                    {
                        headerContent = new GUIContent(localizedLanguages.languages[i]),
                        width = 200,
                        minWidth = 200,
                        maxWidth = 500,
                        autoResize = false,
                        headerTextAlignment = TextAlignment.Center,
                        canSort = false
                    }
                );
        }

        columns = columns_Dynamic.ToArray();
        multiColumnHeaderState = new MultiColumnHeaderState(columns: columns);
        columnHeader = new MultiColumnHeader(multiColumnHeaderState);

        columnHeader.height = 25;
        columnHeader.ResizeToFit();
    }

    void OnGUI()
    {
        LocalizationLanguages localizedLanguages = Resources.Load("Localization Languages") as LocalizationLanguages;

        if(localizedLanguages == null)
        {
            EditorGUILayout.LabelField("Missing a \"Localization Languages\" LocalizationLanguages object in a resources folder.", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Please create one with Assets/Create/Speech#/Loc Languages and move it to a Resources folder.", EditorStyles.boldLabel);
            EditorGUILayout.Space();
        }

        GetPossibleDBs();

        EditorGUILayout.BeginVertical();
        toolbarInt = GUILayout.SelectionGrid(toolbarInt, toolbarStrings_List.ToArray(), 6);
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

        if (locDBs != null && locDBs.Count > toolbarInt && locDBs[toolbarInt] != null && localizedLanguages != null)
        {
            LocalizationDatabase localizedStrings = locDBs[toolbarInt];

            EditorGUILayout.LabelField($"{toolbarStrings_List[toolbarInt]} [Localization Database]", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            Initialize();
            localizedStrings.Initialize();
            localizedLanguages.Initialize();

            SerializedObject ll = new SerializedObject(localizedLanguages);

            ll.Update();
            SerializedProperty prop_ll = ll.GetIterator();
            if (prop_ll.NextVisible(true))
            {
                do
                {
                    //Draw movePoints property manually.
                    if (prop_ll.name != "m_Script")
                    {
                        EditorGUILayout.PropertyField(ll.FindProperty(prop_ll.name), true);
                    }
                }
                while (prop_ll.NextVisible(false));
            }
            ll.ApplyModifiedProperties();

            SerializedObject so = new SerializedObject(localizedStrings);
            so.Update();

            GUILayout.Space(15);

            languageID = EditorGUILayout.TextField("New Language ID", languageID);

            if (GUILayout.Button("Add Language ID"))
            {
                localizedLanguages.AddLanguageID(languageID);
                if (localizedLanguages.languages.Contains(languageID))
                {
                    localizedStrings.AddNewLanguageEntry();
                }
                Initialize();
            }

            if (GUILayout.Button("Add New Loc Entry"))
            {
                localizedStrings.AddNewLocEntry();
                Initialize();
            }

            GUILayout.Space(15);
            
            if (GUILayout.Button("Clear Database"))
            {
                localizedStrings.ClearDatabase();
                Initialize();
            }
            if (GUILayout.Button("Clear Languages"))
            {
                localizedLanguages.ClearLanguages();
                Initialize();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Localized Elements", EditorStyles.boldLabel, GUILayout.Width(200));
            EditorGUILayout.Space();

            // calculate the window visible rect
            GUILayout.Space(10);
            var windowVisibleRect = GUILayoutUtility.GetLastRect();
            windowVisibleRect.width = position.width;
            windowVisibleRect.height = position.height;

            // draw the column headers
            var headerRect = windowVisibleRect;
            headerRect.height = columnHeader.height;
            float xScroll = 0;
            columnHeader.OnGUI(headerRect, xScroll);
            GUILayout.Space(15);

            int[] visibleIndexes = multiColumnHeaderState.visibleColumns;

            float viewHeight = 0;

            scrollPos = GUILayout.BeginScrollView(scrollPos, true, true);
            {

            // draw the column's contents
            for (int i = 0; i < visibleIndexes.Length; i++)
            {
                for (int j = 0; j < localizedStrings.stringID_keys.Count; j++)
                {
                    Rect r = columnHeader.GetColumnRect(visibleIndexes[i]);
                    r.height = 55;
                    viewHeight = r.height * localizedStrings.stringID_keys.Count;
                    Rect rowRect = new Rect(r)
                    {
                        position = new Vector2(r.position.x, r.position.y + r.height * j),
                    };

                    if (j % 2 == 0)
                        EditorGUI.DrawRect(rowRect, _darkerColor);
                    else
                        EditorGUI.DrawRect(rowRect, _lighterColor);

                    if (visibleIndexes[i] == 0)
                    {
                        rowRect.height = 15;
                        rowRect.y += 3;
                        EditorGUI.TextField(rowRect, localizedStrings.stringID_keys[j].ToString());
                    }
                    else
                    {
                        int index = visibleIndexes[i] - 1;
                        rowRect.height = 30;
                        rowRect.width -= 5;
                        rowRect.y += 3;
                        localizedStrings.strings[j].languageStrings[index] = EditorGUI.TextArea(rowRect, localizedStrings.strings[j].languageStrings[index]);

                        SerializedProperty clipElement = null;
                        if (j < so.FindProperty("strings").arraySize)
                        {
                            var element = so.FindProperty("strings").GetArrayElementAtIndex(j);
                            if (index < element.FindPropertyRelative("languageClips").arraySize)
                            {
                                clipElement = element.FindPropertyRelative("languageClips").GetArrayElementAtIndex(index);
                            }
                        }
                        if (clipElement != null)
                        {
                            rowRect.height = 15;
                            rowRect.width -= 10;
                            //rowRect.x += 5;
                            rowRect.y += 35;
                            EditorGUI.PropertyField(rowRect, clipElement, GUIContent.none, true);
                        }
                    }
                }
            }
            float totalW = multiColumnHeaderState.columns.Sum(x => x.width);
            float totalH = columnHeader.GetColumnRect(visibleIndexes[0]).height;

            GUILayout.Label("", GUILayout.Width(totalW), GUILayout.Height(viewHeight));

            so.ApplyModifiedProperties();
            }
            GUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("Missing a Loc DB Asset.", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("There is no database named \"Localization Database\" found in a resources folder.", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Please create one from Assets/Create/Speech#/Loc DB and move it to a resources folder.", EditorStyles.boldLabel);
        }
    }
}
