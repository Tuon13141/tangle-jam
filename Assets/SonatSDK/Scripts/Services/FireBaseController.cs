#define dummy
//#define use_firebase

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sonat;
#if !((dummy || global_dummy) && !use_firebase)
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.Crashlytics;
#if !dummy2
using Firebase.Installations;
#endif
#endif
 
// ReSharper disable InconsistentNaming
public class LogParameter
{
    public int order { get; }

    //        public string log { get; private set; }

    public enum ParamType
    {
        BooleanType,
        StringType,
        IntType,
        FloatType,
    }
    public string stringValue { get; }
    public bool boolValue { get; }
    public int intValue { get; }
    public float floatValue { get; }
    public string stringKey { get; }
    public ParamType type;

#if (dummy || global_dummy) && !use_firebase
    private void CreateFirebaseParam()
    {
       
    }
#else
    public Parameter Param;

    private void CreateFirebaseParam()
    {
        switch (type)
        {
            case ParamType.BooleanType:
                Param = new Parameter(stringKey, boolValue.ToString());
                break;
            case ParamType.StringType:
                Param = new Parameter(stringKey, stringValue);
                break;
            case ParamType.IntType:
                Param = new Parameter(stringKey, intValue);
                break;
            case ParamType.FloatType:
                Param = new Parameter(stringKey, floatValue);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }
#endif

    public LogParameter(ParameterEnum name, string value, int order = 0)
    {
        this.order = order;
        type = ParamType.StringType;
        stringKey = name.ToString();
        stringValue = value;
        CreateFirebaseParam();
    }

    public LogParameter(ParameterEnum name, bool value, int order = 0)
    {
        this.order = order;
        type = ParamType.BooleanType;
        stringKey = name.ToString();
        boolValue = value;
        CreateFirebaseParam();
    }

    public LogParameter(ParameterEnum name, int value, int order = 0)
    {
        this.order = order;
        type = ParamType.IntType;
        stringKey = name.ToString();
        intValue = value;
        CreateFirebaseParam();
    }

    public LogParameter(ParameterEnum name, float value, int order = 0)
    {
        this.order = order;
        type = ParamType.FloatType;
        stringKey = name.ToString();
        floatValue = value;
        CreateFirebaseParam();
    }

    public LogParameter(string name, string value, int order = 0)
    {
        this.order = order;
        type = ParamType.StringType;
        stringKey = name;
        stringValue = value;
        CreateFirebaseParam();
    }

    public LogParameter(string name, int value, int order = 0)
    {
        this.order = order;
        type = ParamType.IntType;
        stringKey = name;
        intValue = value;
        CreateFirebaseParam();
    }

    public LogParameter(string name, bool value, int order = 0)
    {
        this.order = order;
        type = ParamType.BooleanType;
        stringKey = name;
        boolValue = value;
        CreateFirebaseParam();
    }

    public LogParameter(string name, float value, int order = 0)
    {
        this.order = order;
        type = ParamType.FloatType;
        stringKey = name;
        floatValue = value;
        CreateFirebaseParam();
    }
}


public static class FireBaseExtensions
{
}

public enum FireBaseDefaultEvent
{
    my_first_open,
    my_first_game_play,
    my_first_action,
    my_first_game_win,
}

[Serializable]
public class FireBaseDefaultSetting
{
    [SerializeField] private PlayerPrefRemoteArrayInt logLevelStart = new PlayerPrefRemoteArrayInt(
        "log_level_start_array", new[]
        {
            1, 4, 8, 10, 20,30,40,50,60,70,80,90,100
        });

    public PlayerPrefRemoteArrayInt LogLevelStart => logLevelStart;
        
    [SerializeField] private PlayerPrefRemoteArrayInt logInterstitialAdsStart = new PlayerPrefRemoteArrayInt(
        "log_interstitial_ads_start_array", new[]
        {
            1, 4, 8, 10, 20,30,40,50,60,70,80,90,100
        });

    public PlayerPrefRemoteArrayInt LogInterstitialAdsStart => logInterstitialAdsStart;

    [SerializeField] private PlayerPrefRemoteArrayInt logPaidAd = new PlayerPrefRemoteArrayInt(
        "log_paid_ad_array", new[]
        {
            0
        });

    public PlayerPrefRemoteArrayInt LogPaidAd => logPaidAd;
    
}

public class FireBaseController : BaseService
{
    public CustomGetTrackingScript customTrackingScript;

    [SerializeField] private bool waitFirstTime;
    [SerializeField] private float timeWait = 4;

    private readonly PlayerPrefInt firstTime = new PlayerPrefInt("first_time_check_remote");
    public override void ForceInit()
    {
        base.ForceInit();
        if (!firstTime.BoolValue && waitFirstTime)
        {
            firstTime.BoolValue = true;
            if (Kernel.IsInternetConnection())
            {
                Initialized = false;
                StartCoroutine(Wait());
            }
        }
    }

    private IEnumerator Wait()
    {
        float t = 0;
        while (!FireBaseRemoteReady && t < timeWait)
        {
            t += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        Initialized = true;
    }

    public static int UserPropertyLevel
    {
        get => PlayerPrefs.GetInt("user_property_level");
        private set => PlayerPrefs.SetInt("user_property_level",value);
    }
    
    public static string UserPropertyMode
    {
        get => PlayerPrefs.GetString("user_property_mode","classic");
        private set => PlayerPrefs.SetString("user_property_mode",value);
    }
    
    public bool logCtr = true;
    public string FirebaseInstanceId { get; set; }
    public string AnalyticsInstanceId { get; set; }

#if (use_firebase || !(global_dummy || dummy))
    public static DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
#endif

    public RemoteConfigController remoteConfigController;

#if (use_firebase || !(global_dummy || dummy))
    private Firebase.FirebaseApp App => FirebaseApp.DefaultInstance;

#if !dummy2
    protected FirebaseInstallations installations = null;
      protected Task GetIdAsync()
    {
        return installations.GetIdAsync().ContinueWithOnMainThread(task =>
        {
            if (LogTaskCompletion(task, "GetIdAsync"))
            {
                FirebaseInstanceId = task.Result;
                SetUserProperty("fb_instance_id", FirebaseInstanceId);
                Debug.LogFormat(String.Format("Installations {0}", task.Result));
            }
        });
    }

    protected Task GetAnalyticsInstanceIdAsync()
    {
        return FirebaseAnalytics.GetAnalyticsInstanceIdAsync().ContinueWithOnMainThread(task =>
        {
            if (LogTaskCompletion(task, "GetAnalyticsInstanceIdAsync"))
            {
                AnalyticsInstanceId = task.Result;
                Debug.LogFormat(String.Format("GetAnalyticsInstanceIdAsync {0}", task.Result));
            }
        });
    }
#endif

    bool LogTaskCompletion(Task task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            Debug.LogFormat(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            Debug.LogFormat(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string errorCode = "";
                FirebaseException firebaseException = exception as FirebaseException;
                if (firebaseException != null)
                {
                    errorCode = String.Format("Error code={0}: ",
                        firebaseException.ErrorCode.ToString(),
                        firebaseException.Message);
                }
                Debug.LogFormat(errorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            Debug.LogFormat(operation + " completed");
            complete = true;
        }
        return complete;
    }
#endif

    public static bool FireBaseRemoteReady { get; set; }
    public static bool DependencyStatusAvailable;
    protected virtual Dictionary<string, object> defaultConfig => null;

    public IEnumerable<LogParameter> GetAdmobParameter(int step, IEnumerable<LogParameter> input, AdTypeLog adType)
    {
        if (customTrackingScript == null)
            return input;
        return customTrackingScript.GetAdmobLog(step, input, adType);
    }

    protected override void Register()
    {
        base.Register();
        Initialized = true;
#if (use_firebase || !(global_dummy || dummy))
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
       
            Debug.Log("CheckAndFixDependenciesAsync " + dependencyStatus);
            if (dependencyStatus == DependencyStatus.Available)
            {
                DependencyStatusAvailable = true;
                InitializeFirebase();
                StartCoroutine(WaitToLog());

            }
            else
            {
                DependencyStatusAvailable = false;
                Debug.LogError("Could not resolve all Fire base dependencies: " + dependencyStatus);
                StartCoroutine(WaitToLog());
            }
        });

        remoteConfigController.Ready.Action += data =>
        {
            UIDebugLog.Log(nameof(FireBaseController) + " initialized");
            StartCoroutine(HandlerRemote());
            remoteConfigController.Ready.Destroy();
        };
#endif
    }

    private static IEnumerator WaitToLog()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(5);
            bool done = SonatTrackingHelper.TryToPostQueues();
            if(done)
                break;
        }
    }

    private PlayerPrefInt disableNoti = new PlayerPrefInt("disableNoti");
    public string DefaultTopic = "game";

    public bool IsNotificationDisable() => disableNoti.BoolValue;
   

    public  void EnableNotifications()
    {
        disableNoti.BoolValue = false;
#if (use_firebase_message || !(global_dummy || dummy))
        //init notifications
        Firebase.Messaging.FirebaseMessaging.DeleteTokenAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log($"FirebaseManager - Deleted Token");
            Firebase.Messaging.FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(task =>
            {
                Debug.Log($"FirebaseManager - Got New Token");
                Firebase.Messaging.FirebaseMessaging.SubscribeAsync(DefaultTopic).ContinueWithOnMainThread(task => {
                    Debug.Log($"FirebaseManager - Subscribed To Topic - {DefaultTopic}");
                });
            });
        });
#endif
    }

    public void DisableNotification()
    {
        disableNoti.BoolValue = true;
#if (use_firebase_message || !(global_dummy || dummy))
        //init notifications
        Firebase.Messaging.FirebaseMessaging.DeleteTokenAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log($"FirebaseManager - Deleted Token");
            Firebase.Messaging.FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(task =>
            {
                Debug.Log($"FirebaseManager - Got New Token");
                Firebase.Messaging.FirebaseMessaging.UnsubscribeAsync(DefaultTopic).ContinueWithOnMainThread(task => {
                    Debug.Log($"FirebaseManager - UnSubscribed To Topic - {DefaultTopic}");
                });
            });
        });
#endif
    }


#if (use_firebase_message || !(global_dummy || dummy))
    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
        if (disableNoti.BoolValue)
            DisableNotification();
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    }
#endif

    public BooleanAction RemoteConfigReady => remoteConfigController.Ready;

    private IEnumerator HandlerRemote()
    {
        yield return null;
        OnRemoteFetched();
    }

    protected virtual void OnRemoteFetched()
    {
    }

    protected virtual void InitializeFirebase()
    {
#if (use_firebase || !(global_dummy || dummy))
        InitFireBaseAnalytics();
        SonatAnalyticTracker.FirebaseReady = true;
        remoteConfigController.InitRemoteConfig(defaultConfig);
    #if !dummy2
        installations = FirebaseInstallations.DefaultInstance;
        GetIdAsync();
        GetAnalyticsInstanceIdAsync();
#endif
#endif

#if (use_firebase_message || !(global_dummy || dummy))
        //        Debug.LogError("Init Firebase Message");
        if (!disableNoti.BoolValue)
        {

            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        }
#endif

    }

    public void SetUserProperty(string property, string value)
    {
#if (use_firebase || !(global_dummy || dummy))
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event" + Time.time);
            return;
        }
        FirebaseAnalytics.SetUserProperty(property, value);
#endif
    }

  

    #region FireBase Analytics

    private void InitFireBaseAnalytics()
    {
#if (use_firebase || !(global_dummy || dummy))
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        LogEvent(EventNameEnum.app_loading);
        if (PlayerPrefs.GetInt(FireBaseDefaultEvent.my_first_open.ToString()) == 0)
        {
            LogEvent(FireBaseDefaultEvent.my_first_open.ToString());
            PlayerPrefs.SetInt(FireBaseDefaultEvent.my_first_open.ToString(), 1);
        }
#endif
    }

    private void LogEventName(string eventName)
    {
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event" + Time.time);
            return;
        }
#if (use_firebase || !(global_dummy || dummy))
        FirebaseAnalytics.LogEvent(eventName);
#endif
    }

    public void LogEvent(string eventName, string paramName, string paramValue)
    {
        if (!DependencyStatusAvailable)
        {
#if use_firebase
            Debug.LogError("Firebase Not ready to log event " + Time.time);
#endif
            return;
        }
#if (use_firebase || !(global_dummy || dummy))
        FirebaseAnalytics.LogEvent(eventName, new Parameter(paramName, paramValue));
#endif
    }

#if (use_firebase || !(global_dummy || dummy))
    private void LogEvent(string eventName, Parameter[] parameters)
    {
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event" + Time.time);
            return;
        }
        FirebaseAnalytics.LogEvent(eventName, parameters);
    }

    private void LogEvent(string eventName, Parameter parameter)
    {
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event" + Time.time);
            return;
        }
        FirebaseAnalytics.LogEvent(eventName, parameter);
    }
#endif

    public void LogEvent(string eventName, string paramName, int paramValue)
    {
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event" + Time.time);
            return;
        }
#if (use_firebase || !(global_dummy || dummy))
        FirebaseAnalytics.LogEvent(eventName, new Parameter(paramName, paramValue));
#endif
    }


    public void LogEvent(string eventName, string paramName, float paramValue)
    {
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event" + Time.time);
            return;
        }
#if (use_firebase || !(global_dummy || dummy))
        FirebaseAnalytics.LogEvent(eventName, new Parameter(paramName, paramValue));
#endif
    }


    public void LogEvent(EventNameEnum eventName, LogParameter[] parameters)
    {
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event" + Time.time);
            return;
        }

#if (use_firebase || !(global_dummy || dummy))
        if (!DependencyStatusAvailable) return;
        //if (_isTestDevice)
        {
            Parameter[] fbParameters = new Parameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                fbParameters[i] = parameters[i].Param;
            LogEvent(eventName.ToString(), fbParameters);
        }
#endif
    }

    public void SetScreenView(string screenName, string screenClass)
    {
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event" + Time.time);
            return;
        }
#if (use_firebase || !(global_dummy || dummy))
        if (!DependencyStatusAvailable) return;
        FirebaseAnalytics.LogEvent(
            FirebaseAnalytics.EventScreenView,
            new Parameter(FirebaseAnalytics.ParameterScreenName, screenName),
            new Parameter(FirebaseAnalytics.ParameterScreenClass, screenClass));
#endif
    }

    public void SetUserLevel(int level,string mode = "classic")
    {
        UserPropertyLevel = level;
        UserPropertyMode = mode;
        
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event" + Time.time);
            return;
        }
#if (use_firebase || !(global_dummy || dummy))
        if (!DependencyStatusAvailable) return;
            FirebaseAnalytics.SetUserProperty("level", level.ToString());
#endif
    }

    public void EventEarnVirtualCurrency(string currency, int amount, string placement)
    {
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event" + Time.time);
            return;
        }
#if (use_firebase || !(global_dummy || dummy))
        if (!DependencyStatusAvailable) return;
        FirebaseAnalytics.LogEvent(
            FirebaseAnalytics.EventEarnVirtualCurrency,
            new Parameter(FirebaseAnalytics.ParameterValue, amount),
            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, currency),
            new Parameter("placement", placement));
#endif

#if UNITY_EDITOR
        Debug.Log(nameof(EventEarnVirtualCurrency) + "/" + currency + "/" + amount);
#endif
    }

    public void EventSpendVirtualCurrency(string itemName, int amount, string currency, string placement = "")
    {
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event" + Time.time);
            return;
        }
#if (use_firebase || !(global_dummy || dummy))
        if (!DependencyStatusAvailable) return;
        FirebaseAnalytics.LogEvent(
            FirebaseAnalytics.EventSpendVirtualCurrency,
            new Parameter(FirebaseAnalytics.ParameterItemName, itemName),
            new Parameter(FirebaseAnalytics.ParameterValue, amount),
            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, currency),
            new Parameter("placement", placement));
#endif

#if UNITY_EDITOR
        Debug.Log(nameof(EventSpendVirtualCurrency) + ":" + itemName + "/" + currency + "/" + amount);
#endif
    }

    public void EventLevelUp(string character, int level)
    {
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event" + Time.time);
            return;
        }
#if (use_firebase || !(global_dummy || dummy))
        if (!DependencyStatusAvailable) return;
        FirebaseAnalytics.LogEvent(
            FirebaseAnalytics.EventLevelUp,
            new Parameter(FirebaseAnalytics.ParameterCharacter, character),
            new Parameter(FirebaseAnalytics.ParameterLevel, level));
#endif
    }


    public void LogEvent(string eventName, LogParameter logParameter)
    {
//        if (!DependencyStatusAvailable)
//        {
//            Debug.LogError("Firebase Not ready to log event" + Time.time);
//            return;
//        }
#if (use_firebase || !(global_dummy || dummy))
        if (!DependencyStatusAvailable) return;
        // if (_isTestDevice)
        LogEvent(eventName, logParameter.Param);
#endif
    }

    public void LogEvent<T>(string eventName, IList<T> parameters) where T : LogParameter
    {
        if (!DependencyStatusAvailable)
        {
            #if !UNITY_EDITOR && use_firebase
            Debug.LogError("Firebase Not ready to log event" + Time.time);
            #endif
            return;
        }
#if (use_firebase || !(global_dummy || dummy))
        if (!DependencyStatusAvailable) return;
        LogEvent(eventName, parameters.Select(x => x.Param).ToArray());
#endif
    }


    public void LogEvent(string eventName)
    {
        #if !use_firebase
        return;
        #endif
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event " + Time.time);
            return;
        }

        LogEventName(eventName);
    }

    public void LogEvent(EventNameEnum eventName)
    {
#if !use_firebase
        return;
#endif
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event " + Time.time);
            return;
        }
        LogEvent(eventName.ToString());
    }

    public void LogCrash(string mess)
    {
#if !use_firebase
        return;
#else 
        Crashlytics.Log(mess);
#endif
    }
    
    public void LogCrashException(Exception mess)
    {
#if !use_firebase
        return;
#else 
        Crashlytics.LogException(mess);
#endif
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="platform"> admob, max app lovin or iron source</param>
    /// <param name="adNetwork"></param>
    /// <param name="revenue">revenue in USD not microUSD</param>
    /// <param name="precision">float string</param>
    /// <param name="adType">banner, inter, video</param>
    /// <param name="currencyCode">usd maybe</param>
    public void LogFirebaseRevenue(AdsPlatform platform, string adNetwork, double revenue, string precision,
        string adType, string currencyCode = "USD")
    {
        if (!DependencyStatusAvailable)
        {
            Debug.LogError("Firebase Not ready to log event" + Time.time);
            return;
        }

        string placement_name = "";
        if (adType == "banner")
            placement_name = "banner";
        if (adType == AdTypeLog.interstitial.ToString() || adType == AdTypeLog.app_open.ToString())
            placement_name = SonatAnalyticTracker.InterstitialLogName;
        if (adType == AdTypeLog.rewarded.ToString())
            placement_name = SonatAnalyticTracker.RewardedLogPlacement;
        
#if ((use_firebase || !(global_dummy || dummy)) && !dummy2)
        SonatAnalyticTracker.LogFirebaseRevenue(platform, adNetwork, revenue, precision, adType,Kernel.Resolve<FireBaseController>().FirebaseInstanceId, placement_name,currencyCode);
#endif
    }


    #endregion
}

