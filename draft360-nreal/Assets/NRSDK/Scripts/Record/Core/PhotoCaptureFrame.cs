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
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    //
    // 摘要:
    //     Contains information captured from the web camera.
    public sealed class PhotoCaptureFrame : IDisposable
    {
        private byte[] data;

        public PhotoCaptureFrame(CapturePixelFormat format, byte[] data)
        {
            this.data = data;
            this.pixelFormat = format;
        }

        ~PhotoCaptureFrame()
        {

        }

        //
        // 摘要:
        //     The length of the raw IMFMediaBuffer which contains the image captured.
        public int dataLength { get; }
        //
        // 摘要:
        //     Specifies whether or not spatial data was captured.
        public bool hasLocationData { get; }
        //
        // 摘要:
        //     The raw image data pixel format.
        public CapturePixelFormat pixelFormat { get; }

        public void CopyRawImageDataIntoBuffer(List<byte> byteBuffer)
        {

        }
        //
        // 摘要:
        //     Disposes the PhotoCaptureFrame and any resources it uses.
        public void Dispose()
        {

        }
        //
        // 摘要:
        //     Provides a COM pointer to the native IMFMediaBuffer that contains the image data.
        //
        // 返回结果:
        //     A native COM pointer to the IMFMediaBuffer which contains the image data.
        public IntPtr GetUnsafePointerToBuffer()
        {
            return IntPtr.Zero;
        }
        public bool TryGetCameraToWorldMatrix(out Matrix4x4 cameraToWorldMatrix)
        {
            cameraToWorldMatrix = Matrix4x4.identity;
            return true;
        }
        public bool TryGetProjectionMatrix(out Matrix4x4 projectionMatrix)
        {
            projectionMatrix = Matrix4x4.identity;
            return true;
        }
        public bool TryGetProjectionMatrix(float nearClipPlane, float farClipPlane, out Matrix4x4 projectionMatrix)
        {
            projectionMatrix = Matrix4x4.identity;
            return true;
        }

        //
        // 摘要:
        //     This method will copy the captured image data into a user supplied texture for
        //     use in Unity.
        //
        // 参数:
        //   targetTexture:
        //     The target texture that the captured image data will be copied to.
        public void UploadImageDataToTexture(Texture2D targetTexture)
        {
            ImageConversion.LoadImage(targetTexture, data);
        }
    }
}
