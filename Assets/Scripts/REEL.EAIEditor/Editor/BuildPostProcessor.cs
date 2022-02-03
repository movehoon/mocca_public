using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class BuildPostProcessor
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
#if UNITY_STANDALONE_WIN

        //File.Copy("Oringal File", pathToBuiltProject + "\play_data\plugins\" + Your dll);
        string fileNameWithExtension = Path.GetFileName(pathToBuiltProject);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathToBuiltProject);
        //Debug.Log($"fileName: {fileNameWithExtension}");
        string basePath = pathToBuiltProject.Replace(fileNameWithExtension, "") + "\\" + fileNameWithoutExtension + "_Data";
        string originPath = basePath + "\\Plugins\\OVRLipSync.dll";
        string finalPath = basePath + "\\Plugins\\x86_64\\OVRLipSync.dll";
        //Debug.Log($"originPath: {originPath}, finalPath: {finalPath}");
        
        if (Directory.Exists(basePath + "\\Plugins\\x86_64") == true && File.Exists(originPath) == true)
        {
            File.Move(originPath, finalPath);
        }
#endif
    }
}