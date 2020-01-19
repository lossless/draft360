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

    /// @cond EXCLUDE_FROM_DOXYGEN
    public class NRTools
    {
        public static string GetStreamingAssetsPath()
        {
            string path = Application.streamingAssetsPath;
#if UNITY_EDITOR || UNITY_STANDALONE
            path = "file://" + Application.streamingAssetsPath + "/";
#elif UNITY_IPHONE
            path = Application.dataPath +"/Raw/";
#elif UNITY_ANDROID
            path ="jar:file://" + Application.dataPath + "!/assets/";
#endif
            return path;
        }

        public static string GetSdcardPath()
        {
            string path = null;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            path = System.IO.Directory.GetParent(Application.dataPath).ToString() + "/";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            path = "file://" + System.IO.Directory.GetCurrentDirectory()+"/";
#elif UNITY_ANDROID
            path ="file:///storage/emulated/0/";
#endif
            return path;
        }

        public static string GetTrackingImageDataGenPath()
        {
            string path = Application.persistentDataPath + "/TrackingImageData/";
            return path;
        }

        public static string GetTimeByMSeconds(long ms)
        {
            int s = (int)ms / 1000;
            int h = (int)(s / 3600);
            int m = (s % 3600) / 60;
            s = (s % 3600) % 60;
            return string.Format("{0}:{1}:{2}", h > 10 ? h.ToString() : "0" + h, m > 10 ? m.ToString() : "0" + m, s > 10 ? s.ToString() : "0" + s);
        }

        public static ulong GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToUInt64(ts.TotalMilliseconds);
        }
    }
    /// @endcond
}
