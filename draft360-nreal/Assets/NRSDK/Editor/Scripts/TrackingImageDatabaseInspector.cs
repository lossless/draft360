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
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using LitJson;

    /// @cond EXCLUDE_FROM_DOXYGEN
    [CustomEditor(typeof(NRTrackingImageDatabase))]
    public class TrackingImageDatabaseInspector : Editor
    {
        private const float m_ImageSpacerHeight = 55f;
        private const int m_PageSize = 5;
        private const float m_HeaderHeight = 30f;
        private static readonly Vector2 m_ContainerStart = new Vector2(14f, 87f);

        private static BackgroundJobExecutor m_QualityBackgroundExecutor = new BackgroundJobExecutor();
        private static NRTrackingImageDatabase m_DatabaseForQualityJobs = null;
        private static Dictionary<string, JsonData> m_UpdatedQualityScores = new Dictionary<string, JsonData>();
        private static Dictionary<string, float> m_TempWidthDict = new Dictionary<string, float>();

        private int m_PageIndex = 0;
        private const float defaultWidth = 0.4f;

        public override void OnInspectorGUI()
        {
            NRTrackingImageDatabase database = target as NRTrackingImageDatabase;
            if (database == null)
            {
                return;
            }

            RunDirtyQualityJobs(database);

            m_PageIndex = Mathf.Min(m_PageIndex, database.Count / m_PageSize);

            DrawTitle();
            DrawContainer();
            DrawColumnNames();

            int displayedImageCount = 0;
            int removeAt = -1;
            int pageStartIndex = m_PageIndex * m_PageSize;
            int pageEndIndex = Mathf.Min(database.Count, pageStartIndex + m_PageSize);
            for (int i = pageStartIndex; i < pageEndIndex; i++, displayedImageCount++)
            {
                NRTrackingImageDatabaseEntry updatedImage;
                bool wasRemoved;

                DrawImageField(database[i], out updatedImage, out wasRemoved);

                if (wasRemoved)
                {
                    removeAt = i;
                }
                else if (!database[i].Equals(updatedImage))
                {
                    database[i] = updatedImage;
                }
            }

            if (removeAt > -1)
            {
                database.RemoveAt(removeAt);
            }

            DrawImageSpacers(displayedImageCount);
            DrawPageField(database.Count);
        }

        private static void RunDirtyQualityJobs(NRTrackingImageDatabase database)
        {
            if (database == null)
            {
                Debug.Log("database is null");
                return;
            }
            if (m_DatabaseForQualityJobs != database)
            {
                // If another database is already running quality evaluation,
                // stop all pending jobs to prioritise the current database.
                if (m_DatabaseForQualityJobs != null)
                {
                    m_QualityBackgroundExecutor.RemoveAllPendingJobs();
                }

                m_DatabaseForQualityJobs = database;
            }

            UpdateDatabaseQuality(database);

            // Set database dirty to refresh inspector UI for each frame that there are still pending jobs.
            // Otherwise if there exists one frame with no newly finished jobs, the UI will never get refreshed.
            // EditorUtility.SetDirty can only be called from main thread.
            if (m_QualityBackgroundExecutor.PendingJobsCount > 0)
            {
                EditorUtility.SetDirty(database);
                return;
            }

            List<NRTrackingImageDatabaseEntry> dirtyEntries = database.GetDirtyQualityEntries();
            if (dirtyEntries.Count == 0)
            {
                return;
            }

            string cliBinaryPath;
            if (!NRTrackingImageDatabase.FindCliBinaryPath(out cliBinaryPath))
            {
                return;
            }

            string outpath = NRTools.GetTrackingImageDataGenPath() + database.GUID + "/";
            if (!Directory.Exists(outpath))
            {
                Directory.CreateDirectory(outpath);
            }

            var resultjson = database.TrackingImageDataPath + "markers.json";

            for (int i = 0; i < dirtyEntries.Count; ++i)
            {
                NRTrackingImageDatabaseEntry image = dirtyEntries[i];
                var imagePath = AssetDatabase.GetAssetPath(image.Texture);
                imagePath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + imagePath;

                m_QualityBackgroundExecutor.PushJob(() =>
                {
                    BuildImage(cliBinaryPath, image, imagePath, outpath, resultjson);
                });
            }
        }

        public static void BuildDataBase(NRTrackingImageDatabase database)
        {
            Debug.Log("Start to build database...");
            if (database == null)
            {
                Debug.Log("database is null");
                return;
            }

            List<NRTrackingImageDatabaseEntry> dirtyEntries = database.GetDirtyQualityEntries();
            if (database.isCliUpdated)
            {
                dirtyEntries = database.GetAllEntries();
            }
            if (dirtyEntries.Count == 0)
            {
                return;
            }
            Debug.Log("dirtyEntries count:" + dirtyEntries.Count);

            string cliBinaryPath;
            if (!NRTrackingImageDatabase.FindCliBinaryPath(out cliBinaryPath))
            {
                return;
            }

            string outpath = NRTools.GetTrackingImageDataGenPath() + database.GUID + "/";
            if (!Directory.Exists(outpath))
            {
                Directory.CreateDirectory(outpath);
            }

            var resultjson = database.TrackingImageDataPath + "markers.json";

            for (int i = 0; i < dirtyEntries.Count; ++i)
            {
                NRTrackingImageDatabaseEntry image = dirtyEntries[i];
                var imagePath = AssetDatabase.GetAssetPath(image.Texture);

                BuildImage(cliBinaryPath, image, imagePath, outpath, resultjson);
            }

            if (File.Exists(resultjson))
            {
                var json_data = File.ReadAllText(resultjson);
                var json_obj = JsonMapper.ToObject(json_data);
                for (int i = 0; i < dirtyEntries.Count; i++)
                {
                    NRTrackingImageDatabaseEntry image = dirtyEntries[i];
                    var textureGUID = image.TextureGUID;

                    //Debug.Log("update quality dict " + image.Name);
                    var image_info = json_obj[image.Name];
                    m_UpdatedQualityScores.Remove(textureGUID);
                    m_UpdatedQualityScores.Add(textureGUID, image_info);
                }
                UpdateDatabaseQuality(database);
                for (int i = 0; i < database.Count; i++)
                {
                    NRTrackingImageDatabaseEntry image = database[i];
                    Debug.Log(image.ToString());
                }
            }
        }

        private static void BuildImage(string cliBinaryPath, NRTrackingImageDatabaseEntry image, string imagepath, string outpath, string resultjson)
        {
            var textureGUID = image.TextureGUID;

            if (image.Width < float.Epsilon)
            {
                image.Width = image.Height = (int)(defaultWidth * 1000);
            }

            string param = string.Format("-image_path={0} -save_dir={1} -width={2}",
                        imagepath, outpath, image.Width).Trim();

            string result = string.Empty;
            string error = string.Empty;
            ShellHelper.RunCommand(cliBinaryPath, param, out result, out error);
            if (File.Exists(resultjson))
            {
                var json_data = File.ReadAllText(resultjson);
                var json_obj = JsonMapper.ToObject(json_data);
                var image_info = json_obj[image.Name];
                lock (m_UpdatedQualityScores)
                {
                    if (!m_UpdatedQualityScores.ContainsKey(textureGUID))
                    {
                        m_UpdatedQualityScores.Add(textureGUID, image_info);
                    }
                }
            }

            //if (!string.IsNullOrEmpty(error))
            //{
            //    Debug.Log("BuildImage error :" + error);
            //}
        }

        private static void UpdateDatabaseQuality(NRTrackingImageDatabase database)
        {
            lock (m_UpdatedQualityScores)
            {
                if (m_UpdatedQualityScores.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < database.Count; ++i)
                {
                    if (m_UpdatedQualityScores.ContainsKey(database[i].TextureGUID))
                    {
                        NRTrackingImageDatabaseEntry updatedImage = database[i];
                        var image_info = m_UpdatedQualityScores[updatedImage.TextureGUID];

                        updatedImage.Quality = float.Parse(image_info["train_score"].ToString()).ToString("#");
                        updatedImage.Width = float.Parse(float.Parse(image_info["physical_width"].ToString()).ToString("#"));
                        updatedImage.Height = float.Parse(float.Parse(image_info["physical_height"].ToString()).ToString("#"));
                        database[i] = updatedImage;
                        //Debug.Log("UpdateDatabaseQuality :" + updatedImage.Name);
                    }
                }

                m_UpdatedQualityScores.Clear();

                // For refreshing inspector UI as new jobs have been enqueued.
                EditorUtility.SetDirty(database);
            }

            // For refreshing inspector UI for updated quality scores.
            EditorUtility.SetDirty(database);
        }

        private void DrawTitle()
        {
            const string TITLE_STRING = "Images in Database";
            GUIStyle titleStyle = new GUIStyle();
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.stretchWidth = true;
            titleStyle.fontSize = 14;
            titleStyle.normal.textColor = UnityEngine.Color.white;
            titleStyle.padding.bottom = 15;

            EditorGUILayout.BeginVertical();
            GUILayout.Space(15);
            EditorGUILayout.LabelField(TITLE_STRING, titleStyle);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

        private void DrawContainer()
        {
            var containerRect = new Rect(m_ContainerStart.x, m_ContainerStart.y, EditorGUIUtility.currentViewWidth - 30,
                (m_PageSize * m_ImageSpacerHeight) + m_HeaderHeight);
            GUI.Box(containerRect, string.Empty);
        }

        private void DrawColumnNames()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(45);

            var style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleLeft;

            GUILayoutOption[] options = { GUILayout.Height(m_HeaderHeight - 10), GUILayout.MaxWidth(80f) };
            EditorGUILayout.LabelField("Name", style, options);
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Width(m)", style, options);
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Quality", style, options);
            GUILayout.FlexibleSpace();
            GUILayout.Space(60);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private string QualityForDisplay(string quality)
        {
            if (string.IsNullOrEmpty(quality))
            {
                return "Calculating...";
            }

            if (quality == "?")
            {
                return "?";
            }

            return quality + "/100";
        }

        private void DrawImageField(NRTrackingImageDatabaseEntry image, out NRTrackingImageDatabaseEntry updatedImage, out bool wasRemoved)
        {
            updatedImage = new NRTrackingImageDatabaseEntry();

            EditorGUILayout.BeginVertical();
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);

            var buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.margin = new RectOffset(0, 0, 13, 0);

            wasRemoved = GUILayout.Button("X", buttonStyle);

            var labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.alignment = TextAnchor.MiddleLeft;

            var textFieldStyle = new GUIStyle(GUI.skin.textField);
            textFieldStyle.margin = new RectOffset(5, 5, 15, 0);
            //updatedImage.Name = EditorGUILayout.TextField(image.Name, textFieldStyle, GUILayout.MaxWidth(80f));
            updatedImage.Name = image.Name;
            EditorGUILayout.LabelField(image.Name, labelStyle, GUILayout.Height(42), GUILayout.MaxWidth(80f));

            GUILayout.Space(5);
            float tempwidth;
            string key = m_DatabaseForQualityJobs == null ? image.Name : m_DatabaseForQualityJobs.GUID + image.Name;
            if (!m_TempWidthDict.TryGetValue(key, out tempwidth))
            {
                if (image.Width < float.Epsilon)
                {
                    image.Width = defaultWidth * 1000;
                }
                //tempwidth = defaultWidth;
                tempwidth = m_TempWidthDict[key] = image.Width / 1000;
            }
            tempwidth = EditorGUILayout.FloatField(tempwidth, textFieldStyle, GUILayout.MaxWidth(80f));
            m_TempWidthDict[key] = tempwidth;

            var rect = GUILayoutUtility.GetLastRect();
            var e = Event.current;
            bool wasWidthChanged = false;
            if (e.type == EventType.MouseDown && !rect.Contains(e.mousePosition))
            {
                var abs = Mathf.Abs(image.Width / 1000 - tempwidth);
                if (abs > 0.01f)
                {
                    updatedImage.Width = tempwidth * 1000;
                    wasWidthChanged = true;
                    GUI.FocusControl(null);
                }
                else
                {
                    updatedImage.Width = image.Width;
                }
            }
            else
            {
                updatedImage.Width = image.Width;
            }
            //EditorGUILayout.LabelField((image.Width / 1000).ToString(), labelStyle, GUILayout.Height(42), GUILayout.MaxWidth(80f));

            GUILayout.Space(5);
            EditorGUILayout.LabelField(QualityForDisplay(image.Quality), labelStyle,
                                       GUILayout.Height(42), GUILayout.MaxWidth(80f));

            GUILayout.FlexibleSpace();

            updatedImage.Texture = EditorGUILayout.ObjectField(image.Texture, typeof(Texture2D), false,
                 GUILayout.Height(45), GUILayout.Width(45)) as Texture2D;
            if (updatedImage.TextureGUID == image.TextureGUID && !wasWidthChanged)
            {
                updatedImage.Quality = image.Quality;
            }

            GUILayout.Space(15);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

        private void DrawImageSpacers(int displayedImageCount)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space((m_PageSize - displayedImageCount) * m_ImageSpacerHeight);
            EditorGUILayout.EndVertical();
        }

        private void DrawPageField(int imageCount)
        {
            var lastPageIndex = Mathf.Max(imageCount - 1, 0) / m_PageSize;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);

            var labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.alignment = TextAnchor.MiddleLeft;

            EditorGUILayout.LabelField(string.Format("{0} Total Images", imageCount), labelStyle,
                GUILayout.Height(42), GUILayout.Width(100));

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField("Page", labelStyle, GUILayout.Height(42), GUILayout.Width(30));

            var textStyle = new GUIStyle(GUI.skin.textField);
            textStyle.margin = new RectOffset(0, 0, 15, 0);
            var pageString = EditorGUILayout.TextField((m_PageIndex + 1).ToString(), textStyle, GUILayout.Width(30));
            int pageNumber;
            int.TryParse(pageString, out pageNumber);
            m_PageIndex = Mathf.Clamp(pageNumber - 1, 0, lastPageIndex);

            var buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.margin = new RectOffset(10, 10, 13, 0);

            GUI.enabled = m_PageIndex > 0;
            bool moveLeft = GUILayout.Button("<", buttonStyle);
            GUI.enabled = m_PageIndex < lastPageIndex;
            bool moveRight = GUILayout.Button(">", buttonStyle);
            GUI.enabled = true;

            m_PageIndex = moveLeft ? m_PageIndex - 1 : m_PageIndex;
            m_PageIndex = moveRight ? m_PageIndex + 1 : m_PageIndex;

            GUILayout.Space(15);
            EditorGUILayout.EndHorizontal();
        }
    }
    /// @endcond
}
