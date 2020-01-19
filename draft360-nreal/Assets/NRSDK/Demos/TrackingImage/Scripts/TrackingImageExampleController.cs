namespace NRKernal.NRExamples
{
    using System.Collections.Generic;
    using UnityEngine;

    /**
    * @brief Controller for TrackingImage example.
    */
    public class TrackingImageExampleController : MonoBehaviour
    {
        // A prefab for visualizing an TrackingImage.
        public TrackingImageVisualizer TrackingImageVisualizerPrefab;

        // The overlay containing the fit to scan user guide.
        public GameObject FitToScanOverlay;

        private Dictionary<int, TrackingImageVisualizer> m_Visualizers
            = new Dictionary<int, TrackingImageVisualizer>();

        private List<NRTrackableImage> m_TempTrackingImages = new List<NRTrackableImage>();

        public void Update()
        {
#if !UNITY_EDITOR
            // Check that motion tracking is tracking.
            if (NRFrame.SessionStatus != SessionState.Tracking)
            {
                return;
            }
#endif
            // Get updated augmented images for this frame.
            NRFrame.GetTrackables<NRTrackableImage>(m_TempTrackingImages, NRTrackableQueryFilter.New);

            // Create visualizers and anchors for updated augmented images that are tracking and do not previously
            // have a visualizer. Remove visualizers for stopped images.
            foreach (var image in m_TempTrackingImages)
            {
                TrackingImageVisualizer visualizer = null;
                m_Visualizers.TryGetValue(image.GetDataBaseIndex(), out visualizer);
                if (image.GetTrackingState() == TrackingState.Tracking && visualizer == null)
                {
                    NRDebugger.Log("Create new TrackingImageVisualizer!");
                    // Create an anchor to ensure that NRSDK keeps tracking this augmented image.
                    visualizer = (TrackingImageVisualizer)Instantiate(TrackingImageVisualizerPrefab, image.GetCenterPose().position, image.GetCenterPose().rotation);
                    visualizer.Image = image;
                    visualizer.transform.parent = transform;
                    m_Visualizers.Add(image.GetDataBaseIndex(), visualizer);
                }
                else if (image.GetTrackingState() == TrackingState.Stopped && visualizer != null)
                {
                    m_Visualizers.Remove(image.GetDataBaseIndex());
                    Destroy(visualizer.gameObject);
                }

                FitToScanOverlay.SetActive(false);
            }

        }
    }
}
