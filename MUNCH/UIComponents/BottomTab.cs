using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomTab : MonoBehaviour
{
    private RectTransform mRectTransform;

    private float mTotalHeight = 0f;

    public float contentsHeight
    {
        get {
            if(mTotalHeight <= 0f)
            {
                SetContentsHeight();
            }
            return mTotalHeight;
        }
    }
    void Awake()
    {
        SetContentsHeight();
    }

    private void SetContentsHeight()
    {
        if(mRectTransform == null)
        {
            mRectTransform = GetComponent<RectTransform>();
        }

        mRectTransform = GetComponent<RectTransform>();
        float baseWidth = mRectTransform.sizeDelta.x;
        float baseHeight = mRectTransform.sizeDelta.y;

        float totalHeight = baseHeight + ScreenUtil.BottomWhiteSpaceHeight;
        mRectTransform.sizeDelta = new Vector2(baseWidth, totalHeight);
    }
}
