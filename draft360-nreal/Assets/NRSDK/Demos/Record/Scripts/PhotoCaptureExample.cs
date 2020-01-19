using NRKernal.Record;
using System.Linq;
using UnityEngine;

namespace NRKernal.NRExamples
{
    public class PhotoCaptureExample : MonoBehaviour
    {
        NRPhotoCapture photoCaptureObject = null;
        Texture2D targetTexture = null;

        Resolution cameraResolution;

        public NRPreviewer Previewer;

        private void Start()
        {
            this.Create();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T) || NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                TakeAPhoto();
            }

            if (Input.GetKeyDown(KeyCode.Q) || NRInput.GetButtonDown(ControllerButton.HOME))
            {
                Close();
            }

            if (Input.GetKeyDown(KeyCode.O) || NRInput.GetButtonDown(ControllerButton.APP))
            {
                Create();
            }

            if (photoCaptureObject != null)
            {
                Previewer.SetData(photoCaptureObject.CaptureBehaviour.PreviewTexture, true);
            }
        }

        // Use this for initialization
        void Create()
        {
            if (photoCaptureObject != null)
            {
                Debug.LogError("The NRPhotoCapture has already been created.");
                return;
            }

            // Create a PhotoCapture object
            NRPhotoCapture.CreateAsync(false, delegate (NRPhotoCapture captureObject)
            {
                cameraResolution = NRPhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
                targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

                if (captureObject != null)
                {
                    photoCaptureObject = captureObject;
                }
                else
                {
                    Debug.LogError("Can not get a captureObject.");
                }

                CameraParameters cameraParameters = new CameraParameters();
                cameraParameters.hologramOpacity = 0.0f;
                cameraParameters.cameraResolutionWidth = cameraResolution.width;
                cameraParameters.cameraResolutionHeight = cameraResolution.height;
                cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
                cameraParameters.blendMode = BlendMode.Blend;

                // Activate the camera
                photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (NRPhotoCapture.PhotoCaptureResult result)
                {
                    Debug.Log("Start PhotoMode Async");
                });
            });
        }

        void TakeAPhoto()
        {
            if (photoCaptureObject == null)
            {
                Debug.LogError("The NRPhotoCapture has not been created.");
                return;
            }
            // Take a picture
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }

        void OnCapturedPhotoToMemory(NRPhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
            // Copy the raw image data into our target texture
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);

            // Create a gameobject that we can apply our texture to
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
            quadRenderer.material = new Material(Resources.Load<Shader>("Record/Shaders/CaptureScreen"));

            var headTran = NRSessionManager.Instance.NRHMDPoseTracker.centerCamera.transform;
            quad.name = "picture";
            quad.transform.localPosition = headTran.position + headTran.forward * 3f;
            quad.transform.forward = headTran.forward;
            quad.transform.localScale = new Vector3(1.6f, 0.9f, 0);
            quadRenderer.material.SetTexture("_MainTex", targetTexture);
        }

        void Close()
        {
            if (photoCaptureObject == null)
            {
                Debug.LogError("The NRPhotoCapture has not been created.");
                return;
            }
            // Deactivate our camera
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }

        void OnStoppedPhotoMode(NRPhotoCapture.PhotoCaptureResult result)
        {
            if (photoCaptureObject == null)
            {
                Debug.LogError("The NRPhotoCapture has not been created.");
                return;
            }
            // Shutdown our photo capture resource
            photoCaptureObject.Dispose();
            photoCaptureObject = null;
        }
    }
}
