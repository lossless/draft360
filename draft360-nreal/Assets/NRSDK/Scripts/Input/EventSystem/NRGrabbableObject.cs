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
    using System;

    [RequireComponent(typeof(Rigidbody))]
    public class NRGrabbableObject : MonoBehaviour {
        public bool CanGrab { get { return Grabber == null; } }
        public bool IsBeingGrabbed { get { return Grabber; } }
        public NRGrabber Grabber { get; private set; }

        public Collider[] AttachedColliders
        {
            get
            {
                CheckAttachedColliders();
                return m_AttachedColliders;
            }
        }

        public event Action OnGrabBegan { add { m_OnGrabBegan += value; } remove { m_OnGrabBegan -= value; } }
        public event Action OnGrabEnded { add { m_OnGrabEnded += value; } remove { m_OnGrabEnded -= value; } }

        protected Rigidbody m_AttachedRigidbody;
        [SerializeField]
        private Collider[] m_AttachedColliders;
        private bool m_OriginRigidbodyKinematic;

        private Action m_OnGrabBegan;
        private Action m_OnGrabEnded;

        protected virtual void Awake()
        {
            m_AttachedRigidbody = GetComponent<Rigidbody>();
            m_OriginRigidbodyKinematic = m_AttachedRigidbody.isKinematic;
            CheckAttachedColliders();
        }

        public void GrabBegin(NRGrabber grabber)
        {
            if (IsBeingGrabbed || grabber == null)
                return;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            Grabber = grabber;
            if (m_OnGrabBegan != null)
                m_OnGrabBegan();
        }

        public void GrabEnd()
        {
            m_AttachedRigidbody.isKinematic = m_OriginRigidbodyKinematic;
            Grabber = null;
            if (m_OnGrabEnded != null)
                m_OnGrabEnded();
        }

        public void MoveRigidbody(Vector3 targetPos, Quaternion targetRot)
        {
            if (!IsBeingGrabbed)
                return;
            m_AttachedRigidbody.MovePosition(targetPos);
            m_AttachedRigidbody.MoveRotation(targetRot);
        }

        public void MoveTransform(Vector3 targetPos, Quaternion targetRot)
        {
            if (!IsBeingGrabbed)
                return;
            transform.position = targetPos;
            transform.rotation = targetRot;
        }

        private void CheckAttachedColliders()
        {
            if (m_AttachedColliders != null && m_AttachedColliders.Length > 0)
                return;
            m_AttachedColliders = GetComponentsInChildren<Collider>();
            if (m_AttachedColliders == null)
                Debug.LogError("AttachedColliders can not be null for NRGrabbableObject, please set collider !");
        }
    }
}
