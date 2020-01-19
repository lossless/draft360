using UnityEngine;
using UnityEngine.SceneManagement;

namespace NRKernal.NRExamples
{
    public class ExamplesHub : SingletonBehaviour<ExamplesHub>
    {
        private string[] m_Scenes = new string[] {
            "HelloMR",
            "ImageTracking",
            "Input-ControllerInfo",
            "Input-Interaction",
            "RGBCamera",
            "RGBCamera-Capture",
            "RGBCamera-Record"
        };
        private int m_CurrentIndex = 0;
        public int CurrentIndex
        {
            get
            {
                return m_CurrentIndex;
            }
            private set
            {
                m_CurrentIndex = value;
                if (m_CurrentIndex < 0 || m_CurrentIndex >= m_Scenes.Length)
                {
                    m_CurrentIndex = 0;
                }
            }
        }
        private bool m_IsLock = false;

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                LoadNextScene();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                LoadLastScene();
            }

#endif
            if (NRInput.GetTouch().x > 0.8f)
            {
                LoadNextScene();
            }
            if (NRInput.GetTouch().x < -0.8f)
            {
                LoadLastScene();
            }
        }

        public void LoadNextScene()
        {
            if (m_IsLock)
            {
                return;
            }

            m_IsLock = true;
            CurrentIndex++;
            if (CanSceneLoaded(m_Scenes[CurrentIndex]))
            {
                SceneManager.LoadScene(m_Scenes[CurrentIndex]);
            }
            Invoke("Unlock", 1f);
        }

        public void LoadLastScene()
        {
            if (m_IsLock)
            {
                return;
            }

            m_IsLock = true;
            CurrentIndex--;
            if (CanSceneLoaded(m_Scenes[CurrentIndex]))
            {
                SceneManager.LoadScene(m_Scenes[CurrentIndex]);
            }
            Invoke("Unlock", 1f);
        }

        private void Unlock()
        {
            m_IsLock = false;
        }

        private bool CanSceneLoaded(string name)
        {
            return (SceneUtility.GetBuildIndexByScenePath(name) != -1) &&
                !SceneManager.GetActiveScene().name.Equals(name);
        }
    }
}
