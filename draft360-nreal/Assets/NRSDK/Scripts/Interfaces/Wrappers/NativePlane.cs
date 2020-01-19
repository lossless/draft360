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
    using UnityEngine;

    /**
    * @brief 6-dof Plane Tracking's Native API .
    */
    internal partial class NativePlane
    {
        private NativeInterface m_NativeInterface;
        private const int m_MaxPolygonSize = 1024;
        private float[] m_Points;
        private GCHandle m_TmpPointsHandle;

        public NativePlane(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;

            m_Points = new float[m_MaxPolygonSize * 2];
            m_TmpPointsHandle = GCHandle.Alloc(m_Points, GCHandleType.Pinned);
        }

        ~NativePlane()
        {
            m_TmpPointsHandle.Free();
        }

        public TrackablePlaneType GetPlaneType(UInt64 trackable_handle)
        {
            TrackablePlaneType plane_type = TrackablePlaneType.INVALID;
            NativeApi.NRTrackablePlaneGetType(m_NativeInterface.TrackingHandle, trackable_handle, ref plane_type);
            return plane_type;
        }

        public Pose GetCenterPose(UInt64 trackble_handle)
        {
            Pose pose = Pose.identity;
            NativeMat4f center_native_pos = NativeMat4f.identity;
            NativeApi.NRTrackablePlaneGetCenterPose(m_NativeInterface.TrackingHandle, trackble_handle, ref center_native_pos);
            ConversionUtility.ApiPoseToUnityPose(center_native_pos, out pose);
            return pose;
        }

        public float GetExtentX(UInt64 trackable_handle)
        {
            float extent_x = 0;
            NativeApi.NRTrackablePlaneGetExtentX(m_NativeInterface.TrackingHandle, trackable_handle, ref extent_x);
            return extent_x;
        }

        public float GetExtentZ(UInt64 trackable_handle)
        {
            float extent_z = 0;
            NativeApi.NRTrackablePlaneGetExtentZ(m_NativeInterface.TrackingHandle, trackable_handle, ref extent_z);
            return extent_z;
        }

        public int GetPolygonSize(UInt64 trackable_handle)
        {
            int polygon_size = 0;
            NativeApi.NRTrackablePlaneGetPolygonSize(m_NativeInterface.TrackingHandle, trackable_handle, ref polygon_size);
            return polygon_size / 2;
        }

        public float[] GetPolygonData(UInt64 trackable_handle)
        {
            NativeApi.NRTrackablePlaneGetPolygon(m_NativeInterface.TrackingHandle, trackable_handle, (m_TmpPointsHandle.AddrOfPinnedObject()));
            return m_Points;
        }

        private partial struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackablePlaneGetType(UInt64 session_handle,
                UInt64 trackable_handle, ref TrackablePlaneType out_plane_type);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackablePlaneGetCenterPose(UInt64 session_handle,
                UInt64 trackable_handle, ref NativeMat4f out_center_pose);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackablePlaneGetExtentX(UInt64 session_handle,
                UInt64 trackable_handle, ref float out_extent_x);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackablePlaneGetExtentZ(UInt64 session_handle,
                UInt64 trackable_handle, ref float out_extent_z);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackablePlaneGetPolygonSize(UInt64 session_handle,
                UInt64 trackable_handle, ref int out_polygon_size);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackablePlaneGetPolygon(UInt64 session_handle,
                UInt64 trackable_handle, IntPtr out_polygon);
        };
    }
}
