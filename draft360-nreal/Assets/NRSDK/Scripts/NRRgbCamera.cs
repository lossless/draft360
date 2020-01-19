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
    using AOT;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /**
    * @brief Manage the life cycle of the entire rgbcamera.
    */
    public class NRRgbCamera
    {
        public delegate void CaptureEvent();
        public delegate void CaptureErrorEvent(string msg);
        public delegate void CaptureUpdateEvent(byte[] data);
        public static CaptureEvent OnImageUpdate;
        public static CaptureErrorEvent OnError;

        private static NativeCamera m_NativeCamera { get; set; }

        /// @cond EXCLUDE_FROM_DOXYGEN
        private static CameraImageFormat ImageFormat = CameraImageFormat.RGB_888;
        private static IntPtr TexturePtr = IntPtr.Zero;

        public static NativeResolution Resolution
        {
            get
            {
#if !UNITY_EDITOR
                NativeResolution resolution = NRDevice.Instance.NativeHMD.GetEyeResolution(NativeEye.RGB);
#else   
                NativeResolution resolution = new NativeResolution(1280, 720);
#endif
                return resolution;
            }
        }

        public static int FrameCount = 0;
        private static bool isRGBCamStart = false;
        private static bool isInitiate = false;

        public static bool IsRGBCamPlaying
        {
            get
            {
                return isRGBCamStart;
            }
        }

        public class FixedSizedQueue
        {
            private Queue<RGBRawDataFrame> m_Queue = new Queue<RGBRawDataFrame>();
            private object m_LockObj = new object();
            private ObjectPool m_ObjectPool;

            public FixedSizedQueue(ObjectPool pool)
            {
                m_ObjectPool = pool;
            }

            public int Limit { get; set; }

            public int Count
            {
                get
                {
                    return m_Queue.Count;
                }
            }

            public void Enqueue(RGBRawDataFrame obj)
            {
                lock (m_LockObj)
                {
                    m_Queue.Enqueue(obj);
                    if (m_Queue.Count > Limit)
                    {
                        var frame = m_Queue.Dequeue();
                        m_ObjectPool.Put<RGBRawDataFrame>(frame);
                    }
                }

            }

            public RGBRawDataFrame Dequeue()
            {
                lock (m_LockObj)
                {
                    var frame = m_Queue.Dequeue();
                    m_ObjectPool.Put<RGBRawDataFrame>(frame);
                    return frame;
                }
            }
        }

        private static FixedSizedQueue m_RGBFrames;

        public static ObjectPool FramePool;
        /// @endcond

        public static void Initialize()
        {
            if (isInitiate)
            {
                return;
            }
            NRDebugger.Log("[NRRgbCamera] Initialize");
            m_NativeCamera = new NativeCamera();
#if !UNITY_EDITOR
            m_NativeCamera.Create();
            m_NativeCamera.SetCaptureCallback(Capture);
#endif
            if (FramePool == null)
            {
                FramePool = new ObjectPool();
                FramePool.InitCount = 10;
            }
            if (m_RGBFrames == null)
            {
                m_RGBFrames = new FixedSizedQueue(FramePool);
                m_RGBFrames.Limit = 5;
            }

            isInitiate = true;
            SetImageFormat(CameraImageFormat.RGB_888);
        }

        private static void SetImageFormat(CameraImageFormat format)
        {
#if !UNITY_EDITOR
            m_NativeCamera.SetImageFormat(format);
#endif
            ImageFormat = format;
            NRDebugger.Log("[NRRgbCamera] SetImageFormat : " + format.ToString());
        }

        [MonoPInvokeCallback(typeof(NativeCamera.NRRGBCameraImageCallback))]
        public static void Capture(UInt64 rgb_camera_handle, UInt64 rgb_camera_image_handle, UInt64 userdata)
        {
            FrameCount++;
            int RawDataSize = 0;
            if (TexturePtr == IntPtr.Zero)
            {
                m_NativeCamera.GetRawData(rgb_camera_image_handle, ref TexturePtr, ref RawDataSize);
                m_NativeCamera.DestroyImage(rgb_camera_image_handle);

                NRDebugger.Log(string.Format("[NRRgbCamera] on first fram ready textureptr:{0} rawdatasize:{1} Resolution:{2}",
                   TexturePtr, RawDataSize, Resolution.ToString()));
                return;
            }

            m_NativeCamera.GetRawData(rgb_camera_image_handle, ref TexturePtr, ref RawDataSize);
            var timestamp = m_NativeCamera.GetHMDTimeNanos(rgb_camera_image_handle);
            QueueFrame(TexturePtr, RawDataSize, timestamp);

            if (OnImageUpdate != null)
            {
                OnImageUpdate();
            }
            m_NativeCamera.DestroyImage(rgb_camera_image_handle);
        }

        /**
        * @brief Start to play rgb camera.
        */
        public static void Play()
        {
            if (!isInitiate)
            {
                Initialize();
            }
            if (isRGBCamStart)
            {
                return;
            }
            NRDebugger.Log("[NRRgbCamera] Start to play");
#if !UNITY_EDITOR
            m_NativeCamera.StartCapture();
#endif
            isRGBCamStart = true;
        }

        public static bool HasFrame()
        {
            return isRGBCamStart && m_RGBFrames.Count > 0;
        }

        public static RGBRawDataFrame GetRGBFrame()
        {
            return m_RGBFrames.Dequeue();
        }

        private static void QueueFrame(IntPtr textureptr, int size, UInt64 timestamp)
        {
            if (!isRGBCamStart)
            {
                NRDebugger.LogError("rgb camera not stopped properly, it still sending data.");
                return;
            }
            RGBRawDataFrame frame = FramePool.Get<RGBRawDataFrame>();
            bool result = RGBRawDataFrame.MakeSafe(TexturePtr, size, timestamp, ref frame);
            if (result)
            {
                m_RGBFrames.Enqueue(frame);
            }
            else
            {
                FramePool.Put<RGBRawDataFrame>(frame);
            }
        }

        /**
        * @brief Stop the rgb camera.
        */
        public static void Stop()
        {
            if (!isRGBCamStart)
            {
                return;
            }
            NRDebugger.Log("[NRRgbCamera] Start to Stop");
#if !UNITY_EDITOR
            m_NativeCamera.StopCapture();
#endif
            isRGBCamStart = false;
        }

        /**
        * @brief Release the rgb camera.
        */
        public static void Release()
        {
            if (m_NativeCamera != null)
            {
                NRDebugger.Log("[NRRgbCamera] Start to Release");
#if !UNITY_EDITOR
                m_NativeCamera.Release();
                m_NativeCamera = null;
#endif
                OnError = null;
                OnImageUpdate = null;
                isInitiate = false;
                isRGBCamStart = false;
            }
        }

        private static void StateError(string msg)
        {
            if (OnError != null)
            {
                OnError(msg);
            }
        }
    }

    public struct RGBRawDataFrame
    {
        public UInt64 timeStamp;
        public byte[] data;

        unsafe public static bool MakeSafe(IntPtr textureptr, int size, UInt64 timestamp, ref RGBRawDataFrame frame)
        {
            if (textureptr == IntPtr.Zero || size <= 0)
            {
                return false;
            }
            if (frame.data == null || frame.data.Length != size)
            {
                frame.data = new byte[size];
            }
            frame.timeStamp = timestamp;
            Marshal.Copy(textureptr, frame.data, 0, size);
            return true;
        }

        public override string ToString()
        {
            return string.Format("timestamp :{0} data size:{1}", timeStamp, data.Length);
        }
    }
}
