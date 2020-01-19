using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    public class CustomVirtualControlerUI : MonoBehaviour
    {
        public Button showBtn;
        public Button hideBtn;
        public GameObject baseControllerPanel;
        public Button[] colorBtns;
        public Button resetBtn;
        public Slider scaleSlider;

        private TargetModelDisplayCtrl m_ModelCtrl;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            m_ModelCtrl = FindObjectOfType<TargetModelDisplayCtrl>();
            for (int i = 0; i < colorBtns.Length; i++)
            {
                int k = i;
                colorBtns[i].onClick.AddListener(() => { OnColorButtonClick(k); });
            }
            showBtn.onClick.AddListener(() => { SetBaseControllerEnabled(true); });
            hideBtn.onClick.AddListener(() => { SetBaseControllerEnabled(false); });
            resetBtn.onClick.AddListener(OnResetButtonClick);
            scaleSlider.onValueChanged.AddListener(OnScaleSliderValueChanged);
        }

        private void OnColorButtonClick(int index)
        {
            m_ModelCtrl.ChangeModelColor(colorBtns[index].image.color);
        }

        private void OnScaleSliderValueChanged(float val)
        {
            m_ModelCtrl.ChangeModelScale(val);
        }

        private void OnResetButtonClick()
        {
            m_ModelCtrl.ResetModel();
            scaleSlider.value = 0f;
        }

        private void SetBaseControllerEnabled(bool isEnabled)
        {
            baseControllerPanel.SetActive(isEnabled);
        }
    }
}
