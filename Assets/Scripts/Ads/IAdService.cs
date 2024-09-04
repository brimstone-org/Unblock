using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAdService : MonoBehaviour {

    public abstract void Initialize();
    public abstract void Initialize(string[] args);
    public abstract bool ShowAds();

    public event System.Action onAdClosed;

    protected void TriggerOnAdClosed() {
        if (onAdClosed != null)
            onAdClosed();
    }


}
