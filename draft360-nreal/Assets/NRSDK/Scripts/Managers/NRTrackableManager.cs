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
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /**
     * @brief Manages AR system state and handles the session lifecycle.
     * 
     * Through this class, application can create a session, configure it, start/pause or stop it. 
     */
    internal class NRTrackableManager
    {
        private Dictionary<UInt64, NRTrackable> m_TrackableDict = new Dictionary<UInt64, NRTrackable>();

        private Dictionary<TrackableType, Dictionary<UInt64, NRTrackable>> m_TrackableTypeDict = new Dictionary<TrackableType, Dictionary<ulong, NRTrackable>>();

        private List<NRTrackable> m_AllTrackables = new List<NRTrackable>();
        private List<NRTrackable> m_NewTrackables = new List<NRTrackable>();

        private HashSet<NRTrackable> m_OldTrackables = new HashSet<NRTrackable>();

        private NativeInterface m_NativeInterface = null;

        /// @cond EXCLUDE_FROM_DOXYGEN
        public NRTrackableManager(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
        }
        /// @endcond

        private NRTrackable Create(UInt64 trackable_handle, NativeInterface nativeInterface)
        {
            if (trackable_handle == 0)
            {
                return null;
            }

            NRTrackable result;
            if (m_TrackableDict.TryGetValue(trackable_handle, out result))
            {
                return result;
            }

            if (nativeInterface == null)
            {
                return null;
            }
            TrackableType trackableType = nativeInterface.NativeTrackable.GetTrackableType(trackable_handle);
            if (trackableType == TrackableType.TRACKABLE_PLANE)
            {
                result = new NRTrackablePlane(trackable_handle, nativeInterface);
            }
            else if (trackableType == TrackableType.TRACKABLE_IMAGE)
            {
                result = new NRTrackableImage(trackable_handle, nativeInterface);
            }
            else
            {
                throw new NotImplementedException(
                    "TrackableFactory::No constructor for requested trackable type.");
            }
            if (result != null)
            {
                m_TrackableDict.Add(trackable_handle, result);
                if (!m_TrackableTypeDict.ContainsKey(trackableType))
                {
                    m_TrackableTypeDict.Add(trackableType, new Dictionary<ulong, NRTrackable>());
                }
                Dictionary<ulong, NRTrackable> trackbletype_dict = null;
                m_TrackableTypeDict.TryGetValue(trackableType, out trackbletype_dict);
                if (!trackbletype_dict.ContainsKey(trackable_handle))
                {
                    trackbletype_dict.Add(trackable_handle, result);
                    m_AllTrackables.Add(result);
                }
            }

            return result;
        }

        private void UpdateTrackables(TrackableType trackable_type)
        {
            if (m_NativeInterface == null)
            {
                return;
            }
            UInt64 trackablelist_handle = m_NativeInterface.NativeTrackable.CreateTrackableList();
            m_NativeInterface.NativeTracking.UpdateTrackables(trackablelist_handle, trackable_type);
            int count = m_NativeInterface.NativeTrackable.GetSize(trackablelist_handle);
            for (int i = 0; i < count; i++)
            {
                UInt64 trackable_handle = m_NativeInterface.NativeTrackable.AcquireItem(trackablelist_handle, i);
                Create(trackable_handle, m_NativeInterface);
            }
            m_NativeInterface.NativeTrackable.DestroyTrackableList(trackablelist_handle);
        }

        /**
        * @brief Get the list of trackables with specified filter.
        * @param[out] trackableList A list where the returned trackable stored. The previous values will be cleared.
        * @param filter Query filter.
        */
        public void GetTrackables<T>(List<T> trackables, NRTrackableQueryFilter filter) where T : NRTrackable
        {
            TrackableType t_type = GetTrackableType<T>();

#if !UNITY_EDITOR_OSX
            // Update trackable by type
            UpdateTrackables(t_type);
#endif

            // Find the new trackable in this frame
            m_NewTrackables.Clear();
            for (int i = 0; i < m_AllTrackables.Count; i++)
            {
                NRTrackable trackable = m_AllTrackables[i];
                if (!m_OldTrackables.Contains(trackable))
                {
                    m_NewTrackables.Add(trackable);
                    m_OldTrackables.Add(trackable);
                }
            }

            trackables.Clear();

            if (filter == NRTrackableQueryFilter.All)
            {
                for (int i = 0; i < m_AllTrackables.Count; i++)
                {
                    SafeAdd<T>(m_AllTrackables[i], trackables);
                }
            }
            else if (filter == NRTrackableQueryFilter.New)
            {
                for (int i = 0; i < m_NewTrackables.Count; i++)
                {
                    SafeAdd<T>(m_NewTrackables[i], trackables);
                }
            }
        }

        private void SafeAdd<T>(NRTrackable trackable, List<T> trackables) where T : NRTrackable
        {
            if (trackable is T)
            {
                trackables.Add(trackable as T);
            }
        }

        private TrackableType GetTrackableType<T>() where T : NRTrackable
        {
            if (typeof(NRTrackablePlane).Equals(typeof(T)))
            {
                return TrackableType.TRACKABLE_PLANE;
            }
            else if (typeof(NRTrackableImage).Equals(typeof(T)))
            {
                return TrackableType.TRACKABLE_IMAGE;
            }
            return TrackableType.TRACKABLE_BASE;
        }
    }
}
