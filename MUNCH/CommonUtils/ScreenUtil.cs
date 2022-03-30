using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUtil
{
    public static bool IsSafeArea { get { return Screen.height != Screen.safeArea.height || Screen.width != Screen.safeArea.width; } }

    public const float FIX_RESOLUTION_WIDTH = 1080f;

    public static float scaleRatio { get { return FIX_RESOLUTION_WIDTH / Screen.width; } }

    public const float ANDROID_STATUSBAR_HEIGHT = 63f;
    public const float IOS_STATUSBAR_HEIGHT = 54f;

    private static float mTopWhiteSpaceHeight = 0f;
    private static float mBottomWhiteSpaceHeight = float.MinValue;

    public static float TopWhiteSpaceHeight
    {
        get
        {
            if (mTopWhiteSpaceHeight <= 0f)
            {
                float topBarHeight = (Screen.height - (Screen.safeArea.center.y + (Screen.safeArea.height / 2))) * (FIX_RESOLUTION_WIDTH / Screen.width);
#if UNITY_EDITOR
                mTopWhiteSpaceHeight = IsSafeArea ? topBarHeight : ANDROID_STATUSBAR_HEIGHT;
#elif UNITY_ANDROID && !UNITY_EDITOR
                mTopWhiteSpaceHeight = IsSafeArea ? topBarHeight : ANDROID_STATUSBAR_HEIGHT; 
#elif UNITY_IOS && !UNITY_EDITOR
                mTopWhiteSpaceHeight = IsSafeArea ? topBarHeight : IOS_STATUSBAR_HEIGHT;
#endif
                return mTopWhiteSpaceHeight;
            }
            return mTopWhiteSpaceHeight;
        }
    }

    public static float BottomWhiteSpaceHeight
    {
        get
        {
            if (mBottomWhiteSpaceHeight < 0f)
            {
                float bottomSpaceHeight = (Screen.safeArea.center.y - (Screen.safeArea.height / 2)) * (FIX_RESOLUTION_WIDTH / Screen.width);
                mBottomWhiteSpaceHeight = bottomSpaceHeight;
            }

            return mBottomWhiteSpaceHeight;
        }
    }

    private static float mScreenCenterX;
    private static float mScreenCenterY;

    public static float ScreenCenterX
    {
        get
        {
            if (mScreenCenterX <= 0f)
            {
                mScreenCenterX = Screen.width * 0.5f;
            }
            return mScreenCenterX;
        }
    }
    public static float ScreenCenterY
    {
        get
        {
            if (mScreenCenterY <= 0f)
            {
                mScreenCenterY = Screen.height * 0.5f;
            }
            return mScreenCenterY;
        }
    }

    private static void WriteScreenInfoToConsole()
    {
        Debug.Log($"Screen width, height : {Screen.width}, {Screen.height}");
        Debug.Log($"Safe Area width, height : {Screen.safeArea.width}, {Screen.safeArea.height}");
        Debug.Log($"Safe Area center : {Screen.safeArea.center}");
        Debug.Log($"Safe Area Max, Min: {Screen.safeArea.max}, {Screen.safeArea.min}");
        Debug.Log(ScreenUtil.IsSafeArea);
    }

}
