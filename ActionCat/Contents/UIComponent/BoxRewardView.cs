using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Spine;

public class BoxRewardData
{
    private int m_boxId;
    public int boxId{get{return m_boxId;}}

    private List<CommonRewardItem> m_boxRewards;
    public List<CommonRewardItem> boxRewards{get{return m_boxRewards;}}

    private List<CommonRewardItem> m_goodsRewards;
    public List<CommonRewardItem> goodsRewards{get{return m_goodsRewards;}}
    
    public BoxRewardData(Dictionary<string, object> data)
    {
        m_boxId = (int)data["box_id"];
        m_boxRewards = new List<CommonRewardItem>();
        m_goodsRewards = new List<CommonRewardItem>();

        if( data.ContainsKey("gold") )
        {
            int goldQuantity = (int)data["gold"];
            CommonRewardItem goldReward = new CommonRewardItem(0, PriceType.Gold, goldQuantity);
            m_goodsRewards.Add(goldReward);
        }

        Dictionary<string, object>[] gachaResults1 = data["select_type1_item_id"] as Dictionary<string, object>[];
        foreach(Dictionary<string, object> gachaResult1 in gachaResults1)
        {
            int rewardIndex = (int)gachaResult1["item_id"];
            string rewardType = (string)gachaResult1["item_type"];
            int rewardQuantity = (int)gachaResult1["count"];
            CommonRewardItem gachaReward = new CommonRewardItem(rewardIndex, rewardType, rewardQuantity);
            m_boxRewards.Add(gachaReward);
        }


        if(data.ContainsKey("select_type2_item_id"))
        {
            Dictionary<string, object>[] gachaResults2 = data["select_type2_item_id"] as Dictionary<string, object>[];
            foreach(Dictionary<string, object> gachaResult2 in gachaResults2)
            {
                int rewardIndex = (int)gachaResult2["item_id"];
                string rewardType = (string)gachaResult2["item_type"];
                int rewardQuantity = (int)gachaResult2["count"];
                CommonRewardItem gachaReward = new CommonRewardItem(rewardIndex, rewardType, rewardQuantity);
                m_boxRewards.Add(gachaReward);
            }
        }

        if(data.ContainsKey("stone_item_id"))
        {
            int stoneRewardIndex = (int)data["stone_item_id"];
            int stoneRewardQuantity = (int)data["stone_count"];
            PriceType itemType = PriceType.Item;

            CommonRewardItem stoneReward = new CommonRewardItem(stoneRewardIndex, itemType, stoneRewardQuantity);
            m_goodsRewards.Add(stoneReward);
        }

        if(data.ContainsKey("stamina"))
        {
            int staminaCount = (int)data["stamina"];
            PriceType staminaType = PriceType.Stamina;

            CommonRewardItem staminaReward = new CommonRewardItem(0, staminaType, staminaCount);
            m_goodsRewards.Add(staminaReward);
        }
    }   
}

public class BoxRewardView : PopupBone
{
    public SkeletonAnimation boxAnimation;

    private BoxRewardData m_boxRewards;

    public GameObject premiumBoxIdleEffect;
    public GameObject[] openEffects;
    public GameObject endEffect;

    public GameObject rewardResultsObject;

    public RewardResultSlot resultSlotObject;

    private List<RewardResultSlot> m_resultSlots;

    private List<RewardResultSlot> m_bonusGoodsSlots;

    public Transform slotParent;

    public Transform goodsParent;

    public GameObject bonusText;

    public GameObject pressCloseText;

    private bool m_premium = false;

    private bool m_complete = false;

    private bool m_animationPlaying = false;


    public Button btn_OK;
    public Button btn_ReBuy;
    public Image img_reBuyType;
    public Text label_reBuyPrice;
    ShopProductData boxPrice;
    public bool bNoRebuy = false;

    private Dictionary<string, object> m_userData;

    public override void StartInfoSetting(object info)
    {
        base.StartInfoSetting(info);
        Dictionary<string, object> boxRewards = info as Dictionary<string, object>;

        if(boxRewards != null)
            m_boxRewards = new BoxRewardData(boxRewards);

        // OpenSetting();
        m_complete = false;
    }

    public void SetUpdatedUserInfo(Dictionary<string, object> _userData)
    {
        m_userData = _userData;
    }

    public override void OpenSetting()
    {
        // 박스 화면에서는 백버튼이 안먹어도 됨
        boxPrice= GameDatabase.Instance.shopProducts.GetProductDataByBox(m_boxRewards.boxId);
        SettingAnimation();
        ReTryBtnSetting();
    }

    private void SettingAnimation()
    {
        // Debug.Log("m_boxRewards.count :: " + m_boxRewards.boxRewards.Count);
        string skinName = string.Empty;
        switch(m_boxRewards.boxId)
        {
            case 901:
            skinName = "3_2";
            m_premium = false;
            break;
            case 902:
            skinName = "3_1";
            m_premium = true;
            break;
            case 903:
            skinName = "2_2";
            m_premium = false;
            break;
            case 904:
            skinName = "2_1";
            m_premium = true;
            break;
            case 905:
            skinName = "1_2";
            m_premium = false;
            break;
            case 906:
            skinName = "1_1";
            m_premium = true;
            break;
        }
        boxAnimation.skeleton.SetSkin(skinName);
        boxAnimation.AnimationState.Start += HandleAnimationStateStartEvent;
        boxAnimation.AnimationState.Event += HandleAnimationStateEvents;
        boxAnimation.AnimationState.End += HandleAniamtionStateEndEvent;
        boxAnimation.AnimationState.SetAnimation(0, "idle", true);
        SetRewardSlots();
    }

    private void HandleAnimationStateStartEvent(TrackEntry trackEntry)
    {
        // Debug.Log("animation start :: " + trackEntry.Animation.Name);
        if(trackEntry.Animation.Name == "idle" && m_premium)
            premiumBoxIdleEffect.SetActive(true);

        if(trackEntry.Animation.Name == "opened")
            endEffect.SetActive(true);
    }

    private void HandleAnimationStateEvents(TrackEntry trackEntry, Spine.Event e)
    {
        if(trackEntry.Animation.Name == "open" && e.Data.Name == "open")
        {
            foreach(GameObject effect in openEffects)
            {
                effect.SetActive(true);
            }
        }
    }

    private void HandleAniamtionStateEndEvent(TrackEntry trackEntry)
    {
        // Debug.Log("animation end :: " + trackEntry.Animation.Name);
        if(trackEntry.Animation.Name == "open")
        {
            DrawRewardResult();
        }
    }

    private void SetRewardSlots()
    {
        m_resultSlots = new List<RewardResultSlot>();
        List<CommonRewardItem> rewardInfos = m_boxRewards.boxRewards;
        for(int i = 0; i < rewardInfos.Count; i++)
        {
            if(i == 0)
            {
                resultSlotObject.SetCommonRewardSlot(rewardInfos[i]);
                m_resultSlots.Add(resultSlotObject);
                continue;
            }
            RewardResultSlot slot = Utils.GetCreatedObjectComponent<RewardResultSlot>(resultSlotObject.gameObject, slotParent);
            slot.SetCommonRewardSlot(rewardInfos[i]);
            m_resultSlots.Add(slot);
        }

        m_bonusGoodsSlots = new List<RewardResultSlot>();
        List<CommonRewardItem> goodsRewards = m_boxRewards.goodsRewards;
        foreach(CommonRewardItem bonusGoods in goodsRewards)
        {
            RewardResultSlot slot = Utils.GetCreatedObjectComponent<RewardResultSlot>(resultSlotObject.gameObject, goodsParent);
            slot.SetCommonRewardSlot(bonusGoods);
            m_bonusGoodsSlots.Add(slot);
        }
    }

    private void DrawRewardResult()
    {
        boxAnimation.gameObject.SetActive(false);
        rewardResultsObject.SetActive(true);
        StartCoroutine(DelayActiveObject());
    }

    private IEnumerator DelayActiveObject()
    {
        foreach(RewardResultSlot slot in m_resultSlots)
        {
            SoundManager.PlaySFX("Special & Powerup (6)");
            slot.gameObject.SetActive(true);
            yield return YieldHelper.waitForSeconds(200);
        }
        bonusText.SetActive(true);
        yield return YieldHelper.waitForSeconds(200);
        
        Info_User userInfo = CM_Singleton<GameData>.instance.m_Info_User;
        foreach(RewardResultSlot slot in m_bonusGoodsSlots)
        {
            SoundManager.PlaySFX("Special & Powerup (6)");
            slot.gameObject.SetActive(true);
            yield return YieldHelper.waitForSeconds(200);
        }
        // pressCloseText.SetActive(true);
        btn_OK.gameObject.SetActive(true);
        if(boxPrice != null && bNoRebuy==false )
            btn_ReBuy.gameObject.SetActive(true);
        m_complete = true;

        m_animationPlaying = false;
    }

    public void OnClickClose()
    {
        if(m_complete)
        {
            CloseClick();
            PlayUpdateUserInfo();
            return;
        }
    }

    bool bRetryClick = false;
    public void OnClickReTry()
    {
        if(bRetryClick)
            return;

        bRetryClick = true;
        PlayUpdateUserInfo();
        RetryCheck();
    }
    private void RetryCheck()
    {
        Info_User userInfo = CM_Singleton<GameData>.instance.m_Info_User;
        int productPriceValue = boxPrice.price;
        int haveMyValue;
        string chargeTitle;
        string chargeMsg;

        string useTitle;
        string useMsg;

        if(boxPrice.productType == PriceType.Gold )
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

        Debug.Log(haveMyValue +"::"+ productPriceValue);
        if(haveMyValue < productPriceValue)
        {
            CM_Singleton<UIManager>.instance.CreateNoticeMsg(CanvasDepth.Popups, chargeTitle, chargeMsg);
            bRetryClick = false;
            return;
        }
        WebServerAPIs.Instance.BuyShopProduct(boxPrice.index, CallbackBuyShopProduct);
    }

    private void CallbackBuyShopProduct(APIContents contents)
    {
        CloseClick();
        PlayUpdateUserInfo();

        Dictionary<string, object> boxRewards = contents.contents["shop_buy_reward"] as Dictionary<string, object>;
        Dictionary<string, object>[] updateUserItemInfo = contents.contents["user_item"] as Dictionary<string, object>[];
        Dictionary<string, object> updateUserInfo = null;
        if(contents.contents.ContainsKey("user"))
            updateUserInfo = contents.contents["user"] as Dictionary<string, object>;

        CM_Singleton<GameData>.instance.m_Info_User.InventorySetting(updateUserItemInfo);
        
        BoxRewardView rewardView = UIManager.instance.OnOpenPopup<BoxRewardView>(UIPanels.BoxRewardView, boxRewards["box_reward"], CanvasDepth.Popups);
        if(updateUserInfo != null)
            rewardView.SetUpdatedUserInfo(updateUserInfo);
        // int boxIndex = m_shopProductData.productId;
    }






    public void OnClickBoxopen()
    {
        if(m_animationPlaying)
            return;
            
        if(m_complete)
        {
            // CloseClick();
            // PlayUpdateUserInfo();
            return;
        }
        SoundManager.PlaySFX("fx_u_0011 Audio Extracted_1");
        m_animationPlaying = true;
        boxAnimation.AnimationState.SetAnimation(0, "open", false);
        boxAnimation.AnimationState.AddAnimation(0, "opened", false, 0f);
    }




    private void PlayUpdateUserInfo()
    {
        if(m_userData == null)
            return;
        
        Info_User userInfo = CM_Singleton<GameData>.instance.m_Info_User;
        userInfo.UpdateUserInfo(m_userData);
    }


    override public void CloseClick()
    {
        base.CloseClick();
        Destroy(gameObject);
    }








    public void ReTryBtnSetting()
    {
        if(boxPrice != null)
        {
            if(boxPrice.productType == PriceType.BuyGem ||boxPrice.productType == PriceType.FreeGem)
                img_reBuyType.sprite = CM_Singleton<Mng_Atlas>.instance.GetSprite(AtlasName.ItemIcon, "item_gem");
            else if(boxPrice.productType == PriceType.Mileage)
                img_reBuyType.sprite = CM_Singleton<Mng_Atlas>.instance.GetSprite(AtlasName.ItemIcon, "item_mileage");
            else
                img_reBuyType.sprite = CM_Singleton<Mng_Atlas>.instance.GetSprite(AtlasName.ItemIcon, "item_gold");

            label_reBuyPrice.text = string.Format("{0:N0}",boxPrice.price );
        }
     
    }
}
