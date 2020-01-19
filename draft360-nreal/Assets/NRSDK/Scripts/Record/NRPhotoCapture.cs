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
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class NRPhotoCapture : IDisposable
    {
        ~NRPhotoCapture()
        {

        }

        private static IEnumerable<Resolution> m_SupportedResolutions;

        //
        // 摘要:
        //     A list of all the supported device resolutions for taking pictures.
        public static IEnumerable<Resolution> SupportedResolutions
        {
            get
            {
                if (m_SupportedResolutions == null)
                {
                    var resolutions = new List<Resolution>();
                    var resolution = new Resolution();
                    resolution.width = NRRgbCamera.Resolution.width;
                    resolution.height = NRRgbCamera.Resolution.height;
                    resolutions.Add(resolution);
                    m_SupportedResolutions = resolutions;
                }
                return m_SupportedResolutions;
            }
        }

        public NRCaptureBehaviour CaptureBehaviour;

        public static void CreateAsync(bool showHolograms, OnCaptureResourceCreatedCallback onCreatedCallback)
        {
            NRPhotoCapture photocapture = new NRPhotoCapture();
            photocapture.CaptureBehaviour = NRCaptureBehaviour.Create(() =>
            {
                if (onCreatedCallback != null)
                {
                    onCreatedCallback(photocapture);
                }
            });

        }

        //
        // 摘要:
        //     Dispose must be called to shutdown the PhotoCapture instance.
        public void Dispose()
        {
            if (CaptureBehaviour != null)
            {
                CaptureBehaviour.Release();
            }
        }
        //
        // 摘要:
        //     Provides a COM pointer to the native IVideoDeviceController.
        //
        // 返回结果:
        //     A native COM pointer to the IVideoDeviceController.
        public IntPtr GetUnsafePointerToVideoDeviceController()
        {
            return IntPtr.Zero;
        }

        public void StartPhotoModeAsync(CameraParameters setupParams, OnPhotoModeStartedCallback onPhotoModeStartedCallback)
        {
            CaptureBehaviour.SetConfig(setupParams);
            CaptureBehaviour.Play();
            PhotoCaptureResult result = new PhotoCaptureResult();
            result.resultType = CaptureResultType.Success;
            if (onPhotoModeStartedCallback != null)
            {
                onPhotoModeStartedCallback(result);
            }
        }

        public void StopPhotoModeAsync(OnPhotoModeStoppedCallback onPhotoModeStoppedCallback)
        {
            CaptureBehaviour.Stop();

            PhotoCaptureResult result = new PhotoCaptureResult();
            result.resultType = CaptureResultType.Success;
            if (onPhotoModeStoppedCallback != null)
            {
                onPhotoModeStoppedCallback(result);
            }
        }

        public void TakePhotoAsync(string filename, PhotoCaptureFileOutputFormat fileOutputFormat, OnCapturedToDiskCallback onCapturedPhotoToDiskCallback)
        {
            CaptureBehaviour.Do(filename, fileOutputFormat);
        }

        public void TakePhotoAsync(OnCapturedToMemoryCallback onCapturedPhotoToMemoryCallback)
        {
            CaptureBehaviour.DoAsyn(onCapturedPhotoToMemoryCallback);
        }

        //
        // 摘要:
        //     Contains the result of the capture request.
        public enum CaptureResultType
        {
            //
            // 摘要:
            //     Specifies that the desired operation was successful.
            Success = 0,
            //
            // 摘要:
            //     Specifies that an unknown error occurred.
            UnknownError = 1
        }

        //
        // 摘要:
        //     A data container that contains the result information of a photo capture operation.
        public struct PhotoCaptureResult
        {
            //
            // 摘要:
            //     A generic result that indicates whether or not the PhotoCapture operation succeeded.
            public CaptureResultType resultType;
            //
            // 摘要:
            //     The specific HResult value.
            public long hResult;

            //
            // 摘要:
            //     Indicates whether or not the operation was successful.
            public bool success { get; }
        }

        //
        // 摘要:
        //     Called when a PhotoCapture resource has been created.
        //
        // 参数:
        //   captureObject:
        //     The PhotoCapture instance.
        public delegate void OnCaptureResourceCreatedCallback(NRPhotoCapture captureObject);
        //
        // 摘要:
        //     Called when photo mode has been started.
        //
        // 参数:
        //   result:
        //     Indicates whether or not photo mode was successfully activated.
        public delegate void OnPhotoModeStartedCallback(PhotoCaptureResult result);
        //
        // 摘要:
        //     Called when photo mode has been stopped.
        //
        // 参数:
        //   result:
        //     Indicates whether or not photo mode was successfully deactivated.
        public delegate void OnPhotoModeStoppedCallback(PhotoCaptureResult result);
        //
        // 摘要:
        //     Called when a photo has been saved to the file system.
        //
        // 参数:
        //   result:
        //     Indicates whether or not the photo was successfully saved to the file system.
        public delegate void OnCapturedToDiskCallback(PhotoCaptureResult result);
        //
        // 摘要:
        //     Called when a photo has been captured to memory.
        //
        // 参数:
        //   result:
        //     Indicates whether or not the photo was successfully captured to memory.
        //
        //   photoCaptureFrame:
        //     Contains the target texture. If available, the spatial information will be accessible
        //     through this structure as well.
        public delegate void OnCapturedToMemoryCallback(PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame);
    }
}
