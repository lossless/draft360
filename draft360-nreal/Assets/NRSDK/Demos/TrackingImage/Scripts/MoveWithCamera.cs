using UnityEngine;

namespace NRKernal.NRExamples
{
    public class MoveWithCamera : MonoBehaviour
    {
        private float originDistance;
        [SerializeField]
        private bool useRelative = true;

        private void Awake()
        {
            originDistance = useRelative ? Vector3.Distance(transform.position, Vector3.zero) : 0;
        }

        void Update()
        {
            Transform centerCamera = NRSessionManager.Instance.NRHMDPoseTracker.centerCamera.transform;
            transform.position = centerCamera.transform.position + centerCamera.transform.forward * originDistance;
            transform.rotation = centerCamera.transform.rotation;
        }
    }
}
