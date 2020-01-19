/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRToolkit.Sharing
{
    using NRToolkit.Sharing.AutoGenerate;
    using NRToolkit.Sharing.Tools;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class NetObjectManager : Singleton<NetObjectManager>
    {
        private Dictionary<int, NetworkBehaviour> m_NetObjectDict = new Dictionary<int, NetworkBehaviour>();
        private NetWorkObjectPool NetWorkObjectPool;

        internal void Init(NetWorkObjectPool objectPool)
        {
            NetWorkObjectPool = objectPool;
        }

        internal NetworkBehaviour Create(NetObjectInfo info)
        {
            if (m_NetObjectDict.ContainsKey(info.Identify))
            {
                return null;
            }
            NetworkBehaviour netobject = null;
            NetWorkObjectPool.TryGetNetObject(info.Key, out netobject);
            if (netobject != null)
            {
                var behaviour = GameObject.Instantiate<NetworkBehaviour>(netobject);
                behaviour.Initialize(info);
                behaviour.gameObject.name = info.Key;
                m_NetObjectDict.Add(info.Identify, behaviour);
                return behaviour;
            }
            else
            {
                return null;
            }
        }

        private bool Add(int identify, NetworkBehaviour obj)
        {
            if (m_NetObjectDict.ContainsKey(identify))
            {
                return false;
            }

            m_NetObjectDict.Add(identify, obj);
            return true;
        }

        internal bool ContainsKey(int identify)
        {
            return m_NetObjectDict.ContainsKey(identify);
        }

        internal bool Destroy(int identify)
        {
            NetworkBehaviour behaviour = null;
            m_NetObjectDict.TryGetValue(identify, out behaviour);

            if (behaviour == null || behaviour.gameObject == null)
            {
                return true;
            }

            GameObject.Destroy(behaviour.gameObject);
            return true;
        }

        internal void TryGetValue(int id, out NetworkBehaviour behaviour)
        {
            m_NetObjectDict.TryGetValue(id, out behaviour);
        }

        internal void SynObjects(List<NetObjectInfo> netObjectLists)
        {
            Dictionary<int, NetworkBehaviour> newNetObjectDict = new Dictionary<int, NetworkBehaviour>();
            foreach (var item in netObjectLists)
            {
                NetworkBehaviour behaviour = null;
                if (!m_NetObjectDict.ContainsKey(item.Identify))
                {
                    behaviour = this.Create(item);
                }
                else
                {
                    this.TryGetValue(item.Identify, out behaviour);
                }

                if (behaviour != null)
                {
                    newNetObjectDict.Add(item.Identify, behaviour);
                }
            }
            if (m_NetObjectDict.Count != newNetObjectDict.Count)
            {
                foreach (var item in m_NetObjectDict)
                {
                    if (!newNetObjectDict.ContainsKey(item.Key))
                    {
                        NetworkBehaviour behaviour = null;
                        m_NetObjectDict.TryGetValue(item.Key, out behaviour);
                        if (behaviour != null && behaviour.gameObject != null)
                        {
                            GameObject.Destroy(behaviour.gameObject);
                        }
                    }
                }

                m_NetObjectDict.Clear();
                m_NetObjectDict = newNetObjectDict;
            }

        }
    }
}
