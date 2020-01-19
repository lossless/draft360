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
    using UnityEngine;
    using UnityEngine.Serialization;

    /**
     * @brief A configuration used to track the world.
     */
    [CreateAssetMenu(fileName = "NRKernalSessionConfig", menuName = "NRSDK/SessionConfig", order = 1)]
    public class NRSessionConfig : ScriptableObject
    {
        // Chooses whether optimized rendering will be used. It can't be changed in runtime.
        [Tooltip("Chooses whether Optimized Rendering will be used. It can't be changed in runtime")]
        [FormerlySerializedAs("Optimized Rendering")]
        public bool OptimizedRendering = true;

        // Chooses which plane finding mode will be used.
        [Tooltip("Chooses which plane finding mode will be used.")]
        [FormerlySerializedAs("EnablePlaneFinding")]
        public TrackablePlaneFindingMode PlaneFindingMode = TrackablePlaneFindingMode.DISABLE;

        // Chooses which marker finding mode will be used.
        [Tooltip("Chooses which marker finding mode will be used.")]
        [FormerlySerializedAs("EnableImageTracking")]
        public TrackableImageFindingMode ImageTrackingMode = TrackableImageFindingMode.DISABLE;

        // A scriptable object specifying the NRSDK TrackingImageDatabase configuration.
        [Tooltip("A scriptable object specifying the NRSDK TrackingImageDatabase configuration.")]
        public NRTrackingImageDatabase TrackingImageDatabase;

        /**
         * @brief ValueType check if two NRSessionConfig objects are equal.
         * @return True if the two NRSessionConfig objects are value-type equal, otherwise false.
         */
        public override bool Equals(object other)
        {
            NRSessionConfig otherConfig = other as NRSessionConfig;
            if (other == null)
            {
                return false;
            }

            if (OptimizedRendering != otherConfig.OptimizedRendering ||
                PlaneFindingMode != otherConfig.PlaneFindingMode ||
                ImageTrackingMode != otherConfig.ImageTrackingMode ||
                TrackingImageDatabase != otherConfig.TrackingImageDatabase)
            {
                return false;
            }

            return true;
        }

        /**
         * @brief Return a hash code for this object.
         */
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /**
         * @brief ValueType copy from another SessionConfig object into this one.
         */
        public void CopyFrom(NRSessionConfig other)
        {
            OptimizedRendering = other.OptimizedRendering;
            PlaneFindingMode = other.PlaneFindingMode;
            ImageTrackingMode = other.ImageTrackingMode;
            TrackingImageDatabase = other.TrackingImageDatabase;
        }
    }
}
