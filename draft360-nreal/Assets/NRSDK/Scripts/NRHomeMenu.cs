using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    public class NRHomeMenu : MonoBehaviour
    {
        public Button confirmBtn;
        public Button cancelBtn;

        private static NRHomeMenu m_Instance;
        private static bool m_IsShowing = false;
        private static string m_MenuPrefabPath = "NRUI/NRHomeMenu";

        void Start()
        {
            confirmBtn.onClick.AddListener(OnComfirmButtonClick);
            cancelBtn.onClick.AddListener(OnCancelButtonClick);
        }

        void Update()
        {
            if (m_IsShowing && m_Instance && Camera.main)
                m_Instance.FollowTarget(Camera.main.transform);
        }

        private void OnComfirmButtonClick()
        {
            Hide();
            AppManager.QuitApplication();
        }

        private void OnCancelButtonClick()
        {
            Hide();
        }

        private void FollowTarget(Transform target)
        {
            m_Instance.transform.position = target.position;
            m_Instance.transform.rotation = target.rotation;
        }

        private static void CreateMenu()
        {
            GameObject menuPrefab = Resources.Load<GameObject>(m_MenuPrefabPath);
            if (menuPrefab == null)
            {
                Debug.LogError("Can not find prefab: " + m_MenuPrefabPath);
                return;
            }
            GameObject menuGo = Instantiate(menuPrefab);
            m_Instance = menuGo.GetComponent<NRHomeMenu>();
            if (m_Instance)
                DontDestroyOnLoad(menuGo);
            else
                Destroy(menuGo);
        }

        public static void Toggle()
        {
            if (m_IsShowing)
                Hide();
            else
                Show();
        }

        public static void Show()
        {
            if (m_Instance == null)
                CreateMenu();
            if (m_Instance)
            {
                m_Instance.gameObject.SetActive(true);
                m_IsShowing = true;
            }
        }

        public static void Hide()
        {
            if (m_Instance)
            {
                m_Instance.gameObject.SetActive(false);
                m_IsShowing = false;
            }
        }
    }
}
