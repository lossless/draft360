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

    internal partial class NativeConfigration
    {
        private NativeInterface m_NativeInterface;

        private UInt64 m_ConfigHandle = 0;
        private UInt64 m_DatabaseHandle = 0;

        private NativeTrackableImage m_NativeTrackableImage;
        private TrackableImageFindingMode m_ImageTrackingMode = TrackableImageFindingMode.DISABLE;

        public NativeConfigration(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
            m_NativeTrackableImage = m_NativeInterface.NativeTrackableImage;
        }

        public void UpdateConfig(NRSessionConfig config)
        {
            if (m_ConfigHandle == 0)
            {
                m_ConfigHandle = this.Create();
            }

            if (m_ConfigHandle == 0)
            {
                NRDebugger.LogError("Faild to Create ARSessionConfig!!!");
                return;
            }

            var plane_find_mode = this.GetPlaneFindMode(m_ConfigHandle);
            if (plane_find_mode != config.PlaneFindingMode)
            {
                SetPlaneFindMode(m_ConfigHandle, config.PlaneFindingMode);
            }

            if (config.ImageTrackingMode != m_ImageTrackingMode)
            {
                //Trackable Image
                switch (config.ImageTrackingMode)
                {
                    case TrackableImageFindingMode.DISABLE:
                        if (m_DatabaseHandle != 0)
                        {
                            m_NativeTrackableImage.DestroyDataBase(m_DatabaseHandle);
                            m_DatabaseHandle = 0;
                        }
                        var result = SetTrackableImageDataBase(m_ConfigHandle, 0);
                        NRDebugger.Log("[Disable trackable image] result : " + result);
                        break;
                    case TrackableImageFindingMode.ENABLE:
                        if (config.TrackingImageDatabase != null)
                        {
                            m_DatabaseHandle = m_NativeTrackableImage.CreateDataBase();
                            result = m_NativeTrackableImage.LoadDataBase(m_DatabaseHandle, config.TrackingImageDatabase.TrackingImageDataPath);
                            NRDebugger.LogFormat("[LoadDataBase] path:{0} result:{1} ", config.TrackingImageDatabase.TrackingImageDataPath, result);
                            result = SetTrackableImageDataBase(m_ConfigHandle, m_DatabaseHandle);
                            NRDebugger.Log("[SetTrackableImageDataBase] result : " + result);
                        }
                        else
                        {
                            result = SetTrackableImageDataBase(m_ConfigHandle, 0);
                            NRDebugger.Log("[Disable trackable image] result : " + result);
                        }
                        break;
                    default:
                        break;
                }

                m_ImageTrackingMode = config.ImageTrackingMode;
            }
        }

        private UInt64 Create()
        {
            UInt64 config_handle = 0;
            NativeApi.NRConfigCreate(m_NativeInterface.TrackingHandle, ref config_handle);
            return config_handle;
        }

        public TrackablePlaneFindingMode GetPlaneFindMode(UInt64 config_handle)
        {
            TrackablePlaneFindingMode mode = TrackablePlaneFindingMode.DISABLE;
            NativeApi.NRConfigGetTrackablePlaneFindingMode(m_NativeInterface.TrackingHandle, config_handle, ref mode);
            return mode;
        }

        public bool SetPlaneFindMode(UInt64 config_handle, TrackablePlaneFindingMode mode)
        {
            var result = NativeApi.NRConfigSetTrackablePlaneFindingMode(m_NativeInterface.TrackingHandle, config_handle, mode);
            return result == NativeResult.Success;
        }

        public UInt64 GetTrackableImageDataBase(UInt64 config_handle)
        {
            UInt64 database_handle = 0;
            NativeApi.NRConfigGetTrackableImageDatabase(m_NativeInterface.TrackingHandle, config_handle, ref database_handle);
            return database_handle;
        }

        public bool SetTrackableImageDataBase(UInt64 config_handle, UInt64 database_handle)
        {
            var result = NativeApi.NRConfigSetTrackableImageDatabase(m_NativeInterface.TrackingHandle, config_handle, database_handle);
            return result == NativeResult.Success;
        }

        public bool Destroy(UInt64 config_handle)
        {
            var result = NativeApi.NRConfigDestroy(m_NativeInterface.TrackingHandle, config_handle);
            return result == NativeResult.Success;
        }

        private struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRConfigCreate(UInt64 session_handle, ref UInt64 out_config_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRConfigDestroy(UInt64 session_handle, UInt64 config_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRConfigGetTrackablePlaneFindingMode(UInt64 session_handle,
                UInt64 config_handle, ref TrackablePlaneFindingMode out_trackable_plane_finding_mode);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRConfigSetTrackablePlaneFindingMode(UInt64 session_handle,
                UInt64 config_handle, TrackablePlaneFindingMode trackable_plane_finding_mode);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRConfigGetTrackableImageDatabase(UInt64 session_handle,
                UInt64 config_handle, ref UInt64 out_trackable_image_database_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRConfigSetTrackableImageDatabase(UInt64 session_handle,
                UInt64 config_handle, UInt64 trackable_image_database_handle);
        };
    }
}
