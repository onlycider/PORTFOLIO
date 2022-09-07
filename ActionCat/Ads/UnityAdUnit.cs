using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdUnit : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private Action ON_USER_EARNED_REWARD = null;

    private Action ON_FAILED_SHOW_ADS;

    public void SetRewardAdsShowEvent(Action reward_shown_ads)
    {
        ON_USER_EARNED_REWARD = reward_shown_ads;
    }

    public void SetFailAdsShowEvent(Action failed_show_ads)
    {
        ON_FAILED_SHOW_ADS = failed_show_ads;
    }

    public void LoadAd()
    {
        Advertisement.Load("reward_video", this);
    }

    public void ShowRewardedVideo()
    {
        bool bReady = false;
        #if UNITY_ADS
            bReady = Advertisement.IsReady("reward_video")
        #endif
        if(bReady)
        {
            Advertisement.Show("reward_video", this);
        }
        else
        {
            Utils.InvokeAction(ON_FAILED_SHOW_ADS);
        }
        
    }

    // public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    // {
    //     if(showResult == ShowResult.Failed)
    //     {
    //         Utils.InvokeAction(ON_FAILED_SHOW_ADS);
    //         return;
    //     }

    //     // if(showResult == ShowResult.Skipped)
    //     // {
    //     //     Debug.Log("Skip Unity Ads Finished");
    //     //     return;
    //     // }

    //     //
    //     Utils.InvokeAction(ON_USER_EARNED_REWARD);
    // }

    
    // Implement Load Listener and Show Listener interface methods:  
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
        // Optionally execite code if the Ad Unit fails to load, such as attempting to try again.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        Utils.InvokeAction(ON_FAILED_SHOW_ADS);
        LoadAd();
        // Optionally execite code if the Ad Unit fails to show, such as loading another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if(showCompletionState == UnityAdsShowCompletionState.UNKNOWN)
        {
            Utils.InvokeAction(ON_FAILED_SHOW_ADS);
            return;
        }
        Utils.InvokeAction(ON_USER_EARNED_REWARD);
        LoadAd();
    }
}
