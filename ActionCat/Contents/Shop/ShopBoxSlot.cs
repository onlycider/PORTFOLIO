using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBoxSlot : ShopProductSlot
{
    public Text productName;
    public Text productDescription;
    public Text productPrice;
    public Image productImage;

    public GameObject freeRemainTime;

    private bool m_enableFreeRewards;

    public Text remainTimeText;

    public GameObject priceInfo;
    public GameObject freeAdsInfo;

    private AdsUnit m_shopAdsUnit;

    public override void SetShopProductData(ShopProductData _productData)
    {
        base.SetShopProductData(_productData);
        m_enableFreeRewards = false;
        SetProductPrice();
        SetProductDescription();
        SetProductName();
        SetFreeAdsInfo();
        SetFreeAdsSlot();
    }

    private void SetProductPrice()
    {
        productPrice.text = m_shopProductData.price.ToString();
    }

    private void SetProductDescription()
    {
        string descIndex = Utils.GetItemDescIndex(m_shopProductData.productId);
        productDescription.text = CM_Singleton<Table_Text>.instance.GetText(descIndex);
    }

    private void SetProductName()
    {
        string nameIndex = Utils.GetItemNameIndex(m_shopProductData.productId);
        productName.text = CM_Singleton<Table_Text>.instance.GetText(nameIndex);
    }

    private void SetFreeAdsInfo()
    {
        int rewardIndex = m_shopProductData.productId;
        m_shopAdsUnit = GameDatabase.Instance.adsRewards.GetShopAds(rewardIndex);
        
    }

    private void SetFreeAdsSlot()
    {
        if (m_shopAdsUnit == null)
            return;

        DateTime rewardEnableTime = m_shopAdsUnit.relieveShowAdTime;
        if (ServerSyncTime.now > rewardEnableTime)
        {
            m_enableFreeRewards = true;
            freeRemainTime.SetActive(false);
            freeAdsInfo.SetActive(true);
            // priceInfo.SetActive(false);
        }
        else
        {
            m_enableFreeRewards = false;
            freeRemainTime.SetActive(true);
            freeAdsInfo.SetActive(false);
            // priceInfo.SetActive(true);
            StartCoroutine(CheckingFreeRemainTime());
        }
    }

    private IEnumerator CheckingFreeRemainTime()
    {
        TimeSpan remainTime = m_shopAdsUnit.relieveShowAdTime - ServerSyncTime.now;
        while(remainTime > TimeSpan.Zero)
        {
            remainTimeText.text = Utils.GetRemainTime(remainTime);
            yield return YieldHelper.waitForSeconds(1000);
            remainTime = m_shopAdsUnit.relieveShowAdTime - ServerSyncTime.now;
        }
        SetFreeAdsSlot();
    }

    public void BoxDetailInfoClick()
    {
        if(CM_Singleton<View_Shop>.existence == null)
            return;
        
        BoxGachaInfo gachaInfo = CM_Singleton<View_Shop>.instance.GetCachaInfo(m_shopProductData.productId);
        if( gachaInfo == null)
            return;
        
        UIManager.instance.OnOpenPopup<Panel_BoxRateDetail>(UIPanels.Panel_BoxRateDetail, gachaInfo, CanvasDepth.Popups);
    }





    protected override void ClickFunction()
    {
        base.ClickFunction();

        Info_User userInfo = CM_Singleton<GameData>.instance.m_Info_User;
        int productPriceValue = m_shopProductData.price;
        int haveMyValue;
        string chargeTitle;
        string chargeMsg;

        string useTitle;
        string useMsg;

        if(m_shopProductData.productType == PriceType.Gold )
        {
            haveMyValue = userInfo.m_Gold;
            chargeTitle = CM_Singleton<Table_Text>.instance.GetText("Popup_GoldShortage_Title");
            chargeMsg = CM_Singleton<Table_Text>.instance.GetText("Popup_GoldShortage_Msg");
            useTitle = CM_Singleton<Table_Text>.instance.GetText("Confirm_NativeShop_Title");
            useMsg = CM_Singleton<Table_Text>.instance.GetText("Confirm_NativeShop_Msg");
        }
        else
        {
            haveMyValue = userInfo.m_totalGem;
            chargeTitle = CM_Singleton<Table_Text>.instance.GetText("NoticePopup_Msg_8");
            chargeMsg = string.Format(CM_Singleton<Table_Text>.instance.GetText("NoticePopup_Msg_9"), productPriceValue);
            useTitle = CM_Singleton<Table_Text>.instance.GetText("NoticePopup_Msg_6");
            useMsg = string.Format(CM_Singleton<Table_Text>.instance.GetText("NoticePopup_Msg_7"), productPriceValue);
        }

        if(haveMyValue < productPriceValue)
        {
            CM_Singleton<UIManager>.instance.CreateNoticeMsg(CanvasDepth.Popups, chargeTitle, chargeMsg);
            return;
        }

        string titleName = CM_Singleton<Table_Text>.instance.GetText("Item_Purchase");
        UIManager.instance.CreateCommonBuyInfoPopup(titleName, productImage, m_shopProductData.productType, productPriceValue, BuyShopBoxProduct);
    }

    public void AdBtnClick()
    {
        //무료 광고 뽑기가 있다면 광고 시청
        if(m_enableFreeRewards)
        {
            FirebaseLauncher.instance.LogEvent("ad_click_shop", "ProductId", m_shopProductData.index);
            AdsManager.Instance.ShowAds(m_shopAdsUnit.resetIndex, CallbackShownAds);
            return;
        }
    }

    private void BuyShopBoxProduct()
    {
        WebServerAPIs.Instance.BuyShopProduct(m_shopProductData.index, CallbackBuyShopProduct);
    }

    private void CallbackShownAds()
    {
        WebServerAPIs.Instance.RewardAdsView(m_shopAdsUnit.resetIndex, CallbackAdsRewardView);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="contents"></param>
    private void CallbackAdsRewardView(APIContents contents)
    {
        Dictionary<string, object> boxRewards = contents.contents["box_reward"] as Dictionary<string, object>;
        Dictionary<string, object>[] updateUserItemInfo = contents.contents["user_item"] as Dictionary<string, object>[];
        Dictionary<string, object> updateUserInfo = null;
        if(contents.contents.ContainsKey("user"))
            updateUserInfo = contents.contents["user"] as Dictionary<string, object>;

        CM_Singleton<GameData>.instance.m_Info_User.InventorySetting(updateUserItemInfo);
        
        BoxRewardView rewardView = UIManager.instance.OnOpenPopup<BoxRewardView>(UIPanels.BoxRewardView, boxRewards, CanvasDepth.Popups);
        if(updateUserInfo != null)
            rewardView.SetUpdatedUserInfo(updateUserInfo);
        Dictionary<string, object>[] userAdsInfo = contents.contents["user_ad_info"] as Dictionary<string, object>[];
        AdsManager.Instance.SetUserAdUnitInfos(userAdsInfo);

        SetFreeAdsSlot();
    }

    private void CallbackBuyShopProduct(APIContents contents)
    {
        Dictionary<string, object> shopRewards = contents.contents["shop_buy_reward"] as Dictionary<string, object>;
        Dictionary<string, object> boxReward = shopRewards["box_reward"] as Dictionary<string, object>;
        Dictionary<string, object>[] updateUserItemInfo = contents.contents["user_item"] as Dictionary<string, object>[];
        Dictionary<string, object> updateUserInfo = null;
        if(contents.contents.ContainsKey("user"))
            updateUserInfo = contents.contents["user"] as Dictionary<string, object>;

        CM_Singleton<GameData>.instance.m_Info_User.InventorySetting(updateUserItemInfo);
        
        BoxRewardView rewardView = UIManager.instance.OnOpenPopup<BoxRewardView>(UIPanels.BoxRewardView, boxReward, CanvasDepth.Popups);
        if(updateUserInfo != null)
            rewardView.SetUpdatedUserInfo(updateUserInfo);
        // int boxIndex = m_shopProductData.productId;
    }
}
