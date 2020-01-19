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
    * @brief Device Session State.
    */
    public enum SessionState
    {
        /**
        * UnInitialize means the NRSDK has not been initialized.
        */
        UnInitialize = 0,

        /**
         * TRACKING means the object is being tracked and its state is valid.
         */
        Tracking,

        /**
        * LostTracking means that NRSDK has lost tracking, and will never resume tracking. 
        */
        LostTracking
    }
}
