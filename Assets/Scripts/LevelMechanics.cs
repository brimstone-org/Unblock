using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Profiling;
using UnityEngine.UI;

/// <summary>
/// This class controls the entire gameplay mechanic. All logic is in here.
/// </summary>
public class LevelMechanics : MonoBehaviour
{


    /// <summary>
    /// We declare a delegate for level event so we can tie events to it and emit. This specific one passes the level number and the difficulty.
    /// </summary>
    public delegate void LevelEvent(int level, string difficulty);
    /// <summary>
    /// We declare another delegate for a simpler level event. Without the parameters.
    /// </summary>
    public delegate void LevelSimpleEvent();

    /// <summary>
    /// This event should be fired only when a level has been completed.
    /// </summary>
    public static event LevelEvent OnLevelWin;

    /// <summary>
    /// This event should be fired only when a level has been loaded.
    /// </summary>
    public static event LevelEvent OnLevelLoaded;

    /// <summary>
    /// This event should be fired only when a drag starts (a tile starts moving). Currently it is used for sound.
    /// </summary>
    public static event LevelSimpleEvent OnDragStart;

    [SerializeField]
    Tutorial tutorial;

	// ================
	// serialized

	[SerializeField]
    /// <summary>
    /// This holds the GameObject that represents the exit.
    /// </summary>
    GameObject outBlock;

    [SerializeField]
    /// <summary>
    /// Size of the table containing the blocks. The number is represented in blocks. 1 block = 1 unit.
    /// </summary>
    Vector2 MapSize;

    /// <summary>
    /// FIXME:WTF TODO:
    /// </summary>
    float leftX = -3.055f;
    float rightX = 3.055f;

    /// <summary>
    /// This variable holds the optimum moves count.
    /// </summary>
    int solverMoves;

    [SerializeField]
    /// <summary>
    /// This variable holds the position of the level exit.
    /// </summary>
    Vector2 winPos = new Vector2(5f, 3f);
    /// <summary>
    /// Custom setter for the level exit. This positions it on the game table correctly.
    /// </summary>
    /// <value>The winposition.</value>
    public Vector2 WinPos
    {
        get {
            return winPos;
        }
        set {
            winPos = value;
            float x = winPos.x > 0 ? rightX : leftX;
            float y = winPos.y;
            outBlock.transform.localPosition = new Vector3(
           x,
           y - 2.5f,
           outBlock.transform.localPosition.z);
            outBlock.transform.localEulerAngles = new Vector3(
                 outBlock.transform.localPosition.x,
                 outBlock.transform.localEulerAngles.y,
                 x > 0 ? 0 : 180);
        }
    }

    [SerializeField]
    /// <summary>
    /// Holds the OK button used only in editor mode.
    /// </summary>
    GameObject OkButton;

    [SerializeField]
    /// <summary>
    /// Reference to the Solver object existing in the scene. Assign using inspector.
    /// </summary>
    Solver SolverObject = null;

    [SerializeField]
    /// <summary>
    /// Holds the hint button which triggers the solver.
    /// </summary>
    Button Hint;

    [SerializeField]
    /// <summary>
    /// Holds a simple overlay while the solver is working.
    /// </summary>
    GameObject SolvingOverlay = null;

    [SerializeField]
    /// <summary>
    /// List with all the blocks prefabs. The prefabs are combinations of length and rotations.
    /// </summary>
    List<GameObject> BlocksPrefabList = null;

    [SerializeField]
    /// <summary>
    /// The main block's moving speed after exiting the level.
    /// </summary>
    float m_blockWinAnimateSpeedPerSec = 5f;

    [SerializeField]
    /// <summary>
    /// The GameObject that represents the visual indicator for the right solution (shortest). This spawns on blocks and gives indication.
    /// </summary>
    GameObject HintArrow = null;

    [SerializeField]
    /// <summary>
    /// Animation curve used for controlling the HintArrow animation.
    /// </summary>
    AnimationCurve m_hintArrowCurve;

    [SerializeField]
    /// <summary>
    /// The selection object. Used exclusively in editor.
    /// </summary>
    GameObject SelectionObject;

    [SerializeField]
    /// <summary>
    /// Variable that holds level data.
    /// </summary>
    InputField SavedText;

    [SerializeField]
    /// <summary>
    /// Variable that holds a path in JSON from where the levels are loaded. This is mostly auto-generated.
    /// </summary>
    InputField JsonPath;

    [SerializeField]
    /// <summary>
    /// Text variable that holds the level number to be displayed.
    /// </summary>
    Text LevelNumber;

    [SerializeField]
    /// <summary>
    /// Test variable that holds the moves number to be displayed.
    /// </summary>
    Text MovesNumber;

    [SerializeField]
    /// <summary>
    /// Holds a LevelManager class reference.
    /// </summary>
    public LevelManager LevelManager;

    [SerializeField]
    /// <summary>
    /// StarsControl reference for when the level is won.
    /// </summary>
    public StarsControl WinMenu;

    [SerializeField]
    /// <summary>
    /// Unused StarControl reference when a level is lost. Which is not possible at this momment.
    /// </summary>
    public StarsControl LoseMenu;

    [SerializeField]
    /// <summary>
    /// Holds the button for the undo action.
    /// </summary>
    Button UndoButton;

    // ================
    // non serialized

    /// <summary>
    /// Auxiliary holding raycast results within the logic.
    /// </summary>
    RaycastHit2D[] m_raycastResults = null;

    /// <summary>
    /// Is a hint currently shown?
    /// </summary>
    bool m_playingHint = false;

    /// <summary>
    /// Holds the main block.
    /// </summary>
    Block m_mainBlock = null;

    /// <summary>
    /// Did we won? (Finish?)
    /// </summary>
    bool m_won = false;

    /// <summary>
    /// Which block is currently hinted upon?
    /// </summary>
    Block m_currentHintBlock = null;

    /// <summary>
    /// Holds the currently grabbed block.
    /// </summary>
    Block m_grabbedBlock = null;

    /// <summary>
    /// Holds auxiliary data about the drag length.
    /// </summary>
    Vector3 m_grabbedDelta = Vector3.zero;

    /// <summary>
    /// Holds the auxiliary original block position when a drag starts.
    /// </summary>
    Vector3 m_initialBlockPos = Vector3.zero;

    /// <summary>
    /// Are we in editor mode?
    /// </summary>
    bool m_inEditor = false;

    /// <summary>
    /// Holds the index of a block in the prefabs list.
    /// </summary>
    int BlockIndex = 0;

    /// <summary>
    /// Variable holding the current difficulty internal name.
    /// </summary>
    string m_currentDifficulty = "easy";

    /// <summary>
    /// Holds the internal level index.
    /// </summary>
    int m_levelIndex = 0;

    /// <summary>
    /// Auxiliary stack consisting of game states. When a undo is performed, the table is turned back to a previous state.
    /// </summary>
    Stack<string> m_gameStates = new Stack<string>();

    Coroutine winRoutine;

    /// <summary>
    /// When LevelMechanics is enabled, we get the mainblock and initialize variables
    /// that will be used during the game. Loading the level and ending with a UI refresh.
    /// </summary>
    void OnEnable()
    {
        m_mainBlock = GetMainBlock();

        m_raycastResults = new RaycastHit2D[Mathf.RoundToInt(MapSize.x * MapSize.y)];

        if (Hint != null)
            Hint.onClick.AddListener(StartSolver);

        SolverObject.OnSolverFinished += OnSolverFinished;
        m_won = false;

        LevelManager.LoadLevels();

        LevelEntry level = LevelManager.GetLevel(m_currentDifficulty, m_levelIndex);
        LoadLevelFromString(level.LevelString);
        WinPos = level.WinPosition;
        RefreshUI();
    }

    /// <summary>
    /// On disable, we perform a simple cleanup.
    /// </summary>
    void OnDisable()
    {
        if (Hint != null)
            Hint.onClick.RemoveListener(StartSolver);

        SolverObject.OnSolverFinished -= OnSolverFinished;
        m_playingHint = false;
        HintArrow.SetActive(false);
        ClearUndoHistory();
    }

    /// <summary>
    /// Loads the next level if there is one.
    /// </summary>
    public void LoadNextLevel()
    {
        LoadNextLevelImpl(false);
    }

    /// <summary>
    /// Implementation of next level. Usually, if this is called at the end of a level
    /// it's normal to pass false to the unlockCheck because you don't care if the next level is locked.
    /// </summary>
    /// <param name="unlockCheck">If set to <c>true</c>, the method will check if the next level is unlocked.</param>
    public void LoadNextLevelImpl(bool unlockCheck)
    {
        if (OkButton != null)
            OkButton.SetActive(false);

        m_playingHint = false;

        string difficulty = GetCurrentDifficulty();
        int levelsCount = LevelManager.GetLevelsCount(); LevelManager.GetLevelsCount();
        if (unlockCheck)
        {
            levelsCount = PlayerPrefs.GetInt(difficulty + "LastUnlockIndex", 0) + 1;
        }

        ++m_levelIndex;

        if (m_levelIndex > levelsCount - 1)
        {
            m_levelIndex = 0;
        }

        LevelEntry level = LevelManager.GetLevel(m_currentDifficulty, m_levelIndex);
        WinPos = level.WinPosition;
        LoadLevelFromString(level.LevelString);
        ClearUndoHistory();
        RefreshUI();
    }

    /// <summary>
    /// Loads the previous level if there is one.
    /// </summary>
    public void LoadPreviousLevel()
    {
        LoadPreviousLevelImpl(false);
    }


    /// <summary>
    /// Loading the previous level implementation. Has a parameter to know if it should check for level lock state.
    /// </summary>
    /// <param name="unlockCheck">If set to <c>true</c>, method will perform an unlock check.</param>
    void LoadPreviousLevelImpl(bool unlockCheck)
    {
        if (OkButton != null)
            OkButton.SetActive(false);

        m_playingHint = false;

        int levelsCount = LevelManager.GetLevelsCount();
        string difficulty = GetCurrentDifficulty();
        if (unlockCheck)
        {
            levelsCount = PlayerPrefs.GetInt(difficulty + "LastUnlockIndex", 0) + 1;
        }

        --m_levelIndex;

        if (m_levelIndex < 0)
        {
            m_levelIndex = levelsCount - 1;
        }

        LevelEntry level = LevelManager.GetLevel(m_currentDifficulty, m_levelIndex);
        LoadLevelFromString(level.LevelString);
        WinPos = level.WinPosition;
        ClearUndoHistory();
        RefreshUI();
    }

    /// <summary>
    /// Loads the last unlocked level.
    /// </summary>
    public void LoadNextLevelChecked()
    {
        LoadNextLevelImpl(true);
    }

    /// <summary>
    /// Unused and nefunctional.
    /// </summary>
    public void LoadPreviousLevelChecked()
    {
        LoadPreviousLevelImpl(true);
    }

    /// <summary>
    /// Refreshes all the live UI elements.
    /// </summary>
    void RefreshUI()
    {
        if (LevelNumber != null)
            LevelNumber.text = (m_levelIndex + 1).ToString();
        if (MovesNumber != null)
            MovesNumber.text = (m_gameStates.Count()).ToString();
        if (UndoButton != null) {
            if (m_gameStates.Count > 0)
                UndoButton.interactable = true;
            else
                UndoButton.interactable = false;
        }
    }

    /// <summary>
    /// Main loop logic. Handles input, dragging, etc.
    /// </summary>
    void Update()
    {
        if (Time.timeScale == 0)
            return;

        // check win condition
        if (!m_won && (m_mainBlock != null) &&
            (
                m_mainBlock.transform.localPosition == new Vector3(
                WinPos.x - m_mainBlock.LastBlockDelta.x,
                WinPos.y - m_mainBlock.LastBlockDelta.y,
                m_mainBlock.transform.localPosition.z) ||
                m_mainBlock.transform.localPosition == new Vector3(
                WinPos.x,
                WinPos.y,
                m_mainBlock.transform.localPosition.z)
            ) && !InputManager.GetMouseButton(0))
        {
            m_won = true;
            winRoutine = StartCoroutine(OnWin());
        }

        // we don't win while in editor
        if (m_inEditor)
        {
            m_won = false;
        }

        if ((m_mainBlock != null) && m_won)
        {
            var pos = m_mainBlock.transform.localPosition;
            pos.x += Mathf.Sign(WinPos.x - 5) * m_blockWinAnimateSpeedPerSec * Time.deltaTime;
            m_mainBlock.transform.localPosition = pos;
            return;
        }

        bool mbDown = InputManager.GetMouseButton(0);
        if (mbDown)
        {
            if (m_grabbedBlock == null)
            {

                Ray r = Camera.main.ScreenPointToRay(InputManager.mousePosition);
                RaycastHit2D result = Physics2D.Raycast(r.origin, r.direction);
                if ((result != null) && (result.collider != null))
                {
                    m_grabbedBlock = result.collider.gameObject.GetComponent<Block>();
                    if (m_grabbedBlock != null)
                    {
                        m_initialBlockPos = m_grabbedBlock.transform.position;
                        m_grabbedDelta = m_initialBlockPos - r.origin;

                        PushUndoHistory();

                        if (OnDragStart != null)
                            OnDragStart();
                    }
                }
            }
        }

        if (mbDown && m_grabbedBlock != null)
        {
            if (m_playingHint && (m_grabbedBlock != m_currentHintBlock))
            {
                return;
            }

            Ray r = Camera.main.ScreenPointToRay(InputManager.mousePosition);

            Vector3 mousePos = r.origin;

            // snap to axis
            if (m_grabbedBlock.MovementDir.x != 0)
            {
                mousePos.y = m_initialBlockPos.y;
                m_grabbedDelta.y = 0f;
            }

            // snap to axis
            if (m_grabbedBlock.MovementDir.y != 0)
            {
                mousePos.x = m_initialBlockPos.x;
                m_grabbedDelta.x = 0f;
            }

            // collision
            Vector3 newPos = mousePos + m_grabbedDelta;
            Vector3 dir = newPos - m_grabbedBlock.transform.position;

            float moveDist = dir.magnitude;

            dir = dir.normalized * Mathf.Max(MapSize.x, MapSize.y);
            int count = 0;
            if (dir.sqrMagnitude > 0)
            {
                count = Physics2D.RaycastNonAlloc(m_grabbedBlock.transform.position, dir, m_raycastResults);
            }

            if (count > 1)
            {
                float minDistance = float.MaxValue;
                float secondMinDistance = float.MaxValue;
                for (int i = 0; i < count; ++i)
                {
                    var result = m_raycastResults[i];
                    if (result.distance < minDistance)
                    {
                        secondMinDistance = minDistance;
                        minDistance = result.distance;
                    }
                    else if (result.distance < secondMinDistance)
                    {
                        secondMinDistance = result.distance;
                    }
                }
                Vector2 hit = m_grabbedBlock.transform.position + dir.normalized * secondMinDistance;
                count = Physics2D.RaycastNonAlloc(hit, -dir, m_raycastResults);

                float possibleMoveDist = 0f;
                for (int j = 0; j < count; ++j)
                {
                    if (m_raycastResults[j].collider.gameObject == m_grabbedBlock.gameObject)
                    {
                        possibleMoveDist = m_raycastResults[j].distance;
                    }
                }
                if (possibleMoveDist < moveDist)
                {
                    m_grabbedBlock.transform.position += dir.normalized * possibleMoveDist;
                }
                else
                {
                    m_grabbedBlock.transform.position = newPos;
                }
            }
            else
            {
                m_grabbedBlock.transform.position = newPos;
            }

        }
        else if (!mbDown && m_grabbedBlock != null)
        {
            // drag ended, snap to grid
            m_grabbedBlock.transform.localPosition = new Vector3(
                Mathf.Round(m_grabbedBlock.transform.localPosition.x),
                Mathf.Round(m_grabbedBlock.transform.localPosition.y),
                Mathf.Round(m_grabbedBlock.transform.localPosition.z));

            if (m_grabbedBlock.transform.position != m_initialBlockPos)
            {
                RefreshUI();
            }

            m_grabbedBlock = null;
        }

        UpdateEditor();
    }

    /// <summary>
    /// Calculates the stars awarded for completing the level with the current number of moves.
    /// Does this by looking at the constants and the optimum moves for the specific level.
    /// </summary>
    /// <returns>The stars number.</returns>
    public int CalcaulateStarCount()
    {
        LevelEntry level = LevelManager.GetLevel(m_currentDifficulty, m_levelIndex);
        int stars = 0;

        Debug.Log(GetMovesCount() + " " + Mathf.CeilToInt(Constants.FirstStarPercentage * level.MovesCount) + " " + Mathf.CeilToInt(Constants.SecondStarPercentage * level.MovesCount) + " " + Mathf.CeilToInt(Constants.ThirdStarPercentage * level.MovesCount));
        if (GetMovesCount() <= level.MovesCount + Mathf.Max(3, Mathf.CeilToInt(Constants.FirstStarPercentage * level.MovesCount)))
            stars++;
        if (GetMovesCount() <= level.MovesCount + Mathf.Max(2, Mathf.CeilToInt(Constants.SecondStarPercentage * level.MovesCount)))
            stars++;
        if (GetMovesCount() <= level.MovesCount + Mathf.Max(1, Mathf.CeilToInt(Constants.ThirdStarPercentage * level.MovesCount)))
            stars++;
        return stars;
    }

    /// <summary>
    /// Method called when the player completes a level. It calls events, sets info and opens the menu.
    /// </summary>
    IEnumerator OnWin()
    {
        Debug.Log("Win coroutine");
        yield return new WaitForSeconds(1);

        string difficulty = GetCurrentDifficulty();

        if (OnLevelWin != null)
            OnLevelWin(m_levelIndex, difficulty);

        // random number of stars, but never less than it already had. Temporary
        int prevStarCount = PlayerPrefs.GetInt(difficulty + "Level" + (m_levelIndex), 0);


        int starCount = CalcaulateStarCount();

        if (WinMenu != null)
            WinMenu.SetStars(starCount);

        if (starCount < prevStarCount)
        {
            starCount = prevStarCount;
        }

        PlayerPrefs.SetInt(difficulty + "Level" + (m_levelIndex).ToString(), starCount);

        // if next level is locked, unlock it
        if (PlayerPrefs.GetInt(difficulty + "Level" + (m_levelIndex + 1), -1) == -1)
        {
            PlayerPrefs.SetInt(difficulty + "Level" + (m_levelIndex + 1).ToString(), 0);
            PlayerPrefs.SetInt(difficulty + "LastUnlockIndex", m_levelIndex + 1);
        }

        if (WinMenu != null)
            WinMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// Starts the solver algorithm.
    /// </summary>
    public void StartSolver()
    {
        if (m_won)
        {
            Debug.Log("Solver won't start: won = true");
            return;
        }

        if (!m_playingHint)
        {
            m_playingHint = true;
            Block[] blocks = gameObject.GetComponentsInChildren<Block>();
            SolvingOverlay.SetActive(true);
            SolverObject.StartSolving(blocks, MapSize, WinPos);
            Debug.Log("Solver starting");
        }
        else
        {
            Debug.Log("Solver won't start: not playingHint");
            m_playingHint = false;
        }
    }

    /// <summary>
    /// Returns the main block.
    /// </summary>
    /// <returns>The main block.</returns>
    Block GetMainBlock()
    {
        Block[] blocks = gameObject.GetComponentsInChildren<Block>();
        var mainBlocks = blocks.Where(a => a.IsMainBlock).ToList();
        if (mainBlocks.Count > 0)
        {
            return mainBlocks[0];
        }
        return null;
    }

    /// <summary>
    /// This method is called when the solver has finished computing and is used
    /// to translate the moves returned from the solver in actual hints.
    /// </summary>
    /// <param name="moves">Solver result in moves.</param>
    void OnSolverFinished(Stack<BreadthSimMove> moves)
    {

        Debug.Log("Solver finished" + moves);

        if (moves != null)
        {
            solverMoves = moves.Count;
            if(OkButton != null)
                OkButton.SetActive(true);
            StartCoroutine(OnSolverFinishedCoroutine(moves));
        }
        else
        {
            m_playingHint = false;
            SolvingOverlay.SetActive(false);
        }
    }

    /// <summary>
    /// Coroutine that handles the actual hinting and animation mechanism.
    /// </summary>
    /// <param name="moves">Moves returned from the solver.</param>
    IEnumerator OnSolverFinishedCoroutine(Stack<BreadthSimMove> moves)
    {
        SolvingOverlay.SetActive(false);

        Debug.Log("Moves" + moves.Count);

        foreach (var m in moves)
        {
            m_currentHintBlock = m.Block;
            if (m_currentHintBlock != null)
            {
                var movement = m.Block.MovementDir * m.Amount;

                Vector3 targetPos = m.Block.transform.localPosition + new Vector3(movement.x, movement.y, 0);
                GameObject hint = Instantiate(m_currentHintBlock.HintPrefab);
                hint.transform.SetParent(m_currentHintBlock.transform.parent, false);
                hint.transform.localPosition = targetPos;

                HintArrow.SetActive(true);
                HintArrow.transform.position = m.Block.transform.position;

                m_hintArrowCurve.postWrapMode = WrapMode.Loop;
                float startTime = Time.time;

                Vector3 destination = m.Block.transform.position + new Vector3(movement.x, movement.y, 0);

                while ((m.Block != null) && ((m.Block.transform.localPosition != targetPos) || InputManager.GetMouseButton(0)) && !m_won && m_playingHint)
                {
                    Vector3 deltaMove = destination - m.Block.transform.position;
                    Quaternion rot = Quaternion.FromToRotation(Vector3.right, deltaMove);

                    HintArrow.transform.rotation = rot;
                    HintArrow.transform.position = m.Block.transform.position + deltaMove * m_hintArrowCurve.Evaluate(Time.time - startTime);

                    yield return new WaitForEndOfFrame();
                }
                Destroy(hint);
            }

            HintArrow.SetActive(false);

            //yield return new WaitForSeconds(1f);

            if (m_playingHint == false)
            {
                break;
            }
        }

        m_playingHint = false;
    }

    /// <summary>
    /// Sets the index of the block. Used only in editor logic.
    /// </summary>
    /// <param name="index">Index.</param>
    public void SetBlockIndex(int index)
    {
        BlockIndex = index;
    }

    /// <summary>
    /// Sets the selection object. Used only in editor.
    /// </summary>
    /// <param name="obj">Object.</param>
    public void SetSelection(GameObject obj)
    {
        SelectionObject.transform.position = obj.transform.position;
    }

    /// <summary>
    /// Sets a variable indicating we are in editor mode. This switches the logic of this class into editor mode.
    /// </summary>
    /// <param name="inEditor">If set to <c>true</c>, we are in editor mode.</param>
    public void SetInEditor(bool inEditor)
    {
        m_inEditor = inEditor;

        if (m_inEditor)
        {
            // stop the hinting if we go in the editor
            m_playingHint = false;
        }
    }

    /// <summary>
    /// Method used in editor to save the level.
    /// </summary>
    public void SaveLevel()
    {
        LevelManager.SetLevel(m_currentDifficulty, m_levelIndex, SaveLevelToString(), solverMoves, WinPos);
        SavedText.text = SaveLevelToString();

        JsonPath.text = "All levels written to " + Application.persistentDataPath + "/levels.json";
        LevelManager.SaveLevels();
    }

    /// <summary>
    /// Transforms the level data into a string.
    /// </summary>
    /// <returns>The level to string.</returns>
    public string SaveLevelToString()
    {
        string result = "";
        Block[] blocks = gameObject.GetComponentsInChildren<Block>();
        foreach (var block in blocks)
        {
            result += block.SaveBlockType.ToString();
            result += Mathf.RoundToInt(block.transform.localPosition.x).ToString();
            result += Mathf.RoundToInt(block.transform.localPosition.y).ToString();
        }

        return result;
    }

    /// <summary>
    /// Loads a level.
    /// </summary>
    public void LoadLevel()
    {
        LoadLevelFromString(SavedText.text);
    }

    /// <summary>
    /// Loads the level data from string.
    /// </summary>
    /// <param name="level">Level data.</param>
    public void LoadLevelFromString(string level)
    {
        if(tutorial != null)
            tutorial.Clear();

        for (int i = 0; i < transform.childCount; ++i)
        {
            var obj = transform.GetChild(i);
            Destroy(obj.gameObject);
        }
        transform.DetachChildren();

        if (level == null)
        {
            return;
        }

        var charEnumerator = level.GetEnumerator();
        while (charEnumerator.MoveNext())
        {
            char typeStr = charEnumerator.Current;
            int type = int.Parse(typeStr.ToString());
            charEnumerator.MoveNext();

            char xStr = charEnumerator.Current;
            int x = int.Parse(xStr.ToString());
            charEnumerator.MoveNext();

            char yStr = charEnumerator.Current;
            int y = int.Parse(yStr.ToString());
            CreateBlock(type, x, y);
        }

        if (OnLevelLoaded != null)
            OnLevelLoaded(m_levelIndex, m_currentDifficulty);

        InitLevel();
    }

    /// <summary>
    /// Initializes a level.
    /// </summary>
    void InitLevel()
    {
        m_mainBlock = GetMainBlock();
        m_won = false;

        if (tutorial != null && tutorial.IsTutorial(m_levelIndex, m_currentDifficulty))
            tutorial.MakeTutorial();
    }

    /// <summary>
    /// Pushes a state to history. Each time you move a block this should be called.
    /// </summary>
    void PushUndoHistory()
    {
        string level = SaveLevelToString();
        //Debug.Log("pushing history");
        // only push a state if it's different from what we already have at the top
        if (((m_gameStates.Count() > 0) && (m_gameStates.Peek() != level)) || (m_gameStates.Count() == 0))
        {
            m_gameStates.Push(level);
        }
    }

    /// <summary>
    /// Pops the undo history. This method does not return the state. Instead it directly "loads" it, resetting the table to the last state knew in the stack.
    /// </summary>
    public void PopUndoHistory()
    {
       // Debug.Log("popping history");
        if (m_gameStates.Count > 0)
        {
            string level = m_gameStates.Pop();
            LoadLevelFromString(level);

            RefreshUI();
        }
    }

    /// <summary>
    /// Clears all the states in the moves stack.
    /// </summary>
    void ClearUndoHistory()
    {
        m_gameStates.Clear();
    }

    /// <summary>
    /// Update loop for the editor.
    /// </summary>
    public void UpdateEditor()
    {
        if (!m_inEditor)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (BlockIndex < BlocksPrefabList.Count)
            {
                Block[] blocks = gameObject.GetComponentsInChildren<Block>();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 pos = ray.origin - transform.position;

                pos.x = Mathf.RoundToInt(pos.x);
                pos.y = Mathf.RoundToInt(pos.y);

                if (SolverObject.CanAddBlock(blocks, BlocksPrefabList[BlockIndex].GetComponent<Block>(), pos, MapSize))
                {
                    CreateBlock(BlockIndex, pos.x, pos.y);

                    m_mainBlock = GetMainBlock();
                }
            }
            else
            {
                if (BlockIndex == 5)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<Block>() != null)
                        {
                            Destroy(hit.collider.gameObject);
                            m_mainBlock = GetMainBlock();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Creates a new block of type at coords x,y. This should only be used in the Editor.
    /// </summary>
    /// <returns>The block.</returns>
    /// <param name="type">Type.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    Block CreateBlock(int type, float x, float y)
    {
        Vector3 pos;
        pos.x = Mathf.RoundToInt(x);
        pos.y = Mathf.RoundToInt(y);
        pos.z = 0;

        Block obj = Instantiate(BlocksPrefabList[type]).GetComponent<Block>();
        obj.transform.SetParent(transform, false);
        obj.transform.localPosition = pos;

        return obj;
    }

    /// <summary>
    /// Sets the current level difficulty.
    /// </summary>
    /// <param name="difficulty">Difficulty.</param>
    public void SetCurrentDifficulty(string difficulty)
    {
        m_currentDifficulty = difficulty;
    }

    /// <summary>
    /// Gets the current level difficulty.
    /// </summary>
    /// <returns>The current difficulty.</returns>
    public string GetCurrentDifficulty()
    {
        return m_currentDifficulty;
    }

    /// <summary>
    /// Sets the index of the current level. This is used for Load/Save operations.
    /// </summary>
    /// <param name="levelIndex">Level index.</param>
    public void SetCurrentLevelIndex(int levelIndex)
    {
        m_levelIndex = levelIndex;
    }

    /// <summary>
    /// Gets the moves for the current game.
    /// </summary>
    /// <returns>The moves count.</returns>
    public int GetMovesCount()
    {
        return m_gameStates.Count();
    }

    /// <summary>
    /// Reloads the current level.
    /// </summary>
    public void ReloadLevel()
    {
        LevelEntry level = LevelManager.GetLevel(m_currentDifficulty, m_levelIndex);
        LoadLevelFromString(level.LevelString);
        WinPos = level.WinPosition;
        ClearUndoHistory();
        RefreshUI();
        if(winRoutine != null)
            StopCoroutine(winRoutine);
    }
}
