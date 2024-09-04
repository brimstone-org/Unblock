using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Levels menu. This class handles levels listing.
/// </summary>
public class LevelsMenu : MonoBehaviour {
    [SerializeField]
    /// <summary>
    /// TODO:WAT
    /// </summary>
    public GameObject Game;

    [SerializeField]
    /// <summary>
    /// The root for the grid where all the levels will be shown.
    /// </summary>
    public Transform LevelsGridRoot;

    [SerializeField]
    /// <summary>
    /// Prefab of LevelIcon class type.
    /// </summary>
    public GameObject LevelIconPrefab;

    [SerializeField]
    /// <summary>
    /// How many levels per page (screen)?
    /// </summary>
    public int LevelCountPerPage = 12;

    [SerializeField]
    /// <summary>
    /// FIXME:WHAT
    /// </summary>
    public int UnlockedLevelCountMax = 8;

    [SerializeField]
    /// <summary>
    /// FIXME:WHAT
    /// </summary>
    public GoToScreen StartGame;

    [SerializeField]
    /// <summary>
    /// LevelManager reference.
    /// </summary>
    public LevelManager LevelManager;

    [SerializeField]
    /// <summary>
    /// LevelMechanics reference.
    /// </summary>
    public LevelMechanics LevelMechanics;

    /// <summary>
    /// Current page in the levels list.
    /// </summary>
    private int m_currentPage = 0;

    /// <summary>
    /// Get the number of pages of levels.
    /// </summary>
    /// <returns>The page count.</returns>
    int GetPageCount()
    {
        LevelManager.LoadLevels();

        int levelCount = LevelManager.GetLevelsCount();

        int pageCount = levelCount / LevelCountPerPage;
        if (levelCount % LevelCountPerPage > 0)
        {
            ++pageCount;
        }

        return pageCount;
    }

	// Use this for initialization
	void OnEnable () {
        m_currentPage = PlayerPrefs.GetInt("CurrentPage", 0);
        LevelManager.LoadLevels();
        LevelCountPerPage = LevelManager.GetLevelsCount();
        ShowLevels(m_currentPage);
	}

    void OnButtonPress(int levelIndex)
    {
        var mechanics = Game.GetComponentInChildren<LevelMechanics>();
        mechanics.SetCurrentLevelIndex(levelIndex);
        StartGame.Go();   
    }

    void OnDisable()
    {
        PlayerPrefs.SetInt("CurrentPage", m_currentPage);
    }
    public void OnScrollPosChanged(float scrollPos)
    {
        int page = Mathf.RoundToInt(GetPageCount() * scrollPos);
        if (page != m_currentPage)
        {
            m_currentPage = page;
            ShowLevels(m_currentPage);
        }
    }

    /// <summary>
    /// Displays levels for a given page number.
    /// </summary>
    /// <param name="page">Page number.</param>
    void ShowLevels(int page)
    {
        string difficulty = LevelMechanics.GetCurrentDifficulty();

        if (page < GetPageCount())
        {
            // first level is always unlocked
            int firstLevel = PlayerPrefs.GetInt(difficulty+"Level0", -1);
            if (firstLevel == -1)
            {
                PlayerPrefs.SetInt(difficulty+"Level0", 0);
            }

            int unlockedLevelCount = Random.Range(1, UnlockedLevelCountMax);

            for (int childIt = 0; childIt < LevelsGridRoot.childCount; ++childIt)
            {
                Transform child = LevelsGridRoot.GetChild(childIt);
                Destroy(child.gameObject);
            }

            for (int i = 0; i < LevelCountPerPage; ++i)
            {
                int levelNb = page * LevelCountPerPage + i;
                if (levelNb < LevelManager.GetLevelsCount())
                {
                    // popuplate the list
                    LevelIcon levelIcon = Instantiate(LevelIconPrefab).GetComponent<LevelIcon>();

                    // check if level is unlocked. -1 is level locked. 0, 1, 2, 3 is number of stars
                    int starsCount = PlayerPrefs.GetInt(difficulty+"Level" + levelNb.ToString(), -1);

                    levelIcon.SetUnlocked(starsCount != -1);
                    levelIcon.SetStars(starsCount);
                    levelIcon.SetLevelName((levelNb + 1).ToString());
                    levelIcon.transform.SetParent(LevelsGridRoot, false);

                    var index = levelNb;
                    levelIcon.Btn.onClick.AddListener(
                        delegate()
                        {
                            OnButtonPress(index);
                        }
                    );
                }
            }
        }
    }
}
