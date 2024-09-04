using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The actual sound button handler. This handles the On/Off sprite changing.
/// </summary>
[RequireComponent(typeof(Button))]
public class SoundButton : MonoBehaviour {

    [SerializeField]
    /// <summary>
    /// The actual button object.
    /// </summary>
    Button button;

    [SerializeField]
    /// <summary>
    /// Sprite used when the sound is on.
    /// </summary>
    Sprite onSprite;

    [SerializeField]
    /// <summary>
    /// Sprite used when the sound is off.
    /// </summary>
    Sprite offSprite;

    private void OnEnable()
    {
        SoundManager.OnSoundTrigger += UpdateSprite;
    }

    private void OnDisable()
    {
        SoundManager.OnSoundTrigger -= UpdateSprite;
    }

    public void Start(){
        UpdateSprite();
    }

    /// <summary>
    /// Checks for the sound state and updates the sprite accordingly.
    /// </summary>
    private void UpdateSprite()
    {
        if (SoundManager.Instance.SoundOn)
            button.image.sprite = onSprite;
        else
            button.image.sprite = offSprite;
    }
}
