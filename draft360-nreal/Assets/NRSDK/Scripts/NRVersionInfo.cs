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
    /**
    * @brief Holds information about Nreal SDK version info
    */
    public class NRVersionInfo
    {
        public static string GetVersion()
        {
            return NativeVersion.GetVersion();
        }
    }
}
