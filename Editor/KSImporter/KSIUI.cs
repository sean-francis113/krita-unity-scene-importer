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
        


    }

    private void OnLostFocus()
    {
        


    }

    private void OnDestroy()
    {
        


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

    void SaveKSIData()
    {

        EditorPrefs.SetInt("WordCount", KSIData.keywordList.Count);


    }

    void LoadKSIData()
    {



    }

    public static void DisplayKeywordList()
    {

        for (int i = 0; i < KSIData.keywordList.Count; i++)
        {

            EditorGUILayout.BeginHorizontal();
            KSIData.keywordList[i].keyword = EditorGUILayout.TextField("Keyword " + (i + 1), KSIData.keywordList[i].keyword);
            KSIData.keywordList[i].type = (ImportHandler)EditorGUILayout.EnumPopup("Keyword Handler:", KSIData.keywordList[i].type);
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
