using UnityEngine;
using UnityEngine.EventSystems;

namespace NRKernal.NRExamples
{
    public class CubeInteractiveTest : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private MeshRenderer m_MeshRender;

        void Awake()
        {
            m_MeshRender = transform.GetComponent<MeshRenderer>();
        }

        void Update()
        {
            //get controller rotation, and set the value to the cube transform
            transform.rotation = NRInput.GetRotation();
        }

        //when pointer click, set the cube color to random color
        public void OnPointerClick(PointerEventData eventData)
        {
            m_MeshRender.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            // Adding open camera feature
            //CameraController.Instance.OpenCamera();
            //CameraController.Instance.TakePicture();
        }

        //when pointer hover, set the cube color to green
        public void OnPointerEnter(PointerEventData eventData)
        {
            m_MeshRender.material.color = Color.green;
        }

        //when pointer exit hover, set the cube color to white
        public void OnPointerExit(PointerEventData eventData)
        {
            m_MeshRender.material.color = Color.white;
        }
    }
}
