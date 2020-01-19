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

    public class NRSerializedTrackable
    {

        protected readonly SerializedObject m_SerializedObject;
        private readonly SerializedProperty m_TrackableName;
        private readonly SerializedProperty m_InitializedInEditor;

        public SerializedObject SerializedObject { get { return m_SerializedObject; } }
        public SerializedProperty TrackableNameProperty { get { return m_TrackableName; } }
        public string TrackableName { get { return m_TrackableName.stringValue; } set { m_TrackableName.stringValue = value; } }
        public SerializedProperty InitInEditorPreperty { get { return m_InitializedInEditor; } }
        public bool InitializedInEditor { get { return m_InitializedInEditor.boolValue; } set { m_InitializedInEditor.boolValue = value; } }


        public NRSerializedTrackable(SerializedObject target)
        {
            m_SerializedObject = target;
            m_TrackableName = m_SerializedObject.FindProperty("m_TrackableName");
            m_InitializedInEditor = m_SerializedObject.FindProperty("m_InitializedInEditor");
        }

        public Material GetMaterial()
        {
            return ((MonoBehaviour)m_SerializedObject.targetObject).GetComponent<Renderer>().sharedMaterial;
        }

        public Material[] GetMaterials()
        {
            return ((MonoBehaviour)m_SerializedObject.targetObject).GetComponent<Renderer>().sharedMaterials;
        }


        public void SetMaterial(Material material)
        {
            object[] targetObjs = m_SerializedObject.targetObjects;
            for (int i = 0; i < targetObjs.Length; i++)
            {
                ((MonoBehaviour)targetObjs[i]).GetComponent<Renderer>().sharedMaterial = material;
            }
            NREditorSceneManager.Instance.UnloadUnusedAssets();
        }

        public void SetMaterial(Material[] materials)
        {
            object[] targetObjs = m_SerializedObject.targetObjects;
            for (int i = 0; i < targetObjs.Length; i++)
            {
                ((MonoBehaviour)targetObjs[i]).GetComponent<Renderer>().sharedMaterials = materials;
            }
            NREditorSceneManager.Instance.UnloadUnusedAssets();
        }

        public List<GameObject> GetGameObjects()
        {
            List<GameObject> list = new List<GameObject>();
            object[] targetObjs = m_SerializedObject.targetObjects;
            for (int i = 0; i < targetObjs.Length; i++)
            {
                list.Add(((MonoBehaviour)targetObjs[i]).gameObject);
            }
            return list;
        }
    }
}