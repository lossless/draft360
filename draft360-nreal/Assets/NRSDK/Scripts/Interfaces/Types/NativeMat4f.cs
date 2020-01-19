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
    public struct NativeMat4f
    {
        [MarshalAs(UnmanagedType.Struct)]
        public NativeVector4f column0;
        [MarshalAs(UnmanagedType.Struct)]
        public NativeVector4f column1;
        [MarshalAs(UnmanagedType.Struct)]
        public NativeVector4f column2;
        [MarshalAs(UnmanagedType.Struct)]
        public NativeVector4f column3;

        public NativeMat4f(Matrix4x4 m)
        {
            column0 = new NativeVector4f(m.GetColumn(0));
            column1 = new NativeVector4f(m.GetColumn(1));
            column2 = new NativeVector4f(m.GetColumn(2));
            column3 = new NativeVector4f(m.GetColumn(3));
        }

        public Matrix4x4 ToUnityMat4f()
        {
            Matrix4x4 m = new Matrix4x4();
            m.SetColumn(0, column0.ToUnityVector4());
            m.SetColumn(1, column1.ToUnityVector4());
            m.SetColumn(2, column2.ToUnityVector4());
            m.SetColumn(3, column3.ToUnityVector4());
            return m;
        }

        public static NativeMat4f identity
        {
            get
            {
                return new NativeMat4f(Matrix4x4.identity);
            }
        }

        public override string ToString()
        {
            return ToUnityMat4f().ToString();
        }
    }
}
