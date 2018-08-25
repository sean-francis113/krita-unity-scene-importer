using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

public class KSImporter : MonoBehaviour {

    /// <summary>
    /// Holds the Data That Will Be Loaded From the XML File
    /// </summary>
    public class ImageImportData
    {

        public string Name { get; set; }
        public string Filename { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public Vector2 Position { get; set; }

        public ImageImportData()
        {

            Name = "";
            Filename = "";
            Path = "";
            Type = "";
            Position = new Vector2(0, 0);

        }

        public ImageImportData(string name, string filename, string path, string type, Vector2 position)
        {

            Name = name;
            Filename = filename;
            Path = path;
            Type = type;
            Position = position;

        }

        public override string ToString()
        {

            return string.Format("[Name: {0}; Filename: {1}; Path: {2}; Type: {3}; Position: ({4},{5})]", Name, Filename, Path, Type, Position.x, Position.y);

        }

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
    static List<ImageImportData> imports = new List<ImageImportData>();

    /// <summary>
    /// Begins the Import Process
    /// </summary>
    public static void StartImport()
    {

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

        //Debug.Log("Import Would Start Now If It Was Implemented.");
        Debug.Log("Loading XML Data");

        string XMLPath = GetXMLPath();

        //Load Data from XML Data
        LoadXMLData(XMLPath);

        for(int i = 0; i < imports.Count; i++)
        {

            Debug.Log("ImageImportData " + i + ": " + imports[i].ToString());

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

            imports.Add(new ImageImportData(
                element.Element("Name").Value, 
                element.Element("Filename").Value,
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

}
