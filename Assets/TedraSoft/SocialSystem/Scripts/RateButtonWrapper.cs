using UnityEngine;
using System.Collections;

namespace TedrasoftSocial
{
    /// <summary>
    /// Rate button wrapper.
    /// </summary>
	public class RateButtonWrapper : MonoBehaviour
	{

		public void OnClick ()
		{
			if (SocialSystem.Instance != null)
				SocialSystem.Instance.OpenRatePanel ();
		}
	}
}