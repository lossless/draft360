namespace NRKernal.NRExamples
{
    using NRKernal;
    using System.Collections.Generic;
    using UnityEngine;

    public class PolygonPlaneVisualizer : NRTrackableBehaviour
    {
        public Material Material;
        private MeshRenderer m_Renderer;
        private MeshFilter m_Filter;
        private MeshCollider m_Collider;
        List<Vector3> m_PosList = new List<Vector3>();
        Mesh m_PlaneMesh = null;

        private void Update()
        {
#if UNITY_EDITOR
            if (m_PosList.Count == 0)
            {
                m_PosList.Add(new Vector3(1f, 0, 0));
                m_PosList.Add(new Vector3(0, 0, -1));
                m_PosList.Add(new Vector3(-1f, 0, 0));
                m_PosList.Add(new Vector3(0, 0, -1));
            }
            this.DrawFromCenter(new Pose(transform.position, transform.rotation), m_PosList);
#else
            var pose = Trackable.GetCenterPose();
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            ((NRTrackablePlane)Trackable).GetBoundaryPolygon(m_PosList);
            this.DrawFromCenter(pose, m_PosList);
            
            if (Trackable.GetTrackingState() == TrackingState.Stopped)
            {
                Destroy(gameObject);
            }
#endif
        }

        private void DrawFromCenter(Pose centerPose, List<Vector3> vectors)
        {
            if (vectors == null || vectors.Count == 0)
            {
                return;
            }

            var center = centerPose.position;
            Vector3[] vertices3D = new Vector3[vectors.Count + 1];
            vertices3D[0] = center;
            for (int i = 1; i < vectors.Count + 1; i++)
            {
                vertices3D[i] = vectors[i - 1];
            }

            int[] triangles = GenerateTriangles(m_PosList);

            if (m_PlaneMesh == null)
            {
                m_PlaneMesh = new Mesh();
            }
            m_PlaneMesh.vertices = vertices3D;
            m_PlaneMesh.triangles = triangles;

            if (m_Renderer == null)
            {
                m_Renderer = gameObject.GetComponent<MeshRenderer>();
                Vector3 planeNormal = centerPose.rotation * Vector3.up;
                m_Renderer.material.SetVector("_PlaneNormal", planeNormal);
            }
            m_Renderer.material = Material;
            if (m_Filter == null)
            {
                m_Filter = gameObject.GetComponent<MeshFilter>();
            }
            m_Filter.mesh = m_PlaneMesh;

            if (m_Collider == null)
            {
                m_Collider = gameObject.GetComponent<MeshCollider>();
            }
            m_Collider.sharedMesh = m_PlaneMesh;
        }

        private int[] GenerateTriangles(List<Vector3> posList)
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < posList.Count; i++)
            {
                if (i != posList.Count - 1)
                {
                    indices.Add(0);
                    indices.Add(i + 1);
                    indices.Add(i + 2);
                }
                else
                {
                    indices.Add(0);
                    indices.Add(i + 1);
                    indices.Add(1);
                }
            }
            return indices.ToArray();
        }
    }
}