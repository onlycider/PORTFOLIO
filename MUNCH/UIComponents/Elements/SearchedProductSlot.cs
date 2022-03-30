using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MunchProject
{
    public class SearchedProductSlot : MonoBehaviour
    {
        private RegisteredProductInfo mRegisteredProductInfo;

        private RawImage mProductImage;

        public TextMeshProUGUI productNameText;

        public TextMeshProUGUI priceText;

        public void SetRegisteredProductInfo(RegisteredProductInfo info)
        {
            mRegisteredProductInfo = info;
            mProductImage = GetComponent<RawImage>();
            SetProductImage();

            productNameText.gameObject.SetActive(false);
            priceText.gameObject.SetActive(false);
            // SetProductName();
            // SetPriceText();
        }

        private void SetProductImage()
        {
            SourceType sourceType = mRegisteredProductInfo.SourceType;
            mProductImage.texture = sourceType == SourceType.User ? mRegisteredProductInfo.GetSquareTexture() : mRegisteredProductInfo.ProductImage;
        }

        private void SetProductName()
        {
            if (mRegisteredProductInfo.ProductData == null)
            {
                return;
            }
            productNameText.text = mRegisteredProductInfo.ProductData.productName;
        }

        private void SetPriceText()
        {
            if (mRegisteredProductInfo.UserRegisteredProductData == null)
            {
                return;
            }

            priceText.text = mRegisteredProductInfo.UserRegisteredProductData.Price.ToString("N0");
        }

        public void OnClickProductSlot()
        {
            if (mRegisteredProductInfo == null)
            {
                return;
            }

            PanelManager.Instance.AddPanel<ProductInformationView>(PanelTag.ProductInformationView, mRegisteredProductInfo);
        }

        public void InactivateSlot()
        {
            gameObject.SetActive(false);
        }
    }
}