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
    using UnityEngine;
    using System;

    public class SynTransform : SynObject
    {
        [SerializeField]
        public Vector3 position;
        [SerializeField]
        public Quaternion rotation;

        //public SynTransform(NetworkBehaviour behaviour) : base(behaviour) { }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(position);
            writer.Write(rotation);
        }

        public override void DeSerialize(NetworkReader reader)
        {
            position = reader.ReadVector3();
            rotation = reader.ReadQuaternion();
        }

        public override string ToString()
        {
            return string.Format("pos:{0} rot:{1}", position, rotation);
        }
    }
}
