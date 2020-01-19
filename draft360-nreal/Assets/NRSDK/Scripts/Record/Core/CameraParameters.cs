namespace NRKernal.Record
{
    public struct CameraParameters
    {
        public CameraParameters(CamMode webCamMode, BlendMode mode)
        {
            this.hologramOpacity = 1f;
            this.frameRate = 20;

            this.cameraResolutionWidth = NRRgbCamera.Resolution.width;
            this.cameraResolutionHeight = NRRgbCamera.Resolution.height;

            this.pixelFormat = CapturePixelFormat.BGRA32;
            this.blendMode = mode;
        }

        //
        // 摘要:
        //     The opacity of captured holograms.
        public float hologramOpacity { get; set; }
        //
        // 摘要:
        //     The framerate at which to capture video. This is only for use with VideoCapture.
        public int frameRate { get; set; }
        //
        // 摘要:
        //     A valid width resolution for use with the web camera.
        public int cameraResolutionWidth { get; set; }
        //
        // 摘要:
        //     A valid height resolution for use with the web camera.
        public int cameraResolutionHeight { get; set; }
        //
        // 摘要:
        //     The pixel format used to capture and record your image data.
        public CapturePixelFormat pixelFormat { get; set; }
        //
        // 摘要:
        //     The blend mode of camera output.
        public BlendMode blendMode { get; set; }
    }
}
