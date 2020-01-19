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
    using NRKernal;
    using System;
    using System.Runtime.InteropServices;

    public class NativeEncoder
    {
        public const String NRNativeEncodeLibrary = "media_enc";
        public UInt64 EncodeHandle;

        public bool Create()
        {
            var result = NativeApi.HWEncoderCreate(ref EncodeHandle);
            return result == 0;
        }

        public bool Start()
        {
            var result = NativeApi.HWEncoderStart(EncodeHandle);
            NRDebugger.Log("[Encode] Start :" + (result == 0).ToString());
            return result == 0;
        }

        public void SetConfigration(NativeEncodeConfig config)
        {
            var result = NativeApi.HWEncoderSetConfigration(EncodeHandle, LitJson.JsonMapper.ToJson(config));
            NRDebugger.Log("[Encode] SetConfigration :" + (result == 0).ToString());
        }

        public void UpdateSurface(IntPtr texture_id, UInt64 time_stamp)
        {
            var result = NativeApi.HWEncoderUpdateSurface(EncodeHandle, texture_id, time_stamp);
            NRDebugger.Log("[Encode] UpdateSurface :" + (result == 0).ToString());
        }

        public bool Stop()
        {
            var result = NativeApi.HWEncoderStop(EncodeHandle);
            NRDebugger.Log("[Encode] Stop :" + (result == 0).ToString());
            return result == 0;
        }

        public void Destroy()
        {
            NativeApi.HWEncoderDestroy(EncodeHandle);
        }

        private struct NativeApi
        {
            [DllImport(NRNativeEncodeLibrary)]
            public static extern int HWEncoderCreate(ref UInt64 out_encoder_handle);

            [DllImport(NRNativeEncodeLibrary)]
            public static extern int HWEncoderStart(UInt64 encoder_handle);

            [DllImport(NRNativeEncodeLibrary)]
            public static extern int HWEncoderSetConfigration(UInt64 encoder_handle, string config);

            [DllImport(NRNativeEncodeLibrary)]
            public static extern int HWEncoderUpdateSurface(UInt64 encoder_handle, IntPtr texture_id, UInt64 time_stamp);

            [DllImport(NRNativeEncodeLibrary)]
            public static extern int HWEncoderStop(UInt64 encoder_handle);

            [DllImport(NRNativeEncodeLibrary)]
            public static extern int HWEncoderDestroy(UInt64 encoder_handle);
        }
    }
}