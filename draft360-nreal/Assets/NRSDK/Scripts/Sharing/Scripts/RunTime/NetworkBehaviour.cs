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
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    public class NetworkBehaviour : MonoBehaviour
    {
        private List<SynObject> m_SynObjs;
        public List<SynObject> SynObjs
        {
            get
            {
                if (m_SynObjs == null)
                {
                    m_SynObjs = Wraper.GetFieldList(this);
                }

                return m_SynObjs;
            }
        }

        protected int m_Identify = -1;
        protected bool IsOwner = true;
        private bool m_IsInitialize = false;

        public NetworkBehaviour()
        {
            m_SynObjs = Wraper.GetFieldList(this);
        }

        public virtual void Initialize(NetObjectInfo info)
        {
            m_Identify = info.Identify;
            IsOwner = info.Owner.Equals(NetWorkSession.Instance.GUID);
            m_IsInitialize = true;
        }

        void Start()
        {
            StartCoroutine(Updater());
        }

        private IEnumerator Updater()
        {
            var duration = new WaitForSeconds(1.0f / SharringSettings.UpdateFrequency);
            while (true)
            {
                this.SynData();
                yield return duration;
            }
        }

        private void SynData()
        {
            if (IsOwner && m_IsInitialize && SynObjs != null)
            {
                NetworkWriter writer = new NetworkWriter();
                writer.Write((int)NetMsgType.SynValue);
                writer.Write(m_Identify);
                for (int i = 0; i < SynObjs.Count; i++)
                {
                    SynObjs[i].Serialize(writer);
                }
                var usedBytes = writer.ToArray();
                if (usedBytes != null)
                {
                    NetWorkHelper.SynData(usedBytes, Data.RequestType.Everyone);
                }
            }
        }

        public void RPC(string commond)
        {
            if (IsOwner && m_IsInitialize)
            {
                NetworkWriter writer = new NetworkWriter();
                writer.Write((int)NetMsgType.Commond);
                writer.Write(m_Identify);
                byte[] commond_data = Encoding.UTF8.GetBytes(commond);
                writer.Write(commond_data, 0, commond_data.Length);
                var usedBytes = writer.ToArray();
                if (usedBytes != null)
                {
                    NetWorkHelper.SynData(usedBytes, Data.RequestType.Everyone);
                }
            }
        }

        public void DeserializeData(byte[] data)
        {
            if (data == null || IsOwner)
            {
                return;
            }

            NetworkReader reader = new NetworkReader(data);
            for (int i = 0; i < SynObjs.Count; i++)
            {
                SynObjs[i].DeSerialize(reader);
            }
        }

        public void ReplyCommond(string commond)
        {
            Invoke(commond, 0f);
        }
    }
}
