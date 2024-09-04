using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The sound manager class. This handles all the sounds.
/// </summary>
public class SoundManager : MonoBehaviour {

    /// <summary>
    /// Occurs when the sound button is pressed.
    /// </summary>
    public static event System.Action OnSoundTrigger;

    /// <summary>
    /// Singleton design instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SoundManager Instance { get; private set; }

    [SerializeField]
    /// <summary>
    /// Is the sound on?
    /// </summary>
    private bool soundOn;

    [SerializeField]
    /// <summary>
    /// Audio source for button click action.
    /// </summary>
    AudioSource buttonClick;

    [SerializeField]
    /// <summary>
    /// Audio source for the background music.
    /// </summary>
    AudioSource backgroundMusic;

    [SerializeField]
    /// <summary>
    /// Audio source for dragging a tile.
    /// </summary>
    AudioSource dragBlock;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:SoundManager"/> sound on.
    /// </summary>
    /// <value><c>true</c> if sound on; otherwise, <c>false</c>.</value>
    public bool SoundOn
    {
        get {
            return soundOn;
        }
        private set {
            soundOn = value;
            SoundUpdate();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    // Use this for initialization
    void Start () {
        SoundOn = PlayerPrefs.GetInt(Constants.SoundPrefSettings, 1) == 1 ? true : false;

	}

    /// <summary>
    /// Triggers the sound state.
    /// </summary>
    public void SoundTrigger()
    {
        SoundOn = !SoundOn;
    }

    void SoundUpdate()
    {
        int sound = soundOn ? 1 : 0;
        PlayerPrefs.SetInt(Constants.SoundPrefSettings, sound);

        if (soundOn)
            backgroundMusic.Play();
        else
            backgroundMusic.Stop();

        if (OnSoundTrigger != null)
            OnSoundTrigger();
    }

    /// <summary>
    /// Plays the sound corresponding to button click.
    /// </summary>
    public void PlayButtonClick()
    {
        if (!SoundOn)
            return;

        buttonClick.Play();
    }

    /// <summary>
    /// Plays the sound corresponding to tile drag.
    /// </summary>
    public void PlayDragBlock()
    {
        if (!SoundOn)
            return;

        dragBlock.Play();
    }


}
