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
    /**
    * @brief A filter for trackable queries.
    */
    public enum NRTrackableQueryFilter
    {
        // Indicates available trackables.
        All,

        // Indicates new trackables detected in the current NRSDK Frame.
        New,
    }
}
