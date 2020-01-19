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
    using System;
    using UnityEngine;

    /// @cond EXCLUDE_FROM_DOXYGEN
    /**
    * @brief The class parses the raw states of Nreal Light Controller to usable states by invoking parsing method every frame.
    */
    public class NrealLightControllerStateParser : IControllerStateParser
    {
        private enum TouchAreaEnum
        {
            None,
            Center,
            Up,
            Down,
            Left,
            Right
        }

        private bool[] _buttons_down = new bool[3];
        private bool[] _buttons_up = new bool[3];
        private bool[] _buttons = new bool[3];
        private bool[] _down = new bool[3];
        private Vector2 _touch;
        private TouchAreaEnum _currentTouchArea = TouchAreaEnum.None;
        private bool _touch_status;
        private bool _physical_button;
        private bool _physical_button_down;
        private int _current_down_btn = -1;

        private const float Params_Sqrt_2 = 1.414f;
        private const float CenterHalfSideLength = 0.9f / Params_Sqrt_2;
        private const float OKHalfSideLength = 0.9f / Params_Sqrt_2 * 0.5f;
        private const float PRECISION = 0.000001f;

        public void ParserControllerState(ControllerState state)
        {
            try
            {
                _touch_status = (Mathf.Abs(state.touchPos.x) > PRECISION || Mathf.Abs(state.touchPos.y) > PRECISION);
                if (!_touch_status)
                {
                    _touch.x = 0f;
                    _touch.y = 0f;
                }
                else
                {
                    _touch.x = state.touchPos.x;
                    _touch.y = state.touchPos.y;
                }

                UpdateCurrentTouchArea();
                lock (_buttons)
                {
                    lock (_down)
                    {
                        for (int i = 0; i < _buttons.Length; ++i)
                        {
                            _down[i] = _buttons[i];
                        }
                    }

                    _physical_button_down = _physical_button;
                    _physical_button = state.GetButton(ControllerButton.TRIGGER);

                    if (_current_down_btn != -1)
                    {
                        _buttons[_current_down_btn] = _physical_button;
                        if (!_buttons[_current_down_btn])
                            _current_down_btn = -1;
                    }
                    else
                    {
                        _buttons[0] = false;  //Trigger
                        _buttons[1] = false;  //Home
                        _buttons[2] = false;  //App

                        bool _is_down = !_physical_button_down & _physical_button;
                        if (_currentTouchArea == TouchAreaEnum.Center)
                            _buttons[0] = _physical_button && _is_down;
                        else if (_currentTouchArea == TouchAreaEnum.Up)
                            _buttons[1] = _physical_button && _is_down;
                        else if (_currentTouchArea == TouchAreaEnum.Down)
                            _buttons[2] = _physical_button && _is_down;

                        _current_down_btn = -1;
                        for (int i = 0; i < 3; i++)
                        {
                            if (_buttons[i])
                            {
                                _current_down_btn = i;
                                break;
                            }
                        }
                    }

                    lock (_buttons_up)
                    {
                        lock (_buttons_down)
                        {
                            for (int i = 0; i < _buttons.Length; ++i)
                            {
                                _buttons_up[i] = (_down[i] & !_buttons[i]);
                                _buttons_down[i] = (!_down[i] & _buttons[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Debug.LogError("Controller Data Error");
            }

            state.isTouching = _touch_status;
            state.touchPos = _touch;
            state.buttonsState =
                (_buttons[0] ? ControllerButton.TRIGGER : 0)
                | (_buttons[1] ? ControllerButton.HOME : 0)
                | (_buttons[2] ? ControllerButton.APP : 0);
            state.buttonsDown =
                (_buttons_down[0] ? ControllerButton.TRIGGER : 0)
                | (_buttons_down[1] ? ControllerButton.HOME : 0)
                | (_buttons_down[2] ? ControllerButton.APP : 0);
            state.buttonsUp =
                (_buttons_up[0] ? ControllerButton.TRIGGER : 0)
                | (_buttons_up[1] ? ControllerButton.HOME : 0)
                | (_buttons_up[2] ? ControllerButton.APP : 0);
        }

        private void UpdateCurrentTouchArea()
        {
            _currentTouchArea = TouchAreaEnum.None;
            if (!_touch_status)
                return;
            if (_touch.y < -CenterHalfSideLength * 0.9f)
                _currentTouchArea = TouchAreaEnum.Down;
            else if (_touch.y > CenterHalfSideLength * 0.8f)
                _currentTouchArea = TouchAreaEnum.Up;
            else
                _currentTouchArea = TouchAreaEnum.Center;
        }
    }
    /// @endcond
}
