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
    using System.Runtime.InteropServices;

    /// @cond EXCLUDE_FROM_DOXYGEN
    [StructLayout(LayoutKind.Sequential)]
    public struct FrameInfo
    {
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr leftTex;
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr rightTex;
        [MarshalAs(UnmanagedType.Struct)]
        public NativeMat4f pose;

        public FrameInfo(IntPtr left, IntPtr right, NativeMat4f p)
        {
            this.leftTex = left;
            this.rightTex = right;
            this.pose = p;
        }
    }
    /// @endcond
}
