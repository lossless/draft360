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
     * @brief Capture a image from the MR world.
     * 
     * You can capture a RGB only,Virtual only or Blended image through this class.
     */
    public class NRCaptureBehaviour : SingletonBehaviour<NRCaptureBehaviour>
    {
        public delegate void NRCaptureEvent();
        public NRCaptureEvent OnReady;
        public Transform RGBCameraRig;
        public Camera CaptureCamera;
        private NRRGBCamTexture m_RGBTexture;
        private BlendCamera m_CameraInput;
        private IEncoder m_Encoder;
        public Texture PreviewTexture
        {
            get
            {
                return m_CameraInput.BlendTexture;
            }
        }
        private bool m_IsInit = false;

        public Action OnResourceCreated;

        public static NRCaptureBehaviour Create(NRCaptureEvent onCreated = null)
        {
            if (Instance == null)
            {
                NRCaptureBehaviour capture = GameObject.FindObjectOfType<NRCaptureBehaviour>();
                var headParent = NRSessionManager.Instance.NRSessionBehaviour.transform.parent;
                if (capture == null)
                {
                    capture = Instantiate(Resources.Load<NRCaptureBehaviour>("Record/Prefabs/NRCaptureBehaviour"), headParent);
                }
                DontDestroyOnLoad(capture.gameObject);
                capture.OnReady += onCreated;
                return capture;
            }
            else
            {
                return Instance;
            }
        }

#if UNITY_EDITOR
        public Texture2D DefaultTexture;
#endif

        CameraParameters m_CameraParameters;

        private void Start()
        {
            if (isDirty)
            {
                return;
            }

#if UNITY_EDITOR
            DefaultTexture = Resources.Load<Texture2D>("Record/Textures/captureDefault");
            testFrame = new RGBTextureFrame();
            testFrame.texture = DefaultTexture;
#endif
            this.Init();
        }

        private void Init()
        {
            if (m_IsInit)
            {
                return;
            }

            m_Encoder = new ImageEncoder();
            m_RGBTexture = new NRRGBCamTexture();
            m_RGBTexture.OnUpdate += OnFrame;
            m_CameraInput = new BlendCamera(m_Encoder, CaptureCamera, BlendMode.Blend, m_RGBTexture.Width, m_RGBTexture.Height);

            m_IsInit = true;

            if (OnReady != null)
            {
                OnReady();
            }
        }

        private void OnFrame(RGBTextureFrame frame)
        {
            NRDebugger.Log("OnFrame");
            // update camera pose
            UpdateHeadPoseByTimestamp(frame.timeStamp);

            // commit a frame
            m_CameraInput.OnFrame(frame);
        }

#if UNITY_EDITOR
        RGBTextureFrame testFrame;
        private void Update()
        {
            if (m_CameraInput != null)
            {
                testFrame.timeStamp = NRTools.GetTimeStamp();

                m_CameraInput.OnFrame(testFrame);
            }
        }
#endif

        public void Play()
        {
            if (!m_IsInit)
            {
                return;
            }

            m_RGBTexture.Play();
        }

        public void Stop()
        {
            m_RGBTexture.Stop();
        }

        public void SetConfig(CameraParameters cameraparm)
        {
            m_CameraParameters = cameraparm;
        }

        public bool Do(int width, int height, BlendMode blendmode, PhotoCaptureFileOutputFormat format, string outpath)
        {
            var data = this.Do(width, height, blendmode, format);
            if (data == null)
            {
                return false;
            }
            File.WriteAllBytes(outpath, data);

            return true;
        }

        public bool Do(int width, int height, BlendMode blendmode, PhotoCaptureFileOutputFormat format, ref byte[] data)
        {
            data = this.Do(width, height, blendmode, format);
            if (data == null)
            {
                return false;
            }
            return true;
        }

        /**
        * @brief Capture a image Asyn.
        * 
        * if system supports AsyncGPUReadback, using AsyncGPUReadback to get the captured image.
        * else getting the image by synchronization way.
        */
        public void DoAsyn(CaptureTask task)
        {
            if (!m_IsInit)
            {
                return;
            }

            if (SystemInfo.supportsAsyncGPUReadback)
            {
                m_CameraInput.Capture(task);
            }
            else
            {
                var data = Do(task.Width, task.Height, task.BlendMode, task.CaptureFormat);
                if (task.OnReceive != null)
                {
                    task.OnReceive(task, data);
                }
            }
        }

        public void DoAsyn(NRPhotoCapture.OnCapturedToMemoryCallback oncapturedcallback)
        {
            if (!m_IsInit)
            {
                if (oncapturedcallback != null)
                {
                    var result = new NRPhotoCapture.PhotoCaptureResult();
                    result.resultType = NRPhotoCapture.CaptureResultType.UnknownError;
                    oncapturedcallback(result, null);
                }
                return;
            }
            var captureTask = new CaptureTask();
            captureTask.Width = m_CameraParameters.cameraResolutionWidth;
            captureTask.Height = m_CameraParameters.cameraResolutionHeight;
            captureTask.BlendMode = m_CameraParameters.blendMode;
            captureTask.CaptureFormat = m_CameraParameters.pixelFormat == CapturePixelFormat.PNG ? PhotoCaptureFileOutputFormat.PNG : PhotoCaptureFileOutputFormat.JPG;
            captureTask.OnReceive += (task, data) =>
            {
                if (oncapturedcallback != null)
                {
                    var result = new NRPhotoCapture.PhotoCaptureResult();
                    result.resultType = NRPhotoCapture.CaptureResultType.Success;
                    CapturePixelFormat format = task.CaptureFormat == PhotoCaptureFileOutputFormat.PNG ? CapturePixelFormat.PNG : CapturePixelFormat.JPEG;
                    PhotoCaptureFrame frame = new PhotoCaptureFrame(format, data);
                    oncapturedcallback(result, frame);
                }
            };

            this.DoAsyn(captureTask);
        }

        public void Do(string filename, PhotoCaptureFileOutputFormat fileOutputFormat)
        {
            if (!m_IsInit)
            {
                return;
            }

            this.Do(m_CameraParameters.cameraResolutionWidth,
                    m_CameraParameters.cameraResolutionHeight,
                    m_CameraParameters.blendMode,
                    fileOutputFormat,
                    filename
            );
        }

        private byte[] Do(int width, int height, BlendMode blendmode, PhotoCaptureFileOutputFormat format)
        {
            if (!m_IsInit)
            {
                return null;
            }
            byte[] data = null;
            RenderTexture pre = RenderTexture.active;
            RenderTexture targetRT;
            switch (blendmode)
            {
                case BlendMode.RGBOnly:
                    targetRT = m_CameraInput.RGBTexture;
                    break;
                case BlendMode.VirtualOnly:
                    targetRT = m_CameraInput.VirtualTexture;
                    break;
                case BlendMode.Blend:
                    targetRT = m_CameraInput.BlendTexture;
                    break;
                case BlendMode.WidescreenBlend:
                    Debug.LogError("Do not support WidescreenBlend mode.");
                    return null;
                default:
                    return null;
            }
            RenderTexture.active = targetRT;
            Texture2D texture2D = new Texture2D(targetRT.width, targetRT.height, TextureFormat.ARGB32, false);
            texture2D.ReadPixels(new Rect(0, 0, targetRT.width, targetRT.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = pre;

            Texture2D outPutTex = texture2D;
            Texture2D scaleTexture = null;

            // Scale the texture while the output width or height not equal to the targetRT.
            if (width != targetRT.width || height != targetRT.height)
            {
                scaleTexture = ImageEncoder.ScaleTexture(texture2D, width, height);
                outPutTex = scaleTexture;
            }

            switch (format)
            {
                case PhotoCaptureFileOutputFormat.JPG:
                    data = outPutTex.EncodeToJPG();
                    break;
                case PhotoCaptureFileOutputFormat.PNG:
                    data = outPutTex.EncodeToPNG();
                    break;
                default:
                    break;
            }

            // Clear the temp texture.
            Destroy(texture2D);
            if (scaleTexture != null)
            {
                Destroy(scaleTexture);
            }
            return data;
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
            if (m_CameraInput != null)
            {
                m_CameraInput.Dispose();
            }
            m_IsInit = false;
            DestroyImmediate(gameObject);
        }
    }
}
