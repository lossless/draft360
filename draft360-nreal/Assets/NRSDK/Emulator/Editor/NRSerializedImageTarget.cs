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
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class NRSerializedImageTarget : NRSerializedTrackable
    {
        private readonly SerializedProperty m_AspectRatio;
        private readonly SerializedProperty m_Width;
        private readonly SerializedProperty m_Height;
        private readonly SerializedProperty m_TrackingImageDatabase;
        private readonly SerializedProperty m_DatabaseIndex;


        public SerializedProperty AspectRatioProperty { get { return m_AspectRatio; } }
        public float AspectRatio { get { return m_AspectRatio.floatValue; } set { m_AspectRatio.floatValue = value; } }

        public SerializedProperty WidthProperty { get { return m_Width; } }
        public float Width { get { return m_Width.floatValue; } set { m_Width.floatValue = value; } }

        public SerializedProperty HeightProperty { get { return m_Height; } }
        public float Height { get { return m_Height.floatValue; } set { m_Height.floatValue = value; } }

        public SerializedProperty TrackingImageDatabaseProperty { get { return m_TrackingImageDatabase; } }
        public string TrackingImageDatabase { get { return m_TrackingImageDatabase.stringValue; } set { m_TrackingImageDatabase.stringValue = value; } }


        public SerializedProperty DatabaseIndexProperty { get { return m_DatabaseIndex; } }
        public int DatabaseIndex { get { return m_DatabaseIndex.intValue; } set { m_DatabaseIndex.intValue = value; } }


        public NRSerializedImageTarget(SerializedObject target) : base(target)
        {
            m_AspectRatio = target.FindProperty("m_AspectRatio");
            m_Width = target.FindProperty("m_Width");
            m_Height = target.FindProperty("m_Height");
            m_DatabaseIndex = target.FindProperty("m_DatabaseIndex");
        }

        public List<NRTrackableImageBehaviour> GetBehaviours()
        {
            List<NRTrackableImageBehaviour> list = new List<NRTrackableImageBehaviour>();
            Object[] targetObjs = m_SerializedObject.targetObjects;
            for (int i = 0; i < targetObjs.Length; i++)
            {
                list.Add((NRTrackableImageBehaviour)targetObjs[i]);
            }
            return list;
        }
    }
}