using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    public class CameraCaptureController : MonoBehaviour
    {
        public RawImage CaptureImage;
        private NRRGBCamTexture RGBCamTexture { get; set; }

        public Text FrameCount;

        private void Start()
        {
            RGBCamTexture = new NRRGBCamTexture();
            CaptureImage.texture = RGBCamTexture.GetTexture();
            RGBCamTexture.Play();
        }

        void Update()
        {
            if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                RGBCamTexture.Play();
            }

            if (NRInput.GetButtonDown(ControllerButton.HOME))
            {
                RGBCamTexture.Stop();
            }

            FrameCount.text = RGBCamTexture.FrameCount.ToString();
        }

        void OnDestroy()
        {
            RGBCamTexture.Stop();
        }
    }
}
