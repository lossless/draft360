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
    using NRKernal;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;

    public class BlendCamera : IDisposable
    {
        public int Width;
        public int Height;
        private Camera m_TargetCamera;
        private IEncoder m_Encoder;
        private Material m_BlendMaterial;
        public RenderTexture BlendTexture;
        private BlendMode m_BlendMode;
        private Texture2D m_RGBOrigin;
        private RenderTexture m_RGBSource;
        public RenderTexture RGBTexture
        {
            get
            {
                return m_RGBSource;
            }
        }

        public RenderTexture VirtualTexture
        {
            get
            {
                return m_TargetCamera.targetTexture;
            }
        }

        private Texture2D m_TempCombineTex;

        Queue<AsyncGPUReadbackRequest> m_Requests;
        Queue<CaptureTask> m_Tasks;

        private int m_FrameCount;
        public int FrameCount
        {
            get { return m_FrameCount; }
            private set
            {
                m_FrameCount = value;
            }
        }

        public BlendCamera(IEncoder encoder, Camera camera, BlendMode mode, int width, int height)
        {
            Width = width;
            Height = height;
            m_TargetCamera = camera;
            m_Encoder = encoder;

            m_BlendMode = mode;

            switch (m_BlendMode)
            {
                case BlendMode.RGBOnly:
                    BlendTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
                    break;
                case BlendMode.VirtualOnly:
                    BlendTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
                    break;
                case BlendMode.Blend:
                    Shader blendshader;
                    if (QualitySettings.activeColorSpace == ColorSpace.Linear)
                    {
                        blendshader = Resources.Load<Shader>("Record/Shaders/AlphaBlendLinear");
                    }
                    else
                    {
                        blendshader = Resources.Load<Shader>("Record/Shaders/AlphaBlendGamma");
                    }
                    m_BlendMaterial = new Material(blendshader);
                    BlendTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
                    break;
                case BlendMode.WidescreenBlend:
                    BlendTexture = new RenderTexture(2 * width, height, 24, RenderTextureFormat.ARGB32);
                    m_RGBSource = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
                    m_TempCombineTex = new Texture2D(2 * width, height, TextureFormat.ARGB32, false);
                    break;
                default:
                    break;
            }

            m_TargetCamera.targetTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);

            m_Requests = new Queue<AsyncGPUReadbackRequest>();
            m_Tasks = new Queue<CaptureTask>();

            NRKernalUpdater.Instance.OnUpdate -= Update;
            NRKernalUpdater.Instance.OnUpdate += Update;
        }

        public void OnFrame(RGBTextureFrame frame)
        {
            m_RGBOrigin = frame.texture;
            // Render every camera
            m_TargetCamera.Render();

            switch (m_BlendMode)
            {
                case BlendMode.RGBOnly:
                    Graphics.Blit(frame.texture, BlendTexture);
                    break;
                case BlendMode.VirtualOnly:
                    Graphics.Blit(m_TargetCamera.targetTexture, BlendTexture);
                    break;
                case BlendMode.Blend:
                    m_BlendMaterial.SetTexture("_MainTex", m_TargetCamera.targetTexture);
                    m_BlendMaterial.SetTexture("_BcakGroundTex", frame.texture);
                    Graphics.Blit(m_TargetCamera.targetTexture, BlendTexture, m_BlendMaterial);
                    break;
                case BlendMode.WidescreenBlend:
                    CombineTexture(frame.texture, m_TargetCamera.targetTexture, m_TempCombineTex, BlendTexture);
                    break;
                default:
                    break;
            }

            // Commit frame                
            m_Encoder.Commit(BlendTexture, frame.timeStamp);
            FrameCount++;
        }

        public void Capture(CaptureTask task)
        {
            switch (task.BlendMode)
            {
                case BlendMode.RGBOnly:
                    CommitResult(m_RGBOrigin, task);
                    break;
                case BlendMode.VirtualOnly:
                    m_Requests.Enqueue(AsyncGPUReadback.Request(VirtualTexture));
                    m_Tasks.Enqueue(task);
                    break;
                case BlendMode.Blend:
                    m_Requests.Enqueue(AsyncGPUReadback.Request(BlendTexture));
                    m_Tasks.Enqueue(task);
                    break;
                case BlendMode.WidescreenBlend:
                    // Do not support..
                    Debug.Log("Do not support WidescreenBlend mode now...");
                    break;
                default:
                    break;
            }

        }

        // A temp texture for capture.
        private Texture2D tempTexture = null;
        private void Update()
        {
            while (m_Requests.Count > 0)
            {
                var req = m_Requests.Peek();
                var task = m_Tasks.Peek();

                if (req.hasError)
                {
                    NRDebugger.Log("GPU readback error detected");
                    m_Requests.Dequeue();

                    CommitResult(null, task);
                    m_Tasks.Dequeue();
                }
                else if (req.done)
                {
                    var buffer = req.GetData<Color32>();
                    if (tempTexture != null && tempTexture.width != Width && tempTexture.height != Height)
                    {
                        GameObject.Destroy(tempTexture);
                        tempTexture = null;
                    }
                    if (tempTexture == null)
                    {
                        tempTexture = new Texture2D(Width, Height, TextureFormat.RGB24, false);
                    }
                    tempTexture.SetPixels32(buffer.ToArray());
                    tempTexture.Apply();

                    if (task.OnReceive != null)
                    {
                        Texture2D resulttex;
                        if (tempTexture.width != task.Width || tempTexture.height != task.Height)
                        {
                            NRDebugger.LogFormat("[BlendCamera] need to scale the texture which origin width:{0} and out put width:{1}", Width, task.Width);
                            resulttex = ImageEncoder.ScaleTexture(tempTexture, task.Width, task.Height);
                            CommitResult(resulttex, task);

                            //Destroy the scale temp texture.
                            GameObject.Destroy(resulttex);
                        }
                        else
                        {
                            CommitResult(tempTexture, task);
                        }
                    }
                    m_Requests.Dequeue();
                    m_Tasks.Dequeue();
                }
                else
                {
                    break;
                }
            }
        }

        // Combine texture to double width tex.
        private void CombineTexture(Texture2D bgsource, RenderTexture foresource, Texture2D tempdest, RenderTexture dest)
        {
            //ulong timestart, timeend;
            //timestart = NRTools.GetTimeStamp();
            Graphics.Blit(bgsource, m_RGBSource);

            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = m_RGBSource;
            tempdest.ReadPixels(new Rect(0, 0, m_RGBSource.width, m_RGBSource.height), 0, 0);

            RenderTexture.active = foresource;
            tempdest.ReadPixels(new Rect(0, 0, foresource.width, foresource.height), foresource.width, 0);
            tempdest.Apply();
            RenderTexture.active = prev;
            //timeend = NRTools.GetTimeStamp();

            Graphics.Blit(tempdest, dest);
            //Debug.Log("Combine texture cost :" + (timeend - timestart));
        }

        private void CommitResult(Texture2D texture, CaptureTask task)
        {
            if (task.OnReceive == null)
            {
                return;
            }

            if (texture == null)
            {
                task.OnReceive(task, null);
                return;
            }

            byte[] result = null;
            switch (task.CaptureFormat)
            {
                case PhotoCaptureFileOutputFormat.JPG:
                    result = texture.EncodeToJPG();
                    break;
                case PhotoCaptureFileOutputFormat.PNG:
                    result = texture.EncodeToPNG();
                    break;
                default:
                    break;
            }
            task.OnReceive(task, result);
        }

        public void Dispose()
        {
            RenderTexture.active = null;
            if (BlendTexture != null) BlendTexture.Release();
            if (m_RGBSource != null) m_RGBSource.Release();

            GameObject.Destroy(m_TempCombineTex);
            BlendTexture = null;
            m_RGBSource = null;
            m_TempCombineTex = null;

            if (GameObject.FindObjectOfType<NRKernalUpdater>() != null)
            {
                NRKernalUpdater.Instance.OnUpdate -= Update;
            }
        }
    }
}
