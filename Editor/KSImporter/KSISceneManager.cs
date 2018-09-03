using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

public class KSISceneManager : MonoBehaviour {

    /// <summary>
    /// Adds a New Object to the Scene Based on Texture Data
    /// </summary>
    /// <param name="textureData">The Data to Use to Create the New Object</param>
    public static void AddToScene(KSImporter.TextureData textureData)
    {

        switch(textureData.data.handler)
        {

            case ImportHandler.GROUND:
                CreateGround(textureData);
                break;
            case ImportHandler.PLATFORM:
                CreatePlatform(textureData);
                break;
            case ImportHandler.SCENERY:
                CreateScenery(textureData);
                break;
			//<Template Creator Will Add New Function Call Case Here>
            default: //If Handler Was None
                return;

        }

    }

    /// <summary>
    /// Adds a Collider(Box) To imageObject and Adds Ground Layer to imageObject
    /// </summary>
    /// <param name="imageObject"></param>
    /// <param name="textureData"></param>
    public static void CreateGround(KSImporter.TextureData textureData)
    {

        GameObject imageObject = new GameObject(textureData.data.NameInScene);
        imageObject.transform.position = new Vector3(textureData.data.Position.x, textureData.data.Position.y, 0);
        imageObject.AddComponent<SpriteRenderer>().sprite = 
            Sprite.Create(textureData.texture, 
                new Rect(0.0f, 0.0f, 
                    textureData.texture.width, 
                    textureData.texture.height), 
                new Vector2(0.5f, 0.5f), 
                100.0f);

        imageObject.AddComponent<BoxCollider2D>();
        imageObject.AddComponent<Rigidbody2D>();

        //Look Through the Layer/Handler List to Find the Layer Associated With This Handler
        for(int i = 0; i < KSIKeywordHandler.layerList.Count; i++)
        {

            if(textureData.data.handler == KSIKeywordHandler.layerList[i].handler)
            {

                //Set the Object's Layer to the Layer Chosen At UI
                imageObject.layer = LayerMask.NameToLayer(InternalEditorUtility.layers[KSIKeywordHandler.layerList[i].layer]);
                break;

            }

        }

    }//FunctionEnd, DO NOT REMOVE THIS COMMENT!

    /// <summary>
    /// Adds a Collider(Box) to imageObject, Adds a Platform Effector to the Collider(Box) and Adds Platform Layer to imageObject
    /// </summary>
    /// <param name="imageObject"></param>
    /// <param name="textureData"></param>
    public static void CreatePlatform(KSImporter.TextureData textureData)
    {

        GameObject imageObject = new GameObject(textureData.data.NameInScene);
        imageObject.transform.position = new Vector3(textureData.data.Position.x, textureData.data.Position.y, 0);

        imageObject.AddComponent<SpriteRenderer>().sprite =
            Sprite.Create(textureData.texture,
                new Rect(0.0f, 0.0f,
                    textureData.texture.width,
                    textureData.texture.height),
                new Vector2(0.5f, 0.5f),
                100.0f);
        imageObject.AddComponent<Rigidbody2D>();

        BoxCollider2D collider = imageObject.AddComponent<BoxCollider2D>();
        PlatformEffector2D effector = imageObject.AddComponent<PlatformEffector2D>();

        effector.useOneWay = true;
        effector.useSideBounce = false;
        effector.useSideFriction = false;
        
        //Look Through the Layer/Handler List to Find the Layer Associated With This Handler
        for (int i = 0; i < KSIKeywordHandler.layerList.Count; i++)
        {

            if (textureData.data.handler == KSIKeywordHandler.layerList[i].handler)
            {

                //Set the Object's Layer to the Layer Chosen At UI
                imageObject.layer = LayerMask.NameToLayer(InternalEditorUtility.layers[KSIKeywordHandler.layerList[i].layer]);
                break;

            }

        }

    }//FunctionEnd, DO NOT REMOVE THIS COMMENT!

    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageObject"></param>
    /// <param name="textureData"></param>
    public static void CreateScenery(KSImporter.TextureData textureData)
    {

        GameObject imageObject = new GameObject(textureData.data.NameInScene);
        imageObject.transform.position = new Vector3(textureData.data.Position.x, textureData.data.Position.y, 0);

        imageObject.AddComponent<SpriteRenderer>().sprite =
            Sprite.Create(textureData.texture,
                new Rect(0.0f, 0.0f,
                    textureData.texture.width,
                    textureData.texture.height),
                new Vector2(0.5f, 0.5f),
                100.0f);

        //Look Through the Layer/Handler List to Find the Layer Associated With This Handler
        for (int i = 0; i < KSIKeywordHandler.layerList.Count; i++)
        {

            if (textureData.data.handler == KSIKeywordHandler.layerList[i].handler)
            {

                //Set the Object's Layer to the Layer Chosen At UI
                imageObject.layer = LayerMask.NameToLayer(InternalEditorUtility.layers[KSIKeywordHandler.layerList[i].layer]);
                break;

            }

        }

    }//FunctionEnd, DO NOT REMOVE THIS COMMENT!

	//<Template Creator Will Add New Handler Functions Here>

}