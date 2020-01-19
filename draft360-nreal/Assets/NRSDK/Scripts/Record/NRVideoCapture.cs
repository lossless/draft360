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
    using System.Collections.Generic;

    /**
     * @brief Records a video from the MR images directly to disk.
     * 
     * MR images comes from rgb camera or rgb camera image and virtual image blending.
     * The final video recording will be stored on the file system in the MP4 format.
     */
    public class NRVideoCapture : IDisposable
    {
        public NRVideoCapture()
        {
            IsRecording = false;
        }

        ~NRVideoCapture()
        {

        }

        private static IEnumerable<Resolution> m_SupportedResolutions = null;

        /**
         * A list of all the supported device resolutions for recording videos.
         */
        public static IEnumerable<Resolution> SupportedResolutions
        {
            get
            {
                if (m_SupportedResolutions == null)
                {
                    var resolutions = new List<Resolution>();
                    var resolution = new Resolution();
                    var rgbResolution = NRDevice.Instance.NativeHMD.GetEyeResolution(NativeEye.RGB);
                    resolution.width = rgbResolution.width;
                    resolution.height = rgbResolution.height;
                    resolutions.Add(resolution);
                    m_SupportedResolutions = resolutions;
                }

                return m_SupportedResolutions;
            }
        }

        /**
         * Indicates whether or not the VideoCapture instance is currently recording video.
         */
        public bool IsRecording { get; private set; }

        private NRRecordBehaviour m_RecordBehaviour;

        public NRRecordBehaviour RecordBehaviour
        {
            get
            {
                return m_RecordBehaviour;
            }
        }

        public static void CreateAsync(bool showHolograms, OnVideoCaptureResourceCreatedCallback onCreatedCallback)
        {
            NRVideoCapture capture = new NRVideoCapture();
            capture.m_RecordBehaviour = NRRecordBehaviour.Create(() =>
            {
                if (onCreatedCallback != null)
                {
                    onCreatedCallback(capture);
                }
            });
        }

        /**
         * @brief Returns the supported frame rates at which a video can be recorded given a resolution.
         * 
         * @param resolution A recording resolution.
         * @return The frame rates at which the video can be recorded.
         */
        public static IEnumerable<int> GetSupportedFrameRatesForResolution(Resolution resolution)
        {
            List<int> frameRates = new List<int>();
            frameRates.Add(20);
            return frameRates;
        }

        /**
         * @brief Dispose must be called to shutdown the PhotoCapture instance.
         */
        public void Dispose()
        {
            if (m_RecordBehaviour != null)
            {
                m_RecordBehaviour.Release();
            }
        }

        public void StartRecordingAsync(string filename, OnStartedRecordingVideoCallback onStartedRecordingVideoCallback)
        {
            if (IsRecording)
            {
                if (onStartedRecordingVideoCallback != null)
                {
                    var result = new VideoCaptureResult();
                    result.resultType = CaptureResultType.UnknownError;
                    onStartedRecordingVideoCallback(result);
                }
                return;
            }
            m_RecordBehaviour.SetOutPutPath(filename);
            m_RecordBehaviour.StartRecord();
            if (onStartedRecordingVideoCallback != null)
            {
                var result = new VideoCaptureResult();
                result.resultType = CaptureResultType.Success;
                onStartedRecordingVideoCallback(result);
            }

            IsRecording = true;
        }

        public void StartVideoModeAsync(CameraParameters setupParams, AudioState audioState, OnVideoModeStartedCallback onVideoModeStartedCallback)
        {
            m_RecordBehaviour.SetConfigration(setupParams);
            if (onVideoModeStartedCallback != null)
            {
                var result = new VideoCaptureResult();
                result.resultType = CaptureResultType.Success;
                onVideoModeStartedCallback(result);
            }
        }

        public void StopRecordingAsync(OnStoppedRecordingVideoCallback onStoppedRecordingVideoCallback)
        {
            if (!IsRecording)
            {
                if (onStoppedRecordingVideoCallback != null)
                {
                    var result = new VideoCaptureResult();
                    result.resultType = CaptureResultType.UnknownError;
                    onStoppedRecordingVideoCallback(result);
                }
                return;
            }

            m_RecordBehaviour.StopRecord();
            if (onStoppedRecordingVideoCallback != null)
            {
                var result = new VideoCaptureResult();
                result.resultType = CaptureResultType.Success;
                onStoppedRecordingVideoCallback(result);
            }

            IsRecording = false;
        }

        public void StopVideoModeAsync(OnVideoModeStoppedCallback onVideoModeStoppedCallback)
        {
            if (onVideoModeStoppedCallback != null)
            {
                var result = new VideoCaptureResult();
                result.resultType = CaptureResultType.Success;
                onVideoModeStoppedCallback(result);
            }
        }

        /**
         * Contains the result of the capture request.
         */
        public enum CaptureResultType
        {
            /**
            * Specifies that the desired operation was successful.
            */
            Success = 0,

            /**
            * Specifies that an unknown error occurred.
            */
            UnknownError = 1
        }

        /**
         * Specifies what audio sources should be recorded while recording the video.
         */
        public enum AudioState
        {
            /**
            * Only include the mic audio in the video recording.
            */
            MicAudio = 0,

            /**
            * Only include the application audio in the video recording.
            */
            ApplicationAudio = 1,

            /**
            * Include both the application audio as well as the mic audio in the video recording.
            */
            ApplicationAndMicAudio = 2,

            /**
            * Do not include any audio in the video recording.
            */
            None = 3
        }

        /**
         * A data container that contains the result information of a video recording operation.
         */
        public struct VideoCaptureResult
        {
            /**
             * A generic result that indicates whether or not the VideoCapture operation succeeded.
             */
            public CaptureResultType resultType;

            /**
             * The specific HResult value.
             */
            public long hResult;

            /**
             * Indicates whether or not the operation was successful.
             */
            public bool success
            {
                get
                {
                    return resultType == CaptureResultType.Success;
                }
            }
        }

        /**
         * @brief Called when the web camera begins recording the video.
         * 
         * @return Indicates whether or not video recording started successfully.
         */
        public delegate void OnStartedRecordingVideoCallback(VideoCaptureResult result);

        /**
         * @brief Called when a VideoCapture resource has been created.
         * 
         * @param captureObject The VideoCapture instance.
         */
        public delegate void OnVideoCaptureResourceCreatedCallback(NRVideoCapture captureObject);

        /**
        * @brief Called when video mode has been started.
        * 
        * @return Indicates whether or not video mode was successfully activated.
        */
        public delegate void OnVideoModeStartedCallback(VideoCaptureResult result);

        /**
        * @brief Called when video mode has been stopped.
        * 
        * @return Indicates whether or not video mode was successfully deactivated.
        */
        public delegate void OnVideoModeStoppedCallback(VideoCaptureResult result);

        /**
         * @brief Called when the video recording has been saved to the file system.
         * 
         * @return Indicates whether or not video recording was saved successfully to the file system.
         */
        public delegate void OnStoppedRecordingVideoCallback(VideoCaptureResult result);
    }
}
