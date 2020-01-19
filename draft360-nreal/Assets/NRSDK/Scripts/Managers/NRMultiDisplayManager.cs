using UnityEngine;

namespace NRKernal
{
    public class NRMultiDisplayManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_DefaultVirtualDisplayer;
        private NRVirtualDisplayer m_VirtualDisplayer;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            m_VirtualDisplayer = FindObjectOfType<NRVirtualDisplayer>();
            if (m_VirtualDisplayer == null)
                Instantiate(m_DefaultVirtualDisplayer);
        }
    }
}
