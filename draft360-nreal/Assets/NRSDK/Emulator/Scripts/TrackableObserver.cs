using System;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class TrackableObserver : MonoBehaviour
{
    public delegate void TrackingDelegate(Vector3 pos, Quaternion qua);
    public TrackingDelegate FoundEvent;
    public Action LostEvnet;

    public TrackableType TargetType;

    private NRTrackableBehaviour m_TrackableBehaviour;
    private List<NRTrackableImage> m_TempTrackingImages = new List<NRTrackableImage>();
    private List<NRTrackablePlane> m_TempTrackingPlane = new List<NRTrackablePlane>();

    public enum TrackableType
    {
        TrackableImage,
        TrackablePlane,
    }

    // Use this for initialization
    void Start()
    {
        m_TrackableBehaviour = GetComponent<NRTrackableBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TargetType == TrackableType.TrackableImage)
        {
            NRFrame.GetTrackables<NRTrackableImage>(m_TempTrackingImages, NRTrackableQueryFilter.All);
            foreach (var trackableImage in m_TempTrackingImages)
            {
                if (trackableImage.GetDataBaseIndex() == m_TrackableBehaviour.DatabaseIndex)
                {
                    if (trackableImage.GetTrackingState() == TrackingState.Tracking)
                    {
                        if (FoundEvent != null)
                            FoundEvent(trackableImage.GetCenterPose().position, trackableImage.GetCenterPose().rotation);
                    }
                    else
                    {
                        if (LostEvnet != null)
                            LostEvnet();
                    }
                    break;
                }
            }
        }
        else if (TargetType == TrackableType.TrackablePlane)
        {
            NRFrame.GetTrackables<NRTrackablePlane>(m_TempTrackingPlane, NRTrackableQueryFilter.All);
            foreach (var trackablePlane in m_TempTrackingPlane)
            {
                if (m_TrackableBehaviour.DatabaseIndex == -1)
                    m_TrackableBehaviour.DatabaseIndex = trackablePlane.GetDataBaseIndex();
                if (trackablePlane.GetDataBaseIndex() == m_TrackableBehaviour.DatabaseIndex)
                {
                    if (trackablePlane.GetTrackingState() == TrackingState.Tracking)
                    {
                        if (FoundEvent != null)
                            FoundEvent(trackablePlane.GetCenterPose().position, trackablePlane.GetCenterPose().rotation);
                    }
                    else
                    {
                        if (LostEvnet != null)
                            LostEvnet();
                    }
                    break;
                }
            }
        }
    }
}
