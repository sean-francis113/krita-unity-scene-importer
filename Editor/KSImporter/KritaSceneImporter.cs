using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KSImporter
{

    public class KritaSceneImporter : EditorWindow, ISerializationCallbackReceiver
    {

        /// <summary>
        /// Enumerators, Structs and Classes to Hold/Manipulate Necessary Data
        /// </summary>
        #region Data Handlers

        /// <summary>
        /// Enumerator Holding All Keyword Handler Types 
        /// </summary>
        public enum ImportType
        {

            NONE,
            PLATFORM,
            GROUND,
            SCENERY

        };

        public class KeywordHandler
        {

            public string keyword;
            public ImportType type;

            /// <summary>
            /// Initialize a KeywordHandler Object
            /// </summary>
            public KeywordHandler()
            {

                keyword = "";
                type = ImportType.NONE;

            }

            /// <summary>
            /// Initialize a KeywordHandler Object
            /// </summary>
            /// <param name="word">The Keyword</param>
            /// <param name="t">The Handler Type</param>
            public KeywordHandler(string word, ImportType t)
            {

                keyword = word;
                type = t;

            }

        }

        [Serializable]
        public class KSImporterState
        {

            public string baseFilePath;
            public bool useFolders;
            public string xmlFolder;
            public string exportFolder;
            public List<SerializedKeywordHandler> serializedKeywords = new List<SerializedKeywordHandler>();
            public string sceneFilePath;
            public string imageFilePath;
            public bool useCustomNames;
            public string customSceneName;
            public string customImageName;

        }

        [Serializable]
        public class SerializedKeywordHandler
        {

            public string keyword;
            public string typeStr;

        }

        #endregion

        /// <summary>
        /// Where the Variables Are Declared
        /// </summary>
        #region Variables

        public KSImporterState importState = new KSImporterState();

        /// <summary>
        /// The Base of the FilePath to Find the Exported Files
        /// </summary>
        public string baseFilePath = "";

        /// <summary>
        /// Did the User Use Sub-Folders to Export Their Files
        /// </summary>
        public bool useFolders = false;

        /// <summary>
        /// What is the Sub-Folder of the XML File?
        /// </summary>
        public string xmlFolder = "";

        /// <summary>
        /// What is the Sub-Folder of the Exported Images Files?
        /// </summary>
        public string exportFolder = "";

        /// <summary>
        /// The List of Keywords and their Handlers that Will Be Changed In Editor
        /// </summary>
        public List<KeywordHandler> keywordList = new List<KeywordHandler>
        {

            new KeywordHandler("platform", ImportType.PLATFORM),
            new KeywordHandler("ground", ImportType.GROUND),
            new KeywordHandler("backgroundscenery", ImportType.SCENERY)            

        };

        /// <summary>
        /// Where are We Saving the Final Scene?
        /// </summary>
        public string sceneFilePath = "";

        /// <summary>
        /// Where are We Saving the Imported Images?
        /// </summary>
        public string imageFilePath = "";

        /// <summary>
        /// Does the User Want to Use Custom Names When Importing Files?
        /// </summary>
        public bool useCustomNames = false;

        /// <summary>
        /// The Custom Scene Name for the Newly Created Scene
        /// </summary>
        public string customSceneName = "";

        /// <summary>
        /// The Custom Image Name for Imported Images
        /// </summary>
        public string customImageName = "";

        public int indexToChange = 0;

        #endregion

        /// <summary>
        /// Where the Drawing of the Editor Window Happens
        /// </summary>
        #region Editor Window Setup

        //Initialize Here
        private void OnEnable()
        {

            Debug.Log("Showing Updated Window!");

        }

        [MenuItem("Tools/Importers/Krita Scene")]
        public static void ShowImportWindow()
        {

            GetWindow<KritaSceneImporter>(true, "Krita Scene Importer", true);

        }

        public void OnBeforeSerialize()
        {

            Debug.Log("In OnBeforeSerialize()");

            if(importState.serializedKeywords == null)
            {

                importState.serializedKeywords = new List<SerializedKeywordHandler>();

            }

            SaveKeywordList();

        }

        public void OnAfterDeserialize()
        {

            Debug.Log("In OnAfterDeserialize()");

            if(keywordList == null)
            {

                keywordList = new List<KeywordHandler>();

            }

            LoadKeywordList();

        }

        private void OnGUI()
        {

            GUILayout.Label("Settings for File Import", EditorStyles.boldLabel);
            baseFilePath = EditorGUILayout.TextField("File Path:", baseFilePath);
            useFolders = EditorGUILayout.BeginToggleGroup("Used Sub-Folders", useFolders);
            exportFolder = EditorGUILayout.TextField("Export Folder:", exportFolder);
            xmlFolder = EditorGUILayout.TextField("XML Folder:", xmlFolder);
            EditorGUILayout.EndToggleGroup();

            GUILayout.Label("Keywords and Handlers", EditorStyles.boldLabel);
            DisplayKeywordList();
            indexToChange = EditorGUILayout.IntField("Index to Change:", indexToChange);
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Add Keyword At 'Index to Change'"))
            {

                AddKeyword(indexToChange);

            }

            if(GUILayout.Button("Remove Keyword At 'Index to Change'"))
            {

                RemoveKeyword(indexToChange);

            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Settings For Scene Saving", EditorStyles.boldLabel);
            sceneFilePath = EditorGUILayout.TextField("Scene File Path:", sceneFilePath);
            imageFilePath = EditorGUILayout.TextField("Image File Path:", imageFilePath);

            useCustomNames = EditorGUILayout.BeginToggleGroup("Use Custom Names", useCustomNames);
            customSceneName = EditorGUILayout.TextField("Custom Scene Name:", customSceneName);
            customImageName = EditorGUILayout.TextField("Custom Image Set Name:", customImageName);
            EditorGUILayout.EndToggleGroup();


            if (GUILayout.Button("Export Scene"))
            {

                StartExport();

            }

        }

        void DisplayKeywordList()
        {

            for (int i = 0; i < keywordList.Count; i++)
            {

                EditorGUILayout.BeginHorizontal();
                keywordList[i].keyword = EditorGUILayout.TextField("Keyword " + (i + 1), keywordList[i].keyword);
                keywordList[i].type = (ImportType)EditorGUILayout.EnumPopup("Keyword Handler:", keywordList[i].type);
                EditorGUILayout.EndHorizontal();

            }

        }

        void AddKeyword(int indexToChange)
        {

            int i = 0;

            if (keywordList.Count == 0)
            {

                //New Null Reference is Added
                keywordList.Add(new KeywordHandler());

            }
            else if((indexToChange < 0 &&
                    EditorUtility.DisplayDialog("Error: Index Out of Range (Small)", "Provided Index is Too Small! Would You Like to Add a Keyword at the First Index?", "Add Keyword", "Do Not Add Keyword")))
            {

                //New Null Reference is Added At End of List
                keywordList.Add(new KeywordHandler());

                //Get the Last Index of the List
                i = keywordList.Count - 1;

                //While We Can Still Move Up the List
                while (i > 0)
                {

                    //Temporarily Grab Object at i
                    KeywordHandler temp = keywordList[i];
                    //Temporarily Grab Object Above i
                    KeywordHandler aboveTemp = keywordList[i - 1];

                    //Move the Object Above i Down
                    keywordList[i] = aboveTemp;
                    //Move the Object at i Up
                    keywordList[i - 1] = temp;

                    i--;

                }

                DisplayKeywordList();

            }
            else if ((indexToChange > keywordList.Count - 1 && 
                    EditorUtility.DisplayDialog("Error: Index Out of Range (Large)", "Provided Index is Too Large! Would You Like to Add a Keyword at the Last Index?", "Add Keyword", "Do Not Add Keyword")) ||
                (indexToChange < keywordList.Count - 1))
            {

                //New Null Reference is Added At End of List
                keywordList.Add(new KeywordHandler());

                //Get the Last Index of the List
                i = keywordList.Count - 1;

                //While We Can Still Move Up the List
                while (i > indexToChange)
                {

                    //Temporarily Grab Object at i
                    KeywordHandler temp = keywordList[i];
                    //Temporarily Grab Object Above i
                    KeywordHandler aboveTemp = keywordList[i - 1];

                    //Move the Object Above i Down
                    keywordList[i] = aboveTemp;
                    //Move the Object at i Up
                    keywordList[i - 1] = temp;

                    i--;

                }

                DisplayKeywordList();

            }            

        }

        void RemoveKeyword(int indexToChange)
        {

            if (keywordList.Count == 0)
            {

                EditorUtility.DisplayDialog("Error: No List", "There Are No Entries In the List! Cannot Remove Anything!", "Okay");
                return;

            }

            if ((indexToChange > keywordList.Count - 1 &&
                    EditorUtility.DisplayDialog("Error: Index Out of Range (Large)", "Provided Index is Too Large! Would You Like to Remove the Keyword at the Last Index?", "Remove Keyword", "Do Not Remove Keyword")))
            {

                int i = keywordList.Count - 1;
                
                keywordList.RemoveAt(i);
                DisplayKeywordList();

            }
            else if((indexToChange < 0 &&
                    EditorUtility.DisplayDialog("Error: Index Out of Range (Small)", "Provided Index is Too Small! Would You Like to Remove the Keyword at the First Index?", "Remove Keyword", "Do Not Remove Keyword")))
            {

                int i = 0;

                keywordList.RemoveAt(i);
                DisplayKeywordList();

            }            
            else
            {

                keywordList.RemoveAt(indexToChange);
                DisplayKeywordList();

            }

        }

        void SaveKeywordList()
        {

            Debug.Log("Saving List!");

            importState.serializedKeywords.Clear();

            foreach (var word in keywordList)
            {

                importState.serializedKeywords.Add(new SerializedKeywordHandler());
                importState.serializedKeywords[importState.serializedKeywords.Count - 1].keyword = word.keyword;
                importState.serializedKeywords[importState.serializedKeywords.Count - 1].typeStr = TypeToStr(word.type);


            }

        }

        void LoadKeywordList()
        {

            Debug.Log("Loading List!");

            keywordList.Clear();

            if (importState.serializedKeywords.Count != 0)
            {

                for (int i = 0; i < importState.serializedKeywords.Count; i++)
                {

                    keywordList.Add(new KeywordHandler(
                        importState.serializedKeywords[i].keyword,
                        StrToType(importState.serializedKeywords[i].typeStr))
                        );

                }

            }

        }

        string TypeToStr(ImportType type)
        {

            return type.ToString();

        }

        ImportType StrToType(string str)
        {

            switch(str.ToUpper())
            {

                case "NONE":
                    return ImportType.NONE;
                case "PLATFORM":
                    return ImportType.PLATFORM;
                case "GROUND":
                    return ImportType.GROUND;
                case "SCENERY":
                    return ImportType.SCENERY;
                default:
                    return ImportType.NONE;

            }

        }

        #endregion

        /// <summary>
        /// Where the Real Magic of the Importer Happens
        /// </summary>
        #region Krita Scene Importer Functionality

        void StartExport()
        {



        }

        void CreatePlatform()
        {



        }

        void CreateGround()
        {



        }

        void CreateScenery()
        {



        }

        #endregion

    }

}