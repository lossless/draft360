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
     * The reason of HMD untracked.
     */
    internal enum LostTrackingReason
    {
        NONE = 0,

        /**
         * Initializing.
         */
        INITIALIZING = 1,

        /**
         * Move too fast.
         */
        EXCESSIVE_MOTION = 2,

        /**
        * Feature point deficiency.
        */
        INSUFFICIENT_FEATURES = 3,

        /**
         * Reposition.
         */
        RELOCALIZING = 4,
    }
}
