using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class that can rotate the screen.
/// </summary>
public class EditorSwitcher : MonoBehaviour {

    [SerializeField]
    /// <summary>
    /// Holds the transform for the camera while in editor mode.
    /// </summary>
    Transform editorCamera;

    [SerializeField]
    /// <summary>
    /// Holds informational text
    /// </summary>
    UnityEngine.UI.Text text;

    bool normal;
	// Use this for initialization

    /// <summary>
    /// At the start of this script we check in which mode is the editor, normal or rotated.
    /// </summary>
	void Start () {
        normal = PlayerPrefs.GetInt("editorSetting", 1) == 1 ? true : false;
        UpdateRotation();
	}
	
    /// <summary>
    /// Switch the mode of the editor and save the setting.
    /// </summary>
	public void Switch()
    {
        normal = !normal;
        UpdateRotation();
        int norm = normal == true ? 1 : 0;
        PlayerPrefs.SetInt("editorSetting", norm);
    }

    /// <summary>
    /// Update the rotation accordingly to the mode.
    /// </summary>
    public void UpdateRotation()
    {
        if (normal)
        {
            editorCamera.localEulerAngles = Vector3.zero;
            text.text = "To Rotated";
        }
        else
        {
            editorCamera.localEulerAngles = new Vector3(0, 0, -90);
            text.text = "To Normal";
        }
    }

}
