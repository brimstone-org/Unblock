using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Level icon class used in the levels list.
/// </summary>
public class LevelIcon : MonoBehaviour {

    [SerializeField]
    /// <summary>
    /// Name of the level.
    /// </summary>
    Text LevelName;

    [SerializeField]
    /// <summary>
    /// Icon used for levels.
    /// </summary>
    Image Icon;

    [SerializeField]
    /// <summary>
    /// The star list, this stores all the star objects for a level object.
    /// </summary>
    List<Image> Stars;

    [SerializeField]
    /// <summary>
    /// The sprite used for a locked level.
    /// </summary>
    public Sprite LockedLevelIcon;

    [SerializeField]
    /// <summary>
    /// The sprite used for an unlocked level.
    /// </summary>
    public Sprite UnlockedLevelIcon;

    [SerializeField]
    /// <summary>
    /// Sprite used to display a filled star.
    /// </summary>
    public Sprite Star;

    [SerializeField]
    /// <summary>
    /// Sprite used to display an empty star.
    /// </summary>
    public Sprite StarSlot;

    [SerializeField]
    /// <summary>
    /// The actual button object which activates changelevel.
    /// </summary>
    public Button Btn;

    /// <summary>
    /// Is the level unlocked?
    /// </summary>
    private bool m_unlocked = true;

    /// <summary>
    /// Sets the filled stars number.
    /// </summary>
    /// <param name="starCount">Number of filled stars.</param>
    public void SetStars(int starCount)
    {
        int i = 0;
        for (; i < starCount; ++i)
        {
            Stars[i].sprite = Star;
        }
        for (; i < Stars.Count; ++i)
        {
            Stars[i].sprite = StarSlot;
        }
    }

    /// <summary>
    /// Unlocks a level.
    /// </summary>
    /// <param name="unlocked">If set to <c>true</c>, unlocks the level, or locks it if unlocked is false.</param>
    public void SetUnlocked(bool unlocked)
    {
        m_unlocked = unlocked;

        if (unlocked)
        {
            Icon.sprite = UnlockedLevelIcon;
        }
        else
        {
            Icon.sprite = LockedLevelIcon;
        }

        RefreshState();
    }

    /// <summary>
    /// Sets the display name of the level.
    /// </summary>
    /// <param name="name">Name.</param>
    public void SetLevelName(string name)
    {
        LevelName.text = name;
        RefreshState();
    }

    /// <summary>
    /// Refreshs the LevelIcon.
    /// </summary>
    void RefreshState()
    {
        LevelName.gameObject.SetActive(m_unlocked);
        LevelName.gameObject.SetActive(true);
        Btn.interactable = m_unlocked;
    }
}
