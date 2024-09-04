using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Analytics;
using Firebase;
using Firebase.Extensions;

public class FirebaseAnalyticsWrapper : MonoBehaviour
{

    public static FirebaseAnalyticsWrapper Instance { get; private set; }
    // public Text textToken;

    // Use this for initialization
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.Log(
                    "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private string topic = "/topics/all";
    // Log the result of the specified task, returning true if the task
    // completed successfully, false otherwise.
    protected bool LogTaskCompletion(Task task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            Debug.Log(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            Debug.Log(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string errorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    errorCode = string.Format("Error.{0}: ",
                        ((Firebase.Messaging.Error)firebaseEx.ErrorCode).ToString());
                }
                Debug.Log(errorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            Debug.Log(operation + " completed");
            complete = true;
        }
        return complete;
    }
    void InitializeFirebase()
    {
        Debug.Log("Firebase started");
        // FirebaseAnalytics.LogEvent("start_game");
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
        Firebase.Messaging.FirebaseMessaging.SubscribeAsync(topic).ContinueWithOnMainThread(task => {
            LogTaskCompletion(task, "SubscribeAsync");
            CreateChannel("0", "UnblockChannel", "This is the Unblock notifications channel", 0);
        });
        Debug.Log("Firebase Messaging Initialized");

        // This will display the prompt to request permission to receive
        // notifications if the prompt has not already been displayed before. (If
        // the user already responded to the prompt, thier decision is cached by
        // the OS and can be changed in the OS settings).
        Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(
            task => {
                LogTaskCompletion(task, "RequestPermissionAsync");

            }
        );
    }

    private void CreateChannel(string id, string name, string desc, int importance = 3)//3:default importance level
    {
        var SDK_INT = 0;
        using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            SDK_INT = version.GetStatic<int>("SDK_INT");
            Debug.Log("the sdk is under 26");
        }
        if (SDK_INT >= 26)
        {
            Debug.Log("the sdk is over 26");
            using (var NotificationChannel = new AndroidJavaObject("android.app.NotificationChannel", id, name, importance))
            using (var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var context = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var notificationManager = context.Call<AndroidJavaObject>("getSystemService", "notification"))
            {
                NotificationChannel.Call("setDescription", desc);
                notificationManager.Call("createNotificationChannel", NotificationChannel);

            }

        }
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
        //textToken.text = token.Token;
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
       // Debug.Log("Received a new message");
        var notification = e.Message.Notification;
        if (notification != null)
        {
            Debug.Log("title: " + notification.Title);
            Debug.Log("body: " + notification.Body);
            var android = notification.Android;
            if (android != null)
            {
                Debug.Log("android channel_id: " + android.ChannelId);
            }
        }
        if (e.Message.From.Length > 0)
            Debug.Log("from: " + e.Message.From);
        if (e.Message.Link != null)
        {
            Debug.Log("link: " + e.Message.Link.ToString());
        }
        if (e.Message.Data.Count > 0)
        {
            Debug.Log("data:");
            foreach (System.Collections.Generic.KeyValuePair<string, string> iter in
                e.Message.Data)
            {
                Debug.Log("  " + iter.Key + ": " + iter.Value);
            }
        }
    }

    //public void LogString(string text)
    //{
    //    FirebaseAnalytics.LogEvent(text);
    //}
    //public void LogEvent(string StringEvent, Parameter[] allparams)
    //{

    //    FirebaseAnalytics.LogEvent(StringEvent, allparams);

    //}

    void OnDisable()
    {
        Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
    }

    public void LogString(string text)
    {
        FirebaseAnalytics.LogEvent(text);
    }
    public void LogEvent(string StringEvent, Parameter[] allparams)
    {

        FirebaseAnalytics.LogEvent(StringEvent, allparams);

    }

}
