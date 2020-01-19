using UnityEngine;

namespace NRKernal.NRExamples
{
    [DisallowMultipleComponent]
    public class AppManager : MonoBehaviour
    {
        private void OnEnable()
        {
            NRInput.AddClickListener(ControllerHandEnum.Right, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.AddClickListener(ControllerHandEnum.Left, ControllerButton.HOME, OnHomeButtonClick);
        }

        private void OnDisable()
        {
            NRInput.RemoveClickListener(ControllerHandEnum.Right, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.RemoveClickListener(ControllerHandEnum.Left, ControllerButton.HOME, OnHomeButtonClick);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Escape))
                QuitApplication();
#endif
        }

        private void OnHomeButtonClick()
        {
            NRHomeMenu.Toggle();
        }

        public static void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
            NRDevice.Instance.ForceKill();
#else
            Application.Quit();
#endif
        }
    }
}
