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
    using UnityEngine;

    public class ImageEncoder : IEncoder
    {
        public delegate void ImageEncoderCallBack(RenderTexture rt, ulong timestamp);
        public ImageEncoderCallBack OnCommit;

        public ImageEncoder()
        {
        }

        public ImageEncoder(ImageEncoderCallBack oncommit)
        {
            this.OnCommit = oncommit;
        }

        public void Commit(RenderTexture rt, ulong timestamp)
        {
            if (OnCommit != null)
            {
                OnCommit(rt, timestamp);
            }
        }

        public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            NRDebugger.Log("[ImageEncoder] ScaleTexture..");
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                    result.SetPixel(j, i, newColor);
                }
            }

            result.Apply();
            return result;
        }
    }
}
