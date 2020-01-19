using UnityEngine;

namespace NRKernal.NRExamples
{
    public class AutoRotate : MonoBehaviour
    {
        void Update()
        {
            transform.Rotate(Vector3.up, 1f);
        }
    }
}