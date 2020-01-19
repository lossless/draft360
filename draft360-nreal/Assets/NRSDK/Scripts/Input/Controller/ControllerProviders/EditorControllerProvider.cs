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
    /**
    * @brief The class is to emulate controller with mouse and keyboard input in unity editor mode
    */
    public class EditorControllerProvider : ControllerProviderBase
    {
        private Vector2 mouseDelta = new Vector2();

        private const string AXIS_HORIZONTAL = "Horizontal";
        private const string AXIS_VERTICAL = "Vertical";
        private const string AXIS_MOUSE_X = "Mouse X";
        private const string AXIS_MOUSE_Y = "Mouse Y";
        private const float ROTATE_SENSITIVITY = 4;

        public override int ControllerCount
        {
            get
            {
                return 1;
            }
        }

        public EditorControllerProvider(ControllerState[] states) : base(states)
        {
            Inited = true;
        }

        public override void OnPause()
        {

        }

        public override void OnResume()
        {

        }

        public override void Update()
        {
            if (!Inited)
                return;
            UpdateControllerState(0);
        }

        public override void OnDestroy()
        {

        }

        private void UpdateControllerState(int index)
        {
            states[index].controllerType = ControllerType.CONTROLLER_TYPE_EDITOR;
            states[index].availableFeature = ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION;
            states[index].connectionState = ControllerConnectionState.CONTROLLER_CONNECTION_STATE_CONNECTED;
            UpdateRotation(index);

            states[index].touchPos = new Vector2(Input.GetAxis(AXIS_HORIZONTAL), Input.GetAxis(AXIS_VERTICAL));
            states[index].isTouching = !states[index].touchPos.Equals(Vector2.zero);
            states[index].recentered = false;

            states[index].buttonsState =
                (Input.GetKey(KeyCode.Mouse0) ? ControllerButton.TRIGGER : 0)
                | (Input.GetKey(KeyCode.Mouse1) ? ControllerButton.HOME : 0)
                | (Input.GetKey(KeyCode.Mouse2) ? ControllerButton.APP : 0);
            states[index].buttonsDown = 
                (Input.GetKeyDown(KeyCode.Mouse0) ? ControllerButton.TRIGGER : 0)
                | (Input.GetKeyDown(KeyCode.Mouse1) ? ControllerButton.HOME : 0)
                | (Input.GetKeyDown(KeyCode.Mouse2) ? ControllerButton.APP : 0);
            states[index].buttonsUp = 
                (Input.GetKeyUp(KeyCode.Mouse0) ? ControllerButton.TRIGGER : 0)
                | (Input.GetKeyUp(KeyCode.Mouse1) ? ControllerButton.HOME : 0)
                | (Input.GetKeyUp(KeyCode.Mouse2) ? ControllerButton.APP : 0);
            states[index].isCharging = false;
            states[index].batteryLevel = 100;
            CheckRecenter(index);
        }

        private void UpdateRotation(int index)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                mouseDelta.Set(
                    Input.GetAxis(AXIS_MOUSE_X),
                    -Input.GetAxis(AXIS_MOUSE_Y));
                Vector3 deltaDegrees = mouseDelta * ROTATE_SENSITIVITY;
                Quaternion yaw = Quaternion.AngleAxis(deltaDegrees.x, Vector3.up);
                Quaternion pitch = Quaternion.AngleAxis(deltaDegrees.y, Vector3.right);
                states[index].rotation = states[index].rotation * yaw * pitch;
            }
        }

        private void CheckRecenter(int index)
        {
            if (states[index].GetButtonDown(ControllerButton.APP))
            {
                states[index].recentered = true;
                states[index].rotation = Quaternion.identity;
            }
        }
    }
    /// @endcond
}
