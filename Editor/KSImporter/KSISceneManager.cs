using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSISceneManager : MonoBehaviour {

    public static void AddToScene(KSImporter.TextureData textureData)
    {

        GameObject imageObject = Instantiate(new GameObject());

        switch(textureData.data.handler)
        {

            case ImportHandler.GROUND:
                CreateGround(imageObject, textureData);
                break;
            case ImportHandler.PLATFORM:
                CreatePlatform(imageObject, textureData);
                break;
            case ImportHandler.SCENERY:
                CreateScenery(imageObject, textureData);
                break;
			//<Template Creator Will Add New Function Call Case Here>
            default: //If Handler Was None
                return;

        }

    }

    /// <summary>
    /// Adds a Collider(Box) To imageObject and Adds Ground Layer to imageObject (If Ground Layer Exists)
    /// </summary>
    /// <param name="imageObject"></param>
    /// <param name="textureData"></param>
    public static void CreateGround(GameObject imageObject, KSImporter.TextureData textureData)
    {



    }//FunctionEnd, DO NOT REMOVE THIS COMMENT!

    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageObject"></param>
    /// <param name="textureData"></param>
    public static void CreatePlatform(GameObject imageObject, KSImporter.TextureData textureData)
    {



    }//FunctionEnd, DO NOT REMOVE THIS COMMENT!

    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageObject"></param>
    /// <param name="textureData"></param>
    public static void CreateScenery(GameObject imageObject, KSImporter.TextureData textureData)
    {



    }//FunctionEnd, DO NOT REMOVE THIS COMMENT!

	//<Template Creator Will Add New Handler Functions Here>

}


