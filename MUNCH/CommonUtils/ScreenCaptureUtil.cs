using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScreenCaptureUtil : MonoBehaviour
{
    private Camera mCamera;

    public RectTransform targetRectTransform;

    public Canvas canvas;
    void Awake()
    {
        mCamera = GetComponent<Camera>();

        Debug.Log(mCamera.pixelWidth);
        Debug.Log(mCamera.pixelHeight);

        Debug.Log(mCamera.scaledPixelWidth);
        Debug.Log(mCamera.scaledPixelHeight);

    }
    // Start is called before the first frame update
    void Start()
    {
        // CaptureCameraRenderer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickTestCapture()
    {
        CaptureCameraRenderer();
    }

    private void CaptureCameraRenderer()
    {
        int renderWidth = (int)(Camera.main.pixelWidth * ScreenUtil.scaleRatio);
        int renderHeight = (int)(Camera.main.pixelHeight * ScreenUtil.scaleRatio);

        int cardWidth = (int)targetRectTransform.sizeDelta.x - 14;
        int cardHeight = (int)targetRectTransform.sizeDelta.y - 12;




        int xOffset = (renderWidth - cardWidth) / 2;
        int yOffset = (renderHeight - cardHeight) / 2;
        // - (renderHeight - (int)cardScreenPos.y * 2) / 2
        Debug.Log($"{xOffset}, {yOffset}");
        RenderTexture renderTexture = new RenderTexture(renderWidth, renderHeight, 24);
        Texture2D texture2D = new Texture2D(cardWidth, cardHeight, TextureFormat.ARGB32, false);

        mCamera.targetTexture = renderTexture;
        mCamera.Render();

        Vector2 cardScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, targetRectTransform.position);
        yOffset -= (renderHeight - (int)cardScreenPos.y * 2) / 2;
        Debug.Log(cardScreenPos);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(xOffset, yOffset, targetRectTransform.sizeDelta.x, targetRectTransform.sizeDelta.y), 0, 0);
        texture2D.Apply();
        string dir = $"{Application.persistentDataPath}/card/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string filePath = $"{dir}/card1.png";
        byte[] bytes = texture2D.EncodeToPNG();

        FileStream fileStream = File.OpenWrite(filePath);
        fileStream.Write(bytes, 0, bytes.Length);
        fileStream.Close();

        mCamera.targetTexture = null;
    }
}
