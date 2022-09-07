using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollingTexture : MonoBehaviour
{
    private Image m_image;
    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        float movePoint = 0.005f * Time.time;
        Vector2 offset = new Vector2(-movePoint, -movePoint);
        m_image.material.mainTextureOffset = offset;
    }
}
