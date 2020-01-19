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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class NRGrabber : MonoBehaviour
    {
        public ControllerButton grabButton = ControllerButton.GRIP;
        public ControllerHandEnum handEnum;
        public bool grabMultiEnabled = false;
        public bool updatePoseByRigidbody = true;

        private bool m_PreviousGrabPress;
        private Dictionary<NRGrabbableObject, int> m_GrabReadyDict = new Dictionary<NRGrabbableObject, int>();
        private List<NRGrabbableObject> m_GrabbingList = new List<NRGrabbableObject>();
        private Collider[] m_ChildrenColliders;

        void Awake()
        {
            Rigidbody rigid = GetComponent<Rigidbody>();
            rigid.useGravity = false;
            rigid.isKinematic = true;
            m_ChildrenColliders = GetComponentsInChildren<Collider>();
        }

        private void OnEnable()
        {
            NRInput.OnControllerStatesUpdated += OnControllerPoseUpdated;
        }

        private void OnDisable()
        {
            NRInput.OnControllerStatesUpdated -= OnControllerPoseUpdated;
        }

        private void FixedUpdate()
        {
            if (!updatePoseByRigidbody)
                return;
            UpdateGrabbles();
        }

        private void OnTriggerEnter(Collider other)
        {
            NRGrabbableObject grabble = other.GetComponent<NRGrabbableObject>() ?? other.GetComponentInParent<NRGrabbableObject>();
            if (grabble == null)
                return;
            if (m_GrabReadyDict.ContainsKey(grabble))
                m_GrabReadyDict[grabble] += 1;
            else
                m_GrabReadyDict.Add(grabble, 1);
        }

        private void OnTriggerExit(Collider other)
        {
            NRGrabbableObject grabble = other.GetComponent<NRGrabbableObject>() ?? other.GetComponentInParent<NRGrabbableObject>();
            if (grabble == null)
                return;
            int count = 0;
            if (m_GrabReadyDict.TryGetValue(grabble, out count))
            {
                if (count > 1)
                    m_GrabReadyDict[grabble] = count - 1;
                else
                    m_GrabReadyDict.Remove(grabble);
            }
        }

        private void OnControllerPoseUpdated()
        {
            if (updatePoseByRigidbody)
                return;
            UpdateGrabbles();
        }

        private void UpdateGrabbles()
        {
            bool pressGrab = NRInput.GetButton(handEnum, grabButton);
            bool grabAction = !m_PreviousGrabPress && pressGrab;
            bool releaseAction = m_PreviousGrabPress && !pressGrab;
            m_PreviousGrabPress = pressGrab;
            if (grabAction && m_GrabbingList.Count == 0 && m_GrabReadyDict.Keys.Count != 0)
            {
                if (!grabMultiEnabled)
                {
                    NRGrabbableObject nearestGrabble = GetNearestGrabbleObject();
                    if (nearestGrabble)
                        GrabTarget(nearestGrabble);
                }
                else
                {
                    foreach (NRGrabbableObject grabble in m_GrabReadyDict.Keys)
                    {
                        GrabTarget(grabble);
                    }
                }
                SetChildrenCollidersEnabled(false);
            }

            if(releaseAction)
            {
                for (int i = 0; i < m_GrabbingList.Count; i++)
                {
                    m_GrabbingList[0].GrabEnd();
                }
                m_GrabbingList.Clear();
                SetChildrenCollidersEnabled(true);
            }

            if (m_GrabbingList.Count > 0 && !grabAction)
                MoveGrabbingObjects();
        }

        private NRGrabbableObject GetNearestGrabbleObject()
        {
            NRGrabbableObject nearestGrabble = null;
            float nearestSqrMagnitude = float.MaxValue;
            foreach (NRGrabbableObject grabbleObj in m_GrabReadyDict.Keys)
            {
                if (grabbleObj.AttachedColliders == null)
                    continue;
                for (int i = 0; i < grabbleObj.AttachedColliders.Length; i++)
                {
                    Vector3 closestPoint = grabbleObj.AttachedColliders[i].ClosestPointOnBounds(transform.position);
                    float grabbableSqrMagnitude = (transform.position - closestPoint).sqrMagnitude;
                    if (grabbableSqrMagnitude < nearestSqrMagnitude)
                    {
                        nearestSqrMagnitude = grabbableSqrMagnitude;
                        nearestGrabble = grabbleObj;
                    }
                }
            }
            return nearestGrabble;
        }

        private void GrabTarget(NRGrabbableObject target)
        {
            if (!target.CanGrab)
                return;
            target.GrabBegin(this);
            if (!m_GrabbingList.Contains(target))
                m_GrabbingList.Add(target);
            if (m_GrabReadyDict.ContainsKey(target))
                m_GrabReadyDict.Remove(target);
        }

        private void MoveGrabbingObjects()
        {
            for (int i = 0; i < m_GrabbingList.Count; i++)
            {
                if (updatePoseByRigidbody)
                    m_GrabbingList[i].MoveRigidbody(transform.position, transform.rotation);
                else
                    m_GrabbingList[i].MoveTransform(transform.position, transform.rotation);
            }
        }

        private void SetChildrenCollidersEnabled(bool isEnabled)
        {
            if (m_ChildrenColliders == null)
                return;
            for (int i = 0; i < m_ChildrenColliders.Length; i++)
            {
                m_ChildrenColliders[i].enabled = isEnabled;
            }
        }
    }
}
