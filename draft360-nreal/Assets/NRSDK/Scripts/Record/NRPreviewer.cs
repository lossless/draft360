/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    using NRKernal;
    using UnityEngine;
    using UnityEngine.UI;

    /**
    * @brief Preview the camera's record or capture image.
    */
    public class NRPreviewer : MonoBehaviour
    {
        public GameObject Root;
        public RawImage PreviewScreen;
        public Image StateIcon;

        private void Start()
        {
            Root.SetActive(false);
        }

        public void SetData(Texture tex, bool isplaying)
        {
            PreviewScreen.texture = tex;
            StateIcon.color = isplaying ? Color.green : Color.red;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || NRInput.GetButtonDown(ControllerButton.APP))
            {
                Root.SetActive(!Root.activeInHierarchy);

                NRInput.LaserVisualActive = !Root.activeInHierarchy;
                NRInput.ReticleVisualActive = !Root.activeInHierarchy;
            }
            this.BindPreviewTOController();
        }

        private void BindPreviewTOController()
        {
            var inputAnchor = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.RightModelAnchor);
            transform.position = inputAnchor.TransformPoint(Vector3.forward * 0.3f);
            transform.forward = inputAnchor.forward;
        }
    }
}
