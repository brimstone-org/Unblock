using System.Collections;
using System.Collections.Generic;
using Firebase.Analytics;
using UnityEngine;
//using Firebase.Analytics;

public class FirebaseAnalyticsListener : MonoBehaviour
{
    public static FirebaseAnalyticsListener instance { get; set; }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
    }


    void OnEnable()
    {
       // GameManager.OnLevelWin += FLogWon;
    }


    void OnDestroy()
    {
      //  GameManager.OnLevelWin -= FLogWon;
    }

    public void FLogWon(int level, string difficulty)
    {
        Parameter[] paras = {
            new Parameter(FirebaseAnalytics.ParameterLevel,level),
            new Parameter(FirebaseAnalytics.ParameterTravelClass, difficulty)
        };

        FirebaseAnalyticsWrapper.Instance.LogEvent(FirebaseAnalytics.EventPostScore, paras);
    }
}
