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
    public class SynObject
    {
        public virtual void Serialize(NetworkWriter writer) { }
        public virtual void DeSerialize(NetworkReader reader) { }
    }
}
