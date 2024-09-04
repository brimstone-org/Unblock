using UnityEngine;
using System.Collections;
using NativeInApps;

/// <summary>
/// The Ads Manager handles the Ad requests, shows, and alternating.
/// </summary>
public class AdsManager : MonoBehaviour {

    /// <summary>
    /// Holds the first Ad service.
    /// </summary>
    public IAdService secondaryService;

    /// <summary>
    /// Holds the second Ad service.
    /// </summary>
    public IAdService primaryService;

    /// <summary>
    /// Instance for singleton design.
    /// </summary>
    /// <value>The instance.</value>
    public static AdsManager Instance { get; private set; }

    [SerializeField]
    /// <summary>
    /// Number of updates (in our case, games won), when the alternating ad service is fired.
    /// </summary>
    private int showRate = 3;

    /// <summary>
    /// Are Ads disabled? Useful for IAPP
    /// </summary>
    /// <value><c>true</c> if ads disabled; otherwise, <c>false</c>.</value>
	public bool AdsDisabled { get; private set; }

    /// <summary>
    /// Auxiliary var to decide which Ad provider to trigger at a given time.
    /// </summary>
    private bool showPrimary = true;

    /// <summary>
    /// The constant under the Ads state is stored. Useful for IAPP.
    /// </summary>
    private const string disabledAdsPrefString = "No Ads";
    public static string DisabledAdsPrefString
    {
        get {
            return disabledAdsPrefString;
        }
    }

    /// <summary>
    /// Number of levels.
    /// </summary>
    /// <value>The levels count.</value>
    public int LevelsCount { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:AdsManager"/> showed exit ad.
    /// </summary>
    /// <value><c>true</c> if showed exit ad; otherwise, <c>false</c>.</value>
	public bool ShowedExitAd{get; set;}

    
    /// <summary>
    /// Occurs when on exit ad closed.
    /// </summary>
    public event System.Action onExitAdClosed;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }

    private void Start()
    {
        primaryService.Initialize();
        if (secondaryService!=null)
        secondaryService.Initialize();

        RefreshAdsStatus();
    }

    /// <summary>
    /// Shows an ad from one of the services. If one is unavailable, the other one is called instead.
    /// </summary>
    /// <returns><c>true</c>, if ads was shown, <c>false</c> otherwise.</returns>
    public bool ShowAds() {
        bool result = false;

        if (AdsDisabled) {
            Debug.Log("Ads disabled");
            return result;
        }

        

        if (showPrimary) {
            if (!(result = primaryService.ShowAds()) && secondaryService!=null)
                result = secondaryService.ShowAds();
        } else {
            if (secondaryService != null&& !(result = secondaryService.ShowAds()))
                result = primaryService.ShowAds();
        }
        showPrimary = !showPrimary;
        return result;
    }

    /// <summary>
    /// Shows the exit ads.
    /// </summary>
    /// <returns><c>true</c>, if exit ads was shown, <c>false</c> otherwise.</returns>
    public bool ShowExitAds() {
        primaryService.onAdClosed += onExitAdClosed;
        if (secondaryService!=null)
        secondaryService.onAdClosed += onExitAdClosed;

        return ShowAds();

    }

    /// <summary>
    /// Updates the counter for showing ads. Call this usually at the end of a level.
    /// </summary>
	public void UpdateAds()
    {
       // Debug.Log("Showing Ads");
		LevelsCount++;
		if (LevelsCount % showRate == 0)
			ShowAds ();
	}

    /// <summary>
    /// Disables the ads. Useful for Ads IAPP.
    /// </summary>
    public static void DisableAds() {
        AdsManager.Instance.AdsDisabled = true;
        PlayerPrefs.SetInt(DisabledAdsPrefString, 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Subscribes our methods on certain events.
    /// </summary>
    void OnEnable() {
        NativeInApp.OnRefreshCompleted += RefreshAdsStatus;
		NativeInApp.OnItemPurchased += OnItemPurchased;
    }

    /// <summary>
    /// Cleans the subscribes.
    /// </summary>
    void OnDisable() {
        NativeInApp.OnRefreshCompleted -= RefreshAdsStatus;
        NativeInApp.OnItemPurchased -= OnItemPurchased;
    }


    /// <summary>
    /// This method triggers when there was an item just purchased. This is used to check if the Noads iapp was bought.
    /// </summary>
    /// <param name="sku">Id of the </param>
    void OnItemPurchased(string sku) {
        if (sku == NativeInApp_IDS.NO_ADS_PRODUCT_ID)
            DisableAds();

        RefreshAdsStatus();
    }

    /// <summary>
    /// This method used to check if noads IAPP was already bought before.
    /// </summary>
    void RefreshAdsStatus() {
        int enabled = PlayerPrefs.GetInt(DisabledAdsPrefString);

        AdsDisabled = enabled == 0 ? false : true;

        Debug.Log(AdsDisabled);
    }

}