/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRToolkit.Sharing
{
    using NRKernal;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public class SharringEditor : EditorWindow
    {
        private static readonly string SharringServerCliBinary = "NRSharringServer";

        [MenuItem("NRSDK/Start Sharring Server")]
        static void StartServer()
        {
            string binary_path;
            if (FindCliBinaryPath(out binary_path))
            {
                ShellHelper.RunCommand(binary_path, null);
            }
        }

        public static bool FindCliBinaryPath(out string path)
        {
            var binaryName = SharringServerCliBinary;
            string[] cliBinaryGuid = AssetDatabase.FindAssets(binaryName);
            if (cliBinaryGuid.Length == 0)
            {
                Debug.LogErrorFormat("Could not find required tool for building Sharring: {0}. " +
                    "Was it removed from the NRSDK?", binaryName);
                path = string.Empty;
                return false;
            }

            // Remove the '/Assets' from the project path since it will be added in the path below.
            string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            path = Path.Combine(projectPath, AssetDatabase.GUIDToAssetPath(cliBinaryGuid[0]));
            return !string.IsNullOrEmpty(path);
        }
    }
}