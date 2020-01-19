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
    using UnityEngine.EventSystems;

    /// @cond EXCLUDE_FROM_DOXYGEN
    public abstract class EventCameraRaycaster : BaseRaycaster
    {
        private bool isDestroying = false;
        private Camera defaultCam;

        [SerializeField]
        private float nearDistance = 0f;
        [SerializeField]
        private float farDistance = 20f;

        public float NearDistance
        {
            get { return nearDistance; }
            set
            {
                nearDistance = Mathf.Max(0f, value);
                if (eventCamera != null)
                {
                    eventCamera.nearClipPlane = nearDistance;
                }
            }
        }

        public float FarDistance
        {
            get { return farDistance; }
            set
            {
                farDistance = Mathf.Max(0f, nearDistance, value);
                if (eventCamera != null)
                {
                    eventCamera.farClipPlane = farDistance;
                }
            }
        }

        public override Camera eventCamera
        {
            get
            {
                if (isDestroying)
                {
                    return null;
                }

                if (defaultCam == null)
                {
                    var go = new GameObject(name + " FallbackCamera");
                    go.SetActive(false);
                    go.transform.SetParent(EventSystem.current.transform, false);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;

                    defaultCam = go.AddComponent<Camera>();
                    defaultCam.clearFlags = CameraClearFlags.Nothing;
                    defaultCam.cullingMask = 0;
                    defaultCam.orthographic = true;
                    defaultCam.orthographicSize = 1;
                    defaultCam.useOcclusionCulling = false;
#if !(UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
                    defaultCam.stereoTargetEye = StereoTargetEyeMask.None;
#endif
                    defaultCam.nearClipPlane = nearDistance;
                    defaultCam.farClipPlane = farDistance;
                }

                return defaultCam;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            isDestroying = true;

            if (defaultCam != null)
            {
                Destroy(defaultCam);
                defaultCam = null;
            }
        }
    }
    /// @endcond
}
