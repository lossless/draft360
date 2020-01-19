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

    /**
    * @brief The interface contains methods for controller to update virtual controller visuals, and to show the feed back of user interactivation.
    */
    public interface IControllerVisual
    {
        void SetActive(bool isActive);
        void UpdateVisual(ControllerState state);
        void DestroySelf();
    }
}