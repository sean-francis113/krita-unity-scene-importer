using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using System.IO;
using System.Xml.Linq;

public class KSImporter : MonoBehaviour {

    /// <summary>
    /// Holds the Data That Will Be Loaded From the XML File. 
    /// It Will Then Be Altered During Runtime as We Move Files Around
    /// </summary>
    public class ImageData
    {

        public string Name { get; set; }
        public string NameInScene { get; set; }
        public string Filename { get; set; }
        public string FileExtension { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public Vector2 Position { get; set; }

        public ImageData()
        {

            Name = "";
            NameInScene = "";
            Filename = "";
            FileExtension = "";
            Path = "";
            Type = "";
            Position = new Vector2(0, 0);

        }

        public ImageData(ImageData imageData)
        {

            Name = imageData.Name;
            NameInScene = imageData.NameInScene;
            Filename = imageData.Filename;
            FileExtension = imageData.FileExtension;
            Path = imageData.Path;
            Type = imageData.Type;
            Position = imageData.Position;

        }

        public ImageData(string name, string nameInScene, string filename, string fileExtension, string path, string type, Vector2 position)
        {

            Name = name;
            NameInScene = nameInScene;
            Filename = filename;
            FileExtension = fileExtension;
            Path = path;
            Type = type;
            Position = position;

        }

        public override string ToString()
        {

            return string.Format("[Name: {0}; NameInScene: {1}; Filename: {2}; FileExtension: {3}; Path: {4}; Type: {5}; Position: ({6},{7})]", 
                Name, NameInScene, Filename, FileExtension, Path, Type, Position.x, Position.y);

        }

    }

    /// <summary>
    /// Data For the Texture Once it is Created.
    /// </summary>
    public class TextureData
    {

        public Texture2D texture { get; set; }
        public ImageData data { get; set; }

    }

    /// <summary>
    /// The Document Created and Loaded When We Start Loading From XML File
    /// </summary>
    static XDocument xDoc;

    /// <summary>
    /// The List of Elements That We Will Load From xDoc
    /// </summary>
    static IEnumerable<XElement> elements;

    /// <summary>
    /// The List of Imported Data Which We Will Grab From elements
    /// </summary>
    static List<ImageData> imports = new List<ImageData>();

    /// <summary>
    /// The Scene That We Will Create and Load Images Into
    /// </summary>
    static Scene importScene;

    /// <summary>
    /// The Images We Will Be Loading From XML Data
    /// </summary>
    static List<TextureData> textures = new List<TextureData>();

    /// <summary>
    /// Begins the Import Process
    /// </summary>
    public static void StartImport()
    {

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false)
        {
            return;
        }

        if (KSIDataValidator.CheckData())
        {

            Debug.Log("All Data is Valid. Can Begin Import.");
            Import();

        }
        else
        {

            KSIUI.DisplayDialog("Error: Invalid Data", "Could Not Start Import. Invalid Data Was Entered.", "Okay");

        }

    }

    /// <summary>
    /// Import Everything Based on Checked Data
    /// </summary>
    public static void Import()
    {

        Debug.Log("Loading XML Data");

        string XMLPath = GetXMLPath();

        //Load Data from XML Data
        LoadXMLData(XMLPath);

        for(int i = 0; i < imports.Count; i++)
        {

            Debug.Log("ImageImportData " + i + ": " + imports[i].ToString());

        }

        string scenePath = CreateNewScene();

        //If the User Canceled Creating a Scene
        if(scenePath == "")
        {

            return;

        }

        Scene importScene = SceneManager.GetSceneByPath(scenePath);
        
        for(int i = 0; i < imports.Count; i++)
        {

            SaveImage(imports[i]);

            /*
            bool success = LoadTexture(imports[i]);

            if(!success)
            {

                return;

            }
            */
            //success = ImportTexture(textures[i]);

        }
        
    }    

    /// <summary>
    /// Gets the File Path of the XML File Based on User Settings
    /// </summary>
    /// <returns>The String of the XML File Path</returns>
    public static string GetXMLPath()
    {
        
        //If Use Folders is True, Return Combined Path. Else Return Base File Path Plus XMLFileName
        return (KSIData.useFolders ? (KSIData.baseFilePath + KSIData.xmlFolder) : (KSIData.baseFilePath)) + KSIData.xmlFileName;

    }

    /// <summary>
    /// Loads the XML Data from the XML File Specified
    /// </summary>
    /// <param name="xmlPath">The File Path of the XML File to Load From</param>
    public static void LoadXMLData(string xmlPath)
    {

        //Get XML Document
        xDoc = XDocument.Load(xmlPath);

        //Get the Layer Elements in Document
        elements = xDoc.Descendants("Layer");

        //Fill List With Loaded Data
        foreach (var element in elements)
        {

            imports.Add(new ImageData(
                element.Element("Name").Value,
                "",
                element.Element("Filename").Value,
                "",
                element.Element("Path").Value,
                element.Element("Type").Value,
                new Vector2(
                    float.Parse(element.Element("Position").Element("X").Value),
                    float.Parse(element.Element("Position").Element("Y").Value)
                    )
                )
            );

        }

    }

    /// <summary>
    /// Create a New Unity Scene Based on User Settings
    /// </summary>
    /// <returns>The File Path to the New Scene</returns>
    public static string CreateNewScene()
    {

        //Create the Scene Path Scene. This is Where We Will Save Create and Save the Scene
        string scenePath = KSIData.sceneFilePath +
            (KSIData.useCustomNames ? KSIData.customSceneName : KSIData.xmlSceneName);

        if(File.Exists(scenePath) == true)
        {

            if (KSIUI.DisplayDialog("Error: Scene Exists",
                "There is Already a Scene Named " +
                (KSIData.useCustomNames ? KSIData.customSceneName : KSIData.xmlSceneName) +
                "at " + KSIData.sceneFilePath + ". Do You Wish To Replace It?", 
                "Yes, Replace It", "No, Return to Importer"))
            {

                File.Delete(scenePath);
                AssetDatabase.Refresh();

            }
            else
            {

                return "";

            }

        }

        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        EditorSceneManager.SaveScene(newScene, KSIData.sceneFilePath + "\\" + (KSIData.useCustomNames ? KSIData.customSceneName : KSIData.xmlSceneName) + ".unity");

        return newScene.path;

    }

    /// <summary>
    /// Save the Image Into the Project Files
    /// </summary>
    /// <param name="imageData">The Data of the Image to Save</param>
    public static void SaveImage(ImageData imageData)
    {

        //What Image Extensions Does This Importer Support?
        string[] supportedExtensions = { ".png", ".jpg", ".jpeg" };

        //Make Sure We Have the Right Image Name
        string imageName = (KSIData.useCustomNames ? RenameImage(imageData) : imageData.Filename);

        //Initialize the Image Extension
        string imageExtension = "";

        //Look Through the Array of Supported Extensions
        foreach (var extension in supportedExtensions)
        {

            //If We Found the File Has the Currently Itterating Extension
            if (File.Exists(imageData.Path + imageData.Filename + extension))
            {

                imageExtension = extension;
                break;

            }

        }

        //Finally, Make Sure the Destination Path Exists
        //If Not, Create It
        if(!Directory.Exists(KSIData.imageFilePath))
        {

            Directory.CreateDirectory(KSIData.imageFilePath);

        }

        //Now, We Can Save the Image
        File.Copy(imageData.Path + imageData.Filename + imageExtension, KSIData.imageFilePath + (KSIData.imageFilePath[KSIData.imageFilePath.Length - 1] == '/' ||
            KSIData.imageFilePath[KSIData.imageFilePath.Length - 1] == '\\' ? "" : "/") + imageName + imageExtension);

        //Last, Reset the ImageData to Our New Settings
        ImageData temp = new ImageData(imageData);
        temp.Name = imageName;
        temp.FileExtension = imageExtension;
        temp.Path = KSIData.imageFilePath + (
            (KSIData.imageFilePath[KSIData.imageFilePath.Length - 1] == '/' || 
            KSIData.imageFilePath[KSIData.imageFilePath.Length - 1] == '\\' ? "" : "/")
            );
        imageData = new ImageData(temp);

    }

    /// <summary>
    /// Rename an Imported Image Based on Settings From UI. 
    /// NOTE: User Can Freely Change the Function to Fit Their Renaming Needs
    /// </summary>
    /// <param name="imageData">The Image Data That Contains the Name to Change</param>
    /// <returns>The Image's New Name</returns>
    public static string RenameImage(ImageData imageData)
    {

        string newName = "";

        //Split the Old File Name by Underscores, That Way We Can Only Grab the Middle Name
        string[] splitName = imageData.Name.Split('_');

        //Index 0: Image Type/Keyword
        //Index 1: Middle Name/Krita Layer Name
        //Index 2: exported + Date Tag
        //Index 3: Time Tag
        splitName[1] = KSIData.customImageName;

        //Reconstruct the Name
        for(int i = 0; i < splitName.Length; i++)
        {

            newName += splitName[i];

            //If We Have Not Reached the End of the Name
            if(i != splitName.Length - 1)
            {

                //Restore the Underscores
                newName += "_";

            }

        }

        return newName;

    }

    /// <summary>
    /// Using Provided ImageImportData, Creates and Loads a New Texture. 
    /// NOTE: Currently Does Not Work! Need To Update to New Handling of Data and Using Files Saved In Project
    /// </summary>
    /// <param name="imageData">The ImageImportData to Load Texture From</param>
    /// <returns>True if Successful; False Otherwise</returns>
    public static bool LoadTexture(ImageData imageData)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(imageData.Path))
        {

            fileData = File.ReadAllBytes(imageData.Path);
            tex = new Texture2D(2, 2);
            bool success = tex.LoadImage(fileData);

            if(!success)
            {

                KSIUI.DisplayDialog("Error: Image Load Failure", "Something Happened and the Importer Could Not Load the Image at " + imageData.Path + "\n\nCannot Continue Import!", "Okay");
                return false;

            }

        }
        else
        {

            KSIUI.DisplayDialog("Error: Image Does Not Exist", "Something Happened and the Importer Cannot Find the Image at " + imageData.Path + "\n\nCannot Continue Import!", "Okay");
            return false;

        }

        TextureData texture = new TextureData();

        texture.texture = tex;
        texture.data = imageData;

        textures.Add(texture);
        return true;

    }

}
