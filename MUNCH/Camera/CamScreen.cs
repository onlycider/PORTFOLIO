using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MunchProject;

public class CamScreen : MonoBehaviour
{
    private WebCamTexture m_webCamTexture;

    private RawImage m_rawImage;
    public float camScreenWidth
    {
        get
        {
            if(m_webCamTexture != null)
            {
                int rotateAngle = m_webCamTexture.videoRotationAngle;
                float width = m_rawImage.rectTransform.sizeDelta.x;
                float height = m_rawImage.rectTransform.sizeDelta.y;
                return rotateAngle == 0 ? width : height;  
            }
            return m_rawImage.rectTransform.sizeDelta.x;
        }
    }
    public float camScreenHeight
    {
        get
        {
            if(m_webCamTexture != null)
            {
                int rotateAngle = m_webCamTexture.videoRotationAngle;
                float width = m_rawImage.rectTransform.sizeDelta.x;
                float height = m_rawImage.rectTransform.sizeDelta.y;
                return rotateAngle == 0 ? height : width;  
            }
            return m_rawImage.rectTransform.sizeDelta.y;
        }
    }

    private const int STANDARD_WIDTH = 1080;

    public GameObject centerPositionMark;

    void Awake()
    {
        SetRawTexture();
        DrawCamTexture();
    }

    // Start is called before the first frame update
    // void Start()
    // {

    // }

    private void SetRawTexture()
    {
        if (m_rawImage != null)
            return;

        RawImage rawImage = GetComponent<RawImage>();
        if (rawImage != null)
        {
            m_rawImage = rawImage;
        }
    }

    private void DrawCamTexture()
    {
        if (m_rawImage == null)
            return;

        int camHeight = 1920;
        // 후면 메인 카메라
        WebCamDevice device = WebCamTexture.devices[0];

        // 추후 모바일 폰에서 비율 관련되어 수정이 필요할수 있음
        Resolution[] resolutions = device.availableResolutions;
        // Available on iOS and Android only. Returns null for other platforms.
        if (resolutions == null)
        {
            goto SetResolution;
        }

        List<Resolution> standardResolutions = new List<Resolution>();
        foreach (Resolution resolution in resolutions)
        {
            if (resolution.height == STANDARD_WIDTH)
            {
                standardResolutions.Add(resolution);
            }
        }

        foreach (Resolution resolution in standardResolutions)
        {
            Debug.Log($"resolution width, height : {resolution.width}, {resolution.height}");
            if (camHeight < resolution.width)
            {
                camHeight = resolution.width;
            }
        }

        Debug.Log($"selected width :: {camHeight}");
        Debug.Log($"Screen.width :: {Screen.width}");

    SetResolution:
        m_webCamTexture = new WebCamTexture(device.name, STANDARD_WIDTH, camHeight, 120);
        m_rawImage.texture = m_webCamTexture;
        m_webCamTexture.Play();

        float widthRes = (float)STANDARD_WIDTH / (float)Screen.width;
#if UNITY_EDITOR
        m_rawImage.rectTransform.sizeDelta = new Vector2((float)camHeight / widthRes, (float)Screen.width);
#elif UNITY_ANDROID && !UNITY_EDITOR
        m_rawImage.rectTransform.sizeDelta = new Vector2((float)camHeight / widthRes, (float)Screen.width);
#elif UNITY_IOS && !UNITY_EDITOR
        m_rawImage.rectTransform.sizeDelta = new Vector2((float)camHeight, (float)STANDARD_WIDTH);
        m_rawImage.transform.localScale = new Vector3(1f, -1f, 1f);
#endif
        // m_rawImage.rectTransform.anchorMin = new Vector2(0.5f, 1f);
        // m_rawImage.rectTransform.anchorMax = new Vector2(0.5f, 1f);
        // m_rawImage.rectTransform.pivot = new Vector2(0f, 0.5f);
        m_rawImage.rectTransform.anchoredPosition = Vector2.zero;
        m_rawImage.transform.eulerAngles = m_webCamTexture.videoRotationAngle * Vector3.back;
        // m_rawImage.rectTransform.anchoredPosition = new Vector2(0f, (Screen.height / 2) - (m_rawImage.rectTransform.sizeDelta.x / 2));
        centerPositionMark.transform.localPosition = Vector3.zero;
    }

    public void PauseWebCamTexture()
    {
        m_webCamTexture.Pause();
    }

    public void ResumeWebCamTexture()
    {
        m_webCamTexture.Play();
    }

    public void StopWebCamTexture()
    {
        m_webCamTexture.Stop();
    }

    public Texture2D GetPauseWebCamTexture()
    {

        // Texture2D capturedTexture = (Texture2D)m_rawImage.texture;
        // Debug.Log($"rawImage Texture width : {m_rawImage.texture.width}");
        // Debug.Log($"rawImage Texture height : {m_rawImage.texture.height}");
        Texture2D capturedTexture = new Texture2D(m_rawImage.texture.width, m_rawImage.texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(m_rawImage.texture.width, m_rawImage.texture.height, 32);

        Graphics.Blit(m_rawImage.texture, renderTexture);

        RenderTexture.active = renderTexture;
        capturedTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        capturedTexture.Apply(false, false);

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        if (capturedTexture == null)
            Debug.Log("texture2d is null");


        Color32[] original = capturedTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];

        int xLength = capturedTexture.width;
        int yLength = capturedTexture.height;

        int rotatedIndex;
        int originalIndex;

        for (int y = 0; y < yLength; ++y)
        {
            for (int x = 0; x < xLength; ++x)
            {
                rotatedIndex = (x + 1) * yLength - y - 1;
#if UNITY_EDITOR || UNITY_ANDROID
                originalIndex = original.Length - 1 - (y * xLength + x);
#elif UNITY_IOS
                originalIndex = original.Length - 1 - ((yLength - 1 - y) * xLength + x);
#endif
                rotated[rotatedIndex] = original[originalIndex];
            }
        }

        Texture2D texture2D = new Texture2D(yLength, xLength, capturedTexture.format, false);
        texture2D.SetPixels32(rotated);
        texture2D.Apply();
        return texture2D;
    }


}
