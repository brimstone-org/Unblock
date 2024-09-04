using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltraWideSupport : MonoBehaviour {

    [SerializeField]
    float aspectThreshold;
    [SerializeField]
    RectTransform[] toChange;
    [SerializeField]
    new Camera camera;
    [SerializeField]
    float canvasWidth = 400;
    [SerializeField]
    float cameraSize = 6.2f;

    //private void Start()
    //{
    //    float aspect = Screen.height * 1f / Screen.width;
    //    Debug.Log(aspect);
    //    if (aspect < aspectThreshold)
    //        return;

    //    foreach(var v in toChange)
    //    {
    //        var s = v.sizeDelta;
    //        s.x = canvasWidth;
    //        v.sizeDelta = s;
    //    }
    //    camera.orthographicSize = cameraSize;
    //}

}
