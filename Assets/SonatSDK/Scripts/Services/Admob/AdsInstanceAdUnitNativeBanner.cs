#define dummy
//#define use_admob
using Sonat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using LogType = sonat_sdk.Scripts.LogType;
using Object = System.Object;
#if !((dummy || global_dummy) && !use_admob)
using GoogleMobileAds.Common;
using GoogleMobileAds.Api;
using Sonat;

#endif


#if (dummy || global_dummy) && !use_admob
// no
#else
public partial class AdsInstanceAdmob
{
    private AdsState _nativeBannerState = AdsState.NotStart;

    private IEnumerator IeRequestNativeBanner(float time)
    {
        UIDebugLog.Log0("IeRequestNativeBanner",false, LogType.Ads);
        yield return new WaitForSeconds(time);
        RequestNativeBanner();
    }

    public void 
        CheckShowNativeBanner()
    {
        switch (_nativeBannerState)
        {
            case AdsState.Requesting:
                break;
            case AdsState.Failed:
                RequestNativeBanner();
                break;
            case AdsState.Loaded:
                break;
            case AdsState.Showing:
                break;
            case AdsState.Closed:
                RequestNativeBanner();
                break;
            case AdsState.NotStart:
                RequestNativeBanner();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RequestNativeBanner()
    {
        if (string.IsNullOrEmpty(NativeBannerId))
        {
            _nativeBannerState = AdsState.NotStart;
            UIDebugLog.Log0("RequestNativeBanner Fail NativeBannerId null",false,LogType.Ads);
            return;
        }
     
        
#if UNITY_EDITOR
        if (TestingSettings.TurnOffBanner)
        {
            UIDebugLog.Log0("NativeBanner not show" + nameof(TestingSettings.TurnOffBanner),false, LogType.Ads);
            return;
        }
#endif
        if(!LoadNative)
            return;

        if (!Kernel.IsInternetConnection())
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: RequestNativeBanner failed no Internet");
            HandleNativeBannerAdFailedToLoad(null);
            return;
        }
        
        if (!ConsentReady)
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: Consent is not set");
            HandleNativeBannerAdFailedToLoad(null);
            return;
        }

        if (AdsManager.NoAds)
        {
            UIDebugLog.Log0("Banner not show " + nameof(AdsManager.NoAds),false, LogType.Ads);
            return;
        }

        if (AdsManager.NotShowBanner)
        {
            UIDebugLog.Log0("NativeBanner not show" + nameof(AdsManager.NotShowBanner),false, LogType.Ads);
            return;
        }

        if (!showBanner)
        {
            _nativeBannerState = AdsState.NotStart;
            return;
        }

        if (_nativeBannerState == AdsState.Requesting)
            return;

        if (Initialized)
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: RequestNativeBanner"+GetLast(NativeBannerId),false, LogType.Ads);
            _nativeBannerView?.Destroy();
            _nativeBannerView = null;
            _nativeBannerView = new BannerView(NativeBannerId, AdSize.MediumRectangle, AdPosition.Bottom);
            //            float widthInPixels = Screen.safeArea.width > 0 ? Screen.safeArea.width : Screen.width;
            //            int width = (int)(widthInPixels / MobileAds.Utils.GetDeviceScale());
            //            AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveNativeBannerAdSizeWithWidth(width);
            //            _nativeBannerView = new NativeBannerView(NativeBannerId, adaptiveSize, AdPosition.Bottom);

            // Register for ad events.
            _nativeBannerView.OnBannerAdLoaded += HandleNativeBannerAdLoaded;
            _nativeBannerView.OnBannerAdLoadFailed += HandleNativeBannerAdFailedToLoad;
            _nativeBannerView.OnAdFullScreenContentOpened += HandleNativeBannerAdOpened;
            _nativeBannerView.OnAdFullScreenContentClosed += HandleNativeBannerAdClosed;
            _nativeBannerView.OnAdClicked += HandleOnNativeBannerAdClick;
            _nativeBannerView.OnAdPaid += adValue =>
            {
                HandleAdPaidEvent(adValue, AdTypeLog.native_banner, nativeBannerAdapter, AdsPlatform.googleadmob);

                ResponseInfo responseInfo = _nativeBannerView.GetResponseInfo();
                string adRespondId = responseInfo.GetResponseId();
                string adSource = responseInfo.GetMediationAdapterClassName();
                var fireBase2 = Kernel.Resolve<FireBaseController>();
                var parameters2 = fireBase2.GetAdmobParameter(10, new[]
                {
                    new LogParameter("ad_source", adSource),
                    new LogParameter("ad_value", adValue.Value),
                    new LogParameter("ad_response_id", adRespondId),
                }, AdTypeLog.native_banner).ToArray();
                fireBase2.LogEvent("admob_ad_open_success", parameters2);
            };

            // Create an empty ad request.
#if admob_9x_or_newer
            AdRequest request = new AdRequest();
#else
AdRequest request = new AdRequest.Builder().Build();
#endif

            // Load the banner with the request.
            _nativeBannerView.LoadAd(request);
            _nativeBannerState = AdsState.Requesting;

            var fireBase = Kernel.Resolve<FireBaseController>();
            var parameters = fireBase.GetAdmobParameter(3, new[]
            {
                new LogParameter(ParameterEnum.ad_id,GetLast(NativeBannerId)), 
            }, AdTypeLog.native_banner).ToArray();
            fireBase.LogEvent(AdTypeLogStep.admob_ad_request.ToString(), parameters);
        }
    }

    public override bool IsNativeReady()
    {
       return _nativeBannerState == AdsState.Loaded || _nativeBannerState == AdsState.Hidden;
    }
    
    public override void HideNativeBanner()
    {
        if (_nativeBannerState == AdsState.Loaded)
        {
            _nativeBannerView?.Hide();
            _nativeBannerState = AdsState.Hidden;
        }
    }

    public override void ShowNativeBanner()
    {
        if (_nativeBannerState == AdsState.Hidden)
        {
            if (_nativeBannerView != null)
            {
                float dp = _nativeBannerView.GetWidthInPixels() / 300;
                float dpWidth = Screen.width / dp;
                float dpHeight = Screen.height / dp;
                float offsetDP = 0;

                if (_bannerView != null)
                {
                    offsetDP = (_bannerView.GetHeightInPixels() / dp) + 20;
                }

                _nativeBannerView.SetPosition((int)((dpWidth - AdSize.MediumRectangle.Width) / 2), (int)(dpHeight - AdSize.MediumRectangle.Height - offsetDP));

                _nativeBannerView?.Show();
                _nativeBannerState = AdsState.Loaded;
            }
        }
    }


    public void DestroyNativeBanner()
    {
        _nativeBannerState = AdsState.NotStart;
        OnBannerShow.Action.Invoke(false);
        if (_nativeBannerView == null)
            return;
        _nativeBannerView.Destroy();
        _nativeBannerView = null;
    }

    private void HandleNativeBannerAdLoaded()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _nativeBannerState = AdsState.Loaded;
            UIDebugLog.Log0("on NativeBanner Load:"+GetLast(NativeBannerId));
            nativeBannerAdapter = _nativeBannerView.GetResponseInfo().GetMediationAdapterClassName();
            HideNativeBanner();
            try
            {
                ResponseInfo responseInfo = _nativeBannerView.GetResponseInfo();
                string adRespondId = responseInfo.GetResponseId();
                string adSource = responseInfo.GetMediationAdapterClassName();
                var fireBase = Kernel.Resolve<FireBaseController>();
                var parameters = fireBase.GetAdmobParameter(4, new[]
                {
                    new LogParameter("ad_source", adSource),
                    new LogParameter("ad_response_id", adRespondId),
                }, AdTypeLog.native_banner).ToArray();
                fireBase.LogEvent("admob_ad_load_success", parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
    }

#region NativeBanner callback handlers

    int _retryNativeBannerAttempt;

    private void HandleNativeBannerAdFailedToLoad(AdError adError)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _nativeBannerState = AdsState.Failed;
            StartCoroutine(InvokeNativeBanner(false));

            _retryNativeBannerAttempt++;
            double retryDelay = Math.Pow(2, Mathf.Clamp(_retryNativeBannerAttempt, 3, 7));
            _nativeBannerView?.Destroy();
            _nativeBannerView = null;

            StartCoroutine(IeRequestNativeBanner((float) retryDelay));
            if (logFail)
            {
#if UNITY_ANDROID || (UNITY_IOS && LOG_FAIL_IOS) // ios get mess crash
                var mess = adError == null ?"adError=null" : adError.GetMessage();
                UIDebugLog.Log0(
                    $"{nameof(HandleNativeBannerAdFailedToLoad)} ({GetLast(NativeBannerId)})  event received with message: {mess}",
                    false,LogType.Ads);
#endif
                UIDebugLog.Log0(
                    $"Reload NativeBanner in {(float) retryDelay} seconds",
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
                }, AdTypeLog.native_banner).ToArray();
                fireBase.LogEvent("admob_ad_load_fail", parameters);
            }
#else
            var fireBase = Kernel.Resolve<FireBaseController>();
            var parameters = fireBase.GetAdmobParameter(5, new[]
            {
                new LogParameter("error_code", "911"),
            }, AdTypeLog.native_banner).ToArray();
            fireBase.LogEvent("admob_ad_load_fail", parameters);
#endif
        });
    }

    private IEnumerator InvokeNativeBanner(bool value)
    {
        yield return new WaitForSeconds(0.1f);
    }

    private void HandleNativeBannerAdOpened()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            StartCoroutine(InvokeNativeBanner(false));
            if (logFail) UIDebugLog.Log0(nameof(HandleNativeBannerAdOpened) + " event received",false, LogType.Ads);
        });
    }

    private void HandleNativeBannerAdClosed()
    {
        ResponseInfo responseInfo = _nativeBannerView.GetResponseInfo();
        string adSource = responseInfo.GetMediationAdapterClassName();
        var fireBase2 = Kernel.Resolve<FireBaseController>();
        var parameters2 = fireBase2.GetAdmobParameter(12, new[]
        {
            new LogParameter("ad_response_id", responseInfo.GetResponseId()),
            new LogParameter("ad_source", adSource),
        }, AdTypeLog.native_banner).ToArray();
        fireBase2.LogEvent("admob_ad_close", parameters2);


        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            DestroyNativeBanner();
            StartCoroutine(IeRequestNativeBanner(.5f));
            StartCoroutine(InvokeNativeBanner(false));
            if (logFail) UIDebugLog.Log0(nameof(HandleNativeBannerAdClosed) + " event received",false, LogType.Ads);
            StartCoroutine(waitToNativeBannerFalse());
        });
    }

    IEnumerator waitToNativeBannerFalse()
    {
        yield return new WaitForSeconds(5);
    }

#endregion
}

#endif