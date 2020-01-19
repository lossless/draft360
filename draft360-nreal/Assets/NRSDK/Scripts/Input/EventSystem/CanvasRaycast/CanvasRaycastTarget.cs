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
    using UnityEngine.EventSystems;

    /// @cond EXCLUDE_FROM_DOXYGEN
    public interface ICanvasRaycastTarget
    {
        Canvas canvas { get; }
        bool enabled { get; }
        bool ignoreReversedGraphics { get; }
    }
    /// @endcond

    /**
    * @brief The class enables an UGUI Canvas and its children to be interactive with NRInput raycasters.
    */
    [RequireComponent(typeof(Canvas))]
    [DisallowMultipleComponent]
    public class CanvasRaycastTarget : UIBehaviour, ICanvasRaycastTarget
    {
        private Canvas m_canvas;
        [SerializeField]
        private bool m_IgnoreReversedGraphics = true;

        public virtual Canvas canvas { get { return m_canvas ?? (m_canvas = GetComponent<Canvas>()); } }
        public bool ignoreReversedGraphics { get { return m_IgnoreReversedGraphics; } set { m_IgnoreReversedGraphics = value; } }

        protected override void OnEnable()
        {
            base.OnEnable();
            CanvasTargetCollector.AddTarget(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CanvasTargetCollector.RemoveTarget(this);
        }
    }
}