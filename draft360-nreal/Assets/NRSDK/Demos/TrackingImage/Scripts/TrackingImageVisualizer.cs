namespace NRKernal.NRExamples
{
    using UnityEngine;

    /// Uses 4 frame corner objects to visualize an TrackingImage.
    /// </summary>
    public class TrackingImageVisualizer : MonoBehaviour
    {
        // The TrackingImage to visualize.
        public NRTrackableImage Image;

        // A model for the lower left corner of the frame to place when an image is detected.
        public GameObject FrameLowerLeft;

        // A model for the lower right corner of the frame to place when an image is detected.
        public GameObject FrameLowerRight;

        /// A model for the upper left corner of the frame to place when an image is detected.
        public GameObject FrameUpperLeft;

        // A model for the upper right corner of the frame to place when an image is detected.
        public GameObject FrameUpperRight;

        public GameObject Axis;

        public void Update()
        {
            if (Image == null || Image.GetTrackingState() != TrackingState.Tracking)
            {
                FrameLowerLeft.SetActive(false);
                FrameLowerRight.SetActive(false);
                FrameUpperLeft.SetActive(false);
                FrameUpperRight.SetActive(false);
                Axis.SetActive(false);
                return;
            }

            float halfWidth = Image.ExtentX / 2;
            float halfHeight = Image.ExtentZ / 2;
            FrameLowerLeft.transform.localPosition = (halfWidth * Vector3.left) + (halfHeight * Vector3.back);
            FrameLowerRight.transform.localPosition = (halfWidth * Vector3.right) + (halfHeight * Vector3.back);
            FrameUpperLeft.transform.localPosition = (halfWidth * Vector3.left) + (halfHeight * Vector3.forward);
            FrameUpperRight.transform.localPosition = (halfWidth * Vector3.right) + (halfHeight * Vector3.forward);

            var center = Image.GetCenterPose();
            transform.position = center.position;
            transform.rotation = center.rotation;

            FrameLowerLeft.SetActive(true);
            FrameLowerRight.SetActive(true);
            FrameUpperLeft.SetActive(true);
            FrameUpperRight.SetActive(true);
            Axis.SetActive(true);
        }
    }
}
