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
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal partial class NativeController
    {
        private UInt64 m_ControllerHandle = 0;
        private UInt64[] m_StateHandles = new UInt64[NRInput.MAX_CONTROLLER_STATE_COUNT] { 0, 0 };

        public bool Init()
        {
            NRDebugger.Log("NativeController start init:");
            NativeResult result = NativeApi.NRControllerCreate(ref m_ControllerHandle);
            if (result == NativeResult.Success)
            {
                //manually start controller
                NativeApi.NRControllerStart(m_ControllerHandle);

                int count = GetControllerCount();
                NRDebugger.Log("__controller count:" + count);
                for (int i = 0; i < count; i++)
                {
                    result = NativeApi.NRControllerStateCreate(m_ControllerHandle, i, ref m_StateHandles[i]);
                    if (result != NativeResult.Success)
                    {
                        Debug.LogError("Controller State Create Failed!" + result.ToString());
                        return false;
                    }
                }
                NRDebugger.Log("Native Controller Created Sucessed");
                return true;
            }
            NRDebugger.Log("Native Controller Create Failed!");
            return false;
        }

        //public void TestApi(int index)
        //{
        //string msg = "GetControllerCount:" + GetControllerCount() + "\n"
        //+"GetAvailableFeatures:" + GetAvailableFeatures(index) + "\n"
        //+ "GetControllerType:" + GetControllerType(index) + "\n"
        //+ "GetConnectionState:" + GetConnectionState(index) + "\n"
        //+ "GetBatteryLevel:" + GetBatteryLevel(index) + "\n"
        //+ "IsCharging:" + IsCharging(index) + "\n"
        //+ "GetPose:" + GetPose(index).ToString("F3") + "\n"
        //+ "GetGyro:" + GetGyro(index).ToString("F3") + "\n"
        //+ "GetAccel:" + GetAccel(index).ToString("F3") + "\n"
        //+ "GetMag:" + GetMag(index).ToString("F3") + "\n"
        //+ "GetButtonState:" + GetButtonState(index) + "\n"
        //+ "GetButtonUp:" + GetButtonUp(index) + "\n"
        //+ "GetButtonDown:" + GetButtonDown(index) + "\n"
        //+ "IsTouching:" + IsTouching(index) + "\n"
        //+ "GetTouchUp:" + GetTouchUp(index) + "\n"
        //+ "GetTouchDown:" + GetTouchDown(index) + "\n"
        //+ "GetTouch:" + GetTouch(index).ToString("F3") + "\n";
        //Debug.Log(msg);
        //}

        public int GetControllerCount()
        {
            int count = 0;
            if (NativeApi.NRControllerGetCount(m_ControllerHandle, ref count) != NativeResult.Success)
                Debug.LogError("Get Controller Count Failed!");
            return Mathf.Min(count, m_StateHandles.Length);
        }

        public void Pause()
        {
            NativeApi.NRControllerPause(m_ControllerHandle);
        }

        public void Resume()
        {
            NativeApi.NRControllerResume(m_ControllerHandle);
        }

        public void Stop()
        {
            NativeApi.NRControllerStop(m_ControllerHandle);
        }

        public void Destroy()
        {
            Stop();
            NativeApi.NRControllerDestroy(m_ControllerHandle);
        }

        public uint GetAvailableFeatures(int controllerIndex)
        {
            uint availableFeature = 0;
            NativeApi.NRControllerGetAvailableFeatures(m_ControllerHandle, controllerIndex, ref availableFeature);
            return availableFeature;
        }

        public ControllerType GetControllerType(int controllerIndex)
        {
            ControllerType controllerType = ControllerType.CONTROLLER_TYPE_UNKNOWN;
            NativeApi.NRControllerGetType(m_ControllerHandle, controllerIndex, ref controllerType);
            return controllerType;
        }

        public void RecenterController(int controllerIndex)
        {
            NativeApi.NRControllerRecenter(m_ControllerHandle, controllerIndex);
        }

        public void TriggerHapticVibrate(int controllerIndex, Int64 duration, float frequency, float amplitude)
        {
            NativeApi.NRControllerHapticVibrate(m_ControllerHandle, controllerIndex, duration, frequency, amplitude);
        }

        public bool UpdateState(int controllerIndex)
        {
            if (m_StateHandles[controllerIndex] == 0)
                NativeApi.NRControllerStateCreate(m_ControllerHandle, controllerIndex, ref m_StateHandles[controllerIndex]);
            if (m_StateHandles[controllerIndex] == 0)
                return false;
            NativeResult result = NativeApi.NRControllerStateUpdate(m_StateHandles[controllerIndex]);
            return result == NativeResult.Success;
        }

        public void DestroyState(int controllerIndex)
        {
            NativeApi.NRControllerStateDestroy(m_StateHandles[controllerIndex]);
        }

        public ControllerConnectionState GetConnectionState(int controllerIndex)
        {
            ControllerConnectionState state = ControllerConnectionState.CONTROLLER_CONNECTION_STATE_NOT_INITIALIZED;
            NativeApi.NRControllerStateGetConnectionState(m_StateHandles[controllerIndex], ref state);
            return state;
        }

        public int GetBatteryLevel(int controllerIndex)
        {
            int batteryLevel = -1;
            NativeApi.NRControllerStateGetBatteryLevel(m_StateHandles[controllerIndex], ref batteryLevel);
            return batteryLevel;
        }

        public bool IsCharging(int controllerIndex)
        {
            int isCharging = 0;
            NativeApi.NRControllerStateGetCharging(m_StateHandles[controllerIndex], ref isCharging);
            return isCharging == 1;
        }

        public Pose GetPose(int controllerIndex)
        {
            Pose controllerPos = Pose.identity;
            NativeMat4f mat4f = new NativeMat4f(Matrix4x4.identity);
            NativeResult result = NativeApi.NRControllerStateGetPose(m_StateHandles[controllerIndex], ref mat4f);
            if (result == NativeResult.Success)
                ConversionUtility.ApiPoseToUnityPose(mat4f, out controllerPos);
            return controllerPos;
        }

        public Vector3 GetGyro(int controllerIndex)
        {
            NativeVector3f vec3f = new NativeVector3f();
            NativeResult result = NativeApi.NRControllerStateGetGyro(m_StateHandles[controllerIndex], ref vec3f);
            if (result == NativeResult.Success)
                return vec3f.ToUnityVector3();
            return Vector3.zero;
        }

        public Vector3 GetAccel(int controllerIndex)
        {
            NativeVector3f vec3f = new NativeVector3f();
            NativeResult result = NativeApi.NRControllerStateGetAccel(m_StateHandles[controllerIndex], ref vec3f);
            if (result == NativeResult.Success)
                return vec3f.ToUnityVector3();
            return Vector3.zero;
        }

        public Vector3 GetMag(int controllerIndex)
        {
            NativeVector3f vec3f = new NativeVector3f();
            NativeResult result = NativeApi.NRControllerStateGetMag(m_StateHandles[controllerIndex], ref vec3f);
            if (result == NativeResult.Success)
                return vec3f.ToUnityVector3();
            return Vector3.zero;
        }

        public uint GetButtonState(int controllerIndex)
        {
            uint buttonPress = 0;
            NativeApi.NRControllerStateGetButtonState(m_StateHandles[controllerIndex], ref buttonPress);
            return buttonPress;
        }

        public uint GetButtonUp(int controllerIndex)
        {
            uint buttonUp = 0;
            NativeApi.NRControllerStateGetButtonUp(m_StateHandles[controllerIndex], ref buttonUp);
            return buttonUp;
        }

        public uint GetButtonDown(int controllerIndex)
        {
            uint buttonDown = 0;
            NativeApi.NRControllerStateGetButtonDown(m_StateHandles[controllerIndex], ref buttonDown);
            return buttonDown;
        }

        public bool IsTouching(int controllerIndex)
        {
            uint touchState = 0;
            NativeApi.NRControllerStateTouchState(m_StateHandles[controllerIndex], ref touchState);
            return touchState == 1;
        }

        public bool GetTouchUp(int controllerIndex)
        {
            uint touchUp = 0;
            NativeApi.NRControllerStateGetTouchUp(m_StateHandles[controllerIndex], ref touchUp);
            return touchUp == 1;
        }

        public bool GetTouchDown(int controllerIndex)
        {
            uint touchDown = 0;
            NativeApi.NRControllerStateGetTouchDown(m_StateHandles[controllerIndex], ref touchDown);
            return touchDown == 1;
        }

        public Vector2 GetTouch(int controllerIndex)
        {
            NativeVector2f touchPos = new NativeVector2f();
            NativeResult result = NativeApi.NRControllerStateGetTouchPose(m_StateHandles[controllerIndex], ref touchPos);
            if (result == NativeResult.Success)
                return touchPos.ToUnityVector2();
            return Vector3.zero;
        }

        public void UpdateHeadPose(Pose hmdPose)
        {
            NativeMat4f apiPose;
            ConversionUtility.UnityPoseToApiPose(hmdPose, out apiPose);
            NativeApi.NRControllerSetHeadPose(m_ControllerHandle, ref apiPose);
        }

        private partial struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerCreate(ref UInt64 out_controller_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStart(UInt64 controller_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerPause(UInt64 controller_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerResume(UInt64 controller_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStop(UInt64 controller_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerDestroy(UInt64 controller_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerGetCount(UInt64 controller_handle, ref int out_controller_count);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerGetAvailableFeatures(UInt64 controller_handle, int controller_index, ref uint out_controller_available_features);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerGetType(UInt64 controller_handle, int controller_index, ref ControllerType out_controller_type);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerRecenter(UInt64 controller_handle, int controller_index);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateCreate(UInt64 controller_handle, int controller_index, ref UInt64 out_controller_state_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateUpdate(UInt64 controller_state_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateDestroy(UInt64 controller_state_handle);

            //duration(nanoseconds), frequency(Hz), amplitude(0.0f ~ 1.0f)
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerHapticVibrate(UInt64 controller_handle, int controller_index, Int64 duration, float frequency, float amplitude);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetConnectionState(UInt64 controller_state_handle, ref ControllerConnectionState out_controller_connection_state);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetBatteryLevel(UInt64 controller_state_handle, ref int out_controller_battery_level);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetCharging(UInt64 controller_state_handle, ref int out_controller_charging);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetPose(UInt64 controller_state_handle, ref NativeMat4f out_controller_pose);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetGyro(UInt64 controller_state_handle, ref NativeVector3f out_controller_gyro);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetAccel(UInt64 controller_state_handle, ref NativeVector3f out_controller_accel);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetMag(UInt64 controller_state_handle, ref NativeVector3f out_controller_mag);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetButtonState(UInt64 controller_state_handle, ref uint out_controller_button_state);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetButtonUp(UInt64 controller_state_handle, ref uint out_controller_button_up);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetButtonDown(UInt64 controller_state_handle, ref uint out_controller_button_down);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateTouchState(UInt64 controller_state_handle, ref uint out_controller_touch_state);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetTouchUp(UInt64 controller_state_handle, ref uint out_controller_touch_up);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetTouchDown(UInt64 controller_state_handle, ref uint out_controller_touch_down);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetTouchPose(UInt64 controller_state_handle, ref NativeVector2f out_controller_touch_pose);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerSetHeadPose(UInt64 controller_handle, ref NativeMat4f out_controller_pose);
        };
    }
}
