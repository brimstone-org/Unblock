using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour {
    /// <summary>
    /// Fade finished delegate.
    /// </summary>
    public delegate void FadeFinishedDelegate(bool fadeIn);

    /// <summary>
    /// Delegate to execute logic after the fade finished.
    /// </summary>
    public FadeFinishedDelegate OnFadeFinished;

    [SerializeField]
    /// <summary>
    /// Fade image to be used. Can be any Image.
    /// </summary>
    Image FadeImage = null;

    /// <summary>
    /// The duration of the fade, in seconds.
    /// </summary>
    [SerializeField]
    float FadeDuration = 1f;


    /// <summary>
    /// Should the fade begin when the object initializes?
    /// </summary>
    [SerializeField]
    bool AutoStart = true;

    /// <summary>
    /// Did the fade started?
    /// </summary>
    private bool m_started = false;

    /// <summary>
    /// The total time for which the fade ran. Used to do the fade over a period of time.
    /// </summary>
    private float m_time = 0f;

    /// <summary>
    /// Is this a Fade In or a Fade Out?
    /// </summary>
    private bool m_fadeIn = true;


    /// <summary>
    /// At the start of this script we check if we should begin to fade.
    /// </summary>
    void Start()
    {
        if (AutoStart)
        {
            StartFade(true);
        }
    }


    /// <summary>
    /// This method will start the fade, accepts one param that indicates if it's a Fade In or a Fade Out.
    /// </summary>
    /// <param name="fadeIn">If set to <c>true</c> fade in, else, fade out.</param>
    public void StartFade(bool fadeIn)
    {
        if (!m_started)
        {
            m_fadeIn = fadeIn;
            m_started = true;
            m_time = 0f;
            gameObject.SetActive(true);
            StartCoroutine(FadeCoroutine());
        }
    }

    bool m_isFadeOutIn = false;

    public void StartFadeOutIn()
    {
        m_isFadeOutIn = true;
        StartFade(false);
    }

    /// <summary>
    /// This method is called when a fade (In or out) has finished and determines the next action.
    /// </summary>
    /// <param name="fadeIn">If set to <c>true</c> fade in.</param>
    void FadeFinished(bool fadeIn)
    {
        // if fade out finished and
        // a fadeoutin is in progress
        // start the fade in half
        if (!fadeIn && m_isFadeOutIn)
        {
            m_started = false;
            StartFade(true);
        }
        else if (m_isFadeOutIn && fadeIn)
        {
            // everything finished
            m_isFadeOutIn = false;
            m_started = false;
            gameObject.SetActive(false);
        }
        else if (!m_isFadeOutIn)
        {
            m_started = false;
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Does the actual fade.
    /// </summary>
    IEnumerator FadeCoroutine()
    {
        do
        {
            m_time += Time.unscaledDeltaTime;
            float opacity = Mathf.Clamp01(Mathf.InverseLerp(0, FadeDuration, m_time));
            if (m_fadeIn)
            {
                opacity = 1f - opacity; // invert the fade
            }
            FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, opacity);
            yield return new WaitForEndOfFrame();
        } while (m_time <= FadeDuration);

        if (OnFadeFinished != null)
        {
            OnFadeFinished(m_fadeIn);
        }

        FadeFinished(m_fadeIn);
    }
}
