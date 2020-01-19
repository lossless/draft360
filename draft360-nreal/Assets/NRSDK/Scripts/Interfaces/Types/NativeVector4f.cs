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
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeVector4f
    {
        [MarshalAs(UnmanagedType.R4)]
        public float X;
        [MarshalAs(UnmanagedType.R4)]
        public float Y;
        [MarshalAs(UnmanagedType.R4)]
        public float Z;
        [MarshalAs(UnmanagedType.R4)]
        public float W;

        public NativeVector4f(Vector4 v)
        {
            X = v.x;
            Y = v.y;
            Z = v.z;
            W = v.w;
        }

        public Vector4 ToUnityVector4()
        {
            return new Vector4(X, Y, Z, W);
        }

        public override string ToString()
        {
            return string.Format("x:{0}, y:{1}, z:{2}, w:{3}", X, Y, Z, W);
        }
    }
}
