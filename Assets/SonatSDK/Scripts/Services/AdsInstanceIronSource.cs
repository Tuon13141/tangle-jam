#define dummy
//#define use_iron_source

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Object = System.Object;
using System.IO;
using Sonat;
#if use_aps
using AmazonAds;
#endif
#if !((dummy || global_dummy) && !use_iron_source)
using AppsFlyerSDK;

#endif

#if (dummy || global_dummy) && !use_iron_source
public class AdsInstanceIronSource : AdsInstance
{
    [SerializeField] private string sdkKey = "1660f7645";

    public override void ShowVideoAds()
    {
      //  IronSource.Agent.SetPauseGame(true);
       ////UIDebugLog.Log("Dummy : ShowVideoAds");
    }

    public override void LoadVideoAds()
    {
       ////UIDebugLog.Log("Dummy : LoadVideoAds");
    }

    public override bool IsVideoAdsReady()
    {
       ////UIDebugLog.Log("Dummy : IsVideoAdsReady");
        return false;
    }

    public override bool IsInterstitialReady()
    {
        return false;
    }

    public override void ShowInterstitial(bool isOnFocus)
    {
       
    }

    public override void CheckShowBanner()
    {
        
    }

    public override bool BannerIsShowing()
    {
        return false;
    }

    public override bool IntersIsShowing()
    {
        return false;
    }

    public override bool VideoIsShowing()
    {
        return false;
    }


    public override void DestroyBanner()
    {
    }

    public override bool IsInitialized()
    {
        return true;
    }

    public override void HideBanner()
    {
        
    }

    public override void ShowBanner()
    {
    }

    public override void HideNativeBanner()
    {
    }

    public override void ShowNativeBanner()
    {
    }

    public override bool IsNativeReady()
    {
        return false;
    }

    public override bool IsRemoteActive()
    {
        return RemoteConfigKey.mediation_platform.GetValueString() == "iron_source";
    }

    public override void RequestNewAds()
    {
        
    }
    
    public override void ShowDebugger()
    {

    }
    public override void SetupAction(int index, IntAction onInitialized, BooleanAction onVideoAdsLoaded,
        BaseAction onVideoAdsRewarded,
        BooleanAction bannerShowed, bool bannerShow, BaseAction onPaidAd = null)
    {
        ConsentReady = true;
        Initialized = true;
        base.SetupAction(index, onInitialized, onVideoAdsLoaded, onVideoAdsRewarded, bannerShowed, bannerShow, onPaidAd);
        CheckInitialized();
    }

    public override void ShowMrecBanner()
    {
        //TODO
    }

    public override void HideMrecBanner()
    {
        //TODO
    }

    public override bool IsMrecBannerReady()
    {
        //TODO
        return false;
    }

    public override bool ShowAppOpenAds()
    {
        //TODO
        return false;
    }

    public override bool IsAppOpenAdsReady()
    {
        //TODO
        return false;
    }

    public override void ShowCollapsibleBanner()
    {
        //TODO
    }

    public override void HideCollapsibleBanner()
    {
        //TODO
    }

    public override bool IsCollapsibleBannerReady()
    {
        //TODO
        return false;
    }

    public override bool IsNativeAdReady()
    {
        return false;
    }

    public override INativeAd GetNativeAd()
    {
        return null;
    }

    public override void HideNativeAd()
    {
    }
}
#else

// ReSharper disable InconsistentNaming

[RequireComponent(typeof(AskGDPRScript))]
public class AdsInstanceIronSource : AdsInstance
{
    [SerializeField] private string sdkKey = "1660f7645";

    [SerializeField] private bool launchTestSuite;
    private bool _initialized;
    public string bannerIdAndroid;
    public string interIdAndroid;
    public string rewardIdAndroid;
    public string openAppIdAndroid;

    public string bannerIdIOS;
    public string interIdIOS;
    public string rewardIdIOS;
    public string openAppIdIOS;

    private PlayerPrefRemoteString playerPrefBannerId;
    private PlayerPrefRemoteString playerPrefInterstitialId;
    private PlayerPrefRemoteString playerPrefNativeBannerId;
    private PlayerPrefRemoteString playerPrefOpenId;
    private PlayerPrefRemoteString playerPrefRewardedId;

    private AskGDPRScript askGDPR;


    public override bool IsInitialized()
    {
        return _initialized;
    }

    protected virtual string VideoId
    {
        get
        {
            if (playerPrefRewardedId == null)
#if UNITY_ANDROID
                playerPrefRewardedId = new PlayerPrefRemoteString(RemoteConfigKey.ironSource_rewarded_ad_id, rewardIdAndroid);
#elif UNITY_IOS
playerPrefRewardedId = new PlayerPrefRemoteString(RemoteConfigKey.ironSource_rewarded_ad_id, rewardIdIOS);
#endif

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_iron_source_rewarded_id.GetValueString(false)))
                return RemoteConfigKey.pam_iron_source_rewarded_id.GetValueString(false);


            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_iron_source_rewarded_id,
                    playerPrefRewardedId.DefaultValueWhenEmpty());

            return playerPrefRewardedId.DefaultValueWhenEmpty();
        }
    }

    protected virtual string InterId
    {
        get
        {
            if (playerPrefInterstitialId == null)
#if UNITY_ANDROID
                playerPrefInterstitialId = new PlayerPrefRemoteString(RemoteConfigKey.ironSource_inter_ad_id, interIdAndroid);
#elif UNITY_IOS
playerPrefInterstitialId = new PlayerPrefRemoteString(RemoteConfigKey.ironSource_inter_ad_id, interIdIOS);
#endif

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_iron_source_interstitial_id.GetValueString(false)))
                return RemoteConfigKey.pam_iron_source_interstitial_id.GetValueString(false);


            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_iron_source_interstitial_id,
                    playerPrefInterstitialId.DefaultValueWhenEmpty());

            return playerPrefInterstitialId.DefaultValueWhenEmpty();
        }
    }

    protected virtual string BannerId
    {
        get
        {
            if (playerPrefBannerId == null)
#if UNITY_ANDROID
                playerPrefBannerId = new PlayerPrefRemoteString(RemoteConfigKey.ironSource_banner_id, bannerIdAndroid);
#elif UNITY_IOS
            playerPrefBannerId = new PlayerPrefRemoteString(RemoteConfigKey.ironSource_banner_id, bannerIdIOS);
#endif

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_iron_source_banner_id.GetValueString(false)))
                return RemoteConfigKey.pam_iron_source_banner_id.GetValueString(false);


            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_iron_source_banner_id,
                    playerPrefBannerId.DefaultValueWhenEmpty());

            return playerPrefBannerId.DefaultValueWhenEmpty();
        }
    }


    public override void SetupAction(int index, IntAction onInitialized, BooleanAction onVideoAdsLoaded,
        BaseAction onVideoAdsRewarded,
        BooleanAction bannerShowed, bool bannerShow, BaseAction onPaidAd = null)
    {
        base.SetupAction(index, onInitialized, onVideoAdsLoaded, onVideoAdsRewarded, bannerShowed, bannerShow, onPaidAd);
        //ConsentReady = true;
        Initialized = true;
        askGDPR = GetComponent<AskGDPRScript>();
        if (askGDPR != null)
        {
            askGDPR.Ask(InitAdsInstance, OnTrackingYes);
        }
        else
        {
            ConsentReady = true;
            Kernel.Resolve<AdsManager>().ForceConsenReady();
            InitAdsInstance();
        }
        //InitAdsInstance();
    }

    public override void ShowDebugger()
    {

    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    public override void RequestNewAds()
    {
        Debug.Log(nameof(RequestNewAds));

        // _bannerView?.Destroy();
        // _bannerView = null;
        showBanner = false;
        _bannerState = AdsState.NotStart;

        // _interstitial?.Destroy();
        // _interstitial = null;
        _intersState = AdsState.NotStart;

        // _rewardedAd?.Destroy();
        // _rewardedAd = null;
        _videoState = AdsState.NotStart;
        VideoLoaded.Action.Invoke(false);

        Invoke(nameof(RequestBanner), 5);
        Invoke(nameof(RequestInterstitial), 10);
        Invoke(nameof(RequestRewardedAd), 12);

        Kernel.Resolve<FireBaseController>().LogEvent(nameof(RequestNewAds));
    }

    public class AdQualitySdkInit : ISAdQualityInitCallback
    {
        public void adQualitySdkInitSuccess()
        {
            Debug.Log("unity: adQualitySdkInitSuccess");
        }

        public void adQualitySdkInitFailed(ISAdQualityInitError adQualitySdkInitError, string errorMessage)
        {
            Debug.Log("unity: adQualitySdkInitFailed " + adQualitySdkInitError + " message: " + errorMessage);
        }
    }
    private bool inited;
    void InitAdsInstance()
    {
        if (inited) return;
        inited = true;
        ConsentReady = true;

#if use_aps && !UNITY_EDITOR
        Amazon.Initialize(amazonApiKey);
        Amazon.SetAdNetworkInfo(new AdNetworkInfo(DTBAdNetwork.IRON_SOURCE));
#endif

        Kernel.Resolve<AdsManager>().ForceConsenReady();
        IronSource.Agent.SetPauseGame(true);
        IronSource.Agent.setMetaData("do_not_sell", "false");
        IronSource.Agent.setMetaData("Meta_Mixed_Audience", "true");
        IronSource.Agent.setMetaData("is_child_directed", "false");
        if (launchTestSuite)
        {
            IronSource.Agent.setMetaData("is_test_suite", "enable");
        }
        IronSource.Agent.setUserId(SystemInfo.deviceUniqueIdentifier);
        //IronSource.Agent.setConsent(true);
        IronSourceEvents.onSdkInitializationCompletedEvent += HandleInitCompleteAction;

#if UNITY_ANDROID
        string bannerId = string.IsNullOrEmpty(BannerId) ? bannerIdAndroid : BannerId;
        string interId = string.IsNullOrEmpty(InterId) ? interIdAndroid : InterId;
        string videoId = string.IsNullOrEmpty(VideoId) ? rewardIdAndroid : VideoId;
#elif UNITY_IOS
string bannerId = string.IsNullOrEmpty(BannerId) ? bannerIdIOS : BannerId;
        string interId = string.IsNullOrEmpty(InterId) ? interIdIOS : InterId;
        string videoId = string.IsNullOrEmpty(VideoId) ? rewardIdIOS : VideoId;
#endif

        IronSource.Agent.init(sdkKey, videoId, interId, bannerId);
        Debug.Log($"[sonat check iron source] - start init iron source");
        //IronSource.Agent.init(sdkKey);
        UIDebugLog.Log("InitAdsInstance IronSource");
        //Debug.Log($"Banner id request: {BannerId}");
        //Debug.Log($"Inter id request: {InterId}");
        //Debug.Log($"Reward id request: {VideoId}");
#if UNITY_EDITOR
        IronSourceTestAds.Create(HandleOnInterstitialAdOpened, HandleOnInterstitialAdClosed, HandleVideoAdOpenedEvent, HandleRewardedAdClosed, OnAdReceivedRewardEvent);
        HandleInitCompleteAction();
#endif

        //AdQualitySdkInit adQualitySdkInit = new AdQualitySdkInit();
        //ISAdQualityConfig adQualityConfig = new ISAdQualityConfig
        //{
        //    AdQualityInitCallback = adQualitySdkInit
        //};
        //IronSourceAdQuality.Initialize(sdkKey, adQualityConfig);
    }


    public override void OnTrackingYes(bool value)
    {
        IronSource.Agent.setConsent(value);
        //IronSourceAdQuality.SetUserConsent(true);
    }

    public override bool BannerIsShowing()
    {
        UIDebugLog.Log(_bannerState.ToString());
        return _bannerState == AdsState.Loaded;
    }

    public override bool IsRemoteActive()
    {
        return false;
    }


    public override bool IntersIsShowing()
    {
        UIDebugLog.Log(_bannerState.ToString());
        return _intersState == AdsState.Showing;
    }

    public override bool VideoIsShowing()
    {
        UIDebugLog.Log(_bannerState.ToString());
        return _videoState == AdsState.Showing;
    }

    public override void DestroyBanner()
    {
        IronSource.Agent.destroyBanner();
        _bannerState = AdsState.Failed;
        OnBannerShow.Action.Invoke(false);
#if UNITY_EDITOR
        IronSourceTestAds.instan.DestroyBanner();
#endif
    }

    private void HandleInitCompleteAction()
    {
        _initialized = true;
        Debug.Log($"[sonat check iron source] - Iron source consent ready: {ConsentReady}");
        ConsentReady = true;
        InitializedEvent.Action.Invoke(_index);

        if (launchTestSuite)
        {
            IronSource.Agent.launchTestSuite();
        }

        IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;
        Debug.Log($"[sonat check iron source] - iron source init complete!");
        UIDebugLog.Log("ironSourced : _initialized");
        //#if !UNITY_EDITOR
        RequestBanner();
        //#endif
        _requestInterstitial = StartCoroutine(wait(2, RequestInterstitial));
        _requestReward = StartCoroutine(wait(5, RequestRewardedAd));

        StartCoroutine(wait(10, () =>
        {
            UIDebugLog.Log("IronSource.Agent.validateIntegration()");
            IronSource.Agent.validateIntegration();
        }));
    }



    IEnumerator wait(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }

    private bool _initCallBackBanner;

    private void RequestBanner()
    {
        //            MaxSdkCallbacks.Banner.OnAdExpandedEvent    += OnBannerAdExpandedEvent;
        //            MaxSdkCallbacks.Banner.OnAdCollapsedEvent   += OnBannerAdCollapsedEvent;

        if (!Kernel.IsInternetConnection())
        {
            UIDebugLog.Log("AdsInstanceIronSource: RequestBanner failed no Internet");
            HandleBannerAdFailedToLoadNoInternet();
            return;
        }

        if (!showBanner)
        {
            _bannerState = AdsState.NotStart;
            return;
        }

        if (_requestBanner != null)
            StopCoroutine(_requestBanner);
        if (_initialized)
        {
            UIDebugLog.Log("RequestBanner");

            //            MaxSdkCallbacks.Banner.OnAdLoadedEvent      += OnBannerAdLoadedEvent;
            //            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent  += OnBannerAdLoadFailedEvent;
            //            MaxSdkCallbacks.Banner.OnAdClickedEvent     += OnBannerAdClickedEvent;
            //            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
            //            MaxSdkCallbacks.Banner.OnAdExpandedEvent    += OnBannerAdExpandedEvent;
            //            MaxSdkCallbacks.Banner.OnAdCollapsedEvent   += OnBannerAdCollapsedEvent;

            if (!_initCallBackBanner)
            {
                _initCallBackBanner = true;
                IronSourceBannerEvents.onAdLoadedEvent += HandleBannerAdLoaded;
                IronSourceBannerEvents.onAdLoadFailedEvent += HandleBannerAdFailedToLoad;
                IronSourceBannerEvents.onAdClickedEvent += OnBannerAdClickedEvent;
                //                IronSourceEvents.onBannerAdScreenPresentedEvent += () => AdsManager.BannerOpened = true;
                //                IronSourceEvents.onBannerAdScreenDismissedEvent += () => StartCoroutine(WaitBannerFalse());
                //                IronSourceEvents.onBannerAdLeftApplicationEvent += () => AdsManager.AppLeaving = true;

                IronSourceBannerEvents.onAdScreenPresentedEvent += HandlerBannerAdScreenPresentedEvent;
                IronSourceBannerEvents.onAdScreenDismissedEvent += HandlerBannerAdScreenDismissedEvent;
                IronSourceBannerEvents.onAdLeftApplicationEvent += HandlerBannerAdLeftApplicationEvent;
            }
#if use_aps && !UNITY_EDITOR
            LoadAPSBanner();
#else
            IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
#endif

            _bannerState = AdsState.Requesting;
            UIDebugLog.Log("RequestBanner2");
        }

#if UNITY_EDITOR
        _bannerState = AdsState.Loaded;
        if (!AdsManager.NoAds && AdsManager.showBannerOnLoad)
        {
            ShowBanner();
        }
#endif
    }

    private void OnBannerAdClickedEvent(IronSourceAdInfo adInfo)
    {
        UIDebugLog.Log(nameof(OnBannerAdClickedEvent));
        AdsManager.BannerOpened = true;
    }


    private void HandlerBannerAdLeftApplicationEvent(IronSourceAdInfo adInfo)
    {
        UIDebugLog.Log(nameof(HandlerBannerAdLeftApplicationEvent));
        //AdsManager.AppLeaving = true;
    }

    private void HandlerBannerAdScreenDismissedEvent(IronSourceAdInfo adInfo)
    {
        UIDebugLog.Log(nameof(HandlerBannerAdScreenDismissedEvent));
        StartCoroutine(WaitBannerFalse());
    }

    private void HandlerBannerAdScreenPresentedEvent(IronSourceAdInfo adInfo)
    {
        UIDebugLog.Log(nameof(HandlerBannerAdScreenPresentedEvent));
        AdsManager.BannerOpened = true;
        if (adInfo != null)
        {
            SonatAnalyticTracker.LogRevenue(AdsPlatform.ironsource, adInfo.adNetwork, adInfo.revenue.Value, adInfo.precision,
                AdTypeLog.banner, Kernel.Resolve<FireBaseController>().FirebaseInstanceId, "banner");
        }
    }


    IEnumerator WaitBannerFalse()
    {
        yield return new WaitForSeconds(3);
        AdsManager.BannerOpened = false;
    }

    private void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
    {
        string allData = impressionData.allData;
        string adNetwork = impressionData.adNetwork;
        double? revenue = impressionData.revenue;
        UIDebugLog.Log(
            $"duong ImpressionDataReadyEvent \n impressionData.adNetwork{impressionData.adNetwork} \n allData{allData}");
        if (revenue != null)
        {
            SonatAnalyticTracker.LogRevenue(AdsPlatform.ironsource, adNetwork, revenue.Value,
                impressionData.precision, AdTypeLog.undefined, Kernel.Resolve<FireBaseController>().FirebaseInstanceId,
                "undefined");
        }
    }

    public void LogFirebaseRevenue(string platform, string adNetwork, double revenue, string precision,
        string adType, string currencyCode = "USD")
    {
        Firebase.Analytics.Parameter[] LTVParameters =
        {
            new Firebase.Analytics.Parameter("valuemicros", revenue * 1000000f),
            new Firebase.Analytics.Parameter("value", (float) revenue),
            // These values below won’t be used in ROAS recipe.
            // But log for purposes of debugging and future reference.
            new Firebase.Analytics.Parameter("currency", currencyCode),
            new Firebase.Analytics.Parameter("precision", precision),
            new Firebase.Analytics.Parameter("ad_format", adType),
            new Firebase.Analytics.Parameter("ad_source", adNetwork),
            new Firebase.Analytics.Parameter("ad_platform", platform),
            //new Firebase.Analytics.Parameter("adunitid", adUnitId),
            //new Firebase.Analytics.Parameter("network", this.rewardedAd.MediationAdapterClassName())
        };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("paid_ad_impression", LTVParameters);
    }

    private bool _initCallBackInters;

    private void RequestInterstitial()
    {
        UIDebugLog.Log(nameof(RequestInterstitial));
        if (!Kernel.IsInternetConnection())
        {
            UIDebugLog.Log("AdsInstanceAdmob: RequestInterstitial failed no Internet");
            HandleOnInterstitialAdFailedNoInternet();
            return;
        }

        _intersState = AdsState.Requesting;
        if (_requestInterstitial != null)
            StopCoroutine(_requestInterstitial);
#if use_aps && !UNITY_EDITOR
        LoadAPSInter();
#else
        IronSource.Agent.loadInterstitial();
#endif
        if (!_initCallBackInters)
        {
            _initCallBackInters = true;
            IronSourceInterstitialEvents.onAdReadyEvent += HandleOnInterstitialAdLoaded;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += HandleOnInterstitialAdFailedToLoad;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += HandleInterOpeningEvent;
            IronSourceInterstitialEvents.onAdShowFailedEvent += HandleOnInterstitialAdFailedToShow;
            //   IronSourceEvents.onInterstitialAdClickedEvent += () => AdsManager.AppLeaving = true;
            IronSourceInterstitialEvents.onAdOpenedEvent += HandleOnInterstitialAdOpened;
            IronSourceInterstitialEvents.onAdClosedEvent += HandleOnInterstitialAdClosed;

            //            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += HandleOnInterstitialAdLoaded;
            //            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += HandleOnInterstitialAdFailedToLoad;
            //            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += HandleInterOpeningEvent;
            //            //   MaxSdkCallbacks.Interstitial.OnAdClickedEvent += HandleOnInterstitialAdClosed;
            //            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += HandleOnInterstitialAdClosed;
            //            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += HandleOnInterstitialAdFailedToShow;
            //            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += HandleInterstitialAdPaidEvent;
        }
    }

    private bool _initCallBackReward;

    public void RequestRewardedAd()
    {
        UIDebugLog.Log(": RequestRewardedAd");
        if (!Kernel.IsInternetConnection())
        {
            UIDebugLog.Log("AdsInstanceAdmob: RequestInterstitial failed no Internet");
            HandleOnRewardAdFailedToLoadNoInternet();
            return;
        }

        _videoState = AdsState.Requesting;
        if (_requestReward != null)
            StopCoroutine(_requestReward);
#if use_aps && !UNITY_EDITOR
        LoadAPSRewardedVideo();
#else
        IronSource.Agent.loadRewardedVideo();
#endif

        if (!_initCallBackReward)
        {
            _initCallBackReward = true;
            IronSourceRewardedVideoEvents.onAdReadyEvent += HandleRewardedAdLoaded;
            IronSourceRewardedVideoEvents.onAdLoadFailedEvent += HandleOnRewardAdFailedToLoad;

            IronSourceRewardedVideoEvents.onAdOpenedEvent += HandleVideoAdOpenedEvent;
            //            IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += HandleRewardedAdClosed;
            //            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
            //            IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
            //            IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += OnAdReceivedRewardEvent;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += OnAdDisplayFailedEvent;

            //            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += HandleRewardedAdLoaded;
            //            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += HandleOnRewardAdFailedToLoad;
            //            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += HandleVideoAdOpeningEvent;
            //            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += Handler;
            //            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += HandleVideoAdPaidEvent;
            //            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += HandleRewardedAdClosed;
            //            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnAdDisplayFailedEvent;
            //            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedRewardEvent;
        }

#if UNITY_EDITOR
        VideoLoaded.Action.Invoke(true);
#endif
    }




    private AdsState _bannerState;
    private AdsState _intersState;
    private AdsState _videoState;
    private AdsState _openAdsState;

    public override void CheckShowBanner()
    {
        switch (_bannerState)
        {
            case AdsState.Requesting:
                break;
            case AdsState.Failed:
                RequestBanner();
                break;
            case AdsState.Loaded:
                break;
            case AdsState.Showing:
                break;
            case AdsState.Closed:
                break;
            case AdsState.NotStart:
                RequestBanner();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void HandleVideoAdOpenedEvent(IronSourceAdInfo adInfo)
    {

        _videoState = AdsState.Showing;
        AdsManager.VideoOpened = true;
        //        if (RxKernel.Instance != null && !GameRoot.Instance.Pause.Value)
        //            Time.timeScale = 0f;
        if (adInfo != null)
            Debug.Log($"RewardId: {adInfo.instanceId}");
    }

    private void HandleInterOpeningEvent(IronSourceAdInfo adInfo)
    {
        _intersState = AdsState.Showing;

        //        if (RxKernel.Instance != null && !GameRoot.Instance.Pause.Value)
        //            Time.timeScale = 0f;
    }

    //    private void OnApplicationPause(bool pause)
    //    {
    //        UIDebugLog.Log(""+nameof(OnApplicationPause) + " !pause"+!pause+" "+_earned);
    //        if (_earned && !pause)
    //        {
    //            UIDebugLog.Log(""+nameof(OnApplicationPause) + " "+(OnVideoAdsFinishedEvent != null));
    //            OnVideoAdsFinishedEvent?.Invoke();
    //            _earned = false;
    //        }
    //    }

    private float _lastTimeFocus;

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            if (Time.realtimeSinceStartup - _lastTimeFocus > 30 * 60)
            {
                RequestBanner();

                _requestInterstitial = StartCoroutine(wait(2, RequestInterstitial));
                _requestReward = StartCoroutine(wait(5, RequestRewardedAd));
            }

            _lastTimeFocus = Time.realtimeSinceStartup;
        }
    }

    #region Banner callback handlers

    public void HandleBannerAdLoaded(IronSourceAdInfo adInfo)
    {
        Debug.Log($"BannerId: {adInfo.instanceId}");
        UIDebugLog.Log("Banner loaded");
        _bannerState = AdsState.Loaded;
        if (!AdsManager.NoAds && AdsManager.showBannerOnLoad)
        {
            ShowBanner();
        }
        else
        {
            HideBanner();
        }




        //        float height = _bannerView.GetHeightInPixels();
        //        if (ScreenRoot.Instance != null)
        //        {
        //            //            ScreenRoot.Instance.AdaptivePanel.Height = height;
        //            //            FPSDisplay.Message = height.ToString(CultureInfo.InvariantCulture);
        //#if UNITY_ANDROID
        //            var rectTransform = ScreenRoot.Instance.adaptivePanel.rectTransform;
        //            var size = rectTransform.sizeDelta;
        //            size.y = height * 1.1f;
        //            rectTransform.sizeDelta = size;
        //            ScreenRoot.Instance.canvasForAdaptive.gameObject.SetActive(true);
        //#endif
        //            ScreenRoot.Instance.canvasForAdaptive.gameObject.SetActive(true);
        //        }
    }

    int _retryBannerAttempt;
    private Coroutine _requestBanner;


    public void HandleBannerAdFailedToLoadNoInternet()
    {
        _bannerState = AdsState.Failed;
        _retryBannerAttempt++;
        double retryDelay = Math.Pow(2, Mathf.Clamp(_retryBannerAttempt, 3, 7));
        _requestBanner = StartCoroutine(wait((float)retryDelay, RequestBanner));
        UIDebugLog.Log("" + nameof(HandleBannerAdFailedToLoadNoInternet) + " retryDelay-" + retryDelay);
    }

    public void HandleBannerAdFailedToLoad(IronSourceError args)
    {
        _bannerState = AdsState.Failed;
        //        if (ScreenRoot.Instance != null)
        //            ScreenRoot.Instance.canvasForAdaptive.gameObject.SetActive(false);

        _retryBannerAttempt++;
        double retryDelay = Math.Pow(2, Mathf.Clamp(_retryBannerAttempt, 3, 7));
        //        _bannerView?.Destroy();
        //        _bannerView = null;

        _requestBanner = StartCoroutine(wait((float)retryDelay, RequestBanner));

        UIDebugLog.Log("" + nameof(HandleBannerAdFailedToLoad) + " retryDelay-" + retryDelay + " :" +
                       args.getDescription());
    }

    public void HandleBannerAdOpened()
    {
        //        AdsManager.BannerShowed = true;
        //        if (ScreenRoot.Instance != null)
        //            ScreenRoot.Instance.canvasForAdaptive.gameObject.SetActive(false);
        UIDebugLog.Log("" + nameof(HandleBannerAdOpened) + " event received");
    }

    public void HandleBannerAdClosed()
    {
        RequestBanner();
        UIDebugLog.Log("" + nameof(HandleBannerAdClosed) + " event received");
        //        UIRoot.Instance?.AdaptivePanel.SetDefault();
        //        if (ScreenRoot.Instance != null)
        //            ScreenRoot.Instance.canvasForAdaptive.gameObject.SetActive(false);
    }

    public void HandleAdLeftApplication()
    {
        UIDebugLog.Log("" + nameof(HandleBannerAdClosed) + " event received");
    }

    public override void HideBanner()
    {
        //if(_bannerState < AdsState.Loaded)
        //{
        AdsManager.showBannerOnLoad = false;
        //}
        //else
        //{
        IronSource.Agent.hideBanner();
        if (_bannerState == AdsState.Showing)
            _bannerState = AdsState.Hidden;
        //}

#if UNITY_EDITOR
        IronSourceTestAds.instan.HideBanner();
#endif

        OnBannerShow.Action.Invoke(false);
    }

    public override void ShowBanner()
    {
        //if (_bannerState == AdsState.Hidden)
        //{
        AdsManager.showBannerOnLoad = true;
        IronSource.Agent.displayBanner();
        _bannerState = AdsState.Showing;
        OnBannerShow.Action.Invoke(true);
        //}
        //else
        //{
        //OnBannerShow.Action.Invoke(false);
        //}
#if UNITY_EDITOR
        IronSourceTestAds.instan.ShowBanner();
#endif
    }

    public override void HideNativeBanner()
    {
        //        if (_nativeBannerState == AdsState.Loaded)
        //        {
        //            _nativeBannerView?.Hide();
        //            _nativeBannerState = AdsState.Hidden;
        //        }
    }

    public override void ShowNativeBanner()
    {
        //        if (_nativeBannerState == AdsState.Hidden)
        //        {
        //            _nativeBannerView?.Show();
        //            _nativeBannerState = AdsState.Loaded;
        //        }
    }

    public override bool IsNativeReady()
    {
        return false;
    }

    #endregion

    #region Rewarded

    private bool _earned;

    int _retryRewardAttempt;
    private Coroutine _requestReward;
    private Coroutine _requestInterstitial;


    private void HandleOnRewardAdFailedToLoadNoInternet()
    {
        _retryRewardAttempt++;
        double retryDelay = Math.Pow(2, Mathf.Clamp(_retryRewardAttempt, 3, 7));
        UIDebugLog.Log("" + nameof(HandleOnRewardAdFailedToLoadNoInternet) + " retryDelay-" + retryDelay);
        _requestReward = StartCoroutine(wait((float)retryDelay, RequestRewardedAd));
    }


    private void HandleOnRewardAdFailedToLoad(IronSourceError errorInfo)
    {
        _retryRewardAttempt++;
        double retryDelay = Math.Pow(2, Mathf.Clamp(_retryRewardAttempt, 3, 7));
        UIDebugLog.Log("" + nameof(HandleOnRewardAdFailedToLoad) + " retryDelay-" + retryDelay + " :" +
                       errorInfo.getDescription());
        // _requestInterstitial = StartCoroutine(wait(2, RequestInterstitial, false));
        _requestReward = StartCoroutine(wait((float)retryDelay, RequestRewardedAd));
    }


    private void OnAdDisplayFailedEvent(IronSourceError errorInfo, IronSourceAdInfo adInfo)
    {
        _retryRewardAttempt++;
        double retryDelay = Math.Pow(2, Mathf.Clamp(_retryRewardAttempt, 3, 7));
        UIDebugLog.Log(nameof(OnAdDisplayFailedEvent) + " retryDelay-" + retryDelay + " :" +
                       errorInfo.getDescription());

        _requestReward = StartCoroutine(wait((float)retryDelay, RequestRewardedAd));
    }

    public void HandleRewardedAdClosed(IronSourceAdInfo adInfo)
    {
        UIDebugLog.Log("" + nameof(HandleRewardedAdClosed) + " _earned " + _earned);
        _videoState = AdsState.Closed;
        if (_earned)
        {
#if UNITY_IOS
            VideoRewarded?.Action.Invoke();
            _earned = false;
#endif
        }

        _requestReward = StartCoroutine(wait((float).5f, RequestRewardedAd));
        StartCoroutine(waitToRewardFalse());
    }


    IEnumerator waitToRewardFalse()
    {
        yield return new WaitForSeconds(3);
        AdsManager.VideoOpened = false;
    }

    private void HandleRewardedAdLoaded(IronSourceAdInfo adInfo)
    {
        _retryRewardAttempt = 1;
        UIDebugLog.Log("" + nameof(HandleRewardedAdLoaded) + " - Rewarded video loaded:" + IsVideoAdsReady());
        //VideoRewarded?.Action.Invoke();
        VideoLoaded.Action.Invoke(true);
    }

    private void OnAdReceivedRewardEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        UIDebugLog.Log(nameof(OnAdReceivedRewardEvent));
#if UNITY_ANDROID
        // _earned = true;
        VideoRewarded?.Action.Invoke();
#else
        VideoRewarded?.Action.Invoke();
#endif
        if (adInfo != null)
        {
            SonatAnalyticTracker.LogRevenue(AdsPlatform.ironsource, adInfo.adNetwork, adInfo.revenue.Value, adInfo.precision,
        AdTypeLog.rewarded, Kernel.Resolve<FireBaseController>().FirebaseInstanceId, SonatAnalyticTracker.RewardedLogPlacement);
        }
    }


    public override void ShowVideoAds()
    {
        if (IsVideoAdsReady())
        {
            IronSource.Agent.showRewardedVideo();
            _earned = false;
#if UNITY_EDITOR
            IronSourceTestAds.instan.ShowReward();
#endif
        }
    }

    public override void LoadVideoAds()
    {
        // load video should be handle internal
        //        if (!rewardedAd.IsLoaded())
        //            RequestRewardedAd();
    }

    public override bool IsVideoAdsReady()
    {
#if UNITY_EDITOR
        return true;
#endif
        return IronSource.Agent.isRewardedVideoAvailable();
    }

    #endregion

    #region interstitial

    public override bool IsInterstitialReady()
    {
#if UNITY_EDITOR
        return true;
#endif
        return IronSource.Agent.isInterstitialReady();
    }

    public override void ShowInterstitial(bool isOnFocus)
    {
        //if (!PlayerPrefsX.GetBool("PurchasedUser"))
        //{
        if (IsInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
#if UNITY_EDITOR
            IronSourceTestAds.instan.ShowInter();
#endif
        }

        //else
        //    LoadInterstitial();
        //}
    }

    int retryIntersAttempt;

    private void HandleOnInterstitialAdLoaded(IronSourceAdInfo adInfo)
    {
        retryIntersAttempt = 1;
        Debug.Log($"InterId: {adInfo.instanceId}");
        //        interstitialAdapter = adInfo.NetworkName;
        UIDebugLog.Log("" + nameof(HandleOnInterstitialAdLoaded) + " event received");
    }

    private void HandleOnInterstitialAdFailedNoInternet()
    {
        retryIntersAttempt++;
        double retryDelay = Math.Pow(2, Mathf.Clamp(retryIntersAttempt, 3, 8));
        UIDebugLog.Log("" + nameof(HandleOnInterstitialAdFailedNoInternet) + " reload in:" + retryDelay);
        _requestInterstitial = StartCoroutine(wait((float)retryDelay, RequestInterstitial));
    }

    private void HandleOnInterstitialAdFailedToLoad(IronSourceError err)
    {
        retryIntersAttempt++;
        double retryDelay = Math.Pow(2, Mathf.Clamp(retryIntersAttempt, 3, 8));
        UIDebugLog.Log("" + nameof(HandleOnInterstitialAdFailedToLoad) + " event received with message: (" +
                       (float)retryDelay +
                       ") " + err.getDescription());

        _requestInterstitial = StartCoroutine(wait((float)retryDelay, RequestInterstitial));
    }

    private void HandleOnInterstitialAdFailedToShow(IronSourceError err, IronSourceAdInfo adInfo)
    {
        RequestInterstitial();
        UIDebugLog.Log("" + nameof(HandleOnInterstitialAdFailedToShow) + " event received with message: " +
                       err.getDescription());
    }

    public void HandleOnInterstitialAdOpened(IronSourceAdInfo adInfo)
    {
        AdsManager.OpenAdsOrInterstitialOpened = true;
        //        FireBaseController.LogEvent("interstitial", new[]
        //        {
        //            new LogParameter("event", "opened")
        //        });
        if (adInfo != null)
        {
            SonatAnalyticTracker.LogRevenue(AdsPlatform.ironsource, adInfo.adNetwork, adInfo.revenue.Value, adInfo.precision,
        AdTypeLog.interstitial, Kernel.Resolve<FireBaseController>().FirebaseInstanceId, SonatAnalyticTracker.InterstitialLogName);
            UIDebugLog.Log("" + nameof(HandleOnInterstitialAdOpened) + " event received");
        }
    }

    public void HandleOnInterstitialAdClosed(IronSourceAdInfo adInfo)
    {
        _intersState = AdsState.Closed;
        //        if (RxKernel.Instance != null && !GameRoot.Instance.Pause.Value)
        //            Time.timeScale = 1f;

        //        FireBaseController.LogEvent("interstitial", new[]
        //        {
        //            new LogParameter("event", "closed")
        //        });
        UIDebugLog.Log("" + nameof(HandleOnInterstitialAdClosed) + " event received");
        RequestInterstitial();
        StartCoroutine(waitToInterFalse());
    }

    IEnumerator waitToInterFalse()
    {
        yield return new WaitForSeconds(3);
        AdsManager.OpenAdsOrInterstitialOpened = false;
    }

    public void HandleOnInterstitialAdLeavingApplication()
    {
        AppsFlyer.sendEvent("af_ad_click", new Dictionary<string, string>()
        {
            {"af_adrev_ad_type", ""}
        });
        UIDebugLog.Log("" + nameof(HandleOnInterstitialAdLeavingApplication) + " event received");
    }

    #endregion

#if use_aps
    private string amazonApiKey
    {
        get =>
#if UNITY_ANDROID
        amazonApiKeyAndroid;
#else
        amazonApiKeyIos;
#endif
    }
    private string amazonBannerId
    {
        get =>
#if UNITY_ANDROID
            amazonBannerIdAndroid;
#else
            amazonBannerIdIos;
#endif
    }
    private string amazonBannerLargeId
    {
        get =>
#if UNITY_ANDROID
            amazonBannerLargeIdAndroid;
#else
            amazonBannerLargeIdIos;
#endif
    }
    private string amazonStaticInterId
    {
        get =>
#if UNITY_ANDROID
            amazonStaticInterIdAndroid;
#else
            amazonStaticInterIdIos;
#endif
    }
    private string amazonVideoInterId
    {
        get =>
#if UNITY_ANDROID
            amazonVideoInterIdAndroid;
#else
            amazonVideoInterIdIos;
#endif
    }
    private string amazonRewardId
    {
        get =>
#if UNITY_ANDROID
            amazonRewardIdAndroid;
#else
            amazonRewardIdIos;
#endif
    }

    [Header("Amazon Android Ids")]
    public string amazonApiKeyAndroid;
    public string amazonBannerIdAndroid;
    public string amazonBannerLargeIdAndroid;
    public string amazonStaticInterIdAndroid;
    public string amazonVideoInterIdAndroid;
    public string amazonRewardIdAndroid;

    [Header("Amazon ios Ids")]
    public string amazonApiKeyIos;
    public string amazonBannerIdIos;
    public string amazonBannerLargeIdIos;
    public string amazonStaticInterIdIos;
    public string amazonVideoInterIdIos;
    public string amazonRewardIdIos;


    private APSBannerAdRequest bannerAdRequest;

    private void LoadAPSBanner()
    {
        int width;
        int height;
        string slotId;
        if (IsTablet())
        {
            width = 728;
            height = 90;
            slotId = string.IsNullOrEmpty(amazonBannerLargeId) ? amazonBannerId : amazonBannerLargeId;
        }
        else
        {
            width = 320;
            height = 50;
            slotId = amazonBannerId;
        }

        bannerAdRequest = new APSBannerAdRequest(width, height, slotId);
        bannerAdRequest.onFailedWithError += (adError) =>
            {
                IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
            };
        bannerAdRequest.onSuccess += (adResponse) =>
        {
            IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY,
                                               APSMediationUtils.GetBannerNetworkData(slotId, adResponse));
            IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
        };
        bannerAdRequest.LoadAd();
    }


    private APSInterstitialAdRequest interstitialAdRequest;
    private APSVideoAdRequest interstitialVideoAdRequest;
    private bool staticInterLoaded = false;
    private bool videoInterLoaded = false;
    private void LoadAPSInter()
    {
        interstitialAdRequest = new APSInterstitialAdRequest(amazonStaticInterId);
        interstitialVideoAdRequest = new APSVideoAdRequest(320, 480, amazonVideoInterId);
        interstitialAdRequest.onSuccess += (adResponse) =>
            {
                IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY
     , APSMediationUtils.GetInterstitialNetworkData(amazonStaticInterId, adResponse));
                staticInterLoaded = true;
                if (videoInterLoaded && staticInterLoaded)
                    IronSource.Agent.loadInterstitial();
            };
        interstitialAdRequest.onFailedWithError += (adError) =>
        {
            staticInterLoaded = true;
            if (videoInterLoaded && staticInterLoaded)
                IronSource.Agent.loadInterstitial();
            Debug.Log("onFailedWithError interstitial:" + adError);
        };
        interstitialVideoAdRequest.onSuccess += (adResponse) =>
        {
            IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY,
                                                APSMediationUtils.GetInterstitialNetworkData(amazonVideoInterId, adResponse));
            videoInterLoaded = true;
            if (videoInterLoaded && staticInterLoaded)
                IronSource.Agent.loadInterstitial();
        };
        interstitialVideoAdRequest.onFailedWithError += (adError) =>
        {
            videoInterLoaded = true;
            if (videoInterLoaded && staticInterLoaded)
                IronSource.Agent.loadInterstitial();
        };

        interstitialAdRequest.LoadAd();
        interstitialVideoAdRequest.LoadAd();
    }

    private APSVideoAdRequest rewardedVideoAdRequest;
    public void LoadAPSRewardedVideo()
    {
        rewardedVideoAdRequest = new APSVideoAdRequest(320, 480, amazonRewardId);
        rewardedVideoAdRequest.onSuccess += (adResponse) =>
        {
            IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY,
                                                APSMediationUtils.GetRewardedNetworkData(amazonRewardId, adResponse));
            IronSource.Agent.loadRewardedVideo(); // If manual rewarded mode
        };
        interstitialVideoAdRequest.onFailedWithError += (adError) =>
        {
            IronSource.Agent.loadRewardedVideo(); // If manual rewarded mode
        };
        rewardedVideoAdRequest.LoadAd();
    }


#endif
    public static bool IsTablet()
    {

        float ssw;
        if (Screen.width > Screen.height) { ssw = Screen.width; } else { ssw = Screen.height; }

        if (ssw < 800) return false;

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            float screenWidth = Screen.width / Screen.dpi;
            float screenHeight = Screen.height / Screen.dpi;
            float size = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
            if (size >= 6.5f) return true;
        }

        return false;
    }
}
#endif