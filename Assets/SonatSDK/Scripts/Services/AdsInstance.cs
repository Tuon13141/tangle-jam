using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AdsInstance : MonoBehaviour
{
    public bool testAds;

    public enum AdsState
    {
        NotStart = 0,
        Requesting = 1,
        Failed = 2,
        Loaded = 3,
        Showing = 4,
        Closed = 5,
        Hidden  =6,
    }
    
//    [SerializeField] protected  PlayerPrefRemoteBool loadNative =
//        new PlayerPrefRemoteBool(RemoteConfigKey.load_native_ads, true);

    protected virtual bool LoadNative => RemoteConfigKey.load_native_ads.GetValueBoolean();
    protected virtual bool LoadMrec => RemoteConfigKey.load_mrec_ads.GetValueBoolean();
    protected virtual bool LoadCollapsible => RemoteConfigKey.load_collapsible_ads.GetValueBoolean();

    protected IntAction InitializedEvent = new IntAction();
    protected BaseAction VideoRewarded = new BaseAction();
    protected BaseAction OnPaidAd = new BaseAction();
    protected BooleanAction OnBannerShow = new BooleanAction();
    protected BooleanAction VideoLoaded = new BooleanAction();
    public bool showBanner = true;
    public bool isDefault = true;
    protected int _index;

    public virtual bool ConsentReady { get; protected set; }
    protected virtual bool Initialized { get; set; }
    protected AdDurationTracker adDurationTracker;
    
    public void CheckInitialized()
    {
        if (Initialized && ConsentReady)
            InitializedEvent.Action.Invoke(_index);
    }
    
    public virtual void SetupAction(int index,IntAction onInitialized, BooleanAction onVideoAdsLoaded,
        BaseAction onVideoAdsRewarded, BooleanAction bannerShowed, bool bannerShow, BaseAction onPaidAd = null)
    {
        adDurationTracker = AdDurationTracker.Instance;
        _index = index;
        InitializedEvent = onInitialized;
        VideoLoaded = onVideoAdsLoaded;
        VideoRewarded = onVideoAdsRewarded;
        OnBannerShow = bannerShowed;
        OnPaidAd = onPaidAd;
        showBanner = bannerShow;
    }

    public virtual void LoadGDPR()
    {
        
    }

    public virtual void OnTrackingYes(bool value)
    {
       
    }

    public virtual void FillTestAdsId()
    {
        
    }
    
    public virtual void EnableTestAds()
    {
        testAds = true;
    }

    public virtual void ShowId()
    {
        
    }

    public static string GetLast(string source, int tailLength = 6)
    {
        if (string.IsNullOrEmpty(source))
            return "";
        if (tailLength > source.Length)
            return "";
        
        return source.Substring(source.Length - tailLength);
    }
    
    
    public abstract void ShowVideoAds();
    public abstract void LoadVideoAds();
    public abstract bool IsVideoAdsReady();
    public abstract bool IsInterstitialReady();
    public abstract void ShowInterstitial(bool isOnFocus);
    public abstract void CheckShowBanner();
    public abstract bool BannerIsShowing();
    public abstract bool IntersIsShowing();
    public abstract bool VideoIsShowing();
    public abstract void DestroyBanner();
    public abstract bool IsInitialized();
    
    public abstract void HideBanner();
    public abstract void ShowBanner();
    
    public abstract void HideNativeBanner();
    public abstract void ShowNativeBanner();
    public abstract bool IsNativeReady();

    public abstract void ShowMrecBanner();
    public abstract void HideMrecBanner();
    public abstract bool IsMrecBannerReady();

    public abstract bool ShowAppOpenAds();
    public abstract bool IsAppOpenAdsReady();

    public abstract void ShowCollapsibleBanner();
    public abstract void HideCollapsibleBanner();
    public abstract bool IsCollapsibleBannerReady();

    public abstract bool IsNativeAdReady();
    public abstract INativeAd GetNativeAd();
    public abstract void HideNativeAd();

    private bool isNative;

    public abstract bool IsRemoteActive();

    public virtual void HideNative()
    {
        HideNativeBanner();
    }
    
    public virtual bool SwitchNativeBanner(bool toNative)
    {
        if (isNative == toNative)
            return false;
        if (toNative)
        {
            if (IsNativeReady())
            {
                HideBanner();
                ShowNativeBanner();
                isNative = true;
                return true;
            }

            return false;
        }

        isNative = false;
        HideNativeBanner();
        ShowBanner();
        return true;
    }

    public abstract void RequestNewAds();

    public abstract void ShowDebugger();

    public virtual void OnAdsShowed()
	{
//#if UNITY_IOS
        Time.timeScale = 0;
//#endif
	}

    public virtual void OnAdsClosed()
	{
        Time.timeScale = 1;
        Kernel.Resolve<AdsManager>().StartCallbackInter();
	}

    public virtual void ResetTimeShowOnFocusInter()
    {
        Kernel.Resolve<AdsManager>().ResetTimeShowOnFocusInter();
    }

    //private void OnApplicationFocus(bool focus)
    //{
    //	if (!focus)
    //	{
    //           adDurationTracker.OnUserLeavesApp();
    //	}
    //	else
    //	{
    //           adDurationTracker.OnUserReturnsToApp();
    //       }
    //}
}
