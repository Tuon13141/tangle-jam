using SonatSdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
// ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
// █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

public enum DeviceSegment
{
    low_end,
    mid_range_low,
    mid_range_high,
    high_end,
}

[DefaultExecutionOrder(-100)]
public class Kernel : MonoBehaviour
{
    public static Kernel kernel { get; set; }
    public static BaseAction KernelLoaded = new BaseAction();
    [SerializeField] private FireBaseController fireBase;
    private static readonly Dictionary<Type, BaseService> Services = new Dictionary<Type, BaseService>();

    protected static readonly Dictionary<Type, ScriptableObject> Dict = new Dictionary<Type, ScriptableObject>();

    protected static readonly Dictionary<Type, BaseRemoteDatabaseScript> RemoteDict =
        new Dictionary<Type, BaseRemoteDatabaseScript>();

    // because Kernel cannot resolve
    [SerializeField] protected ScriptableObject[] databases;
    [SerializeField] protected BaseRemoteDatabaseScript[] remoteDatabases;

#if UNITY_EDITOR
    public ScriptableObject[] DatabasesForEditorWindow => databases;
#endif

    public static Action OnInternetDisconnected;
    public static Action<bool> NeedPauseGame;

    public bool autoCheckInternet;

    private static bool _ready;
    public static bool IsReady() => _ready;

    public static PlayerPrefInt appOpenCount;

#if use_firebase_auth
    public bool autoSignInFirebaseAuth = true;
#endif

    // to create sonat log for compulsory parameter for show ads, ex : block new has 2 type of mode
    [SerializeField] private GameObject parameterValueRetriever;
    public static ILogParameterValueRetriever ParameterValueRetriever { get; set; }

//    public readonly static BaseAction OnKernelReady = new BaseAction();

    public static T GetDatabase<T>() where T : ScriptableObject
    {
        var typeT = typeof(T);
        if (RemoteDict.ContainsKey(typeT) && RemoteDict[typeT].GetDatabase() != null)
            return (T) RemoteDict[typeof(T)].GetDatabase();

        if (Dict.ContainsKey(typeT))
            return (T) Dict[typeof(T)];

        return null;
    }
    
    public T FindDatabase<T>() where T : ScriptableObject
    {
        foreach (var scriptableObject in databases)
        {
            if (scriptableObject is T)
                return (T) scriptableObject;
        }
        return null;
    }

    public static T Resolve<T>() where T : BaseService
    {
        if (Services.ContainsKey(typeof(T)))
            return (T) Services[typeof(T)];
        return ResolveDirty<T>();
    }

    public static T ResolveDirty<T>() where T : BaseService
    {
        foreach (var keyValuePair in Services)
        {
            if (keyValuePair.Key.IsAssignableFrom(typeof(T)))
                return keyValuePair.Value as T;
        }

        return null;
    }

    public virtual void Awake()
    {
        kernel = this;
        DontDestroyOnLoad(this);
        _ready = false;
        appOpenCount = new PlayerPrefInt("SONAT_APP_OPEN_COUNT", 0);
        appOpenCount.Value++;
        var scripts = GetComponents<OnServiceLoadingScript>();
        foreach (var script in scripts)
            script.Register();
        
        RegisterDatabase();
        RegisterRemoteDatabase();
        if (!FireBaseController.FireBaseRemoteReady)
            SharedRemoteConfigController.OnInitialized.Action += data => RegisterRemoteDatabase();
        SharedRemoteConfigController.OnInitialized.Action += OnFireBaseInitialized;
        foreach (var baseService in GetComponentsInChildren<BaseService>())
        {
            Debug.Log(baseService.GetThisType());
            Services.Add(baseService.GetThisType(), baseService);
            baseService.ForceInit();
        }
        
        LoadParameterValueRetriever();
        StartCoroutine(WaitForOnKernelLoaded());
    }

    public void Start()
    {
        ram = Mathf.FloorToInt(SystemInfo.systemMemorySize / 1000f);
        segment = DeviceSegment.low_end;
        if (ram < 3)
            segment = DeviceSegment.low_end;
        else if (ram <= 4)
            segment = DeviceSegment.mid_range_low;
        else if (ram <= 6)
            segment = DeviceSegment.mid_range_high;
        else
            segment = DeviceSegment.high_end;

        StartCoroutine(WaitForRegister());
    }

    private IEnumerator WaitForRegister()
    {
        yield return new WaitForSeconds(1);
        foreach (var keyValuePair in Services)
            keyValuePair.Value.CheckRegister();
        fireBase.CheckRegister();
    }

    float _deltaTime;

    private void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        if (Time.frameCount % 1000 == 0)
        {
            var fps = (int) (1.0f / _deltaTime);
            //UIDebugLog.Log($"{fps} fps");
            if (Time.frameCount % 18000 == 0 && IsReady())
            {
                var fpsBy3 = (fps / 3) * 3;
                fireBase.LogEvent("statistics", "fps_value", fpsBy3.ToString());
            }
        }
    }

    private readonly PlayerPrefInt _logSystemState = new PlayerPrefInt("logSystemState");

    public static DeviceSegment segment = DeviceSegment.low_end;
    public static int ram;

    private void OnFireBaseInitialized(bool finish)
    {
        if (finish)
        {
            if (_logSystemState.Value == 0)
            {
                _logSystemState.Value = 1;
                fireBase.SetUserProperty("system_memory", ram.ToString());
                fireBase.SetUserProperty("device_segment", segment.ToString());
            }
        }
    }

    protected virtual void LoadParameterValueRetriever()
    {
        ParameterValueRetriever = parameterValueRetriever != null &&
                                  parameterValueRetriever.GetComponent<ILogParameterValueRetriever>() != null
            ? parameterValueRetriever.GetComponent<ILogParameterValueRetriever>()
            : new DummyLogParameterValueRetriever();
        if (ParameterValueRetriever is DummyLogParameterValueRetriever)
            Debug.Log("current game doesn't have CustomParameterRetriever'");
    }

    private void RegisterDatabase()
    {
        Dict.Clear();
        for (var i = 0; i < databases.Length; i++)
            Dict.Add(databases[i].GetType(), databases[i]);
    }

    private void RegisterRemoteDatabase()
    {
        RemoteDict.Clear();
        for (var i = 0; i < remoteDatabases.Length; i++)
        {
            remoteDatabases[i].Register(Dict[remoteDatabases[i].GetDatabaseType()]);
            remoteDatabases[i].LoadRemote();
            RemoteDict.Add(remoteDatabases[i].GetDatabaseType(), remoteDatabases[i]);

            if (remoteDatabases[i].GetDatabase() != null)
                OnUpdateRemote(remoteDatabases[i].GetDatabaseType());
        }
    }

    protected virtual void OnUpdateRemote(Type type)
    {
    }

    private IEnumerator WaitForOnKernelLoaded()
    {
        _ready = false;
        float t = 0f;
        int count = 0;
        while (!_ready)
        {
            yield return null;
            _ready = CheckReady();
            t += Time.deltaTime;
            if (t > 5 && !_ready)
            {
                t = 0f;
                count++;
                if (count >= 3)
                    _ready = CheckReady(true);
                if (count == 6)
                {
                    _ready = CheckReady(true, true);
                    Debug.LogError("time out CheckService ready");
                    _timeOut = true;
                    break;
                }
            }
        }
        Debug.Log($"[sonat check iron source] - Kernel is ready!!");
        _ready = true;
        OnKernelLoaded();
    }

    public void CheckConsent()
    {
        if (_timeOut)
        {
            StartCoroutine(checkForConsent());
        }
    }

    private IEnumerator checkForConsent()
    {
        _ready = false;
        while (!_ready)
        {
            yield return new WaitForSeconds(0.1f);
            _ready = CheckReady();
        }
    }

    private bool _timeOut;


    protected virtual void OnKernelLoaded()
    {
        KernelLoaded.Action?.Invoke();
        KernelLoaded.Destroy();
        var scripts = GetComponents<OnServiceLoadedScript>();
        foreach (var script in scripts)
            script.Register();
		//  Resolve<FireBaseController>().LogEvent("kernel_loaded");
		if (autoCheckInternet)
		{
            CheckConnectInternet();
        }
        
#if use_campaign_segment
        UserSegment.InitOnStart();
#endif
        
        UIDebugLog.Log("Kernel loaded!");
    }

    private static bool CheckReady(bool log = false, bool logFirebase = false)
    {
        foreach (var keyValuePair in Services)
            if (keyValuePair.Value.Initialized == false)
            {
                //Debug.Log($"[sonat check iron source] - {keyValuePair.Key} is not ready {keyValuePair.Key.Name}");
                if (log)
                {
                    UIDebugLog.Log(keyValuePair.Key + " is not ready " + keyValuePair.Key.Name);
                }

                if (logFirebase)
                {
                    Resolve<FireBaseController>().LogEvent("service_not_ready",
                        new LogParameter(ParameterEnum.name, keyValuePair.Key.Name));
                }

                return false;
			}

        return true;
    }


    // level and playtimes start from 0


    public static void LogStartTutorial(int tutIndex = -1)
    {
        if (tutIndex < 0)
        {
            Resolve<FireBaseController>().LogEvent("start_tutorial");
            Resolve<AppFlyerController>().SendEvent("start_tutorial");
        }
        else
        {
            Resolve<FireBaseController>().LogEvent("start_tutorial_" + tutIndex);
            Resolve<FireBaseController>().LogEvent("start_tutorial_" + tutIndex);
        }
    }

    public static void LogFinishTutorial(int tutIndex = -1)
    {
        if (tutIndex < 0)
        {
            Resolve<FireBaseController>().LogEvent("finish_tutorial");
            Resolve<AppFlyerController>().SendEvent("finish_tutorial");
            ResolveDirty<AdsManager>().StartEnableBanner();
            Debug.Log("LogFinishTutorial");
        }
        else
        {
            Resolve<FireBaseController>().LogEvent("finish_tutorial_" + tutIndex);
            Resolve<FireBaseController>().LogEvent("finish_tutorial_" + tutIndex);
        }
    }

    public static void LogInterstitial(string placementName)
    {
        LogInterstitial(placementName, new LogParameter[0]);
    }

    public static void LogInterstitial(string placementName, LogParameter[] logs)
    {
        var list = logs != null ? logs.ToList() : new List<LogParameter>();
        list.Add(new LogParameter(ParameterEnum.placement, placementName));
        list.Add(new LogParameter(ParameterEnum.level, ParameterValueRetriever.LevelDisplay));
        list.Add(new LogParameter(ParameterEnum.mode, ParameterValueRetriever.Mode));
        LogFirebase(EventNameEnum.show_interstitial, list.ToArray());

        list.Add(new LogParameter(ParameterEnum.af_adrev_ad_type, EventNameEnumForAf.show_interstitial.ToString()));
        LogAppFlyer(EventNameEnumForAf.af_ad_view, list.ToArray());
        LogAppFlyer(EventNameEnumForAf.show_interstitial);
    }

    public static void LogStartVideo(string placementName)
    {
        LogFirebase(EventNameEnum.show_video, new[]
        {
            new LogParameter(ParameterEnum.placement, placementName)
        });
        LogAppFlyer(EventNameEnumForAf.show_video,
            new[]
            {
                new LogParameter(ParameterEnum.placement, placementName),
                new LogParameter(ParameterEnum.af_adrev_ad_type, "video"),
            });
    }

    public static void LogShowVideo(string placementName)
    {
        LogFirebase(EventNameEnum.show_video, new[]
        {
            new LogParameter(ParameterEnum.placement, placementName)
        });
    }

    public static void LogFirebase(EventNameEnum eventName, LogParameter[] logs)
    {
        Resolve<FireBaseController>().LogEvent(eventName.ToString(), logs);
    }

    public static void LogFirebase(EventNameEnum eventName)
    {
        Resolve<FireBaseController>().LogEvent(eventName.ToString());
    }

    public static void LogAppFlyer(EventNameEnumForAf eventName, LogParameter[] logs)
    {
        var dict = new Dictionary<string, string>();
//        var dict = new Dictionary<string, string>()
//        {
//            {"af_adrev_ad_type", ""},
//        };
        foreach (var parameter in logs)
            dict.Add(parameter.stringKey, parameter.stringValue);
        Resolve<AppFlyerController>().SendEvent(eventName.ToString(), dict);
    }

    public static void LogAppFlyer(EventNameEnumForAf eventName)
    {
        Resolve<AppFlyerController>().SendEvent(eventName.ToString());
    }


    public static bool IsInternetConnection()
    {
#if UNITY_EDITOR
        return TestingSettings.InternetConnected;
#endif
        //return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
        //       Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    [SerializeField] private OnScreenDebugLog onScreenDebugLog;
    [SerializeField] private OnScreenDebugLogForUIDebugLog onScreenDebugLog2;

    public void AddOnScreenDebugLog()
    {
        if (OnScreenDebugLog.Instance == null)
        {
            if (onScreenDebugLog == null)
            {
                var newGo = new GameObject();
                onScreenDebugLog = newGo.AddComponent<OnScreenDebugLog>();
                DontDestroyOnLoad(newGo);
            }
           
            onScreenDebugLog.IsShow = true;
            onScreenDebugLog.gameObject.SetActive(true);
        }
        else
        {
            OnScreenDebugLog.Instance.gameObject.SetActive(true);
            OnScreenDebugLog.Instance.IsShow = true;
        }
    }
    
    [ContextMenu("AddOnScreenDebugLog2")]
    public void AddOnScreenDebugLog2()
    {
        if (OnScreenDebugLogForUIDebugLog.Instance == null)
        {
            if (onScreenDebugLog2 == null)
            {
                var newGo = new GameObject();
                onScreenDebugLog2 = newGo.AddComponent<OnScreenDebugLogForUIDebugLog>();
                DontDestroyOnLoad(newGo);
            }
           
            onScreenDebugLog2.IsShow = true;
            onScreenDebugLog2.gameObject.SetActive(true);
        }
        else
        {
            OnScreenDebugLogForUIDebugLog.Instance.gameObject.SetActive(true);
            OnScreenDebugLogForUIDebugLog.Instance.IsShow = true;
        }
    }

    /// <summary>
    /// call one time on game ready
    /// </summary>
    public static void StartCheckConnectInternet()
	{
        kernel.CheckConnectInternet();
	}

    private Coroutine waitCheckInternet;
    public void WaitCheckInternet()
	{
        if (waitCheckInternet == null)
        {
            int timeGap = (int)RemoteConfigKey.check_internet_time_gap.GetValueInt(30);
            if (timeGap > 0)
                waitCheckInternet = StartCoroutine(WaitCheckConnectInternet(timeGap));
        }
    }

    private IEnumerator WaitCheckConnectInternet(int timeGap)
    {
        yield return new WaitForSeconds(timeGap);
        waitCheckInternet = null;
        CheckConnectInternet();
    }

    public void CheckConnectInternet()
	{
        if (!IsInternetConnection())
        {
            if(waitCheckInternet != null)
			{
                StopCoroutine(waitCheckInternet);
                waitCheckInternet = null;
            }
            OnInternetDisconnected?.Invoke();
        }
        else
        {
            WaitCheckInternet();
        }
    }


    public static void ShowSdkInfo()
	{
        SonatSdkInspector.GetInstance().ShowInfo();
	}

#if UNITY_IOS 
    public string iOSAppId;
#endif
    public static void OpenHelpAndSupport(Action callback = null)
	{
#if use_appflyer && use_firebase && use_iap
#if UNITY_IOS
        string bundle_id = kernel.iOSAppId;
        string platform = "ios";
#elif UNITY_ANDROID
        string bundle_id = Application.identifier;
        string platform = "android";
#endif

        string user_id = AppFlyerController.CustomerUserId;
        string fb_analytics_instance_id = Resolve<FireBaseController>().AnalyticsInstanceId;
        string fb_instance_id = Resolve<FireBaseController>().FirebaseInstanceId;
        string af_user_id = AppFlyerController.GetUserId();
        string membership = Resolve<BasePurchaser>().PaidUser() ? "paid" : "free";
        string app_version = Application.version;
        string url = $"https://help.sonatgame.com/home?bundle_id={bundle_id}&platform={platform}&user_id={user_id}&fb_analytics_instance_id={fb_analytics_instance_id}&fb_instance_id={fb_instance_id}&af_user_id={af_user_id}&membership={membership}&app_version={app_version}";
        //string url = $"https://help.sonatgame.com";
#if use_webview
        SonatWebView.ShowUrlFullScreen(url, callback);
#else
        Application.OpenURL(url);
#endif
#endif
    }

}