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
    using System;
    using System.IO;
    using UnityEngine;
    using static NRKernal.NRHMDPoseTracker;

    /**
    * @brief Manages AR system state and handles the session lifecycle.
    * 
    * Through this class, application can create a session, configure it, start/pause or stop it. 
    */
    internal class NRSessionManager
    {
        private static NRSessionManager m_Instance;

        public static NRSessionManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new NRSessionManager();
                }
                return m_Instance;
            }
        }

        /**
         * Current lost tracking reason.
         */
        public LostTrackingReason LostTrackingReason
        {
            get
            {
                if (m_IsInitialized)
                {
                    return NativeAPI.NativeHeadTracking.GetTrackingLostReason();
                }
                else
                {
                    return LostTrackingReason.INITIALIZING;
                }
            }
        }

        public NRSessionBehaviour NRSessionBehaviour { get; set; }

        public NRHMDPoseTracker NRHMDPoseTracker { get; set; }

        public NativeInterface NativeAPI { get; set; }

        private NRRenderer RenderController { get; set; }

        public NRVirtualDisplayer VirtualDisplayer { get; set; }

        private bool m_IsInitialized = false;
        public bool IsInitialized
        {
            get
            {
                return m_IsInitialized;
            }
        }

        public void CreateSession(NRSessionBehaviour session)
        {
            if (IsInitialized || session == null)
            {
                return;
            }
            if (NRSessionBehaviour != null)
            {
                NRDebugger.LogError("Multiple SessionBehaviour components cannot exist in the scene. " +
                  "Destroying the newest.");
                GameObject.DestroyImmediate(session.gameObject);
                return;
            }
            NRDevice.Instance.Init();
            NativeAPI = new NativeInterface();
#if !UNITY_EDITOR_OSX
            NativeAPI.NativeTracking.Create();
#endif
            NRSessionBehaviour = session;

            NRHMDPoseTracker = session.GetComponent<NRHMDPoseTracker>();
            TrackingMode mode = NRHMDPoseTracker.TrackingMode == TrackingType.Tracking3Dof ? TrackingMode.MODE_3DOF : TrackingMode.MODE_6DOF;
            SetTrackingMode(mode);

            this.DeployData();

            NRKernalUpdater.Instance.OnUpdate += NRFrame.OnUpdate;
        }

        private void DeployData()
        {
            if (NRSessionBehaviour.SessionConfig == null)
            {
                return;
            }
            var database = NRSessionBehaviour.SessionConfig.TrackingImageDatabase;
            if (database == null)
            {
                NRDebugger.Log("augmented image data base is null!");
                return;
            }
            string deploy_path = database.TrackingImageDataOutPutPath;
            NRDebugger.Log("[TrackingImageDatabase] DeployData to path :" + deploy_path);
            ZipUtility.UnzipFile(database.RawData, deploy_path, NativeConstants.ZipKey);
        }

        public void SetConfiguration(NRSessionConfig config)
        {
            if (config == null)
            {
                return;
            }
#if !UNITY_EDITOR_OSX
            NativeAPI.Configration.UpdateConfig(config);
#endif
        }

        private void SetTrackingMode(TrackingMode mode)
        {
#if !UNITY_EDITOR_OSX
            NativeAPI.NativeTracking.SetTrackingMode(mode);
#endif
        }

        public void Recenter()
        {
            if (!m_IsInitialized)
            {
                return;
            }
#if !UNITY_EDITOR_OSX
            NativeAPI.NativeTracking.Recenter();
#endif
        }

        public static void SetAppSettings(bool useOptimizedRendering)
        {
            Application.targetFrameRate = 240;
            QualitySettings.maxQueuedFrames = -1;
            QualitySettings.vSyncCount = useOptimizedRendering ? 0 : 1;
            Screen.fullScreen = true;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        public void StartSession()
        {
            if (m_IsInitialized)
            {
                return;
            }

            var config = NRSessionBehaviour.SessionConfig;
            if (config != null)
            {
                SetAppSettings(config.OptimizedRendering);
#if !UNITY_EDITOR
                if (config.OptimizedRendering)
                {
                    if (NRSessionBehaviour.gameObject.GetComponent<NRRenderer>() == null)
                    {
                        RenderController = NRSessionBehaviour.gameObject.AddComponent<NRRenderer>();
                        RenderController.Initialize(NRHMDPoseTracker.leftCamera, NRHMDPoseTracker.rightCamera, NRHMDPoseTracker.GetHeadPose);
                    }
                }
#endif
            }
            else
            {
                SetAppSettings(false);
            }
#if !UNITY_EDITOR_OSX
            NativeAPI.NativeTracking.Start();

            NativeAPI.NativeHeadTracking.Create();
#endif

#if UNITY_EDITOR
            InitEmulator();
#endif
            m_IsInitialized = true;
        }

        public void DisableSession()
        {
            if (!m_IsInitialized)
            {
                return;
            }
#if !UNITY_EDITOR_OSX
            if (NRVirtualDisplayer.RunInBackground)
            {
                if (VirtualDisplayer != null)
                {
                    VirtualDisplayer.Pause();
                }
                NativeAPI.NativeTracking.Pause();
                if (RenderController != null)
                {
                    RenderController.Pause();
                }
            }
            else
            {
                NRDevice.Instance.ForceKill();
            }
#endif
        }

        public void ResumeSession()
        {
            if (!m_IsInitialized)
            {
                return;
            }
#if !UNITY_EDITOR_OSX
            NativeAPI.NativeTracking.Resume();
            if (RenderController != null)
            {
                RenderController.Resume();
            }
            if (VirtualDisplayer != null)
            {
                VirtualDisplayer.Resume();
            }
#endif
        }

        private bool m_IsDestroyed = false;
        public void DestroySession()
        {
            if (!m_IsInitialized && m_IsDestroyed)
            {
                return;
            }
            m_IsDestroyed = true;
#if !UNITY_EDITOR_OSX
            NativeAPI.NativeHeadTracking.Destroy();
            NativeAPI.NativeTracking.Destroy();
            if (VirtualDisplayer != null)
            {
                VirtualDisplayer.Destory();
            }
            if (RenderController != null)
            {
                RenderController.Destroy();
            }
            NRDevice.Instance.Destroy();
#endif
            if (GameObject.FindObjectOfType<NRKernalUpdater>() != null)
            {
                NRKernalUpdater.Instance.OnUpdate -= NRFrame.OnUpdate;
            }
            m_IsInitialized = false;
        }

        private void InitEmulator()
        {
            if (!NREmulatorManager.Inited && !GameObject.Find("NREmulatorManager"))
            {
                NREmulatorManager.Inited = true;
                GameObject.Instantiate(Resources.Load("Prefabs/NREmulatorManager"));
            }
            if (!GameObject.Find("NREmulatorHeadPos"))
            {
                GameObject.Instantiate(Resources.Load("Prefabs/NREmulatorHeadPose"));
            }
        }
    }
}
