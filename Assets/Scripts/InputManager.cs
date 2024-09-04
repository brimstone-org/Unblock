using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public static bool IsFaking { get; set; }
    private static Vector3 mPosition;
    public static Vector3 mousePosition { get {
            if (IsFaking)
                return mPosition;
            return Input.mousePosition;
        }
        set {
            mPosition = value;
        }
    }

    public static bool[] MouseButtons = new bool[10];

    public static bool GetMouseButton(int index)
    {
        if (IsFaking)
        {
            return MouseButtons[index];
        }

        return Input.GetMouseButton(index);
    }


}
