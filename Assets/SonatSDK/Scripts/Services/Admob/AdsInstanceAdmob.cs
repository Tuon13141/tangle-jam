#define dummy
//#define use_admob
using Sonat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using sonat_sdk.Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using LogType = sonat_sdk.Scripts.LogType;
using Object = System.Object;
#if !((dummy || global_dummy) && !use_admob)
using GoogleMobileAds.Common;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using Sonat;

#endif


#if (dummy || global_dummy) && !use_admob
public partial class AdsInstanceAdmob : AdsInstance
{
    [SerializeField] private bool loadGDPR;
    [SerializeField] private string androidAppId;
    [SerializeField] private string iosAppId;
    public string AndroidAppId => androidAppId;
    public string IosAppId => iosAppId;

    public string[] testDeviceIds = new[] {"B15BB24370A69E2E02CE1EA2F2E5BD10"}; // redmi 5a


    [Header("Android")] public string bannerId;
    public string interstitialId;
    public string videoId;
    public string openId;

    [Header("IOS")] public string bannerId_ios;
    public string interstitialId_ios;
    public string videoId_ios;
    public string openId_ios;
    
    
    public override void ShowVideoAds()
    {
        VideoRewarded.Action.Invoke();
        Debug.Log("Dummy : ShowVideoAds");
    }
    
    public override void SetupAction(int index,IntAction onInitialized, BooleanAction onVideoAdsLoaded,
        BaseAction onVideoAdsRewarded, BooleanAction bannerShowed, bool bannerShow, BaseAction onPaidAd = null)
    {
        base.SetupAction(index, onInitialized, onVideoAdsLoaded, onVideoAdsRewarded, bannerShowed, bannerShow);
        VideoLoaded.Action.Invoke(true);
        
        ConsentReady = true;
        Initialized = true;
        CheckInitialized();
    }

    public override void LoadVideoAds()
    {
        VideoLoaded.Action.Invoke(true);
       ////Debug.Log("Dummy : LoadVideoAds");
    }


    public override void RequestNewAds()
    {
        VideoLoaded.Action.Invoke(true);
       ////Debug.Log("Dummy : LoadVideoAds");
    }

    public override bool IsVideoAdsReady()
    {
       ////Debug.Log("Dummy : IsVideoAdsReady");
        return true;
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

    public override bool IsRemoteActive()
    {
        return false;
    }

    public override bool IsNativeReady()
    {
        return false;
    }
    
    public override void ShowDebugger()
    {

    }

    public void ResetGDPR()
    {
     //   ConsentInformation.Reset();
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


// ReSharper disable InconsistentNaming

public partial class AdsInstanceAdmob : AdsInstance
{
    [SerializeField] private string androidAppId;
    [SerializeField] private string iosAppId;
    public string AndroidAppId => androidAppId;
    public string IosAppId => iosAppId;

    [SerializeField] private bool logFail = true;

    [SerializeField] private PlayerPrefRemoteBool forceDisableTestAds =
        new PlayerPrefRemoteBool(RemoteConfigKey.force_disable_test_ads, false);

    [ContextMenu("Show id")]
    public override void ShowId()
    {
        UIDebugLog.Log0("AdmobId :" + nameof(BannerId) + ":" + GetLast(BannerId), false,LogType.Ads);
        UIDebugLog.Log0("AdmobId :" + nameof(InterstitialId) + ":" + GetLast(InterstitialId), false,LogType.Ads);
        UIDebugLog.Log0("AdmobId :" + nameof(VideoId) + ":" + GetLast(VideoId), false,LogType.Ads);
        UIDebugLog.Log0("AdmobId :" + nameof(OpenId) + ":" + GetLast(OpenId), false,LogType.Ads);
        UIDebugLog.Log0("AdmobId :" + nameof(NativeBannerId) + ":" + GetLast(NativeBannerId), false,LogType.Ads);

        if (status != null)
            foreach (var mediation in status)
                UIDebugLog.Log0(
                    $"admob Adapter={mediation.Key} InitializationState={mediation.Value.InitializationState}",
                    false,LogType.Ads);

#if UNITY_EDITOR
        //   Debug.Log(RemoteConfigKey.admob_banner_ad_id.GetValueString(false));
#endif
    }

    public override void ShowDebugger()
    {
        MobileAds.OpenAdInspector(error =>
        {
            if (error != null)
                UIDebugLog.Log0("OpenAdInspector :" + error.GetMessage(), false,LogType.Ads);
        });
        UIDebugLog.Log0(GetType().Name + " " + nameof(ShowDebugger), false,LogType.Ads);
    }

#if UNITY_ANDROID
    private PlayerPrefRemoteString playerPrefBannerId;
    private PlayerPrefRemoteString playerPrefInterstitialId;
    private PlayerPrefRemoteString playerPrefNativeBannerId;
    private PlayerPrefRemoteString playerPrefOpenId;
    private PlayerPrefRemoteString playerPrefRewardedId;
    private PlayerPrefRemoteString playerPrefCollapsibleBannerId;
    private PlayerPrefRemoteString playerPrefNativeId;


    protected virtual string BannerId
    {
        get
        {
            if (testAds && !forceDisableTestAds.Value)
                return TestBannerId;
            if (playerPrefBannerId == null)
                playerPrefBannerId = new PlayerPrefRemoteString(RemoteConfigKey.admob_banner_ad_id, bannerId);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_admob_banner_id.GetValueString(false)))
            {
//                #if UNITY_EDITOR
//                Debug.LogError("test pam"+RemoteConfigKey.pam_admob_banner_id.GetValueString(false));
//                #endif
                return RemoteConfigKey.pam_admob_banner_id.GetValueString(false);
            }

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_admob_banner_id,
                    playerPrefBannerId.DefaultValueWhenEmpty());

            return playerPrefBannerId.DefaultValueWhenEmpty();
        }
    }

    protected virtual string InterstitialId
    {
        get
        {
            if (testAds && !forceDisableTestAds.Value)
                return TestInterstitialId;
            if (playerPrefInterstitialId == null)
                playerPrefInterstitialId =
                    new PlayerPrefRemoteString(RemoteConfigKey.admob_inter_ad_id, interstitialId);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_admob_interstitial_id.GetValueString(false)))
                return RemoteConfigKey.pam_admob_interstitial_id.GetValueString(false);

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_admob_interstitial_id,
                    playerPrefInterstitialId.DefaultValueWhenEmpty());

            return playerPrefInterstitialId.DefaultValueWhenEmpty();
        }
    }

    protected virtual string VideoId
    {
        get
        {
            if (testAds && !forceDisableTestAds.Value)
                return TestVideoId;
            if (playerPrefRewardedId == null)
                playerPrefRewardedId = new PlayerPrefRemoteString(RemoteConfigKey.admob_rewarded_ad_id, videoId);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_admob_rewarded_id.GetValueString(false)))
                return RemoteConfigKey.pam_admob_rewarded_id.GetValueString(false);

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_admob_rewarded_id,
                    playerPrefRewardedId.DefaultValueWhenEmpty());


            return playerPrefRewardedId.DefaultValueWhenEmpty();
        }
    }

    protected virtual string OpenId
    {
        get
        {
            if (testAds && !forceDisableTestAds.Value)
                return TestOpenId;
            if (playerPrefOpenId == null)
                playerPrefOpenId = new PlayerPrefRemoteString(RemoteConfigKey.admob_open_ad_id, openId);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_admob_open_id.GetValueString(false)))
                return RemoteConfigKey.pam_admob_open_id.GetValueString(false);


            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_admob_open_id,
                    playerPrefOpenId.DefaultValueWhenEmpty());

            return playerPrefOpenId.DefaultValueWhenEmpty();
        }
    }

    protected virtual string NativeBannerId
    {
        get
        {
            if (testAds && !forceDisableTestAds.Value)
                return TestNativeBannerId;
            if (playerPrefNativeBannerId == null)
                playerPrefNativeBannerId =
                    new PlayerPrefRemoteString(RemoteConfigKey.admob_native_banner_ad_id, nativeBannerId);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_admob_native_id.GetValueString(false)))
                return RemoteConfigKey.pam_admob_native_id.GetValueString(false);

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_admob_native_id,
                    playerPrefNativeBannerId.DefaultValueWhenEmpty());

            return playerPrefNativeBannerId.Value;
        }
    }

    protected virtual string CollapsibleBannerId
    {
        get
        {
            if (testAds && !forceDisableTestAds.Value)
                return TestCollapsibleBannerId;
            if (playerPrefCollapsibleBannerId == null)
                playerPrefCollapsibleBannerId =
                    new PlayerPrefRemoteString(RemoteConfigKey.admob_collapsible_ad_id, collapsibleBannerId);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_admob_collapsible_id.GetValueString(false)))
                return RemoteConfigKey.pam_admob_collapsible_id.GetValueString(false);

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_admob_collapsible_id,
                    playerPrefCollapsibleBannerId.DefaultValueWhenEmpty());

            return playerPrefCollapsibleBannerId.Value;
        }
    }

    protected virtual string NativeId
    {
        get
        {
            if (testAds && !forceDisableTestAds.Value)
                return TestNativeId;
            if (playerPrefNativeId == null)
                playerPrefNativeId =
                    new PlayerPrefRemoteString(RemoteConfigKey.admob_native_ad_id, nativeId);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_admob_nativeads_id.GetValueString(false)))
                return RemoteConfigKey.pam_admob_nativeads_id.GetValueString(false);

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_admob_nativeads_id,
                    playerPrefNativeId.DefaultValueWhenEmpty());

            return playerPrefNativeId.Value;
        }
    }

    protected const string TestBannerId = "ca-app-pub-3940256099942544/6300978111";
    protected const string TestInterstitialId = "ca-app-pub-3940256099942544/1033173712";
    protected const string TestVideoId = "ca-app-pub-3940256099942544/5224354917";
    protected const string TestOpenId = "ca-app-pub-3940256099942544/9257395921";
    protected const string TestNativeBannerId = "ca-app-pub-3940256099942544/6300978111";
    protected const string TestCollapsibleBannerId = "ca-app-pub-3940256099942544/6300978111";
    protected const string TestNativeId = "ca-app-pub-3940256099942544/2247696110";

#else
    private PlayerPrefRemoteString playerPrefBannerId;
    private PlayerPrefRemoteString playerPrefInterstitialId;
    private PlayerPrefRemoteString playerPrefNativeBannerId;
    private PlayerPrefRemoteString playerPrefOpenId;
    private PlayerPrefRemoteString playerPrefRewardedId;
    private PlayerPrefRemoteString playerPrefCollapsibleBannerId;
    private PlayerPrefRemoteString playerPrefNativeId;

//#if UNITY_IOS*
    protected virtual string BannerId
    {
        get
        {
            if(testAds && !forceDisableTestAds.Value)
                return TestBannerId;
            if (playerPrefBannerId == null)
                playerPrefBannerId = new PlayerPrefRemoteString(RemoteConfigKey.admob_banner_ad_id, bannerId_ios);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() && !string.IsNullOrEmpty(RemoteConfigKey.pam_admob_banner_id.GetValueString(false)))
               return RemoteConfigKey.pam_admob_banner_id.GetValueString(false);
            
            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_admob_banner_id,playerPrefBannerId.DefaultValueWhenEmpty());
            
            return playerPrefBannerId.DefaultValueWhenEmpty();
        }
    }

    protected virtual string InterstitialId
    {
        get
        {
            if(testAds && !forceDisableTestAds.Value)
                return TestInterstitialId;
            if (playerPrefInterstitialId == null)
                playerPrefInterstitialId =
                    new PlayerPrefRemoteString(RemoteConfigKey.admob_inter_ad_id, interstitialId_ios);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() && !string.IsNullOrEmpty(RemoteConfigKey.pam_admob_interstitial_id.GetValueString(false)))
                return RemoteConfigKey.pam_admob_interstitial_id.GetValueString(false);
            
            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_admob_interstitial_id,playerPrefInterstitialId.DefaultValueWhenEmpty());
            
            return playerPrefInterstitialId.DefaultValueWhenEmpty();
        }
    }

    
  

    protected virtual string VideoId
    {
        get
        {
            if(testAds && !forceDisableTestAds.Value)
                return TestVideoId;
            if (playerPrefRewardedId == null)
                playerPrefRewardedId =
                    new PlayerPrefRemoteString(RemoteConfigKey.admob_rewarded_ad_id, videoId_ios);

           if (RemoteConfigKey.enabled_pam.GetValueBoolean() && !string.IsNullOrEmpty(RemoteConfigKey.pam_admob_rewarded_id.GetValueString(false)))
                return RemoteConfigKey.pam_admob_rewarded_id.GetValueString(false);
            
            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_admob_rewarded_id,playerPrefRewardedId.DefaultValueWhenEmpty());
            
            return playerPrefRewardedId.DefaultValueWhenEmpty();
        }
    }

    protected virtual string OpenId
    {
        get
        {
            if(testAds && !forceDisableTestAds.Value)
                return TestOpenId;
            if (playerPrefOpenId == null)
                playerPrefOpenId = new PlayerPrefRemoteString(RemoteConfigKey.admob_open_ad_id, openId_ios);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() && !string.IsNullOrEmpty(RemoteConfigKey.pam_admob_open_id.GetValueString(false)))
                return RemoteConfigKey.pam_admob_open_id.GetValueString(false);
            
            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_admob_open_id,playerPrefOpenId.DefaultValueWhenEmpty());
            
            return playerPrefOpenId.DefaultValueWhenEmpty();
        }
    }

    protected virtual string NativeBannerId
    {
        get
        {
            if(testAds && !forceDisableTestAds.Value)
                return TestNativeBannerId;
            if (playerPrefNativeBannerId == null)
                playerPrefNativeBannerId = new PlayerPrefRemoteString(RemoteConfigKey.admob_native_banner_ad_id,
                    nativeBannerId_ios);

         if (RemoteConfigKey.enabled_pam.GetValueBoolean() && !string.IsNullOrEmpty(RemoteConfigKey.pam_admob_native_id.GetValueString(false)))
                return RemoteConfigKey.pam_admob_native_id.GetValueString(false);
            
            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_admob_native_id,playerPrefNativeBannerId.DefaultValueWhenEmpty());
            
            return playerPrefNativeBannerId.DefaultValueWhenEmpty();
        }
    }

    protected virtual string CollapsibleBannerId
    {
        get
        {
            if (testAds && !forceDisableTestAds.Value)
                return TestCollapsibleBannerId;
            if (playerPrefCollapsibleBannerId == null)
                playerPrefCollapsibleBannerId =
                    new PlayerPrefRemoteString(RemoteConfigKey.admob_collapsible_ad_id, collapsibleBannerId_ios);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_admob_collapsible_id.GetValueString(false)))
                return RemoteConfigKey.pam_admob_collapsible_id.GetValueString(false);

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_admob_collapsible_id,
                    playerPrefCollapsibleBannerId.DefaultValueWhenEmpty());

            return playerPrefCollapsibleBannerId.Value;
        }
    }

    protected virtual string NativeId
    {
        get
        {
            if (testAds && !forceDisableTestAds.Value)
                return TestNativeId;
            if (playerPrefNativeId == null)
                playerPrefNativeId =
                    new PlayerPrefRemoteString(RemoteConfigKey.admob_native_ad_id, nativeId_ios);

            if (RemoteConfigKey.enabled_pam.GetValueBoolean() &&
                !string.IsNullOrEmpty(RemoteConfigKey.pam_admob_nativeads_id.GetValueString(false)))
                return RemoteConfigKey.pam_admob_nativeads_id.GetValueString(false);

            return Kernel.Resolve<FireBaseController>().remoteConfigController
                .GetValueByLevel(RemoteConfigKey.by_level_condition_admob_nativeads_id,
                    playerPrefNativeId.DefaultValueWhenEmpty());

            return playerPrefNativeId.Value;
        }
    }

    private const string TestBannerId = "ca-app-pub-3940256099942544/2934735716";
    private const string TestInterstitialId = "ca-app-pub-3940256099942544/4411468910";
    private const string TestVideoId = "ca-app-pub-3940256099942544/1712485313";
    private const string TestOpenId = "ca-app-pub-3940256099942544/9257395921";
    private const string TestNativeBannerId = "ca-app-pub-3940256099942544/2934735716";
    protected const string TestCollapsibleBannerId = "ca-app-pub-3940256099942544/6300978111";
    protected const string TestNativeId = "ca-app-pub-3940256099942544/2247696110";

#endif
    public string[] testDeviceIds = new[] {"B15BB24370A69E2E02CE1EA2F2E5BD10"}; // redmi 5a

    [MyButton(nameof(FillTestAdsId))] public int test;

    public override void FillTestAdsId()
    {
        playerPrefBannerId = null;
        playerPrefInterstitialId = null;
        playerPrefOpenId = null;
        playerPrefRewardedId = null;
        playerPrefNativeBannerId = null;
        playerPrefCollapsibleBannerId = null;
        playerPrefNativeId = null;

#if UNITY_ANDROID
        bannerId = TestBannerId;
        interstitialId = TestInterstitialId;
        videoId = TestVideoId;
        openId = TestOpenId;
        nativeBannerId = TestNativeBannerId;
        collapsibleBannerId = TestCollapsibleBannerId;
        nativeId = TestNativeId;
#else
        bannerId_ios = TestBannerId;
        interstitialId_ios = TestInterstitialId;
        videoId_ios = TestVideoId;
        openId_ios = TestOpenId;
        nativeBannerId_ios = TestNativeBannerId;
        collapsibleBannerId_ios = TestCollapsibleBannerId;
        nativeId_ios = TestNativeId;
#endif
    }


    [Header("Android")] public string bannerId;
    public string interstitialId;
    public string videoId;
    public string openId;
    public string nativeBannerId;
    public string collapsibleBannerId;
    public string nativeId;

    [Header("IOS")] public string bannerId_ios;
    public string interstitialId_ios;
    public string videoId_ios;
    public string openId_ios;
    public string nativeBannerId_ios;
    public string collapsibleBannerId_ios;
    public string nativeId_ios;

    // ca-app-pub-3940256099942544~3347511713
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitial;
    private AppOpenAd _appOpenAds;
    private BannerView _bannerView;
    private BannerView _nativeBannerView;
    private BannerView _collapsibleBannerView;
#if use_admob_native_ad
    private NativeAd _nativeAd;
#endif

    private bool _consentReady;

    public override bool ConsentReady
    {
        get => _consentReady;
        protected set => _consentReady = value;
    }

    private string bannerAdapter;
    private string interstitialAdapter;
    private string rewardedAdapter;
    private string openAdsAdapter;
    private string nativeBannerAdapter;
    private string collapsibleBannerAdapter;
    private string nativeAdapter;


    [SerializeField] private PlayerPrefRemoteArrayInt ctr_time_logs = new PlayerPrefRemoteArrayInt(
        nameof(ctr_time_logs), new[]
        {
            20, 45, 45, 90
        });


    private IEnumerator IeRequestInters(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: Request Interstitial");
        RequestInterstitial();
    }

    private IEnumerator IeRequestReward(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        RequestRewardedAd();
    }

    private IEnumerator IeRequestOpenAds(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        RequestOpenAds();
    }

    public override bool IsInitialized()
    {
        return Initialized;
    }

    public override bool IntersIsShowing()
    {
//        UIDebugLog.Log0(_intersState, false,LogType.Ads);
        return _intersState == AdsState.Showing;
    }

    public override bool VideoIsShowing()
    {
        //      UIDebugLog.Log0("_videoState" + _videoState, false,LogType.Ads);
        return _rewardedState == AdsState.Showing;
    }
    

    public override void SetupAction(int index, IntAction onInitialized, BooleanAction onVideoAdsLoaded,
        BaseAction onVideoAdsRewarded, BooleanAction bannerShowed, bool bannerShow,  BaseAction onPaidAd = null)
    {
        Debug.Log("duong " + ConsentInformation.ConsentStatus);
        if (RemoteConfigKey.gdpr_reset.GetValueBoolean())
            ResetGDPR();

        base.SetupAction(index, onInitialized, onVideoAdsLoaded, onVideoAdsRewarded, bannerShowed, bannerShow,onPaidAd);

        bool load = RemoteConfigKey.gdpr_force.GetValueBoolean() ||
                    loadGDPR && !RemoteConfigKey.gdpr_ignore.GetValueBoolean();

       
        Debug.Log(load + "RemoteConfigKey.gdpr_force.GetValueBoolean()" + RemoteConfigKey.gdpr_force.GetValueBoolean());
        if (!load && SdkExtenstions.GetEpochDate() >= RemoteConfigKey.start_gdpr_date.GetValueInt())
        {
            load = true;
            Debug.Log(load + "RemoteConfigKey.StartGdprDate.GetValueInt()" + RemoteConfigKey.start_gdpr_date.GetValueInt());
        }
        
#if UNITY_EDITOR
        if (turnOffGdprEditor)
        {
            load = false;
        }
#endif
        
//#if !UNITY_EDITOR
        if (load)
        {
            LoadGDPR();
        }
        else
        {
            ConsentReady = true;
            Initialized = true;
            CheckInitialized();
        }

//#endif

        StartCoroutine(waitFor());
    }
    


    private IEnumerator waitFor(float timeWaitMax = 3)
    {
        float t = timeWaitMax;
        while (t > 0 && !FireBaseController.DependencyStatusAvailable)
        {
            yield return null;
            t -= Time.deltaTime;
        }

        while (!ConsentReady)
            yield return null;
        Debug.Log("duong wait to consent ready : pass");
        SetupAdmob(FireBaseController.DependencyStatusAvailable);
    }

    public void ResetGDPR()
    {
        Debug.LogError("Reset GDPR");
        ConsentInformation.Reset();
        PlayerPrefs.SetInt(PlayerPrefEnum.cache_consent_admob.ToString(), 0);
        ConsentReady = false;
    }

    public override bool IsRemoteActive()
    {
        return RemoteConfigKey.mediation_platform.GetValueString() == "admob";
    }

    [SerializeField] private bool loadGDPR;
    [SerializeField] private bool turnOffGdprEditor;
    
    [SerializeField] private TimeStartLoadAds timeStartLoadAds;

    private void SetupAdmob(bool logFireBaseInitFinish)
    {
        Debug.Log("duong SetupAdmob");
#if UNITY_IOS
        MobileAds.SetiOSAppPauseOnBackground(true);
#endif

        List<String> deviceIds = new List<String>()
            {AdRequest.TestDeviceSimulator};
        //        deviceIds.Add("B15BB24370A69E2E02CE1EA2F2E5BD10");
        foreach (var testAdsId in testDeviceIds)
            deviceIds.Add(testAdsId);

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefEnum.test_device_id.ToString())))
            deviceIds.Add(PlayerPrefs.GetString(PlayerPrefEnum.test_device_id.ToString()));
#if admob_9x_or_newer
        RequestConfiguration requestConfiguration = new RequestConfiguration()
        {
            TestDeviceIds = deviceIds,
        };
        
#else
RequestConfiguration requestConfiguration =
            new RequestConfiguration.Builder()
                .SetTestDeviceIds(deviceIds).build();
#endif



        MobileAds.SetRequestConfiguration(requestConfiguration);
        MobileAds.Initialize(HandleInitCompleteAction);
        if (Kernel.IsReady())
        {
            if (logFireBaseInitFinish)
                Kernel.Resolve<FireBaseController>()
                    .LogEvent("admob_sdk_init_start", new LogParameter("flow_seq", "1"));
        }
    }


    public override void LoadGDPR()
    {
        if (PlayerPrefs.GetInt(PlayerPrefEnum.cache_consent_admob.ToString()) == 1)
        {
            ConsentReady = true;
            Initialized = true;
            CheckInitialized();
            return;
        }

        List<String> deviceIds = new List<String>()
            {AdRequest.TestDeviceSimulator};
        //        deviceIds.Add("B15BB24370A69E2E02CE1EA2F2E5BD10");
        foreach (var testAdsId in testDeviceIds)
            deviceIds.Add(testAdsId);

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefEnum.test_device_id.ToString())))
            deviceIds.Add(PlayerPrefs.GetString(PlayerPrefEnum.test_device_id.ToString()));

        var debugSettings = new ConsentDebugSettings
        {
            // Geography appears as in EEA for debug devices.
            DebugGeography = DebugGeography.EEA,
            TestDeviceHashedIds = deviceIds
        };

        // Here false means users are not under age.
        ConsentRequestParameters request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
            ConsentDebugSettings = debugSettings,
        };

        // Check the current consent information status.
        Debug.Log($" ConsentInformation.Update");
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }


    private void OnConsentInfoUpdated(FormError error)
    {
        Debug.Log($"duong OnConsentInfoUpdated IsConsentFormAvailable:{ConsentInformation.IsConsentFormAvailable()}");
        //if (!ConsentInformation.IsConsentFormAvailable())
        //{
            ConsentReady = true;
            Initialized = true;
            CheckInitialized();
        //}

        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError("duong " + error.Message);
            return;
        }

        if (ConsentInformation.IsConsentFormAvailable())
        {
            Debug.Log(
                $"duong LoadConsentForm ConsentInformation.IsConsentFormAvailable(){ConsentInformation.IsConsentFormAvailable()}");
            LoadConsentForm();
        }

        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
    }

    private ConsentForm _consentForm;

    void LoadConsentForm()
    {
        // Loads a consent form.
        ConsentForm.Load(OnLoadConsentForm);
    }

    private bool consentFlag;

    void OnLoadConsentForm(ConsentForm consentForm, FormError error)
    {
        UIDebugLog.Log0($"duong OnLoadConsentForm ConsentInformation.ConsentStatus{ConsentInformation.ConsentStatus}",
            true);

        if (
            ConsentInformation.ConsentStatus == ConsentStatus.Obtained
//            || ConsentInformation.ConsentStatus == ConsentStatus.NotRequired
        )
        {
            if (ConsentInformation.ConsentStatus == ConsentStatus.Obtained && consentFlag)
            {
                PlayerPrefs.SetInt(PlayerPrefEnum.cache_consent_admob.ToString(), 1);
                new CustomSonatLog("confirm_consent", new List<Sonat.LogParameter>()).Post(false, 2);
                new CustomSonatLog(
                    AdmobConsentHelper.CanShowAds()
                        ? sonat_log_enum.cmp_user_consent
                        : sonat_log_enum.cmp_user_do_not_consent, new List<Sonat.LogParameter>()).Post(false, 2);

                Kernel.Resolve<FireBaseController>().SetUserProperty("consent_status",
                    AdmobConsentHelper.CanShowAds() ? " consent" : "do_not_consent");
                if (Kernel.kernel != null)
                    Kernel.kernel.CheckConsent();
            }
        }


        if (ConsentInformation.ConsentStatus == ConsentStatus.Obtained || ConsentInformation.ConsentStatus ==
                                                                       ConsentStatus.NotRequired
                                                                       || ConsentInformation.ConsentStatus ==
                                                                       ConsentStatus.Unknown)
        {
            ConsentReady = true;
            Initialized = true;
            CheckInitialized();
			if (!PlayerPrefs.HasKey("ADMOB_TRACKING_GDPR"))
			{
                PlayerPrefs.SetInt("ADMOB_TRACKING_GDPR", 1);
                new SonatLogGDPR()
                {
                    status = ConsentInformation.ConsentStatus.ToString()
                }.Post();
			}
        }

        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        // The consent form was loaded.
        // Save the consent form for future requests.
        _consentForm = consentForm;

        // You are now ready to show the form.
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {
            _consentForm.Show(OnShowForm);
            new CustomSonatLog(sonat_log_enum.cmp_impression, null).Post(false, 2);
            consentFlag = true;
        }
    }


    void OnShowForm(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        AdsManager.FakeLastTimeShowBanner();
        // Handle dismissal by reloading form.
        LoadConsentForm();
    }

    private AdsState _intersState = AdsState.NotStart;
    private AdsState _openAdsState = AdsState.NotStart;
    private AdsState _rewardedState = AdsState.NotStart;

    private void HandleInitCompleteAction(InitializationStatus initStatus)
    {
        Debug.Log("duong SetupAdmob HandleInitCompleteAction");
        if (Kernel.IsReady())
            Kernel.Resolve<FireBaseController>().LogEvent("admob_sdk_init_complete", new LogParameter("flow_seq", "2"));
        else
            Kernel.KernelLoaded.Action += () =>
            {
                Kernel.Resolve<FireBaseController>()
                    .LogEvent("admob_sdk_init_complete", new LogParameter("flow_seq", "2"));
            };

        ShowId();

        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
//            Initialized = true;
//            CheckInitialized();
            UIDebugLog.Log0("admob : _initialized", false,LogType.Ads);
            status = initStatus.getAdapterStatusMap();
            if (status != null)
                foreach (var mediation in status)
                    UIDebugLog.Log0(
                        $"admob Adapter={mediation.Key} InitializationState={mediation.Value.InitializationState}",
                        false,LogType.Ads);

            StartCoroutine(WaitToRequest());
           // StartRequest();
        });
    }

    private void StartRequest()
    {
        StartCoroutine(IeRequestBanner(timeStartLoadAds.timeLoadBanner));
        StartCoroutine(IeRequestNativeBanner(timeStartLoadAds.timeLoadNative));
        StartCoroutine(IeRequestInters(timeStartLoadAds.timeLoadInterstitial));
        StartCoroutine(IeRequestOpenAds(timeStartLoadAds.timeLoadOpenAds));
        StartCoroutine(IeRequestReward(timeStartLoadAds.timeLoadRewarded));
        StartCoroutine(IeRequestCollapsibleBanner(timeStartLoadAds.timeLoadBanner));
        StartCoroutine(IeRequestNativeAd(timeStartLoadAds.timeLoadNative));
    }

    IEnumerator WaitToRequest()
    {
        yield return null;
        while (AdsManager.WaitAtt)
            yield return null;
        StartRequest();
    }


    private Dictionary<string, AdapterStatus> status;

    public override void RequestNewAds()
    {
        UIDebugLog.Log0(nameof(RequestNewAds), false,LogType.Ads);

        _bannerView?.Destroy();
        _bannerView = null;
        _bannerState = AdsState.NotStart;
        showBanner = true;

        _nativeBannerView?.Destroy();
        _nativeBannerView = null;
        _nativeBannerState = AdsState.NotStart;

        _interstitial?.Destroy();
        _interstitial = null;
        _intersState = AdsState.NotStart;

        _appOpenAds?.Destroy();
        _appOpenAds = null;
        _openAdsState = AdsState.NotStart;

        _rewardedAd?.Destroy();
        _rewardedAd = null;
        _rewardedState = AdsState.NotStart;
        VideoLoaded.Action.Invoke(false);


        StartCoroutine(IeRequestBanner(1));
        StartCoroutine(IeRequestNativeBanner(1));
        StartCoroutine(IeRequestInters(2));
        StartCoroutine(IeRequestOpenAds(3));
        StartCoroutine(IeRequestReward(4));
        StartCoroutine(IeRequestCollapsibleBanner(1));
        StartCoroutine(IeRequestNativeAd(5));

        Kernel.Resolve<FireBaseController>().LogEvent(nameof(RequestNewAds));
    }

    private void RequestInterstitial()
    {
        if (string.IsNullOrEmpty(InterstitialId))
        {
            _intersState = AdsState.Failed;
            UIDebugLog.Log0("RequestInterstitial Fail InterstitialId=null", false,LogType.Ads);
            return;
        }

        if (!Kernel.IsInternetConnection())
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: RequestInterstitial failed no Internet");
            _intersState = AdsState.Failed;
            HandleOnInterstitialAdFailedToLoad(null);
            return;
        }

        if (!ConsentReady)
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: Consent is not set");
            HandleOnInterstitialAdFailedToLoad(null);
            return;
        }

        if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: RequestInterstitial" + GetLast(InterstitialId), false,LogType.Ads);
        _intersState = AdsState.Requesting;

        if (_interstitial != null)
        {
            _interstitial.Destroy();
            _interstitial = null;
        }

#if admob_9x_or_newer
        var adRequest = new AdRequest();
#else
        var adRequest = new AdRequest.Builder()
            .Build();
#endif

        // send the request to load the ad.
        InterstitialAd.Load(InterstitialId, adRequest,
            (ad, error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    HandleOnInterstitialAdFailedToLoad(error);
                    return;
                }

                _interstitial = ad;
                HandleOnInterstitialAdLoaded();
                _interstitial.OnAdFullScreenContentOpened += HandleOnInterstitialAdOpening;
                // Called when the ad is closed.
                _interstitial.OnAdFullScreenContentClosed += HandleOnInterstitialAdClosed;
                _interstitial.OnAdFullScreenContentFailed += HandleOnInterstitialAdFailedToShow;
                _interstitial.OnAdClicked += HandleOnInterstitialAdClick;
                // Called when the ad click caused the user to leave the application.
                _interstitial.OnAdPaid += (adValue) =>
                {
                    HandleAdPaidEvent(adValue, AdTypeLog.interstitial, interstitialAdapter, AdsPlatform.googleadmob);
                    ResponseInfo responseInfo = _interstitial.GetResponseInfo();
                    string adRespondId = responseInfo.GetResponseId();
                    string adSource = responseInfo.GetMediationAdapterClassName();
                    var fireBase2 = Kernel.Resolve<FireBaseController>();
                    var parameters2 = fireBase2.GetAdmobParameter(10, new[]
                    {
                        new LogParameter("ad_source", adSource),
                        new LogParameter("ad_value", adValue.Value),
                        new LogParameter("ad_response_id", adRespondId),
                    }, AdTypeLog.interstitial).ToArray();
                    fireBase2.LogEvent("admob_ad_open_success", parameters2);
                };
            });

        var fireBase = Kernel.Resolve<FireBaseController>();
        var parameters = fireBase.GetAdmobParameter(3, new[]
        {
            new LogParameter(ParameterEnum.ad_id, GetLast(InterstitialId)),
        }, AdTypeLog.interstitial).ToArray();
        fireBase.LogEvent(AdTypeLogStep.admob_ad_request.ToString(), parameters);
    }

    private void HandleOnInterstitialAdClick()
    {
        new SonatPaidAdClick()
        {
            ad_format = AdTypeLog.interstitial,
            ad_placement = SonatAnalyticTracker.InterstitialLogName,
            fb_instance_id = Kernel.Resolve<FireBaseController>().FirebaseInstanceId
        }.Post();
        adDurationTracker.OnUserClickedAd();
    }

    private void HandleOnOpenAdClick()
    {
        new SonatPaidAdClick()
        {
            ad_format = AdTypeLog.app_open,
            ad_placement = SonatAnalyticTracker.InterstitialLogName,
            fb_instance_id = Kernel.Resolve<FireBaseController>().FirebaseInstanceId
        }.Post();
    }


    private void HandleOnRewardedAdClick()
    {
        new SonatPaidAdClick()
        {
            ad_format = AdTypeLog.rewarded_video,
            ad_placement = SonatAnalyticTracker.RewardedLogPlacement,
            fb_instance_id = Kernel.Resolve<FireBaseController>().FirebaseInstanceId
        }.Post();
        adDurationTracker.OnUserClickedAd();
    }

    private void HandleOnBannerAdClick()
    {
        new SonatPaidAdClick()
        {
            ad_format = AdTypeLog.banner,
            ad_placement = "banner",
            fb_instance_id = Kernel.Resolve<FireBaseController>().FirebaseInstanceId
        }.Post();
    }

    private void HandleOnNativeBannerAdClick()
    {
        new SonatPaidAdClick()
        {
            ad_format = AdTypeLog.native,
            ad_placement = "banner",
            fb_instance_id = Kernel.Resolve<FireBaseController>().FirebaseInstanceId
        }.Post();
    }

    private void HandleOnCollapsibleBannerAdClick()
    {
        new SonatPaidAdClick()
        {
            ad_format = AdTypeLog.collapsible_banner,
            ad_placement = "banner",
            fb_instance_id = Kernel.Resolve<FireBaseController>().FirebaseInstanceId
        }.Post();
    }

    private void RequestOpenAds()
    {
        if (string.IsNullOrEmpty(OpenId))
        {
            _openAdsState = AdsState.Failed;
            UIDebugLog.Log0("RequestOpenAds Fail OpenId null", false,LogType.Ads);
            return;
        }

        if (!Kernel.IsInternetConnection())
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: RequestOpenAds failed no Internet");
            _openAdsState = AdsState.Failed;
            HandleOnOpenAdFailedToLoad(null);
            return;
        }

        if (!ConsentReady)
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: Consent is not set");
            HandleOnOpenAdFailedToLoad(null);
            return;
        }

        if (string.IsNullOrEmpty(OpenId))
        {
            UIDebugLog.Log0("AdsInstanceAdmob RequestOpenAds Err : OpenAdId null", false,LogType.Ads);
            return;
        }

        if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: RequestOpenAds", false,LogType.Ads);
        _openAdsState = AdsState.Requesting;

        if (_appOpenAds != null)
        {
            _appOpenAds.Destroy();
            _appOpenAds = null;
        }

#if admob_9x_or_newer
        AdRequest adRequest = new AdRequest();
#else
AdRequest adRequest = new AdRequest.Builder().Build();
#endif

        AppOpenAd.Load(OpenId,
#if !admob_9x_or_newer
            ScreenOrientation.Portrait,
#endif
            adRequest,
            (ad, error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    HandleOnOpenAdFailedToLoad(error);
                    return;
                }

                _appOpenAds = ad;
                HandleOnOpenAdLoaded();
            });

        var fireBase = Kernel.Resolve<FireBaseController>();
        var parameters = fireBase.GetAdmobParameter(3, new LogParameter[]
        {
            new LogParameter(ParameterEnum.ad_id, GetLast(OpenId)),
        }, AdTypeLog.app_open).ToArray();
        fireBase.LogEvent(AdTypeLogStep.admob_ad_request.ToString(), parameters);
    }

    public override bool IsAppOpenAdsReady()
    {
        return _appOpenAds != null && _appOpenAds.CanShowAd();
    }

    public override bool ShowAppOpenAds()
    {
        if (!IsAppOpenAdsReady() || _openAdsState == AdsState.Showing)
            return false;

        _appOpenAds.OnAdFullScreenContentClosed += HandleOnOpenAdsClosed;
        _appOpenAds.OnAdFullScreenContentFailed += HandleOnOpenAdsFailToShow;
        _appOpenAds.OnAdFullScreenContentOpened += HandleOnOpenAdsOpened;
        _appOpenAds.OnAdClicked += HandleOnOpenAdClick;
        // _appOpenAds.OnAdDidRecordImpression += OnAdDidRecordImpression;
        _appOpenAds.OnAdPaid += (adValue) =>
        {
            HandleAdPaidEvent(adValue, AdTypeLog.app_open, openAdsAdapter, AdsPlatform.googleadmob);
            ResponseInfo responseInfo = _appOpenAds.GetResponseInfo();
            string adRespondId = responseInfo.GetResponseId();
            string adSource = responseInfo.GetMediationAdapterClassName();
            var fireBase2 = Kernel.Resolve<FireBaseController>();
            var parameters2 = fireBase2.GetAdmobParameter(10, new[]
            {
                new LogParameter("ad_source", adSource),
                new LogParameter("ad_value", adValue.Value),
                new LogParameter("ad_response_id", adRespondId),
            }, AdTypeLog.interstitial).ToArray();
            fireBase2.LogEvent("admob_ad_open_success", parameters2);
        };

        Debug.Log("show open ads");
        _appOpenAds.Show();
        return true;
    }

    public void RequestRewardedAd()
    {
        if (string.IsNullOrEmpty(VideoId))
        {
            _rewardedState = AdsState.Failed;
            UIDebugLog.Log0("RequestRewardedAd Fail VideoId null", false,LogType.Ads);
            return;
        }

        if (!Kernel.IsInternetConnection())
        {
            _rewardedState = AdsState.Failed;
            HandleOnRewardAdFailedToLoad(null);
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: RequestRewardedAd failed no Internet");
            return;
        }

        if (!ConsentReady)
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: Consent is not set");
            HandleOnRewardAdFailedToLoad(null);
            return;
        }

        if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: CreateAndLoadRewardedAd", false,LogType.Ads);

        _rewardedState = AdsState.Requesting;

        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

#if admob_9x_or_newer
        AdRequest adRequest = new AdRequest();
#else
AdRequest adRequest = new AdRequest.Builder()
            .Build();
#endif

        RewardedAd.Load(VideoId, adRequest,
            (ad, error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    HandleOnRewardAdFailedToLoad(error);
                    return;
                }

                _rewardedAd = ad;
                HandleRewardedAdLoaded();
                _rewardedAd.OnAdFullScreenContentOpened += HandleRewardedAdsOpened;
                // Called when the ad is closed.
                _rewardedAd.OnAdFullScreenContentClosed += HandleRewardedAdClosed;
                _rewardedAd.OnAdFullScreenContentFailed += HandleOnRewardAdFailedToShow;
                _rewardedAd.OnAdClicked += HandleOnRewardedAdClick;
                // Called when the ad click caused the user to leave the application.
                _rewardedAd.OnAdPaid += (adValue) =>
                {
                    HandleAdPaidEvent(adValue, AdTypeLog.rewarded_video, rewardedAdapter, AdsPlatform.googleadmob);
                    ResponseInfo responseInfo = _rewardedAd.GetResponseInfo();
                    string adRespondId = responseInfo.GetResponseId();
                    string adSource = responseInfo.GetMediationAdapterClassName();
                    var fireBase2 = Kernel.Resolve<FireBaseController>();
                    var parameters2 = fireBase2.GetAdmobParameter(10, new[]
                    {
                        new LogParameter("ad_source", adSource),
                        new LogParameter("ad_value", adValue.Value),
                        new LogParameter("ad_response_id", adRespondId),
                    }, AdTypeLog.rewarded_video).ToArray();
                    fireBase2.LogEvent("admob_ad_open_success", parameters2);
                };
            });


        var fireBase = Kernel.Resolve<FireBaseController>();
        var parameters = fireBase.GetAdmobParameter(3, new LogParameter[]
        {
            new LogParameter(ParameterEnum.ad_id, GetLast(VideoId)),
        }, AdTypeLog.rewarded_video).ToArray();
        fireBase.LogEvent(AdTypeLogStep.admob_ad_request.ToString(), parameters);
    }


    private void HandleOnInterstitialAdLoaded()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            UIDebugLog.Log0(nameof(HandleOnInterstitialAdLoaded) + ":" + GetLast(InterstitialId) + " event received",
                false,LogType.Ads);
            retryIntersAttempt = 1;
            _intersState = AdsState.Loaded;
            if (_interstitial?.GetResponseInfo() != null)
            {
                var responseInfo = _interstitial.GetResponseInfo();
                interstitialAdapter = responseInfo.GetMediationAdapterClassName();
                try
                {
                    string adRespondId = responseInfo.GetResponseId();
                    string adSource = responseInfo.GetMediationAdapterClassName();
                    var fireBase = Kernel.Resolve<FireBaseController>();
                    var parameters = fireBase.GetAdmobParameter(4, new[]
                    {
                        new LogParameter("ad_source", adSource),
                        new LogParameter("ad_response_id", adRespondId),
                    }, AdTypeLog.interstitial).ToArray();
                    fireBase.LogEvent("admob_ad_load_success", parameters);
                }
                catch (Exception e)
                {
                    Kernel.Resolve<FireBaseController>().LogCrashException(e);
                }
            }
        });
    }

    private void HandleRewardedAdLoaded()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _retryRewardAttempt = 1;
            _rewardedState = AdsState.Loaded;
            rewardedAdapter = _rewardedAd.GetResponseInfo().GetMediationAdapterClassName();
            if (logFail)
                UIDebugLog.Log0(
                    nameof(HandleRewardedAdLoaded) + GetLast(VideoId) + " - Rewarded video loaded:" +
                    _rewardedAd.CanShowAd(),
                    false,LogType.Ads);
            VideoLoaded.Action.Invoke(true);


            ResponseInfo responseInfo = _rewardedAd.GetResponseInfo();
            string adRespondId = responseInfo.GetResponseId();
            string adSource = responseInfo.GetMediationAdapterClassName();
            var fireBase = Kernel.Resolve<FireBaseController>();
            var parameters = fireBase.GetAdmobParameter(4, new[]
            {
                new LogParameter("ad_source", adSource),
                new LogParameter("ad_response_id", adRespondId),
            }, AdTypeLog.rewarded_video).ToArray();
            fireBase.LogEvent("admob_ad_load_success", parameters);
        });
    }

    private void HandleAdPaidEvent(AdValue adValue, AdTypeLog adType, string adapter,
        AdsPlatform platform)
    {
        string placement_name = "";
        if (adType == AdTypeLog.banner)
            placement_name = "banner";
        if (adType == AdTypeLog.interstitial || adType == AdTypeLog.app_open)
            placement_name = SonatAnalyticTracker.InterstitialLogName;
        if (adType == AdTypeLog.rewarded)
            placement_name = SonatAnalyticTracker.RewardedLogPlacement;

        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            double revenue = adValue.Value / ((double)1000000f);
            
            if (logFail) UIDebugLog.Log0(adType + " " + nameof(HandleAdPaidEvent) + " event received ", false,LogType.Ads);
            SonatAnalyticTracker.LogRevenue(platform, adapter, revenue,
                adValue.Precision.ToString(), adType, Kernel.Resolve<FireBaseController>().FirebaseInstanceId,
                placement_name, adValue.CurrencyCode);

            if(adType != AdTypeLog.banner)
			{
                AdDurationTracker.AdMetrics adMetrics = adDurationTracker.OnAdDismissedFullScreenContent();
                new SonatLogAdDuration(AdsPlatform.googleadmob.ToString(), adType.ToString(), adapter, revenue.ToString(), adMetrics).Post();
            }

            if(OnPaidAd != null)
                OnPaidAd.Action.Invoke();
//            if (firebaseController.logCtr)
//            {
//                if (adType == AdTypeLog.interstitial || adType == AdTypeLog.app_open || adType == AdTypeLog.rewarded)
//                {
//                    string placement_name = "";
//                    if (adType == AdTypeLog.banner)
//                        placement_name = "banner";
//                    if (adType == AdTypeLog.interstitial || adType == AdTypeLog.app_open)
//                        placement_name = SonatAnalyticTracker.InterstitialLogName;
//                    if (adType == AdTypeLog.rewarded)
//                        placement_name = SonatAnalyticTracker.RewardedLogName;
//                    Kernel.Resolve<FireBaseController>().LogEvent(EventNameEnum.ctr_impression, new[]
//                    {
//                        new LogParameter(ParameterEnum.format, adTypeLogName[(int) adType]),
//                        new LogParameter(ParameterEnum.ad_value, adValue.Value),
//                        new LogParameter(ParameterEnum.placement_name, placement_name),
//                        new LogParameter(ParameterEnum.fb_instance_id,
//                            Kernel.Resolve<FireBaseController>().FirebaseInstanceId),
//                    });
//                }
//            }
        });
    }

    private void HandleRewardedAdsOpened()
    {
        if (logFail)
            UIDebugLog.Log0(nameof(HandleRewardedAdsOpened));
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _rewardedState = AdsState.Showing;
            VideoLoaded.Action.Invoke(false);
        });
    }


    private AdSize _adaptiveSize;

    //    private void OnApplicationPause(bool pause)
    //    {
    //        Debug.Log("OnApplicationPause", gameObject);
    //    }

    private float _lastTimeFocus;

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            if (Time.realtimeSinceStartup - _lastTimeFocus > 30 * 60)
            {
                StartCoroutine(IeRequestBanner(1));
                StartCoroutine(IeRequestNativeBanner(1));
                StartCoroutine(IeRequestInters(5));
                StartCoroutine(IeRequestReward(12));
                StartCoroutine(IeRequestCollapsibleBanner(1));
                StartCoroutine(IeRequestNativeAd(8));
            }

            _lastTimeFocus = Time.realtimeSinceStartup;
        }
    }

#region Rewarded

    private int _retryRewardAttempt;

    private void HandleOnRewardAdFailedToLoad(AdError adError)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _retryRewardAttempt++;
            double retryDelay = Math.Pow(2, Mathf.Clamp(_retryRewardAttempt, 3, 7));
            _rewardedAd?.Destroy();
            _rewardedAd = null;
            StartCoroutine(IeRequestReward((float) retryDelay));

            if (logFail)
            {
#if UNITY_ANDROID || (UNITY_IOS && LOG_FAIL_IOS) // ios get mess crash
                var mess = adError == null ? "adError=null" : adError.GetMessage();
                UIDebugLog.Log0(
                    $"{nameof(HandleOnRewardAdFailedToLoad)} ({GetLast(VideoId)})  event received with message: {mess}",
                    false,LogType.Ads);
#endif

                UIDebugLog.Log0(
                    $"Reload Rewarded in {(float) retryDelay} seconds",
                    false,LogType.Ads);
            }

#if UNITY_ANDROID
            if (adError != null)
            {
                int errorCode = adError.GetCode();
                var fireBase = Kernel.Resolve<FireBaseController>();
                var parameters = fireBase.GetAdmobParameter(5, new[]
                {
                    new LogParameter("error_code", errorCode),
                }, AdTypeLog.rewarded_video).ToArray();
                fireBase.LogEvent("admob_ad_load_fail", parameters);
            }
#else
        var fireBase = Kernel.Resolve<FireBaseController>();
        var parameters = fireBase.GetAdmobParameter(5, new[]
        {
            new LogParameter("error_code", "911"),
        }, AdTypeLog.rewarded_video).ToArray();
        fireBase.LogEvent("admob_ad_load_fail", parameters);
#endif
        });
    }

    private void HandleOnRewardAdFailedToShow(AdError addError)
    {
#if UNITY_ANDROID
        if (addError != null)
        {
            ResponseInfo responseInfo = _rewardedAd.GetResponseInfo();
            int errorCode = addError.GetCode();
            string adRespondId = responseInfo.GetResponseId();
            string adSource = responseInfo.GetMediationAdapterClassName();
            var fireBase2 = Kernel.Resolve<FireBaseController>();
            var parameters2 = fireBase2.GetAdmobParameter(11, new[]
            {
                new LogParameter("ad_source", adSource),
                new LogParameter("ad_response_id", adRespondId),
                new LogParameter("error_code", errorCode),
            }, AdTypeLog.rewarded_video).ToArray();
            fireBase2.LogEvent("admob_ad_open_fail", parameters2);
        }
#else
        var fireBase = Kernel.Resolve<FireBaseController>();
        var parameters = fireBase.GetAdmobParameter(5, new[]
        {
            new LogParameter("error_code", "911"),
        }, AdTypeLog.rewarded_video).ToArray();
        fireBase.LogEvent("admob_ad_load_fail", parameters);
#endif

        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _retryRewardAttempt++;
            double retryDelay = Math.Pow(2, Mathf.Clamp(_retryRewardAttempt, 3, 7));
            _rewardedAd?.Destroy();
            _rewardedAd = null;
            StartCoroutine(IeRequestReward((float) retryDelay));

            if (logFail)
                UIDebugLog.Log0(nameof(HandleOnRewardAdFailedToShow) + " retryDelay-" + retryDelay, false,LogType.Ads);
        });
    }

    private void HandleRewardedAdClosed()
    {
        new SonatLogLastScreenView().Post();

        if (logFail)
            UIDebugLog.Log0(nameof(HandleRewardedAdClosed));

        if (Kernel.IsReady())
        {
            ResponseInfo responseInfo = _rewardedAd.GetResponseInfo();
            string adSource = responseInfo.GetMediationAdapterClassName();
            var fireBase2 = Kernel.Resolve<FireBaseController>();
            var parameters2 = fireBase2.GetAdmobParameter(12, new[]
            {
                new LogParameter("ad_source", adSource),
            }, AdTypeLog.rewarded_video).ToArray();
            fireBase2.LogEvent("admob_ad_close", parameters2);
        }

        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _rewardedState = AdsState.Closed;
            StartCoroutine(IeRequestReward(.5f));
            StartCoroutine(WaitToRewardFalse());
//            var tRewards = Time.realtimeSinceStartup - _timeShowRewardsAds;
//            if (tRewards >= ctr_time_logs.Value[2] && tRewards < ctr_time_logs.Value[3])
//            {
//                UIDebugLog.Log0("ctr_click_reward");
//            }
//            _timeShowRewardsAds = -1000;
        });
    }

    private static IEnumerator WaitToRewardFalse()
    {
        yield return new WaitForSecondsRealtime(3);
        AdsManager.VideoOpened = false;
    }

    private void HandleUserEarnedReward(Reward reward)
    {
        if (Kernel.IsReady())
        {
            ResponseInfo responseInfo = _rewardedAd.GetResponseInfo();
            string adSource = responseInfo.GetMediationAdapterClassName();
            string respondId = responseInfo.GetResponseId();
            var fireBase2 = Kernel.Resolve<FireBaseController>();
            var parameters2 = fireBase2.GetAdmobParameter(13, new[]
            {
                new LogParameter("ad_source", adSource),
                new LogParameter("ad_response_id", respondId),
            }, AdTypeLog.rewarded_video).ToArray();
            fireBase2.LogEvent("admob_reward_earn", parameters2);
        }

        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            if (logFail) UIDebugLog.Log0(nameof(HandleUserEarnedReward), false,LogType.Ads);
            VideoRewarded.Action.Invoke();
        });
    }


    public override void ShowVideoAds()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            adDurationTracker.OnAdShowedFullScreenContent();
            _rewardedAd.Show(HandleUserEarnedReward);
        }
        else
        {
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
        return _rewardedAd != null && _rewardedState == AdsState.Loaded;
    }

#endregion

#region interstitial

    public override bool IsInterstitialReady()
    {
        // if (RemoteConfigKey.use_open_ads_as_interstitial.GetValueBoolean() && IsAppOpenAdsReady())
        //     return true;
        return IsRealInterstitialReady();
    }

    private bool IsRealInterstitialReady()
    {
        return _interstitial != null && _interstitial.CanShowAd();
    }

    public override void ShowInterstitial(bool isOnFocus)
    {
        UIDebugLog.Log0("useOpenAdsAsInterstitial.Value " + RemoteConfigKey.use_open_ads_as_interstitial.GetValueBoolean(), false,LogType.Ads);
        if (!(isOnFocus && RemoteConfigKey.use_open_ads_as_interstitial.GetValueBoolean() && ShowAppOpenAds()))
            if (IsRealInterstitialReady())
            {
                adDurationTracker.OnAdShowedFullScreenContent();
                _interstitial.Show();
                OnAdsShowed();
            }
    }

    int retryIntersAttempt;


    private void HandleOnInterstitialAdFailedToLoad(LoadAdError loadAdError)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            retryIntersAttempt++;
            double retryDelay = Math.Pow(2, Mathf.Clamp(retryIntersAttempt, 3, 8));
            _interstitial?.Destroy();
            _interstitial = null;
            StartCoroutine(IeRequestInters((float) retryDelay));

            if (logFail && loadAdError != null)
            {
#if UNITY_ANDROID || (UNITY_IOS && LOG_FAIL_IOS) // ios get mess crash
                var mess = loadAdError.GetResponseInfo();
                UIDebugLog.Log0(
                    $"{nameof(HandleOnInterstitialAdFailedToLoad)} ({GetLast(InterstitialId)}) event received with message: {mess}",
                    false,LogType.Ads);
#endif
                UIDebugLog.Log0($"Reload Interstitial in {(float) retryDelay} seconds", false,LogType.Ads);
            }
        });

#if UNITY_ANDROID
        if (loadAdError != null)
        {
            int errorCode = loadAdError.GetCode();
            var fireBase = Kernel.Resolve<FireBaseController>();
            var parameters = fireBase.GetAdmobParameter(5, new[]
            {
                new LogParameter("error_code", errorCode),
            }, AdTypeLog.interstitial).ToArray();
            fireBase.LogEvent("admob_ad_load_fail", parameters);
        }
#else
        var fireBase = Kernel.Resolve<FireBaseController>();
        var parameters = fireBase.GetAdmobParameter(5, new[]
        {
            new LogParameter("error_code", "911"),
        }, AdTypeLog.interstitial).ToArray();
        fireBase.LogEvent("admob_ad_load_fail", parameters);
#endif
    }

    private void HandleOnInterstitialAdFailedToShow(AdError args)
    {
        string messErr = String.Empty;
#if UNITY_ANDROID
        if (args != null)
        {
            ResponseInfo responseInfo = _interstitial.GetResponseInfo();
            int errorCode = args.GetCode();
            string adRespondId = responseInfo.GetResponseId();
            string adSource = responseInfo.GetMediationAdapterClassName();
            var fireBase = Kernel.Resolve<FireBaseController>();
            var parameters2 = fireBase.GetAdmobParameter(11, new[]
            {
                new LogParameter("ad_source", adSource),
                new LogParameter("ad_response_id", adRespondId),
                new LogParameter("error_code", errorCode),
            }, AdTypeLog.interstitial).ToArray();
            fireBase.LogEvent("admob_ad_open_fail", parameters2);

            if (logFail)
            {
                try
                {
                    messErr = args.GetMessage();
                }
                catch
                {
                    // ignored
                }
            }
        }
#else
        var fireBase = Kernel.Resolve<FireBaseController>();
        var parameters = fireBase.GetAdmobParameter(5, new[]
        {
            new LogParameter("error_code", "911"),
        }, AdTypeLog.interstitial).ToArray();
        fireBase.LogEvent("admob_ad_load_fail", parameters);
#endif

        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _interstitial?.Destroy();
            _interstitial = null;
            _intersState = AdsState.Closed;
            OnAdsClosed();
            new SonatLogLastScreenView().Post();

            StartCoroutine(IeRequestInters(.5f));
            if (logFail)
                UIDebugLog.Log0(nameof(HandleOnInterstitialAdFailedToShow) + " event received with message: " + messErr,
                    false,LogType.Ads);
        });
    }


    private void HandleOnInterstitialAdOpening()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _intersState = AdsState.Showing;
            AdsManager.OpenAdsOrInterstitialOpened = true;
            if (logFail) UIDebugLog.Log0(nameof(HandleOnInterstitialAdOpening) + " event received", false,LogType.Ads);
        });
    }

    private void HandleOnInterstitialAdClosed()
    {
        OnAdsClosed();
        ResponseInfo responseInfo = _interstitial.GetResponseInfo();
        new SonatLogLastScreenView().Post();
//        if (args != null)
        {
            string adSource = responseInfo.GetMediationAdapterClassName();
            var fireBase2 = Kernel.Resolve<FireBaseController>();
            var parameters2 = fireBase2.GetAdmobParameter(12, new[]
            {
                new LogParameter("ad_source", adSource),
            }, AdTypeLog.interstitial).ToArray();
            fireBase2.LogEvent("admob_ad_close", parameters2);
        }

        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _intersState = AdsState.Closed;
            if (logFail) UIDebugLog.Log0(nameof(HandleOnInterstitialAdClosed) + " event received", false,LogType.Ads);
            _interstitial?.Destroy();
            StartCoroutine(IeRequestInters(.5f));
            StartCoroutine(WaitToInterstitialFalse());
        });
    }

    private static IEnumerator WaitToInterstitialFalse()
    {
        yield return new WaitForSecondsRealtime(3);
        AdsManager.OpenAdsOrInterstitialOpened = false;
    }

    // open ads
    private int retryOpenAttempt;

    private void HandleOnOpenAdFailedToLoad(AdError addError)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            retryOpenAttempt++;
            double retryDelay = Math.Pow(2, Mathf.Clamp(retryOpenAttempt, 3, 8));
            _appOpenAds?.Destroy();
            _appOpenAds = null;
            StartCoroutine(IeRequestOpenAds((float) retryDelay));

            if (logFail)
            {
#if UNITY_ANDROID || (UNITY_IOS && LOG_FAIL_IOS) // ios get mess crash
                var mess = addError == null ? "adError=null" : addError.GetMessage();
                UIDebugLog.Log0(
                    $"{nameof(HandleOnOpenAdFailedToLoad)} ({GetLast(OpenId)})  event received with message: {mess}",
                    false,LogType.Ads);
#endif

                UIDebugLog.Log0(
                    $"Reload OpenAds in {(float) retryDelay} seconds",
                    false,LogType.Ads);
            }
        });

#if UNITY_ANDROID
        if (addError != null)
        {
            int errorCode = addError.GetCode();
            var fireBase = Kernel.Resolve<FireBaseController>();
            var parameters = fireBase.GetAdmobParameter(5, new[]
            {
                new LogParameter("error_code", errorCode),
            }, AdTypeLog.app_open).ToArray();
            fireBase.LogEvent("admob_ad_load_fail", parameters);
        }
#else
            var fireBase = Kernel.Resolve<FireBaseController>();
            var parameters = fireBase.GetAdmobParameter(5, new[]
            {
                new LogParameter("error_code", "911"),
            }, AdTypeLog.app_open).ToArray();
            fireBase.LogEvent("admob_ad_load_fail", parameters);
#endif
    }

    private void HandleOnOpenAdsFailToShow(AdError adError)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            /* app open cannot get respond
            int errorCode = args.AdError.GetCode();
            ResponseInfo responseInfo = _appOpenAds.GetResponseInfo();
            string adRespondId = responseInfo.GetResponseId();
            string adSource = responseInfo.GetMediationAdapterClassName();
            var fireBase2 = Kernel.Resolve<FireBaseController>();
            var parameters2 = fireBase2.GetAdmobParameter(11, new[]
            {
                new LogParameter("ad_source", adSource),
                new LogParameter("ad_response_id", adRespondId),
                new LogParameter("error_code", errorCode),
            },AdTypeLog.app_open).ToArray();
            fireBase2.LogEvent("admob_ad_open_fail",parameters2);
            */

            _appOpenAds?.Destroy();
            _appOpenAds = null;
            _openAdsState = AdsState.Closed;
            StartCoroutine(IeRequestOpenAds(.5f));
            if (logFail)
            {
                var mess = adError.GetMessage();
                UIDebugLog.Log0(nameof(HandleOnOpenAdFailedToLoad) + " event received with message: " + mess,
                    false,LogType.Ads);
            }
        });
    }

    private void HandleOnOpenAdsOpened()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            AdsManager.OpenAdsOrInterstitialOpened = true;
            if (logFail) UIDebugLog.Log0(nameof(HandleOnOpenAdsOpened) + " event received", false,LogType.Ads);
        });
    }

    private void HandleOnOpenAdsClosed()
    {
        if (RemoteConfigKey.use_open_ads_as_interstitial.GetValueBoolean())
        {
            OnAdsClosed();
        }
        Debug.Log("Closed app open ad");
        ResetTimeShowOnFocusInter();

        //        ResponseInfo responseInfo = _appOpenAds.GetResponseInfo();
        //        string adSource = responseInfo.GetMediationAdapterClassName();
        //        var fireBase2 = Kernel.Resolve<FireBaseController>();
        //        var parameters2 = fireBase2.GetAdmobParameter(12, new[]
        //        {
        //            new LogParameter("ad_source", adSource),
        //        },AdTypeLog.app_open).ToArray();
        //        fireBase2.LogEvent("admob_ad_close",parameters2);

        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _openAdsState = AdsState.Closed;
            //        if (Kernel.IsReady() && !Kernel.Pause)
            //            Time.timeScale = 1f;
            if (logFail) UIDebugLog.Log0(nameof(HandleOnOpenAdsClosed) + " event received", false,LogType.Ads);
            _appOpenAds?.Destroy();
            StartCoroutine(IeRequestOpenAds(.5f));
            StartCoroutine(waitToOpenFalse());
        });
    }

    private void HandleOnOpenAdLoaded()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            retryOpenAttempt = 1;
            _openAdsState = AdsState.Loaded;

            //            Debug.Log("HandleOnOpenAdLoaded ExecuteInUpdate 2"+_appOpenAds.GetResponseInfo());
            //            
            //            openAdsAdapter = _appOpenAds.GetResponseInfo().GetMediationAdapterClassName();
            if (logFail)
                UIDebugLog.Log0(nameof(HandleOnOpenAdLoaded) + " event received: " + GetLast(OpenId), false,LogType.Ads);
            //            
            //            Debug.Log("HandleOnOpenAdLoaded ExecuteInUpdate end");
            //
            //            ResponseInfo responseInfo = _appOpenAds.GetResponseInfo();
            //            string adRespondId = responseInfo.GetResponseId();
            //            string adSource = responseInfo.GetMediationAdapterClassName();
            //            var fireBase = Kernel.Resolve<FireBaseController>();
            //            var parameters = fireBase.GetAdmobParameter(4, new[]
            //            {
            //                new LogParameter("ad_source", adSource),
            //                new LogParameter("ad_response_id", adRespondId),
            //            },AdTypeLog.app_open).ToArray();
            //            fireBase.LogEvent("admob_ad_load_success",parameters);
        });
    }


    IEnumerator waitToOpenFalse()
    {
        Debug.Log("duong waitToOpenFalse");

        yield return new WaitForSecondsRealtime(3);
        AdsManager.OpenAdsOrInterstitialOpened = false;
    }

    public void HandleOnOpenAdLeavingApplication(object sender, EventArgs args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            Kernel.Resolve<AppFlyerController>().SendEvent("af_ad_click", new Dictionary<string, string>()
            {
                {"af_adrev_ad_type", ""}
            });
            if (logFail)
                UIDebugLog.Log0(nameof(HandleOnOpenAdLeavingApplication) + " event received", false,LogType.Ads);
        });
    }

    #endregion

    #region Mrec banner
    public override void ShowMrecBanner()
    {
        ShowNativeBanner();
    }

    public override void HideMrecBanner()
    {
        HideNativeBanner();
    }

    public override bool IsMrecBannerReady()
    {
        return IsNativeReady();
    }
    #endregion
    
    [Serializable]
    public class TimeStartLoadAds
    {
        public float timeLoadOpenAds = 0.5f;
        public float timeLoadBanner = 1f;
        public float timeLoadInterstitial = 5f;
        public float timeLoadRewarded = 2f;
        public float timeLoadNative = 4f;
    }
}
#endif