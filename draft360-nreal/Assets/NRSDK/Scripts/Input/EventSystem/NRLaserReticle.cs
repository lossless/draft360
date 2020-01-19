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
    using UnityEngine;
    /// @cond EXCLUDE_FROM_DOXYGEN
    public class NRLaserReticle : MonoBehaviour
    {
        public enum ReticleState
        {
            Hide,
            Normal,
            Hover,
        }

        [SerializeField]
        private NRPointerRaycaster m_Raycaster;
        [SerializeField]
        private GameObject m_DefaultVisual;
        [SerializeField]
        private GameObject m_HoverVisual;

        private GameObject m_HitTarget;
        private bool m_IsVisible = true;

        public float defaultDistance = 2.5f;
        public float reticleSizeRatio = 0.02f;

        private Transform m_CameraRoot
        {
            get
            {
                return NRInput.CameraCenter;
            }
        }

        public GameObject HitTarget
        {
            get
            {
                return m_HitTarget;
            }
        }

        private void Awake()
        {
            SwitchReticleState(ReticleState.Hide);
            defaultDistance = Mathf.Clamp(defaultDistance, m_Raycaster.NearDistance, m_Raycaster.FarDistance);
        }

        protected virtual void LateUpdate()
        {
            if (!m_IsVisible || !NRInput.ReticleVisualActive)
            {
                SwitchReticleState(ReticleState.Hide);
                return;
            }
            var result = m_Raycaster.FirstRaycastResult();
            var points = m_Raycaster.BreakPoints;
            var pointCount = points.Count;
            if (result.isValid)
            {
                SwitchReticleState(ReticleState.Hover);
                transform.position = result.worldPosition;
                transform.rotation = Quaternion.LookRotation(result.worldNormal, m_Raycaster.transform.forward);

                m_HitTarget = result.gameObject;
            }
            else
            {
                SwitchReticleState(ReticleState.Normal);
                if (pointCount != 0)
                {
                    transform.localPosition = Vector3.forward * defaultDistance;
                    transform.localRotation = Quaternion.identity;
                }

                m_HitTarget = null;
            }
            if (m_CameraRoot)
                transform.localScale = Vector3.one * reticleSizeRatio * (transform.position - m_CameraRoot.transform.position).magnitude;
        }

        private void OnDisable()
        {
            SwitchReticleState(ReticleState.Hide);
        }

        private void SwitchReticleState(ReticleState state)
        {
            switch (state)
            {
                case ReticleState.Hide:
                    m_DefaultVisual.SetActive(false);
                    m_HoverVisual.SetActive(false);
                    break;
                case ReticleState.Normal:
                    m_DefaultVisual.SetActive(true);
                    m_HoverVisual.SetActive(false);
                    break;
                case ReticleState.Hover:
                    m_DefaultVisual.SetActive(false);
                    m_HoverVisual.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        public void SetVisible(bool isVisible)
        {
            this.m_IsVisible = isVisible;
        }
    }
    /// @endcond
}


