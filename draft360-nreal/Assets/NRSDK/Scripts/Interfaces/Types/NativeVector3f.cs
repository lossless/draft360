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
    public struct NativeVector3f
    {
        [MarshalAs(UnmanagedType.R4)]
        public float X;
        [MarshalAs(UnmanagedType.R4)]
        public float Y;
        [MarshalAs(UnmanagedType.R4)]
        public float Z;

        public NativeVector3f(Vector3 v)
        {
            X = v.x;
            Y = v.y;
            Z = v.z;
        }

        public float this[int i]
        {
            get
            {
                if (i == 0)
                {
                    return X;
                }
                if (i == 1)
                {
                    return Y;
                }
                if (i == 2)
                {
                    return Z;
                }
                return -1;
            }
            set
            {
                if (i == 0)
                {
                    X = value;
                }
                if (i == 1)
                {
                    Y = value;
                }
                if (i == 2)
                {
                    Z = value;
                }
            }
        }

        public Vector3 ToUnityVector3()
        {
            return new Vector3(X, Y, -Z);
        }
        public override string ToString()
        {
            return string.Format("x:{0}, y:{1}, z:{2}", X, Y, Z);
        }
    }
}
