using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script used in editor to snap tiles to an imaginary grid.
/// </summary>
[ExecuteInEditMode]
public class SnapToGrid : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var mech = FindObjectOfType<LevelMechanics>();
        if (mech != null)
        {
            gameObject.transform.SetParent(mech.transform);
        }
        if (Application.isPlaying)
        {
            enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = new Vector3(
            Mathf.Round(transform.localPosition.x),
            Mathf.Round(transform.localPosition.y),
            Mathf.Round(transform.localPosition.z));
	}
}
