namespace NRKernal.Record
{
    //
    // 摘要:
    //     The encoded image or video pixel format to use for PhotoCapture and VideoCapture.
    public enum CapturePixelFormat
    {
        //
        // 摘要:
        //     8 bits per channel (blue, green, red, and alpha).
        BGRA32 = 0,
        //
        // 摘要:
        //     8-bit Y plane followed by an interleaved U/V plane with 2x2 subsampling.
        NV12 = 1,
        //
        // 摘要:
        //     Encode photo in JPEG format.
        JPEG = 2,
        //
        // 摘要:
        //     Portable Network Graphics Format.
        PNG = 3
    }
}
