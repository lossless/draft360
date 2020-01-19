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
    * @brief Tracable object type.
    */
    public enum TrackableType
    {
        /**
         * TRACKABLE_BASE means the base object of trackable.
         */
        TRACKABLE_BASE = 0,

        /**
         * TRACKABLE_PLANE means the trackable object is a plane.
         */
        TRACKABLE_PLANE = 1,

        /**
         * TRACKABLE_IMAGE means the trackable object is a tracking image.
         */
        TRACKABLE_IMAGE = 2,
    }

    /**
    * @brief Trackable image's finding mode.
    */
    public enum TrackableImageFindingMode
    {
        /**
         * Disable image tracking.
         */
        DISABLE = 0,

        /**
         * Enable image tracking.
         */
        ENABLE = 1,
    }
}
