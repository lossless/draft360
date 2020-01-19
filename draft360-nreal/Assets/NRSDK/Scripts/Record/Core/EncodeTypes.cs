/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    public enum CodecType
    {
        Local = 0,
        Rtmp = 1,
        Rtp = 2,
    }

    public enum BlendMode
    {
        RGBOnly,
        VirtualOnly,
        Blend,
        WidescreenBlend
    }

    public delegate void CaptureTaskCallback(CaptureTask task, byte[] data);

    public struct CaptureTask
    {
        public int Width;
        public int Height;
        public BlendMode BlendMode;
        public PhotoCaptureFileOutputFormat CaptureFormat;
        public CaptureTaskCallback OnReceive;

        public CaptureTask(int w, int h, BlendMode mode, PhotoCaptureFileOutputFormat format, CaptureTaskCallback callback)
        {
            this.Width = w;
            this.Height = h;
            this.BlendMode = mode;
            this.CaptureFormat = format;
            this.OnReceive = callback;
        }
    }

    public struct NativeEncodeConfig
    {
        public int width;
        public int height;
        public int bitRate;
        public int fps;
        public int codecType;    // 0 local; 1 rtmp ; 2...
        public string outPutPath;
        public int useStepTime;
        public bool useAlpha;

        public NativeEncodeConfig(int w, int h, int bitrate, int f, CodecType codectype, string path, bool usealpha = false)
        {
            this.width = w;
            this.height = h;
            this.bitRate = bitrate;
            this.fps = 20;
            this.codecType = (int)codectype;
            this.outPutPath = path;
            this.useStepTime = 0;
            this.useAlpha = usealpha;
        }

        public NativeEncodeConfig(CameraParameters cameraparam, string path = "")
        {
            this.width = cameraparam.blendMode == BlendMode.WidescreenBlend ? 2 * cameraparam.cameraResolutionWidth : cameraparam.cameraResolutionWidth;
            this.height = cameraparam.cameraResolutionHeight;
            this.bitRate = 4096000;
            this.fps = cameraparam.frameRate;
            this.codecType = path.Contains("rtmp") ? 1 : 0;
            this.outPutPath = path;
            this.useStepTime = 0;
            this.useAlpha = false;
        }

        public void SetOutPutPath(string path)
        {
            this.codecType = path.Contains("rtmp") ? 1 : 0;
            this.outPutPath = path;
        }

        public NativeEncodeConfig(NativeEncodeConfig config)
            : this(config.width, config.height, config.bitRate, config.fps, (CodecType)config.codecType, config.outPutPath, config.useAlpha)
        {
        }

        public override string ToString()
        {
            return LitJson.JsonMapper.ToJson(this);
        }
    }
}
