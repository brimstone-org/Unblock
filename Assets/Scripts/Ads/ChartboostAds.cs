using UnityEngine;
using System.Collections;
using ChartboostSDK;

/// <summary>
/// Class that wraps and initializes Chartbooost SDK.
/// </summary>
public class ChartboostAds : IAdService
{

    // Use this for initialization
    public override void Initialize()
    {
        Chartboost.didCloseInterstitial += Chartboost_didCloseInterstitial;
        Chartboost.setAutoCacheAds(true);
        Chartboost.cacheInterstitial(CBLocation.HomeScreen);
    }

    private void Chartboost_didCloseInterstitial(CBLocation obj)
    {
        TriggerOnAdClosed();
    }

    public override void Initialize(string[] args)
    {
        Initialize();
    }
    /// <summary>
    /// Shows the ads.
    /// </summary>
    /// <returns><c>true</c>, if ads was shown, <c>false</c> otherwise.</returns>
    public override bool ShowAds()
    {
        bool result = false;
        Debug.Log("Chartboost: Trying to show ads");
        if (Chartboost.hasInterstitial(CBLocation.HomeScreen))
        {
            Chartboost.showInterstitial(CBLocation.HomeScreen);
            Chartboost.cacheInterstitial(CBLocation.HomeScreen);
            Debug.Log("Chartboost: Ad Shown");
            result = true;
        }
        else
        {
            Debug.Log("Chartboost: Failed to show Add");
            Chartboost.cacheInterstitial(CBLocation.HomeScreen);
        }
        return result;
    }

}
