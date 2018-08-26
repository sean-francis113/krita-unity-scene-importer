using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class KSIUI : EditorWindow {

    bool loadData = true;

    //Initialize Here
    private void OnEnable()
    {

        Debug.Log("Showing Updated Window!");        

    }

    [MenuItem("Tools/Importers/Krita Scene")]
    public static void ShowImportWindow()
    {

        KSIUI window = GetWindow<KSIUI>(true, "Krita Scene Importer", true);
        window.minSize = new Vector2(600, 600);

    }

    private void OnFocus()
    {

        Debug.Log("Window Focused.");

        if (loadData)
        {

            Debug.Log("Loading Data.");
            LoadKSIData();

        }
        else
        {

            Debug.Log("Not Loading Data.");
            loadData = true;

        }

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

        EditorGUILayout.Space();

        if(GUILayout.Button("Open File Browser"))
        {

            //Temporarily Prevent Importer From Loading Data When Refocusing on Window
            //Otherwise the Data We Set Will Disappear
            loadData = false;

            //Set UI Data When User Chooses a XML File
            SetFilePathData(EditorUtility.OpenFilePanel("Select XML File To Import With", KSIData.filepathLastOpened ?? Application.dataPath, ""));

        }

        EditorGUILayout.Space();

        KSIData.xmlSceneName = EditorGUILayout.TextField("Scene Name: ", KSIData.xmlSceneName);
        KSIData.baseFilePath = EditorGUILayout.TextField("File Path:", KSIData.baseFilePath);
        KSIData.xmlFileName = EditorGUILayout.TextField("XML File Name: ", KSIData.xmlFileName);

        EditorGUILayout.Space();

        KSIData.useFolders = EditorGUILayout.BeginToggleGroup("Used Sub-Folders", KSIData.useFolders);
        KSIData.exportFolder = EditorGUILayout.TextField("Export Folder:", KSIData.exportFolder);
        KSIData.xmlFolder = EditorGUILayout.TextField("XML Folder:", KSIData.xmlFolder);
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.Space();

        GUILayout.Label("Keywords and Handlers", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        DisplayKeywordList();
        EditorGUILayout.Space();
        KSIData.indexToChange = EditorGUILayout.IntField("Index to Change:", KSIData.indexToChange);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Keyword At 'Index to Change'"))
        {

            KSIKeywordHandler.AddKeyword(KSIData.indexToChange);

        }

        if (GUILayout.Button("Remove Keyword At 'Index to Change'"))
        {

            KSIKeywordHandler.RemoveKeyword(KSIData.indexToChange);

        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.Label("Create New Keyword Handler", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        KSITemplateCreator.handlerName = EditorGUILayout.TextField("New Handler Name:", KSITemplateCreator.handlerName);

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Handler"))
        {

            KSITemplateCreator.AddHandler();

        }

        EditorGUILayout.Space();

        GUILayout.Label("Settings For Scene Saving", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        KSIData.sceneFilePath = EditorGUILayout.TextField("Scene File Path:", KSIData.sceneFilePath);
        KSIData.imageFilePath = EditorGUILayout.TextField("Image File Path:", KSIData.imageFilePath);

        EditorGUILayout.Space();

        KSIData.useCustomNames = EditorGUILayout.BeginToggleGroup("Use Custom Names", KSIData.useCustomNames);
        KSIData.customSceneName = EditorGUILayout.TextField("Custom Scene Name:", KSIData.customSceneName);
        KSIData.customImageName = EditorGUILayout.TextField("Custom Image Set Name:", KSIData.customImageName);
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.Space();

        if (GUILayout.Button("Import Scene"))
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
        EditorPrefs.SetString("XMLSceneName", KSIData.xmlSceneName);
        EditorPrefs.SetString("BasePath", KSIData.baseFilePath);
        EditorPrefs.SetString("XMLFileName", KSIData.xmlFileName);
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
            EditorPrefs.SetString("Handler" + i, KSIKeywordHandler.TypeToStr(KSIData.keywordList[i].handler));

        }

        //Save Data After Keyword Handlers
        EditorPrefs.SetInt("Index", KSIData.indexToChange);
        EditorPrefs.SetString("NewHandler", KSITemplateCreator.handlerName);
        EditorPrefs.SetString("ScenePath", KSIData.sceneFilePath);
        EditorPrefs.SetString("ImagePath", KSIData.imageFilePath);
        EditorPrefs.SetBool("UseCustomNames", KSIData.useCustomNames);
        EditorPrefs.SetString("CustomScene", KSIData.customSceneName);
        EditorPrefs.SetString("CustomImage", KSIData.customImageName);
        EditorPrefs.SetString("FileLastOpened", KSIData.filepathLastOpened);

    }

    /// <summary>
    /// Loads All of the Data Saved From the UI
    /// </summary>
    void LoadKSIData()
    {

        KSIData.xmlSceneName = EditorPrefs.GetString("XMLSceneName");
        KSIData.baseFilePath = EditorPrefs.GetString("BasePath");
        KSIData.xmlFileName = EditorPrefs.GetString("XMLFileName");
        KSIData.useFolders = EditorPrefs.GetBool("UseFolders");
        KSIData.exportFolder = EditorPrefs.GetString("ExportFolder");
        KSIData.xmlFolder = EditorPrefs.GetString("XMLFolder");

        for(int i = 0; i < KSIData.keywordList.Count; i++)
        {

            if (KSIData.keywordList[i] == null)
            {

                KSIData.keywordList.Add(
                    new KSIKeywordHandler(
                        EditorPrefs.GetString("Key" + i),
                        KSIKeywordHandler.StrToType(EditorPrefs.GetString("Handler" + i))));

            }
            else
            {
                KSIData.keywordList[i].keyword = EditorPrefs.GetString("Key" + i);
                KSIData.keywordList[i].handler = KSIKeywordHandler.StrToType(EditorPrefs.GetString("Handler" + i));

            }

        }

        KSIData.keywordList.TrimExcess();

        KSIData.indexToChange = EditorPrefs.GetInt("Index");
        KSITemplateCreator.handlerName = EditorPrefs.GetString("NewHandler");
        KSIData.sceneFilePath = EditorPrefs.GetString("ScenePath");
        KSIData.imageFilePath = EditorPrefs.GetString("ImagePath");
        KSIData.useCustomNames = EditorPrefs.GetBool("UseCustomNames");
        KSIData.customSceneName = EditorPrefs.GetString("CustomScene");
        KSIData.customImageName = EditorPrefs.GetString("CustomImage");
        KSIData.filepathLastOpened = EditorPrefs.GetString("FileLastOpened");

    }

    /// <summary>
    /// Repaints the UI With the New List of Keywords
    /// </summary>
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

    /// <summary>
    /// Sets the UI Fields Based On the Provided File Path
    /// </summary>
    /// <param name="filePath">The File Path of the XML File</param>
    public static void SetFilePathData(string filePath)
    {

        //Set Last Opened Filepath
        KSIData.filepathLastOpened = filePath;

        //If The User Clicked Out, Closed, or Canceled the File Panel
        if(filePath == "")
        {

            return;

        }

        //If User Did Not Choose an XML File
        if(!filePath.EndsWith(".xml"))
        {

            DisplayDialog("Error: XML File Not Chosen", 
                "You Did Not Choose a XML File!\n\n Cannot Continue With Setting UI Data!", 
                "Okay");

        }

        //Grab and Set the Scene Name
        KSIData.xmlSceneName = GetSceneName(filePath);

        //Grab and Set the XML File Name
        KSIData.xmlFileName = GetXMLName(filePath);

        //Try to Determine if User Is Using Sub-Folders
        CheckSubFolderUse(filePath);

    }

    /// <summary>
    /// Pulls Apart the File Provided To Find the Unity Scene Name
    /// </summary>
    /// <param name="filePath">The File Path of the XML File</param>
    /// <returns></returns>
    public static string GetSceneName(string filePath)
    {

        //The Characters We Are Going to Split the Name Line By
        char[] splitChars = { ' ', '\n', '\t', '<', '>' };

        //Grab All of the Lines from the XML File
        string[] lines = System.IO.File.ReadAllLines(filePath);

        //Find the Line That Has the Scene Name Declared
        string sceneLine = lines[KSIData.UNITY_SCENE_DECLARATION_LINE - 1];

        //Split the Scene Line Further To Grab Just the Name
        string[] slArray = sceneLine.Split(splitChars);

        string sceneName = "";

        //The Name Should Always Be Set in the Third Index
        //The First Index is the Tab
        //The Second is the < Sign
        sceneName = slArray[2];

        //Return Calculated Scene Name
        return sceneName;        

    }

    /// <summary>
    /// Looks at filePath and Returns the Name of the XML File
    /// </summary>
    /// <param name="filePath">The File Path of the XML File</param>
    /// <returns>The Name of the XML File</returns>
    public static string GetXMLName(string filePath)
    {

        //The Separators of the Directories
        char[] DIRECTORY_SEPARATORS = { '\\', '/' };

        //Get an Array of Directory Names
        string[] directoryNames = filePath.Split(DIRECTORY_SEPARATORS);

        //Return the Last Index of the Directory Names
        return directoryNames[directoryNames.Length - 1];

    }

    /// <summary>
    /// Try to Determine if User is Using Sub-Folders and Set UI Accordingly
    /// </summary>
    /// <param name="filePath">The File Path of the XML File</param>
    public static void CheckSubFolderUse(string filePath)
    {

        //The Separators of the Directories
        char[] DIRECTORY_SEPARATORS = { '\\', '/' };

        //The Labels for the XML Folder
        //User is Free to Add/Remove As Wished
        string[] XML_FOLDER_NAMES = { "xml" };

        //The Labels for the Exported Image Folder
        //User is Free to Add/Remove As Wished
        string[] IMAGE_FOLDER_NAMES = { "export" };

        //Initialize the Base File Path (Path Without File or Parent Directory) for Use Later
        string baseFilePath = "";

        //Get an Array of Directory Names
        string[] directoryNames = filePath.Split(DIRECTORY_SEPARATORS);

        //Grab the Parent Directory of the XML File
        string parentDirectory = directoryNames[directoryNames.Length - 2];

        //Construct the BaseFilePath
        for (int i = 0; i < (directoryNames.Length - 2); i++)
        {

            baseFilePath += directoryNames[i] + "\\";

        }

        //Look Through the XML_FOLDER_NAMES Array
        for (int i = 0; i < XML_FOLDER_NAMES.Length; i++)
        {

            //If We Found a Valid Sub-Folder
            if(parentDirectory == XML_FOLDER_NAMES[i])
            {

                KSIData.useFolders = true;
                KSIData.xmlFolder = XML_FOLDER_NAMES[i];
                break;

            }

            KSIData.useFolders = false;

        }
        
        //If We Determined the User Used Sub-Folders
        if(KSIData.useFolders)
        {           

            //Look Through the IMAGE_FOLDER_NAMES Array
            for(int i = 0; i < IMAGE_FOLDER_NAMES.Length; i++)
            {

                Debug.Log("Checking File Path: " + (baseFilePath + IMAGE_FOLDER_NAMES[i] + '\\'));

                //Does a Folder With a IMAGE_FOLDER_NAMES Index Exist?
                if(Directory.Exists(baseFilePath + IMAGE_FOLDER_NAMES[i] + '\\'))
                {

                    Debug.Log("Export Folder Exists!");
                    KSIData.exportFolder = IMAGE_FOLDER_NAMES[i];
                    break;

                }

                KSIData.exportFolder = "";

            }

            //Set the Base File Path Without Either XML or Image Export Folders
            KSIData.baseFilePath = baseFilePath;

        }
        else
        {

            //Set the Base File Path as It Would Be Without Sub-Folders
            KSIData.baseFilePath = baseFilePath + directoryNames[directoryNames.Length - 2] + "\\";

        }

    }

    /// <summary>
    /// Displays a Dialog Window
    /// </summary>
    /// <param name="title">The Title of the Window</param>
    /// <param name="message">The Message Displayed Within the Window</param>
    /// <param name="okayButtonString">The Label Shown on the 'Confirm' or 'Okay' Button</param>
    /// <returns>True When the User Presses the Okay Button; False Otherwise</returns>
    public static bool DisplayDialog(string title, string message, string okayButtonString)
    {

        return EditorUtility.DisplayDialog(title, message, okayButtonString);

    }

    /// <summary>
    /// Displays a Dialog Window
    /// </summary>
    /// <param name="title">The Title of the Window</param>
    /// <param name="message">The Message Displayed Within the Window</param>
    /// <param name="okayButtonString">The Label Shown on the 'Confirm' or 'Okay' Button</param>
    /// <param name="cancelButtonString">The Label Shown on the 'Cancel' Button</param>
    /// <returns>True When the User Presses the Okay Button; False Otherwise</returns>
    public static bool DisplayDialog(string title, string message, string okayButtonString, string cancelButtonString)
    {

        return EditorUtility.DisplayDialog(title, message, okayButtonString, cancelButtonString);

    }

}
