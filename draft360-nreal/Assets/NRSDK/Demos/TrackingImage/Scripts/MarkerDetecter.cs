using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.NRExamples
{
    public class MarkerDetecter : MonoBehaviour
    {
        // A prefab for tracking and visualizing detected markers.
        public GameObject DetectedMarkerPrefab;

        // A list to hold new planes NRSDK began tracking in the current frame. This object is used across
        // the application to avoid per-frame allocations.
        private List<NRTrackableImage> m_NewMarkers = new List<NRTrackableImage>();

        public void Update()
        {
            NRFrame.GetTrackables<NRTrackableImage>(m_NewMarkers, NRTrackableQueryFilter.New);
            for (int i = 0; i < m_NewMarkers.Count; i++)
            {
                Debug.Log("[MarkerDetecter] Get New TrackableImages!! " + m_NewMarkers[i].TrackableNativeHandle);
                // Instantiate a visualization marker.
                NRAnchor anchor = m_NewMarkers[i].CreateAnchor();
                GameObject markerObject = Instantiate(DetectedMarkerPrefab, Vector3.zero, Quaternion.identity, anchor.transform);
                markerObject.GetComponent<TrackableImageBehaviour>().Initialize(m_NewMarkers[i]);
            }
        }
    }
}
