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
    using UnityEngine;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;

    /// @cond EXCLUDE_FROM_DOXYGEN
    public class ControllerStateParseUtility : MonoBehaviour
    {
        private static IControllerStateParser[] m_ControllerStateParsers = new IControllerStateParser[NRInput.MAX_CONTROLLER_STATE_COUNT];

        private static Dictionary<ControllerType, System.Type> m_DefaultParserClassTypeDict = new Dictionary<ControllerType, System.Type>()
        {
            {ControllerType.CONTROLLER_TYPE_PHONE, typeof(NRPhoneControllerStateParser)},
            {ControllerType.CONTROLLER_TYPE_NREALLIGHT, typeof(NrealLightControllerStateParser)}
        };

        private static IControllerStateParser CreateControllerStateParser(System.Type parserType)
        {
            if (parserType != null)
            {
                object parserObj = Activator.CreateInstance(parserType);
                if (parserObj is IControllerStateParser)
                    return parserObj as IControllerStateParser;
            }
            return null;
        }

        private static System.Type GetDefaultControllerStateParserType(ControllerType controllerType)
        {
            if (m_DefaultParserClassTypeDict.ContainsKey(controllerType))
                return m_DefaultParserClassTypeDict[controllerType];
            return null;
        }

        public static IControllerStateParser GetControllerStateParser(ControllerType controllerType, int index)
        {
            System.Type parserType = GetDefaultControllerStateParserType(controllerType);
            if (parserType == null)
                m_ControllerStateParsers[index] = null;
            else if (m_ControllerStateParsers[index] == null || parserType != m_ControllerStateParsers[index].GetType())
                m_ControllerStateParsers[index] = CreateControllerStateParser(parserType);
            return m_ControllerStateParsers[index];
        }

        public static void SetDefaultControllerStateParserType(ControllerType controllerType, System.Type parserType)
        {
            if (parserType == null && m_DefaultParserClassTypeDict.ContainsKey(controllerType))
            {
                m_DefaultParserClassTypeDict.Remove(controllerType);
                return;
            }
            if (m_DefaultParserClassTypeDict.ContainsKey(controllerType))
                m_DefaultParserClassTypeDict[controllerType] = parserType;
            else
                m_DefaultParserClassTypeDict.Add(controllerType, parserType);
        }
    }
    /// @endcond
}
