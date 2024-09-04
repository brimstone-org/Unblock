using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Events tied to sound.
/// </summary>
public class SoundGameEvents : MonoBehaviour {

    private void Start()
    {
        LevelMechanics.OnDragStart += SoundManager.Instance.PlayDragBlock;
    }

    private void OnApplicationQuit()
    {
        LevelMechanics.OnDragStart -= SoundManager.Instance.PlayDragBlock;
    }

}
