using UnityEngine;

namespace NRKernal.NRExamples
{
    public class TargetModelDisplayCtrl : MonoBehaviour
    {
        public Transform modelTarget;
        public MeshRenderer modelRenderer;
        public Color defaultColor = Color.white;
        public float minScale = 1f;
        public float maxScale = 3f;

        private Vector3 m_AroundLocalAxis = Vector3.down;
        private float m_TouchScrollSpeed = 10000f;
        private Vector2 m_PreviousPos;

        void Start()
        {
            ResetModel();
        }

        private void Update()
        {
            if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                m_PreviousPos = NRInput.GetTouch();
            }
            else if (NRInput.GetButton(ControllerButton.TRIGGER))
            {
                UpdateScroll();
            }
            else if (NRInput.GetButtonUp(ControllerButton.TRIGGER))
            {
                m_PreviousPos = Vector2.zero;
            }
        }

        private void UpdateScroll()
        {
            if (m_PreviousPos == Vector2.zero)
                return;
            Vector2 deltaMove = NRInput.GetTouch() - m_PreviousPos;
            m_PreviousPos = NRInput.GetTouch();
            modelTarget.Rotate(m_AroundLocalAxis, deltaMove.x * m_TouchScrollSpeed * Time.deltaTime, Space.Self);
        }

        public void ChangeModelColor(Color color)
        {
            modelRenderer.material.color = color;
        }

        public void ChangeModelScale(float val) // 0 ~ 1 
        {
            modelTarget.localScale = Vector3.one * Mathf.SmoothStep(minScale, maxScale, val);
        }

        public void ResetModel()
        {
            modelTarget.localRotation = Quaternion.identity;
            ChangeModelScale(0f);
            ChangeModelColor(defaultColor);
        }
    }
}
