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
    public class ControllerTracker : MonoBehaviour {
        public ControllerHandEnum defaultHandEnum;
        public NRPointerRaycaster raycaster;
        public Transform modelAnchor;

        private float m_VerifyYAngle = 0f;
        private bool m_IsEnabled;
        private bool m_Is6dof;
        private Vector3 m_DefaultLocalOffset;
        private Vector3 m_TargetPos = Vector3.zero;
        private bool m_IsMovingToTarget;
        private Vector3 m_LaserDefaultLocalOffset;
        private Vector3 m_ModelDefaultLocalOffset;

        private const float TrackTargetSpeed = 6f;
        private const float MaxDistanceFromTarget = 0.12f;

        private Transform CameraCenter
        {
            get
            {
                return NRInput.CameraCenter;
            }
        }

        private void Awake()
        {
            m_DefaultLocalOffset = transform.localPosition;
            m_LaserDefaultLocalOffset = raycaster.transform.localPosition;
            m_ModelDefaultLocalOffset = modelAnchor.localPosition;
        }

        private void OnEnable()
        {
            NRInput.OnControllerRecentering += OnRecentering;
            NRInput.OnControllerStatesUpdated += OnControllerStatesUpdated;
        }

        private void OnDisable()
        {
            NRInput.OnControllerRecentering -= OnRecentering;
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
            m_IsEnabled = NRInput.CheckControllerAvailable(defaultHandEnum);
            raycaster.gameObject.SetActive(m_IsEnabled && NRInput.RaycastMode == RaycastModeEnum.Laser);
            modelAnchor.gameObject.SetActive(m_IsEnabled);
            if (m_IsEnabled)
                TrackPose();
        }

        private void TrackPose()
        {
            m_Is6dof = NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_POSITION)
                && NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION);
            if (m_Is6dof)
            {
                raycaster.transform.localPosition = Vector3.zero;
                modelAnchor.localPosition = Vector3.zero;
                UpdatePosition();
            }
            else
            {
                raycaster.transform.localPosition = m_LaserDefaultLocalOffset;
                modelAnchor.localPosition = m_ModelDefaultLocalOffset;
                SmoothTrackTargetPosition();
            }
            UpdateRotation();
        }

        private void UpdatePosition()
        {
            transform.position = NRInput.GetPosition(defaultHandEnum);
        }

        private void UpdateRotation()
        {
            transform.localRotation = NRInput.GetRotation(defaultHandEnum);
            transform.Rotate(Vector3.up * m_VerifyYAngle, Space.World);
        }

        private void SmoothTrackTargetPosition()
        {
            int sign = defaultHandEnum == ControllerHandEnum.Right ? 1 : -1;
            m_TargetPos = CameraCenter.position + Vector3.up * m_DefaultLocalOffset.y
                + new Vector3(CameraCenter.right.x, 0f, CameraCenter.right.z).normalized * Mathf.Abs(m_DefaultLocalOffset.x) * sign
                + new Vector3(CameraCenter.forward.x, 0f, CameraCenter.forward.z).normalized * m_DefaultLocalOffset.z;
            if (!m_IsMovingToTarget)
            {
                if (Vector3.Distance(transform.position, m_TargetPos) > MaxDistanceFromTarget)
                    m_IsMovingToTarget = true;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, m_TargetPos, TrackTargetSpeed * Time.deltaTime);
                if (Vector3.Distance(m_TargetPos, transform.position) < 0.02f)
                    m_IsMovingToTarget = false;
            }
        }

        private void OnRecentering()
        {
            Vector3 horizontalFoward = Vector3.ProjectOnPlane(CameraCenter.forward, Vector3.up);
            m_VerifyYAngle = Mathf.Sign(Vector3.Cross(Vector3.forward, horizontalFoward).y) * Vector3.Angle(horizontalFoward, Vector3.forward);
        }
    }
    /// @endcond
}