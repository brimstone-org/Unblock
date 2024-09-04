using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Block class.
/// </summary>
public class Block : MonoBehaviour {
    [SerializeField]
    public int SaveBlockType = 0;

    [SerializeField]
    /// <summary>
    /// Holds the axis in which the block can move.
    /// </summary>
    public Vector2 MovementDir = new Vector2(1, 0);

    [SerializeField]
    /// <summary>
    /// The last block delta.
    /// </summary>
    public Vector2 LastBlockDelta = Vector2.zero;

    [SerializeField]
    /// <summary>
    /// Is the block movable?
    /// </summary>
    public bool Movable = true;

    [SerializeField]
    /// <summary>
    /// Logical elements used in Solver.
    /// </summary>
    public List<Vector2> LogicalElements = new List<Vector2>();

    [SerializeField]
    /// <summary>
    /// True if the block is the main (movable one). False otherwise.
    /// </summary>
    public bool IsMainBlock = false;

    [SerializeField]
    /// <summary>
    /// GameObject representing the prefab that appears as a hint.
    /// </summary>
    public GameObject HintPrefab = null;
}
