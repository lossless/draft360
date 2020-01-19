using UnityEngine;

namespace NRKernal.NRExamples
{
    public class TrackablePlaneBehaviour : NRTrackableBehaviour
    {
        public NRTrackablePlane TrackablePlane
        {
            get
            {
                return (NRTrackablePlane)Trackable;
            }
        }

        private void Update()
        {
            if (TrackablePlane != null && TrackablePlane.GetTrackingState() == TrackingState.Tracking)
            {
                Pose pos = Trackable.GetCenterPose();
                transform.position = pos.position;
                transform.rotation = pos.rotation;

                transform.localScale = new Vector3(TrackablePlane.ExtentX, 0.001f, TrackablePlane.ExtentZ);
            }
        }
    }
}
