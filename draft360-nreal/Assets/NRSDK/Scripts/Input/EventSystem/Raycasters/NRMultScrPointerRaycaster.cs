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
    using System.Collections.ObjectModel;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// @cond EXCLUDE_FROM_DOXYGEN
    [DisallowMultipleComponent]
    public class NRMultScrPointerRaycaster : NRPointerRaycaster
    {
        public GameObject Mouse;

        private Camera m_UICamera;
        private float m_ScreenWidth;
        private float m_ScreenHeight;
        private Vector3 m_LastTouch;

        protected override void Start()
        {
            base.Start();

            m_UICamera = gameObject.GetComponent<Camera>();
            var resolution = NRPhoneScreen.Resolution;
            m_ScreenWidth = resolution.x;
            m_ScreenHeight = resolution.y;
        }

        public override void Raycast()
        {
            sortedRaycastResults.Clear();
            breakPoints.Clear();

            var zScale = transform.lossyScale.z;
            var amountDistance = (FarDistance - NearDistance) * zScale;
            float distance;
            Ray ray;
            RaycastResult firstHit = default(RaycastResult);
            distance = amountDistance;

            var touch_x = MultiScreenController.SystemButtonState.originTouch.x;
            var touch_y = MultiScreenController.SystemButtonState.originTouch.y;
            var realTouchPos = new Vector3((touch_x + 1) * m_ScreenWidth * 0.5f, (touch_y + 1) * m_ScreenHeight * 0.5f, 0f);
            Vector3 touchpos = MultiScreenController.SystemButtonState.pressing ? realTouchPos : m_LastTouch;
            m_LastTouch = MultiScreenController.SystemButtonState.pressUp ? Vector3.one * 10000f : touchpos;
            touchpos = m_UICamera.ScreenToWorldPoint(touchpos);

            if(Mouse)
                Mouse.transform.position = touchpos + m_UICamera.transform.forward * 0.3f;
            ray = new Ray(touchpos, m_UICamera.transform.forward);

            breakPoints.Add(touchpos);

            eventCamera.farClipPlane = eventCamera.nearClipPlane + distance;
            eventCamera.orthographicSize = m_UICamera.orthographicSize;
            eventCamera.aspect = m_UICamera.aspect;
            eventCamera.transform.position = ray.origin - (ray.direction * eventCamera.nearClipPlane);
            eventCamera.transform.rotation = Quaternion.LookRotation(ray.direction, transform.up);

            Raycast(ray, distance, sortedRaycastResults);

            firstHit = FirstRaycastResult();
            breakPoints.Add(firstHit.isValid ? firstHit.worldPosition : ray.GetPoint(distance));

#if UNITY_EDITOR
            if (showDebugRay)
                Debug.DrawLine(breakPoints[0], breakPoints[1], firstHit.isValid ? Color.green : Color.red);
#endif
        }

        public override void GraphicRaycast(Canvas canvas, bool ignoreReversedGraphics, Ray ray, float distance, NRPointerRaycaster raycaster, List<RaycastResult> raycastResults)
        {
            if (canvas == null) { return; }

            var eventCamera = raycaster.eventCamera;
            var screenCenterPoint = eventCamera.WorldToScreenPoint(eventCamera.transform.position); //new Vector2(m_ScreenWidth * 0.5f, m_ScreenHeight * 0.5f + 1800);
            var graphics = GraphicRegistry.GetGraphicsForCanvas(canvas);

            for (int i = 0; i < graphics.Count; ++i)
            {
                var graphic = graphics[i];
                // -1 means it hasn't been processed by the canvas, which means it isn't actually drawn
                if (graphic.depth == -1 || !graphic.raycastTarget) { continue; }

                if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, screenCenterPoint, eventCamera)) { continue; }

                if (ignoreReversedGraphics && Vector3.Dot(ray.direction, graphic.transform.forward) <= 0f) { continue; }

                if (!graphic.Raycast(screenCenterPoint, eventCamera)) { continue; }

                float dist;
                new Plane(graphic.transform.forward, graphic.transform.position).Raycast(ray, out dist);
                if (dist > distance) { continue; }
                var racastResult = new RaycastResult
                {
                    gameObject = graphic.gameObject,
                    module = raycaster,
                    distance = dist,
                    worldPosition = ray.GetPoint(dist),
                    worldNormal = -graphic.transform.forward,
                    screenPosition = screenCenterPoint,
                    index = raycastResults.Count,
                    depth = graphic.depth,
                    sortingLayer = canvas.sortingLayerID,
                    sortingOrder = canvas.sortingOrder
                };
                raycastResults.Add(racastResult);

                // Send onhover event to NRButton.
                var button = graphic.gameObject.GetComponent<NRButton>();
                if (button != null)
                {
                    button.OnHover(racastResult);
                }
            }
        }
    }
    /// @endcond
}
