/****************************************************************************
* Copyright 2019 Nreal Techonology Limited.All rights reserved.
*
* This file is part of NRSDK.
*
* https://www.nreal.ai/        
*
*****************************************************************************/

namespace NRKernal
{
    using UnityEditor;
    using UnityEngine;
    using System.IO;
    using UnityEngine.Rendering;

    [InitializeOnLoad]
    public class ProjectTipsWindow : EditorWindow
    {
        private abstract class Check
        {
            protected string key;

            public void Ignore()
            {
                EditorPrefs.SetBool(ignorePrefix + key, true);
            }

            public bool IsIgnored()
            {
                return EditorPrefs.HasKey(ignorePrefix + key);
            }

            public void DeleteIgnore()
            {
                EditorPrefs.DeleteKey(ignorePrefix + key);
            }

            public abstract bool IsValid();

            public abstract void DrawGUI();

            public abstract bool IsFixable();

            public abstract void Fix();
        }

        private class CkeckAndroidVsyn : Check
        {
            public CkeckAndroidVsyn()
            {
                key = this.GetType().Name;
            }

            public override bool IsValid()
            {
                return QualitySettings.vSyncCount == 0;
            }

            public override void DrawGUI()
            {
                EditorGUILayout.HelpBox("vSyn is opened on Mobile Devices", MessageType.Error);

                string message = @"In order to render correct on mobile devices, the vSyn in quality settings must be disabled. 
in dropdown list of Quality Settings > V Sync Count, choose 'Dont't Sync' for all levels.";
                EditorGUILayout.LabelField(message, EditorStyles.textArea);
            }

            public override bool IsFixable()
            {
                return true;
            }

            public override void Fix()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
                    EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
                {

                    QualitySettings.vSyncCount = 0;
                }
            }
        }

        private class CkeckAndroidSDCardPermission : Check
        {
            public CkeckAndroidSDCardPermission()
            {
                key = this.GetType().Name;
            }

            public override bool IsValid()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    return PlayerSettings.Android.forceSDCardPermission;
                }
                else
                {
                    return false;
                }
            }

            public override void DrawGUI()
            {
                EditorGUILayout.HelpBox("Sdcard permission not available", MessageType.Error);

                string message = @"In order to run correct on mobile devices, the sdcard write permission should be set. 
in dropdown list of Player Settings > Other Settings > Write Permission, choose 'External(SDCard)'.";
                EditorGUILayout.LabelField(message, EditorStyles.textArea);
            }

            public override bool IsFixable()
            {
                return true;
            }

            public override void Fix()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    PlayerSettings.Android.forceSDCardPermission = true;
                }
            }
        }

        private class CkeckAndroidOrientation : Check
        {
            public CkeckAndroidOrientation()
            {
                key = this.GetType().Name;
            }

            public override bool IsValid()
            {
                return PlayerSettings.defaultInterfaceOrientation == UIOrientation.Portrait;
            }

            public override void DrawGUI()
            {
                EditorGUILayout.HelpBox("Orientation is not portrait", MessageType.Error);

                string message = @"In order to display correct on mobile devices, the orientation should be set to portrait. 
in dropdown list of Player Settings > Resolution and Presentation > Default Orientation, choose 'Portrait'.";
                EditorGUILayout.LabelField(message, EditorStyles.textArea);
            }

            public override bool IsFixable()
            {
                return true;
            }

            public override void Fix()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
                }
            }
        }

        private class CkeckAndroidGraphicsAPI : Check
        {
            public CkeckAndroidGraphicsAPI()
            {
                key = this.GetType().Name;
            }

            public override bool IsValid()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    var graphics = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
                    if (graphics != null && graphics.Length == 1 &&
                        graphics[0] == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }

            public override void DrawGUI()
            {
                EditorGUILayout.HelpBox("GraphicsAPIs is not OpenGLES3", MessageType.Error);

                string message = @"In order to render correct on mobile devices, the graphicsAPIs should be set to OpenGLES3. 
in dropdown list of Player Settings > Other Settings > Graphics APIs , choose 'OpenGLES3'.";
                EditorGUILayout.LabelField(message, EditorStyles.textArea);
            }

            public override bool IsFixable()
            {
                return true;
            }

            public override void Fix()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new GraphicsDeviceType[1] { GraphicsDeviceType.OpenGLES3 });
                }
            }
        }

        private class CkeckColorSpace : Check
        {
            public CkeckColorSpace()
            {
                key = this.GetType().Name;
            }

            public override bool IsValid()
            {
                return PlayerSettings.colorSpace == ColorSpace.Linear;
            }

            public override void DrawGUI()
            {
                EditorGUILayout.HelpBox("ColorSpace is not Linear", MessageType.Warning);

                string message = @"In order to display correct on mobile devices, the colorSpace should be set to linear. 
in dropdown list of Player Settings > Other Settings > Color Space, choose 'Linear'.";
                EditorGUILayout.LabelField(message, EditorStyles.textArea);
            }

            public override bool IsFixable()
            {
                return true;
            }

            public override void Fix()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    PlayerSettings.colorSpace = ColorSpace.Linear;
                }
            }
        }

        private static Check[] checks = new Check[]
        {
            new CkeckAndroidVsyn(),
            new CkeckAndroidSDCardPermission(),
            new CkeckAndroidOrientation(),
            new CkeckAndroidGraphicsAPI(),
            new CkeckColorSpace(),
        };

        private static ProjectTipsWindow m_Window;
        private Vector2 m_ScrollPosition;
        private const string ignorePrefix = "NRKernal.ignore";

        static ProjectTipsWindow()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        [MenuItem("NRSDK/Project Tips", false, 50)]
        public static void ShowWindow()
        {
            m_Window = GetWindow<ProjectTipsWindow>(true);
            m_Window.minSize = new Vector2(320, 300);
            m_Window.maxSize = new Vector2(320, 800);
            m_Window.titleContent = new GUIContent("NRSDK | Project Tips");
        }

        private static void Update()
        {
            bool show = false;

            foreach (Check check in checks)
            {
                if (!check.IsIgnored() &&
                    !check.IsValid())
                {
                    show = true;
                }
            }

            if (show)
            {
                ShowWindow();
            }

            EditorApplication.update -= Update;
        }

        public void OnGUI()
        {
            var resourcePath = GetResourcePath();
            var logo = AssetDatabase.LoadAssetAtPath<Texture2D>(resourcePath + "icon.png");
            var rect = GUILayoutUtility.GetRect(position.width, 80, GUI.skin.box);
            GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);

            string aboutText = "This window provides tips to help fix common issues with the NRSDK and your project.";
            EditorGUILayout.LabelField(aboutText, EditorStyles.textArea);

            int ignoredCount = 0;
            int fixableCount = 0;
            int invalidNotIgnored = 0;

            for (int i = 0; i < checks.Length; i++)
            {
                Check check = checks[i];

                bool ignored = check.IsIgnored();
                bool valid = check.IsValid();
                bool fixable = check.IsFixable();

                if (!valid &&
                    !ignored &&
                    fixable)
                {
                    fixableCount++;
                }

                if (!valid &&
                    !ignored)
                {
                    invalidNotIgnored++;
                }

                if (ignored)
                {
                    ignoredCount++;
                }
            }

            Rect issuesRect = EditorGUILayout.GetControlRect();
            GUI.Box(new Rect(issuesRect.x - 4, issuesRect.y, issuesRect.width + 8, issuesRect.height), "Tips", EditorStyles.toolbarButton);

            if (invalidNotIgnored > 0)
            {
                m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
                {
                    for (int i = 0; i < checks.Length; i++)
                    {
                        Check check = checks[i];

                        if (!check.IsIgnored() &&
                            !check.IsValid())
                        {
                            invalidNotIgnored++;

                            GUILayout.BeginVertical("box");
                            {
                                check.DrawGUI();

                                EditorGUILayout.BeginHorizontal();
                                {
                                    // Aligns buttons to the right
                                    GUILayout.FlexibleSpace();

                                    if (check.IsFixable())
                                    {
                                        if (GUILayout.Button("Fix"))
                                            check.Fix();
                                    }

                                    //if (GUILayout.Button("Ignore"))
                                    //    check.Ignore();
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            GUILayout.EndVertical();
                        }
                    }
                }
                GUILayout.EndScrollView();
            }

            GUILayout.FlexibleSpace();

            if (invalidNotIgnored == 0)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label("No issues found");

                        if (GUILayout.Button("Close Window"))
                            Close();
                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.BeginHorizontal("box");
            {
                if (fixableCount > 0)
                {
                    if (GUILayout.Button("Accept All"))
                    {
                        if (EditorUtility.DisplayDialog("Accept All", "Are you sure?", "Yes, Accept All", "Cancel"))
                        {
                            for (int i = 0; i < checks.Length; i++)
                            {
                                Check check = checks[i];

                                if (!check.IsIgnored() &&
                                    !check.IsValid())
                                {
                                    if (check.IsFixable())
                                        check.Fix();
                                }
                            }
                        }
                    }
                }

                //if (invalidNotIgnored > 0)
                //{
                //    if (GUILayout.Button("Ignore All"))
                //    {
                //        if (EditorUtility.DisplayDialog("Ignore All", "Are you sure?", "Yes, Ignore All", "Cancel"))
                //        {
                //            for (int i = 0; i < checks.Length; i++)
                //            {
                //                Check check = checks[i];

                //                if (!check.IsIgnored())
                //                    check.Ignore();
                //            }
                //        }
                //    }
                //}

                //if (ignoredCount > 0)
                //{
                //    if (GUILayout.Button("Show Ignored"))
                //    {
                //        foreach (Check check in checks)
                //            check.DeleteIgnore();
                //    }
                //}
            }
            GUILayout.EndHorizontal();
        }

        private string GetResourcePath()
        {
            var ms = MonoScript.FromScriptableObject(this);
            var path = AssetDatabase.GetAssetPath(ms);
            path = Path.GetDirectoryName(path);
            return path.Substring(0, path.Length - "Editor".Length - 1) + "Textures/";
        }
    }
}
