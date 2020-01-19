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
    using System.Runtime.InteropServices;

    /**
    * @brief 6-dof Trackable Image Tracking's Native API .
    */
    internal partial class NativeVersion
    {
        public static string GetVersion()
        {
            NRVersion version = new NRVersion();
            NativeApi.NRGetVersion(ref version);
            return version.ToString();
        }

        private partial struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRGetVersion(ref NRVersion out_version);
        };
    }
}
