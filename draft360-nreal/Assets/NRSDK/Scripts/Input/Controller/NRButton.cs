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
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    internal class NRButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Sprite ImageNormal;
        public Sprite ImageHover;
        public Action<string, GameObject, RaycastResult> TriggerEvent;
        public const string Enter = "Enter";
        public const string Hover = "Hover";
        public const string Exit = "Exit";

        private Image ButtonImage;

        void Start()
        {
            ButtonImage = gameObject.GetComponent<Image>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (TriggerEvent != null)
            {
                TriggerEvent(Enter, gameObject, eventData.pointerCurrentRaycast);
            }

            if (ImageHover != null && ButtonImage != null)
            {
                ButtonImage.sprite = ImageHover;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (TriggerEvent != null)
            {
                TriggerEvent(Exit, gameObject, eventData.pointerCurrentRaycast);
            }

            if (ImageNormal != null && ButtonImage != null)
            {
                ButtonImage.sprite = ImageNormal;
            }
        }

        // Get onhover by NRMultScrPointerRaycaster
        public void OnHover(RaycastResult racastResult)
        {
            if (TriggerEvent != null && ButtonImage != null)
            {
                TriggerEvent(Hover, gameObject, racastResult);
            }
        }
    }
}
