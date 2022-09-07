using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UserBattlePassSeason
{
    private int m_uid;
    public int uid{get{return m_uid;}}

    private bool m_purchasedSeasonPass;
    public bool purchasedSeasonPass{get{return m_purchasedSeasonPass;}}

    public UserBattlePassSeason(Dictionary<string, object> data)
    {

    }

}

public class BattlePassView : PopupBone
{
    public BattlePassSeasonData m_battlePassSeasonData;
    private BattlePassRewards m_battlePassRewards;
    private ShopProductData m_buyBattlePassPointData;

    public PassPointRewards pointRewardsObject;

    public RectTransform rewardsParent;

    public Text seasonText;
    public Text remainTimeText;

    public Text passPointText;

    private Info_User m_userInfo;

    private Dictionary<int, UserPassPointRewardInfo> m_userPassPointRewards;
    private UserBattlePassSeason m_userBattlePassSeasonInfo;

    public UIGauge pointValueGauge;

    public RectTransform scrollviewPort;
    public RectTransform scrollContent;

    private bool m_purchasedPass;

    public UGUI_MultiImgButton purchaseButton;
    public GameObject battlePassRewardInfo;

    private Dictionary<int, PassPointRewards> m_pointRewards;


    public Image benefitItemIcon;

    public Text bonusItemCount;
    public Text pointValueInfo;
    public GameObject go_BonusLock;

    public override void StartInfoSetting(object info)
    {
        base.StartInfoSetting(info);
        // m_purchasedPass = CM_Singleton<GameData>.instance.m_Info_User.bBattlePassPurchased
        Dictionary<string, object> userPassInfo = info as Dictionary<string, object>;
        Dictionary<string, object>[] passRewardsInfos = userPassInfo["user_battle_pass"] as Dictionary<string,object>[];
        m_userPassPointRewards = new Dictionary<int, UserPassPointRewardInfo>();
        if(passRewardsInfos != null && passRewardsInfos.Length > 0)
        {
            foreach(Dictionary<string, object> passRewardsInfo in passRewardsInfos)
            {
                UserPassPointRewardInfo userPassRewardsInfo = new UserPassPointRewardInfo(passRewardsInfo);
                m_userPassPointRewards.Add(userPassRewardsInfo.step, userPassRewardsInfo);
            }
        }

        if(userPassInfo.ContainsKey("user_battle_pass_season") == false)
        {
            m_userBattlePassSeasonInfo = null;
            CM_Singleton<GameData>.instance.m_Info_User.bBattlePassPurchased = false;
            m_purchasedPass = false;
            return;
        }

        Dictionary<string, object> passSeasonInfo = userPassInfo["user_battle_pass_season"] as Dictionary<string, object>;
        if(passSeasonInfo != null && passSeasonInfo.Count > 0)
        {
            m_userBattlePassSeasonInfo = new UserBattlePassSeason(passSeasonInfo);
            CM_Singleton<GameData>.instance.m_Info_User.bBattlePassPurchased = true;
            m_purchasedPass = true;
            purchaseButton.gameObject.SetActive(false);
            battlePassRewardInfo.SetActive(true);
        }
    }
    
    public override void OpenSetting()
    {
        SoundManager.PlaySFX("Sci-Fi Click");
        CM_Singleton<GameData>.instance.m_Mng_BackKey.EnrollBackKey(gameObject, CloseClick);
        m_battlePassSeasonData = GameDatabase.Instance.m_battlePassSeason.GetCurrentSeasonData();
        m_battlePassRewards = GameDatabase.Instance.m_battlePassRewards;
        m_userInfo = CM_Singleton<GameData>.instance.m_Info_User;
        SetSeasonText();
        // SetSeasonEndText();
        SetBattlePassPoint();
        DrawPointRewards();
        SetBonusBattlePass();
        CheckSeasonEndTime();

        StartCoroutine(FirstFocusing());
    }

    private void SetSeasonText()
    {
        string seasonTextFormat = CM_Singleton<Table_Text>.instance.GetText("BattlePass_Season");
        seasonText.text = string.Format(seasonTextFormat, m_battlePassSeasonData.season);
    }

    private void CheckSeasonEndTime()
    {
        StartCoroutine(EndTimeChecker());
    }

    private IEnumerator EndTimeChecker()
    {
        TimeSpan remainTimeSpan = m_battlePassSeasonData.endDate - ServerSyncTime.now;
        while(remainTimeSpan > TimeSpan.Zero)
        {
            SetSeasonEndText();
            yield return YieldHelper.waitForSeconds(1000);
            remainTimeSpan = m_battlePassSeasonData.endDate - ServerSyncTime.now;
        }

        Table_Text textLibrary = CM_Singleton<GameData>.instance.GetTextTable();
        string title = textLibrary.GetText("BattlePass_Season_Wait_Title");
        string message = textLibrary.GetText("BattlePass_Season_Wait_Desc");
        UIManager uimanager = CM_Singleton<UIManager>.instance;
        uimanager.CreateNoticeMsg(title, message, "", ()=>{Destroy(gameObject);});
    }

    private void SetSeasonEndText()
    {
        string seasonEndMessage = CM_Singleton<Table_Text>.instance.GetText("BattlePass_SeasonEnd");
        TimeSpan remainTimeSpan = m_battlePassSeasonData.endDate - DateTime.Now;
        string remainTime = Utils.GetRemainTime(remainTimeSpan);
        remainTimeText.text = Utils.GetAppendedSentence(remainTime, seasonEndMessage); 
        // string remainTime;
    }

    private void SetBattlePassPoint()
    {
        passPointText.text = m_userInfo.m_battlePassPoint.ToString();
    }

    private void SetBonusBattlePass()
    {
        Mng_Atlas atlasManager = CM_Singleton<Mng_Atlas>.instance;
        string spriteName = Utils.GetItemSpriteName(m_battlePassSeasonData.bonusReward);
        benefitItemIcon.sprite = atlasManager.GetSprite(AtlasName.ItemIcon, spriteName);
        bonusItemCount.text = $"x{m_battlePassSeasonData.bonusReward.rewardQuantity}";
        pointValueInfo.text = $"({m_userInfo.m_battlePassPoint}/{m_battlePassSeasonData.bonusMaxPoint})";

        go_BonusLock.SetActive( !m_purchasedPass );
    }

    private void DrawPointRewards()
    {
        m_pointRewards = new Dictionary<int, PassPointRewards>();
        Dictionary<int, BattlePassRewardData> rewards = m_battlePassRewards.battlePassRewards;
        int index = 0;
        int step;
        int point = m_userInfo.m_battlePassPoint;
        int preSectorPoint = 0;
        int sectorIndex = rewards.Count;
        int sectorSize = 0;
        int remainPoint = 0;
        int selectedStep = 0;
        foreach(BattlePassRewardData rewardData in rewards.Values)
        {
            step = index + 1;
            UserPassPointRewardInfo userRewardInfo = null;
            if(m_userPassPointRewards.ContainsKey(step))
                userRewardInfo = m_userPassPointRewards[step];


            if(point >= preSectorPoint && point <= rewardData.needPoint)
            {
                sectorIndex = index;
                remainPoint = point - preSectorPoint;
                sectorSize = rewardData.needPoint - preSectorPoint;
                selectedStep = step;
            }


            preSectorPoint = rewardData.needPoint;

            if(index == 0)
            {
                pointRewardsObject.SetPassPointRewards(rewardData, userRewardInfo);
                pointRewardsObject.transform.SetParent(rewardsParent);
                pointRewardsObject.SetPurchasedPass(m_purchasedPass);
                pointRewardsObject.SetRewardEnable(point);
                m_pointRewards.Add(step, pointRewardsObject);
                index++;
                continue;
            }
            PassPointRewards passPointRewards = Utils.GetCreatedObjectComponent<PassPointRewards>(pointRewardsObject.gameObject, rewardsParent);
            passPointRewards.SetPassPointRewards(rewardData, userRewardInfo);
            passPointRewards.SetPurchasedPass(m_purchasedPass);
            passPointRewards.SetRewardEnable(point);
            m_pointRewards.Add(step, passPointRewards);
            index++;
        }
        float slotWidth = pointRewardsObject.rectTransform.rect.width;
        HorizontalLayoutGroup layoutGroup = rewardsParent.GetComponent<HorizontalLayoutGroup>();
        

        float gaugeMaxWidth = (slotWidth * rewards.Count) + (layoutGroup.spacing * (rewards.Count - 1)) + layoutGroup.padding.left;
        pointValueGauge.InitializeGauge(gaugeMaxWidth);

        float spacing = slotWidth + layoutGroup.spacing;
        float firstSector = (spacing / 2) + 10f;
        firstSector = Mathf.Ceil(firstSector);

        float firstSectorPoint = 1f;
        float addValue = 0f;
        if(sectorIndex < 1)
            firstSectorPoint = (float)remainPoint / (float)sectorSize;
        else if(sectorIndex < rewards.Count)
        {
            addValue = ((spacing) * ((float)remainPoint / (float)sectorSize));
            sectorIndex -= 1;
        }
        else
        {
            remainPoint = point - preSectorPoint;
            addValue = (float)remainPoint;
        }

        float gaugeWidth = ((spacing) * sectorIndex) + addValue + (firstSector * firstSectorPoint);
        pointValueGauge.SetGaugeWidth(gaugeWidth);
    }
    IEnumerator CoRefresh()
    {
        yield return YieldHelper.waitForSeconds(200);

        Dictionary<int, BattlePassRewardData> rewards = m_battlePassRewards.battlePassRewards;
        int index = 0;
        int step;
        int point = m_userInfo.m_battlePassPoint;
        int preSectorPoint = 0;
        int sectorIndex = rewards.Count;
        int sectorSize = 0;
        int remainPoint = 0;
        int selectedStep = 0;

        foreach(BattlePassRewardData rewardData in rewards.Values)
        {
            step = index + 1;
            UserPassPointRewardInfo userRewardInfo = null;
            if(m_userPassPointRewards.ContainsKey(step))
                userRewardInfo = m_userPassPointRewards[step];


            if(point >= preSectorPoint && point <= rewardData.needPoint)
            {
                sectorIndex = index;
                remainPoint = point - preSectorPoint;
                sectorSize = rewardData.needPoint - preSectorPoint;
                selectedStep = step;
            }


            preSectorPoint = rewardData.needPoint;

            if(index == 0)
            {
                index++;
                continue;
            }
            index++;
        }
        float slotWidth = pointRewardsObject.rectTransform.rect.width;
        HorizontalLayoutGroup layoutGroup = rewardsParent.GetComponent<HorizontalLayoutGroup>();
        
        float gaugeMaxWidth = (slotWidth * rewards.Count) + (layoutGroup.spacing * (rewards.Count - 1)) + layoutGroup.padding.left;

        float spacing = slotWidth + layoutGroup.spacing;
        float firstSector = (spacing / 2) + 10f;
        firstSector = Mathf.Ceil(firstSector);

        float firstSectorPoint = 1f;
        float addValue = 0f;
        if(sectorIndex < 1)
            firstSectorPoint = (float)remainPoint / (float)sectorSize;
        else if(sectorIndex < rewards.Count)
        {
            addValue = ((spacing) * ((float)remainPoint / (float)sectorSize));
            sectorIndex -= 1;
        }
        else
        {
            remainPoint = point - preSectorPoint;
            addValue = (float)remainPoint;
        }

        float gaugeWidth = ((spacing) * sectorIndex) + addValue + (firstSector * firstSectorPoint);
        pointValueGauge.SetGaugeWidth(gaugeWidth);

        StartCoroutine(FirstFocusing());
    }

    private IEnumerator FirstFocusing()
    {
        yield return YieldHelper.waitForSeconds(200);
        SetFocus();
    }

    private void SetFocus()
    {
        float focusPoint = pointValueGauge.transform.localPosition.x + 4f + pointValueGauge.gaugeWidth - (scrollviewPort.rect.width / 2);
        Vector2 focusPosition = new Vector2(-focusPoint, scrollContent.localPosition.y);

        if (focusPosition.x > 0)
            return;
        scrollContent.DOLocalMoveX(focusPosition.x, 0.5f).SetId("ScrollTween");
    }


    public override void CloseClick()
    {
        base.CloseClick();
        CM_Singleton<LobbyUI>.instance.BattlePassRewardNotiCheck();
        DOTween.Kill("ScrollTween");
        Destroy(gameObject);
    }

    public void OnClickPurchaseButton()
    {
        FirebaseLauncher.instance.LogEvent("btn_battle_pass_buy");

        CM_Singleton<UIManager>.instance.OnOpenPopup<BattlePassPurchasePopup>(UIPanels.Panel_Battle_Purchasing, this, CanvasDepth.Popups);
    }

    public void UpdateBattlePassPurchased(Dictionary<string, object> _passSeasonInfo, int _userPassPoint)
    {
        if(_passSeasonInfo != null && _passSeasonInfo.Count > 0)
        {
            m_userBattlePassSeasonInfo = new UserBattlePassSeason(_passSeasonInfo);
            CM_Singleton<GameData>.instance.m_Info_User.bBattlePassPurchased = true;
            m_purchasedPass = true;
            purchaseButton.gameObject.SetActive(false);
            battlePassRewardInfo.SetActive(true);
        }

        foreach(PassPointRewards rewards in m_pointRewards.Values)
        {
            rewards.SetPurchasedPass(m_purchasedPass);
            rewards.SetRewardEnable(_userPassPoint);
        }
        go_BonusLock.SetActive( !m_purchasedPass );
    }

    public void OnClickHelpButton()
    {
        string title = CM_Singleton<Table_Text>.instance.GetText("NoticePopup_Notice_Title");
        string message = string.Format(CM_Singleton<Table_Text>.instance.GetText("NoticePopup_Msg_15"));
        CM_Singleton<UIManager>.instance.CreateNoticeMsg(CanvasDepth.Popups, title, message);
    }

    public void OnClickBuyBattlePassPoint()
    {
        m_buyBattlePassPointData = GameDatabase.Instance.shopProducts.GetShopGroup(6)[1];
        int price = m_buyBattlePassPointData.price;
        int getPointValue = m_buyBattlePassPointData.productQuantity;


        UIManager.instance.CreateCommonBuyInfoPopup( CM_Singleton<Table_Text>.instance.GetText("BattlePass_Point_Purchase"), 
            string.Format(CM_Singleton<Table_Text>.instance.GetText("BattlePass_Message_3"), price, getPointValue),
            "item_pass_ticket", m_buyBattlePassPointData.productType, price, BuyBattlePassPoint, 0.5f);

        FirebaseLauncher.instance.LogEvent("btn_battle_pass_point");

        // UIManager.instance.CreateConfirmMsg(CanvasDepth.Popups, 
        //         CM_Singleton<Table_Text>.instance.GetText("BattlePass_Point_Purchase"), 
        //         string.Format(CM_Singleton<Table_Text>.instance.GetText("BattlePass_Message_3"), price, getPointValue),
        //         CM_Singleton<Table_Text>.instance.GetText("Btn_OK"), 
        //         CM_Singleton<Table_Text>.instance.GetText("Btn_Cancel"), 
        //         BuyBattlePassPoint, 
        //         null);
    }

    private void BuyBattlePassPoint()
    {
        WebServerAPIs.Instance.BuyShopProduct(m_buyBattlePassPointData.index, CallbackBuyBattlePassPoint);
    }


    private void CallbackBuyBattlePassPoint(APIContents data)
    {
        Dictionary<string, object> result = data.contents["user"] as Dictionary<string, object>;
        int updateFreeGemValue = (int)result["gem_f"];
        int updatePaidGemValue = (int)result["gem_c"];
        int updateBattlePassPointValue = (int)result["battle_pass_point"];

        m_userInfo.UpdateGemValue(updateFreeGemValue, updatePaidGemValue);
        m_userInfo.UpdateBattlePassPoint(updateBattlePassPointValue);
        passPointText.text = $"{updateBattlePassPointValue}";

        foreach (var item in m_pointRewards.Values)
            item.SetRewardEnable(updateBattlePassPointValue);
        
        // 게이지 변동
        StartCoroutine("CoRefresh");
    }

    public void OnClickBattlePassBonusButton()
    {
        Table_Text textLibrary = CM_Singleton<GameData>.instance.GetTextTable();
        string title = textLibrary.GetText("BattlePass_Season_AddReward_Title");
        string messageFormat = textLibrary.GetText("BattlePass_Season_AddReward_Desc");

        CommonRewardItem bonusReward = m_battlePassSeasonData.bonusReward;
        string nameIndex = Utils.GetItemNameIndex(bonusReward.rewardIndex);
        string name = textLibrary.GetText(nameIndex);
        string message = string.Format(messageFormat, m_battlePassSeasonData.bonusNeedPoint, m_battlePassSeasonData.bonusNeedPoint, name, bonusReward.rewardQuantity, m_battlePassSeasonData.bonusMaxPoint);

        UIManager uimanager = CM_Singleton<UIManager>.instance;
        uimanager.CreateNoticeMsg(title, message);
    }

    
}
