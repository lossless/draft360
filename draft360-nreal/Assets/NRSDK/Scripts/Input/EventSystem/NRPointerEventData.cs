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
    public class NRPointerEventData : PointerEventData
    {
        public readonly NRPointerRaycaster raycaster;

        public Vector3 position3D;
        public Quaternion rotation;

        public Vector3 position3DDelta;
        public Quaternion rotationDelta;

        public Vector3 pressPosition3D;
        public Quaternion pressRotation;

        public float pressDistance;
        public GameObject pressEnter;
        public bool pressPrecessed;

        public NRPointerEventData(NRPointerRaycaster raycaster, EventSystem eventSystem) : base(eventSystem)
        {
            this.raycaster = raycaster;
        }

        public virtual bool GetPress()
        {
            if (raycaster is NRMultScrPointerRaycaster)
            {
                return MultiScreenController.SystemButtonState.pressing;
            }
            else
            {
                return NRInput.GetButton(raycaster.RelatedHand, ControllerButton.TRIGGER);
            }
        }

        public virtual bool GetPressDown()
        {
            if (raycaster is NRMultScrPointerRaycaster)
            {
                return MultiScreenController.SystemButtonState.pressDown;
            }
            else
            {
                return NRInput.GetButtonDown(raycaster.RelatedHand, ControllerButton.TRIGGER);
            }
        }

        public virtual bool GetPressUp()
        {
            if (raycaster is NRMultScrPointerRaycaster)
            {
                return MultiScreenController.SystemButtonState.pressUp;
            }
            else
            {
                return NRInput.GetButtonUp(raycaster.RelatedHand, ControllerButton.TRIGGER);
            }
        }

    }
    /// @endcond
}
