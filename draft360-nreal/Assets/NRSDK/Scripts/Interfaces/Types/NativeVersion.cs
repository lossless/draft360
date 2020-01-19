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
    public struct NRVersion
    {
        [MarshalAs(UnmanagedType.I4)]
        int major;
        [MarshalAs(UnmanagedType.I4)]
        int minor;
        [MarshalAs(UnmanagedType.I4)]
        int revision;

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", major, minor, revision);
        }
    }
}
