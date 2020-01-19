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

    internal class NRImageTargetAccessor : NRTrackableAccessor
    {

        private readonly NRSerializedImageTarget m_SerializedObject;

        public NRImageTargetAccessor(NRTrackableImageBehaviour target)
        {
            m_Target = target;
            m_SerializedObject = new NRSerializedImageTarget(new SerializedObject(m_Target));
        }

        public override void ApplyDataAppearance()
        {
            NRTrackableImageEditor.UpdateAspectRatio(m_SerializedObject);
            NRTrackableImageEditor.UpdateMaterial(m_SerializedObject);
        }

        public override void ApplyDataProperties()
        {
            NRTrackableImageEditor.UpdateScale(m_SerializedObject);
        }
    }

    public struct ImageTargetData
    {
        public Vector2 Size;
        public string PreviewImage;
    }
}