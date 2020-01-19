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
    public struct NativeVector2f
    {
        [MarshalAs(UnmanagedType.R4)]
        public float X;
        [MarshalAs(UnmanagedType.R4)]
        public float Y;

        public NativeVector2f(Vector2 v)
        {
            X = v.x;
            Y = v.y;
        }
        public Vector2 ToUnityVector2()
        {
            return new Vector2(X, Y);
        }
        public override string ToString()
        {
            return string.Format("x:{0}, y:{1}", X, Y);
        }
    }
}
