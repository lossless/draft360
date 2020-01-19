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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// @cond EXCLUDE_FROM_DOXYGEN
    public class CanvasTargetCollector : MonoBehaviour
    {
        private static readonly List<ICanvasRaycastTarget> canvases = new List<ICanvasRaycastTarget>();

        public static void AddTarget(ICanvasRaycastTarget obj)
        {
            if(obj != null)
                canvases.Add(obj);
        }

        public static void RemoveTarget(ICanvasRaycastTarget obj)
        {
            if (obj != null)
                canvases.Remove(obj);
        }

        public static List<ICanvasRaycastTarget> GetCanvases()
        {
            return canvases;
        }
    }
    /// @endcond
}