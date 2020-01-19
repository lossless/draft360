/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;
    using UnityEditor;
    using UnityEditor.Build;
    using System.IO;
#if UNITY_2018_1_OR_NEWER
    using UnityEditor.Build.Reporting;
    using UnityEngine;
    using UnityEditor.Android;
    using System.Text;
#endif

#if UNITY_2018_1_OR_NEWER
    internal class PreprocessBuildBase : IPreprocessBuildWithReport, IPostGenerateGradleAndroidProject
#else
    internal class PreprocessBuildBase : IPreprocessBuild
#endif
    {
        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

#if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildReport report)
        {
            OnPreprocessBuild(report.summary.platform, report.summary.outputPath);
        }
#endif

        public virtual void OnPreprocessBuild(BuildTarget target, string path)
        {
            if (target == BuildTarget.Android)
            {
                OnPreprocessBuildForAndroid();
            }
        }

        private const string DefaultXML = @"<?xml version='1.0' encoding='utf-8'?>
<manifest
    xmlns:android='http://schemas.android.com/apk/res/android'
    package='com.unity3d.player'
    xmlns:tools='http://schemas.android.com/tools'
    android:installLocation='preferExternal'>
  <uses-sdk tools:overrideLibrary='com.nreal.glasses_sdk'/>
  <supports-screens
      android:smallScreens='true'
      android:normalScreens='true'
      android:largeScreens='true'
      android:xlargeScreens='true'
      android:anyDensity='true'/>

  <application
      android:theme='@style/UnityThemeSelector'
      android:icon='@mipmap/app_icon'
      android:label='@string/app_name'>
    <activity android:name='com.unity3d.player.UnityPlayerActivity'>
      <intent-filter>
        <action android:name='android.intent.action.MAIN' />
        <category android:name='android.intent.category.INFO' tools:node='replace'/>
      </intent-filter>
    </activity>
    <meta-data android:name='nreal_sdk' android:value='true'/>
  </application>
  <uses-permission android:name='android.permission.CAMERA'/>
  <uses-permission android:name='android.permission.BLUETOOTH'/>
  <uses-permission android:name='android.permission.BLUETOOTH_ADMIN' />
</manifest>
        ";

        private const bool isShowOnDesktop = true;
        [MenuItem("NRSDK/PreprocessBuildForAndroid")]
        public static void OnPreprocessBuildForAndroid()
        {
            string basePath = Application.dataPath + "/Plugins/Android";
            if (!Directory.Exists(Application.dataPath + "/Plugins"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Plugins");
            }
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            string xmlPath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
            if (!File.Exists(xmlPath))
            {
                string xml = DefaultXML.Replace("\'", "\"");
                if (isShowOnDesktop)
                {
                    xml = xml.Replace("<category android:name=\"android.intent.category.INFO\" tools:node=\"replace\"/>",
                       "<category android:name=\"android.intent.category.LAUNCHER\" />");
                }
                File.WriteAllText(xmlPath, xml);
            }
            else
            {
                AutoGenerateAndroidManifest(xmlPath);
            }
            AssetDatabase.Refresh();
        }

        public static void AutoGenerateAndroidManifest(string path)
        {
            var androidManifest = new AndroidManifest(path);

            androidManifest.SetCameraPermission();
            androidManifest.SetBlueToothPermission();
            androidManifest.SetSDKMetaData();
            androidManifest.SetAPKDisplayedOnLauncher(isShowOnDesktop);

            androidManifest.Save();
        }

        public void OnPostGenerateGradleAndroidProject(string basePath)
        {
            var pathBuilder = new StringBuilder(basePath);
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
            AutoGenerateAndroidManifest(pathBuilder.ToString());
        }
    }
}