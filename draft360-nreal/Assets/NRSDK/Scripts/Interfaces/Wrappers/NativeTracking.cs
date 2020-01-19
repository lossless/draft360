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
    * @brief Native Tracking API.
    */
    internal partial class NativeTracking
    {
        private NativeInterface m_NativeInterface;
        private UInt64 m_TrackingHandle;

        public NativeTracking(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
        }

        public bool Create()
        {
            NativeResult result = NativeApi.NRTrackingCreate(ref m_TrackingHandle);
            NRDebugger.Log("[NativeSession Create :]" + result.ToString());
            m_NativeInterface.TrackingHandle = m_TrackingHandle;
            return result == NativeResult.Success;
        }

        public bool SetTrackingMode(TrackingMode mode)
        {
            NativeResult result = NativeApi.NRTrackingInitSetTrackingMode(m_TrackingHandle, mode);
            NRDebugger.Log("[NativeSession SetTrackingMode :]" + mode.ToString() + " result:" + result.ToString());
            return result == NativeResult.Success;
        }

        public bool Start()
        {
            NativeResult result = NativeApi.NRTrackingStart(m_TrackingHandle);
            NRDebugger.Log("[NativeSession Start :]" + result.ToString());
            return result == NativeResult.Success;
        }

        public bool Pause()
        {
            NativeResult result = NativeApi.NRTrackingPause(m_TrackingHandle);
            NRDebugger.Log("[NativeSession Pause :]" + result.ToString());
            return result == NativeResult.Success;
        }

        public bool Resume()
        {
            NativeResult result = NativeApi.NRTrackingResume(m_TrackingHandle);
            NRDebugger.Log("[NativeSession Resume :]" + result.ToString());
            return result == NativeResult.Success;
        }

        // only worked at 3dof mode
        public void Recenter()
        {
            var result = NativeApi.NRTrackingRecenter(m_TrackingHandle);
            NRDebugger.Log("[NativeSession Recenter] :" + result.ToString());
        }

        public bool Destroy()
        {
            NativeResult result = NativeApi.NRTrackingDestroy(m_TrackingHandle);
            NRDebugger.Log("[NativeSession Destroy :]" + result.ToString());
            return result == NativeResult.Success;
        }

        public void UpdateTrackables(UInt64 trackable_list_handle, TrackableType trackable_type)
        {
            NativeApi.NRTrackingUpdateTrackables(m_NativeInterface.TrackingHandle, trackable_type, trackable_list_handle);
        }

        private struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingCreate(ref UInt64 out_tracking_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingInitSetTrackingMode(UInt64 tracking_handle, TrackingMode tracking_mode);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingStart(UInt64 tracking_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingDestroy(UInt64 tracking_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingPause(UInt64 tracking_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingResume(UInt64 tracking_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingRecenter(UInt64 tracking_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingUpdateTrackables(UInt64 tracking_handle,
               TrackableType trackable_type, UInt64 out_trackable_list_handle);
        };
    }
}
