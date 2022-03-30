using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CloudVisionApiData;
using MunchProject;
using UnityEngine.UI;

public class APITypeButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public ApiType apiType;

    // private ApiType m_apiType;
    // public ApiType apiType{get{return m_apiType;}}

    private Action<APITypeButton> SELECTED_APITYPE_CALLBACK;

    private Image m_imageComponent;

    void Awake()
    {
        SetButtonText(apiType);
        m_imageComponent = GetComponent<Image>();
    }

    public void SetButtonText(ApiType type)
    {
        buttonText.text = type.ToString();
    }

    public void SetSelectedTypeCallback(Action<APITypeButton> callback)
    {
        SELECTED_APITYPE_CALLBACK = callback;
    }

    public void SetDeselectedState()
    {
        m_imageComponent.color = Color.white;
    }

    public void SetSelectedState()
    {
        m_imageComponent.color = Color.cyan;
    }

    public void OnClickAPITypeButton()
    {
        Utils.InvokeAction(SELECTED_APITYPE_CALLBACK, this);
    }
}
