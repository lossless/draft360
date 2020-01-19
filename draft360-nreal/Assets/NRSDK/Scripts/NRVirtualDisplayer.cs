using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace NRKernal
{
    public class NRVirtualDisplayer : SingletonBehaviour<NRVirtualDisplayer>
    {
        [SerializeField]
        private Camera m_UICamera;
        [SerializeField]
        private MultiScreenController m_VirtualController;

        private bool m_IsInit = false;
        private Vector3 m_StartPos = Vector3.one * 1000f;
        private Vector2 m_ScreenResolution;
        private static RenderTexture m_ControllerScreen;
        private static IntPtr m_RenderTexturePtr;

        private delegate void RenderEventDelegate(int eventID);
        private static RenderEventDelegate RenderThreadHandle = new RenderEventDelegate(RunOnRenderThread);
        private static IntPtr RenderThreadHandlePtr = Marshal.GetFunctionPointerForDelegate(RenderThreadHandle);

        internal static NativeMultiDisplay NativeMultiDisplay { get; private set; }

        public Action OnMultiDisplayInited;

        public static bool RunInBackground;
        private bool m_IsPlay = false;

        private void OnApplicationPause(bool pause)
        {
            // If NRSessionBehaviour is exist, do not oprate.
            if (NRSessionManager.Instance.NRSessionBehaviour != null)
            {
                return;
            }

            if (pause)
            {
                if (RunInBackground)
                {
                    this.Pause();
                }
                else
                {
                    NRDevice.Instance.ForceKill();
                }
            }
            else
            {
                this.Resume();
            }
        }

        public void Pause()
        {
            if (!m_IsInit || !m_IsPlay)
            {
                return;
            }
#if UNITY_ANDROID && !UNITY_EDITOR
            NativeMultiDisplay.Pause();
#endif
            m_IsPlay = false;
        }

        public void Resume()
        {
            if (!m_IsInit || m_IsPlay)
            {
                return;
            }
#if UNITY_ANDROID && !UNITY_EDITOR
            NativeMultiDisplay.Resume();
#endif
            m_IsPlay = true;
        }

        new private void Awake()
        {
            m_UICamera.enabled = false;
        }

        private void Start()
        {
            Invoke("Init", 0.1f);
        }

        private void Update()
        {
            if (!m_IsInit)
                return;
#if UNITY_EDITOR
            UpdateEmulator();
            if (m_VirtualController)
                m_VirtualController.gameObject.SetActive(NRInput.EmulateVirtualDisplayInEditor);
#endif
        }

        public void Destory()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (NativeMultiDisplay != null)
            {
                NativeMultiDisplay.Destroy();
                NativeMultiDisplay = null;
            }
#endif
            if (m_ControllerScreen != null)
            {
                m_ControllerScreen.Release();
                m_ControllerScreen = null;
            }
        }

        new void OnDestroy()
        {
            if (isDirty)
            {
                return;
            }
            base.OnDestroy();
            Destory();
        }

        private void Init()
        {
            if (m_IsInit)
                return;
            NRDebugger.Log("[MultiScreenController] Init.");
            NRDevice.Instance.Init();
            m_ScreenResolution = NRPhoneScreen.Resolution;
            transform.position = m_StartPos;
            DontDestroyOnLoad(transform);
            m_UICamera.enabled = true;
            m_UICamera.aspect = m_ScreenResolution.x / m_ScreenResolution.y;
            m_UICamera.orthographicSize = 6;

            m_ControllerScreen = new RenderTexture((int)m_ScreenResolution.x, (int)m_ScreenResolution.y, 24);
            m_UICamera.targetTexture = m_ControllerScreen;
            m_RenderTexturePtr = m_ControllerScreen.GetNativeTexturePtr();
            NRSessionManager.Instance.VirtualDisplayer = this;

#if UNITY_ANDROID && !UNITY_EDITOR
            NativeMultiDisplay = new NativeMultiDisplay();
            NativeMultiDisplay.Create();
            // Creat multiview controller..
            GL.IssuePluginEvent(RenderThreadHandlePtr, 0);
#elif UNITY_EDITOR
            InitEmulator();
#endif
            if (m_VirtualController)
                m_VirtualController.Init();
            NRDebugger.Log("[MultiScreenController] Init successed.");
            m_IsInit = true;
            if (OnMultiDisplayInited != null)
                OnMultiDisplayInited();

            m_IsPlay = true;
        }

        [MonoPInvokeCallback(typeof(RenderEventDelegate))]
        private static void RunOnRenderThread(int eventID)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            NativeMultiDisplay.UpdateHomeScreenTexture(m_RenderTexturePtr);
#endif
        }

#if UNITY_EDITOR
        private static Vector2 m_EmulatorTouch = Vector2.zero;
        private Vector2 m_EmulatorPhoneScreenAnchor;
        private float m_EditorScreenWidth;
        private float m_EditorScreenHeight;
        private float m_EmulatorRawImageWidth;
        private float m_EmulatorRawImageHeight;

        public static Vector2 GetEmulatorScreenTouch()
        {
            return m_EmulatorTouch;
        }

        private void InitEmulator()
        {
            GameObject emulatorVirtualController = new GameObject("NREmulatorVirtualController");
            DontDestroyOnLoad(emulatorVirtualController);
            Canvas controllerCanvas = emulatorVirtualController.AddComponent<Canvas>();
            controllerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            GameObject rawImageObj = new GameObject("RamImage");
            rawImageObj.transform.parent = controllerCanvas.transform;
            RawImage emulatorPhoneRawImage = rawImageObj.AddComponent<RawImage>();
            emulatorPhoneRawImage.raycastTarget = false;
            float scaleRate = 0.18f;
            m_EmulatorRawImageWidth = m_ScreenResolution.x * scaleRate;
            m_EmulatorRawImageHeight = m_ScreenResolution.y * scaleRate;
            emulatorPhoneRawImage.rectTransform.pivot = Vector2.right;
            emulatorPhoneRawImage.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, 0f);
            emulatorPhoneRawImage.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0f, 0f);
            emulatorPhoneRawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_EmulatorRawImageWidth);
            emulatorPhoneRawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_EmulatorRawImageHeight);
            emulatorPhoneRawImage.texture = m_ControllerScreen;

            Vector2 gameViewSize = new Vector2(m_ScreenResolution.x, m_ScreenResolution.y);

            float gameViewPixelWidth = 0, gameViewPixelHeight = 0;
            float gameViewAspect = 0;

            if (NRPhoneScreen.GetGameViewSize(out gameViewPixelWidth, out gameViewPixelHeight, out gameViewAspect))
            {
                if (gameViewPixelWidth != 0 && gameViewPixelHeight != 0)
                {
                    gameViewSize.x = gameViewPixelWidth;
                    gameViewSize.y = gameViewPixelHeight;
                }
            }

            m_EditorScreenWidth = gameViewSize.x;
            m_EditorScreenHeight = gameViewSize.y;
            m_EmulatorPhoneScreenAnchor = new Vector2(m_EditorScreenWidth - m_EmulatorRawImageWidth, 0f);
        }

        private void UpdateEmulator()
        {
            if (NRInput.EmulateVirtualDisplayInEditor
                && Input.GetMouseButton(0)
                && Input.mousePosition.x > m_EmulatorPhoneScreenAnchor.x
                && Input.mousePosition.y < (m_EmulatorPhoneScreenAnchor.y + m_EmulatorRawImageHeight))
            {
                m_EmulatorTouch.x = (Input.mousePosition.x - m_EmulatorPhoneScreenAnchor.x) / m_EmulatorRawImageWidth * 2f - 1f;
                m_EmulatorTouch.y = (Input.mousePosition.y - m_EmulatorPhoneScreenAnchor.y) / m_EmulatorRawImageHeight * 2f - 1f;
            }
            else
            {
                m_EmulatorTouch = Vector2.zero;
            }
        }
#endif
    }
}
