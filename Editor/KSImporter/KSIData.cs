using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KSIData {

    /// <summary>
    /// The Base of the FilePath to Find the Exported Files
    /// </summary>
    public static string baseFilePath = "";

    /// <summary>
    /// Did the User Use Sub-Folders to Export Their Files
    /// </summary>
    public static bool useFolders = false;

    /// <summary>
    /// What is the Sub-Folder of the XML File?
    /// </summary>
    public static string xmlFolder = "";

    /// <summary>
    /// What is the Sub-Folder of the Exported Images Files?
    /// </summary>
    public static string exportFolder = "";

    /// <summary>
    /// The List of Keywords and their Handlers that Will Be Changed In Editor
    /// </summary>
    public static List<KeywordHandler> keywordList = new List<KeywordHandler>
    {

        new KeywordHandler("platform", ImportType.PLATFORM),
        new KeywordHandler("ground", ImportType.GROUND),
        new KeywordHandler("backgroundscenery", ImportType.SCENERY)

    };

    /// <summary>
    /// Where are We Saving the Final Scene?
    /// </summary>
    public static string sceneFilePath = "";

    /// <summary>
    /// Where are We Saving the Imported Images?
    /// </summary>
    public static string imageFilePath = "";

    /// <summary>
    /// Does the User Want to Use Custom Names When Importing Files?
    /// </summary>
    public static bool useCustomNames = false;

    /// <summary>
    /// The Custom Scene Name for the Newly Created Scene
    /// </summary>
    public static string customSceneName = "";

    /// <summary>
    /// The Custom Image Name for Imported Images
    /// </summary>
    public static string customImageName = "";

    /// <summary>
    /// The Index of KeywordList to Change
    /// </summary>
    public static int indexToChange = 0;

}
