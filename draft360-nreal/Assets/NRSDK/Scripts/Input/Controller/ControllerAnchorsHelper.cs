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
    public enum ControllerAnchorEnum
    {
        GazePoseTrackerAnchor,
        RightPoseTrackerAnchor,
        LeftPoseTrackerAnchor,
        RightModelAnchor,
        LeftModelAnchor,
        RightLaserAnchor,
        LeftLaserAnchor
    }

    /**
    * @brief The class is for user to easily get the transform of common controller anchors.
    */
    public class ControllerAnchorsHelper : MonoBehaviour
    {
        [SerializeField]
        private Transform m_GazePoseTrackerAnchor;
        [SerializeField]
        private Transform m_RightPoseTrackerAnchor;
        [SerializeField]
        private Transform m_LeftPoseTrackerAnchor;
        [SerializeField]
        private Transform m_RightModelAnchor;
        [SerializeField]
        private Transform m_LeftModelAnchor;
        [SerializeField]
        private Transform m_RightLaserAnchor;
        [SerializeField]
        private Transform m_LeftLaserAnchor;

        public Transform GetAnchor(ControllerAnchorEnum anchorEnum)
        {
            switch (anchorEnum)
            {
                case ControllerAnchorEnum.GazePoseTrackerAnchor:
                    return m_GazePoseTrackerAnchor;
                case ControllerAnchorEnum.RightPoseTrackerAnchor:
                    return m_RightPoseTrackerAnchor;
                case ControllerAnchorEnum.LeftPoseTrackerAnchor:
                    return m_LeftPoseTrackerAnchor;
                case ControllerAnchorEnum.RightModelAnchor:
                    return m_RightModelAnchor;
                case ControllerAnchorEnum.LeftModelAnchor:
                    return m_LeftModelAnchor;
                case ControllerAnchorEnum.RightLaserAnchor:
                    return m_RightLaserAnchor;
                case ControllerAnchorEnum.LeftLaserAnchor:
                    return m_LeftLaserAnchor;
                default:
                    break;
            }
            return null;
        }
    }
    /// @endcond
}