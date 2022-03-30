using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    private RectTransform mRectTransform;
    public Navigation navigation;
    public BottomTab bottomTab;

    private void Awake()
    {
        mRectTransform = GetComponent<RectTransform>();

        float contentsHeight = Screen.height * (mRectTransform.sizeDelta.x / Screen.width);
        float yPosition = 0f;
        if (navigation != null)
        {
            contentsHeight -= navigation.contentsHeight;
            yPosition = -navigation.contentsHeight;
        }

        if(bottomTab != null)
        {
            contentsHeight -= bottomTab.contentsHeight;
        }

        mRectTransform.sizeDelta = new Vector2(mRectTransform.sizeDelta.x, contentsHeight);
        mRectTransform.anchoredPosition = new Vector2(0f, yPosition);
    }
}