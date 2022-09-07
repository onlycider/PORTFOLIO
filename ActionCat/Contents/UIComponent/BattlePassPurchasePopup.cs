using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattlePassPurchasePopup : PopupBone
{
    private BattlePassView m_parentView;
    public Image shadowImage;
    public Transform popupTr;
    public Text lb_PassDate;
    public override void StartInfoSetting(object info)
    {
        base.StartInfoSetting(info);
        m_parentView = info as BattlePassView;
        
        lb_PassDate.text = $"{m_parentView.m_battlePassSeasonData.startDate} ~ {m_parentView.m_battlePassSeasonData.endDate} (UTC+9)";
    }
    public override void OpenSetting()
    {
        SoundManager.PlaySFX("Sci-Fi Click");
        CM_Singleton<GameData>.instance.m_Mng_BackKey.EnrollBackKey(gameObject, CloseClick);
        popupTr.localScale = Vector3.zero;
        shadowImage.color = new Color32(255, 255, 255, 0);
        DOTween.To(() => shadowImage.color, x => shadowImage.color = x, new Color32(255, 255, 255, 255), 0.08f);
        popupTr.DOScale(Vector3.one, 0.08f);
    }

    public override void CloseClick()
    {
        base.CloseClick();
        DOTween.To(() => shadowImage.color, x => shadowImage.color = x, new Color32(255, 255, 255, 0), 0.08f);
        popupTr.DOScale(Vector3.zero, 0.08f).OnComplete(() => Destroy(gameObject));

    }

    public void OnClickPurchaseButton()
    {
        FirebaseLauncher.instance.LogEvent("btn_battle_pass_buy_pay");

        // int season = GameDatabase.Instance.battlePassSeason.GetCurrentSeasonData().season;
        // WebServerAPIs.Instance.BuyBattlePass(season, CallbackBuyBattlePass);
        IAPManager.instance.BuyProduct("battle_pass", CallbackBuyBattlePass);
    }

    private void CallbackBuyBattlePass(APIContents contents)
    {
        SoundManager.PlaySFX("Item Purchase (4)");
        Dictionary<string, object> productInfo = contents.contents["product_info"] as Dictionary<string, object>;
        Dictionary<string, object> passSeasonInfo = productInfo["user_battle_pass_season"] as Dictionary<string, object>;
        Dictionary<string, object> userInfo = productInfo["user"] as Dictionary<string, object>;
        int userPassPoint = (int)userInfo["battle_pass_point"];
        m_parentView.UpdateBattlePassPurchased(passSeasonInfo, userPassPoint);
        CloseClick();
    }
}
