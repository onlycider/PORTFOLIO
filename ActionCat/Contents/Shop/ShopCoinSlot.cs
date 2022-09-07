using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCoinSlot : ShopProductSlot
{
    public Text productQuantity;
    public Text productPrice;
    public Image productImage;

    public override void SetShopProductData(ShopProductData _productData)
    {
        base.SetShopProductData(_productData);

        SetProductQuantity();
        SetProductPrice();
    }

    private void SetProductQuantity()
    {
        productQuantity.text = m_shopProductData.productQuantity.ToString();
    }

    private void SetProductPrice()
    {
        productPrice.text = m_shopProductData.price.ToString();
    }

    protected override void ClickFunction()
    {
        base.ClickFunction();
        Info_User userInfo = CM_Singleton<GameData>.instance.m_Info_User;
        int productPriceValue = m_shopProductData.price;
        int productQuantity = m_shopProductData.productQuantity;

        if(userInfo.m_totalGem < productPriceValue)
        {
            string title = CM_Singleton<Table_Text>.instance.GetText("NoticePopup_Msg_8");
            string message = string.Format(CM_Singleton<Table_Text>.instance.GetText("NoticePopup_Msg_10"), productQuantity, productPriceValue);
            CM_Singleton<UIManager>.instance.CreateNoticeMsg(CanvasDepth.Popups, title, message);
            return;
        }

        string titleName = CM_Singleton<Table_Text>.instance.GetText("Charge_Gold");
        float iconScaleMag = 0.5f;
        UIManager.instance.CreateCommonBuyInfoPopup(titleName, productImage, m_shopProductData.productType, productPriceValue, BuyShopCoinProduct, iconScaleMag);
    }

    private void BuyShopCoinProduct()
    {
        WebServerAPIs.Instance.BuyShopProduct(m_shopProductData.index, CallbackBuyShopProduct);
    }

    private void CallbackBuyShopProduct(APIContents contents)
    {
        Dictionary<string, object> updateUserInfo = null;
        if(contents.contents.ContainsKey("user"))
        {
            updateUserInfo = contents.contents["user"] as Dictionary<string, object>;
            CM_Singleton<GameData>.instance.m_Info_User.UpdateUserInfo(updateUserInfo, gameObject);
        }
    }
}
