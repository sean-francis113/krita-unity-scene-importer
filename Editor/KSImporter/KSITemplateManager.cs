using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class KSITemplateManager {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handlerName"></param>
    public static void AddHandler(string handlerName)
    {

        string KSIKeywordHandlerPath = "Assets/Editor/KSImporter/KSIKeywordHandler.cs";
        string KSISceneManagerPath = "Assets/Editor/KSImporter/KSISceneManager.cs";
        string importHandlerTemplate = 
            "\t<HandlerName>,\n" +
            "\t//<Template Creator Will Add User Created Handlers Here>";
        string strToTypeTemplate =
            "case \"<HandlerName>\":\n" +
            "\t\t\t\treturn ImportHandler.<HandlerName>;\n" +
            "\t\t\t//<Template Creator Will Add New StrToType Case Here>";
        string functionCallTemplate = 
            "case ImportHandler.<HandlerName>:\n" +
            "\t\t\t\tCreate<HandlerName>(textureData);\n"+
            "\t\t\t\tbreak;\n" +
            "\t\t\t//<Template Creator Will Add New Function Call Case Here>";
        string functionTemplate = 
            "/// <summary>\n" +
            "\t/// \n" +
            "\t/// </summary>\n" +
            "\t/// <param name=\"imageObject\"></param>\n" +
            "\t/// <param name=\"textureData\"></param>\n" +
            "\tpublic static void Create<HandlerName>(KSImporter.TextureData textureData)\n" +
            "\t{\n\n" +
            "\t\tGameObject imageObject = new GameObject(textureData.data.NameInScene);\n" +
            "\t\timageObject.transform.position = new Vector3(textureData.data.Position.x, textureData.data.Position.y, 0);\n" +
            "\t\timageObject.AddComponent<SpriteRenderer>().sprite = \n" +
            "\t\t\tSprite.Create(textureData.texture, \n" +
            "\t\t\t\tnew Rect(0.0f, 0.0f, \n" +
            "\t\t\t\t\ttextureData.texture.width, \n" +
            "\t\t\t\t\ttextureData.texture.height), \n" +
            "\t\t\t\tnew Vector2(0.5f, 0.5f), \n" +
            "\t\t\t\t100.0f);\n\n" +
            "\t\t//Add Functionality Here As Desired\n\n" +
            "\t\t//Look Through the Layer/Handler List to Find the Layer Associated With This Handler\n" +
            "\t\tfor (int i = 0; i < KSIKeywordHandler.layerList.Count; i++)\n" +
            "\t\t{\n\n" +
            "\t\t\tif (textureData.data.handler == KSIKeywordHandler.layerList[i].handler)\n" +
            "\t\t\t\t//Set the Object's Layer to the Layer Chosen At UI\n" +
            "\t\t\t\timageObject.layer = LayerMask.NameToLayer(InternalEditorUtility.layers[KSIKeywordHandler.layerList[i].layer]);\n" +
            "\t\t\t\tbreak;\n\n" +
            "\t\t\t}\n\n" +
            "\t\t}\n\n" +
            "\t}//FunctionEnd, DO NOT REMOVE THIS COMMENT!\n\n" +
            "\t//<Template Creator Will Add New Handler Functions Here>";

        string importHandlerName = handlerName.ToUpper();

        string handlerFunctionName = CreateFunctionName(handlerName);

        importHandlerTemplate = SetImportHandlerTemplate(importHandlerTemplate, importHandlerName);

        strToTypeTemplate = SetStrToTypeTemplate(strToTypeTemplate, importHandlerName);

        functionCallTemplate = SetFunctionCallTemplate(functionCallTemplate, handlerFunctionName);

        functionTemplate = SetFunctionTemplate(functionTemplate, handlerFunctionName);

        Debug.Log("ImportHandlerTemplate:\n" + importHandlerTemplate);
        Debug.Log("Function Call Template:\n" + functionCallTemplate);
        Debug.Log("Function Template:\n" + functionTemplate);

        bool success = false;

        success = AddToKeywordHandler(importHandlerTemplate, strToTypeTemplate, KSIKeywordHandlerPath);

        if(!success)
        {

            return;

        }

        ResetLineEndings(KSIKeywordHandlerPath);

        success = AddToSceneManager(functionCallTemplate, functionTemplate, KSISceneManagerPath);

        if(!success)
        {

            return;

        }

        ResetLineEndings(KSISceneManagerPath);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handlerName"></param>
    public static void RemoveHandler(string handlerName)
    {

        string KSIKeywordHandlerPath = "Assets/Editor/KSImporter/KSIKeywordHandler.cs";
        string KSISceneManagerPath = "Assets/Editor/KSImporter/KSISceneManager.cs";
        string importHandlerTemplate =
            "<HandlerName>,";
        string strToTypeTemplate =
            "case \"<HandlerName>\":";
        string functionCallTemplate =
            "case ImportHandler.<HandlerName>:";
        string functionTemplate =
            "public static void Create<HandlerName>(KSImporter.TextureData textureData)";

        string importHandlerName = handlerName.ToUpper();

        string handlerFunctionName = CreateFunctionName(handlerName);

        importHandlerTemplate = SetImportHandlerTemplate(importHandlerTemplate, importHandlerName);

        strToTypeTemplate = SetStrToTypeTemplate(strToTypeTemplate, importHandlerName);

        functionCallTemplate = SetFunctionCallTemplate(functionCallTemplate, handlerFunctionName);

        functionTemplate = SetFunctionTemplate(functionTemplate, handlerFunctionName);

        Debug.Log("ImportHandlerTemplate:\n" + importHandlerTemplate);
        Debug.Log("Function Call Template:\n" + functionCallTemplate);
        Debug.Log("Function Template:\n" + functionTemplate);

        bool success = false;

        success = RemoveFromKeywordHandler(importHandlerTemplate, strToTypeTemplate, KSIKeywordHandlerPath);

        if (!success)
        {

            return;

        }

        ResetLineEndings(KSIKeywordHandlerPath);

        success = RemoveFromSceneManager(functionCallTemplate, functionTemplate, KSISceneManagerPath);
        
        if (!success)
        {

            return;

        }

        ResetLineEndings(KSISceneManagerPath);
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handlerName"></param>
    /// <returns></returns>
    public static string CreateFunctionName(string handlerName)
    {

        string handlerFunctionName = "";

        //Set Up Name To Be Used With Function
        for (int i = 0; i < handlerName.Length; i++)
        {

            if (i == 0)
            {

                //First Character Uppercase
                handlerFunctionName += handlerName[i].ToString().ToUpper();

            }
            else
            {

                //Lowercase The Rest of the Name
                string lowerChar = handlerName[i].ToString().ToLower();

                handlerFunctionName += lowerChar;

            }

        }

        return handlerFunctionName;

    }

    /// <summary>
    /// Sets the Import Handler Template With the New Handler Name
    /// </summary>
    /// <param name="template">The Template We Are Using</param>
    /// <param name="handlerName">The Handler Name To Insert</param>
    /// <returns>The New Template</returns>
    public static string SetImportHandlerTemplate(string template, string handlerName)
    {

        return template.Replace("<HandlerName>", handlerName);
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="template"></param>
    /// <param name="handlerFunctionName"></param>
    /// <returns></returns>
    public static string SetFunctionCallTemplate(string template, string handlerFunctionName)
    {

        string newString = template.Replace("ImportHandler.<HandlerName>", "ImportHandler." + handlerFunctionName.ToUpper());

        string finalString = newString.Replace("<HandlerName>", handlerFunctionName);

        return finalString;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="template"></param>
    /// <param name="handlerFunctionName"></param>
    /// <returns></returns>
    public static string SetFunctionTemplate(string template, string handlerFunctionName)
    {

        return template.Replace("<HandlerName>", handlerFunctionName);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="template"></param>
    /// <param name="importHandlerName"></param>
    /// <returns></returns>
    public static string SetStrToTypeTemplate(string template, string importHandlerName)
    {

        return template.Replace("<HandlerName>", importHandlerName);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="importTemplate"></param>
    /// <param name="strToTypeTemplate"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool AddToKeywordHandler(string importTemplate, string strToTypeTemplate, string path)
    {

        string scriptString = File.ReadAllText(path);

        if(scriptString.Contains(importTemplate))
        {

            KSIUI.DisplayDialog("Error: Duplicate Handler",
                "The Handler You Are Trying to Create Already Exists! You Cannot Create Duplicate Handlers!", 
                "Okay");

            return false;
            
        }
        else if(scriptString.Contains(strToTypeTemplate))
        {

            KSIUI.DisplayDialog("Error: Duplicate Handler",
                "The Handler You Are Trying to Create Already Exists! You Cannot Create Duplicate Handlers!",
                "Okay");

            return false;

        }

        string newScriptString = scriptString.Replace("//<Template Creator Will Add User Created Handlers Here>", importTemplate);

        string finalScriptString = newScriptString.Replace("//<Template Creator Will Add New StrToType Case Here>", strToTypeTemplate);

        File.WriteAllText(path, finalScriptString);

        return true;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="functionCallTemplate"></param>
    /// <param name="functionDefTemplate"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool AddToSceneManager(string functionCallTemplate, string functionDefTemplate, string path)
    {

        string scriptString = File.ReadAllText(path);

        //Should Be Caught Before Now, But Just In Case
        if (scriptString.Contains(functionCallTemplate))
        {

            KSIUI.DisplayDialog("Error: Duplicate Handler",
                "The Handler You Are Trying to Create Already Exists! You Cannot Create Duplicate Handlers!",
                "Okay");

            return false;

        }
        else if (scriptString.Contains(functionDefTemplate))
        {

            KSIUI.DisplayDialog("Error: Duplicate Handler",
                "The Handler You Are Trying to Create Already Exists! You Cannot Create Duplicate Handlers!",
                "Okay");

            return false;

        }

        string newScriptString = scriptString.Replace("//<Template Creator Will Add New Function Call Case Here>", functionCallTemplate);

        string finalScriptString = newScriptString.Replace("//<Template Creator Will Add New Handler Functions Here>", functionDefTemplate);

        File.WriteAllText(path, finalScriptString);

        return true;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="importTemplate"></param>
    /// <param name="strToTypeTemplate"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool RemoveFromKeywordHandler(string importTemplate, string strToTypeTemplate, string path)
    {

        string scriptString = File.ReadAllText(path);
        bool foundTemplate = false;
        int indexOfTemplate = 0;

        string[] scriptSplit = scriptString.Split('\n');
        List<string> splitList = new List<string>();

        for(int h = 0; h < scriptSplit.Length; h++)
        {

            splitList.Add(scriptSplit[h]);
            //Debug.Log("Line " + (h + 1) + ":\n" + scriptSplit[h]);

        }

        for(int i = 0; i < splitList.Count; i++)
        {

            //We Found the Import Handler!
            if(splitList[i].Contains(importTemplate))
            {

                Debug.Log("We Found the Template!");
                indexOfTemplate = i;
                foundTemplate = true;
                splitList.RemoveAt(indexOfTemplate);

            }

            //We Found the StrToType Case!
            else if(splitList[i].Contains(strToTypeTemplate))
            {

                indexOfTemplate = i;
                foundTemplate = true;

                splitList.RemoveAt(i + 1);
                splitList.RemoveAt(i);

            }

        }

        string finalString = "";

        //If We Looked Through the Whole Script But Did Not Find the Template
        if(!foundTemplate)
        {

            KSIUI.DisplayDialog("Error: Handler Does Not Exist",
                "The Handler You Are Trying to Remove Does Not Exist! You Cannot Remove Non-Existant Handlers!",
                "Okay");

            return false;

        }
        else
        {

            //Reconstruct the Final Script String
            for(int i = 0; i < splitList.Count; i++)
            {

                finalString += splitList[i] + "\n";

            }

        }

        File.WriteAllText(path, finalString);

        return true;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="functionCallTemplate"></param>
    /// <param name="functionDefTemplate"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool RemoveFromSceneManager(string functionCallTemplate, string functionDefTemplate, string path)
    {

        string scriptString = File.ReadAllText(path);
        bool foundTemplate = false;
        bool foundStart = false;
        bool foundEnd = false;
        int indexOfTemplate = 0;
        int indexToStart = 0;
        int indexToEnd = 0;

        string[] scriptSplit = scriptString.Split('\n');
        List<string> splitList = new List<string>();

        for (int h = 0; h < scriptSplit.Length; h++)
        {

            splitList.Add(scriptSplit[h]);

        }

        for (int i = 0; i < splitList.Count; i++)
        {

            //We Found the Function Definition!
            if (splitList[i].Contains(functionDefTemplate))
            {

                indexOfTemplate = i;
                foundTemplate = true;

                for (int j = indexOfTemplate; j > 0; j--)
                {

                    //We Found the Summary Start!
                    if (splitList[j].Contains("/// <summary>"))
                    {

                        indexToStart = j;
                        foundStart = true;                                            
                        break;

                    }

                }

                for (int k = indexOfTemplate; k < splitList.Count; k++)
                {

                    //We Found the End of the Function!
                    if (splitList[k].Contains("}//FunctionEnd"))
                    {

                        indexToEnd = k;
                        foundEnd = true;
                        break;

                    }

                }

                if (foundStart && foundEnd)
                {

                    for (int index = indexToEnd + 1; index >= indexToStart; index--)
                    {

                        splitList.RemoveAt(index);

                    }

                }
                else if(!foundStart && foundEnd)
                {

                    for (int index = indexToEnd; index >= indexOfTemplate; index--)
                    {

                        splitList.RemoveAt(index);

                    }

                }

            }

            //We Found the Function Call!
            else if (splitList[i].Contains(functionCallTemplate))
            {

                indexOfTemplate = i;
                foundTemplate = true;

                splitList.RemoveAt(i + 2);
                splitList.RemoveAt(i + 1);
                splitList.RemoveAt(i);

            }

        }

        string finalString = "";

        //If We Looked Through the Whole Script But Did Not Find the Template
        if (!foundTemplate)
        {

            KSIUI.DisplayDialog("Error: Handler Does Not Exist",
                "The Handler You Are Trying to Remove Does Not Exist! You Cannot Remove Non-Existant Handlers!",
                "Okay");

            return false;

        }
        else if(foundTemplate && !foundEnd)
        {

            KSIUI.DisplayDialog("Error: Function End Not Found",
                "We Could Not Find the End of the Handler's Function! Cannot Finish Removing Handler!",
                "Okay");

            return false;

        }
        else
        {

            //Reconstruct the Final Script String
            for (int i = 0; i < splitList.Count; i++)
            {

                finalString += splitList[i] + "\n";

            }

        }

        File.WriteAllText(path, finalString);

        return true;

    }
    
    /// <summary>
    /// Reloads the Provided Script So it Has Consistant Line Endings
    /// </summary>
    /// <param name="path">The Filepath to the Script to Reset</param>
    public static void ResetLineEndings(string path)
    {

        string[] newStringArr = File.ReadAllLines(path);

        File.WriteAllLines(path, newStringArr);

    }

}