using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour {

	public void SetScale(float scale)
    {
        Time.timeScale = scale;
    }
}
