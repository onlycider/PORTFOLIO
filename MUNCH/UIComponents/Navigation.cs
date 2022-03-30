using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    private const float CONST_HEIGHT = 140f;
    private float mTotalHeight = 0f;
    private RectTransform mRectTransform;

    public float contentsHeight
    {
        get
        {
            if (mTotalHeight <= 0)
            {
                mTotalHeight = ScreenUtil.TopWhiteSpaceHeight + CONST_HEIGHT;
            }
            return mTotalHeight;
        }
    }

    void Awake()
    {
        mRectTransform = GetComponent<RectTransform>();
        mTotalHeight = contentsHeight;
        mRectTransform.pivot = new Vector2(0.5f, 0f);
        mRectTransform.anchoredPosition = new Vector2(0, -mTotalHeight);
        mRectTransform.sizeDelta = new Vector2(mRectTransform.sizeDelta.x, mTotalHeight);
    }
}
