#define dummy

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Sonat;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using LogType = sonat_sdk.Scripts.LogType;
using Object = System.Object;
#if !((dummy || global_dummy) && !use_max)
using System.IO;
using AppsFlyerSDK;
#if use_aps
using AmazonAds;
#endif
#endif
#if (dummy || global_dummy) && !use_max
public partial class AdsInstanceMaxLovin : AdsInstance
{
    public string[] testDeviceIds = new[] {"B15BB24370A69E2E02CE1EA2F2E5BD10"}; 

    public string bannerId;
    public string bannerId_ios;
    public string interstitialId;
    public string interstitialId_ios;
    public string videoId;
    public string videoId_ios;
    public string openId;
    public string openId_ios;
    public string mrecBannerId;
    public string mrecBannerId_ios;
    public string nativeBannerId;
    public string nativeBannerId_ios;

    public override void ShowVideoAds()
    {
       ////Debug.Log("Dummy : ShowVideoAds");
    }

    public override void LoadVideoAds()
    {
       ////Debug.Log("Dummy : LoadVideoAds");
    }

    public override bool IsVideoAdsReady()
    {
       ////Debug.Log("Dummy : IsVideoAdsReady");
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

    public override void ShowMrecBanner()
    {
    }

    public override void HideMrecBanner()
    {
    }

    public override bool IsMrecBannerReady()
    {
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

    public override bool IsRemoteActive()
    {
       return false;
    }

    public override void RequestNewAds()
    {
    }
    
    public override void ShowDebugger()
    {

    }

    public override void ShowCollapsibleBanner()
    {
    }

    public override void HideCollapsibleBanner()
    {
    }

    public override bool IsCollapsibleBannerReady()
    {
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
using AppLovinMax;

// ReSharper disable InconsistentNaming
public partial class AdsInstanceMaxLovin : AdsInstance
{
    public string[] testDeviceIds = new[] { "B15BB24370A69E2E02CE1EA2F2E5BD10" };
    private string lastBannerId;

#if UNITY_ANDROID
    private PlayerPrefRemoteString playerPrefBannerId;
    private PlayerPrefRemoteString playerPrefInterstitialId;
    private PlayerPrefRemoteString playerPrefMrecBannerId;
    private PlayerPrefRemoteString playerPrefOpenId;
    private PlayerPrefRemoteString playerPrefRewardedId;
    private PlayerPrefRemoteString playerPrefNativeBannerId;

    protected virtual string BannerId
    {
        get
        {
            if (testAds)
                return TestBannerId;
            if (playerPrefBannerId == null)
                playerPrefBannerId = new PlayerPrefRemoteString(RemoteConfigKey.max_banner_ad_id, bannerId);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_max_banner_id.GetValueString(false)))
                return RemoteConfigKey.pam_max_banner_id.GetValueString(false);


            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_max_banner_id,
                    playerPrefBannerId.DefaultValueWhenEmpty());

            return playerPrefBannerId.DefaultValueWhenEmpty();
        }
    }

    protected virtual string InterstitialId
    {
        get
        {
            if (testAds)
                return TestInterstitialId;
            if (playerPrefInterstitialId == null)
                playerPrefInterstitialId =
                    new PlayerPrefRemoteString(RemoteConfigKey.max_inter_ad_id, interstitialId);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_max_interstitial_id.GetValueString(false)))
                return RemoteConfigKey.pam_max_interstitial_id.GetValueString(false);

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_max_interstitial_id,
                    playerPrefInterstitialId.DefaultValueWhenEmpty());

            return playerPrefInterstitialId.DefaultValueWhenEmpty();
        }
    }
    protected virtual string VideoId
    {
        get
        {
            if (testAds)
                return TestVideoId;
            if (playerPrefRewardedId == null)
                playerPrefRewardedId = new PlayerPrefRemoteString(RemoteConfigKey.max_rewarded_ad_id, videoId);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_max_rewarded_id.GetValueString(false)))
                return RemoteConfigKey.pam_max_rewarded_id.GetValueString(false);


            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_max_rewarded_id,
                    playerPrefRewardedId.DefaultValueWhenEmpty());

            return playerPrefRewardedId.DefaultValueWhenEmpty();
        }
    }

    protected virtual string OpenId
    {
        get
        {
            if (testAds)
                return TestOpenId;
            if (playerPrefOpenId == null)
                playerPrefOpenId = new PlayerPrefRemoteString(RemoteConfigKey.max_open_ad_id, openId);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_max_open_id.GetValueString(false)))
                return RemoteConfigKey.pam_max_open_id.GetValueString(false);

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_max_open_id,
                    playerPrefOpenId.DefaultValueWhenEmpty());


            return playerPrefOpenId.DefaultValueWhenEmpty();
        }
    }

    protected virtual string MRECBannerId
    {
        get
        {
            if (testAds)
                return TestMrecBannerId;
            if (playerPrefMrecBannerId == null)
                playerPrefMrecBannerId =
                    new PlayerPrefRemoteString(RemoteConfigKey.max_mrec_banner_ad_id, mrecBannerId);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_max_mrec_id.GetValueString(false)))
                return RemoteConfigKey.pam_max_mrec_id.GetValueString(false);


            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_max_merec_id,
                    playerPrefMrecBannerId.DefaultValueWhenEmpty());


            return playerPrefMrecBannerId.DefaultValueWhenEmpty();
        }
    }

    protected virtual string NativeBannerId
    {
        get
        {
            if (testAds)
                return TestNativeBannerId;
            if (playerPrefNativeBannerId == null)
                playerPrefNativeBannerId =
                    new PlayerPrefRemoteString(RemoteConfigKey.max_native_banner_ad_id, nativeBannerId);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_max_native_id.GetValueString(false)))
                return RemoteConfigKey.pam_max_native_id.GetValueString(false);

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_max_native_id,
                    playerPrefNativeBannerId.DefaultValueWhenEmpty());

            return playerPrefNativeBannerId.DefaultValueWhenEmpty();
        }
    }

    private const string TestBannerId = "ca-app-pub-3940256099942544/6300978111";
    private const string TestInterstitialId = "ca-app-pub-3940256099942544/1033173712";
    private const string TestVideoId = "ca-app-pub-3940256099942544/5224354917";
    private const string TestOpenId = "ca-app-pub-3940256099942544/3419835294";
    private const string TestMrecBannerId = "";
    private const string TestNativeBannerId = "ca-app-pub-3940256099942544/6300978111";
#endif

#if UNITY_IOS
    private PlayerPrefRemoteString playerPrefBannerId;
    private PlayerPrefRemoteString playerPrefInterstitialId;
    private PlayerPrefRemoteString playerPrefMrecBannerId;
    private PlayerPrefRemoteString playerPrefOpenId;
    private PlayerPrefRemoteString playerPrefRewardedId;
    private PlayerPrefRemoteString playerPrefNativeBannerId;

//#if UNITY_IOS*
    protected virtual string BannerId
    {
        get
        {
            if (testAds)
                return TestBannerId;
            if (playerPrefBannerId == null)
                playerPrefBannerId = new PlayerPrefRemoteString(RemoteConfigKey.max_banner_ad_id, bannerId_ios);

       if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_max_banner_id.GetValueString(false)))
                return RemoteConfigKey.pam_max_banner_id.GetValueString(false);

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_max_banner_id,playerPrefBannerId.DefaultValueWhenEmpty());

            return playerPrefBannerId.Value;
        }
    }

    protected virtual string InterstitialId
    {
        get
        {
            if (testAds)
                return TestInterstitialId;
            if (playerPrefInterstitialId == null)
                playerPrefInterstitialId =
                    new PlayerPrefRemoteString(RemoteConfigKey.max_inter_ad_id, interstitialId_ios);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_max_interstitial_id.GetValueString(false)))
                return RemoteConfigKey.pam_max_interstitial_id.GetValueString(false);

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_max_interstitial_id,playerPrefInterstitialId.DefaultValueWhenEmpty());

            return playerPrefInterstitialId.Value;
        }
    }

    protected virtual string VideoId
    {
        get
        {
            if (testAds)
                return TestVideoId;
            if (playerPrefRewardedId == null)
                playerPrefRewardedId =
                    new PlayerPrefRemoteString(RemoteConfigKey.max_rewarded_ad_id, videoId_ios);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_max_rewarded_id.GetValueString(false)))
                return RemoteConfigKey.pam_max_rewarded_id.GetValueString(false);



            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_max_rewarded_id,playerPrefRewardedId.DefaultValueWhenEmpty());

            return playerPrefRewardedId.Value;
        }
    }

    protected virtual string OpenId
    {
        get
        {
            if (testAds)
                return TestOpenId;
            if (playerPrefOpenId == null)
                playerPrefOpenId = new PlayerPrefRemoteString(RemoteConfigKey.max_open_ad_id, openId_ios);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_max_open_id.GetValueString(false)))
                return RemoteConfigKey.pam_max_open_id.GetValueString(false);

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_max_open_id,playerPrefOpenId.DefaultValueWhenEmpty());

            return playerPrefOpenId.Value;
        }
    }

    protected virtual string MRECBannerId
    {
        get
        {
            if (testAds)
                return TestMrecBannerId;
            if (playerPrefMrecBannerId == null)
                playerPrefMrecBannerId = new PlayerPrefRemoteString(RemoteConfigKey.max_mrec_banner_ad_id,
                    mrecBannerId_ios);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_max_mrec_id.GetValueString(false)))
                return RemoteConfigKey.pam_max_mrec_id.GetValueString(false);


            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_max_merec_id, playerPrefMrecBannerId.DefaultValueWhenEmpty());

            return playerPrefMrecBannerId.Value;
        }
    }

    protected virtual string NativeBannerId
    {
        get
        {
            if (testAds)
                return TestNativeBannerId;
            if (playerPrefNativeBannerId == null)
                playerPrefNativeBannerId =
                    new PlayerPrefRemoteString(RemoteConfigKey.max_native_banner_ad_id, nativeBannerId_ios);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_max_native_id.GetValueString(false)))
                return RemoteConfigKey.pam_max_native_id.GetValueString(false);

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_max_native_id,
                    playerPrefNativeBannerId.DefaultValueWhenEmpty());

            return playerPrefNativeBannerId.DefaultValueWhenEmpty();
        }
    }

    private const string TestBannerId = "ca-app-pub-3940256099942544/2934735716";
    private const string TestInterstitialId = "ca-app-pub-3940256099942544/4411468910";
    private const string TestVideoId = "ca-app-pub-3940256099942544/1712485313";
    private const string TestOpenId = "ca-app-pub-3940256099942544/5662855259";
    private const string TestMrecBannerId = "";
    private const string TestNativeBannerId = "ca-app-pub-3940256099942544/2934735716";
#endif

    public string sdkKey = "e6udsink9DPhP8w-pg8EQUTuAQodpcy05ORhzPm7fMh3W-8q_Pia-zcvtxw6dylzw9ISl6oqid8CQsgzi7QrHg";
    public string bannerId;
    public string bannerId_ios;
    public string interstitialId;
    public string interstitialId_ios;
    public string videoId;
    public string videoId_ios;
    public string openId;
    public string openId_ios;
    public string mrecBannerId;
    public string mrecBannerId_ios;
    public string nativeBannerId;
    public string nativeBannerId_ios;
    public bool setBannerBgr;


    // ca-app-pub-3940256099942544~3347511713
    //    private Maxlo _rewardedAd;
    //    private InterstitialAd _interstitial;
    //    private BannerView _bannerView;
    private bool _initialized;

    private string bannerAdapter;
    private string mrecAdapter;
    private string interstitialAdapter;
    private string rewardedAdapter;
    private string openAppAdapter;
    private string nativeAdapter;
    public bool waitMaxPolicy;

    [SerializeField] private bool showDebugger;

    public override bool BannerIsShowing()
    {
        Debug.Log(_bannerState);
        return _bannerState == AdsState.Loaded;
    }

    public override bool IntersIsShowing()
    {
        Debug.Log(_bannerState);
        return _intersState == AdsState.Showing;
    }

    public override bool VideoIsShowing()
    {
        Debug.Log(_bannerState);
        return _videoState == AdsState.Showing;
    }

    public override void SetupAction(int index, IntAction onInitialized, BooleanAction onVideoAdsLoaded,
        BaseAction onVideoAdsRewarded, BooleanAction bannerShowed, bool bannerShow, BaseAction onPaidAd = null)
    {
        base.SetupAction(index, onInitialized, onVideoAdsLoaded, onVideoAdsRewarded, bannerShowed, bannerShow, onPaidAd);
        if (!waitMaxPolicy || PlayerPrefs.HasKey("TRACKING_GDPR_FIREBASE_MAX"))
        {
            ConsentReady = true;
        }
        SetupMAXMediation();
    }

    public bool test;

    public override bool IsRemoteActive()
    {
        return RemoteConfigKey.mediation_platform.GetValueString() == "max";
    }

    void SetupMAXMediation()
    {

#if use_aps && !UNITY_EDITOR
        Amazon.Initialize(amazonApiKey);
        Amazon.SetAdNetworkInfo(new AdNetworkInfo(DTBAdNetwork.MAX));
#endif

        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            // AppLovin SDK is initialized, start loading ads
            HandleInitCompleteAction();

            if (showDebugger)
                MaxSdk.ShowMediationDebugger();
        };

        MaxSdk.SetSdkKey(sdkKey);
        //MaxSdk.SetUserId("USER_ID");
        if (test)
        {
#if use_aps && !UNITY_EDITOR
            Amazon.EnableLogging(true);
            Amazon.EnableTesting(true);
#endif
            Application.RequestAdvertisingIdentifierAsync(
                (advertisingId, trackingEnabled, error) =>
                {
                    Debug.Log("duong advertisingId " + advertisingId + " " + trackingEnabled + " " + error);
                    MaxSdk.SetTestDeviceAdvertisingIdentifiers(new[]
                        {advertisingId, "b6a0be23-81f1-416c-92af-a784be283317"});
                    MaxSdk.InitializeSdk();
                }
            );
        }
        else
        {
            Debug.Log("   MaxSdk.InitializeSdk();");
            MaxSdk.InitializeSdk();
        }

    }

    public override void OnTrackingYes(bool value)
    {
        base.OnTrackingYes(value);
        MaxSdk.SetHasUserConsent(value);
    }

    public override void ShowDebugger()
    {
    }

    void SetupAdmob(string testDevices)
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            // AppLovin SDK is initialized, start loading ads
        };
        MaxSdk.SetSdkKey(sdkKey);
        MaxSdk.SetUserId(testDevices);
        MaxSdk.InitializeSdk();
    }


    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        new SonatLogAdsState("banner", "clicked").Post();
    }


    private AdsState _bannerState;
    private AdsState _mrecBannerState;
    private AdsState _intersState;
    private AdsState _videoState;
    private AdsState _appOpenAdsState;
    private AdsState _nativeBannerState;

    public override void CheckShowBanner()
    {
        UIDebugLog.Log(GetType() + ".CheckShowBanner()" + _bannerState);
        switch (_bannerState)
        {
            case AdsState.Requesting:
                break;
            case AdsState.Failed:
                RequestBanner();
                break;
            case AdsState.Loaded:
                if (!AdsManager.NotShowBanner)
                {
                    ShowBanner();
                    OnBannerShow.Action?.Invoke(true);
                }

                break;
            case AdsState.Showing:
                break;
            case AdsState.Closed:
                break;
            case AdsState.Hidden:
                break;
            case AdsState.NotStart:
                RequestBanner();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        switch (_mrecBannerState)
        {
            case AdsState.Requesting:
                break;
            case AdsState.Failed:
                RequestMrecBanner();
                break;
            case AdsState.Loaded:
                break;
            case AdsState.Showing:
                break;
            case AdsState.Closed:
                RequestMrecBanner();
                break;
            case AdsState.NotStart:
                RequestMrecBanner();
                break;
            case AdsState.Hidden:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandleInitCompleteAction()
    {
        _initialized = true;
        ConsentReady = true;
        InitializedEvent.Action?.Invoke(_index);
        Debug.Log("sonat max : _initialized");


        //#if !UNITY_EDITOR
        RequestBanner(true);
        //#endif

        _requestInterstitial = StartCoroutine(IeRequestInters(1, true));
        _requestReward = StartCoroutine(IeRequestReward(1, true));

        if (!string.IsNullOrEmpty(OpenId) && RemoteConfigKey.show_app_open_ads.GetValueBoolean() && Kernel.appOpenCount.Value > RemoteConfigHelper.GetValueInt("min_to_show_app_open_ads", 0))
            _requestOpenAppAds = StartCoroutine(IeRequestOpenAds(0.2f, true));
        if (!string.IsNullOrEmpty(MRECBannerId))
            _requestMrec = StartCoroutine(IeRequestMrecBanner(1, true));

        if (!PlayerPrefs.HasKey("TRACKING_GDPR_FIREBASE_MAX"))
		{
            PlayerPrefs.SetInt("TRACKING_GDPR_FIREBASE_MAX", 1);
            StartCoroutine(WaitTrackingGDPR());
		}
    }

    

    //    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    //    {
    //        // Rewarded ad is hidden. Pre-load the next ad
    //        RequestRewardedAd();
    //    }
    //    _rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
    //    _rewardedAd.OnAdClosed += HandleRewardedAdClosed;
    //    _rewardedAd.OnAdFailedToLoad += HandleOnRewardAdFailedToLoad;
    //    _rewardedAd.OnPaidEvent += HandleVideoAdPaidEvent;
    //    _rewardedAd.OnAdOpening += HandleVideoAdOpeningEvent;
    IEnumerator WaitTrackingGDPR()
	{
        yield return new WaitForSeconds(3f);
        new SonatLogGDPR()
        {
            status = MaxSdk.HasUserConsent() ? "true" : "false"
        }.Post();
	}

    private void RequestBanner(bool register = false)
    {
        if (AdsManager.NotShowBanner)
        {
            UIDebugLog.Log("AdsManager.NotShowBanner " + AdsManager.NotShowBanner);
            //     return;
        }

        if (string.IsNullOrEmpty(BannerId)) return;

        //            MaxSdkCallbacks.Banner.OnAdExpandedEvent    += OnBannerAdExpandedEvent;
        //            MaxSdkCallbacks.Banner.OnAdCollapsedEvent   += OnBannerAdCollapsedEvent;
        //        if(_requestBanner != null)
        //            StopCoroutine(_requestBanner);

        if (_initialized && !AdsManager.NoAds)
        {
            // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            lastBannerId = BannerId;
            new SonatLogAdsState("banner", "request").Post();
#if !use_aps || UNITY_EDITOR
            MaxSdk.CreateBanner(BannerId, MaxSdkBase.BannerPosition.BottomCenter);
#else
            LoadBannerWithAPS();
#endif
            // Set background or background color for banners to be fully functional
            if (setBannerBgr)
                MaxSdk.SetBannerBackgroundColor(BannerId, new Color(0f, 0f, 0f));
            //            MaxSdkCallbacks.Banner.OnAdLoadedEvent      += OnBannerAdLoadedEvent;
            //            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent  += OnBannerAdLoadFailedEvent;
            //            MaxSdkCallbacks.Banner.OnAdClickedEvent     += OnBannerAdClickedEvent;
            //            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
            //            MaxSdkCallbacks.Banner.OnAdExpandedEvent    += OnBannerAdExpandedEvent;
            //            MaxSdkCallbacks.Banner.OnAdCollapsedEvent   += OnBannerAdCollapsedEvent;
            if (register)
            {
                MaxSdkCallbacks.Banner.OnAdLoadedEvent += HandleBannerAdLoaded;
                MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += HandleBannerAdFailedToLoad;
                MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
                MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += HandleBannerPaidEvent;
            }


            _bannerState = AdsState.Requesting;

#if UNITY_EDITOR

            _bannerState = AdsState.Loaded;
            if (!AdsManager.NotShowBanner && AdsManager.showBannerOnLoad)
            {
                ShowBanner();
            }
#endif
        }
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log(nameof(OnBannerAdLoadedEvent));
    }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.Log(nameof(OnBannerAdLoadFailedEvent));
    }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log(nameof(OnBannerAdRevenuePaidEvent));
    }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log(nameof(OnBannerAdExpandedEvent));
    }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log(nameof(OnBannerAdCollapsedEvent));
    }

    private void HandleBannerPaidEvent(string id, MaxSdkBase.AdInfo adInfo)
    {
        new SonatLogAdsState("banner", "paid").Post();
        SonatAnalyticTracker.LogRevenue(AdsPlatform.applovinmax, bannerAdapter, adInfo.Revenue, adInfo.RevenuePrecision,
            AdTypeLog.banner, Kernel.Resolve<FireBaseController>().FirebaseInstanceId, "banner");
    }

 

    //    private void OnApplicationPause(bool pause)
    //    {
    //        Debug.Log("duong "+nameof(OnApplicationPause) + " !pause"+!pause+" "+_earned);
    //        if (_earned && !pause)
    //        {
    //            Debug.Log("duong "+nameof(OnApplicationPause) + " "+(OnVideoAdsFinishedEvent != null));
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
                _requestMrec = StartCoroutine(IeRequestMrecBanner(2));
                _requestInterstitial = StartCoroutine(IeRequestInters(2));
                _requestReward = StartCoroutine(IeRequestReward(3));
            }

            _lastTimeFocus = Time.realtimeSinceStartup;
        }
    }

#region Banner callback handlers

    public void HandleBannerAdLoaded(string sender, MaxSdkBase.AdInfo info)
    {
        UIDebugLog.Log("duong Banner loaded" + info.AdUnitIdentifier);
        bannerAdapter = info.NetworkName; // _bannerView.GetResponseInfo().GetMediationAdapterClassName();

            _bannerState = AdsState.Loaded;
        if (!AdsManager.NotShowBanner && !AdsManager.NoAds)
        {
            new SonatLogAdsState("banner", "loaded").Post();
            AdsManager.BannerHeight = MaxSdk.GetBannerLayout(info.AdUnitIdentifier).height;
            Debug.Log($"tho: Ads banner heigh {AdsManager.BannerHeight}");
			//ShowBanner();
			//OnBannerShow.Action.Invoke(true);
			if (AdsManager.showBannerOnLoad)
			{
                ShowBanner();
			}
			else
			{
                HideBanner();
			}
        }
        



        //AdsManager.BannerHeight = 110;

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

    public void HandleBannerAdFailedToLoad(string id, MaxSdkBase.ErrorInfo args)
    {
        _bannerState = AdsState.Failed;
        OnBannerShow.Action?.Invoke(false);

        _retryBannerAttempt++;
        double retryDelay = Math.Pow(2, Mathf.Clamp(_retryBannerAttempt, 3, 7));
        //        _bannerView?.Destroy();
        //        _bannerView = null;
        _requestBanner = StartCoroutine(IeRequestBanner((float)retryDelay));
        UIDebugLog.Log(nameof(HandleBannerAdFailedToLoad) + " retryDelay-" + retryDelay + " :" + args.Message);
    }

    public void HandleBannerAdOpened(string id, MaxSdkBase.AdInfo adInfo)
    {
        AdsManager.BannerOpened = true;
        OnBannerShow.Action?.Invoke(false);
        UIDebugLog.Log(nameof(HandleBannerAdOpened) + " event received");
    }

    public void HandleBannerAdClosed(string id, MaxSdkBase.AdInfo adInfo)
    {
        DestroyBanner();
        RequestBanner();
        OnBannerShow.Action?.Invoke(false);
        UIDebugLog.Log(nameof(HandleBannerAdClosed) + " event received", false, LogType.Ads);
        StartCoroutine(waitToBannerFalse());
    }

    IEnumerator waitToBannerFalse()
    {
        yield return new WaitForSeconds(5);
        AdsManager.BannerOpened = false;
    }

    public void HandleAdLeftApplication(string id, MaxSdkBase.AdInfo adInfo)
    {
        UIDebugLog.Log(nameof(HandleBannerAdClosed) + " event received");
    }

#endregion

#region Rewarded

    private bool _earned;

    int _retryRewardAttempt;
    private Coroutine _requestReward;

    public void RequestRewardedAd(bool register = false)
    {
        if (string.IsNullOrEmpty(VideoId)) return;
        _videoState = AdsState.Requesting;
        UIDebugLog.Log("RequestRewardedAd Rewarded Ads Id :" + VideoId);
        if (register)
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += HandleRewardedAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += HandleOnRewardAdFailedToLoad;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += HandleVideoAdOpeningEvent;
            //        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += Handler;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += HandleRewardedAdPaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += HandleRewardedAdClosed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnAdDisplayFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += HandleVideoAdClickedEvent;
        }
#if !use_aps || UNITY_EDITOR
        MaxSdk.LoadRewardedAd(VideoId);
#else
        LoadRewardVideoWithAPS();
#endif
    }

    private void HandleVideoAdOpeningEvent(string id, MaxSdkBase.AdInfo adInfo)
    {
        _videoState = AdsState.Showing;
        //        if (RxKernel.Instance != null && !GameRoot.Instance.Pause.Value)
        //            Time.timeScale = 0f;
    }

    private void HandleVideoAdClickedEvent(string id, MaxSdkBase.AdInfo adInfo)
    {
        adDurationTracker.OnUserClickedAd();
    }

    public void HandleRewardedAdClosed(string id, MaxSdkBase.AdInfo adInfo)
    {
        new SonatLogLastScreenView().Post();
        UIDebugLog.Log(nameof(HandleRewardedAdClosed) + " _earned " + _earned);
        _videoState = AdsState.Closed;
        if (_earned)
        {
#if UNITY_IOS
            VideoRewarded.Action?.Invoke();
            _earned = false;
#endif
        }


        _requestReward = StartCoroutine(IeRequestReward(1));
        //        if (!Kernel.Pause)
        //            Time.timeScale = 1f;
        UIDebugLog.Log(nameof(HandleRewardedAdClosed) + " _earned " + _earned, false, LogType.Ads);
        StartCoroutine(waitToRewardFalse());

        AdDurationTracker.AdMetrics adMetrics = adDurationTracker.OnAdDismissedFullScreenContent();
        new SonatLogAdDuration(AdsPlatform.applovinmax.ToString(), adInfo.AdFormat, adInfo.NetworkName, adInfo.Revenue.ToString(), adMetrics).Post();


        //_requestReward = Observable.Timer(TimeSpan.FromSeconds(.5f)).Subscribe(_ => RequestRewardedAd());
        //if (RxKernel.Instance != null && !GameRoot.Instance.Pause.Value)
        //    Time.timeScale = 1f;
    }

    IEnumerator waitToRewardFalse()
    {
        yield return new WaitForSeconds(3);
        AdsManager.VideoOpened = false;
    }


    private void HandleRewardedAdLoaded(string id, MaxSdkBase.AdInfo adInfo)
    {
        _retryRewardAttempt = 1;
        rewardedAdapter = adInfo.NetworkName;
        UIDebugLog.Log(nameof(HandleRewardedAdLoaded) + " - Rewarded video loaded:" + IsVideoAdsReady());
        VideoLoaded.Action?.Invoke(true);

        //Debug.Log()

        //BoosterController.instance.SetIconAds();
    }


    private void OnAdReceivedRewardEvent(string id, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        UIDebugLog.Log(nameof(OnAdReceivedRewardEvent));
#if UNITY_ANDROID
        //    _earned = true;
        VideoRewarded.Action?.Invoke();
#else
      VideoRewarded.Action?.Invoke();
#endif
    }

    private void HandleRewardedAdPaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        SonatAnalyticTracker.LogRevenue(AdsPlatform.applovinmax, rewardedAdapter, adInfo.Revenue,
            adInfo.RevenuePrecision, AdTypeLog.rewarded, Kernel.Resolve<FireBaseController>().FirebaseInstanceId,
            SonatAnalyticTracker.RewardedLogPlacement);
    }

    private void HandleOnRewardAdFailedToLoad(string id, MaxSdkBase.ErrorInfo errorInfo)
    {
        _retryRewardAttempt++;
        double retryDelay = Math.Pow(2, Mathf.Clamp(_retryRewardAttempt, 3, 7));
        UIDebugLog.Log(nameof(HandleOnRewardAdFailedToLoad) + " retryDelay-" + retryDelay + " :" + errorInfo.Message);
        _requestReward = StartCoroutine(IeRequestReward((float)retryDelay));
    }


    private void OnAdDisplayFailedEvent(string sender, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        new SonatLogLastScreenView().Post();
        _retryRewardAttempt++;
        double retryDelay = Math.Pow(2, Mathf.Clamp(_retryRewardAttempt, 3, 7));
        UIDebugLog.Log(nameof(OnAdDisplayFailedEvent) + " retryDelay-" + retryDelay + " :" + errorInfo.Message);

        _requestReward = StartCoroutine(IeRequestReward((float)retryDelay));
    }

    

    private void HandleInterstitialAdPaidEvent(string id, MaxSdkBase.AdInfo adInfo)
    {
        new SonatLogAdsState("inter", "paid").Post();
        SonatAnalyticTracker.LogRevenue(AdsPlatform.applovinmax, interstitialAdapter, adInfo.Revenue,
            adInfo.RevenuePrecision, AdTypeLog.interstitial, Kernel.Resolve<FireBaseController>().FirebaseInstanceId,
            SonatAnalyticTracker.InterstitialLogName);
    }

    public override void ShowVideoAds()
    {
        if (IsVideoAdsReady())
        {
            adDurationTracker.OnAdShowedFullScreenContent();
            MaxSdk.ShowRewardedAd(VideoId);
            _earned = false;
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
        return MaxSdk.IsRewardedAdReady(VideoId);
    }

#endregion

#region interstitial

    public override bool IsInterstitialReady()
    {
        return MaxSdk.IsInterstitialReady(InterstitialId);
    }

    public override void ShowInterstitial(bool isOnFocus)
    {
        if (!(isOnFocus && RemoteConfigKey.use_open_ads_as_interstitial.GetValueBoolean() && ShowAppOpenAds()))
            if (MaxSdk.IsInterstitialReady(InterstitialId))
            {
                new SonatLogAdsState("inter", "show").Post();
                adDurationTracker.OnAdShowedFullScreenContent();
                MaxSdk.ShowInterstitial(InterstitialId);
            }

    }

    [ContextMenu("Show id")]
    public override void ShowId()
    {
        UIDebugLog.Log0("MaxId :" + nameof(InterstitialId) + ":" + GetLast(InterstitialId), false, LogType.Ads);
        UIDebugLog.Log0("MaxId :" + nameof(VideoId) + ":" + GetLast(VideoId), false, LogType.Ads);
        UIDebugLog.Log0("MaxId :" + nameof(OpenId) + ":" + GetLast(OpenId), false, LogType.Ads);
        UIDebugLog.Log0("MaxId :" + nameof(MRECBannerId) + ":" + GetLast(MRECBannerId), false, LogType.Ads);
        UIDebugLog.Log0("MaxId :" + nameof(NativeBannerId) + ":" + GetLast(NativeBannerId), false, LogType.Ads);
    }

    int retryIntersAttempt;

    private Coroutine _requestInterstitial;
    private Coroutine _requestMrec;

    private void RequestInterstitial(bool register = false)
    {
        if (string.IsNullOrEmpty(InterstitialId)) return;
        UIDebugLog.Log("duong : RequestInterstitial:" + InterstitialId);
        _intersState = AdsState.Requesting;
        //        if(_requestInterstitial != null)
        //            StopCoroutine(_requestInterstitial);

        if (register)
        {
            UIDebugLog.Log("duong : Register Max Callback");

            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += HandleOnInterstitialAdLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += HandleOnInterstitialAdFailedToLoad;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += HandleInterOpeningEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += HandleInterClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += HandleOnInterstitialAdClosed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += HandleOnInterstitialAdFailedToShow;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += HandleInterstitialAdPaidEvent;
        }
        new SonatLogAdsState("inter", "load").Post();
#if !use_aps || UNITY_EDITOR
        MaxSdk.LoadInterstitial(InterstitialId);
#else
        LoadInterWithAPS();
#endif
    }

    private void HandleInterOpeningEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        _intersState = AdsState.Showing;
        //        if (RxKernel.Instance != null && !GameRoot.Instance.Pause.Value)
        //            Time.timeScale = 0f;
    }

    private void HandleInterClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        _intersState = AdsState.Showing;
        adDurationTracker.OnUserClickedAd();
        //        if (RxKernel.Instance != null && !GameRoot.Instance.Pause.Value)
        //            Time.timeScale = 0f;
    }

    private void HandleOnInterstitialAdLoaded(string id, MaxSdkBase.AdInfo adInfo)
    {
        new SonatLogAdsState("inter", "loaded").Post();
        retryIntersAttempt = 1;
        interstitialAdapter = adInfo.NetworkName;
        UIDebugLog.Log(nameof(HandleOnInterstitialAdLoaded) + " event received" + IsInterstitialReady());
    }

    private void HandleOnInterstitialAdFailedToLoad(string id, MaxSdkBase.ErrorInfo errorInfo)
    {
        new SonatLogAdsState("inter", "load_failed").Post();
        retryIntersAttempt++;
        double retryDelay = Math.Pow(2, Mathf.Clamp(retryIntersAttempt, 3, 8));
        UIDebugLog.Log(nameof(HandleOnInterstitialAdFailedToLoad) + " event received with message: (" +
                       (float)retryDelay +
                       ") " + errorInfo.Message);

        _requestInterstitial = StartCoroutine(IeRequestInters((float)retryDelay));
    }

    private void HandleOnInterstitialAdFailedToShow(string sender, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo)
    {
        new SonatLogAdsState("inter", "failed_to_show").Post();
        _intersState = AdsState.Failed;
        RequestInterstitial();
        UIDebugLog.Log(nameof(HandleOnInterstitialAdFailedToShow) + " event received with message: " +
                       errorInfo.Message);
        OnAdsClosed();
    }

    public void HandleOnInterstitialAdOpened(string id, MaxSdkBase.AdInfo adInfo)
    {
        new SonatLogAdsState("inter", "opened").Post();
        AdsManager.OpenAdsOrInterstitialOpened = true;
        //        FireBaseController.LogEvent("interstitial", new[]
        //        {
        //            new LogParameter("event", "opened")
        //        });
        UIDebugLog.Log(nameof(HandleOnInterstitialAdOpened) + " event received");
        OnAdsShowed();
    }

    public void HandleOnInterstitialAdClosed(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        new SonatLogAdsState("inter", "closed").Post();
        OnAdsClosed();
        _intersState = AdsState.Closed;
        new SonatLogLastScreenView().Post();


        AdsManager.OpenAdsOrInterstitialOpened = false;
        //        FireBaseController.LogEvent("interstitial", new[]
        //        {
        //            new LogParameter("event", "closed")
        //        });
        UIDebugLog.Log(nameof(HandleOnInterstitialAdClosed) + " event received");

        AdDurationTracker.AdMetrics adMetrics = adDurationTracker.OnAdDismissedFullScreenContent();
        new SonatLogAdDuration(AdsPlatform.applovinmax.ToString(), adInfo.AdFormat, adInfo.NetworkName, adInfo.Revenue.ToString(), adMetrics).Post();

        RequestInterstitial();
    }

    public void HandleOnInterstitialAdLeavingApplication(string id, MaxSdkBase.AdInfo adInfo)
    {
        new SonatLogAdsState("inter", "leaving_app").Post();
        AppsFlyer.sendEvent("af_ad_click", new Dictionary<string, string>()
        {
            {"af_adrev_ad_type", ""}
        });
        UIDebugLog.Log(nameof(HandleOnInterstitialAdLeavingApplication) + " event received");
    }

    public override void DestroyBanner()
    {
        if (!string.IsNullOrEmpty(lastBannerId))
            MaxSdk.DestroyBanner(lastBannerId);
        _bannerState = AdsState.Failed;
        OnBannerShow.Action?.Invoke(false);
        DestroyMrecBanner();
    }

    public override bool IsInitialized()
    {
        return _initialized;
    }

    public override void HideBanner()
    {
        //if (_bannerState == AdsState.Hidden) return;
        AdsManager.showBannerOnLoad = false;

        if (!string.IsNullOrEmpty(lastBannerId))
        {
            new SonatLogAdsState("banner", "hide").Post();
            MaxSdk.HideBanner(lastBannerId);
        }
        if (_bannerState != AdsState.Hidden)
            OnBannerShow.Action?.Invoke(false);
        _bannerState = AdsState.Hidden;
    }

    public override void ShowBanner()
    {
        //if (_bannerState == AdsState.Showing || _bannerState == AdsState.Failed) return;
        if (!string.IsNullOrEmpty(lastBannerId))
        {
            AdsManager.showBannerOnLoad = true;
            new SonatLogAdsState("banner", "show").Post();
            MaxSdk.ShowBanner(lastBannerId);
            if (_bannerState != AdsState.Showing)
                OnBannerShow.Action?.Invoke(true);
            _bannerState = AdsState.Showing;
        }
    }

    public override void RequestNewAds()
    {
        showBanner = false;
        _bannerState = AdsState.Failed;
        _mrecBannerState = AdsState.Failed;
        _intersState = AdsState.Failed;
        _videoState = AdsState.Failed;
        VideoLoaded.Action?.Invoke(false);


        _requestMrec = StartCoroutine(IeRequestMrecBanner(1));
        _requestBanner = StartCoroutine(IeRequestBanner(1));
        _requestInterstitial = StartCoroutine(IeRequestInters(2));
        _requestReward = StartCoroutine(IeRequestReward(3));

        Kernel.Resolve<FireBaseController>().LogEvent(nameof(RequestNewAds));
    }

    private IEnumerator IeRequestBanner(float time)
    {
        UIDebugLog.Log("IeRequestBanner", false, LogType.Ads);
        yield return new WaitForSeconds(time);
        RequestBanner();
    }

    private IEnumerator IeRequestInters(float time, bool init = false)
    {
        yield return new WaitForSeconds(time);
        RequestInterstitial(init);
    }

    private IEnumerator IeRequestReward(float time, bool init = false)
    {
        yield return new WaitForSeconds(time);
        RequestRewardedAd(init);
    }

#endregion

    private IEnumerator IeRequestOpenAds(float time, bool init = false)
    {
        yield return new WaitForSeconds(time);
        RequestOpenAds(init);
    }

    int retryOpenAppAdsAttempt;
    private Coroutine _requestOpenAppAds;

    private void RequestOpenAds(bool register = false)
    {
        if (string.IsNullOrEmpty(OpenId))
        {
            return;
        }

        UIDebugLog.Log("duong : RequestOpenAppAds:" + OpenId);
        _appOpenAdsState = AdsState.Requesting;
        //        if(_requestInterstitial != null)
        //            StopCoroutine(_requestInterstitial);
        MaxSdk.LoadAppOpenAd(OpenId);
        if (register)
        {
            UIDebugLog.Log("duong : Register Max Callback");

            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += HandleOnAppOpenAdLoaded;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += HandleOnAppOpenAdFailedToLoad;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += HandleOnAppOpenAdOpened;
            //   MaxSdkCallbacks.Interstitial.OnAdClickedEvent += HandleOnInterstitialAdClosed;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += HandleOnAppOpenAdClosed;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += HandleOnAppOpenAdFailedToShow;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += HandleOnAppOpenAdPaidEvent;
        }
    }

    public override bool IsAppOpenAdsReady()
    {
        if (string.IsNullOrEmpty(OpenId)) return false;
        return MaxSdk.IsAppOpenAdReady(OpenId);
    }

    public override bool ShowAppOpenAds()
	{
        if(IsAppOpenAdsReady() && _appOpenAdsState != AdsState.Showing && !AdsManager.NoAds)
		{
            MaxSdk.ShowAppOpenAd(OpenId);
            return true;
		}

        return false;
	}

    private void HandleOnAppOpenAdLoaded(string id, MaxSdkBase.AdInfo adInfo)
    {
        retryOpenAppAdsAttempt = 1;
        openAppAdapter = adInfo.NetworkName;
        _appOpenAdsState = AdsState.Loaded;
        UIDebugLog.Log(nameof(HandleOnAppOpenAdLoaded) + " event received" + IsAppOpenAdsReady());
        //ShowOpenAppAds();
    }

    private void HandleOnAppOpenAdFailedToLoad(string id, MaxSdkBase.ErrorInfo errorInfo)
    {
        retryOpenAppAdsAttempt++;
        double retryDelay = Math.Pow(2, Mathf.Clamp(retryOpenAppAdsAttempt, 3, 8));
        UIDebugLog.Log(nameof(HandleOnInterstitialAdFailedToLoad) + " event received with message: (" +
                       (float)retryDelay +
                       ") " + errorInfo.Message);
        _appOpenAdsState = AdsState.Failed;
        _requestOpenAppAds = StartCoroutine(IeRequestOpenAds((float)retryDelay));
    }

    private void HandleOnAppOpenAdFailedToShow(string sender, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo)
    {
        _appOpenAdsState = AdsState.Failed;
        RequestOpenAds();
        UIDebugLog.Log(nameof(HandleOnAppOpenAdFailedToShow) + " event received with message: " +
                       errorInfo.Message);
    }

    public void HandleOnAppOpenAdOpened(string id, MaxSdkBase.AdInfo adInfo)
    {
        //AdsManager.OpenAdsOrInterstitialOpened = true;
        //        FireBaseController.LogEvent("interstitial", new[]
        //        {
        //            new LogParameter("event", "opened")
        //        });
        _appOpenAdsState = AdsState.Showing;
        UIDebugLog.Log(nameof(HandleOnAppOpenAdOpened) + " event received");
        OnAdsShowed();
    }

    public void HandleOnAppOpenAdClosed(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        OnAdsClosed();
        _appOpenAdsState = AdsState.Closed;
        new SonatLogLastScreenView().Post();
        StartCoroutine(IeRequestOpenAds(.5f));
        ResetTimeShowOnFocusInter();

        //AdsManager.OpenAdsOrInterstitialOpened = false;
        //        FireBaseController.LogEvent("interstitial", new[]
        //        {
        //            new LogParameter("event", "closed")
        //        });
        UIDebugLog.Log(nameof(HandleOnAppOpenAdClosed) + " event received");
        //RequestInterstitial();
    }

    private void HandleOnAppOpenAdPaidEvent(string id, MaxSdkBase.AdInfo adInfo)
    {
        SonatAnalyticTracker.LogRevenue(AdsPlatform.applovinmax, openAppAdapter, adInfo.Revenue,
            adInfo.RevenuePrecision, AdTypeLog.app_open, Kernel.Resolve<FireBaseController>().FirebaseInstanceId,
            SonatAnalyticTracker.InterstitialLogName);
    }

    public void HandleOnAppOpenAdLeavingApplication(string id, MaxSdkBase.AdInfo adInfo)
    {
        AppsFlyer.sendEvent("af_ad_click", new Dictionary<string, string>()
        {
            {"af_adrev_ad_type", ""}
        });
        UIDebugLog.Log(nameof(HandleOnAppOpenAdLeavingApplication) + " event received");
    }

#region Collapsible Banner
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
#endregion

#region Native Ads
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

    private void LoadBannerWithAPS()
    {
        int width;
        int height;
        string slotId;
        if (MaxSdkUtils.IsTablet())
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

        var apsBanner = new APSBannerAdRequest(width, height, slotId);
        apsBanner.onSuccess += (adResponse) =>
        {
            MaxSdk.SetBannerLocalExtraParameter(BannerId, "amazon_ad_response", adResponse.GetResponse());
            CreateMaxBannerAd();
        };
        apsBanner.onFailedWithError += (adError) =>
        {
            MaxSdk.SetBannerLocalExtraParameter(BannerId, "amazon_ad_error", adError.GetAdError());
            CreateMaxBannerAd();
        };

        apsBanner.LoadAd();
    }

    private void CreateMaxBannerAd()
    {
        MaxSdk.CreateBanner(BannerId, MaxSdkBase.BannerPosition.BottomCenter);
        //MaxSdk.SetBannerPlacement(«ad - unit - ID», "«placement»");
    }

#region inter
    private bool IsFirstLoad = true;
    private bool staticInterLoaded = false;
    private bool videoInterLoaded = false;

    private void LoadInterWithAPS()
    {
        if (IsFirstLoad)
        {
            IsFirstLoad = false;

            var interstitialAd = new APSInterstitialAdRequest(amazonStaticInterId);
            var interstitialVideoAd = new APSVideoAdRequest(320, 480, amazonVideoInterId);
            interstitialAd.onSuccess += (adResponse) =>
            {
                MaxSdk.SetInterstitialLocalExtraParameter(InterstitialId, "amazon_ad1_response", adResponse.GetResponse());
                staticInterLoaded = true;
                if (videoInterLoaded && staticInterLoaded)
                    MaxSdk.LoadInterstitial(InterstitialId);
            };
            interstitialAd.onFailedWithError += (adError) =>
            {
                MaxSdk.SetInterstitialLocalExtraParameter(InterstitialId, "amazon_ad1_error", adError.GetAdError());
                staticInterLoaded = true;
                if (videoInterLoaded && staticInterLoaded)
                    MaxSdk.LoadInterstitial(InterstitialId);
            };

            interstitialVideoAd.onSuccess += (adResponse) =>
            {
                MaxSdk.SetInterstitialLocalExtraParameter(InterstitialId, "amazon_ad_response", adResponse.GetResponse());
                videoInterLoaded = true;
                if (videoInterLoaded && staticInterLoaded)
                    MaxSdk.LoadInterstitial(InterstitialId);
            };
            interstitialVideoAd.onFailedWithError += (adError) =>
            {
                MaxSdk.SetInterstitialLocalExtraParameter(InterstitialId, "amazon_ad_error", adError.GetAdError());
                videoInterLoaded = true;
                if (videoInterLoaded && staticInterLoaded)
                    MaxSdk.LoadInterstitial(InterstitialId);
            };
            interstitialAd.LoadAd();
            interstitialVideoAd.LoadAd();
        }
        else
        {
            MaxSdk.LoadInterstitial(InterstitialId);
        }
    }

#endregion
#region reward
    private bool IsFirstLoadReward = true;

    private void LoadRewardVideoWithAPS()
    {
        if (IsFirstLoadReward)
        {
            IsFirstLoadReward = false;

            var rewardedVideoAd = new APSVideoAdRequest(320, 480, amazonRewardId);
            rewardedVideoAd.onSuccess += (adResponse) =>
            {
                MaxSdk.SetRewardedAdLocalExtraParameter(VideoId, "amazon_ad_response", adResponse.GetResponse());
                MaxSdk.LoadRewardedAd(VideoId);
            };
            rewardedVideoAd.onFailedWithError += (adError) =>
            {
                MaxSdk.SetRewardedAdLocalExtraParameter(VideoId, "amazon_ad_error", adError.GetAdError());
                MaxSdk.LoadRewardedAd(VideoId);
            };

            rewardedVideoAd.LoadAd();
        }
        else
        {
            MaxSdk.LoadRewardedAd(VideoId);
        }
    }
#endregion


#endif
}
#endif