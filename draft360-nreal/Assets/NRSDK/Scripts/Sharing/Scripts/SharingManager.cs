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
    using NRKernal;
    using System;
    using UnityEngine;
    using System.Collections;
    using System.Text;

    public class SharingManager : MonoBehaviour
    {
        public NetworkBehaviour Player;
        public NetWorkObjectPool ObjectPool;

        private static SharingManager m_Instance = null;
        public static SharingManager Instance
        {
            get
            {
                return m_Instance;
            }
            private set
            {
                if (m_Instance != value && m_Instance != null)
                {
                    Destroy(value.gameObject);
                    Debug.Log("There are more than one SharringManager. Destroy the new one!");
                    return;
                }
                m_Instance = value;
            }
        }

        public string CurrentRoom { get; set; }
        public enum ConnectState
        {
            DisConnected,
            Connected,
            Error
        }
        public ConnectState NetState { get; set; }

        private void OnEnable()
        {
            NetWorkSession.Instance.Initialize();

            NetWorkSession.Instance.OnConnect += OnNetConnected;
            NetWorkSession.Instance.OnConnectError += OnNetConnectError;
            NetWorkSession.Instance.OnClosed += OnNetClosed;

            NetWorkSession.Instance.MsgGroup.SynContextEvent.OnDataReceiveResp += OnSynContextResp;
            NetWorkSession.Instance.MsgGroup.CreateNetObjectEvent.OnDataReceiveResp += OnCreateNetObjectResp;
            NetWorkSession.Instance.MsgGroup.DestroyNetObjectEvent.OnDataReceiveResp += OnDestroyNetObjectResp;
            NetWorkSession.Instance.MsgGroup.SynDataEvent.OnDataReceiveResp += OnSynDataResp;
        }

        private void Start()
        {
            Wraper.Initialize();
            ObjectPool.Init();
            NetObjectManager.Instance.Init(ObjectPool);
            AutoConnect();
        }

        private void OnDisable()
        {
            NetWorkSession.Instance.OnConnect -= OnNetConnected;
            NetWorkSession.Instance.OnConnectError -= OnNetConnectError;
            NetWorkSession.Instance.OnClosed -= OnNetClosed;
            NetWorkSession.Instance.MsgGroup.CreateNetObjectEvent.OnDataReceiveResp -= OnCreateNetObjectResp;
            NetWorkSession.Instance.MsgGroup.SynDataEvent.OnDataReceiveResp -= OnSynDataResp;
        }

        private IEnumerator HeartBeatFunc()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                SynContextRequest();
            }
        }

        #region request
        public void SynContextRequest()
        {
            NetWorkSession.Instance.MsgGroup.SynContextEvent.Request();
        }

        public void CreateNetObjRequest(NetworkBehaviour behaviour)
        {
            string key = behaviour.GetType().Name.ToString();
            NetWorkSession.Instance.MsgGroup.CreateNetObjectEvent.Request(key);
        }

        public void AutoConnect()
        {
            NetWorkSession.Instance.SearchLocalServer();
        }

        public void SynDataRequest(int id, byte[] data)
        {
            byte[] totalBytes;
            using (var bufferStream = new System.IO.MemoryStream())
            {
                bufferStream.Write(BitConverter.GetBytes(id), 0, 4);
                bufferStream.Write(data, 0, data.Length);
                totalBytes = bufferStream.ToArray();
            }
            NetWorkSession.Instance.MsgGroup.SynDataEvent.Request(totalBytes, Data.RequestType.Others);
        }
        #endregion

        #region response
        private void OnSynContextResp(SynContextResp resp)
        {
            //处理数据同步
            NetObjectManager.Instance.SynObjects(resp.netObjectLists);
        }

        private void OnNetClosed()
        {
            NetState = ConnectState.DisConnected;
            NRDebugger.Log("OnNetClosed");
        }

        private void OnNetConnectError()
        {
            NetState = ConnectState.Error;
            NRDebugger.LogError("OnNetConnectError");
        }

        private void OnNetConnected()
        {
            NetState = ConnectState.Connected;
            NRDebugger.Log("OnNetConnected");

            if (Player != null)
            {
                CreateNetObjRequest(Player);
            }

            StartCoroutine(HeartBeatFunc());
        }

        private void OnCreateNetObjectResp(CreateNetObjectResp data)
        {
            NetObjectManager.Instance.Create(data.netObjectInfo);

            this.SynContextRequest();
        }

        private void OnDestroyNetObjectResp(DestroyNetObjectResp data)
        {
            NetObjectManager.Instance.Destroy(data.Identify);
        }

        private void OnSynDataResp(byte[] data)
        {
            int msgtype = BitConverter.ToInt32(data, 0);

            int id = BitConverter.ToInt32(data, 4);
            NetworkBehaviour behaviour;
            NetObjectManager.Instance.TryGetValue(id, out behaviour);
            if (behaviour == null)
            {
                //Debug.Log("Can not find the behaviour : " + id);
                return;
            }
            byte[] usedBytes;
            using (var bufferStream = new System.IO.MemoryStream())
            {
                bufferStream.Write(data, 8, data.Length - 8);
                usedBytes = bufferStream.ToArray();
            }

            if (msgtype == (int)NetMsgType.SynValue)
            {
                behaviour.DeserializeData(usedBytes);
            }
            else if (msgtype == (int)NetMsgType.Commond)
            {
                behaviour.ReplyCommond(Encoding.UTF8.GetString(usedBytes));
            }
        }
        #endregion

        private void OnDestroy()
        {
            NetWorkSession.Instance.Close();
        }
    }
}
