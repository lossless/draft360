using System;
using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    public class ControllerInfoDisplayUI : MonoBehaviour
    {
        public Text mainInfoText;
        public Text extraInfoText;

        private string m_ExtraInfoStr;
        private int m_MaxLine = 20;
        private ControllerHandEnum m_CurrentDebugHand = ControllerHandEnum.Right;

        private void Update()
        {
            if (NRInput.GetAvailableControllersCount() < 2)
            {
                m_CurrentDebugHand = NRInput.DomainHand;
            }
            else
            {
                if (NRInput.GetButtonDown(ControllerHandEnum.Right, ControllerButton.TRIGGER))
                {
                    m_CurrentDebugHand = ControllerHandEnum.Right;
                }
                else if (NRInput.GetButtonDown(ControllerHandEnum.Left, ControllerButton.TRIGGER))
                {
                    m_CurrentDebugHand = ControllerHandEnum.Left;
                }
            }

            if (NRInput.GetButtonDown(m_CurrentDebugHand, ControllerButton.TRIGGER))
                AddExtraInfo("trigger_btn_down");

            if (NRInput.GetButtonDown(m_CurrentDebugHand, ControllerButton.HOME))
                AddExtraInfo("home_btn_down");

            if (NRInput.GetButtonDown(m_CurrentDebugHand, ControllerButton.APP))
                AddExtraInfo("app_btn_down");

            if (NRInput.GetButtonUp(m_CurrentDebugHand, ControllerButton.TRIGGER))
                AddExtraInfo("trigger_btn_up");

            if (NRInput.GetButtonUp(m_CurrentDebugHand, ControllerButton.HOME))
                AddExtraInfo("home_btn_up");

            if (NRInput.GetButtonUp(m_CurrentDebugHand, ControllerButton.APP))
                AddExtraInfo("app_btn_up");

            FollowMainCam();
            RefreshInfoTexts();
        }

        private void FollowMainCam()
        {
            transform.position = Camera.main.transform.position;
            transform.rotation = Camera.main.transform.rotation;
        }

        private void RefreshInfoTexts()
        {
            mainInfoText.text =
                "controller count: " + NRInput.GetAvailableControllersCount().ToString() + "\n"
                + "type: " + NRInput.GetControllerType().ToString() + "\n"
                + "current debug hand: " + m_CurrentDebugHand.ToString() + "\n"
                + "position available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_POSITION).ToString() + "\n"
                + "rotation available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION).ToString() + "\n"
                + "gyro available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_GYRO).ToString() + "\n"
                + "accel available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ACCEL).ToString() + "\n"
                + "mag available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_MAG).ToString() + "\n"
                + "battery available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_BATTERY).ToString() + "\n"
                + "vibration available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_HAPTIC_VIBRATE).ToString() + "\n"
                + "rotation: " + NRInput.GetRotation(m_CurrentDebugHand).ToString("F3") + "\n"
                + "position: " + NRInput.GetPosition(m_CurrentDebugHand).ToString("F3") + "\n"
                + "touch: " + NRInput.GetTouch(m_CurrentDebugHand).ToString("F3") + "\n"
                + "trigger button: " + NRInput.GetButton(m_CurrentDebugHand, ControllerButton.TRIGGER).ToString() + "\n"
                + "home button: " + NRInput.GetButton(m_CurrentDebugHand, ControllerButton.HOME).ToString() + "\n"
                + "app button: " + NRInput.GetButton(m_CurrentDebugHand, ControllerButton.APP).ToString() + "\n"
                + "grip button: " + NRInput.GetButton(m_CurrentDebugHand, ControllerButton.GRIP).ToString() + "\n"
                + "touchpad button: " + NRInput.GetButton(m_CurrentDebugHand, ControllerButton.TOUCHPAD_BUTTON).ToString() +"\n"
                + "gyro: " + NRInput.GetGyro(m_CurrentDebugHand).ToString("F3") + "\n"
                + "accel: " + NRInput.GetAccel(m_CurrentDebugHand).ToString("F3") + "\n"
                + "mag: " + NRInput.GetMag(m_CurrentDebugHand).ToString("F3") + "\n"
                + "battery: " + NRInput.GetControllerBattery(m_CurrentDebugHand);
            extraInfoText.text = m_ExtraInfoStr;
        }

        private void AddExtraInfo(string infoStr)
        {
            if (string.IsNullOrEmpty(infoStr))
                return;
            if (string.IsNullOrEmpty(m_ExtraInfoStr))
                m_ExtraInfoStr = infoStr;
            else
                m_ExtraInfoStr = m_ExtraInfoStr + Environment.NewLine + infoStr;
            int count = m_ExtraInfoStr.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length;
            if (count > m_MaxLine)
                m_ExtraInfoStr = m_ExtraInfoStr.Substring(m_ExtraInfoStr.IndexOf(Environment.NewLine) + Environment.NewLine.Length);
        }
    }
}
