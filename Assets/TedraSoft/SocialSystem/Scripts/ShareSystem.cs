using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TedrasoftSocial
{
    /// <summary>
    /// Share system.
    /// </summary>
    public class ShareSystem : MonoBehaviour
    {
        /// <summary>
        /// Shares the game on social platforms depending on player input.
        /// </summary>
        public void ShareGame()
        {
            ShareText(Application.productName, SocialSystem.Instance.GetShareLink());
        }

        /// <summary>
        /// Share method accepting the subject and the body of a message to share.
        /// </summary>
        /// <param name="subject">Subject.</param>
        /// <param name="body">Body.</param>
        public void ShareText(string subject, string body)
        {
#if UNITY_ANDROID
            ShareAndroid(subject, body);
#endif
        }

        /// <summary>
        /// Share method on Android.
        /// </summary>
        /// <param name="subject">Subject.</param>
        /// <param name="body">Body.</param>
        private void ShareAndroid(string subject, string body)
        {
            //execute the below lines if being run on a Android device
#if UNITY_ANDROID
            //Reference of AndroidJavaClass class for intent
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            //Reference of AndroidJavaObject class for intent
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            //call setAction method of the Intent object created
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            //set the type of sharing that is happening
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            //add data to be passed to the other activity i.e., the data to be sent
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body);
            //get the current activity
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            //start the activity by sending the intent data
            AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, subject);
            currentActivity.Call("startActivity", jChooser);
#endif
        }
    }
}