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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// @cond EXCLUDE_FROM_DOXYGEN
    internal static class ControllerVisualFactory
    {
        public static GameObject CreateControllerVisualObject(ControllerVisualType visualType)
        {
            GameObject visualObj = null;
            string prefabPath = "";
            string folderPath = "ControllerVisuals/";
            switch (visualType)
            {
                case ControllerVisualType.NrealLight:
                    prefabPath = folderPath + "nreal_light_controller_visual";
                    break;
                case ControllerVisualType.Phone:
                    prefabPath = folderPath + "phone_controller_visual";
                    break;
                case ControllerVisualType.FinchShift:
                    prefabPath = folderPath + "finch_shift_controller_visual";
                    break;
                default:
                    Debug.LogError("Can not find controller visual for: " + visualType + ", set to default visual");
                    prefabPath = folderPath + "nreal_light_controller_visual";
                    break;
            }
            if (!string.IsNullOrEmpty(prefabPath))
            {
                GameObject controllerPrefab = Resources.Load<GameObject>(prefabPath);
                if (controllerPrefab)
                    visualObj = GameObject.Instantiate(controllerPrefab);
            }
            if(visualObj == null)
                Debug.LogError("Create controller visual failed, prefab path:" + prefabPath);
            return visualObj;
        }

        public static ControllerVisualType GetDefaultVisualType(ControllerType controllerType)
        {
            switch (controllerType)
            {
                case ControllerType.CONTROLLER_TYPE_PHONE:
                    return ControllerVisualType.Phone;
                case ControllerType.CONTROLLER_TYPE_FINCHSHIFT:
                    return ControllerVisualType.FinchShift;
                default:
                    return ControllerVisualType.NrealLight;
            }
        }
    }
    /// @endcond
}