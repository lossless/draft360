/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* NRSDK is distributed in the hope that it will be usefull                                                              
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// @cond EXCLUDE_FROM_DOXYGEN
    public class FinchShiftControllerVisual : MonoBehaviour, IControllerVisual
    {
        [Serializable]
        public class Buttons
        {
            public ControllerButton buttonType;

            /// <summary>
            /// Pressed button visualisation model.
            /// </summary>
            public MeshRenderer buttonMeshRenderer;

            private readonly Color pressed = new Color(0.671f, 0.671f, 0.671f);
            private readonly Color unpressed = Color.black;
            /// <summary>
            /// Update pressing state of buttons.
            /// </summary>
            /// <param name="isPressing"></param>
            public void UpdateState(bool isPressing)
            {
                buttonMeshRenderer.material.color = isPressing ? pressed : unpressed;
                buttonMeshRenderer.material.SetColor("_EmissionColor", isPressing ? pressed : unpressed);
            }
        }

        [Serializable]
        public class BatteryLevel
        {
            /// <summary>
            /// Sprite, visualizing a certain level of charge.
            /// </summary>
            public Sprite BatteryMaterial;

            /// <summary>
            /// The level of charge in percent.
            /// </summary>
            [Range(0, 100)]
            public int MinimumBatteryBorder;
        }

        /// <summary>
        /// Object to visualise controller state.
        /// </summary>
        public GameObject Model;

        /// <summary>
        /// List of visualisable buttons.
        /// </summary>
        public Buttons[] buttonsArr = new Buttons[0];

        /// <summary>
        /// Battery level renderer.
        /// </summary>
        public SpriteRenderer BatteryObject;

        /// <summary>
        /// Array of different charge level materials.
        /// </summary>
        public BatteryLevel[] BatteryLevels = new BatteryLevel[4];

        /// <summary>
        /// Touch point model element transform.
        /// </summary>
        public Transform TouchPoint;

        private float m_BatteryLevel;
        private float m_TouchPointPower;
        private ControllerState m_ControllerState;

        private const float EPSILON = 0.05f;
        private const float CHARGE_LEVEL_EPSILON = 1.5f;
        private const float TOUCH_POINT_DEPTH = 0.001f;
        private const float TOUCHPAD_RADIUS = 0.0175f;
        private const float TOUCHPOINT_RADIUS = 0.0056f;
        private const float SCALE_TIMER = 0.15f;

        public void SetActive(bool isActive)
        {
            if (!gameObject)
                return;
            gameObject.SetActive(isActive);
        }

        public void DestroySelf()
        {
            if (gameObject)
                Destroy(gameObject);
        }

        public void UpdateVisual(ControllerState state)
        {
            if (!gameObject || !gameObject.activeSelf)
                return;
            if (state == null)
                return;
            m_ControllerState = state;
            ButtonUpdate();
            BatteryUpdate();
            UpdateTouchpad();
        }

        private void ButtonUpdate()
        {
            foreach (var b in buttonsArr)
            {
                b.UpdateState(m_ControllerState.GetButton(b.buttonType));
            }
        }

        private void BatteryUpdate()
        {
            if (BatteryObject == null)
            {
                return;
            }

            bool isBatteryActive = m_ControllerState.IsFeatureAvailable(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_BATTERY);
            if (BatteryObject.gameObject.activeSelf != isBatteryActive)
            {
                BatteryObject.gameObject.SetActive(isBatteryActive);
            }

            float currentBatteryLevel = Mathf.Clamp(m_ControllerState.batteryLevel, 0f, 99.9f);
            if (isBatteryActive && Math.Abs(currentBatteryLevel - m_BatteryLevel) > CHARGE_LEVEL_EPSILON)
            {
                Sprite batterySprite = null;
                float maxBorder = 0;

                m_BatteryLevel = currentBatteryLevel;

                foreach (BatteryLevel i in BatteryLevels)
                {
                    if (currentBatteryLevel > i.MinimumBatteryBorder && maxBorder <= i.MinimumBatteryBorder)
                    {
                        maxBorder = i.MinimumBatteryBorder;
                        batterySprite = i.BatteryMaterial;
                    }
                }

                BatteryObject.sprite = batterySprite;
            }
        }

        private void UpdateTouchpad()
        {
            Vector3 size = new Vector3(TOUCHPOINT_RADIUS, TOUCH_POINT_DEPTH, TOUCHPOINT_RADIUS);
            float speed = Time.deltaTime / Mathf.Max(EPSILON, SCALE_TIMER) * (m_ControllerState.isTouching ? 1 : -1);
            m_TouchPointPower = Mathf.Clamp01(m_TouchPointPower + speed);

            if (TouchPoint != null)
            {
                TouchPoint.localScale = m_ControllerState.GetButton(ControllerButton.TOUCHPAD_BUTTON) ? Vector3.zero : size * m_TouchPointPower;

                if (m_ControllerState.isTouching)
                {
                    TouchPoint.localPosition = new Vector3(m_ControllerState.touchPos.x, TouchPoint.localPosition.y, m_ControllerState.touchPos.y) * TOUCHPAD_RADIUS;
                }
            }
        }
    }
    /// @endcond
}
