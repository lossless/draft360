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
    [DisallowMultipleComponent]
    public class ControllerVisualManager : MonoBehaviour
    {
        private ControllerState[] m_States;
        private IControllerVisual[] m_ControllerVisuals;

        private void OnEnable()
        {
            NRInput.OnControllerStatesUpdated += OnControllerStatesUpdated;
        }

        private void OnDisable()
        {
            NRInput.OnControllerStatesUpdated -= OnControllerStatesUpdated;
        }

        public void Init(ControllerState[] states)
        {
            this.m_States = states;
            m_ControllerVisuals = new IControllerVisual[states.Length];
        }

        public void ChangeControllerVisual(int index, ControllerVisualType visualType)
        {
            if (m_ControllerVisuals[index] != null)
                DestroyVisual(index);
            CreateControllerVisual(index, visualType);
        }

        private void OnControllerStatesUpdated()
        {
            UpdateAllVisuals();
        }

        private void UpdateAllVisuals()
        {
            int availableCount = NRInput.GetAvailableControllersCount();
            if(availableCount > 1)
            {
                UpdateVisual(0, m_States[0]);
                UpdateVisual(1, m_States[1]);
            }
            else
            {
                int activeVisual = NRInput.DomainHand == ControllerHandEnum.Right ? 0 : 1;
                int deactiveVisual = NRInput.DomainHand == ControllerHandEnum.Right ? 1 : 0;
                UpdateVisual(activeVisual, m_States[0]);
                DestroyVisual(deactiveVisual);
            }
        }

        private void UpdateVisual(int index, ControllerState state)
        {
            if (m_ControllerVisuals[index] == null && state.controllerType != ControllerType.CONTROLLER_TYPE_UNKNOWN)
            {
                ControllerVisualType visualType = ControllerVisualFactory.GetDefaultVisualType(state.controllerType);
                CreateControllerVisual(index, visualType);
            }
            if (m_ControllerVisuals[index] != null)
            {
                m_ControllerVisuals[index].SetActive(NRInput.ControllerVisualActive);
                m_ControllerVisuals[index].UpdateVisual(state);
            }
        }

        private void CreateControllerVisual(int index, ControllerVisualType visualType)
        {
            GameObject visualGo = ControllerVisualFactory.CreateControllerVisualObject(visualType);
            if (visualGo == null)
                return;
            m_ControllerVisuals[index] = visualGo.GetComponent<IControllerVisual>();
            if (m_ControllerVisuals[index] != null)
            {
                ControllerAnchorEnum ancherEnum = (index == 0 ? ControllerAnchorEnum.RightModelAnchor : ControllerAnchorEnum.LeftModelAnchor);
                visualGo.transform.parent = NRInput.AnchorsHelper.GetAnchor(ancherEnum);
                visualGo.transform.localPosition = Vector3.zero;
                visualGo.transform.localRotation = Quaternion.identity;
                visualGo.transform.localScale = Vector3.one;
            }
            else
            {
                Debug.LogError("The ControllerVisual prefab:"+ visualGo.name+ " does not contain IControllerVisual interface");
                Destroy(visualGo);
            }
        }

        private void DestroyVisual(int index)
        {
            if (m_ControllerVisuals[index] != null)
            {
                m_ControllerVisuals[index].DestroySelf();
                m_ControllerVisuals[index] = null;
            }
        }
    }
    /// @endcond
}
