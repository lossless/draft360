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

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeFov4f
    {
        /// The tangent of the angle between the viewing vector and the left edge of the field of view. The value is positive.
        [MarshalAs(UnmanagedType.R4)]
        public float left_tan;

        /// The tangent of the angle between the viewing vector and the right edge of the field of view. The value is positive.
        [MarshalAs(UnmanagedType.R4)]
        public float right_tan;

        /// The tangent of the angle between the viewing vector and the top edge of the field of view. The value is positive.
        [MarshalAs(UnmanagedType.R4)]
        public float top_tan;

        /// The tangent of the angle between the viewing vector and the bottom edge of the field of view. The value is positive.
        [MarshalAs(UnmanagedType.R4)]
        public float bottom_tan;
    }
}