/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    using UnityEngine;
    using NRKernal;
    using System;
    using System.IO;

    /**
    * @brief Record video from the MR world.
    * 
    * You can record a RGB only,Virtual only or Blended image through this class.
    */
    public class NRRecordBehaviour : SingletonBehaviour<NRRecordBehaviour>
    {
        public delegate void NRRecordEvent();
        public NRRecordEvent OnReady;
        public Transform RGBCameraRig;
        public Camera CaptureCamera;
        private NRRGBCamTexture m_RGBTexture;
        private BlendCamera m_BlendCamera;
        private VideoEncoder m_Encoder;
        public Texture PreviewTexture
        {
            get
            {
                return m_BlendCamera.BlendTexture;
            }
        }
        private BlendMode m_BlendMode;
        private bool m_IsInit = false;

        public static NRRecordBehaviour Create(NRRecordEvent onCreated = null)
        {
            if (Instance == null)
            {
                NRRecordBehaviour recordcontroller = GameObject.FindObjectOfType<NRRecordBehaviour>();
                var headParent = NRSessionManager.Instance.NRSessionBehaviour.transform.parent;
                if (recordcontroller == null)
                {
                    recordcontroller = Instantiate(Resources.Load<NRRecordBehaviour>("Record/Prefabs/NRRecorderBehaviour"), headParent);
                }
                recordcontroller.OnReady += onCreated;
                return recordcontroller;
            }
            else
            {
                return Instance;
            }
        }

        private void Start()
        {
            if (isDirty) return;
            this.Init();
        }

        private void Init()
        {
            if (m_IsInit)
            {
                return;
            }

            m_Encoder = new VideoEncoder();
            m_RGBTexture = new NRRGBCamTexture();
            m_RGBTexture.OnUpdate += OnFrame;

            m_IsInit = true;

            if (OnReady != null)
            {
                OnReady();
            }
        }

        private void OnFrame(RGBTextureFrame frame)
        {
            // Update camera pose
            UpdateHeadPoseByTimestamp(frame.timeStamp);

            // Commit a frame
            m_BlendCamera.OnFrame(frame);
        }

        public void StartRecord()
        {
            if (!m_IsInit)
            {
                return;
            }
            m_RGBTexture.Play();
            m_Encoder.Start();
        }

        public void StopRecord()
        {
            if (!m_IsInit)
            {
                return;
            }
            m_RGBTexture.Stop();
            m_Encoder.Stop();
        }

        public void SetConfigration(CameraParameters cameraparam)
        {
            NativeEncodeConfig config = new NativeEncodeConfig(cameraparam);
            m_BlendMode = cameraparam.blendMode;
            if (m_BlendCamera == null)
            {
                m_BlendCamera = new BlendCamera(m_Encoder, CaptureCamera, m_BlendMode, config.width, config.height);
            }

            m_Encoder.SetConfig(config);
        }

        public void SetOutPutPath(string path)
        {
            m_Encoder.EncodeConfig.SetOutPutPath(path);
        }

        private void UpdateHeadPoseByTimestamp(UInt64 timestamp, UInt64 predict = 0)
        {
            Pose head_pose = Pose.identity;
            var result = NRFrame.GetHeadPoseByTime(ref head_pose, timestamp, predict);
            if (result)
            {
                RGBCameraRig.transform.localPosition = head_pose.position;
                RGBCameraRig.transform.localRotation = head_pose.rotation;
            }
        }

        public void Release()
        {
            if (m_RGBTexture != null)
            {
                m_RGBTexture.Stop();
                m_RGBTexture.Dispose();
            }
            if (m_BlendCamera != null)
            {
                m_BlendCamera.Dispose();
            }
            if (m_Encoder != null)
            {
                m_Encoder.Destory();
            }
            this.m_IsInit = false;
            DestroyImmediate(gameObject);
        }
    }
}
