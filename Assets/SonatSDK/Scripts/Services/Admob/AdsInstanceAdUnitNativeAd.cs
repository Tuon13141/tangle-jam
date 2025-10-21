#define dummy
//#define use_admob
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using LogType = sonat_sdk.Scripts.LogType;
using Sonat;

#if !((dummy || global_dummy) && !use_admob)
using GoogleMobileAds.Common;
using GoogleMobileAds.Api;
#endif


#if (dummy || global_dummy) && !use_admob
// no
#elif !use_admob_native_ad
public partial class AdsInstanceAdmob
{
    private IEnumerator IeRequestNativeAd(float time)
    {
        yield break;
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
public partial class AdsInstanceAdmob
{
    private AdsState _nativeAdState = AdsState.NotStart;

    private IEnumerator IeRequestNativeAd(float time)
    {
        UIDebugLog.Log0("IeRequestNativeAds", false, LogType.Ads);
        yield return new WaitForSeconds(time);
        RequestNativeAd();
    }

    public void CheckShowNativeAd()
    {
        switch (_nativeAdState)
        {
            case AdsState.Requesting:
                break;
            case AdsState.Failed:
                RequestNativeAd();
                break;
            case AdsState.Loaded:
                break;
            case AdsState.Showing:
                break;
            case AdsState.Closed:
                RequestNativeAd();
                break;
            case AdsState.NotStart:
                RequestNativeAd();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RequestNativeAd()
    {
        if (string.IsNullOrEmpty(NativeId))
        {
            _nativeAdState = AdsState.NotStart;
            UIDebugLog.Log0("RequestNativeAd Fail NativeId null", false, LogType.Ads);
            return;
        }

        if (!Kernel.IsInternetConnection())
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: RequestNativeAd failed no Internet");
            HandleNativeAdFailedToLoad(null, null);
            return;
        }

        if (!ConsentReady)
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: Consent is not set");
            HandleNativeAdFailedToLoad(null, null);
            return;
        }

        if (AdsManager.NoAds)
        {
            UIDebugLog.Log0("NativeAd not show " + nameof(AdsManager.NoAds), false, LogType.Ads);
            return;
        }

        if (!showBanner)
        {
            _nativeAdState = AdsState.NotStart;
            return;
        }

        if (_nativeAdState == AdsState.Requesting)
            return;

        if (Initialized)
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: RequestNativeAd" + GetLast(NativeId), false, LogType.Ads);

            _nativeAd?.Destroy();
            _nativeAd = null;

            // Create an empty ad request.
            AdLoader adLoader = new AdLoader.Builder(NativeId)
                .ForNativeAd()
                .Build();

            adLoader.OnNativeAdLoaded += HandleNativeAdLoaded;
            adLoader.OnAdFailedToLoad += HandleNativeAdFailedToLoad;
            adLoader.OnNativeAdOpening += HandleNativeAdOpened;
            adLoader.OnNativeAdClosed += HandleNativeAdClosed;
            adLoader.OnNativeAdClicked += HandleOnNativeAdClick;

#if admob_9x_or_newer
            AdRequest request = new AdRequest();
#else
            AdRequest request = new AdRequest.Builder().Build();
#endif

            // Load the banner with the request.
            adLoader.LoadAd(request);
            _nativeAdState = AdsState.Requesting;

            var fireBase = Kernel.Resolve<FireBaseController>();
            var parameters = fireBase.GetAdmobParameter(3, new[]
            {
                new LogParameter(ParameterEnum.ad_id,GetLast(NativeId)),
            }, AdTypeLog.native_ad).ToArray();
            fireBase.LogEvent(AdTypeLogStep.admob_ad_request.ToString(), parameters); 
        }
    }

    public override bool IsNativeAdReady()
    {
        return _nativeAdState == AdsState.Loaded /*|| _nativeAdState == AdsState.Hidden*/;
    }

    public override void HideNativeAd()
    {
        if (_nativeAdState == AdsState.Loaded)
        {
            _nativeAdState = AdsState.Hidden;

            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                DestroyNativeAd();
                StartCoroutine(IeRequestNativeAd(.5f));
                StartCoroutine(InvokeNativeAd(false));
                if (logFail) UIDebugLog.Log0(nameof(HandleNativeAdClosed) + " event received", false, LogType.Ads);
            });
        }
    }

    public override INativeAd GetNativeAd()
    {
        AdmobNativeAd resultAds = null;

        if (IsNativeAdReady() && _nativeAd != null)
        {
            resultAds = new AdmobNativeAd();
            resultAds.Ad = _nativeAd;
        }

        return resultAds;
    }

    public void DestroyNativeAd()
    {
        _nativeAdState = AdsState.NotStart;
        OnBannerShow.Action.Invoke(false);
        if (_nativeAd == null)
            return;
        _nativeAd.Destroy();
        _nativeAd = null;
    }

    private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _nativeAdState = AdsState.Loaded;
            _nativeAd = args.nativeAd;

            UIDebugLog.Log0("on Native Ad Load:" + GetLast(NativeId));
            nativeAdapter = _nativeAd.GetResponseInfo().GetMediationAdapterClassName();

            ResponseInfo responseInfo = _nativeAd.GetResponseInfo();
            string adRespondId = responseInfo.GetResponseId();
            string adSource = responseInfo.GetMediationAdapterClassName();
            var fireBase = Kernel.Resolve<FireBaseController>();

            _nativeAd.OnPaidEvent += (sender, adValue) =>
            {
                HandleAdPaidEvent(adValue.AdValue, AdTypeLog.native_ad, nativeAdapter, AdsPlatform.googleadmob);

                var parameters2 = fireBase.GetAdmobParameter(10, new[]
                {
                    new LogParameter("ad_source", adSource),
                    new LogParameter("ad_value", adValue.AdValue.Value),
                    new LogParameter("ad_response_id", adRespondId),
                }, AdTypeLog.native_ad).ToArray();
                fireBase.LogEvent("admob_ad_open_success", parameters2);
            };

            try
            {
                var parameters = fireBase.GetAdmobParameter(4, new[]
                {
                    new LogParameter("ad_source", adSource),
                    new LogParameter("ad_response_id", adRespondId),
                }, AdTypeLog.native_ad).ToArray();
                fireBase.LogEvent("admob_ad_load_success", parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
    }

    #region NativeAd callback handlers
    int _retryNativeAdAttempt;

    private void HandleNativeAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _nativeAdState = AdsState.Failed;
            StartCoroutine(InvokeNativeAd(false));

            _retryNativeAdAttempt++;
            double retryDelay = Math.Pow(2, Mathf.Clamp(_retryNativeAdAttempt, 3, 7));
            _nativeAd?.Destroy();
            _nativeAd = null;

            StartCoroutine(IeRequestNativeAd((float)retryDelay));
            if (logFail)
            {
#if UNITY_ANDROID || (UNITY_IOS && LOG_FAIL_IOS) // ios get mess crash
                var mess = args == null ? "adError=null" : args.LoadAdError.GetMessage();
                UIDebugLog.Log0(
                    $"{nameof(HandleNativeAdFailedToLoad)} ({GetLast(NativeId)})  event received with message: {mess}",
                    false, LogType.Ads);
#endif
                UIDebugLog.Log0(
                    $"Reload NativeAds in {(float)retryDelay} seconds",
                    false, LogType.Ads);
            }

#if UNITY_ANDROID
            if (args != null)
            {
                int errorCode = args.LoadAdError.GetCode();
                var fireBase = Kernel.Resolve<FireBaseController>();
                var parameters = fireBase.GetAdmobParameter(5, new[]
                {
                    new LogParameter("error_code", errorCode),
                }, AdTypeLog.native_ad).ToArray();
                fireBase.LogEvent("admob_ad_load_fail", parameters);
            }
#else
            var fireBase = Kernel.Resolve<FireBaseController>();
            var parameters = fireBase.GetAdmobParameter(5, new[]
            {
                new LogParameter("error_code", "911"),
            }, AdTypeLog.native_ad).ToArray();
            fireBase.LogEvent("admob_ad_load_fail", parameters);
#endif
        });
    }

    private IEnumerator InvokeNativeAd(bool value)
    {
        yield return new WaitForSeconds(0.1f);
    }

    private void HandleNativeAdOpened(object sender, EventArgs args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            StartCoroutine(InvokeNativeAd(false));
            if (logFail) UIDebugLog.Log0(nameof(HandleNativeAdOpened) + " event received", false, LogType.Ads);
        });
    }

    private void HandleNativeAdClosed(object sender, EventArgs args)
    {
        ResponseInfo responseInfo = _nativeAd.GetResponseInfo();
        string adSource = responseInfo.GetMediationAdapterClassName();
        var fireBase2 = Kernel.Resolve<FireBaseController>();

        var parameters2 = fireBase2.GetAdmobParameter(12, new[]
        {
            new LogParameter("ad_response_id", responseInfo.GetResponseId()),
            new LogParameter("ad_source", adSource),
        }, AdTypeLog.native_ad).ToArray();
        fireBase2.LogEvent("admob_ad_close", parameters2);
    }

    private void HandleOnNativeAdClick(object sender, EventArgs args)
    {
        new SonatPaidAdClick()
        {
            ad_format = AdTypeLog.native_ad,
            ad_placement = "banner",
            fb_instance_id = Kernel.Resolve<FireBaseController>().FirebaseInstanceId
        }.Post();
    }
    #endregion
}
#endif