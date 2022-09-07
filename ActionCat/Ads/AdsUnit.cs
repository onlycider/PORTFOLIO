using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api;

public class AdsUnit
{
    public enum AdsType
    {
        None, 
        Choice,
        Normal,
        Daily,
        Forced,
    }

    private const string m_appPubId = "ca-app-pub-4944321431692633/";
    private const string m_testPubId = "ca-app-pub-4944321431692633/2006898468"; 

    private RewardedAd m_rewardedAd;
    public RewardedAd rewardedAd{get{return m_rewardedAd;}}

    private string m_adUnitId;

    private int m_index;
    public int index{get{return m_index;}}

	private int m_resetIndex;
    public int resetIndex{get{return m_resetIndex;}}

    private DateTime m_lastRewardTime = DateTime.MinValue;
    public DateTime relieveShowAdTime{get{return m_lastRewardTime.AddSeconds(m_waitSeconds);}}
    public DateTime lastRewardTime { get { return m_lastRewardTime; } }

    private int m_waitSeconds = 120;
    public int waitSeconds{get{return m_waitSeconds;}}

    private int m_limitCount;
    public int limitCount{get{return m_limitCount;}}

    private int m_remainCount;
    public int remainCount{get{return m_remainCount;}}

    private bool m_enableShow = true;
    public bool enableShow{get{return m_enableShow;}}

    private int m_rewardedOrder;
    public int rewardedOrder{get{return m_rewardedOrder;}}

    private Action ON_AD_OPENED = null;

    private Action ON_USER_EARNED_REWARD = null;

    private Action ON_CLOSED_REWARDEDAD = null;

    private Action ON_FAILED_TO_SHOW = null;

    private AdsType m_adsType = AdsType.None;
    public AdsType adsType{get{return m_adsType;}}

    private CommonRewardItem m_adsRewardItem;
    public CommonRewardItem adsRewardItem{get{return m_adsRewardItem;}}

    private bool m_rewarded = false;
    public bool rewarded{get{return m_rewarded;}}



    public AdsUnit(Dictionary<string, object> data)
    {
#if UNITY_ANDROID
        string adUnitId = data["ad_unit_id"].ToString();
#elif UNITY_IOS
        string adUnitId = data["ad_ios_unit_id"].ToString();
#endif
        // if(WebServerAPIs.Instance.isLive == false || adUnitId == "undefined")
        //     m_adUnitId = m_testPubId;
        // else
        m_adUnitId = Utils.GetAppendedSentence(m_appPubId, adUnitId);

        m_index = (int)data["id"];
        m_resetIndex = (int)data["reset_id"];
        int limitCount = (int)data["limit_count"];
        m_limitCount = limitCount;
        m_remainCount = limitCount;
        m_waitSeconds = (int)data["wait_seconds"];
        string adsType = (string)data["ad_type"];
        SetAdsType(adsType);

        if(m_adsType == AdsType.Normal || m_adsType == AdsType.Daily)
        {
            int rewardIndex = (int)data["reward_id"];
            string rewardType = (string)data["reward_type"];
            int rewardQuantity = (int)data["reward_qty"];
            m_adsRewardItem = new CommonRewardItem(rewardIndex, rewardType, rewardQuantity);
        }


        LoadAd();
    }

    public void LoadAd()
    {
        // if(m_rewardedAd.IsLoaded())
        //     return;
        m_rewardedAd = new RewardedAd(m_adUnitId);

        // Called when an ad request has successfully loaded.
        m_rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        m_rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        m_rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        m_rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        m_rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        m_rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // string userAge = PlayerPrefsManager.GetString(PlayerPrefsKey.USER_AGE);
        int age = PlayerPrefsManager.GetInt(PlayerPrefsKey.USER_AGE, 12);
        string ageGrade = string.Empty;

        if (1 <= age && age <= 6)
            ageGrade = "G";
        else if (7 <= age && age <= 11)
            ageGrade = "PG";
        else if (12 <= age && age <= 17)
            ageGrade = "T";
        else
            ageGrade = "MA";

        AdRequest request = new AdRequest.Builder().AddExtra("max_ad_content_rating", ageGrade).Build();
        //Debug.Log("age :" + age + ", grade :" + ageGrade);
        m_rewardedAd.LoadAd(request);

    }

    public void SetUserAdUnitRewardInfo(Dictionary<string, object> data)
    {
        int rewardCount = (int)data["count"];
        DateTime rewardTime = DateTime.Parse((string)data["date_time"], System.Globalization.CultureInfo.InvariantCulture); 
        SetRemainCount(rewardCount);
        SetLastRewardTime(rewardTime);
    }

    public void SetRemainCount(int rewardCount)
	{
		m_remainCount = m_limitCount - rewardCount;
	}

	public void InitializeRemainCount()
	{
		m_remainCount = m_limitCount;
	}

    public void InitializeRewardedOrder()
    {
        m_rewardedOrder = 0;
    }

    public void SetLastRewardTime(DateTime rewardTime)
    {
        m_lastRewardTime =  rewardTime;
        
    }

    public void SetOnAdOpeningAction(Action on_ad_opened)
    {
        ON_AD_OPENED = on_ad_opened;
    }

    public void SetOnUserEarnedRewardActione(Action on_user_earned_reward)
    {
        ON_USER_EARNED_REWARD = on_user_earned_reward;
    }

    public void SetOnClosedRewardedAd(Action on_closed_rewardedad)
    {
        ON_CLOSED_REWARDEDAD = on_closed_rewardedad;
    }

    public void SetOnFailedToShowAction(Action on_failed_to_show)
    {
        ON_FAILED_TO_SHOW = on_failed_to_show;
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
        m_enableShow = true;
        m_rewarded = false;
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.LoadAdError.GetMessage());
        m_enableShow = false;
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        m_rewarded = false;
        MonoBehaviour.print("HandleRewardedAdOpening event received");
        Utils.InvokeAction(ON_AD_OPENED);
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
        Utils.InvokeAction(ON_FAILED_TO_SHOW);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
        Utils.InvokeAction(ON_CLOSED_REWARDEDAD);
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        // string type = args.Type;
        // double amount = args.Amount;
        // MonoBehaviour.print(
        //     "HandleRewardedAdRewarded event received for "
        //                 + amount.ToString() + " " + type);

        m_rewarded = true;
        Utils.InvokeAction(ON_USER_EARNED_REWARD);
    }

    private void SetAdsType(string stringAdsType)
    {
        switch(stringAdsType)
        {
            case "C" :
                m_adsType = AdsType.Choice;
                break;
            case "N" :
                m_adsType = AdsType.Normal;
                break;
            case "D" :
                m_adsType = AdsType.Daily;
                break;
            case "F" :
                m_adsType = AdsType.Forced;
                break;
            default :
                m_adsType = AdsType.None;
                break;
        }
    }
}

public class InterstitialAdUnit
{
    private InterstitialAd m_interstitialAd;
    public InterstitialAd interstitialAd{get{return m_interstitialAd;}}

    public InterstitialAdUnit()
    {
        LoadAd();
    }

    public void LoadAd()
    {
        #if UNITY_ANDROID
        string adUnitId = "ca-app-pub-4944321431692633/8074981972";
#elif UNITY_IOS
        string adUnitId = "ca-app-pub-4944321431692633/2984105693";
#else
        string adUnitId = "unexpected_platform";
#endif
        m_interstitialAd = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        m_interstitialAd.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        m_interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        m_interstitialAd.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        m_interstitialAd.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        // m_interstitialAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // string userAge = PlayerPrefsManager.GetString(PlayerPrefsKey.USER_AGE);
        int age = PlayerPrefsManager.GetInt(PlayerPrefsKey.USER_AGE, 12);
        string ageGrade = string.Empty;

        if (1 <= age && age <= 6)
            ageGrade = "G";
        else if (7 <= age && age <= 11)
            ageGrade = "PG";
        else if (12 <= age && age <= 17)
            ageGrade = "T";
        else
            ageGrade = "MA";

        AdRequest request = new AdRequest.Builder().AddExtra("max_ad_content_rating", ageGrade).Build();
        //Debug.Log("age :" + age + ", grade :" + ageGrade);
        m_interstitialAd.LoadAd(request);
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.LoadAdError.GetMessage());
        LoadAd();
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
        LoadAd();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }
}
