using UnityEngine;
using System.Collections;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

/// <summary>
/// Handles achievement board popup.
/// </summary>
public class AchievementsButton : MonoBehaviour {

	public void OnClick(){
		Social.ShowAchievementsUI ();
	}
}
