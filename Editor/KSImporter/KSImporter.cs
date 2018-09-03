using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using System.IO;
using System.Xml.Linq;

public class KSImporter : MonoBehaviour {

    #region Data Sets

    /// <summary>
    /// Holds the Data That Will Be Loaded From the XML File. 
    /// It Will Then Be Altered During Runtime as We Move Files Around
    /// </summary>
    public class ImageData
    {

        /// <summary>
        /// The Name of the Image
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Name of the Associated Game Object in Scene
        /// </summary>
        public string NameInScene { get; set; }

        /// <summary>
        /// The Full Filename of the Image
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// The Image's Extension (.png, .jpg, etc)
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// The Full Filepath of the Image
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The Keyword of the Image
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// How Will We Handle Creating This Image In Scene?
        /// </summary>
        public ImportHandler handler; 

        /// <summary>
        /// Where Was This Image in the Krita Project?
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="imageData">Image Data to Construct With</param>
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameInScene"></param>
        /// <param name="filename"></param>
        /// <param name="fileExtension"></param>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="position"></param>
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

        /// <summary>
        /// The Image's Unity Texture
        /// </summary>
        public Texture2D texture { get; set; }

        /// <summary>
        /// The Image's Data
        /// </summary>
        public ImageData data { get; set; }

    }

    #endregion

    #region Local Variables

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
    /// Used to Add a Unique Number to Files With the Same Type and Name
    /// </summary>
    static int customNameCount = 0;

    #endregion

    #region Functionality

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

        if(KSIData.useCustomNames && KSIData.customImageName != "")
        {

            SetCustomNames();

        }

        //Create a New String and Get Its Path
        string scenePath = CreateNewScene();

        //If the User Canceled Creating a Scene
        if(scenePath == "")
        {

            return;

        }

        //Grab a Reference to the Scene So We Can Save it When We Are All Done
        Scene importScene = SceneManager.GetSceneByPath(scenePath);
        
        //For Each Imported Image We Need to Handle
        for(int i = 0; i < imports.Count; i++)
        {

            //Get the Image So We Can Pass it by Reference
            ImageData curImage = imports[i];

            //Save the Image Into the Project
            bool success = SaveImage(ref curImage);

            if(!success)
            {

                return;

            }

            Debug.Log("New Image Info: " + curImage.ToString());

            //Load the Texture For Use With the Importer
            success = LoadTexture(ref curImage);

            if(!success)
            {

                return;

            }

            //Save the Image Information
            imports[i] = curImage;
            TextureData curTexture = textures[textures.Count - 1];
            Debug.Log("Texture Data: " + curTexture.data.ToString());

            //Set the Importer Information
            SetImporter(ref curTexture);

            //Set the Handler Information
            SetHandler(ref curTexture);

            //Set the Images Name in Scene
            SetImageSceneName(ref curTexture);

            //Save the New Texture Information
            textures[textures.Count - 1] = curTexture;

        }

        //Now That All of the Images Are Imported, Start Adding Them To The Scene
        foreach (var texture in textures)
        {

            KSISceneManager.AddToScene(texture);

        }

        //Save the Final Scene
        EditorSceneManager.SaveScene(importScene);

        //Reset Lists For Reuse
        textures.Clear();
        imports.Clear();
        
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
            ((KSIData.useCustomNames && KSIData.customSceneName != "") ? KSIData.customSceneName : KSIData.xmlSceneName) + ".unity";

        if(File.Exists(scenePath) == true)
        {

            if (KSIUI.DisplayDialog("Error: Scene Exists",
                "There is Already a Scene Named " +
                (KSIData.useCustomNames ? KSIData.customSceneName : KSIData.xmlSceneName) +
                "at " + KSIData.sceneFilePath + ". Do You Wish To Replace It?", 
                "Yes, Replace It", "No, Return to Importer"))
            {

                File.Delete(scenePath);

            }
            else
            {

                return "";

            }

        }

        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject mainCamera = new GameObject("Main Camera", typeof(Camera));
        mainCamera.transform.position = new Vector3(0.0f, 0.0f, -10.0f);

        EditorSceneManager.SaveScene(newScene, KSIData.sceneFilePath + (KSIData.useCustomNames ? KSIData.customSceneName : KSIData.xmlSceneName) + ".unity");

        AssetDatabase.Refresh();

        return newScene.path;

    }

    /// <summary>
    /// Save the Image Into the Project Files
    /// </summary>
    /// <param name="imageData">The Data of the Image to Save</param>
    public static bool SaveImage(ref ImageData imageData)
    {

        //What Image Extensions Does This Importer Support?
        string[] supportedExtensions = { ".png", ".jpg", ".jpeg" };

        bool fileFound = false;

        for (int i = 0; i < supportedExtensions.Length; i++)
        {

            //If We Found the File Has the Currently Itterating Extension
            if (File.Exists(imageData.Path + imageData.Filename + supportedExtensions[i]))
            {

                fileFound = true;
                break;

            }

        }

        if (!fileFound)
        {

            //If We Did Not Find the File, Tell the User
            KSIUI.DisplayDialog("Error: File Does Not Exist!",
                "The File the Importer is Looking for Does Not Exist!\n\nFile: " + imageData.Filename + "\n\nCannot Continue Import!",
                "Okay");
            return false;

        }

        //Make Sure We Have the Right Image Name
        string imageName = RenameImage(imageData);

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

            string errorMessage = "The Image You Are Trying to Save is Not Supported By the Importer!\n\nSupported Extensions: { ";

            for(int i = 0; i < supportedExtensions.Length; i++)
            {

                if (i != supportedExtensions.Length - 1)
                {

                    errorMessage += "'" + supportedExtensions[i] + "' , ";

                }
                else
                {

                    errorMessage += "'" + supportedExtensions[i] + "' ";

                }

            }

            errorMessage += "}";

            KSIUI.DisplayDialog("Error: Unsupported Image", errorMessage, "Okay");

            return false;

        }

        //Make Sure the Destination Path Exists
        //If Not, Create It
        if(!Directory.Exists(KSIData.imageFilePath))
        {

            Directory.CreateDirectory(KSIData.imageFilePath);

        }

        //If the File Already Exists
        if (File.Exists(KSIData.imageFilePath + imageName + imageExtension))
        {

            //Delete the Image So We Can Re Save the File
            File.Delete(KSIData.imageFilePath + imageName + imageExtension);

        }

        //We Can Save the Image
        File.Copy(imageData.Path + imageData.Filename + imageExtension, KSIData.imageFilePath + imageName + imageExtension);

        //Last, Reset the ImageData to Our New Settings
        ImageData temp = new ImageData(imageData);
        temp.Filename = imageName;
        temp.FileExtension = imageExtension;
        temp.Path = KSIData.imageFilePath + imageName + imageExtension;
        imageData = new ImageData(temp);

        AssetDatabase.Refresh();

        return true;

    }

    /// <summary>
    /// Rename an Imported Image's Filename Based on Settings From UI. 
    /// NOTE: User Can Freely Change the Function to Fit Their Renaming Needs
    /// </summary>
    /// <param name="imageData">The Image Data That Contains the Name to Change</param>
    /// <returns>The Image's New Name</returns>
    public static string RenameImage(ImageData imageData)
    {

        string newName = "";

        //What Index Does User Want to Stop Name Reconstruction At?
        //Index 0: Image Type/Keyword
        //Index 1: Middle Name/Krita Layer Name
        //Index 2: exported + Date Tag
        //Index 3: Time Tag
        const int NAME_END_INDEX = 1;

        //Split the Old File Name by Underscores, That Way We Can Grab Specific Names
        string[] splitName = imageData.Filename.Split('_');

        //Reconstruct the Name
        for(int i = 0; i <= NAME_END_INDEX; i++)
        {

            Debug.Log("Itterated Reconstruction!");
            newName += splitName[i];

            //If We Have Not Reached the End of the Name
            if(i != NAME_END_INDEX)
            {

                //Restore the Underscores
                newName += "_";

            }

            Debug.Log("New Name Now: " + newName);

        }

        return newName;

    }

    /// <summary>
    /// Sets the Filenames of the Images to Their New Custom Names
    /// </summary>
    public static void SetCustomNames()
    {

        for (int i = 0; i < KSIData.keywordList.Count; i++)
        {

            customNameCount = 0;
            string currentKeyword = KSIData.keywordList[i].keyword.ToLower();

            for (int j = 0; i < imports.Count; i++)
            {               

                string oldName = imports[j].Filename;
                string keyword = imports[j].Type;

                if (keyword.ToLower() == currentKeyword)
                {

                    string[] nameSplit = oldName.Split('_');

                    nameSplit[1] = KSIData.customImageName + customNameCount;

                    string finalName = "";

                    for(int k = 0; k < nameSplit.Length; k++)
                    {

                        if(k != nameSplit.Length - 1)
                        {

                            finalName += nameSplit[k] + "_";

                        }
                        else
                        {

                            finalName += nameSplit[k];

                        }

                    }

                    imports[j].Filename = finalName;
                    break;

                }

            }

        }

    }

    /// <summary>
    /// Using Provided ImageImportData, Creates and Loads a New Texture. 
    /// NOTE: Currently Does Not Work! Need To Update to New Handling of Data and Using Files Saved In Project
    /// </summary>
    /// <param name="imageData">The ImageImportData to Load Texture From</param>
    /// <returns>True if Successful; False Otherwise</returns>
    public static bool LoadTexture(ref ImageData imageData)
    {

        Texture2D tex = null;
        byte[] fileData;

        string absolutePath = Application.dataPath;

        string[] pathSplit = absolutePath.Split('/');

        absolutePath = "";

        for(int i = 0; i < pathSplit.Length - 1; i++)
        {

            absolutePath += pathSplit[i] + "/";

        }

        string texFilePath = absolutePath + imageData.Path;

        if (File.Exists(texFilePath))
        {

            fileData = File.ReadAllBytes(texFilePath);
            tex = new Texture2D(2, 2);
            bool success = tex.LoadImage(fileData);

            if(!success)
            {

                KSIUI.DisplayDialog("Error: Image Load Failure", "Something Happened and the Importer Could Not Load the Image at " + texFilePath + "\n\nCannot Continue Import!", "Okay");
                return false;

            }

        }
        else
        {

            KSIUI.DisplayDialog("Error: Image Does Not Exist", "Something Happened and the Importer Cannot Find the Image at " + texFilePath + "\n\nCannot Continue Import!", "Okay");
            return false;

        }

        TextureData texture = new TextureData();

        texture.texture = tex;
        texture.data = imageData;

        textures.Add(texture);
        return true;

    }

    /// <summary>
    /// Sets the Texture Importer for the Passed In Texture
    /// </summary>
    /// <param name="texture">The Texture Data to Set the Importer For</param>
    public static void SetImporter(ref TextureData texture)
    {

        TextureImporter importer = AssetImporter.GetAtPath(texture.data.Path) as TextureImporter;

        importer.mipmapEnabled = true;
        importer.isReadable = true;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePivot = Vector2.zero;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Trilinear;
        importer.alphaIsTransparency = true;

        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.SaveAndReimport();

        AssetDatabase.Refresh();

    }

    /// <summary>
    /// Get and Set the Appropriate Handler Information for Importing Into the Scene
    /// </summary>
    /// <param name="textureData">The Image That We Will Be Working On</param>
    public static void SetHandler(ref TextureData textureData)
    {

        textureData.data.handler = KSIKeywordHandler.FindHandler(textureData.data.Type);

    }

    /// <summary>
    /// Set the Name That the Final Object Will Have In Scene. 
    /// NOTE: Can Be Altered To User's Needs
    /// </summary>
    /// <param name="textureData"></param>
    public static void SetImageSceneName(ref TextureData textureData)
    {

        textureData.data.NameInScene = textureData.data.Filename;

    }

    #endregion

}