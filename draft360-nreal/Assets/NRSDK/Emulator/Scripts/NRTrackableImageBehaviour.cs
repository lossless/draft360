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
    using UnityEngine;

    public class NRTrackableImageBehaviour : NRTrackableBehaviour
    {
        [HideInInspector, SerializeField]
        private float m_AspectRatio;

        [HideInInspector, SerializeField]
        private float m_Width;

        [HideInInspector, SerializeField]
        private float m_Height;

        [HideInInspector, SerializeField]
        private string m_TrackingImageDatabase;

        private void Awake()
        {
#if !UNITY_EDITOR
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null) Destroy(meshRenderer);
            MeshFilter mesh = GetComponent<MeshFilter>();
            if (mesh != null) Destroy(mesh);
#endif
        }

#if UNITY_EDITOR
        private void Update()
        {
            float extent = transform.lossyScale.x * 1000;
            if (NREmulatorManager.Instance.IsInGameView(transform.position))
            {
                NREmulatorManager.Instance.NativeEmulatorApi.UpdateTrackableData<NRTrackableImage>
                (transform.position, transform.rotation, extent, extent, (uint)DatabaseIndex, TrackingState.Tracking);
            }
            else
            {
                NREmulatorManager.Instance.NativeEmulatorApi.UpdateTrackableData<NRTrackableImage>
                (transform.position, transform.rotation, extent, extent, (uint)DatabaseIndex, TrackingState.Stopped);
            }

        }
#endif
    }
}