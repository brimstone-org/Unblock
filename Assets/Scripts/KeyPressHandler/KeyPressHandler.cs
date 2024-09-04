using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPressHandler : MonoBehaviour {

    [SerializeField]
    KeyCode toHandle;

    List<System.Action> list = new List<System.Action>();

    public void PushAction(System.Action action)
    {
        list.Add(action);
    }

    public void RemoveAction(System.Action action)
    {
        if (list.Contains(action))
            list.Remove(action);
    }

    private void DoAction()
    {
        if (list.Count == 0)
            return;

        var action = list[list.Count - 1];

        action();
    }

    private void Update()
    {
        if (Input.GetKeyUp(toHandle))
        {
            DoAction();
        }
    }

}
