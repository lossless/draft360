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
    * @brief Device Tracking State.
    */
    public enum TrackingState
    {
        /**
         * TRACKING means the object is being tracked and its state is valid.
         */
        Tracking = 0,

        /**
         * PAUSED indicates that NRSDK has paused tracking, 
         * and the related data is not accurate.  
         */
        Paused = 1,

        /**
         * STOPPED means that NRSDK has stopped tracking, and will never resume tracking. 
         */
        Stopped = 2
    }

    /**
   * @brief Device Tracking State.
   */
    public enum TrackingMode
    {
        /**
         * 6Dof mode.
         */
        MODE_6DOF = 0,

        /**
         * 3Dof mode, only rotation.  
         */
        MODE_3DOF = 1,
    }
}
