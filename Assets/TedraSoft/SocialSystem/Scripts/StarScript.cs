using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Tedrasoft_Rate
{
    /// <summary>
    /// Star script that handles the stars within the rating dialog.
    /// </summary>
	public class StarScript : MonoBehaviour
	{
		[SerializeField]
        /// <summary>
        /// Actual star array.
        /// </summary>
		Image[] stars;

		[SerializeField]
        /// <summary>
        /// Holds the inspector-picked color for the pressed stars.
        /// </summary>
		Color pressedColor;

		[SerializeField]
        /// <summary>
        /// Holds the inspector-picked color for normal stars (unchecked).
        /// </summary>
		Color normalColor;

        /// <summary>
        /// Handles drag-rating.
        /// </summary>
		public void OnMouseOverStart(){
			for (int i = 0; i < stars.Length; i++) {
				stars [i].color = pressedColor;
			}
		}

        /// <summary>
        /// Handles end of drag-rating.
        /// </summary>
		public void OnMouseOverEnd(){
			for (int i = 0; i < stars.Length; i++) {
				stars [i].color = normalColor;
			}
		}

	}
}
