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

    /**
     * @brief A trackable image in the real world detected by NRInternel.
     */
    public class NRTrackableImage : NRTrackable
    {
        internal NRTrackableImage(UInt64 nativeHandle, NativeInterface nativeInterface)
          : base(nativeHandle, nativeInterface)
        {
        }

        /**
         * @brief Gets the position and orientation of the marker's center in Unity world space.
         */
        public override Pose GetCenterPose()
        {
            return NativeInterface.NativeTrackableImage.GetCenterPose(TrackableNativeHandle);
        }

        /**
         * @brief Gets the width of marker.
         */
        public float ExtentX
        {
            get
            {
                return Size.x;
            }
        }

        /**
         * @brief Gets the height of marker.
         */
        public float ExtentZ
        {
            get
            {
                return Size.y;
            }
        }

        /**
         * @brief Get the size of trackable image .
         * @return size of trackable imag(width、height).
         */
        public Vector2 Size
        {
            get
            {
                return NativeInterface.NativeTrackableImage.GetSize(TrackableNativeHandle);
            }
        }
    }
}
