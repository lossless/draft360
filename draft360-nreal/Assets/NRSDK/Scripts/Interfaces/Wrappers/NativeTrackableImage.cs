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
    using System.Text;
    using UnityEngine;

    /**
    * @brief 6-dof Trackable Image Tracking's Native API .
    */
    internal partial class NativeTrackableImage
    {
        private NativeInterface m_NativeInterface;

        public NativeTrackableImage(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
        }

        public UInt64 CreateDataBase()
        {
            UInt64 database_handle = 0;
            NativeApi.NRTrackableImageDatabaseCreate(m_NativeInterface.TrackingHandle, ref database_handle);
            return database_handle;
        }

        public bool DestroyDataBase(UInt64 database_handle)
        {
            var result = NativeApi.NRTrackableImageDatabaseDestroy(m_NativeInterface.TrackingHandle, database_handle);
            return result == NativeResult.Success;
        }

        public bool LoadDataBase(UInt64 database_handle, string path)
        {
            var result = NativeApi.NRTrackableImageDatabaseLoadDirectory(m_NativeInterface.TrackingHandle, database_handle, path);
            return result == NativeResult.Success;
        }

        public Pose GetCenterPose(UInt64 trackable_handle)
        {
            Pose pose = Pose.identity;
            NativeMat4f center_pose_native = NativeMat4f.identity;
            NativeApi.NRTrackableImageGetCenterPose(m_NativeInterface.TrackingHandle, trackable_handle, ref center_pose_native);

            ConversionUtility.ApiPoseToUnityPose(center_pose_native, out pose);
            return pose;
        }

        public Vector2 GetSize(UInt64 trackable_handle)
        {
            float extent_x, extent_z;
            extent_x = extent_z = 0;
            NativeApi.NRTrackableImageGetExtentX(m_NativeInterface.TrackingHandle, trackable_handle, ref extent_x);
            NativeApi.NRTrackableImageGetExtentZ(m_NativeInterface.TrackingHandle, trackable_handle, ref extent_z);
            return new Vector2(extent_x * 0.001f, extent_z * 0.001f);
        }

        private partial struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableImageDatabaseCreate(UInt64 session_handle,
                ref UInt64 out_trackable_image_database_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableImageDatabaseDestroy(UInt64 session_handle,
                UInt64 trackable_image_database_handle);

            [DllImport(NativeConstants.NRNativeLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern NativeResult NRTrackableImageDatabaseLoadDirectory(UInt64 session_handle,
                UInt64 trackable_image_database_handle, string trackable_image_database_directory);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableImageGetCenterPose(UInt64 session_handle,
                UInt64 trackable_handle, ref NativeMat4f out_center_pose);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableImageGetExtentX(UInt64 session_handle,
                UInt64 trackable_handle, ref float out_extent_x);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableImageGetExtentZ(UInt64 session_handle,
                UInt64 trackable_handle, ref float out_extent_z);
        };
    }
}
