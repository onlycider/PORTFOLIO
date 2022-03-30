using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

namespace MunchProject
{
    public class ProductCard : MonoBehaviour
    {
        public enum ETwistPivotTypes
        {
            Top,
            Center,
            Bottom,
        }
        public TextMeshProUGUI productNameText;
        public TextMeshProUGUI categoryNameText;
        public TextMeshProUGUI manufacturerNameText;

        public TextMeshProUGUI descriptionText;

        public TextMeshProUGUI priceText;

        public RawImage productImage;

        private RegisteredProductInfo mRegisteredProductInfo;

        private UserRegisteredProductData mUserRegisteredProductInfo;
        private ProductData mProductData;

        private RectTransform mRectTransform;

        public RectTransform cardPivotRect;

        public Vector2 anchoredPosition
        {
            get { return mRectTransform.anchoredPosition; }
        }

        private ETwistPivotTypes mSelectedTwistPivotType;

        public Vector3 twistedEulerAngles { get { return cardPivotRect.eulerAngles; } }

        private CanvasRenderer mCanvasRenderer;

        public Action SAVE_CARD_ACTION;

        void Awake()
        {
            mRectTransform = GetComponent<RectTransform>();
            mCanvasRenderer = GetComponent<CanvasRenderer>();
        }

        public void SetProductCard(RegisteredProductInfo info)
        {
            mRectTransform = GetComponent<RectTransform>();
            mRegisteredProductInfo = info;
            mUserRegisteredProductInfo = mRegisteredProductInfo.UserRegisteredProductData;
            mProductData = mRegisteredProductInfo.ProductData;
            SetProductImage();
            SetProductNameText();
            SetCategoryNameText();
            SetManufacturerNameText();
            SetDescriptionText();
            SetPriceText();
        }

        public void SetAnchoredPosition(Vector2 position)
        {
            mRectTransform.anchoredPosition = position;
        }

        public void MovingCard(Vector2 movedPosition)
        {
            mRectTransform.anchoredPosition += movedPosition;
        }

        public void InitializeAnchoredPosition()
        {
            mRectTransform.anchoredPosition = Vector2.zero;
        }

        public void SetTwistPivotType(ETwistPivotTypes pivotType)
        {
            switch (pivotType)
            {
                case ETwistPivotTypes.Top:
                    cardPivotRect.pivot = new Vector2(0.5f, 1f);
                    cardPivotRect.anchorMax = new Vector2(0.5f, 1f);
                    cardPivotRect.anchorMin = new Vector2(0.5f, 1f);
                    break;
                case ETwistPivotTypes.Center:
                    cardPivotRect.pivot = new Vector2(0.5f, 0.5f);
                    cardPivotRect.anchorMax = new Vector2(0.5f, 0.5f);
                    cardPivotRect.anchorMin = new Vector2(0.5f, 0.5f);
                    break;
                case ETwistPivotTypes.Bottom:
                    cardPivotRect.pivot = new Vector2(0.5f, 0f);
                    cardPivotRect.anchorMax = new Vector2(0.5f, 0f);
                    cardPivotRect.anchorMin = new Vector2(0.5f, 0f);
                    break;
            }

            cardPivotRect.anchoredPosition = Vector2.zero;
            mSelectedTwistPivotType = pivotType;
        }

        public void TwistCardAngles(Vector3 eulerAngles)
        {
            switch (mSelectedTwistPivotType)
            {
                case ETwistPivotTypes.Top:
                    cardPivotRect.eulerAngles = eulerAngles;
                    break;
                case ETwistPivotTypes.Center:
                    cardPivotRect.eulerAngles = eulerAngles;
                    break;
                case ETwistPivotTypes.Bottom:
                    cardPivotRect.eulerAngles = -eulerAngles;
                    break;
            }
        }

        public void SetCardEulerAngles(Vector3 eulerAngles)
        {
            cardPivotRect.eulerAngles = eulerAngles;
        }

        private void SetProductImage()
        {
            SourceType sourceType = mRegisteredProductInfo.SourceType;
            productImage.texture = sourceType == SourceType.User ? mRegisteredProductInfo.GetVgaTexture() : mRegisteredProductInfo.ProductImage;
        }

        private void SetProductNameText()
        {
            if(mRegisteredProductInfo.SourceType == SourceType.Sample)
            {
                productNameText.text = mUserRegisteredProductInfo.uid;
                return;
            }
            productNameText.text = mProductData.productName;
        }

        private void SetCategoryNameText()
        {
            if(mRegisteredProductInfo.SourceType == SourceType.Sample)
            {
                categoryNameText.text = "샘플 카테고리";
                return;
            }
            categoryNameText.text = mProductData.category;
        }

        private void SetManufacturerNameText()
        {
            if(mRegisteredProductInfo.SourceType == SourceType.Sample)
            {
                manufacturerNameText.text = "샘플 제조사";
                return;
            }
            manufacturerNameText.text = mProductData.manufacturer;
        }

        private void SetDescriptionText()
        {
            if (mRegisteredProductInfo.SourceType == SourceType.Sample)
            {
                descriptionText.text = "샘플 설명";
                return;
            }
            descriptionText.text = mUserRegisteredProductInfo.Description;
        }

        private void SetPriceText()
        {
            if (mRegisteredProductInfo.SourceType == SourceType.Sample)
            {
                priceText.text = 10000.ToString("N0");
                return;
            }

            priceText.text = mUserRegisteredProductInfo.Price.ToString("N0");
        }

        public void DestoryProductCard()
        {
            mRegisteredProductInfo.DestroyInstance();
        }

        public void SaveCardToAlbum()
        {
            int renderWidth = (int)(Camera.main.pixelWidth * ScreenUtil.scaleRatio);
            int renderHeight = (int)(Camera.main.pixelHeight * ScreenUtil.scaleRatio);

            int cardWidth = (int)mRectTransform.sizeDelta.x - 14;
            int cardHeight = (int)mRectTransform.sizeDelta.y - 12;

            int xOffset = (renderWidth - cardWidth) / 2;
            int yOffset = (renderHeight - cardHeight) / 2;

            RenderTexture renderTexture = new RenderTexture(renderWidth, renderHeight, 24);
            Texture2D texture2D = new Texture2D(cardWidth, cardHeight, TextureFormat.ARGB32, false);

            Camera.main.targetTexture = renderTexture;
            Camera.main.Render();

            Vector2 cardScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
            yOffset -= (renderHeight - (int)cardScreenPos.y * 2) / 2;

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(xOffset, yOffset, mRectTransform.sizeDelta.x, mRectTransform.sizeDelta.y), 0, 0);
            texture2D.Apply();

            // byte[] bytes = texture2D.EncodeToPNG();
            NativeGallery.SaveImageToGallery(texture2D, "Munch", $"{mUserRegisteredProductInfo.uid}.png", CallbackSaveCard);

            Camera.main.targetTexture = null;
        }

        private void CallbackSaveCard(bool success, string path)
        {
            if(success)
            {
                Utils.InvokeAction(SAVE_CARD_ACTION);
                Debug.Log(path);
            }
        }
    }
}