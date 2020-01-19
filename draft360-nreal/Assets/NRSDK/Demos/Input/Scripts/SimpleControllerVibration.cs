using UnityEngine;

namespace NRKernal.NRExamples
{
    public class SimpleControllerVibration : MonoBehaviour
    {
        public float vibrationTime = 0.06f;
        public ControllerButton[] vibrationButtons = { ControllerButton.TRIGGER, ControllerButton.APP, ControllerButton.HOME };

        void Update()
        {
            if (vibrationButtons == null || vibrationButtons.Length == 0)
                return;
            for (int i = 0; i < vibrationButtons.Length; i++)
            {
                if (NRInput.GetButtonDown(ControllerHandEnum.Right, vibrationButtons[i]))
                    NRInput.TriggerHapticVibration(ControllerHandEnum.Right, vibrationTime);
                if (NRInput.GetButtonDown(ControllerHandEnum.Left, vibrationButtons[i]))
                    NRInput.TriggerHapticVibration(ControllerHandEnum.Left, vibrationTime);
            }
        }
    }
}
