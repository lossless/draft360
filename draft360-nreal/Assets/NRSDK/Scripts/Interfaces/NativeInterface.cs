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
#if UNITY_STANDALONE_WIN
    using System.Runtime.InteropServices;
    using UnityEngine;
#endif

    /**
     * @brief Manage the Total Native API .
     */
    internal partial class NativeInterface
    {
        public NativeInterface()
        {
            //Add Standalone plugin search path.
#if UNITY_STANDALONE_WIN
            NativeApi.SetDllDirectory(System.IO.Path.Combine(Application.dataPath, "Plugins"));
#endif
            NativeHeadTracking = new NativeHeadTracking(this);
            NativeTracking = new NativeTracking(this);
            NativeTrackableImage = new NativeTrackableImage(this);
            NativePlane = new NativePlane(this);
            NativeTrackable = new NativeTrackable(this);
            TrackableFactory = new NRTrackableManager(this);
            Configration = new NativeConfigration(this);
        }

        public UInt64 TrackingHandle { get; set; }

        public NativeHeadTracking NativeHeadTracking { get; set; }

        public NativeTracking NativeTracking { get; set; }

        public NativeTrackableImage NativeTrackableImage { get; set; }

        public NativePlane NativePlane { get; set; }

        public NativeTrackable NativeTrackable { get; set; }

        public NRTrackableManager TrackableFactory { get; set; }

        public NativeConfigration Configration { get; set; }

        private partial struct NativeApi
        {
#if UNITY_STANDALONE_WIN
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool SetDllDirectory(string lpPathName);
#endif
        }
    }
}
