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

    /// @cond EXCLUDE_FROM_DOXYGEN
    public class NRDevice : SingleTon<NRDevice>
    {
        internal NativeHMD NativeHMD { get; set; }
        private bool m_IsInit = false;
        private AndroidJavaObject m_UnityActivity;

        public void Init()
        {
            if (m_IsInit)
            {
                return;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            // Init before all actions.
            AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            m_UnityActivity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            NativeApi.NRSDKInitSetAndroidActivity(m_UnityActivity.GetRawObject());
            
            NativeHMD = new NativeHMD();
            NativeHMD.Create();
#endif
            m_IsInit = true;
        }

        public void QuitApp()
        {
            Debug.Log("Start To Quit Application...");
            NRSessionManager.Instance.DestroySession();
            Application.Quit();
        }

        public void ForceKill()
        {
            Debug.Log("Start To kill Application...");
            AndroidJavaClass processClass = new AndroidJavaClass("android.os.Process");
            int myPid = processClass.CallStatic<int>("myPid");
            processClass.CallStatic("killProcess", myPid);
        }

        public void Destroy()
        {
            if (NativeHMD != null)
            {
                NativeHMD.Destroy();
                NativeHMD = null;
            }
        }

        private struct NativeApi
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSDKInitSetAndroidActivity(IntPtr android_activity);
#endif
        }
    }
    /// @endcond
}
