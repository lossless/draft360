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
    public abstract class ControllerProviderBase
    {
        protected ControllerState[] states;
        public bool Inited { get; protected set; }

        public ControllerProviderBase(ControllerState[] states)
        {
            this.states = states;
        }

        public abstract int ControllerCount { get; }

        public abstract void OnPause();

        public abstract void OnResume();

        public abstract void Update();

        public abstract void OnDestroy();

        public virtual void TriggerHapticVibration(int controllerIndex, float durationSeconds = 0.1f, float frequency = 200f, float amplitude = 0.8f) { }

    }
    /// @endcond
}