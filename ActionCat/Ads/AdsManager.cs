using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using GoogleMobileAds.Api;
using UnityEngine.Advertisements;

using System.Threading;

public class AdsManager : MonoBehaviour//, IUnityAdsInitializationListener
{
    public static AdsManager Instance;
    private Action REWARD_ACTION;
    private Action REWARD_CLOSE_ACTION;

    private bool m_adsOpened = false;
    public bool adsOpened { get { return m_adsOpened; } }

    private Dictionary<int, AdsUnit> m_userAdUnitInfos;
    public Dictionary<int, AdsUnit> userAdUnitInfos { get { return m_userAdUnitInfos; } }

    private DateTime m_adsOpenTime = DateTime.MinValue;

    private AdsUnit m_lastShownAd = null;

    private InterstitialAdUnit m_interstitialAdUnit;

    private List<int> m_displayStages;

    public UnityAdUnit unityAds;
    // public UnityInterstialAdUnit interstitialUnityAds;

    public bool bFailSkiper = false;

#if UNITY_ANDROID
    private string m_gameId = "";
#elif UNITY_IOS
    private string m_gameId = "";
#endif
    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        MobileAds.Initialize(initStatus => {
            Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
            foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
            {
                string className = keyValuePair.Key;
                AdapterStatus status = keyValuePair.Value;
                switch (status.InitializationState)
                {
                case AdapterState.NotReady:
                    // The adapter initialization did not complete.
                    MonoBehaviour.print("Adapter: " + className + " not ready.");
                    break;
                case AdapterState.Ready:
                    // The adapter was successfully initialized.
                    MonoBehaviour.print("Adapter: " + className + " is initialized.");
                    break;
                }
            }
        });
    }

    

    private void InitializeUnityAds()
    {
        bool testMode = !WebServerAPIs.Instance.isLive;
        Advertisement.Initialize(m_gameId, testMode);
    }

    public void SetUserAdUnitInfos()
    {
        m_userAdUnitInfos = GameDatabase.Instance.adsRewards.adsUnitInfos;
        foreach(KeyValuePair<int, AdsUnit> pair in m_userAdUnitInfos)
        {
            AdsUnit adUnit = pair.Value;
            adUnit.SetOnAdOpeningAction(OnOpenedRewardedAd);
            adUnit.SetOnClosedRewardedAd(OnClosedRewardedAd);
            // adUnit.SetOnUserEarnedRewardActione(OnUserEarnedReward);
        }

        m_interstitialAdUnit = new InterstitialAdUnit();
        // InitializeUnityAds();
    }

    public void SetUserAdUnitInfos(Dictionary<string, object>[] userAdRewardInfo)
    {
        if(userAdRewardInfo != null && userAdRewardInfo.Length > 0)
        {
            for(int i = 0; i < userAdRewardInfo.Length; i++)
            {
                Dictionary<string, object> info = userAdRewardInfo[i];
                int adsIndex = (int)info["reset_id"];
                if(m_userAdUnitInfos.ContainsKey(adsIndex))
                {
                    m_userAdUnitInfos[adsIndex].SetUserAdUnitRewardInfo(info);
                }
            }
        }
    }

    public void SetUserAdUnitInfo(Dictionary<string, object> userAdRewardInfo)
    {
        Dictionary<string, object> info = userAdRewardInfo;
        int adsIndex = (int)info["reset_id"];
        if (m_userAdUnitInfos.ContainsKey(adsIndex))
        {
            m_userAdUnitInfos[adsIndex].SetUserAdUnitRewardInfo(info);
        }
    }

    public void ShowAds(int adIndex, Action reward_action)
    {
        // Debug.LogErrorFormat($"khj in ShowAds");
        if(m_userAdUnitInfos.ContainsKey(adIndex))
            m_lastShownAd = m_userAdUnitInfos[adIndex];

        // Debug.LogErrorFormat($"khj in {m_lastShownAd}");
        if(m_lastShownAd == null)
        {
            // Debug.LogErrorFormat($"khj in null {m_lastShownAd}");
            DrawFailedToShowAd();
            // string warningTitle = CM_Singleton<Table_Text>.instance.GetText("NoticePopup_ADsWarning_Title");
            // string warningMessage = CM_Singleton<Table_Text>.instance.GetText("NoticePopup_ADsWarning_Msg");
            // UIManager.instance.CreateNoticeMsg(CanvasDepth.Popups, warningTitle, warningMessage, "", FailReload);
            return;
        }

        REWARD_ACTION = reward_action;



        if(CM_Singleton<GameData>.instance.m_Info_User.bBattlePassPurchased)
        {
            Utils.InvokeAction(REWARD_ACTION);
            return;
        }

#if UNITY_EDITOR
        Utils.InvokeAction(REWARD_ACTION);
#else


        if (m_lastShownAd.rewardedAd.IsLoaded()) 
        {
            // Debug.LogErrorFormat($"khj in IsLoad");
            m_lastShownAd.rewardedAd.Show();
        }
        else
        {
            // Debug.LogErrorFormat($"khj in UnityAds Video");
            unityAds.ShowRewardedVideo();
            return;
        }
        m_adsOpenTime = ServerSyncTime.now;
        // Debug.LogErrorFormat($"khj in End {m_adsOpenTime}");
#endif
    }

    public void ShowInterstitialAd(int number = 0)
    {
        if(WebServerAPIs.Instance.IsCkeckServer())  
            return;

        if(CM_Singleton<GameData>.instance.m_Info_User.purchased)
            return;

        if(CM_Singleton<GameData>.instance.m_Info_User.m_userLv < 5)
            return;


        // AdsManager.Instance.ShowAds( 14, CallbackShowEnergyChargeAds);
        // if(m_interstitialAdUnit.interstitialAd.IsLoaded())
        // {
        //     m_interstitialAdUnit.interstitialAd.Show();
        // }
        // else
        // {
        //     unityAds.ShowRewardedVideo();
        //     // interstitialUnityAds.ShowAd();
        // }
    }



    public void DrawFailedToShowAd()
    {
        if(bFailSkiper)
        {
            //한번 실패했는데 또 실패할경우 그냥 광고 스킵..
            Utils.InvokeAction(REWARD_ACTION);
            bFailSkiper =false;
            return;
        }
        bFailSkiper =true;

        string warningTitle = CM_Singleton<Table_Text>.instance.GetText("NoticePopup_ADsWarning_Title");
        string warningMessage = CM_Singleton<Table_Text>.instance.GetText("NoticePopup_ADsWarning_Msg");
        UIManager.instance.CreateNoticeMsg(CanvasDepth.Popups, warningTitle, warningMessage, "", FailReload);
    }

    private void FailReload()
    {
        m_lastShownAd.LoadAd();
    }

    public void AddRewardAdsCloseAction(Action reward_close_action)
    {
        REWARD_CLOSE_ACTION = reward_close_action;
    }

    private void OnOpenedRewardedAd()
    {
        m_adsOpened = true;
    }

    private void OnClosedRewardedAd()
    {
        // DateTime adsRewardTime = ServerSyncTime.now;
        // TimeSpan staytime = adsRewardTime - m_adsOpenTime;
        // // if(staytime.TotalSeconds > 300)
        // // {
        // //     // Debug.Log("Restart !!!!! ");
        // //     m_lastShownAd.LoadAd();
        // //     REWARD_CLOSE_ACTION = null;
        // //     // PanelManager.Instance.AddPanel<AdsErrorPopup>(PanelTag.AdsErrorPopup);
        // //     return;
        // // }
        // Debug.Log("OnClosedRewardedAd received ------- !!!!!!!!!!!! ");
        // StartCoroutine(DelayCloseEvent());
        Debug.Log("OnClosedRewardedAd received ------- !!!!!!!!!!!! ");

        Thread t = new Thread(ThreadClosedRewardedAd);
        t.Start("Dispatched ClosedRewardedAd");
    }

    private IEnumerator DelayCloseEvent()
    {
        // yield return YieldHelper.waitForRealSeconds(200);
        yield return new WaitForSecondsRealtime(200f * 0.001f);
        if(m_lastShownAd.rewarded)
        {
            Utils.InvokeAction(REWARD_ACTION);
            REWARD_ACTION = null;
            bFailSkiper =false; //광고정상 호출시 스킵퍼는 켜져있어도 끄기 //초기화..
            m_lastShownAd.LoadAd();
            yield break;
        }

        m_lastShownAd.LoadAd();
        Utils.InvokeAction(REWARD_CLOSE_ACTION);
        REWARD_CLOSE_ACTION = null;
    }

    private void OnUserEarnedReward()
    {
        bFailSkiper =false; //광고정상 호출시 스킵퍼는 켜져있어도 끄기 //초기화..
        Debug.Log("OnUserEarnedReward received ------- !!!!!!!!!!!! ");

        Thread t = new Thread(ThreadEarnedReward);
        t.Start("Dispatched EarnedReward");
    }

    // public void MakeDisplayAdsStage()
    // {
    //     m_displayStages = new List<int>(3);
    //     m_displayStages.Add(UnityEngine.Random.Range(1, 10));
    //     m_displayStages.Add(UnityEngine.Random.Range(11, 20));
    //     m_displayStages.Add(UnityEngine.Random.Range(21, 30));
    // }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        unityAds.SetRewardAdsShowEvent(OnUserEarnedReward);
        unityAds.SetFailAdsShowEvent(DrawFailedToShowAd);
        unityAds.LoadAd();
        // interstitialUnityAds.LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }



    //디스패쳐 오브젝트 필요..
    private void ThreadEarnedReward(object param)
    {
        Dispatcher.Current.BeginInvoke(() =>
        {
            Debug.Log("Thread"+param);
            Utils.InvokeAction(REWARD_ACTION);
            REWARD_ACTION = null;
        });
    }
    private void ThreadClosedRewardedAd(object param)
    {
        Dispatcher.Current.BeginInvoke(() =>
        {
            Debug.Log("Thread"+param);
            if(m_lastShownAd.rewarded)
            {
                Utils.InvokeAction(REWARD_ACTION);
                REWARD_ACTION = null;
                bFailSkiper =false; //광고정상 호출시 스킵퍼는 켜져있어도 끄기 //초기화..
                m_lastShownAd.LoadAd();
                return;
            }

            m_lastShownAd.LoadAd();
            Utils.InvokeAction(REWARD_CLOSE_ACTION);
            REWARD_CLOSE_ACTION = null;
        });
    }

}
