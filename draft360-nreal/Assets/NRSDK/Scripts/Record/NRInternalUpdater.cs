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
    using System;

    public class NRKernalUpdater : MonoBehaviour
    {
        private static NRKernalUpdater m_Instance;

        public static NRKernalUpdater Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject updateObj = new GameObject("NRKernalUpdater");
                    GameObject.DontDestroyOnLoad(updateObj);
                    m_Instance = updateObj.AddComponent<NRKernalUpdater>();
                }
                return m_Instance;
            }
        }

        public event Action OnUpdate;

        private void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}
