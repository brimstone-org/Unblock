using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Class used for TODO:FIXME:
/// </summary>
[System.Serializable]
public class LevelEntry
{
    /// <summary>
    /// Level string. TODO:FIX;
    /// </summary>
	public string LevelString;

    /// <summary>
    /// Number of optimumMoves? TODO:FIX;
    /// </summary>
	public int MovesCount;

	/// <summary>
	/// The win position.
	/// </summary>
	public Vector2 WinPosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:LevelEntry"/> class.
    /// </summary>
    /// <param name="level">Level's name/string</param>
    /// <param name="moves">Optimum moves.</param>
    /// <param name="winPosition">Win position.</param>
	public LevelEntry(string level, int moves, Vector2 winPosition){
		LevelString = level;
		MovesCount = moves;
        WinPosition = winPosition;
	}
   
}

/// <summary>
/// Level pack class, or set.
/// </summary>
[System.Serializable]
public class LevelSet
{
    [SerializeField]
    /// <summary>
    /// Difficulty of the pack.
    /// </summary>
    public string Difficulty;

    [SerializeField]
    /// <summary>
    /// List of levels.
    /// </summary>
	public List<LevelEntry> LevelsList = null;
}

/// <summary>
/// Game levels / packs holder.
/// </summary>
[System.Serializable]
public class GameLevels
{
    [SerializeField]
    /// <summary>
    /// Levels number per pack.
    /// </summary>
    public int LevelsAmountPerDifficulty;

    [SerializeField]
    /// <summary>
    /// Easy Pack.
    /// </summary>
    public LevelSet Easy = null;

    [SerializeField]
    /// <summary>
    /// Medium Pack.
    /// </summary>
    public LevelSet Medium = null;

    [SerializeField]
    /// <summary>
    /// Hard Pack.
    /// </summary>
    public LevelSet Hard = null;

    [SerializeField]
    /// <summary>
    /// Bonus Pack.
    /// </summary>
    public LevelSet Bonus = null;
}

/// <summary>
/// The Level Manager, this handles level loading, changing, etc.
/// </summary>
public class LevelManager : MonoBehaviour {

    /// <summary>
    /// Number of levels.
    /// </summary>
    public GameLevels m_gameLevels { get; private set; }

    /// <summary>
    /// Is the level loaded?
    /// </summary>
    bool m_levelsLoaded = false;

    [SerializeField]
    /// <summary>
    /// Is the request for editor?
    /// </summary>
    private bool forEditor = false;

    private void Start()
    {
        Camera.main.orthographicSize = (6.5f / Screen.width * Screen.height / 2.0f);
    }

	/// <summary>
    /// Loads all levels of any difficulty from different paths, depending if it's in editor or not.
    /// </summary>
    public void LoadLevels () {
        if (m_levelsLoaded)
        {
            return;
        }
        m_levelsLoaded = true;

        string userPath = System.IO.Path.Combine(Application.persistentDataPath, "levels.json");

        string contents = null;

        try
        {
            if ( File.Exists(userPath) )
            {
                System.IO.StreamReader fileStream = System.IO.File.OpenText(userPath); //Try to read from editor path.
                contents = fileStream.ReadToEnd();
            }
        }
        catch(FileNotFoundException)
        {
           
        }

        if ((contents == null || contents == "") || !forEditor)
        {
            string defaultPath = GetAssetPath("levels.json");  //If it's not in editor path, grab it from assets.
            contents = ReadFile(defaultPath);
        }

        if (contents != null)
        {
            m_gameLevels = JsonUtility.FromJson<GameLevels>(contents);
        }
        else
        {
            m_gameLevels = new GameLevels();
            int defaultLevelsAmountPerDifficulty = 100;
            m_gameLevels.LevelsAmountPerDifficulty = defaultLevelsAmountPerDifficulty;

			m_gameLevels.Easy = new LevelSet(){Difficulty="easy", LevelsList = new List<LevelEntry>(new LevelEntry[defaultLevelsAmountPerDifficulty])};
			m_gameLevels.Medium = new LevelSet(){Difficulty="medium", LevelsList = new List<LevelEntry>(new LevelEntry[defaultLevelsAmountPerDifficulty])};
			m_gameLevels.Hard = new LevelSet(){Difficulty="hard", LevelsList = new List<LevelEntry>(new LevelEntry[defaultLevelsAmountPerDifficulty])};
			m_gameLevels.Bonus = new LevelSet(){Difficulty="bonus", LevelsList = new List<LevelEntry>(new LevelEntry[defaultLevelsAmountPerDifficulty])};

            SaveLevels();
        }

        //Application.streamingAssetsPath
	}

    /// <summary>
    /// Saves all the levels, useful for editor exporting.
    /// </summary>
    public void SaveLevels()
    {
        string userPath = System.IO.Path.Combine(Application.persistentDataPath, "levels.json");
        string json = JsonUtility.ToJson(m_gameLevels);
        System.IO.File.WriteAllText(userPath, json);
    }

    /// <summary>
    /// Gets the asset path.
    /// </summary>
    /// <returns>The asset path.</returns>
    /// <param name="asset">Asset.</param>
    string GetAssetPath(string asset)
    {
        return System.IO.Path.Combine(Application.streamingAssetsPath, asset);
    }

    /// <summary>
    /// Reads the file.
    /// </summary>
    /// <returns>The file.</returns>
    /// <param name="filePath">File path.</param>
    string ReadFile(string filePath)
    {
        string contents = null;
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW linkstream = new WWW(filePath);

            while (!linkstream.isDone)
            {
            }
            contents = linkstream.text;
        }
        else
        {
            try
            {
                System.IO.StreamReader fileStream = System.IO.File.OpenText(filePath);
                contents = fileStream.ReadToEnd();
            }
            catch(FileNotFoundException)
            {

            }
        }

        return contents;
    }

    /// <summary>
    /// Gets a Level pack by it's name.
    /// </summary>
    /// <returns>The level set by name.</returns>
    /// <param name="name">Name.</param>
    LevelSet GetLevelSetByName(string name)
    {
        LevelSet levels = null;
        if (name == "easy")
        {
            levels = m_gameLevels.Easy;
        }
        else if (name == "medium")
        {
            levels = m_gameLevels.Medium;
        }
        else if (name == "hard")
        {
            levels = m_gameLevels.Hard;
        }
        else if (name == "bonus")
        {
            levels = m_gameLevels.Bonus;
        }
        return levels;
    }

    /// <summary>
    /// Sets the level. //TODO:FIXTHIS
    /// </summary>
    /// <param name="difficulty">Difficulty.</param>
    /// <param name="levelIndex">Level index.</param>
    /// <param name="levelString">Level string.</param>
    /// <param name="moves">Optimum moves.</param>
    /// <param name="winPos">Win position.</param>
	public void SetLevel(string difficulty, int levelIndex, string levelString, int moves, Vector2 winPos)
    {
        LevelSet levels = GetLevelSetByName(difficulty);

		levels.LevelsList[levelIndex] = new LevelEntry(levelString, moves, winPos);
    }

    /// <summary>
    /// Gets the level. //TODO:More explicit
    /// </summary>
    /// <returns>The level.</returns>
    /// <param name="difficulty">Difficulty.</param>
    /// <param name="levelIndex">Level index.</param>
	public LevelEntry GetLevel(string difficulty, int levelIndex)
    {
        LevelSet levels = GetLevelSetByName(difficulty);

        return levels.LevelsList[levelIndex];
    }

    /// <summary>
    /// Gets the number of levels per pack.
    /// </summary>
    /// <returns>Number of levels per pack.</returns>
    public int GetLevelsCount()
    {
        return m_gameLevels.LevelsAmountPerDifficulty;
    }
}
