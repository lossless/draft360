using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.NRExamples
{
    public class TrackingInfomationTips : SingletonBehaviour<TrackingInfomationTips>
    {
        private Dictionary<TipType, GameObject> m_TipsDict = new Dictionary<TipType, GameObject>();
        public enum TipType
        {
            UnInitialized,
            LostTracking,
            None
        }
        private GameObject m_CurrentTip;

        private int m_LeftCullingMask;
        private int m_RightCullingMask;
        private Camera leftCamera;
        private Camera rightCamera;
        private Camera centerCamera;

        void Start()
        {
            leftCamera = NRSessionManager.Instance.NRHMDPoseTracker.leftCamera;
            rightCamera = NRSessionManager.Instance.NRHMDPoseTracker.rightCamera;
            centerCamera = NRSessionManager.Instance.NRHMDPoseTracker.centerCamera;
        }

        private void OnEnable()
        {
            NRHMDPoseTracker.OnHMDLostTracking += OnHMDLostTracking;
            NRHMDPoseTracker.OnHMDPoseReady += OnHMDPoseReady;
        }

        private void OnDisable()
        {
            NRHMDPoseTracker.OnHMDLostTracking -= OnHMDLostTracking;
            NRHMDPoseTracker.OnHMDPoseReady -= OnHMDPoseReady;
        }

        private void OnHMDPoseReady()
        {
            Debug.Log("[NRHMDPoseTracker] OnHMDPoseReady");
        }

        private void OnHMDLostTracking()
        {
            Debug.Log("[NRHMDPoseTracker] OnHMDLostTracking");
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private void Update()
        {
            if (NRFrame.SessionStatus == SessionState.UnInitialize)
            {
                ShowTips(TipType.UnInitialized);
            }
            else if (NRFrame.SessionStatus == SessionState.LostTracking)
            {
                ShowTips(TipType.LostTracking);
            }
            else
            {
                ShowTips(TipType.None);
            }
    }
#endif

        public void ShowTips(TipType type)
        {
            switch (type)
            {
                case TipType.UnInitialized:
                case TipType.LostTracking:
                    GameObject go;
                    m_TipsDict.TryGetValue(type, out go);

                    if (go == m_CurrentTip && go != null)
                    {
                        return;
                    }

                    if (go != null)
                    {
                        go.SetActive(true);
                    }
                    else
                    {
                        go = Instantiate(Resources.Load(type.ToString() + "Tip"), centerCamera.transform) as GameObject;
                        m_TipsDict.Add(type, go);
                    }

                    if (m_CurrentTip != null)
                    {
                        m_CurrentTip.SetActive(false);
                    }
                    m_CurrentTip = go;
                    break;
                case TipType.None:
                    if (m_CurrentTip != null)
                    {
                        m_CurrentTip.SetActive(false);
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetCameraByTrackingStatus(bool isopen)
        {
            if (isopen)
            {
                leftCamera.cullingMask = m_LeftCullingMask;
                rightCamera.cullingMask = m_RightCullingMask;
            }
            else
            {
                leftCamera.cullingMask = 0;
                rightCamera.cullingMask = 0;
            }
        }

        new void OnDestroy()
        {
            if (isDirty) return;
            if (m_TipsDict != null)
            {
                foreach (var item in m_TipsDict)
                {
                    if (item.Value != null)
                    {
                        GameObject.Destroy(item.Value);
                    }
                }
            }
        }
    }
}