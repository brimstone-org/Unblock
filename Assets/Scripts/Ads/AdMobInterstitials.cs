#if (UNITY_ANDROID && !UNITY_AMAZON) || UNITY_IOS

using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System;

/// <summary>
/// This class handles any ads displayed through Admob.
/// </summary>
public class AdMobInterstitials : IAdService
{
    /// <summary>
    /// Holds the current interstitial.
    /// </summary>
    private InterstitialAd currentInterstitial;
    public InterstitialAd CurrentInterstitial
    {
        get {
            return currentInterstitial;
        }

        private set {
            currentInterstitial = value;
        }
    }

    /// <summary>
    /// Is an interstitial available?
    /// </summary>
    private bool hasInterstitial = false;
    public bool HasInterstitial
    {
        get {
            return hasInterstitial;
        }

        set {
            hasInterstitial = value;
        }
    }


    [SerializeField]
    /// <summary>
    /// AdMob ID for Android. Set from Inspector.
    /// </summary>
    private string androidId = "ca-app-pub-9461498517326987/5002468955";

    [SerializeField]
    /// <summary>
    /// AdMob ID for iOS. Set from Inspector.
    /// </summary>
    private string iosId = "ca-app-pub-8530091499387924/2522403293";



    public override void Initialize(string[] args)
    {
        Initialize();
    }

    public override void Initialize()
    {
        CurrentInterstitial = RequestInterstitial();
    }

    /// <summary>
    /// Requests an Ad and returns one.
    /// </summary>
    /// <returns>The interstitial.</returns>
    private InterstitialAd RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = androidId;
#elif UNITY_IPHONE
		string adUnitId = iosId;
#else
                string adUnitId = "unexpected_platform";
#endif

        Debug.Log("ADMOBDebug: Interstitial Request");
        // Initialize an InterstitialAd.
        InterstitialAd interstitial = new InterstitialAd(adUnitId);
        // Create an empty ad request.


        AdRequest request = new AdRequest.Builder()
         .AddTestDevice(AdRequest.TestDeviceSimulator)       // Simulator.
         .AddTestDevice("CB5A1YZ5HM")  // My test device.
         .AddTestDevice("QLF7N15928003477")
         .AddTestDevice("0ad2aa94063d90f9")
         .Build();

        // Load the interstitial with the request.
        interstitial.LoadAd(request);
        interstitial.OnAdClosed += HandleInterstitialClosed;
        interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
        interstitial.OnAdLoaded += HandleInterstitialAdLoaded;

        return interstitial;
    }

    private void HandleInterstitialAdLoaded(object sender, EventArgs e)
    {
        HasInterstitial = true;
        Debug.Log("ADMOBDebug: Interstitial Ad has loaded");
    }

    private void HandleInterstitialFailedToLoad(object sender, EventArgs e)
    {
        HasInterstitial = false;
        Debug.Log("ADMOBDebug: InterstitialFailed to load event received");
    }

    private void HandleInterstitialClosed(object sender, EventArgs e)
    {
        Debug.Log("ADMOBDebug: HandleInterstitialClosed event received");

        InterstitialAd thisAd = (InterstitialAd)sender;

        thisAd.Destroy();
        HasInterstitial = false;
        Debug.Log("ADMOBDebug: HandleInterstitial Destroyed");

        CurrentInterstitial = RequestInterstitial();
        Debug.Log("ADMOBDebug: HandleInterstitial Requested new interstitial");
    }

    /// <summary>
    /// Show an Ad
    /// </summary>
    /// <returns><c>true</c>, if ads was shown, <c>false</c> otherwise.</returns>
    public override bool ShowAds()
    {
        //returns false when ad wasn't loaded

        bool Result = false;
        if ((CurrentInterstitial != null) && (CurrentInterstitial.IsLoaded()))
        {
            Debug.Log("ADMOBDebug: Interstitial is loaded and displaying");
            Result = true;
            CurrentInterstitial.Show();
        }
        else
        {
            Result = false;
            CurrentInterstitial = RequestInterstitial();
        }
        return Result;
    }

    /// <summary>
    /// Checks if there is an Interstitial available.
    /// </summary>
    /// <returns><c>true</c>, if for ads was checked, <c>false</c> otherwise.</returns>
    public bool CheckForAds()
    {
        bool hasAds = false;
        if ((CurrentInterstitial != null) && (CurrentInterstitial.IsLoaded())) hasAds = true;
        return hasAds;
    }


}

#endif

