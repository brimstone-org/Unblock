using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Scripts that controls the stars shown.
/// </summary>
public class StarsControl : MonoBehaviour {

	[SerializeField]
    /// <summary>
    /// Holds all the stars to be changed.
    /// </summary>
    List<Image> Stars;

    [SerializeField]
    /// <summary>
    /// Holds the sprite for a filled star.
    /// </summary>
    Sprite StarSprite;

    [SerializeField]
    /// <summary>
    /// Holds the sprite for an empty star.
    /// </summary>
    Sprite StarSlotSprite;

    /// <summary>
    /// Fills the number of stars in the list with the number passed as parameters. The number of the stars becomes the fill sprite, the rest is the slot sprite.
    /// </summary>
    /// <param name="count">Number of stars to be set as filled.</param>
    public void SetStars(int count)
    {
        int boundCheckCount = Mathf.Min(count, Stars.Count);
        int i = 0;
        for (; i < boundCheckCount; ++i)
        {
            Stars[i].sprite = StarSprite;
        }

        for (; i < Stars.Count; ++i)
        {
            Stars[i].sprite = StarSlotSprite;
        }
    }
}
