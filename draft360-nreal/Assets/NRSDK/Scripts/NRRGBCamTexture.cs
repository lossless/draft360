namespace NRKernal
{
    using System;
    using UnityEngine;

    /**
    * @brief Create a rgb camera texture.
    */
    public class NRRGBCamTexture : IDisposable
    {
        public Action<RGBTextureFrame> OnUpdate;

        public int Height
        {
            get
            {
                return NRRgbCamera.Resolution.height;
            }
        }

        public int Width
        {
            get
            {
                return NRRgbCamera.Resolution.width;
            }
        }

        private bool m_IsPlaying = false;
        public bool IsPlaying
        {
            get
            {
                return m_IsPlaying;
            }
        }

        public bool DidUpdateThisFrame
        {
            get
            {
                return NRRgbCamera.HasFrame();
            }
        }

        public int FrameCount = 0;

        private Texture2D m_texture;

        public RGBTextureFrame CurrentFrame;

        public NRRGBCamTexture()
        {
            m_texture = new Texture2D(NRRgbCamera.Resolution.width, NRRgbCamera.Resolution.height, TextureFormat.RGB24, false);
        }

        public void Play()
        {
            if (m_IsPlaying)
            {
                return;
            }
            NRRgbCamera.OnImageUpdate -= OnFrameUpdated;
            NRRgbCamera.OnImageUpdate += OnFrameUpdated;
            NRKernalUpdater.Instance.OnUpdate -= UpdateTexture;
            NRKernalUpdater.Instance.OnUpdate += UpdateTexture;
            NRRgbCamera.Play();
            m_IsPlaying = true;
        }

        public void Pause()
        {
            Stop();
        }

        public Texture2D GetTexture()
        {
            return m_texture;
        }

        private void OnFrameUpdated()
        {
            FrameCount++;
        }

        private void UpdateTexture()
        {
            if (!NRRgbCamera.HasFrame())
            {
                return;
            }
            RGBRawDataFrame rgbRawDataFrame = NRRgbCamera.GetRGBFrame();

            m_texture.LoadRawTextureData(rgbRawDataFrame.data);
            m_texture.Apply();

            CurrentFrame.timeStamp = rgbRawDataFrame.timeStamp;
            CurrentFrame.texture = m_texture;

            if (OnUpdate != null)
            {
                OnUpdate(CurrentFrame);
            }
        }

        public void Stop()
        {
            if (!m_IsPlaying)
            {
                return;
            }
            NRRgbCamera.OnImageUpdate -= OnFrameUpdated;
            NRKernalUpdater.Instance.OnUpdate -= UpdateTexture;
            NRRgbCamera.Stop();
            m_IsPlaying = false;
        }

        public void Dispose()
        {
            NRRgbCamera.Release();
            GameObject.Destroy(m_texture);
        }
    }

    public struct RGBTextureFrame
    {
        public UInt64 timeStamp;
        public Texture2D texture;
    }
}
