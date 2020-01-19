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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// @cond EXCLUDE_FROM_DOXYGEN
    internal static class ControllerProviderFactory
    {
        public static Type androidControllerProviderType = typeof(NRControllerProvider);

        public static ControllerProviderBase CreateControllerProvider(ControllerState[] states)
        {
            ControllerProviderBase provider = CreateControllerProvider(androidControllerProviderType, states);
            return provider;
        }

        private static ControllerProviderBase CreateControllerProvider(Type providerType, ControllerState[] states)
        {
            if (providerType != null)
            {
                object parserObj = Activator.CreateInstance(providerType, new object[] { states });
                if (parserObj is ControllerProviderBase)
                    return parserObj as ControllerProviderBase;
            }
            return null;
        }
    }
    /// @endcond
}