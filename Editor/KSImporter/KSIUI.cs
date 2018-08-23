using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class KSIUI : EditorWindow {

    //Initialize Here
    private void OnEnable()
    {

        Debug.Log("Showing Updated Window!");        

    }

    [MenuItem("Tools/Importers/Krita Scene")]
    public static void ShowImportWindow()
    {

        KSIUI window = GetWindow<KSIUI>(true, "Krita Scene Importer", true);
        window.minSize = new Vector2(600, 475);

    }

    private void OnFocus()
    {

        Debug.Log("Window Focused. Loading Data.");
        LoadKSIData();

    }

    private void OnLostFocus()
    {

        Debug.Log("Window Lost Focused. Saving Data.");
        SaveKSIData();

    }

    private void OnDestroy()
    {

        Debug.Log("Window Destroyed. Saving Data.");
        SaveKSIData();

    }

    private void OnGUI()
    {

        GUILayout.Label("Settings for File Import", EditorStyles.boldLabel);
        KSIData.baseFilePath = EditorGUILayout.TextField("File Path:", KSIData.baseFilePath);
        KSIData.useFolders = EditorGUILayout.BeginToggleGroup("Used Sub-Folders", KSIData.useFolders);
        KSIData.exportFolder = EditorGUILayout.TextField("Export Folder:", KSIData.exportFolder);
        KSIData.xmlFolder = EditorGUILayout.TextField("XML Folder:", KSIData.xmlFolder);
        EditorGUILayout.EndToggleGroup();

        GUILayout.Label("Keywords and Handlers", EditorStyles.boldLabel);
        DisplayKeywordList();
        KSIData.indexToChange = EditorGUILayout.IntField("Index to Change:", KSIData.indexToChange);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Keyword At 'Index to Change'"))
        {

            KeywordHandler.AddKeyword(KSIData.indexToChange);

        }

        if (GUILayout.Button("Remove Keyword At 'Index to Change'"))
        {

            KeywordHandler.RemoveKeyword(KSIData.indexToChange);

        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Create New Keyword Handler", EditorStyles.boldLabel);
        KSITemplateCreator.handlerName = EditorGUILayout.TextField("New Handler Name:", KSITemplateCreator.handlerName);
        if(GUILayout.Button("Create Handler"))
        {

            KSITemplateCreator.AddHandler();

        }

        GUILayout.Label("Settings For Scene Saving", EditorStyles.boldLabel);
        KSIData.sceneFilePath = EditorGUILayout.TextField("Scene File Path:", KSIData.sceneFilePath);
        KSIData.imageFilePath = EditorGUILayout.TextField("Image File Path:", KSIData.imageFilePath);

        KSIData.useCustomNames = EditorGUILayout.BeginToggleGroup("Use Custom Names", KSIData.useCustomNames);
        KSIData.customSceneName = EditorGUILayout.TextField("Custom Scene Name:", KSIData.customSceneName);
        KSIData.customImageName = EditorGUILayout.TextField("Custom Image Set Name:", KSIData.customImageName);
        EditorGUILayout.EndToggleGroup();


        if (GUILayout.Button("Export Scene"))
        {

            KSImporter.StartImport();

        }

    }

    /// <summary>
    /// Saves All of the Data Set in the UI
    /// </summary>
    void SaveKSIData()
    {

        //Save Data Up To Keyword Handlers
        EditorPrefs.SetString("BasePath", KSIData.baseFilePath);
        EditorPrefs.SetBool("UseFolders", KSIData.useFolders);
        EditorPrefs.SetString("ExportFolder", KSIData.exportFolder);
        EditorPrefs.SetString("XMLFolder", KSIData.xmlFolder);

        int i = 0;

        //Clear Previous Keyword Handler Keys
        while(EditorPrefs.GetString("Key" + i) != "")
        {

            EditorPrefs.DeleteKey("Key" + i);
            EditorPrefs.DeleteKey("Handler" + i);
            i++;

        }
        
        //Save Keyword Handler Keys
        for(i = 0; i < KSIData.keywordList.Count; i++)
        {

            EditorPrefs.SetString("Key" + i, KSIData.keywordList[i].keyword);
            EditorPrefs.SetString("Handler" + i, KeywordHandler.TypeToStr(KSIData.keywordList[i].handler));

        }

        //Save Data After Keyword Handlers
        EditorPrefs.SetInt("Index", KSIData.indexToChange);
        EditorPrefs.SetString("NewHandler", KSITemplateCreator.handlerName);
        EditorPrefs.SetString("ScenePath", KSIData.sceneFilePath);
        EditorPrefs.SetString("ImagePath", KSIData.imageFilePath);

    }

    /// <summary>
    /// Loads All of the Data Saved From the UI
    /// </summary>
    void LoadKSIData()
    {

        KSIData.baseFilePath = EditorPrefs.GetString("BasePath");
        KSIData.useFolders = EditorPrefs.GetBool("UseFolders");
        KSIData.exportFolder = EditorPrefs.GetString("ExportFolder");
        KSIData.xmlFolder = EditorPrefs.GetString("XMLFolder");

        for(int i = 0; i < KSIData.keywordList.Count; i++)
        {

            if (KSIData.keywordList[i] == null)
            {

                KSIData.keywordList.Add(
                    new KeywordHandler(
                        EditorPrefs.GetString("Key" + i),
                        KeywordHandler.StrToType(EditorPrefs.GetString("Handler" + i))));

            }
            else
            {
                KSIData.keywordList[i].keyword = EditorPrefs.GetString("Key" + i);
                KSIData.keywordList[i].handler = KeywordHandler.StrToType(EditorPrefs.GetString("Handler" + i));

            }

        }

        KSIData.keywordList.TrimExcess();

        KSIData.indexToChange = EditorPrefs.GetInt("Index");
        KSITemplateCreator.handlerName = EditorPrefs.GetString("NewHandler");
        KSIData.sceneFilePath = EditorPrefs.GetString("ScenePath");
        KSIData.imageFilePath = EditorPrefs.GetString("ImagePath");

    }

    public static void DisplayKeywordList()
    {

        for (int i = 0; i < KSIData.keywordList.Count; i++)
        {

            EditorGUILayout.BeginHorizontal();
            KSIData.keywordList[i].keyword = EditorGUILayout.TextField("Keyword " + (i + 1), KSIData.keywordList[i].keyword);
            KSIData.keywordList[i].handler = (ImportHandler)EditorGUILayout.EnumPopup("Keyword Handler:", KSIData.keywordList[i].handler);
            EditorGUILayout.EndHorizontal();

        }

    }

    public static bool DisplayDialog(string title, string message, string okayButtonString)
    {

        return EditorUtility.DisplayDialog(title, message, okayButtonString);

    }

    public static bool DisplayDialog(string title, string message, string okayButtonString, string cancelButtonString)
    {

        return EditorUtility.DisplayDialog(title, message, okayButtonString, cancelButtonString);

    }

}
