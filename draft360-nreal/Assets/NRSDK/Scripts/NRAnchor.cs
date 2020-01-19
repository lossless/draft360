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
    using System.Collections.Generic;
    using UnityEngine;

    /**
    * @brief Update the transform of  a trackable.
    */
    internal partial class NRAnchor : MonoBehaviour
    {
        private static Dictionary<Int64, NRAnchor> m_AnchorDict = new Dictionary<Int64, NRAnchor>();

        public NRTrackable Trackable;

        private bool m_IsSessionDestroyed;

        /**
        * @brief Create a anchor for the trackable object 
        * 
        * Instantiate a NRAnchor object whick Update trackable pose every frame
        * 
        * @return NRAnchor
        */
        internal static NRAnchor Factory(NRTrackable trackable)
        {
            if (trackable == null)
            {
                return null;
            }

            NRAnchor result;
            if (m_AnchorDict.TryGetValue(trackable.GetDataBaseIndex(), out result))
            {
                return result;
            }

            NRAnchor anchor = (new GameObject()).AddComponent<NRAnchor>();
            anchor.gameObject.name = "Anchor";
            anchor.Trackable = trackable;

            m_AnchorDict.Add(trackable.GetDataBaseIndex(), anchor);
            return anchor;

        }

        private void Update()
        {
            if (Trackable == null)
            {
                NRDebugger.LogError("NRAnchor components instantiated outside of NRInternel are not supported. " +
                    "Please use a 'Create' method within NRInternel to instantiate anchors.");
                return;
            }

            if (IsSessionDestroyed())
            {
                return;
            }

            var pose = Trackable.GetCenterPose();
            transform.position = pose.position;
            transform.rotation = pose.rotation;

        }

        private void OnDestroy()
        {
            if (Trackable == null)
            {
                return;
            }

            m_AnchorDict.Remove(Trackable.GetDataBaseIndex());
        }

        /**
        * Check whether the session is already destroyed
        */
        private bool IsSessionDestroyed()
        {
            if (!m_IsSessionDestroyed)
            {
                var nativeInterface = NRSessionManager.Instance.NativeAPI;
                if (nativeInterface != Trackable.NativeInterface)
                {
                    Debug.LogErrorFormat("The session which created this anchor has been destroyed. " +
                    "The anchor on GameObject {0} can no longer update.",
                        this.gameObject != null ? this.gameObject.name : "Unknown");
                    m_IsSessionDestroyed = true;
                }
            }

            return m_IsSessionDestroyed;
        }
    }
}
