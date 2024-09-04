using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Level exit setter for the editor.
/// </summary>
public class LevelExit : MonoBehaviour {

    [SerializeField]
    /// <summary>
    /// LevelMechanics Reference.
    /// </summary>
    LevelMechanics mech;

    [SerializeField]
    /// <summary>
    /// Object used as exit.
    /// </summary>
    GameObject outBlock;

    [SerializeField]
    Dropdown inputX;
    [SerializeField]
    Dropdown inputY;


    private void Awake()
    {
        inputX.value = 1;
        inputY.value = Mathf.RoundToInt(mech.WinPos.y);
        inputX.onValueChanged.AddListener( (text) => { OnEdit(); });
        inputY.onValueChanged.AddListener( (text) => { OnEdit(); });
        
    }


    void OnEdit()
    {
        float x = inputX.value;
        float y = inputY.value;

        if (x > 0)
            mech.WinPos = new Vector2(5, y);
        else
            mech.WinPos = new Vector2(0, y);

    }

}
