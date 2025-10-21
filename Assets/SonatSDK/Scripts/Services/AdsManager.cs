using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Serialization;
using LogType = sonat_sdk.Scripts.LogType;

// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
using System.Runtime.InteropServices;
#endif


#if UNITY_IOS
namespace AudienceNetwork
{
    public static class AdSettings
    {
        [DllImport("__Internal")] 
        private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);

        public static void SetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled)
        {
#if !UNITY_EDITOR
            FBAdSettingsBridgeSetAdvertiserTrackingEnabled(advertiserTrackingEnabled);
#endif
        }
    }
}

#endif

public class AdsManager : BaseService
{
    [SerializeField] private bool waitConsentForm = false;
    [SerializeField] private bool inFocusIntersLevelCondition = true; // condition if onfocus check level or not

    public static bool GDPRRequested;

    [ContextMenu("test")]
    void test()
    {
        Debug.LogError(_initilized);
    }
    private bool ConsentReady { get; set; }

    private bool _initilized;
    public static bool showBannerOnLoad = true;
    public bool CalculateWaitConsent => RemoteConfigKey.gdpr_force.GetValueBoolean() || waitConsentForm;
    public override bool Initialized
    {
       
        get {
            //Debug.Log($"[sonat check iron source] - AdsManager Initialized: i_{_initilized} Caculate_{CalculateWaitConsent} CsReady_{ConsentReady}");
            return _initilized && (!CalculateWaitConsent || ConsentReady); 
        }
      //  get { return _initilized; }
        protected set => _initilized = value ;
    }

    public BaseAction onReady = new BaseAction();
    public BaseAction onVideoRewarded = new BaseAction(); // event when video ads finish and got rewarded
    public BooleanAction onVideoLoaded = new BooleanAction(); // event when video ads loaded
    public BaseAction onPaidAdEvent = new BaseAction(); // event when video ads loaded

    private readonly IntAction _onAnAdsInstanceInitialized = new IntAction(); // event when ads initialized

    public BooleanAction
        onBannerShow = new BooleanAction(); // event when banner showed, for enable UI bottom ads background panel

    public BooleanAction onInterstitialShow = new BooleanAction();

    public void AddOnVideoLoaded(Action<bool> setActiveAds)
    {
        onVideoLoaded.Action += setActiveAds;
    }

    [SerializeField] private bool isVideoReady;

    public static int AppLeaving { get; set; }
    public static bool VideoOpened { get; set; }
    public static bool OpenAdsOrInterstitialOpened { get; set; }
    public static bool BannerOpened { get; set; } // to skip Application Focus Interstitial Ads when open banner
    public static float BannerHeight { get; set; }

    public static bool IsAdsOpened() => VideoOpened || OpenAdsOrInterstitialOpened || BannerOpened;

    private Action _rewardedAction;
    private static float _lastTimeShowInters = -999;
    private static float _lastTimeShowInGameInter = -999;
    private static float _lastTimeShowOnFocusInter = -999;
    private static float _lastTimeShowRewardAds = -999;

    public static void FakeLastTimeShowBanner()
    {
        _lastTimeShowInters = Time.time;
    }

    public static bool NoAds
    {
        get => PlayerPrefs.GetInt(nameof(NoAds)) == 1;
        private set => PlayerPrefs.SetInt(nameof(NoAds), value ? 1 : 0);
    }

    public static bool NotShowBanner { get; set; }

    private AdsInstance[] _adsInstances;

    private Dictionary<Type, AdsInstance> _screenContainer;

    private readonly List<AdsInstance> _readyAdsInstances = new List<AdsInstance>();

    public T Resolve<T>() where T : AdsInstance
    {
        return (T) _screenContainer[typeof(T)];
    }

    //    public string VideoLogName { get; set; }
    //    public string InterstitialLogName { get; set; }
    public void ForceConsenReady()
    {
        ConsentReady = true;
    }
    public static int PlayTimesOrLevel
    {
        get => PlayerPrefs.GetInt("PlayTimesOrLevel");
        set => PlayerPrefs.SetInt("PlayTimesOrLevel", value);
    }
    public Action<bool> OnNoAdsUpdate;

    public virtual bool IsNoAds()
    {
//        if (!NoAds)
        return NoAds;
    }

    public virtual void EnableNoAds()
    {
        NoAds = true;
        CheckNoAds();
        OnNoAdsUpdate?.Invoke(NoAds);
    }

    public void DisableNoAds()
    {
        NoAds = false;
        ClearAds();
        OnNoAdsUpdate?.Invoke(NoAds);
    }

    public void CheckNoAds()
    {
        foreach (var adsInstance in _readyAdsInstances)
        {
            if(adsInstance.showBanner)
                adsInstance.showBanner = !IsNoAds();
            adsInstance.DestroyBanner();
        }
    }



    public void StartEnableBanner()
    {
        NotShowBanner = false;
        CheckShowBanner();
    }

    public void RequestBannerForce()
    {
        foreach (var adsInstance in _readyAdsInstances)
        {
            adsInstance.showBanner = !IsNoAds();
            adsInstance.CheckShowBanner();
        }
    }

    [SerializeField] private int countDownFirstTime = 0;
    private bool ValidTimePlayShowAds { get; set; }
    protected override void Register()
    {
        base.Register();

        var countDown = PlayerPrefs.GetInt("count_down_to_first_time_show_ads", countDownFirstTime);
        if (countDown <= 0)
            ValidTimePlayShowAds = true;
        else
            StartCoroutine(IeCountDown());

#if UNITY_IOS
        AskAtt();
        bool setFanFlag;
        if ((int)ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == 3)
            setFanFlag = true; //If==3, App is AUTHORIZED in settings
        else setFanFlag = false;  //DENIED, RESTRICTED or NOT DETERMINED (==2,1,0)
    
        AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(setFanFlag);
        foreach (var videoAdsInstance in _readyAdsInstances)
            videoAdsInstance.OnTrackingYes(setFanFlag);
            StartCoroutine(WaitCheckAtt());
#endif

    }

    private IEnumerator IeCountDown()
    {
        var countDown = PlayerPrefs.GetInt("count_down_to_first_time_show_ads", countDownFirstTime);
        while (countDown > 0)
        {
            yield return new WaitForSeconds(1);
            countDown--;
            PlayerPrefs.SetInt("count_down_to_first_time_show_ads", countDown);
        }
        ValidTimePlayShowAds = true;
    }

    public static bool WaitAtt;
    public static void AskAtt()
    {
        CheckShowLogAtt("CALL", "call");
#if use_max
    return;
#endif
#if UNITY_IOS
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
            ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
    }

    private static void CheckShowLogAtt(string action, string status)
	{
		if (!PlayerPrefs.HasKey($"SONAT_LOG_ATT_{action}"))
		{
#if !dummy_log
            new SonatLogATT()
            {
                status = status
            }.Post();
#endif
        }
	}

    private IEnumerator WaitCheckAtt()
	{
#if UNITY_IOS
        yield return new WaitForSeconds(1);
        string status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus().ToString();
        Debug.Log($"ATT_STATUS: {status}");
        CheckShowLogAtt("STATUS", status);
#else
        yield break;
#endif
    }

    private void Start()
    {
        ForceInit();
        _onAnAdsInstanceInitialized.Action += CheckInitialized;
        onVideoLoaded.Action += OnVideoLoadedChangedHandler;
        onVideoRewarded.Action += OnVideoFinishHandler;
        onBannerShow.Action += OnBannerShowHandler;
        _screenContainer = new Dictionary<Type, AdsInstance>();
        _adsInstances = ActiveAds().ToArray();
        StartCoroutine(Wait(0.5f));
        
//        StartCoroutine(IntervalCheckVideoReady());
    }

    [SerializeField] private PlayerPrefRemoteString mediation_platform;

    private IEnumerable<AdsInstance> ActiveAds()
    {
        mediation_platform = new PlayerPrefRemoteString(RemoteConfigKey.mediation_platform,
            RemoteConfigKey.mediation_platform.GetValueString());
        var ads = GetComponents<AdsInstance>();
        if (ads.Length == 1)
            yield return ads[0];
        if (ads.Length > 1)
        {
            var mediationPlatform = mediation_platform.DefaultValueWhenEmpty();
            if (string.IsNullOrEmpty(mediationPlatform))
            {
                foreach (var adsInstance in ads)
                    if (adsInstance.isDefault)
                    {
                        yield return adsInstance;
                        break;
                    }
            }
            else
            {
                UIDebugLog.Log("mediation_platform is " + mediationPlatform);
                int count = 0;
                foreach (var adsInstance in ads)
                    if (adsInstance.IsRemoteActive())
                    {
                        count++;
                        yield return adsInstance;
                    }

                if (count == 0)
                    foreach (var adsInstance in ads)
                        if (adsInstance.isDefault)
                        {
                            yield return adsInstance;
                            count++;
                            break;
                        }

                if (count == 0)
                    yield return ads[0];
            }
        }
    }

    private IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        for (var index = 0; index < _adsInstances.Length; index++)
        {
            var adsInstance = _adsInstances[index];
            adsInstance.SetupAction(index, _onAnAdsInstanceInitialized, onVideoLoaded, onVideoRewarded,
                onBannerShow,
                !IsNoAds());
            _screenContainer.TryAdd(adsInstance.GetType(), adsInstance);
        }
    }

    public void DestroyBanner()
    {
        foreach (var videoAdsInstance in _readyAdsInstances)
            videoAdsInstance.DestroyBanner();
    }

    [MyButtonInt(nameof(SwitchBanner))] public int testSwitch;

    public void SwitchBanner(bool toNative)
    {
        foreach (var videoAdsInstance in _readyAdsInstances)
        {
            if (videoAdsInstance.SwitchNativeBanner(toNative))
            {
            }

            break;
        }
    }

    public void HideNative()
    {
        foreach (var videoAdsInstance in _readyAdsInstances)
            videoAdsInstance.HideNativeBanner();
    }

    public void HideBanner()
    {
        if (_readyAdsInstances == null || _readyAdsInstances.Count == 0) showBannerOnLoad = false;
        foreach (var videoAdsInstance in _readyAdsInstances)
            videoAdsInstance.HideBanner();
    }
    
    public void ShowBanner()
    {
        if (IsNoAds()) return;
        showBannerOnLoad = true;
        foreach (var videoAdsInstance in _readyAdsInstances)
            videoAdsInstance.ShowBanner();
    }


    private void OnBannerShowHandler(bool show)
    {
        
    }

    public bool TrueInitialized { get; set; }

    private void CheckInitialized(int index)
    {
        _readyAdsInstances.Add(_adsInstances[index]);
        Debug.Log($"[sonat check iron source] - loaded ads instance {_readyAdsInstances.Count} / {_adsInstances.Length}");
        if (_readyAdsInstances.Count == _adsInstances.Length)
        {
            onReady.Action?.Invoke();
            TrueInitialized = true;
            onReady.Destroy();
            ConsentReady = _readyAdsInstances.All(x => x.ConsentReady);
            Debug.Log($"[sonat check iron source] - Consen Ready: {ConsentReady}");
        }

//        
//        Initialized = _readyAdsInstances.All(videoAdsInstance => videoAdsInstance.IsInitialized());
//        if (Initialized)
//            onAdsInitialized.Destroy();
    }


    public float TimeStartVideo { get; set; }


    protected virtual bool CustomValid() => true;

    public float TimeStartInters { get; set; }

    [Obsolete("Method is obsolete.", false)]
    public void ShowVideoAds(Action rewardedAction, string placementName, LogParameter[] logs = null)
    {
        ShowVideoAds(rewardedAction, new SonatLogVideoRewarded()
        {
            placement =  placementName
        });
    }

    private bool CheckConditionTimeGap(int level, bool isOnFocus)
    {
        // gap inter common
        int timeGapCommon = Kernel.Resolve<FireBaseController>().remoteConfigController.GetValueByLevel(RemoteConfigKey.by_level_condition_inter_common_time_gap, level, 0);
        float timeInterRange = Time.time - _lastTimeShowInters;
        if (timeInterRange < timeGapCommon) return false;

        // gap inter - reward
        int timeGapByReward = Kernel.Resolve<FireBaseController>().remoteConfigController.GetValueByLevel(RemoteConfigKey.by_level_condition_inter_rw_time_gap, level, 0);
        float timeRewardRange = Time.time - _lastTimeShowRewardAds;
        if (timeRewardRange < timeGapByReward) return false;

        // Old ver
        if (timeGapCommon == 0 && timeGapByReward == 0)
        {
            int timeGapOld = Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_interstitial_time_gap, level,
                    (int)RemoteConfigKey.interstitial_time_gap.GetValueInt());
            float timeInterRangeOld = Time.time - _lastTimeShowInters;
            if (timeInterRangeOld < timeGapOld) return false;
        }


        if (isOnFocus)
        {
            // gap inter on focus
            int timeGap = Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_ads_on_focus_time_gap, level,
                    (int)RemoteConfigKey.ads_on_focus_time_gap.GetValueInt());

            float timeOnFocusRange = Time.time - _lastTimeShowOnFocusInter;
            return timeOnFocusRange >= timeGap;
        }
        else
        {
            int timeGap = Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_ads_ingame_time_gap, level, 0);
            float timeIngameRange = Time.time - _lastTimeShowInGameInter;
            return timeIngameRange >= timeGap;
        }
    }


    [Obsolete("Method is obsolete.", false)]
    public bool ShowInterstitial(string placementName, bool isOnFocus = false, int level = int.MaxValue, LogParameter[] logs = null)
    {
        Kernel.LogInterstitial(placementName,logs);
        var log = new SonatLogShowInterstitial()
        {
            level = level,
            mode = "classic",
            placement = placementName
        };
       return ShowInterstitial(log, isOnFocus);
    }

    public AdsInstance IsInterstitialAdsValid(int level, bool conditionLevel, bool isOnFocus)
    {
        if (!CustomValid())
            return null;

        if (IsNoAds())
        {
            UIDebugLog.Log("Couldn't start inters Ads : NoAds=true",false, LogType.Ads);
            return null;
        }

#if use_firebase
//        if (!(SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched ||
//              SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.FetchedFail))
//        {
//            UIDebugLog.Log(
//                "Couldn't start inters Ads : RemoteConfig not Fetched" + SharedRemoteConfigController.FetchStatus,
//                LogType.Ads);
//            return null;
//        }
#endif

        if (BannerOpened)
        {
            UIDebugLog.Log($"Couldn't start inters Ads :'{nameof(BannerOpened)}:{BannerOpened}",false, LogType.Ads);
            return null;
        }

        if (OpenAdsOrInterstitialOpened)
        {
            UIDebugLog.Log(
                $"Couldn't start inters Ads :'{nameof(OpenAdsOrInterstitialOpened)}:{OpenAdsOrInterstitialOpened}",false,
                LogType.Ads);
            return null;
        }

        if (VideoOpened)
        {
            UIDebugLog.Log($"Couldn't start inters Ads :'{nameof(VideoOpened)}:{VideoOpened}",false, LogType.Ads);
            return null;
        }

        if (AppLeaving > 0)
        {
            UIDebugLog.Log($"Couldn't start inters Ads :'{nameof(AppLeaving)}:{AppLeaving}",false, LogType.Ads);
            return null;
        }

        if (conditionLevel)
        {
            long condition = 0;
            if (isOnFocus)
                condition = (RemoteConfigKey.level_start_show_on_focus_ads).GetValueInt();

            if (condition == 0)
                condition = (RemoteConfigKey.start_level_show_interstitial).GetValueInt();
            if (level < condition)
            {
                UIDebugLog.Log(
                    $"Couldn't start inters Ads : Level condition not valid {nameof(level)}={level} < {condition}(condition)",false,
                    LogType.Ads);
                return null;
            }
        }

        if (!CheckConditionTimeGap(level, isOnFocus))
        {
            UIDebugLog.Log(
                $"Couldn't start inters Ads : Time gap invalid {Time.time - _lastTimeShowInters} < interstitial_time_gap={(RemoteConfigKey.interstitial_time_gap).GetValueInt()})",false,
                LogType.Ads);
            return null;
        }

        var interstitialAdsSource = GetInterstitialAds();
        if (interstitialAdsSource == null)
        {
            UIDebugLog.Log($"Couldn't start inters Ads : No ads ready, total source ={_adsInstances.Length}",false, LogType.Ads);
            if (Kernel.IsInternetConnection() && _adsInstances.Length> 0 && Time.deltaTime - _lastTimeFailed > 60 && Time.deltaTime - _lastTimeShowInters > 60)
            {
                new CustomSonatLog("show_interstitial_fail_no_ads", new List<Sonat.LogParameter>()).Post(true);
                _lastTimeFailed = Time.deltaTime;
            }
            return null;
        }

        return interstitialAdsSource;
    }

    private float _lastTimeFailed;

    public void EnableTestAdsThenClearAds()
    {
        foreach (var readyAdsInstance in _readyAdsInstances)
            readyAdsInstance.EnableTestAds();
        ClearAds();
    }

    private SonatLogVideoRewarded _videoLog;

    protected virtual void SetRewardedLogName( SonatLogVideoRewarded log)
    {
        if(log != null)
            SonatAnalyticTracker.RewardedLogPlacement = log.placement;
    }
    public void ShowVideoAds(Action rewardedAction, SonatLogVideoRewarded log)
    {
        SetRewardedLogName(log);
        TimeStartVideo = Time.unscaledTime;
#if UNITY_EDITOR
        if (TestingSettings.SkipRewarded)
        {
            rewardedAction.Invoke();
            return;
        }
#endif
        _videoLog = log;
        _rewardedAction = rewardedAction;
        foreach (var videoAdsInstance in _readyAdsInstances)
        {
            if (videoAdsInstance.IsVideoAdsReady())
            {
                _rewardedAction = rewardedAction;
                videoAdsInstance.ShowVideoAds();
                VideoOpened = true;
                _lastTimeShowRewardAds = Time.time;
                Kernel.LogStartVideo(SonatAnalyticTracker.RewardedLogPlacement);
                new SonatLogScreenView()
                {
                    screen_name = "rewarded_ads",
                    saveLastScreen = false
                }.Post();
                break;
            }
        }
    }

    public bool ShowInterstitial(SonatLogShowInterstitial log, bool isOnFocus = false, bool conditionLevel = false,Action actionAfterAds = null,int loadingIndex = 0)
    {
        if (!ValidTimePlayShowAds)
            return false;

        if (RemoteConfigHelper.HasKey(RemoteConfigKey.ignore_inter_by_log_name.ToString()))
        {
            var ignores = RemoteConfigKey.ignore_inter_by_log_name.GetValueString();
            if (!string.IsNullOrEmpty(ignores))
            {
                var splits = ignores.Split(",");
                foreach (var split in splits)
                {
                    if (split == log.placement)
                    {
                        UIDebugLog.Log("Inters ads was ignored by remote config :"+split);
                        return false;
                    }
                }
            }
        }
        
        
        TimeStartInters = Time.unscaledTime;
        SonatAnalyticTracker.InterstitialLogName = log.placement;
        
        

        var validInterstitialAdsSource = IsInterstitialAdsValid(log.level,conditionLevel, isOnFocus);

        if (validInterstitialAdsSource == null)
        {
            if (actionAfterAds != null)
            {
                UIDebugLog.Log("no ads, invoke action");
                actionAfterAds.Invoke();
            }
            return false;
        }

        CheckLoadLoadingTransition();
        if (LoadingTransition.Instance != null)
            StartCoroutine(IeShowInterInTransition(log,validInterstitialAdsSource, actionAfterAds,isOnFocus,loadingIndex));
        else
        {
            //    ShowInterstitial(log, false, conditionLevel);
            this.callbackInter = actionAfterAds;
            waitCallbackInter = null;
            ShowInters(log, isOnFocus, validInterstitialAdsSource);
            //if(actionAfterAds!= null)
            //    actionAfterAds.Invoke();
        }

        return true;
    }

    private Coroutine waitCallbackInter;
    private Action callbackInter;
    public float timeOutInter = 5f;
    // ie show for a valid interstitialAdsSource
    private IEnumerator IeShowInterInTransition(SonatLogShowInterstitial log,AdsInstance interstitialAdsSource, Action nextGame,bool isOnFocus,int loadingIndex)
    {
        //        var startTimeScale = Time.timeScale;
        //        Time.timeScale = 0;
        this.callbackInter = nextGame;
        waitCallbackInter = null;
        yield return LoadingTransition.Instance.LoadingIn(loadingIndex);
        ShowInters(log, isOnFocus, interstitialAdsSource);
        yield return new WaitForSeconds(timeOutInter);
        StartCallbackInter();
        //yield return LoadingTransition.Instance.LoadingOut(nextGame);
//        Time.timeScale = startTimeScale;
    }

    public void StartCallbackInter()
	{
        if (waitCallbackInter != null) return;
        waitCallbackInter = StartCoroutine(WaitCallback());
	}

    IEnumerator WaitCallback()
    {
        yield return new WaitForEndOfFrame();
        if (LoadingTransition.Instance != null)
            yield return LoadingTransition.Instance.LoadingOut(callbackInter);
        else
            callbackInter?.Invoke();
        //waitCallbackInter = null;
    }
    private PlayerPrefInt showInterCount = new PlayerPrefInt("SONAT_INTER_COUNT");
    private void ShowInters(SonatLogShowInterstitial log,bool isOnFocus,AdsInstance interstitialAdsSource)
    {
        interstitialAdsSource.ShowInterstitial(isOnFocus);
        _lastTimeShowInters = Time.time;
		if (isOnFocus)
		{
            _lastTimeShowOnFocusInter = Time.time;
		}
		else
		{
            _lastTimeShowInGameInter = Time.time;
		}

        new SonatLogScreenView()
        {
            screen_name = "inter_ads",
            saveLastScreen = false
        }.Post();
        onInterstitialShow.Action.Invoke(true);

        log.Post();
        Kernel.LogAppFlyer(EventNameEnumForAf.af_ad_view, new[]
        {
            new LogParameter(ParameterEnum.af_adrev_ad_type, EventNameEnumForAf.show_interstitial.ToString())
        });
        Kernel.LogAppFlyer(EventNameEnumForAf.show_interstitial);
        UIDebugLog.Log("ShowInterstitial invoked",false, LogType.Ads);
        showInterCount.Value++;
    }

    public bool CanShowPopupNoAds(bool onetime = false)
    {
        if (onetime)
        {
            return !IsNoAds() && showInterCount.Value == RemoteConfigKey.show_interstitial_first_no_ads.GetValueInt(1);
        }
        else
            return !IsNoAds() && showInterCount.Value >= RemoteConfigKey.show_interstitial_first_no_ads.GetValueInt();
    }



    public bool IsVideoAdsReady()
    {
        foreach (var videoAdsInstance in _readyAdsInstances)
            if (videoAdsInstance.IsVideoAdsReady())
                return true;
        return false;
    }

    /// <summary>
    /// kiểm tra để show lại banner
    /// </summary>
    public void CheckShowBanner()
    {
        Debug.Log("Sonat :CheckShowBanner :NoAds=" + IsNoAds());
        if (IsNoAds()) return;
        foreach (var videoAdsInstance in _readyAdsInstances)
            videoAdsInstance.CheckShowBanner();
    }

    public bool IsBannerShowing()
    {
        foreach (var videoAdsInstance in _readyAdsInstances)
            if (videoAdsInstance.BannerIsShowing())
                return true;
        return false;
    }

    public bool IsVideoShowing()
    {
        foreach (var videoAdsInstance in _readyAdsInstances)
            if (videoAdsInstance.VideoIsShowing())
                return true;
        return false;
    }

    public bool IsIntersShowing()
    {
        foreach (var videoAdsInstance in _readyAdsInstances)
            if (videoAdsInstance.IntersIsShowing())
                return true;
        return false;
    }

    public bool IsInterstitialAdsReady()
    {
        return GetInterstitialAds() != null;
    }

    private AdsInstance GetInterstitialAds()
    {
        if (_readyAdsInstances.Count == 0)
            Debug.LogError("Sonat no AdsIntance initialized");
        foreach (var videoAdsInstance in _readyAdsInstances)
            if (videoAdsInstance.IsInterstitialReady())
            {
                Debug.Log("ads ready:" + videoAdsInstance);
                return videoAdsInstance;
            }

        return null;
    }

    private void OnVideoFinishHandler()
    {
        _rewardedAction.Invoke();
        if(_videoLog != null)
            _videoLog.Post();
        OnVideoLoadedChangedHandler(false);
        Kernel.LogAppFlyer(EventNameEnumForAf.video_rewarded);
        Kernel.LogAppFlyer(EventNameEnumForAf.af_ad_view, new[]
        {
            new LogParameter(ParameterEnum.af_adrev_ad_type, EventNameEnumForAf.video_rewarded.ToString())
        });
    }

    private void OnVideoLoadedChangedHandler(bool ready)
    {
        isVideoReady = IsVideoAdsReady();
    }

    protected virtual bool OnApplicationFocusValid() => true;

    private float _applicationOutTime;

    private void OnApplicationPause(bool pauseStatus)
    {
        UIDebugLog.Log0("OnApplicationPause:pauseStatus=" + pauseStatus);

        if (pauseStatus)
            _applicationOutTime = Time.realtimeSinceStartup;
    }

    private DateTime _lastOutFocusTime;

    public void LoadGdpr()
    {
        foreach (var readyAdsInstance in _readyAdsInstances)
            if(!readyAdsInstance.ConsentReady)
                readyAdsInstance.LoadGDPR();
    }
    
    protected virtual void OnApplicationFocus(bool hasFocus)
    {
		if (hasFocus)
		{
            Time.timeScale = 1;
		}
        UIDebugLog.Log0("OnApplicationFocus:hasFocus=" + hasFocus);
        
        if (Time.time > 30)
        {
            foreach (var readyAdsInstance in _readyAdsInstances)
                if(!readyAdsInstance.ConsentReady)
                    readyAdsInstance.LoadGDPR();
        }

        //  return;
        if (!hasFocus)
        {
            _lastOutFocusTime = DateTime.Now;
            _applicationOutTime = Time.realtimeSinceStartup;
        }

        if (!Initialized)
        {
            return;
        }

        if (!hasFocus)
            return;

        if (RemoteConfigKey.turn_off_focus_ads.GetValueBoolean())
        {
            UIDebugLog.Log("RemoteConfigKey.turn_off_focus_ads.GetValueBoolean()"+RemoteConfigKey.turn_off_focus_ads.GetValueBoolean(),false, LogType.Ads);
            if (AppLeaving > 0)
            {
                if(!countingDown)
                    StartCoroutine(WaitForShopClosed());
            }
            return;
        }

        if (FireBaseController.FireBaseRemoteReady)
        {
            var current = DateTime.Now;
            var diff = current - _lastOutFocusTime;
            if (diff.Seconds > RemoteConfigKey.seconds_to_dispose_ads.GetValueInt())
            {
                Kernel.Resolve<AdsManager>().ClearAds();
                AppLeaving = 0;
                UIDebugLog.Log("Show inters fail :Dispose all ads",false, LogType.Ads);
                return;
            }
        }

        if (!Kernel.IsReady())
        {
            AppLeaving = 0;
            UIDebugLog.Log("Show inters fail : Kernel not ready",false, LogType.Ads);
            return;
        }

        if (Time.time < 3)
        {
            AppLeaving = 0;
            UIDebugLog.Log("Show inters fail : Time too soon < 3s",false, LogType.Ads);
            return;
        }

//        if(Kernel.IsReady() && hasFocus)
//            Kernel.Resolve<FireBaseController>().LogEvent("AdsManagerOnFocus");
        if (!OnApplicationFocusValid())
        {
            UIDebugLog.Log("Show inters fail : OnApplicationFocusValid is False",false, LogType.Ads);
            return;
        }


        if (Time.realtimeSinceStartup - _applicationOutTime > 1800)
        {
            UIDebugLog.Log("Show inters fail : _applicationOutTime too long",false, LogType.Ads);
            _applicationOutTime = Time.realtimeSinceStartup;
            AppLeaving = 0;
            return;
        }
        else if (Time.realtimeSinceStartup - _applicationOutTime < RemoteConfigKey.min_seconds_out_focus.GetValueInt(10))
        {
            AppLeaving = 0;
            UIDebugLog.Log("Show inters fail : _applicationOutTime too short", false, LogType.Ads);
            // _applicationOutTime = Time.realtimeSinceStartup;
            return;
        }


#if UNITY_EDITOR
        if (TestingSettings.TurnOffOnFocusAds)
            return;
        UIDebugLog.Log("Ready to check show inters",false, LogType.Ads);
        ShowIntersOnFocus();
#else
        StartCoroutine(DelayActionCoroutine(0.1f, ShowIntersOnFocus));
#endif
    }

    private IEnumerator DelayActionCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }


    private void ShowIntersOnFocus()
    {
        string logName = "on_focus";
        var log = logName.CreateDefaultLogInterstitial();
        bool showInterSuccess = ShowInterstitial(log, true, inFocusIntersLevelCondition);
        if (!showInterSuccess && OnInterstitialAdsNotShow != null)
            OnInterstitialAdsNotShow.Invoke(true);
        if (AppLeaving > 0)
        {
            if(!countingDown)
                StartCoroutine(WaitForShopClosed());
        }
        if (BannerOpened)
            StartCoroutine(WaitForBannerOpenedFalse());
    }

    private void ClearAds()
    {
        for (var i = 0; i < _readyAdsInstances.Count; i++)
        {
            _readyAdsInstances[i].RequestNewAds();
        }
    }

    public static Action<bool> OnInterstitialAdsNotShow;

    private static bool countingDown;
    private static IEnumerator WaitForShopClosed()
    {
        countingDown = true;
        while (AppLeaving > 0)
        {
            AppLeaving--;
            yield return new WaitForSeconds(1);
        }

        countingDown = false;
    }

    private static IEnumerator WaitForVideoOpenedFalse()
    {
        yield return new WaitForSeconds(3);
        VideoOpened = false;
    }

    private static IEnumerator WaitForBannerOpenedFalse()
    {
        yield return new WaitForSeconds(3);
        BannerOpened = false;
    }

    private static IEnumerator WaitForInterstitialOpenedFalse()
    {
        yield return new WaitForSeconds(3);
        OpenAdsOrInterstitialOpened = false;
    }

    public void ShowAdsIds()
    {
        foreach (var readyAdsInstance in _readyAdsInstances)
        {
            readyAdsInstance.ShowId();
        }
    }

    public void ShowDebugInspector()
    {
        foreach (var readyAdsInstance in _readyAdsInstances)
        {
            readyAdsInstance.ShowDebugger();
            break;
        }
    }

    public void ShowNativeBanner()
	{
        foreach (var readyAdsInstance in _readyAdsInstances)
        {
            if (readyAdsInstance.IsNativeReady())
            {
                readyAdsInstance.ShowNativeBanner();
                break;
            }
        }
    }

    public void HideNativeBanner()
	{
        foreach (var readyAdsInstance in _readyAdsInstances)
        {
            readyAdsInstance.HideNativeBanner();
        }
    }

//    public void ShowInterstitialInTransition(SonatLogShowInterstitial log, Action actionAfterAds,int loadingIndex, bool conditionLevel = true)
//    {
//        if (!IsInterstitialAdsReady())
//            actionAfterAds.Invoke();
//        else
//        {
//            CheckLoadLoadingTransition();
//            if (LoadingTransition.Instance != null)
//                StartCoroutine(IeShowInterInTransition(log, actionAfterAds,loadingIndex, conditionLevel));
//            else
//            {
//                ShowInterstitial(log, false, conditionLevel);
//                actionAfterAds.Invoke();
//            }
//        }
//    }
//
//    private IEnumerator IeShowInterInTransition(SonatLogShowInterstitial log, Action nextGame,int loadingIndex, bool conditionLevel = true)
//    {
//        yield return LoadingTransition.Instance.LoadingIn(loadingIndex);
//        ShowInterstitial(log, false, conditionLevel);
//        yield return LoadingTransition.Instance.LoadingOut(nextGame);
//    }
//    
  

    private void CheckLoadLoadingTransition()
    {
        if (LoadingTransition.Instance == null)
        {
            var transition = FindObjectOfType<LoadingTransition>(true);
            if (transition != null)
                transition.CheckRegister();
        }
    }

    #region Mrec Banner
    public void ShowMrecBanner()
    {
        if(IsNoAds()) return;
        foreach (var readyAdsInstance in _readyAdsInstances)
        {
            if (readyAdsInstance.IsMrecBannerReady())
            {
                readyAdsInstance.ShowMrecBanner();
                break;
            }
        }
    }
    
    public void HideMrecBanner()
    {
        foreach (var videoAdsInstance in _readyAdsInstances)
            videoAdsInstance.HideMrecBanner();
    }

    public bool IsMrecBannerReady()
    {
        foreach (var readyAdsInstance in _readyAdsInstances)
        {
            if (readyAdsInstance.IsMrecBannerReady())
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region App Open Ads
    public void ShowAppOpenAds()
    {
        foreach (var readyAdsInstance in _readyAdsInstances)
        {
            if (readyAdsInstance.IsAppOpenAdsReady())
            {
                readyAdsInstance.ShowAppOpenAds();
                break;
            }
        }
    }

    public bool IsAppOpenAdsReady()
    {
        foreach (var readyAdsInstance in _readyAdsInstances)
            if (readyAdsInstance.IsAppOpenAdsReady())
                return true;
        return false;
    }

    public void ResetTimeShowOnFocusInter()
    {
        AppLeaving = 10;

        if (!countingDown)
            StartCoroutine(WaitForShopClosed());
    }
    #endregion

    #region Collapsible Banner
    public void ShowCollapsibleBanner()
    {
        if (IsNoAds()) return;
        foreach (var readyAdsInstance in _readyAdsInstances)
        {
            if (readyAdsInstance.IsCollapsibleBannerReady())
            {
                readyAdsInstance.ShowCollapsibleBanner();
                break;
            }
        }
    }

    public void HideCollapsibleBanner()
    {
        foreach (var readyAdsInstance in _readyAdsInstances)
            readyAdsInstance.HideCollapsibleBanner();
    }

    public bool IsCollapsibleBannerReady()
    {
        foreach (var readyAdsInstance in _readyAdsInstances)
        {
            if (readyAdsInstance.IsCollapsibleBannerReady())
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Native Ads
    public bool IsNativeAdReady()
    {
        foreach (var readyAdsInstance in _readyAdsInstances)
        {
            if (readyAdsInstance.IsNativeAdReady())
            {
                return true;
            }
        }

        return false;
    }

    public void HideNativeAd()
    {
        foreach (var readyAdsInstance in _readyAdsInstances)
            readyAdsInstance.HideNativeAd();
    }

    public INativeAd GetNativeAd()
    {
        if (IsNoAds()) return null;

        foreach (var readyAdsInstance in _readyAdsInstances)
        {
            if (readyAdsInstance.IsNativeAdReady())
            {
                return readyAdsInstance.GetNativeAd();
            }
        }

        return null;
    }
    #endregion
}