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
    * @brief The interface contains methods for controller provider to parse raw sates to usable states.
    */
    public interface IControllerStateParser
    {
        void ParserControllerState(ControllerState state);
    }
}