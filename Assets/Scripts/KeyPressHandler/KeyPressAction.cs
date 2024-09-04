using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyPressAction : MonoBehaviour {

    [SerializeField]
    KeyPressHandler handler;

    [SerializeField]
    UnityEvent onBack;

    private void OnBack()
    {
        if(onBack != null)
            onBack.Invoke();
    }

    private void OnEnable()
    {
        handler.PushAction(OnBack);
    }

    private void OnDisable()
    {
        handler.RemoveAction(OnBack);
    }

}
