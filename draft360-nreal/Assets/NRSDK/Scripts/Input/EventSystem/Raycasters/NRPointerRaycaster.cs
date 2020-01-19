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
    using System.Collections.ObjectModel;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// @cond EXCLUDE_FROM_DOXYGEN
    [DisallowMultipleComponent]
    public class NRPointerRaycaster : EventCameraRaycaster
    {
        public enum MaskTypeEnum
        {
            Inclusive,
            Exclusive
        }

        private static readonly RaycastHit[] hits = new RaycastHit[64];

        public MaskTypeEnum maskType = MaskTypeEnum.Exclusive;
        public LayerMask mask;
        public int raycastMask { get { return maskType == MaskTypeEnum.Inclusive ? (int)mask : ~mask; } }
        public bool showDebugRay = true;
        public bool enablePhysicsRaycast = true;
        public bool enableGraphicRaycast = true;

        protected readonly List<NRPointerEventData> buttonEventDataList = new List<NRPointerEventData>();
        protected readonly List<RaycastResult> sortedRaycastResults = new List<RaycastResult>();
        protected readonly List<Vector3> breakPoints = new List<Vector3>();

        public ControllerHandEnum RelatedHand { get; private set; }
        public List<Vector3> BreakPoints { get { return breakPoints; } }
        public NRPointerEventData HoverEventData { get { return buttonEventDataList.Count > 0 ? buttonEventDataList[0] : null; } }
        public ReadOnlyCollection<NRPointerEventData> ButtonEventDataList { get { return buttonEventDataList.AsReadOnly(); } }

        protected override void Start()
        {
            base.Start();
            ControllerTracker controllerTracker = GetComponentInParent<ControllerTracker>();
            RelatedHand = controllerTracker ? controllerTracker.defaultHandEnum : NRInput.DomainHand;
            buttonEventDataList.Add(new NRPointerEventData(this, EventSystem.current));
        }

        // called by StandaloneInputModule, not supported
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {

        }

        public virtual void Raycast()
        {
            sortedRaycastResults.Clear();
            breakPoints.Clear();

            var zScale = transform.lossyScale.z;
            var amountDistance = (FarDistance - NearDistance) * zScale;
            var origin = transform.TransformPoint(0f, 0f, NearDistance);
            breakPoints.Add(origin);

            Vector3 direction;
            float distance;
            Ray ray;
            RaycastResult firstHit = default(RaycastResult);

            direction = transform.forward;
            distance = amountDistance;
            ray = new Ray(origin, direction);

            eventCamera.farClipPlane = eventCamera.nearClipPlane + distance;
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

        protected virtual Comparison<RaycastResult> GetRaycasterResultComparer()
        {
            return NRInputModule.defaultRaycastComparer;
        }

        // override OnEnable & OnDisable on purpose so that this BaseRaycaster won't be registered into RaycasterManager
        protected override void OnEnable()
        {
            //base.OnEnable();
            NRInputModule.AddRaycaster(this);
        }

        protected override void OnDisable()
        {
            //base.OnDisable();
            NRInputModule.RemoveRaycaster(this);
        }

        public virtual Vector2 GetScrollDelta()
        {
            return Vector2.zero;
        }

        public RaycastResult FirstRaycastResult()
        {
            for (int i = 0, imax = sortedRaycastResults.Count; i < imax; ++i)
            {
                if (!sortedRaycastResults[i].isValid)
                    continue;
                return sortedRaycastResults[i];
            }
            return default(RaycastResult);
        }

        public void Raycast(Ray ray, float distance, List<RaycastResult> raycastResults)
        {
            var results = new List<RaycastResult>();
            if (enablePhysicsRaycast)
                PhysicsRaycast(ray, distance, results);
            if (enableGraphicRaycast)
            {
                var tempCanvases = CanvasTargetCollector.GetCanvases();
                for (int i = tempCanvases.Count - 1; i >= 0; --i)
                {
                    var target = tempCanvases[i];
                    if (target == null || !target.enabled)
                        continue;
                    GraphicRaycast(target.canvas, target.ignoreReversedGraphics, ray, distance, this, results);
                }
            }
            var comparer = GetRaycasterResultComparer();
            if (comparer != null)
                results.Sort(comparer);
            for (int i = 0, imax = results.Count; i < imax; ++i)
            {
                raycastResults.Add(results[i]);
            }
        }

        public virtual void PhysicsRaycast(Ray ray, float distance, List<RaycastResult> raycastResults)
        {
            var hitCount = Physics.RaycastNonAlloc(ray, hits, distance, raycastMask);
            for (int i = 0; i < hitCount; ++i)
            {
                raycastResults.Add(new RaycastResult
                {
                    gameObject = hits[i].collider.gameObject,
                    module = this,
                    distance = hits[i].distance,
                    worldPosition = hits[i].point,
                    worldNormal = hits[i].normal,
                    screenPosition = NRInputModule.ScreenCenterPoint,
                    index = raycastResults.Count,
                    sortingLayer = 0,
                    sortingOrder = 0
                });
            }
        }

        public virtual void GraphicRaycast(Canvas canvas, bool ignoreReversedGraphics, Ray ray, float distance, NRPointerRaycaster raycaster, List<RaycastResult> raycastResults)
        {
            if (canvas == null) { return; }

            var eventCamera = raycaster.eventCamera;
            var screenCenterPoint = NRInputModule.ScreenCenterPoint;
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

                raycastResults.Add(new RaycastResult
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
                });
            }
        }
    }
    /// @endcond
}
