namespace NRKernal
{
    using UnityEngine;

    public class NRPhoneScreen
    {
        private static float m_ScreenWidth = 0;
        private static float m_ScreenHeight = 0;

        public const float DefaultWidth = 1080;
        public const float DefaultHeight = 2340;

        public static Vector2 Resolution
        {
            get
            {
                if (m_ScreenWidth < float.Epsilon || m_ScreenHeight < float.Epsilon)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    AndroidJavaClass j = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject currentActivity = j.GetStatic<AndroidJavaObject>("currentActivity");
                    AndroidJavaObject displayManager = currentActivity.Call<AndroidJavaObject>("getSystemService", new AndroidJavaObject("java.lang.String", "display"));
                    AndroidJavaObject display = displayManager.Call<AndroidJavaObject>("getDisplay", 0);
                    AndroidJavaObject outSize = new AndroidJavaObject("android.graphics.Point");
                    display.Call("getRealSize", outSize);
                    m_ScreenWidth = outSize.Get<int>("x");
                    m_ScreenHeight = outSize.Get<int>("y");
#elif UNITY_EDITOR
                    m_ScreenWidth = DefaultWidth;
                    m_ScreenHeight = DefaultHeight;
#endif
                    NRDebugger.Log(string.Format("[NRPhoneScreen] width:{0} height:{1}", m_ScreenWidth, m_ScreenHeight));
                }

                return new Vector2(m_ScreenWidth, m_ScreenHeight);
            }
        }

#if UNITY_EDITOR
        public static bool GetGameViewSize(out float width, out float height, out float aspect)
        {
            try
            {
                System.Type gameViewType = System.Type.GetType("UnityEditor.GameView,UnityEditor");
                System.Reflection.MethodInfo GetMainGameView = gameViewType.GetMethod("GetMainGameView", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                object mainGameViewInst = GetMainGameView.Invoke(null, null);
                if (mainGameViewInst == null)
                {
                    width = height = aspect = 0;
                    return false;
                }
                System.Reflection.FieldInfo s_viewModeResolutions = gameViewType.GetField("s_viewModeResolutions", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (s_viewModeResolutions == null)
                {
                    System.Reflection.PropertyInfo currentGameViewSize = gameViewType.GetProperty("currentGameViewSize", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    object gameViewSize = currentGameViewSize.GetValue(mainGameViewInst, null);
                    System.Type gameViewSizeType = gameViewSize.GetType();
                    int gvWidth = (int)gameViewSizeType.GetProperty("width").GetValue(gameViewSize, null);
                    int gvHeight = (int)gameViewSizeType.GetProperty("height").GetValue(gameViewSize, null);
                    int gvSizeType = (int)gameViewSizeType.GetProperty("sizeType").GetValue(gameViewSize, null);
                    if (gvWidth == 0 || gvHeight == 0)
                    {
                        width = height = aspect = 0;
                        return false;
                    }
                    else if (gvSizeType == 0)
                    {
                        width = height = 0;
                        aspect = (float)gvWidth / (float)gvHeight;
                        return true;
                    }
                    else
                    {
                        width = gvWidth; height = gvHeight;
                        aspect = (float)gvWidth / (float)gvHeight;
                        return true;
                    }
                }
                else
                {
                    Vector2[] viewModeResolutions = (Vector2[])s_viewModeResolutions.GetValue(null);
                    float[] viewModeAspects = (float[])gameViewType.GetField("s_viewModeAspects", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(null);
                    string[] viewModeStrings = (string[])gameViewType.GetField("s_viewModeAspectStrings", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(null);
                    if (mainGameViewInst != null
                        && viewModeStrings != null
                        && viewModeResolutions != null && viewModeAspects != null)
                    {
                        int aspectRatio = (int)gameViewType.GetField("m_AspectRatio", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(mainGameViewInst);
                        string thisViewModeString = viewModeStrings[aspectRatio];
                        if (thisViewModeString.Contains("Standalone"))
                        {
                            width = UnityEditor.PlayerSettings.defaultScreenWidth; height = UnityEditor.PlayerSettings.defaultScreenHeight;
                            aspect = width / height;
                        }
                        else if (thisViewModeString.Contains("Web"))
                        {
                            width = UnityEditor.PlayerSettings.defaultWebScreenWidth; height = UnityEditor.PlayerSettings.defaultWebScreenHeight;
                            aspect = width / height;
                        }
                        else
                        {
                            width = viewModeResolutions[aspectRatio].x; height = viewModeResolutions[aspectRatio].y;
                            aspect = viewModeAspects[aspectRatio];
                            // this is an error state
                            if (width == 0 && height == 0 && aspect == 0)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("GameCamera.GetGameViewSize - has a Unity update broken this?\nThis is not a fatal error !\n" + e.ToString());
            }
            width = height = aspect = 0;
            return false;
        }
#endif
    }
}
