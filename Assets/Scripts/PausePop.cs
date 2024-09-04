using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePop : MonoBehaviour {

    public GameObject Panel;

    //private void OnApplicationFocus(bool focus)
    //{

    //    Time.timeScale = 0;
    //    Panel.SetActive(true);
    //}

    public void Resume()
    {
        Panel.gameObject.SetActive(false);
        Time.timeScale = 1;
    }



}
