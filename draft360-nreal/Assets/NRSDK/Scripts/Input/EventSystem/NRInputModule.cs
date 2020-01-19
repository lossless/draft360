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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// @cond EXCLUDE_FROM_DOXYGEN
    public class NRInputModule : BaseInputModule
    {
        private int m_processedFrame;
        private static readonly List<NRPointerRaycaster> raycasters = new List<NRPointerRaycaster>();

        public static bool Active { get { return m_Instance != null; } }
        public static Vector2 ScreenCenterPoint { get { return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f); } }
        private static bool isApplicationQuitting = false;

        private static NRInputModule m_Instance;
        public static NRInputModule Instance
        {
            get
            {
                Initialize();
                return m_Instance;
            }
        }

        public override void UpdateModule()
        {
            Initialize();
            if (isActiveAndEnabled && EventSystem.current.currentInputModule != this)
                ProcessRaycast();
        }

        protected virtual void ProcessRaycast()
        {
            if (m_processedFrame == Time.frameCount)
                return;
            m_processedFrame = Time.frameCount;

            RaycastAll();
        }

        private void RaycastAll()
        {
            for (int i = 0; i < raycasters.Count; i++)
            {
                var raycaster = raycasters[i];
                if (raycaster == null)
                    continue;
                raycaster.Raycast();

                var result = raycaster.FirstRaycastResult();
                var scrollDelta = raycaster.GetScrollDelta();
                var raycasterPos = raycaster.BreakPoints[0];
                var raycasterRot = raycaster.transform.rotation;

                var hoverEventData = raycaster.HoverEventData;
                if (hoverEventData == null) { continue; }

                hoverEventData.Reset();
                hoverEventData.delta = Vector2.zero;
                hoverEventData.scrollDelta = scrollDelta;
                hoverEventData.position = ScreenCenterPoint;
                hoverEventData.pointerCurrentRaycast = result;

                hoverEventData.position3DDelta = raycasterPos - hoverEventData.position3D;
                hoverEventData.position3D = raycasterPos;
                hoverEventData.rotationDelta = Quaternion.Inverse(hoverEventData.rotation) * raycasterRot;
                hoverEventData.rotation = raycasterRot;

                for (int j = 0, jmax = raycaster.ButtonEventDataList.Count; j < jmax; ++j)
                {
                    var buttonEventData = raycaster.ButtonEventDataList[j];
                    if (buttonEventData == null || buttonEventData == hoverEventData) { continue; }

                    buttonEventData.Reset();
                    buttonEventData.delta = Vector2.zero;
                    buttonEventData.scrollDelta = scrollDelta;
                    buttonEventData.position = ScreenCenterPoint;
                    buttonEventData.pointerCurrentRaycast = result;

                    buttonEventData.position3DDelta = hoverEventData.position3DDelta;
                    buttonEventData.position3D = hoverEventData.position3D;
                    buttonEventData.rotationDelta = hoverEventData.rotationDelta;
                    buttonEventData.rotation = hoverEventData.rotation;
                }

                ProcessPress(hoverEventData);
                ProcessMove(hoverEventData);
                ProcessDrag(hoverEventData);

                for (int j = 1, jmax = raycaster.ButtonEventDataList.Count; j < jmax; ++j)
                {
                    var buttonEventData = raycaster.ButtonEventDataList[j];
                    if (buttonEventData == null || buttonEventData == hoverEventData) { continue; }

                    buttonEventData.pointerEnter = hoverEventData.pointerEnter;

                    ProcessPress(buttonEventData);
                    ProcessDrag(buttonEventData);
                }
            }
        }

        public static void Initialize()
        {
            if (Active || isApplicationQuitting) { return; }

            var instances = FindObjectsOfType<NRInputModule>();
            if (instances.Length > 0)
            {
                m_Instance = instances[0];
                if (instances.Length > 1)
                    Debug.LogWarning("Multiple NRInputModule not supported!");
            }

            if (!Active)
            {
                EventSystem eventSystem = EventSystem.current;
                if (eventSystem == null)
                {
                    eventSystem = FindObjectOfType<EventSystem>();
                }
                if (eventSystem == null)
                {
                    eventSystem = new GameObject("[EventSystem]").AddComponent<EventSystem>();
                }
                if (eventSystem == null)
                {
                    Debug.LogWarning("EventSystem not found or create fail!");
                    return;
                }

                m_Instance = eventSystem.gameObject.AddComponent<NRInputModule>();
                DontDestroyOnLoad(eventSystem.gameObject);
            }
        }

        public override void Process()
        {
            Initialize();
            if (isActiveAndEnabled)
            {
                ProcessRaycast();
            }
        }

        private void OnApplicationQuit()
        {
            isApplicationQuitting = true;
        }

        public static void AddRaycaster(NRPointerRaycaster raycaster)
        {
            if (raycaster == null)
            {
                return;
            }
            Initialize();
            raycasters.Add(raycaster);
        }

        public static void RemoveRaycaster(NRPointerRaycaster raycaster)
        {
            raycasters.Remove(raycaster);
        }

        public static readonly Comparison<RaycastResult> defaultRaycastComparer = RaycastComparer;
        private static int RaycastComparer(RaycastResult lhs, RaycastResult rhs)
        {
            if (lhs.module != rhs.module)
            {
                if (lhs.module.eventCamera != null && rhs.module.eventCamera != null && lhs.module.eventCamera.depth != rhs.module.eventCamera.depth)
                {
                    if (lhs.module.eventCamera.depth < rhs.module.eventCamera.depth) { return 1; }
                    if (lhs.module.eventCamera.depth == rhs.module.eventCamera.depth) { return 0; }
                    return -1;
                }

                if (lhs.module.sortOrderPriority != rhs.module.sortOrderPriority)
                {
                    return rhs.module.sortOrderPriority.CompareTo(lhs.module.sortOrderPriority);
                }

                if (lhs.module.renderOrderPriority != rhs.module.renderOrderPriority)
                {
                    return rhs.module.renderOrderPriority.CompareTo(lhs.module.renderOrderPriority);
                }
            }

            if (lhs.sortingLayer != rhs.sortingLayer)
            {
                var rid = SortingLayer.GetLayerValueFromID(rhs.sortingLayer);
                var lid = SortingLayer.GetLayerValueFromID(lhs.sortingLayer);
                return rid.CompareTo(lid);
            }

            if (lhs.sortingOrder != rhs.sortingOrder)
            {
                return rhs.sortingOrder.CompareTo(lhs.sortingOrder);
            }

            if (!Mathf.Approximately(lhs.distance, rhs.distance))
            {
                return lhs.distance.CompareTo(rhs.distance);
            }

            if (lhs.depth != rhs.depth)
            {
                return rhs.depth.CompareTo(lhs.depth);
            }

            return lhs.index.CompareTo(rhs.index);
        }

        protected virtual void ProcessMove(PointerEventData eventData)
        {
            var hoverGO = eventData.pointerCurrentRaycast.gameObject;
            if (eventData.pointerEnter != hoverGO)
            {
                HandlePointerExitAndEnter(eventData, hoverGO);
            }
        }

        protected virtual void ProcessPress(NRPointerEventData eventData)
        {
            if (eventData.GetPress())
            {
                if (!eventData.pressPrecessed)
                {
                    ProcessPressDown(eventData);
                }

                HandlePressExitAndEnter(eventData, eventData.pointerCurrentRaycast.gameObject);
            }
            else if (eventData.pressPrecessed)
            {
                ProcessPressUp(eventData);
                HandlePressExitAndEnter(eventData, null);
            }
        }

        protected void ProcessPressDown(NRPointerEventData eventData)
        {
            var currentOverGo = eventData.pointerCurrentRaycast.gameObject;

            eventData.pressPrecessed = true;
            eventData.eligibleForClick = true;
            eventData.delta = Vector2.zero;
            eventData.dragging = false;
            eventData.useDragThreshold = true;
            eventData.pressPosition = eventData.position;
            eventData.pressPosition3D = eventData.position3D;
            eventData.pressRotation = eventData.rotation;
            eventData.pressDistance = eventData.pointerCurrentRaycast.distance;
            eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;

            DeselectIfSelectionChanged(currentOverGo, eventData);

            var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.pointerDownHandler);

            if (newPressed == null)
            {
                newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
            }

            var time = Time.unscaledTime;

            if (newPressed == eventData.lastPress)
            {
                if (eventData.raycaster != null && time < (eventData.clickTime + NRInput.ClickInterval))
                {
                    ++eventData.clickCount;
                }
                else
                {
                    eventData.clickCount = 1;
                }

                eventData.clickTime = time;
            }
            else
            {
                eventData.clickCount = 1;
            }

            eventData.pointerPress = newPressed;
            eventData.rawPointerPress = currentOverGo;

            eventData.clickTime = time;

            eventData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

            if (eventData.pointerDrag != null)
            {
                ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.initializePotentialDrag);
            }
        }

        protected void ProcessPressUp(NRPointerEventData eventData)
        {
            var currentOverGo = eventData.pointerCurrentRaycast.gameObject;

            ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);

            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            if (eventData.pointerPress == pointerUpHandler && eventData.eligibleForClick)
            {
                ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerClickHandler);
            }
            else if (eventData.pointerDrag != null && eventData.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.dropHandler);
            }

            eventData.pressPrecessed = false;
            eventData.eligibleForClick = false;
            eventData.pointerPress = null;
            eventData.rawPointerPress = null;

            if (eventData.pointerDrag != null && eventData.dragging)
            {
                ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);
            }

            eventData.dragging = false;
            eventData.pointerDrag = null;

            if (currentOverGo != eventData.pointerEnter)
            {
                HandlePointerExitAndEnter(eventData, null);
                HandlePointerExitAndEnter(eventData, currentOverGo);
            }
        }

        protected bool ShouldStartDrag(NRPointerEventData eventData)
        {
            if (!eventData.useDragThreshold || eventData.raycaster == null) { return true; }
            var currentPos = eventData.position3D + (eventData.rotation * Vector3.forward) * eventData.pressDistance;
            var pressPos = eventData.pressPosition3D + (eventData.pressRotation * Vector3.forward) * eventData.pressDistance;
            var threshold = NRInput.DragThreshold;
            return (currentPos - pressPos).sqrMagnitude >= threshold * threshold;
        }

        protected void ProcessDrag(NRPointerEventData eventData)
        {
            var moving = !Mathf.Approximately(eventData.position3DDelta.sqrMagnitude, 0f) || !Mathf.Approximately(Quaternion.Angle(Quaternion.identity, eventData.rotationDelta), 0f);

            if (moving && eventData.pointerDrag != null && !eventData.dragging && ShouldStartDrag(eventData))
            {
                ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.beginDragHandler);
                eventData.dragging = true;
            }

            if (eventData.dragging && moving && eventData.pointerDrag != null)
            {
                if (eventData.pointerPress != eventData.pointerDrag)
                {
                    ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);

                    eventData.eligibleForClick = false;
                    eventData.pointerPress = null;
                    eventData.rawPointerPress = null;
                }
                ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.dragHandler);
            }
        }

        protected static void HandlePressExitAndEnter(NRPointerEventData eventData, GameObject newEnterTarget)
        {
            if (eventData.pressEnter == newEnterTarget) { return; }

            var oldTarget = eventData.pressEnter == null ? null : eventData.pressEnter.transform;
            var newTarget = newEnterTarget == null ? null : newEnterTarget.transform;
            var commonRoot = default(Transform);

            for (var t = oldTarget; t != null; t = t.parent)
            {
                if (newTarget != null && newTarget.IsChildOf(t))
                {
                    commonRoot = t;
                    break;
                }
                else
                {
                    ExecuteEvents.Execute(t.gameObject, eventData, NRExecutePointerEvents.PressExitHandler);
                }
            }

            eventData.pressEnter = newEnterTarget;

            for (var t = newTarget; t != commonRoot; t = t.parent)
            {
                ExecuteEvents.Execute(t.gameObject, eventData, NRExecutePointerEvents.PressEnterHandler);
            }
        }

        protected void DeselectIfSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent)
        {
            var selectHandlerGO = ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo);
            if (eventSystem != null && selectHandlerGO != eventSystem.currentSelectedGameObject)
            {
                eventSystem.SetSelectedGameObject(null, pointerEvent);
            }
        }
    }
    /// @endcond
}
