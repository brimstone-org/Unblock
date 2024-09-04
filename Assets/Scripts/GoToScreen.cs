using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToScreen : MonoBehaviour {
    [SerializeField]
    /// <summary>
    /// Holds the current GameObject which usually refeers to a Canvas.
    /// </summary>
    GameObject ThisScreen;

    [SerializeField]
    /// <summary>
    /// List of GameObjects (Canvases) to disable after the transition. 
    /// </summary>
    List<GameObject> ExtraScreensToHide;

    [SerializeField]
    /// <summary>
    /// List of object(s) to enable
    /// </summary>
    List<GameObject> Targets;

    [SerializeField]
    /// <summary>
    /// Reference to Fade class used to do te actual fading.
    /// </summary>
    Fade FadeScreen;

    /// <summary>
    /// This method wraps up anything that had to be done at the end of a fade. Handles both fadeIn and fadeOut depending on the parameter.
    /// </summary>
    /// <param name="fadeIn">If set to <c>true</c>, the fade was a fadeIn, otherwise false->fadeOut.</param>
    void FadeFinished(bool fadeIn)
    {
        if (!fadeIn)
        {
            // fade out finished,
            // switch to target
            foreach (var target in Targets)
            {
                target.SetActive(true);
            }
            ThisScreen.SetActive(false);

            foreach (var toHide in ExtraScreensToHide)
            {
                toHide.SetActive(false);
            }

            if ( FadeScreen != null )
            {
                FadeScreen.OnFadeFinished -= FadeFinished;
            }
        }
    }

    /// <summary>
    /// Starts the fade.
    /// </summary>
    public void Go()
    {
        if ( FadeScreen != null )
        {
            FadeScreen.OnFadeFinished += FadeFinished;
        }

        if (FadeScreen == null)
        {
            foreach (var target in Targets)
            {
                target.SetActive(true);
            }
            ThisScreen.SetActive(false);

            foreach (var toHide in ExtraScreensToHide)
            {
                toHide.SetActive(false);
            }
        }
        else
        {
            FadeScreen.StartFadeOutIn();
        }
    }
}
