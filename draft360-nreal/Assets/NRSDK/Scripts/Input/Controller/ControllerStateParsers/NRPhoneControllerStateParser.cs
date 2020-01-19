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
    * @brief The class parses the touch position of smart phone to usable states by invoking parsing method every frame.
    */
    public class NRPhoneControllerStateParser : IControllerStateParser
    {
        private bool[] _buttons_down = new bool[3];
        private bool[] _buttons_up = new bool[3];
        private bool[] _buttons = new bool[3];
        private bool[] _down = new bool[3];
        private Vector2 _touch;
        private bool _physical_button;
        private int _current_down_btn = -1;
        private bool _last_physical_button;
        private const float PRECISION = 0.000001f;

        public void ParserControllerState(ControllerState state)
        {
            _last_physical_button = _physical_button;
            _physical_button = (Mathf.Abs(state.touchPos.x) > PRECISION || Mathf.Abs(state.touchPos.y) > PRECISION);
            lock (_buttons)
            {
                lock (_down)
                {
                    for (int i = 0; i < _buttons.Length; ++i)
                    {
                        _down[i] = _buttons[i];
                    }
                }

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

                    var sysbtnState = MultiScreenController.SystemButtonState;
                    for (int i = 0; i < sysbtnState.buttons.Length; i++)
                    {
                        _buttons[i] = sysbtnState.buttons[i];
                    }

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

            MultiScreenController.SystemButtonState.originTouch = state.touchPos;
            MultiScreenController.SystemButtonState.pressing = _physical_button;
            MultiScreenController.SystemButtonState.pressDown = (_physical_button && !_last_physical_button);
            MultiScreenController.SystemButtonState.pressUp = (!_physical_button && _last_physical_button);
            state.touchPos = MultiScreenController.SystemButtonState.touch;
            state.isTouching = !state.touchPos.Equals(Vector2.zero);

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
    }
    /// @endcond
}
