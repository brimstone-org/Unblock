using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NativeInApps;

/// <summary>
/// Script used on Unlockable (paid) levels.
/// </summary>
public class UnlockableLevel : MonoBehaviour
{

    /// <summary>
    /// Is the level unlocked?
    /// </summary>
    private bool unlocked = false;

    [SerializeField]
    /// <summary>
    /// The name of the level.
    /// </summary>
    Text levelName;

    [SerializeField]
    /// <summary>
    /// The behind the scenes levelstring
    /// </summary>
    string levelString;

    [SerializeField]
    /// <summary>
    /// ID of the purchaseable asset.
    /// </summary>
    string productId;

    [SerializeField]
    /// <summary>
    /// Difficulty of the level expressed as a string.
    /// </summary>
    string levelDifficulty;

    [SerializeField]
    /// <summary>
    /// A screen to go to when unlocked.
    /// </summary>
    GoToScreen go;

    [SerializeField]
    /// <summary>
    /// Reference to the LevelMechanics script, drag it in the editor.
    /// </summary>
    LevelMechanics mechanics;

    [SerializeField]
    /// <summary>
    /// Reference to the button to be pressed to unlock the level. It is the same button that actually allows jumping to the level if it's unlocked.
    /// </summary>
    Button button;

    [SerializeField]
    /// <summary>
    /// The lock GameObject, this can look in any way you want, this will only appear over the level button if it's locked. It's recommended to turn off raycasthit on the respective Sprite.
    /// </summary>
    GameObject lockObject;

    /// <summary>
    /// At start of this script we check if this was already bought.
    /// </summary>
    private void Start()
    {
        RefreshStatus();
    }

    /// <summary>
    /// When the script is enabled we subscribe to OnPurchased event. We tie in our method where we'll do checks.
    /// </summary>
    private void OnEnable()
    {
        NativeInApp.OnItemPurchased += NativeInApp_OnItemPurchased;
    }

    /// <summary>
    /// The method subscribed to the Purchased event. It takes as parameter the newly purchased item, if it matches this item ID, it unlocks it.
    /// </summary>
    /// <param name="SKU">Purchased item ID</param>
    private void NativeInApp_OnItemPurchased(string SKU)
    {
        if (SKU == productId)
            PlayerPrefs.SetInt(SKU, 1);

        RefreshStatus();
    }

    /// <summary>
    /// When the script is disabled we clean out the subscribes.
    /// </summary>
    private void OnDisable()
    {
        NativeInApp.OnItemPurchased -= NativeInApp_OnItemPurchased;
    }

    /// <summary>
    /// Check when clicking on a object 
    /// </summary>
    public void OnClick()
    {
        if (unlocked)
        {
            go.Go();
            mechanics.SetCurrentDifficulty(levelDifficulty);
        }
        else
        {
            NativeInApp.Instance.BuyProductID(productId);
        }
    }

    void RefreshStatus()
    {
        if (PlayerPrefs.GetInt(productId) == 1)
            unlocked = true;

        if (unlocked)
        {
            levelName.text = levelString;
            lockObject.SetActive(false);
        }
        else
        {
            levelName.text = levelString + " - " + NativeInApp.Instance.GetLocalizedPrice(productId);
            lockObject.SetActive(true);
        }
    }
}
