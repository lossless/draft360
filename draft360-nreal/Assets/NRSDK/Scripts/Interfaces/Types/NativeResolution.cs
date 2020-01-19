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
    public struct NativeResolution
    {
        [MarshalAs(UnmanagedType.I4)]
        public int width;
        [MarshalAs(UnmanagedType.I4)]
        public int height;

        public NativeResolution(int w, int h)
        {
            this.width = w;
            this.height = h;
        }

        public override string ToString()
        {
            return string.Format("Screen width : {0}  height:{1}", width, height);
        }
    }
}
