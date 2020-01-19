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
    using UnityEngine;

    /**
    * @brief Eye pose data.
    */
    public struct EyePoseData
    {
        /**
        * Left eye pose.
        */
        public Pose LEyePose;

        /**
        * Right eye pose.
        */
        public Pose REyePose;

        /**
        * RGB eye pose.
        */
        public Pose RGBEyePos;
    }

    /**
    * @brief Eye project matrix.
    */
    public struct EyeProjectMatrixData
    {
        /**
        * Left eye projectmatrix.
        */
        public Matrix4x4 LEyeMatrix;

        /**
        * Right eye projectmatrix.
        */
        public Matrix4x4 REyeMatrix;

        /**
        * RGB eye projectmatrix.
        */
        public Matrix4x4 RGBEyeMatrix;
    }
}
