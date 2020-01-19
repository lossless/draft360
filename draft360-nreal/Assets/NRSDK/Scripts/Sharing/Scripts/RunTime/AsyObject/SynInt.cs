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
    using System;

    [Serializable]
    public class SynInt : SynObject
    {
        public int value;

        //public SynInt(NetworkBehaviour behaviour) : base(behaviour) { }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(value);
        }

        public override void DeSerialize(NetworkReader reader)
        {
            value = reader.ReadInt32();
        }
    }
}
