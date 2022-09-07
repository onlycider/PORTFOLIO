using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Purchasing;

public class ShopGemSlot : ShopProductSlot
{
    public Text productQuantity;
    public Text productPrice;

    private ShopProductData m_productData;

    public override void SetShopProductData(ShopProductData _productData)
    {
        base.SetShopProductData(_productData);
        m_productData = _productData;
        SetProductQuantity();
        SetProductPrice();
    }

    private void SetProductQuantity()
    {
        productQuantity.text = m_shopProductData.productQuantity.ToString();
    }

    private void SetProductPrice()
    {
        productPrice.text = IAPManager.instance.GetLocalizePrice(m_productData.inappIdentifier);
    }

    protected override void ClickFunction()
    {
        base.ClickFunction();
        if(m_shopProductData.inappIdentifier == "none")
            return;

        Debug.Log(m_shopProductData.inappIdentifier);
        IAPManager.instance.BuyProduct(m_shopProductData.inappIdentifier, CallbackBuyShopProduct);
    }

    private void CallbackBuyShopProduct(APIContents contents)
    {
        Dictionary<string, object> productInfo = contents.contents["product_info"] as Dictionary<string, object>;
        Dictionary<string, object> userData = productInfo["user"] as Dictionary<string, object>;
        CM_Singleton<GameData>.instance.m_Info_User.UpdateUserInfo(userData, gameObject);

        View_Shop shopView = CM_Singleton<View_Shop>.instance;
        if(shopView != null)
        {
            shopView.SetAdsRemoveButton();
        }
        // CM_Singleton<View_Shop>.instance.SetAdsRemoveButton();
    }
}
