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
    public class NRTrackablePlaneBehaviour : NRTrackableBehaviour
    {
        private void Start()
        {
#if UNITY_EDITOR
            DatabaseIndex = NREmulatorManager.SIMPlaneID;
            NREmulatorManager.SIMPlaneID++;
#endif

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
                NREmulatorManager.Instance.NativeEmulatorApi.UpdateTrackableData<NRTrackablePlane>
                (transform.position, transform.rotation, extent, extent, (uint)DatabaseIndex, TrackingState.Tracking);
            }
            else
            {
                NREmulatorManager.Instance.NativeEmulatorApi.UpdateTrackableData<NRTrackablePlane>
                (transform.position, transform.rotation, extent, extent, (uint)DatabaseIndex, TrackingState.Stopped);
            }
        }
#endif
    }
}