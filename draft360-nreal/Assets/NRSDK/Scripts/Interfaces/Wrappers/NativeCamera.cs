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
    using System.Runtime.InteropServices;

    /**
    * @brief Session Native API.
    */
    internal partial class NativeCamera
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void NRRGBCameraImageCallback(UInt64 rgb_camera_handle,
               UInt64 rgb_camera_image_handle, UInt64 userdata);

        private UInt64 m_NativeCameraHandle;

        public bool Create()
        {
            var result = NativeApi.NRRGBCameraCreate(ref m_NativeCameraHandle);
            NRDebugger.Log("[NativeCamera] Create: " + result.ToString());
            return result == NativeResult.Success;
        }

        public bool GetRawData(UInt64 imageHandle, ref IntPtr ptr, ref int size)
        {
            uint data_size = 0;
            var result = NativeApi.NRRGBCameraImageGetRawData(m_NativeCameraHandle, imageHandle, ref ptr, ref data_size);
            size = (int)data_size;
            return result == NativeResult.Success;
        }

        public NativeResolution GetResolution(UInt64 imageHandle)
        {
            NativeResolution resolution = new NativeResolution(0, 0);
            var result = NativeApi.NRRGBCameraImageGetResolution(m_NativeCameraHandle, imageHandle, ref resolution);
            NRDebugger.Log("[NativeCamera] GetResolution: " + result.ToString());
            return resolution;
        }

        public UInt64 GetHMDTimeNanos(UInt64 imageHandle)
        {
            UInt64 time = 0;
            NativeApi.NRRGBCameraImageGetHMDTimeNanos(m_NativeCameraHandle, imageHandle, ref time);
            return time;
        }

        public bool SetCaptureCallback(NRRGBCameraImageCallback callback, UInt64 userdata = 0)
        {
            var result = NativeApi.NRRGBCameraSetCaptureCallback(m_NativeCameraHandle, callback, userdata);
            NRDebugger.Log("[NativeCamera] SetCaptureCallback: " + result.ToString());
            return result == NativeResult.Success;
        }

        public bool SetImageFormat(CameraImageFormat format)
        {
            var result = NativeApi.NRRGBCameraSetImageFormat(m_NativeCameraHandle, format);
            NRDebugger.Log("[NativeCamera] SetImageFormat: " + result.ToString());
            return result == NativeResult.Success;
        }

        public bool StartCapture()
        {
            var result = NativeApi.NRRGBCameraStartCapture(m_NativeCameraHandle);
            NRDebugger.Log("[NativeCamera] StartCapture: " + result.ToString());
            return result == NativeResult.Success;
        }

        public bool StopCapture()
        {
            var result = NativeApi.NRRGBCameraStopCapture(m_NativeCameraHandle);
            NRDebugger.Log("[NativeCamera] StopCapture: " + result.ToString());
            return result == NativeResult.Success;
        }

        public bool DestroyImage(UInt64 imageHandle)
        {
            var result = NativeApi.NRRGBCameraImageDestroy(m_NativeCameraHandle, imageHandle);
            return result == NativeResult.Success;
        }

        public bool Release()
        {
            var result = NativeApi.NRRGBCameraDestroy(m_NativeCameraHandle);
            NRDebugger.Log("[NativeCamera] Release: " + result.ToString());
            return result == NativeResult.Success;
        }

        private struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraImageGetRawData(UInt64 rgb_camera_handle,
                UInt64 rgb_camera_image_handle, ref IntPtr out_image_raw_data, ref UInt32 out_image_raw_data_size);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraImageGetResolution(UInt64 rgb_camera_handle,
                UInt64 rgb_camera_image_handle, ref NativeResolution out_image_resolution);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraImageGetHMDTimeNanos(
                UInt64 rgb_camera_handle, UInt64 rgb_camera_image_handle,
                ref UInt64 out_image_hmd_time_nanos);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraCreate(ref UInt64 out_rgb_camera_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraDestroy(UInt64 rgb_camera_handle);

            [DllImport(NativeConstants.NRNativeLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern NativeResult NRRGBCameraSetCaptureCallback(
                UInt64 rgb_camera_handle, NRRGBCameraImageCallback image_callback, UInt64 userdata);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraSetImageFormat(
                UInt64 rgb_camera_handle, CameraImageFormat format);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraStartCapture(UInt64 rgb_camera_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraStopCapture(UInt64 rgb_camera_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraImageDestroy(UInt64 rgb_camera_handle,
                UInt64 rgb_camera_image_handle);
        };
    }
}
