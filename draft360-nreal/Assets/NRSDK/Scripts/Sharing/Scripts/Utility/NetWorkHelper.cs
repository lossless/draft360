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
    using NRToolkit.Sharing.Data;

    public class NetWorkHelper
    {
        public static void SynData(byte[] data, RequestType requestType = RequestType.Others)
        {
            if (NetWorkSession.Instance.IsConnected)
            {
                NetWorkSession.Instance.MsgGroup.SynDataEvent.Request(data, requestType);
            }
        }
    }
}
