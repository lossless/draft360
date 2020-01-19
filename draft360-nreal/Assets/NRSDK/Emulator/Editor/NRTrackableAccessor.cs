/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.NREditor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal abstract class NRTrackableAccessor
    {

        protected NRTrackableBehaviour m_Target;

        public abstract void ApplyDataProperties();

        public abstract void ApplyDataAppearance();
    }


    internal class NRAccessorFactory
    {
        public static NRTrackableAccessor Create(NRTrackableBehaviour target)
        {
            if (target is NRTrackableImageBehaviour)
            {
                return new NRImageTargetAccessor((NRTrackableImageBehaviour)target);
            }
            Debug.LogError(target.GetType().ToString() + "is not derived from NRTrackableImageBehaviour");
            return null;
        }
    }

}
