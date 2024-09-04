using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

/// <summary>
/// Google Play Games Manager. Handles GPGS Initialization or GameCenter's in case of using iOS.
/// </summary>
public class GPGSManager : MonoBehaviour {

	public static GPGSManager Instance { get; private set; }

	// Use this for initialization
	void Awake () {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
		}
	}

	void Start(){
		#if UNITY_ANDROID && !UNITY_AMAZON
		PlayGamesPlatform.Activate();
#elif UNITY_ANDROID && UNITY_AMAZON
        bool usesLeaderboards = false;
        bool usesAchievements = true;
        bool usesWhispersync = false;

        AGSClient.Init (usesLeaderboards, usesAchievements, usesWhispersync);
#elif UNITY_IOS
		GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
#endif
        Social.localUser.Authenticate((bool success) => {
			// handle success or failure
		});
	}
	
        /// <summary>
        /// Unlocks the achievement.
        /// </summary>
        /// <param name="achievement">Achievement.</param>
	public void UnlockAchievement(string achievement){
		Social.ReportProgress(achievement, 100.0f, (bool success) => {
			// handle success or failure
		});
	}

}
