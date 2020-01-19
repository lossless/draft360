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
    * @brief 6-dof Trackable's Native API .
    */
    internal partial class NativeTrackable
    {
        private NativeInterface m_NativeInterface;

        public NativeTrackable(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
        }

        public UInt64 CreateTrackableList()
        {
            UInt64 trackable_list_handle = 0;
            NativeApi.NRTrackableListCreate(m_NativeInterface.TrackingHandle, ref trackable_list_handle);
            return trackable_list_handle;
        }

        public void DestroyTrackableList(UInt64 trackable_list_handle)
        {
            NativeApi.NRTrackableListDestroy(m_NativeInterface.TrackingHandle, trackable_list_handle);
        }

        public int GetSize(UInt64 trackable_list_handle)
        {
            int list_size = 0;
            NativeApi.NRTrackableListGetSize(m_NativeInterface.TrackingHandle, trackable_list_handle, ref list_size);
            return list_size;
        }

        public UInt64 AcquireItem(UInt64 trackable_list_handle, int index)
        {
            UInt64 trackable_handle = 0;
            NativeApi.NRTrackableListAcquireItem(m_NativeInterface.TrackingHandle, trackable_list_handle, index, ref trackable_handle);
            return trackable_handle;
        }

        public UInt32 GetIdentify(UInt64 trackable_handle)
        {
            UInt32 identify = NativeConstants.IllegalInt;
            NativeApi.NRTrackableGetIdentifier(m_NativeInterface.TrackingHandle, trackable_handle, ref identify);
            return identify;
        }

        public TrackableType GetTrackableType(UInt64 trackable_handle)
        {
            TrackableType trackble_type = TrackableType.TRACKABLE_BASE;
            NativeApi.NRTrackableGetType(m_NativeInterface.TrackingHandle, trackable_handle, ref trackble_type);
            return trackble_type;
        }

        public TrackingState GetTrackingState(UInt64 trackable_handle)
        {
            TrackingState status = TrackingState.Stopped;
            NativeApi.NRTrackableGetTrackingState(m_NativeInterface.TrackingHandle, trackable_handle, ref status);
            return status;
        }

        private partial struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableListCreate(UInt64 session_handle,
                ref UInt64 out_trackable_list_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableListDestroy(UInt64 session_handle,
                UInt64 out_trackable_list_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableListGetSize(UInt64 session_handle,
                UInt64 trackable_list_handle, ref int out_list_size);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableListAcquireItem(UInt64 session_handle,
                UInt64 trackable_list_handle, int index, ref UInt64 out_trackable);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableGetIdentifier(UInt64 session_handle,
                UInt64 trackable_handle, ref UInt32 out_identifier);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableGetType(UInt64 session_handle,
                UInt64 trackable_handle, ref TrackableType out_trackable_type);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableGetTrackingState(UInt64 session_handle,
                UInt64 trackable_handle, ref TrackingState out_tracking_state);
        };
    }
}
