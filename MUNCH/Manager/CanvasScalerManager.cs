using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerManager : MonoBehaviour
{
    private CanvasScaler m_canvasScaler;
    private const float m_baseResolution = 16f/9f;
    void Start()
    {
        m_canvasScaler = GetComponent<CanvasScaler>();
        m_canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        m_canvasScaler.matchWidthOrHeight = 1f;
    }

    void FixedUpdate()
    {
        float screenResolution = (float)Screen.width / (float)Screen.height;
        if(screenResolution >= m_baseResolution)
            m_canvasScaler.matchWidthOrHeight = 1f;
        else
            m_canvasScaler.matchWidthOrHeight = 0f;
    }
}
