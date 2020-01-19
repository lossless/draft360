using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace NRKernal.NRExamples
{
    public class HiddenDebugPanel : MonoBehaviour
    {
        public Transform m_ButtonsRoot;
        private UserDefineButton[] Buttons;

        void Start()
        {
            Buttons = gameObject.GetComponentsInChildren<UserDefineButton>();
            m_ButtonsRoot.gameObject.SetActive(false);

            foreach (var item in Buttons)
            {
                item.OnClick += OnItemTriggerEvent;
            }
        }

        private void OnItemTriggerEvent(string key)
        {
            if (key.Equals("InvisibleBtn"))
            {
                m_ButtonsRoot.gameObject.SetActive(!m_ButtonsRoot.gameObject.activeInHierarchy);
            }
            else if (CanSceneLoaded(key))
            {
                SceneManager.LoadScene(key);
            }
        }

        private bool CanSceneLoaded(string name)
        {
            return (SceneUtility.GetBuildIndexByScenePath(name) != -1) &&
                !SceneManager.GetActiveScene().name.Equals(name);
        }
    }
}
