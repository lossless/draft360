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
     * @brief A Trackable in the real world detected by NRInternel.
     * 
     * The base class of TrackablePlane and TrackableImage. Through this class, 
     * application can get the infomation of a trackable object.
     */
    public abstract class NRTrackable
    {
        internal UInt64 TrackableNativeHandle = 0;

        internal NativeInterface NativeInterface;

        internal NRTrackable(UInt64 trackableNativeHandle, NativeInterface nativeinterface)
        {
            TrackableNativeHandle = trackableNativeHandle;
            NativeInterface = nativeinterface;
        }

        /**
        * Get the id of trackable.
        */
        public int GetDataBaseIndex()
        {
            UInt32 identify = NativeInterface.NativeTrackable.GetIdentify(TrackableNativeHandle);
            identify &= 0X0000FFFF;
            return (int)identify;
        }

        /**
         * Get the tracking state of current trackable.
         */
        public TrackingState GetTrackingState()
        {
            return NativeInterface.NativeTrackable.GetTrackingState(TrackableNativeHandle);
        }

        /**
         * Get the tracking type of current trackable.
         */
        public TrackableType GetTrackableType()
        {
            return NativeInterface.NativeTrackable.GetTrackableType(TrackableNativeHandle);
        }

        /**
         * Get the center pose of current trackable.
         */
        public virtual Pose GetCenterPose()
        {
            return Pose.identity;
        }

        /**
         * Creates an anchor attached to current trackable at given pose.
         */
        internal NRAnchor CreateAnchor()
        {
            return NRAnchor.Factory(this);
        }
    }
}
