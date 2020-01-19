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
    public class GazeTracker : MonoBehaviour
    {
        [SerializeField]
        private NRPointerRaycaster m_Raycaster;
        private bool m_IsEnabled;

        private Transform CameraCenter
        {
            get
            {
                return NRInput.CameraCenter;
            }
        }

        private void Start()
        {
            OnControllerStatesUpdated();
        }

        private void OnEnable()
        {
            NRInput.OnControllerStatesUpdated += OnControllerStatesUpdated;
        }

        private void OnDisable()
        {
            NRInput.OnControllerStatesUpdated -= OnControllerStatesUpdated;
        }

        private void OnControllerStatesUpdated()
        {
            UpdateTracker();
        }

        private void UpdateTracker()
        {
            if (CameraCenter == null)
                return;
            m_IsEnabled = NRInput.RaycastMode == RaycastModeEnum.Gaze;
            m_Raycaster.gameObject.SetActive(m_IsEnabled);
            if (m_IsEnabled)
            {
                transform.position = CameraCenter.position;
                transform.rotation = CameraCenter.rotation;
            }
        }
    }
    /// @endcond
}
