using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class KSImporter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Begins the Import Process
    /// </summary>
    public static void StartImport()
    {

        Debug.Log("Import Would Begin Now If It Was Implemented!");

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

        Debug.Log("Import Would Start Now If It Was Implemented.");

    }

}
