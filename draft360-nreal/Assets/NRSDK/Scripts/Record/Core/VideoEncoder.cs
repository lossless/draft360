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
    using System;
    using AOT;
    using System.Runtime.InteropServices;

    public class VideoEncoder : IEncoder
    {
        public static NativeEncoder NativeEncoder { get; set; }
        private bool m_IsStarted = false;

        public NativeEncodeConfig EncodeConfig;
        private delegate void RenderEventDelegate(int eventID);
        private static RenderEventDelegate RenderThreadHandle = new RenderEventDelegate(RunOnRenderThread);
        private static IntPtr RenderThreadHandlePtr = Marshal.GetFunctionPointerForDelegate(RenderThreadHandle);

        private const int STARTENCODEEVENT = 0x0001;

        public VideoEncoder(int width, int height, int bitRate, int fps, CodecType codectype, string path)
        {
            EncodeConfig = new NativeEncodeConfig(width, height, bitRate, fps, codectype, path);
            NativeEncoder = new NativeEncoder();
            NativeEncoder.Create();
        }

        public VideoEncoder()
        {
            NativeEncoder = new NativeEncoder();
            NativeEncoder.Create();
        }

        [MonoPInvokeCallback(typeof(RenderEventDelegate))]
        private static void RunOnRenderThread(int eventID)
        {
            if (eventID == STARTENCODEEVENT)
            {
                NativeEncoder.Start();
            }
        }

        public void SetConfig(NativeEncodeConfig config)
        {
            EncodeConfig = config;
        }

        public void Start()
        {
            if (m_IsStarted)
            {
                return;
            }

            NativeEncoder.SetConfigration(EncodeConfig);
            GL.IssuePluginEvent(RenderThreadHandlePtr, STARTENCODEEVENT);
            m_IsStarted = true;
        }

        public void Commit(RenderTexture rt, UInt64 timestamp)
        {
            if (!m_IsStarted)
            {
                return;
            }
            NativeEncoder.UpdateSurface(rt.GetNativeTexturePtr(), timestamp);
        }

        public void Stop()
        {
            if (!m_IsStarted)
            {
                return;
            }
            NativeEncoder.Stop();
            m_IsStarted = false;
        }

        public void Destory()
        {
            NativeEncoder.Destroy();
        }
    }
}
