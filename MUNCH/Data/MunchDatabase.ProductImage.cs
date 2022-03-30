using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public partial class MunchDatabase
    {
        private Texture2D m_originalImage;
        public Texture2D originalImage { get { return m_originalImage; } }

        private Texture2D m_squareImage;

        private Texture2D m_vgaImage;
        // private ProductImage m_originalImage;
        // public ProductImage originalImage{get{return m_originalImage;}}

        public void InitializeProductImage()
        {
            m_originalImage = null;
            m_squareImage = null;
            m_vgaImage = null;
        }

        public void SetProductImage(Texture2D texture)
        {
            m_originalImage = texture;
        }

        public void SaveProductImage(string directoryName)
        {
            if (m_originalImage == null)
            {
                return;
            }

            string filePath = $"{directoryName}product_image.png";
            byte[] bytes = m_originalImage.EncodeToPNG();

            FileStream fileStream = File.OpenWrite(filePath);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
        }

        public Texture2D LoadRegisterdProductImage(string directoryName)
        {
            string filePath = $"{directoryName}/product_image.png";
            // Debug.Log(filePath);
            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
            texture.LoadImage(bytes);
            return texture;
        }

        public Texture2D GetSquareTexture()
        {
            if (m_originalImage == null)
            {
                return null;
            }

            if (m_squareImage != null)
            {
                return m_squareImage;
            }

            int w = m_originalImage.width;
            int h = m_originalImage.height;

            const int cropWidth = 1080;

            int yOffset = (h / 2) - (cropWidth / 2);

            int cropIndex = 0;

            Color32[] source = m_originalImage.GetPixels32();
            Color32[] centerImage = new Color32[cropWidth * cropWidth];
            for (int y = 0; y < cropWidth; y++)
            {
                for (int x = 0; x < cropWidth; x++)
                {
                    cropIndex = x + (y + yOffset) * cropWidth;
                    centerImage[y * cropWidth + x] = source[cropIndex];
                }
            }

            m_squareImage = new Texture2D(cropWidth, cropWidth, m_originalImage.format, false);
            m_squareImage.SetPixels32(centerImage);
            m_squareImage.Apply();
            return m_squareImage;
        }

        public Texture2D GetVgaTexture()
        {
            if (m_originalImage == null)
            {
                return null;
            }

            if (m_vgaImage != null)
            {
                return m_vgaImage;
            }

            int w = m_originalImage.width;
            int h = m_originalImage.height;


            const int cropWidth = 1080;
            const int cropHeight = 810;

            int yOffset = (h / 2) - (cropHeight / 2);

            Color32[] source = m_originalImage.GetPixels32();
            Color32[] centerImage = new Color32[cropWidth * cropHeight];

            int cropIndex = 0;
            for (int y = 0; y < cropHeight; y++)
            {
                for (int x = 0; x < cropWidth; x++)
                {
                    cropIndex = ((y + yOffset) * cropWidth) + x;
                    centerImage[y * cropWidth + x] = source[cropIndex];
                }
            }

            m_vgaImage = new Texture2D(cropWidth, cropHeight, m_originalImage.format, false);
            m_vgaImage.SetPixels32(centerImage);
            m_vgaImage.Apply();
            return m_vgaImage;
        }
    }
}

