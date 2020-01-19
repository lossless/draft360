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
    public class NrealLightControllerVisual : MonoBehaviour, IControllerVisual
    {
        public void DestroySelf()
        {
            if(gameObject)
                Destroy(gameObject);
        }

        public void SetActive(bool isActive)
        {
            if (!gameObject)
                return;
            gameObject.SetActive(isActive);
        }

        public void UpdateVisual(ControllerState state)
        {
            if (!gameObject || !gameObject.activeSelf)
                return;

        }
    }
    /// @endcond
}
