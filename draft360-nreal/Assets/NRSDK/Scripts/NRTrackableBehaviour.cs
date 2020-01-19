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

    /**
    * @brief Base classes for all trackable monobehaviour objects.
    */
    public class NRTrackableBehaviour : MonoBehaviour
    {
        public NRTrackable Trackable;

        public void Initialize(NRTrackable trackable)
        {
            Trackable = trackable;
        }

        [HideInInspector, SerializeField]
        protected string m_TrackableName = "";

        [HideInInspector, SerializeField]
        protected bool m_PreserveChildSize;

        [HideInInspector, SerializeField]
        protected bool m_InitializedInEditor;

        [HideInInspector, SerializeField]
        protected int m_DatabaseIndex = -1;

        public string TrackableName
        {
            get
            {
                return m_TrackableName;
            }
            set
            {
                m_TrackableName = value;
            }
        }

        public bool PreserveChildSize
        {
            get
            {
                return m_PreserveChildSize;
            }
            set
            {
                m_PreserveChildSize = value;
            }
        }

        public bool InitializedInEditor
        {
            get
            {
                return m_InitializedInEditor;
            }
            set
            {
                m_InitializedInEditor = value;
            }
        }

        public int DatabaseIndex
        {
            get
            {
                return m_DatabaseIndex;
            }
            set
            {
                m_DatabaseIndex = value;
            }
        }
    }
}
