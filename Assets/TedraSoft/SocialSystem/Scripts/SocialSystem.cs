using UnityEngine;
using System.Collections;

namespace TedrasoftSocial
{
    /// <summary>
    /// TedraSoft Social System.
    /// </summary>
	public class SocialSystem : MonoBehaviour
	{
        /// <summary>
        /// Singleton design instance.
        /// </summary>
        /// <value>The instance.</value>
		public static SocialSystem Instance { get; private set; }

        /// <summary>
        /// The name of the android package.
        /// </summary>
		public string androidPackageName = "";

        /// <summary>
        /// The iOS AppId.
        /// </summary>
		public string iOSAppId = "";

        /// <summary>
        /// Android link to share.
        /// </summary>
		private string shareLinkAndroid = "https://play.google.com/store/apps/details?id=";

        /// <summary>
        /// iOS link to share.
        /// </summary>
		private string shareLinkiOS = "https://itunes.apple.com/app/id";

        /// <summary>
        /// Others links to share.
        /// </summary>
		public string shareLinkOther = "";

        /// <summary>
        /// Email of the developer.
        /// </summary>
		public string email = "office@tedrasoft.com";

        /// <summary>
        /// Normal rate text.
        /// </summary>
		public string subjectNormal = "Normal rate";

        /// <summary>
        /// Bad rate text.
        /// </summary>
		public string subjectBad = "Bad rate";

        /// <summary>
        /// The rate canvas on Android.
        /// </summary>
		public Transform rateCanvasAndroid;

        /// <summary>
        /// The rate canvas for iOS.
        /// </summary>
		public Transform rateCanvasIOS;

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:TedrasoftSocial.SocialSystem"/> rate is in progress.
        /// </summary>
        /// <value><c>true</c> if rate is in progress; otherwise, <c>false</c>.</value>
		public bool RateInProgress{ get; private set; }

		void Awake ()
		{
			if (Instance == null) {
				Instance = this;
				DontDestroyOnLoad (this.gameObject);
			} else if (Instance != this) {
				Destroy (this.gameObject);
			}
		}

		// Use this for initialization
		void Start ()
		{
			shareLinkAndroid += androidPackageName;
			shareLinkiOS += iOSAppId;
		}

        /// <summary>
        /// Returns the share link fpr the specific platform.
        /// </summary>
        /// <returns>The share link.</returns>
        public string GetShareLink()
        {
#if UNITY_ANDROID
            return shareLinkAndroid;
#elif UNITY_IOS
            return shareLinkiOS;
#endif
        }

        /// <summary>
        /// Pops up a rate panel that checks if user already rated. Don't call this for the button. Call this only for random rate PopUps.
        /// </summary>
        public void RatePopUp ()
		{
			if (PlayerPrefs.GetInt ("Rated") == 1)
				return;
			OpenRatePanel ();
		}

        /// <summary>
        /// Opens the rate panel. Use this on the button so that the player is able to rate again (change the rating).
        /// </summary>
		public void OpenRatePanel ()
		{
			RateInProgress = true;
			#if UNITY_ANDROID
			rateCanvasAndroid.gameObject.SetActive (true);
			#elif UNITY_IOS
			rateCanvasIOS.gameObject.SetActive(true);
			#endif
		}

        /// <summary>
        /// Closes the rate panel.
        /// </summary>
		public void CloseRatePanel ()
		{
			#if UNITY_ANDROID
			rateCanvasAndroid.gameObject.SetActive (false);
			#elif UNITY_IOS
			rateCanvasIOS.gameObject.SetActive(false);
			#endif

			StartCoroutine (DelayedState (1));

		}

        /// <summary>
        /// Handles good rating.
        /// </summary>
		public void LikeButtonClick ()
		{

			PlayerPrefs.SetInt ("Rated", 1);

			#if UNITY_ANDROID
			Application.OpenURL (shareLinkAndroid);
			#elif UNITY_IOS
			Application.OpenURL(shareLinkiOS);
			#else
			Application.OpenURL(iOSAppId);
			#endif
			CloseRatePanel ();
		}

        /// <summary>
        /// Delays the rate in progress.
        /// </summary>
        /// <returns>The state.</returns>
        /// <param name="seconds">Seconds.</param>
		IEnumerator DelayedState (float seconds)
		{
			yield return new WaitForSeconds (seconds);

			RateInProgress = false;
		}

        /// <summary>
        /// Handles normal rating.
        /// </summary>
		public void NormalButtonClick ()
		{
			PlayerPrefs.SetInt ("Rated", 1);
			SendEmail (email, subjectNormal);
			CloseRatePanel ();
		}

        /// <summary>
        /// Handles bad rating.
        /// </summary>
		public void BadButtonClick ()
		{
			PlayerPrefs.SetInt ("Rated", 1);
			SendEmail (email, subjectBad);
			CloseRatePanel ();
		}

        /// <summary>
        /// Sends an e-mail to the developer.
        /// </summary>
        /// <param name="email">Email.</param>
        /// <param name="subject">Subject.</param>
		public void SendEmail (string email, string subject)
		{
			subject = MyEscapeURL (Application.productName + " " + subject);
			Application.OpenURL ("mailto:" + email + "?subject=" + subject);
		}

        /// <summary>
        /// Escapes an URL.
        /// </summary>
        /// <returns>The escaped URL.</returns>
        /// <param name="url">URL to be escaped.</param>
		string MyEscapeURL (string url)
		{
			return WWW.EscapeURL (url).Replace ("+", "%20");
		}

	}
}
