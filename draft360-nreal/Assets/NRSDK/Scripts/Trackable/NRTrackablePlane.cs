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
    using System.Collections.Generic;
    using UnityEngine;

    /**
     * @brief A plane in the real world detected by NRInternel.
     */
    public class NRTrackablePlane : NRTrackable
    {
        internal NRTrackablePlane(UInt64 nativeHandle, NativeInterface nativeInterface)
          : base(nativeHandle, nativeInterface)
        {
        }

        /**
         * @brief Get the plane type.
         * @return Plane type.
         */
        public TrackablePlaneType GetPlaneType()
        {
            return NativeInterface.NativePlane.GetPlaneType(TrackableNativeHandle);
        }

        /**
         * @brief Gets the position and orientation of the plane's center in Unity world space.
         */
        public override Pose GetCenterPose()
        {
            return NativeInterface.NativePlane.GetCenterPose(TrackableNativeHandle);
        }

        /**
         * @brief Gets the extent of plane in the X dimension, centered on the plane position.
         */
        public float ExtentX
        {
            get
            {
                return NativeInterface.NativePlane.GetExtentX(TrackableNativeHandle);
            }
        }

        /**
         * @brief Gets the extent of plane in the Z dimension, centered on the plane position.
         */
        public float ExtentZ
        {
            get
            {
                return NativeInterface.NativePlane.GetExtentZ(TrackableNativeHandle);
            }
        }

        /**
         * @brief Gets a list of points (in clockwise order) in plane coordinate representing a boundary polygon for the plane.
         * 
         * @param[out] polygonList A list used to be filled with polygon points.
         */
        public void GetBoundaryPolygon(List<Vector3> polygonList)
        {
            polygonList.Clear();
            int size = NativeInterface.NativePlane.GetPolygonSize(TrackableNativeHandle);
            float[] temp_data = NativeInterface.NativePlane.GetPolygonData(TrackableNativeHandle);
            float[] point_data = new float[size * 2];
            Array.Copy(temp_data, point_data, size * 2);

            Pose centerPos = GetCenterPose();
            var unityWorldTPlane = Matrix4x4.TRS(centerPos.position, centerPos.rotation, Vector3.one);
            for (int i = 2 * size - 2; i >= 0; i -= 2)
            {
                var point = unityWorldTPlane.MultiplyPoint3x4(new Vector3(point_data[i], 0, -point_data[i + 1]));
                polygonList.Add(point);
            }
        }
    }
}
