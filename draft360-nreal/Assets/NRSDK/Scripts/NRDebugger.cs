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
    using UnityEngine;

    /// @cond EXCLUDE_FROM_DOXYGEN
    /**
     * @brief A tool for log.
     */
    public class NRDebugger
    {
#if UNITY_EDITOR
        public static bool EnableLog = false;
#else
        public static bool EnableLog = false;
#endif

        static public void Log(object message)
        {
            Log(message, null);
        }

        static public void Log(object message, Object context)
        {
            if (EnableLog)
            {
                Debug.Log(message, context);
            }
        }

        static public void LogFormat(string format, params object[] args)
        {
            if (EnableLog)
            {
                Debug.LogFormat(format, args);
            }
        }

        static public void LogError(object message)
        {
            LogError(message, null);
        }

        static public void LogError(object message, Object context)
        {
            if (EnableLog)
            {
                Debug.LogError(message, context);
            }
        }

        static public void LogWarning(object message)
        {
            LogWarning(message, null);
        }

        static public void LogWarning(object message, Object context)
        {
            if (EnableLog)
            {
                Debug.LogWarning(message, context);
            }
        }
    }
    /// @endcond
}
