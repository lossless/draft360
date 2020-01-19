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

    /**
    * @brief HMDPoseTracker update the infomations of  pose tracker.
    * 
    * This component is used to initialize the camera parameter, update the device posture, 
    * In addition, application can change TrackingType through this component.
    */
    public class NRHMDPoseTracker : MonoBehaviour
    {
        internal delegate void HMDPoseTrackerEvent();
        internal static event HMDPoseTrackerEvent OnHMDPoseReady;
        internal static event HMDPoseTrackerEvent OnHMDLostTracking;

        /**
        * @brief HMD tracking type
        */
        public enum TrackingType
        {
            /**
            * Track the position an rotation.
            */
            Tracking6Dof = 0,

            /**
            * Track the rotation only.
            */
            Tracking3Dof = 1,
        }

        [SerializeField]
        private TrackingType m_TrackingType;

        public TrackingType TrackingMode
        {
            get
            {
                return m_TrackingType;
            }
        }

        /**
        * Use relative coordinates or not.
        */
        public bool UseRelative = false;
        private LostTrackingReason m_LastReason = LostTrackingReason.INITIALIZING;

        public Camera leftCamera;
        public Camera centerCamera;
        public Camera rightCamera;

        private bool isInited = false;
        private bool isReady = false;

        private void Awake()
        {
#if UNITY_EDITOR
            leftCamera.cullingMask = 0;
            rightCamera.cullingMask = 0;
            centerCamera.cullingMask = -1;
            centerCamera.depth = 1;
#endif
        }

        void Init()
        {
#if !UNITY_EDITOR
            bool result;
            var matrix_data = NRFrame.GetEyeProjectMatrix(out result, leftCamera.nearClipPlane, leftCamera.farClipPlane);
            if (result)
            {
                leftCamera.projectionMatrix = matrix_data.LEyeMatrix;
                rightCamera.projectionMatrix = matrix_data.REyeMatrix;

                var eyeposFromHead = NRFrame.EyePosFromHead;
                leftCamera.transform.localPosition = eyeposFromHead.LEyePose.position;
                leftCamera.transform.localRotation = eyeposFromHead.LEyePose.rotation;
                rightCamera.transform.localPosition = eyeposFromHead.REyePose.position;
                rightCamera.transform.localRotation = eyeposFromHead.REyePose.rotation;
                centerCamera.transform.localPosition = (leftCamera.transform.localPosition + rightCamera.transform.localPosition) * 0.5f;
                centerCamera.transform.localRotation = Quaternion.Lerp(leftCamera.transform.localRotation, rightCamera.transform.localRotation, 0.5f);

                isInited = true;
            }
#else
            isInited = true;
#endif
        }

        void Update()
        {
            if (NRSessionManager.Instance.IsInitialized && !isInited)
            {
                this.Init();
            }

            CheckHMDPoseState();
            UpdatePoseByTrackingType();
        }

        /**
         * @brief Get the real pose of device in unity world coordinate by "UseRelative".
         * @return Real pose of device.
         */
        public void GetHeadPose(ref Pose pose)
        {
            if (!NRSessionManager.Instance.IsInitialized)
            {
                pose.position = Vector3.zero;
                pose.rotation = Quaternion.identity;
                return;
            }
            var poseTracker = NRSessionManager.Instance.NRHMDPoseTracker;
            pose.position = poseTracker.UseRelative ? gameObject.transform.localPosition : gameObject.transform.position;
            pose.rotation = poseTracker.UseRelative ? gameObject.transform.localRotation : gameObject.transform.rotation;
        }

        private void UpdatePoseByTrackingType()
        {
            Pose pose = NRFrame.HeadPose;

            // update pos
            switch (m_TrackingType)
            {
                case TrackingType.Tracking6Dof:
                    if (UseRelative)
                    {
                        transform.localRotation = pose.rotation;
                        transform.localPosition = pose.position;
                    }
                    else
                    {
                        transform.rotation = pose.rotation;
                        transform.position = pose.position;
                    }
                    break;
                case TrackingType.Tracking3Dof:
                    if (UseRelative)
                    {
                        transform.localRotation = pose.rotation;
                    }
                    else
                    {
                        transform.rotation = pose.rotation;
                    }
                    break;
                default:
                    break;
            }

            centerCamera.transform.localPosition = (leftCamera.transform.localPosition + rightCamera.transform.localPosition) * 0.5f;
            centerCamera.transform.localRotation = Quaternion.Lerp(leftCamera.transform.localRotation, rightCamera.transform.localRotation, 0.5f);
        }

        private void CheckHMDPoseState()
        {
            if (!NRSessionManager.Instance.IsInitialized || !isInited)
            {
                return;
            }
            var currentReason = NRFrame.LostTrackingReason;
            if (m_LastReason == LostTrackingReason.NONE && currentReason != LostTrackingReason.NONE && isReady)
            {
                if (OnHMDLostTracking != null)
                {
                    OnHMDLostTracking.Invoke();
                }
            }
            if (m_LastReason != LostTrackingReason.NONE && currentReason == LostTrackingReason.NONE && !isReady)
            {
                if (OnHMDPoseReady != null)
                {
                    OnHMDPoseReady.Invoke();
                }
                isReady = true;
            }

            m_LastReason = currentReason;
        }
    }
}
