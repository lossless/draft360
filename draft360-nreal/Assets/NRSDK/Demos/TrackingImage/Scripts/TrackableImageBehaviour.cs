using UnityEngine;

namespace NRKernal.NRExamples
{
    public class TrackableImageBehaviour : MonoBehaviour
    {
        private NRTrackableImage m_DetectedMarker;

        public void Initialize(NRTrackableImage marker)
        {
            m_DetectedMarker = marker;
        }

        private void Update()
        {
            if (m_DetectedMarker != null && m_DetectedMarker.GetTrackingState() == TrackingState.Tracking)
            {
                Vector2 size = m_DetectedMarker.Size;
                transform.localScale = new Vector3(size.x, transform.localScale.y, size.y);
            }
        }
    }
}
