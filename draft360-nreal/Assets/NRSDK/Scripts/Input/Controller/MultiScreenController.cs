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
    using UnityEngine.EventSystems;

    public class SystemButtonState
    {
        //TRIGGER APP HOME
        public bool[] buttons = new bool[3];
        public Vector2 touch;
        public Vector2 originTouch;

        public bool pressing;
        public bool pressDown;
        public bool pressUp;
    }

    public class MultiScreenController : SingletonBehaviour<MultiScreenController>
    {
        public static SystemButtonState SystemButtonState = new SystemButtonState();

        // System defined button.
        [SerializeField]
        private NRButton Trigger;
        [SerializeField]
        private NRButton App;
        [SerializeField]
        private NRButton Home;

        new void Awake()
        {
            if (isDirty) return;
            base.Awake();
        }

        new void OnDestroy()
        {
            if (isDirty) return;
            base.OnDestroy();
        }

        public void Init()
        {
            InitSystemButtonEvent();
            AutoResizeButtons();
        }

        private void InitSystemButtonEvent()
        {
            Trigger.TriggerEvent += OnBtnTrigger;
            App.TriggerEvent += OnBtnTrigger;
            Home.TriggerEvent += OnBtnTrigger;
        }

        private void OnBtnTrigger(string key, GameObject go, RaycastResult racastInfo)
        {
            if (key.Equals(NRButton.Enter))
            {
                if (go == Trigger.gameObject)
                {
                    SystemButtonState.buttons[0] = true;
                }
                if (go == Home.gameObject)
                {
                    SystemButtonState.buttons[1] = true;
                }
                if (go == App.gameObject)
                {
                    SystemButtonState.buttons[2] = true;
                }
            }
            else if (key.Equals(NRButton.Exit))
            {
                if (go == Trigger.gameObject)
                {
                    SystemButtonState.buttons[0] = false;
                }
                if (go == Home.gameObject)
                {
                    SystemButtonState.buttons[1] = false;
                }
                if (go == App.gameObject)
                {
                    SystemButtonState.buttons[2] = false;
                }
            }

            if (go == Trigger.gameObject 
                &&(key.Equals(NRButton.Hover) || key.Equals(NRButton.Enter)))
            {
                CalculateTouchPos(go, racastInfo);
            }
            else
            {
                SystemButtonState.touch = Vector2.zero;
            }
        }

        private void CalculateTouchPos(GameObject go, RaycastResult racastInfo)
        {
            RectTransform rect = go.GetComponent<RectTransform>();
            Vector3[] v = new Vector3[4];
            rect.GetWorldCorners(v);

            var touchToCenter = racastInfo.worldPosition - go.transform.position;
            var rightToCenter = (v[3] - v[0]) * 0.5f;
            var topToCenter = (v[1] - v[0]) * 0.5f;
            var halfWidth = (v[3] - v[0]).magnitude * 0.5f;
            var halfHeight = (v[1] - v[0]).magnitude * 0.5f;
            var alpha = Vector3.Angle(rightToCenter, touchToCenter);
            var touchToX = (touchToCenter * Mathf.Cos(alpha * Mathf.PI / 180)).magnitude;
            var touchToY = (touchToCenter * Mathf.Sin(alpha * Mathf.PI / 180)).magnitude;

            bool x_forward = Vector3.Dot(touchToCenter, rightToCenter) > 0;
            bool y_forward = Vector3.Dot(touchToCenter, topToCenter) > 0;

            var touchx = touchToX > halfWidth ? (x_forward ? 1f : -1f) : (x_forward ? touchToX / halfWidth : -touchToX / halfWidth);
            var touchy = touchToY > halfHeight ? (y_forward ? 1f : -1f) : (y_forward ? touchToY / halfHeight : -touchToY / halfHeight);
            SystemButtonState.touch = new Vector2(touchx, touchy);
        }

        // Resize the buttons by resolution.width
        private void AutoResizeButtons()
        {
            Transform buttonRoot = App.transform.parent;
            var resolution = NRPhoneScreen.Resolution;
            float t = resolution.x / NRPhoneScreen.DefaultWidth;

            buttonRoot.localScale = t * Vector3.one;
            var rectTransform = buttonRoot.GetComponent<RectTransform>();
            rectTransform.anchoredPosition3D = new Vector3(0, rectTransform.anchoredPosition3D.y * t, 0);
        }

#if UNITY_EDITOR
        private void OnDisable()
        {
            if(!NRInput.EmulateVirtualDisplayInEditor)
                ClearSystemButtonState();
        }

        private void ClearSystemButtonState()
        {

        }
#endif
    }
}
