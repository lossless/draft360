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
    using UnityEngine;
    using System.Runtime.InteropServices;


    internal partial class NativeEmulator
    {
        public NativeEmulator()
        {
        }

        private UInt64 m_TrackingHandle;
        private UInt64 m_ControllerHandle;


        #region Tracking

        public bool CreateSIMTracking()
        {
            NativeResult result = NativeApi.NRSIMTrackingCreate(ref m_TrackingHandle);
            return result == NativeResult.Success;
        }

        public bool SetHeadTrackingPose(Vector3 position, Quaternion rotation)
        {
            NativeVector3f nativeVector3F = new NativeVector3f(new Vector3(position.x, position.y, -position.z));
            NativeVector4f nativeVector4F = new NativeVector4f(new Vector4(rotation.x, rotation.y, -rotation.z, -rotation.w));
            NativeResult result = NativeApi.NRSIMTrackingSetHeadTrackingPose(m_TrackingHandle, nativeVector3F, nativeVector4F);
            return result == NativeResult.Success;
        }

        public bool UpdateTrackableImageData(Vector3 centerPos, Quaternion centerQua, float extentX, float extentZ, UInt32 identifier, TrackingState state)
        {
            NativeVector3f pos = new NativeVector3f(new Vector3(centerPos.x, centerPos.y, -centerPos.z));
            NativeVector4f qua = new NativeVector4f(new Vector4(-centerQua.x, -centerQua.y, centerQua.z, centerQua.w));
            NativeResult result = NativeApi.NRSIMTrackingUpdateTrackableImageData(m_TrackingHandle, pos, qua, extentX, extentZ, identifier, (int)state);
            return result == NativeResult.Success;
        }

        public bool UpdateTrackablePlaneData(Vector3 centerPos, Quaternion centerQua, float extentX, float extentZ, UInt32 identifier, TrackingState state)
        {
            NativeVector3f pos = new NativeVector3f(new Vector3(centerPos.x, centerPos.y, -centerPos.z));
            NativeVector4f qua = new NativeVector4f(new Vector4(-centerQua.x, -centerQua.y, centerQua.z, centerQua.w));
            NativeResult result = NativeApi.NRSIMTrackingUpdateTrackablePlaneData(m_TrackingHandle, pos, qua, extentX, extentZ, identifier, (int)state);
            return result == NativeResult.Success;
        }

        /**
         * @brief Set the trackables data for Unity Editor develop.
         */
        public bool UpdateTrackableData<T>(Vector3 centerPos, Quaternion centerQua, float extentX, float extentZ, System.UInt32 identifier, TrackingState state) where T : NRTrackable
        {
            if (typeof(NRTrackablePlane).Equals(typeof(T)))
            {
                return NREmulatorManager.Instance.NativeEmulatorApi.UpdateTrackablePlaneData(centerPos, centerQua, extentX, extentZ, identifier, state);
            }
            else if (typeof(NRTrackableImage).Equals(typeof(T)))
            {
                return NREmulatorManager.Instance.NativeEmulatorApi.UpdateTrackableImageData(centerPos, centerQua, extentX, extentZ, identifier, state);
            }
            else
            {
                return false;
            }
        }

        #endregion


        #region Controller

        public bool CreateSIMController()
        {
            NativeResult result = NativeApi.NRSIMControllerCreate(ref m_ControllerHandle);
            return result == NativeResult.Success;
        }

        public bool DestorySIMController()
        {
            NativeResult result = NativeApi.NRSIMControllerDestroyAll();
            return result == NativeResult.Success;
        }

        public bool SetControllerTimestamp(UInt64 timestamp)
        {
            NativeResult result = NativeApi.NRSIMControllerSetTimestamp(m_ControllerHandle, timestamp);
            return result == NativeResult.Success;
        }

        public bool SetControllerPosition(Vector3 postion)
        {
            NativeVector3f nv3 = new NativeVector3f(postion);
            NativeResult result = NativeApi.NRSIMControllerSetPosition(m_ControllerHandle, nv3);
            return result == NativeResult.Success;
        }

        public bool SetControllerRotation(Quaternion quaternion)
        {
            NativeVector4f nv4 = new NativeVector4f(new Vector4(quaternion.x, quaternion.y, -quaternion.z, -quaternion.w));
            NativeResult result = NativeApi.NRSIMControllerSetRotation(m_ControllerHandle, nv4);
            return result == NativeResult.Success;
        }

        public bool SetControllerAccelerometer(Vector3 accel)
        {
            NativeVector3f nv3 = new NativeVector3f(accel);
            NativeResult result = NativeApi.NRSIMControllerSetAccelerometer(m_ControllerHandle, nv3);
            return result == NativeResult.Success;
        }

        public bool SetControllerButtonState(Int32 buttonState)
        {
            NativeResult result = NativeApi.NRSIMControllerSetButtonState(m_ControllerHandle, buttonState);
            return result == NativeResult.Success;
        }

        public bool SetControllerIsTouching(bool isTouching)
        {
            NativeResult result = NativeApi.NRSIMControllerSetIsTouching(m_ControllerHandle, isTouching);
            return result == NativeResult.Success;
        }

        public bool SetControllerTouchPoint(float x, float y)
        {
            NativeVector3f nv3 = new NativeVector3f(new Vector3(x, y, 0f));
            NativeResult result = NativeApi.NRSIMControllerSetTouchPoint(m_ControllerHandle, nv3);
            return result == NativeResult.Success;
        }

        public bool SetControllerSubmit()
        {
            NativeResult result = NativeApi.NRSIMControllerSubmit(m_ControllerHandle);
            return result == NativeResult.Success;
        }

        #endregion


        private partial struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMTrackingCreate(ref UInt64 out_tracking_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMTrackingSetHeadTrackingPose(UInt64 tracking_handle, NativeVector3f position, NativeVector4f rotation);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMTrackingUpdateTrackableImageData
                (UInt64 tracking_handle, NativeVector3f center_pos, NativeVector4f center_rotation, float extent_x, float extent_z, UInt32 identifier, int state);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMTrackingUpdateTrackablePlaneData
                (UInt64 tracking_handle, NativeVector3f center_pos, NativeVector4f center_rotation, float extent_x, float extent_z, UInt32 identifier, int state);


            // Controller
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerCreate(ref UInt64 out_controller_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerDestroyAll();

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSetTimestamp(UInt64 controller_handle, UInt64 timestamp);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSetPosition(UInt64 controller_handle, NativeVector3f position);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSetRotation(UInt64 controller_handle, NativeVector4f rotation);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSetAccelerometer(UInt64 controller_handle, NativeVector3f accel);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSetButtonState(UInt64 controller_handle, Int32 buttonState);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSetIsTouching(UInt64 controller_handle, bool isTouching);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSetTouchPoint(UInt64 controller_handle, NativeVector3f point);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSubmit(UInt64 controller_handle);

            [DllImport("kernel32", SetLastError = true)]
            public static extern bool FreeLibrary(IntPtr hModule);
        }

    }
}
