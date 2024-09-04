using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

/// <summary>
/// The solver script. This computes the optimum solution from a given point.
/// </summary>
public class Solver : MonoBehaviour {
   
    public delegate void OnSolverFinishedDelegate(Stack<BreadthSimMove> solution);
    public OnSolverFinishedDelegate OnSolverFinished;
	
    /// <summary>
    /// Starts the solving.
    /// </summary>
    /// <param name="blocks">Array containing all the blocks.</param>
    /// <param name="mapSize">Level size.</param>
    /// <param name="winPos">Win position.</param>
    public void StartSolving(Block[] blocks, Vector2 mapSize, Vector2 winPos)
    {
        StartCoroutine(SimulateBreadth(blocks, mapSize, winPos));
    }

    /// <summary>
    /// Can add a block on the specified position?
    /// </summary>
    /// <returns><c>true</c>, if a block can be added on the given pos, <c>false</c> otherwise.</returns>
    /// <param name="blocks">The blocks.</param>
    /// <param name="b">The specific block we want to test.</param>
    /// <param name="pos">The position we want to move it.</param>
    /// <param name="mapSize">Size of the level.</param>
    public bool CanAddBlock(Block[] blocks, Block b, Vector2 pos, Vector2 mapSize)
    {
        BreadthSimMap map = new BreadthSimMap(Mathf.RoundToInt(mapSize.x), Mathf.RoundToInt(mapSize.y));
        BreadthSimState sim = new BreadthSimState();

        foreach (var block in blocks)
        {
            BreadthSimBlockState blockState = new BreadthSimBlockState(block, block.transform.localPosition);
            sim.BlockStates.Add(blockState);
            sim.Move = null;

            if (block.IsMainBlock)
            {
                map.PlayerBlockIndex = sim.BlockStates.Count - 1;
            }
        }
        map.BreadthSimWriteState(sim);

        foreach (var elem in b.LogicalElements)
        {
            var elemPos = pos + elem;
            if (!map.WithinBounds(elemPos) )
            {
                return false;
            }
            else if ( !map.IsEmpty(elemPos))
            {
                return false;
            }
        }
        return true;
    }
  
    /// <summary>
    /// Simulates a breadth search.
    /// </summary>
    /// <param name="blocks">Blocks.</param>
    /// <param name="mapSize">Map size.</param>
    /// <param name="winPos">Win position.</param>
    IEnumerator SimulateBreadth(Block[] blocks, Vector2 mapSize, Vector2 winPos)
    {
        BreadthSimState sim = new BreadthSimState();
        List<Block> blocksList = new List<Block>(blocks);

        int width = Mathf.RoundToInt(mapSize.x);
        int height = Mathf.RoundToInt(mapSize.y);
        BreadthSimMap map = new BreadthSimMap(width, height);
        map.WinPosX = Mathf.RoundToInt(winPos.x);
        map.WinPosY = Mathf.RoundToInt(winPos.y);

        sim.Move = null;
        bool foundMainBlock = false;
        foreach (var block in blocksList)
        {
            BreadthSimBlockState blockState = new BreadthSimBlockState(block, block.transform.localPosition);
            sim.BlockStates.Add(blockState);
            sim.Move = null;

            if (block.IsMainBlock)
            {
                map.PlayerBlockIndex = sim.BlockStates.Count - 1;
                foundMainBlock = true;
            }
        }

        if (!foundMainBlock)
        {
            if (OnSolverFinished != null)
            {
                OnSolverFinished(null);
            }
            yield break;
        }

        Queue<BreadthSimState> queue = new Queue<BreadthSimState>();
        queue.Enqueue(sim);

        var comparer = new BreadthSimStateComparer();
        SortedList<BreadthSimState, BreadthSimState> previousStates = new SortedList<BreadthSimState, BreadthSimState>(comparer);

        int stepsPerFrame = 40;
        int stepsThisFrame = 0;


        while (queue.Count > 0)
        {
            BreadthSimState state = queue.Dequeue();
            //DebugShowState(state);
            if ( BreathSimStateExistedBefore(state, previousStates ))
            {
                continue;    
            }

            if (!map.CheckIsWinningState(state))
            {
                map.BreadthSimWriteState(state);
                //map.DebugPrintMap();

                for (int i = 0; i < state.BlockStates.Count; ++i)
                {
                    int dist = 1;
                    BreadthSimState newState = null;
                    while ((newState = map.MoveBlock(i, dist, state)) != null)
                    {
                        BreadthSimMove move = new BreadthSimMove(state.BlockStates[i].Block, dist);
                        move.PreviousMove = state.Move;
                        newState.Move = move;

                        queue.Enqueue(newState);
                        ++dist;
                    }

                    dist = -1;
                    newState = null;
                    while ((newState = map.MoveBlock(i, dist, state)) != null)
                    {
                        BreadthSimMove move = new BreadthSimMove(state.BlockStates[i].Block, dist);
                        move.PreviousMove = state.Move;
                        newState.Move = move;

                        queue.Enqueue(newState);
                        --dist;
                    }
                }
            }
            else
            {
                BreadthSimMove sm = state.Move;
                string moves = "";

                Stack<BreadthSimMove> resultMoves = new Stack<BreadthSimMove>();
                do
                {
                    resultMoves.Push(sm);
                    moves = sm.Block.gameObject.name + " moved " + sm.Amount.ToString() +"\n" + moves;    
                    sm = sm.PreviousMove;
                } while (sm != null);

                if (OnSolverFinished != null)
                {
                    OnSolverFinished(resultMoves);
                }

				//Debug.LogError("Hash Collision count " + BreadthSimStateComparer.collissionCount + "/" + BreadthSimStateComparer.totalCompares);
				//Debug.Log(moves);

                yield break;
            }

            if (stepsThisFrame >= stepsPerFrame)
            {
                stepsThisFrame = 0;
                yield return new WaitForEndOfFrame();
            }
            ++stepsThisFrame;
        }

        if (OnSolverFinished != null)
        {
            OnSolverFinished(null);
        }
    }

    /// <summary>
    /// Checks if a state was calculated before. If not, it adds it to the list, otherwise returns true.
    /// </summary>
    /// <returns>True if the state was previously calculated, otherwise adds it.</returns>
    /// <param name="state">The state</param>
    /// <param name="prevStates">List of previous states</param>
    bool BreathSimStateExistedBefore(BreadthSimState state, SortedList<BreadthSimState, BreadthSimState> prevStates)
    {
        Profiler.BeginSample("BreathSimStateExistedBefore");

        if (!prevStates.ContainsKey(state))
        {
            prevStates.Add(state, state);
            Profiler.EndSample();
            return false;
        }

        Profiler.EndSample();
        return true;
    }

    /// <summary>
    /// Method used in debugging to show a specific state in the search graph.
    /// </summary>
    /// <param name="state">State you want to view</param>
    void DebugShowState(BreadthSimState state)
    {
        foreach (var b in state.BlockStates)
        {
            b.Block.transform.localPosition = b.Pos;
        }
    }
}

/// <summary>
/// Holder of movement information.
/// </summary>
public class Move
{
    /// <summary>
    /// Block which is moved.
    /// </summary>
    public Block Block;

    /// <summary>
    /// How much it moved.
    /// </summary>
    public int Amount;

    /// <summary>
    /// Score.
    /// </summary>
    public int Score;

    /// <summary>
    /// Is it a winning move?
    /// </summary>
    public bool WinningMove = false;

    /// <summary>
    /// List of blocks free after the move.
    /// </summary>
    public List<Block> FreedBlocks = null;

    /// <summary>
    /// Method to compare if this move is equal to another one.
    /// </summary>
    /// <returns><c>true</c>, if it's equal, <c>false</c> otherwise.</returns>
    /// <param name="other">The other move, to compare to.</param>
    public bool IsEqual(Move other)
    {
        return other.Block == Block && other.Amount == Amount;
    }
}

/// <summary>
/// A state in the Simulation.
/// </summary>
public class SimulationState
{
    /// <summary>
    /// List of moves.
    /// </summary>
    public List<Move> Moves = null;

    /// <summary>
    /// Index of State
    /// </summary>
    public int Index = 0;

    /// <summary>
    /// List of blocks freed by parent move.
    /// </summary>
    public List<Block> FreedBlocksByParentMove = null;
}

/// <summary>
/// Breadth sim map.
/// </summary>
public class BreadthSimMap
{
    public int Width = 1;
    public int Height = 1;

    public int WinPosX = 0;
    public int WinPosY = 0;

    public int PlayerBlockIndex = -1;

    /// <summary>
    /// Block List.
    /// </summary>
    List<Block> m_map;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:BreadthSimMap"/> class.
    /// </summary>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    public BreadthSimMap(int width, int height)
    {
        Width = width;
        Height = height;

        m_map = new List<Block>();
        // +2 because we have borders
        int count = width * height;
        m_map.Capacity = count;

        for (int i = 0; i < count; ++i)
        {
            m_map.Add(null);
        }
    }

    /// <summary>
    /// Moves the block at a given index over a given distance and creates a new state.
    /// </summary>
    /// <returns>A state</returns>
    /// <param name="BlockIndex">Block index.</param>
    /// <param name="dist">Distance.</param>
    /// <param name="state">State.</param>
    public BreadthSimState MoveBlock(int BlockIndex, int dist, BreadthSimState state)
    {
        Profiler.BeginSample("MoveBlock");
        var blockState = state.BlockStates[BlockIndex];
        Vector2 moveDist = blockState.Block.MovementDir * dist;

        Vector2 movedOrigin = blockState.Pos + moveDist;

        foreach (var blockElem in blockState.Block.LogicalElements)
        {
            var pos = movedOrigin + blockElem;
            if (WithinBounds(pos))
            {
                Block b = GetBlockAtPos(m_map, pos.x, pos.y);
                if (b != blockState.Block && b != null)
                {
                    Profiler.EndSample();
                    return null;
                }
            }
            else
            {
                Profiler.EndSample();
                return null;
            }
        }

        BreadthSimState newState = new BreadthSimState(state);

        newState.BlockStates[BlockIndex].Pos = movedOrigin;

        Profiler.EndSample();
        return newState;
    }

    /// <summary>
    /// Checks if the given state is a winning one.
    /// </summary>
    /// <returns><c>true</c>, if the given state is winning, <c>false</c> otherwise.</returns>
    /// <param name="state">State.</param>
    public bool CheckIsWinningState(BreadthSimState state)
    {
        Profiler.BeginSample("CheckIsWinningState");
        BreadthSimBlockState blockState = state.BlockStates[PlayerBlockIndex];
        foreach (Vector2 elem in blockState.Block.LogicalElements)
        {
            Vector2 pos = blockState.Pos + elem;
            if (Mathf.FloorToInt(pos.x) == WinPosX)
            {
                if (Mathf.FloorToInt(pos.y) == WinPosY)
                {
                    return true;
                }
            }
        }
        Profiler.EndSample();

        return false;
    }

    /// <summary>
    /// Breadths the state of the sim write. Does no map bound checks. Attempts to write state to the map.
    /// </summary>
    /// <param name="state">State.</param>
    public void BreadthSimWriteState(BreadthSimState state)
    {
        Profiler.BeginSample("BreadthSimWriteState");
        for (int i = 0; i < m_map.Count; ++i)
        {
            m_map[i] = null;
        }

        foreach (var blockState in state.BlockStates)
        {
            foreach (Vector2 elem in blockState.Block.LogicalElements)
            {
                Vector2 pos = blockState.Pos + elem;
                SetBlockAtPos(m_map, pos.x, pos.y, blockState.Block);
            }
        }
        Profiler.EndSample();
    }

    /// <summary>
    /// Is position within bounds?
    /// </summary>
    /// <returns><c>true</c>, if position was withing bounds, <c>false</c> otherwise.</returns>
    /// <param name="pos">Position to check.</param>
    public bool WithinBounds(Vector2 pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < Width && pos.y < Height;
    }
    /// <summary>
    /// Is the space in the map at the given position, empty?
    /// </summary>
    /// <returns><c>true</c>, if there is no block on position, <c>false</c> otherwise.</returns>
    /// <param name="pos">Position.</param>
    public bool IsEmpty(Vector2 pos)
    {
        return GetBlockAtPos(m_map, pos.x, pos.y) == null;
    }

    /// <summary>
    /// Sets a block at a given position on the given map.
    /// </summary>
    /// <param name="map">Map of blocks.</param>
    /// <param name="x">The x coordinate (index).</param>
    /// <param name="y">The y coordinate (index).</param>
    /// <param name="b">The new block.</param>
    void SetBlockAtPos(List<Block> map, float x, float y, Block b)
    {
        map[Mathf.RoundToInt(y) * Width + Mathf.RoundToInt(x)] = b;
    }

    /// <summary>
    /// Gets the block at a given position from a given map.
    /// </summary>
    /// <returns>The block at position.</returns>
    /// <param name="map">Map of blocks.</param>
    /// <param name="x">The x coordinate (index).</param>
    /// <param name="y">The y coordinate (index).</param>
    Block GetBlockAtPos(List<Block> map, float x, float y)
    {
        return map[Mathf.RoundToInt(y) * Width + Mathf.RoundToInt(x)];
    }

    /// <summary>
    /// Method used in debug to print the moves map.
    /// </summary>
    public void DebugPrintMap()
    {
        string str = "";
        string rowstr = "";

        int columnIndex = 0;
        for(int i = 0; i < m_map.Count; ++i )
        {
            var elem = m_map[i];

            if (elem == null)
            {
                rowstr += "_______";
            }
            else
            {
                rowstr += elem.GetInstanceID().ToString("+000000;-000000");
            }
            ++columnIndex;
            if (columnIndex >= Width)
            {
                str = rowstr + "\n" + str;
                rowstr = "";
                columnIndex = 0;
            }
            else
            {
                rowstr += " ";
            }
        }

        Debug.Log(str);

        //Debug.Break();
    }
}

/// <summary>
/// Class that holds a Simulated Breadth Search move.
/// </summary>
public class BreadthSimMove
{
    /// <summary>
    /// The block.
    /// </summary>
    public Block Block;

    /// <summary>
    /// Movement amount.
    /// </summary>
    public int Amount;

    /// <summary>
    /// Last move.
    /// </summary>
    public BreadthSimMove PreviousMove = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:BreadthSimMove"/> class.
    /// </summary>
    /// <param name="b">The block.</param>
    /// <param name="a">The amount of movement.</param>
    public BreadthSimMove(Block b, int a)
    {
        Block = b;
        Amount = a;
    }
}

/// <summary>
/// Holds the state of a block in a Bread search Simulation
/// </summary>
public class BreadthSimBlockState
{
    /// <summary>
    /// The block.
    /// </summary>
    public Block Block;

    /// <summary>
    /// Block's position.
    /// </summary>
    public Vector2 Pos;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:BreadthSimBlockState"/> class.
    /// </summary>
    /// <param name="b">The block component.</param>
    /// <param name="pos">The position.</param>
    public BreadthSimBlockState(Block b, Vector2 pos)
    {
        Block = b;
        Pos = pos;
    }
}

/// <summary>
/// Breadth Saerch Simulation State holder.
/// </summary>
public class BreadthSimState 
{
    /// <summary>
    /// Hash of the state.
    /// </summary>
    private uint m_hash = 0;

    /// <summary>
    /// Gets the hash. Calculates it if it's not set.
    /// </summary>
    /// <value>The hash of the state.</value>
    public uint Hash
    {
        get
        {
            if (m_hash == 0)
            {
                RecalculateHash();
            }

            return m_hash;
        }
    }

    /// <summary>
    /// List of block states.
    /// </summary>
    List<BreadthSimBlockState> m_blockStates = null;

    /// <summary>
    /// Gets or sets the block states.
    /// </summary>
    /// <value>The block states.</value>
    public List<BreadthSimBlockState> BlockStates
    {
        get{return m_blockStates; }
        set
        {
            m_blockStates = value;
            m_hash = 0;
        }
    }

    /// <summary>
    /// A move.
    /// </summary>
    public BreadthSimMove Move;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:BreadthSimState"/> class using another state.
    /// </summary>
    /// <param name="other">Other state.</param>
    public BreadthSimState(BreadthSimState other)
    {
        BlockStates = new List<BreadthSimBlockState>();
        BlockStates.Capacity = other.BlockStates.Count;
        foreach (var blockState in other.BlockStates)
        {
            BlockStates.Add(new BreadthSimBlockState(blockState.Block, blockState.Pos));
        }

        if (other.Move != null)
        {
            Move = new BreadthSimMove(other.Move.Block, other.Move.Amount);
        }
        else
        {
            Move = null;
        }
    }

    /// <summary>
    /// Recalculates the hash.
    /// </summary>
    private void RecalculateHash()
    {
        m_hash = 0;
        int shift = 0;
        foreach(var blockState in m_blockStates)
        {
            m_hash += ((uint) blockState.Pos.x << shift) * 20000;
            m_hash += ((uint) blockState.Pos.y << shift);
            m_hash += (uint) Mathf.Abs(blockState.Block.gameObject.GetInstanceID()) << shift;
            ++shift;
            shift = shift % m_blockStates.Count;
        }
        // avoid hash being zero
        if (m_hash == 0)
        {
            ++m_hash;
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="T:BreadthSimState"/> class.
    /// </summary>
    public BreadthSimState()
    {
        BlockStates = new List<BreadthSimBlockState>();
        Move = null;
    }
}

/// <summary>
/// A comparer class for states.
/// </summary>
public class BreadthSimStateComparer : Comparer<BreadthSimState>
{
    /// <summary>
    /// Number of hash collisions
    /// </summary>
    public static int collissionCount = 0;

    /// <summary>
    /// The total compare number.
    /// </summary>
    public static int totalCompares = 0;

    /// <summary>
    /// Compare override. Compares two states.
    /// </summary>
    /// <returns>Collisions number.</returns>
    /// <param name="astate">First state.</param>
    /// <param name="bstate">Second state.</param>
    public override int Compare(BreadthSimState astate, BreadthSimState bstate)
    {
        ++totalCompares;

        // this function is the costliest of the entire algorithm.
        // using a hash to compare the states
        // improves performance of this function by about 4x in my tests
        int result = astate.Hash.CompareTo(bstate.Hash);

        if ( result == 0 )
        {
            var a = astate.BlockStates;
            var b = bstate.BlockStates;
            result = a.Count.CompareTo(b.Count);
            if (result == 0 )
            {
                for(int i = 0; i < a.Count;++i)
                {
                    result = a[i].Pos.x.CompareTo(b[i].Pos.x);
                    if ( result == 0 )
                    {
                        result = a[i].Pos.y.CompareTo(b[i].Pos.y);
                        if(result != 0 )
                        {
                            ++collissionCount;
                            return result;
                        }
                    }
                    else
                    {
                        ++collissionCount;
                        return result;
                    }
                }
            }
        }

        return result;

    }
}