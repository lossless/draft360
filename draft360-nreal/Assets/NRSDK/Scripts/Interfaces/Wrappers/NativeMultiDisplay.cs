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

    /**
   * @brief HMD Eye offset Native API .
   */
    internal partial class NativeMultiDisplay
    {
        private UInt64 m_MultiDisplayHandle;

        public bool Create()
        {
            NativeResult result = NativeApi.NRDisplayCreate(ref m_MultiDisplayHandle);
            NRDebugger.Log("[NativeMultiDisplay Create]:" + result.ToString());
            return result == NativeResult.Success;
        }

        public bool UpdateHomeScreenTexture(IntPtr rendertexture)
        {
            NativeResult result = NativeApi.NRDisplaySetMainDisplayTexture(m_MultiDisplayHandle, rendertexture);
            NRDebugger.Log("[NativeMultiDisplay UpdateHomeScreenTexture]:" + result.ToString());
            return result == NativeResult.Success;
        }

        public bool Pause()
        {
            NativeResult result = NativeApi.NRDisplayPause(m_MultiDisplayHandle);
            NRDebugger.Log("[NativeMultiDisplay Pause]:" + result.ToString());
            return result == NativeResult.Success;
        }

        public bool Resume()
        {
            NativeResult result = NativeApi.NRDisplayResume(m_MultiDisplayHandle);
            NRDebugger.Log("[NativeMultiDisplay Resume]:" + result.ToString());
            return result == NativeResult.Success;
        }

        public bool Destroy()
        {
            NativeResult result = NativeApi.NRDisplayDestroy(m_MultiDisplayHandle);
            NRDebugger.Log("[NativeMultiDisplay Destroy]:" + result.ToString());
            return result == NativeResult.Success;
        }

        private struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRDisplayCreate(ref UInt64 out_display_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRDisplayPause(UInt64 display_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRDisplayResume(UInt64 display_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRDisplaySetMainDisplayTexture(UInt64 display_handle,
                IntPtr controller_texture);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRDisplayDestroy(UInt64 display_handle);
        };
    }
}
