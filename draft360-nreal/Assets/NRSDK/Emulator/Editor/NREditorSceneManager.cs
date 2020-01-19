/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.NREditor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System;

    public class NREditorSceneManager
    {
        private bool m_SceneInitialized;
        private bool m_UnloadUnusedAssets;
        private bool m_ApplyAppearance;
        private bool m_ApplyProperties;
        private static NREditorSceneManager m_Instance;
        private EditorApplication.CallbackFunction m_UpdateCallback;
        public static NREditorSceneManager Instance
        {
            get
            {
                if (NREditorSceneManager.m_Instance == null)
                {
                    Type typeFromHandle = typeof(NREditorSceneManager);
                    lock (typeFromHandle)
                    {
                        if (NREditorSceneManager.m_Instance == null)
                        {
                            NREditorSceneManager.m_Instance = new NREditorSceneManager();
                        }
                    }
                }
                return NREditorSceneManager.m_Instance;
            }
        }

        public bool SceneInitialized { get { return m_SceneInitialized; } }
        


        private NREditorSceneManager()
        {

            //return;
            m_UpdateCallback = new EditorApplication.CallbackFunction(EditorUpdate);
            if (EditorApplication.update == null || !EditorApplication.update.Equals(m_UpdateCallback))
            {
                EditorApplication.update = (EditorApplication.CallbackFunction)System.Delegate.Combine(EditorApplication.update, m_UpdateCallback);
            }
           
            m_SceneInitialized = false;
        }

        public void InitScene()
        {
            m_SceneInitialized = true;

        }

        public void EditorUpdate()
        {
            NRTrackableBehaviour[] trackables = GameObject.FindObjectsOfType<NRTrackableBehaviour>();
            if (m_ApplyAppearance)
            {
                UpdateTrackableAppearance(trackables);
                m_ApplyAppearance = false;
            }
            if (m_ApplyProperties)
            {
                UpdateTrackableProperties(trackables);
                m_ApplyProperties = false;
            }
            if (m_UnloadUnusedAssets)
            {
                Resources.UnloadUnusedAssets();
                m_UnloadUnusedAssets = false;
            }
        }
        private void UpdateTrackableAppearance(NRTrackableBehaviour[] trackables)
        {
            if (!Application.isPlaying)
            {
                for (int i = 0; i < trackables.Length; i++)
                {
                    NRTrackableBehaviour trackableBehaviour = trackables[i];
                    NRTrackableAccessor trackableAccessor = NRAccessorFactory.Create(trackableBehaviour);
                    if (trackableAccessor != null)
                    {
                        trackableAccessor.ApplyDataAppearance();
                    }
                }
            }
        }

        private void UpdateTrackableProperties(NRTrackableBehaviour[] trackables)
        {
            for (int i = 0; i < trackables.Length; i++)
            {
                NRTrackableBehaviour trackableBehaviour = trackables[i];
                NRTrackableAccessor trackableAccessor = NRAccessorFactory.Create(trackableBehaviour);
                if (trackableAccessor != null)
                {
                    trackableAccessor.ApplyDataProperties();
                }
            }
        }

        public void UnloadUnusedAssets()
        {
            m_UnloadUnusedAssets = true;
        }
    }
}