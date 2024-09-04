using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsListeners : MonoBehaviour {

    [SerializeField]
    /// <summary>
    /// Reference to the level manager script. Drag the object holding the LevelManager script here.
    /// </summary>
    LevelManager levelManager;

    /// <summary>
    /// When enabled, we subscribe to OnLevelWin coming from LevelMechanics event with our own method.
    /// </summary>
	void OnEnable()
    {
        LevelMechanics.OnLevelWin += OnLevelWin;
    }

	/// <summary>
    /// When disabled, we clear the previous subscribe(s).
	/// </summary>
	void OnDisable()
    {
        LevelMechanics.OnLevelWin -= OnLevelWin;
    }


    /// <summary>
    /// This method is called when a Level is won (finished). It updates the Ads cycles and checks for pack completion and achievements.
    /// </summary>
    /// <param name="level">Level number.</param>
    /// <param name="difficulty">Difficulty.</param>
    void OnLevelWin(int level, string difficulty)
    {
        AdsManager.Instance.UpdateAds();

        switch (difficulty)
        {
            case "easy":
                if (level == levelManager.m_gameLevels.Easy.LevelsList.Count - 1)
                    GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_easy_completed);
                break;
            case "medium":
                if (level == levelManager.m_gameLevels.Medium.LevelsList.Count - 1)
                    GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_medium_completed);
                break;
            case "hard":
                if (level == levelManager.m_gameLevels.Hard.LevelsList.Count - 1)
                    GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_hard_completed);
                break;
            case "bonus":
                if (level == levelManager.m_gameLevels.Bonus.LevelsList.Count - 1)
                    GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_bonus_completed);
                break;
        }
    }
}
