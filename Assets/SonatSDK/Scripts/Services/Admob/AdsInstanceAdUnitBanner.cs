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
    private AdsState _bannerState = AdsState.NotStart;

    private IEnumerator IeRequestBanner(float time)
    {
        UIDebugLog.Log0("IeRequestBanner", false,LogType.Ads);
        yield return new WaitForSeconds(time);
        RequestBanner();
    }

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
                RequestBanner();
                break;
            case AdsState.NotStart:
                RequestBanner();
                break;
            case AdsState.Hidden:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

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
            case AdsState.Hidden:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RequestBanner()
    {
        if (string.IsNullOrEmpty(BannerId))
        {
            _bannerState = AdsState.Failed;
            UIDebugLog.Log0("RequestBanner Fail BannerId null",false, LogType.Ads);
            return;
        }

        if (RemoteConfigKey.no_banner.GetValueBoolean())
        {
            _bannerState = AdsState.Failed;
            UIDebugLog.Log0("RequestBanner Fail RemoteConfigKey.no_banner",false, LogType.Ads);
            return;
        }

#if UNITY_EDITOR
        if (TestingSettings.TurnOffBanner)
        {
            UIDebugLog.Log0("Banner not show" + nameof(TestingSettings.TurnOffBanner),false, LogType.Ads);
            return;
        }
#endif

        if (!Kernel.IsInternetConnection())
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: RequestBanner failed no Internet");
            HandleBannerAdFailedToLoad(null);
            return;
        }

        if (!ConsentReady)
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: Consent is not set");
            HandleBannerAdFailedToLoad(null);
            return;
        }

        if (AdsManager.NoAds)
        {
            UIDebugLog.Log0("Banner not show " + nameof(AdsManager.NoAds),false, LogType.Ads);
            return;
        }


        if (AdsManager.NotShowBanner)
        {
            UIDebugLog.Log0("Banner not show " + nameof(AdsManager.NotShowBanner),false, LogType.Ads);
            return;
        }

        if (!showBanner)
        {
            _bannerState = AdsState.NotStart;
            return;
        }

        if (_bannerState == AdsState.Requesting)
            return;


        if (Initialized)
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: RequestBanner" + GetLast(BannerId),false, LogType.Ads);
            _bannerView?.Destroy();
            _bannerView = null;

            _adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            _bannerView = new BannerView(BannerId, _adaptiveSize, AdPosition.Bottom);
            //            float widthInPixels = Screen.safeArea.width > 0 ? Screen.safeArea.width : Screen.width;
            //            int width = (int)(widthInPixels / MobileAds.Utils.GetDeviceScale());
            //            AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(width);
            //            _bannerView = new BannerView(BannerId, adaptiveSize, AdPosition.Bottom);
#if admob_9x_or_newer
            AdRequest request = new AdRequest();
#else
 AdRequest request = new AdRequest.Builder().Build();
#endif

            // Register for ad events.
            _bannerView.OnBannerAdLoaded += HandleBannerAdLoaded;
            _bannerView.OnBannerAdLoadFailed += HandleBannerAdFailedToLoad;
            _bannerView.OnAdFullScreenContentOpened += HandleBannerAdOpened;
            _bannerView.OnAdFullScreenContentClosed += HandleBannerAdClosed;
            _bannerView.OnAdClicked += HandleOnBannerAdClick;
            _bannerView.OnAdPaid += adValue =>
            {
                HandleAdPaidEvent(adValue, AdTypeLog.banner, bannerAdapter, AdsPlatform.googleadmob);

                ResponseInfo responseInfo = _bannerView.GetResponseInfo();
                string adRespondId = responseInfo.GetResponseId();
                string adSource = responseInfo.GetMediationAdapterClassName();
                var fireBase2 = Kernel.Resolve<FireBaseController>();
                var parameters2 = fireBase2.GetAdmobParameter(10, new[]
                {
                    new LogParameter("ad_source", adSource),
                    new LogParameter("ad_value", adValue.Value),
                    new LogParameter("ad_response_id", adRespondId),
                }, AdTypeLog.banner).ToArray();
                fireBase2.LogEvent("admob_ad_open_success", parameters2);
            };
            _bannerView.LoadAd(request);
            _bannerState = AdsState.Requesting;

            var fireBase = Kernel.Resolve<FireBaseController>();
            var parameters = fireBase.GetAdmobParameter(3, new[]
            {
                new LogParameter(ParameterEnum.ad_id,GetLast(BannerId)), 
            }, AdTypeLog.banner).ToArray();
            fireBase.LogEvent(AdTypeLogStep.admob_ad_request.ToString(), parameters);
        }
    }

    public override void HideBanner()
    {
        OnBannerShow.Action.Invoke(false);
        AdsManager.showBannerOnLoad = false;
        if (_bannerState == AdsState.Loaded)
        {
            _bannerView?.Hide();
            _bannerState = AdsState.Hidden;
        }
    }

    public override void ShowBanner()
    {
        AdsManager.showBannerOnLoad = true;
        if (_bannerState == AdsState.Hidden)
        {
            OnBannerShow.Action.Invoke(true);
            _bannerView?.Show();
            _bannerState = AdsState.Loaded;
        }
        else
        {
            OnBannerShow.Action.Invoke(false);
        }
    }

    public override bool BannerIsShowing()
    {
        UIDebugLog.Log(_bannerState.ToString(),false, LogType.Ads);
        return _bannerState == AdsState.Loaded;
    }

    public override void DestroyBanner()
    {
        _bannerState = AdsState.NotStart;
        OnBannerShow.Action.Invoke(false);
        if (_bannerView == null)
            return;
        _bannerView.Destroy();
        _bannerView = null;
    }

    private string _lastBanner;

    private void HandleBannerAdLoaded()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _bannerState = AdsState.Loaded;
            if (!string.Equals(_lastBanner, BannerId, StringComparison.Ordinal))
            {
                _lastBanner = BannerId;
                UIDebugLog.Log0("HandleBannerAdLoaded=" + GetLast(_lastBanner),false, LogType.Ads);
            }

           

            if (_bannerView?.GetResponseInfo() != null)
            {
                ResponseInfo responseInfo = _bannerView.GetResponseInfo();
                AdsManager.BannerHeight = _bannerView.GetHeightInPixels();
                bannerAdapter = responseInfo.GetMediationAdapterClassName();

                try
                {
                    string adRespondId = responseInfo.GetResponseId();
                    string adSource = responseInfo.GetMediationAdapterClassName();
                    var fireBase = Kernel.Resolve<FireBaseController>();
                    var parameters = fireBase.GetAdmobParameter(4, new[]
                    {
                        new LogParameter("ad_source", adSource),
                        new LogParameter("ad_response_id", adRespondId),
                    }, AdTypeLog.banner).ToArray();
                    fireBase.LogEvent("admob_ad_load_success", parameters);
                }
                catch (Exception e)
                {
                    Kernel.Resolve<FireBaseController>().LogCrashException(e);
                }
            }
            
            if (!AdsManager.showBannerOnLoad || AdsManager.NoAds || AdsManager.NotShowBanner)
            {
                HideBanner();
            }
            else
            {
                if (OnBannerShow != null && OnBannerShow.Action != null)
                    OnBannerShow.Action.Invoke(true);
            }
        });
    }

#region Banner callback handlers

    int _retryBannerAttempt;

    private void HandleBannerAdFailedToLoad(AdError adError)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _bannerState = AdsState.Failed;
            StartCoroutine(InvokeBanner(false));

            _retryBannerAttempt++;
            double retryDelay = Math.Pow(2, Mathf.Clamp(_retryBannerAttempt, 3, 7));
            _bannerView?.Destroy();
            _bannerView = null;

            StartCoroutine(IeRequestBanner((float) retryDelay));
            if (logFail)
            {
#if UNITY_ANDROID || (UNITY_IOS && LOG_FAIL_IOS) // ios get mess crash
                var mess = adError == null ? "adError=null" : adError.GetMessage();
                UIDebugLog.Log0(
                    $"{nameof(HandleBannerAdFailedToLoad)} ({GetLast(BannerId)})  event received with message: {mess}",false,
                    LogType.Ads);
#endif
                UIDebugLog.Log0(
                    $"Reload Banner in {(float) retryDelay} seconds",false,
                    LogType.Ads);
            }

#if UNITY_ANDROID
            if (adError != null)
            {
                int errorCode = adError.GetCode();
                var fireBase = Kernel.Resolve<FireBaseController>();
                var parameters = fireBase.GetAdmobParameter(5, new[]
                {
                    new LogParameter("error_code", errorCode),
                }, AdTypeLog.banner).ToArray();
                fireBase.LogEvent("admob_ad_load_fail", parameters);
            }
#else
            var fireBase = Kernel.Resolve<FireBaseController>();
            var parameters = fireBase.GetAdmobParameter(5, new[]
            {
                new LogParameter("error_code", "911"),
            }, AdTypeLog.banner).ToArray();
            fireBase.LogEvent("admob_ad_load_fail", parameters);
#endif
        });
    }

    private IEnumerator InvokeBanner(bool value)
    {
        yield return new WaitForSeconds(0.1f);
        OnBannerShow.Action.Invoke(value);
    }

    private void HandleBannerAdOpened()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            AdsManager.BannerOpened = true;
            StartCoroutine(InvokeBanner(false));
            if (logFail) UIDebugLog.Log0(nameof(HandleBannerAdOpened) + " event received",false, LogType.Ads);
        });
    }

    private void HandleBannerAdClosed()
    {
        ResponseInfo responseInfo = _bannerView.GetResponseInfo();
        string adSource = responseInfo.GetMediationAdapterClassName();
        var fireBase2 = Kernel.Resolve<FireBaseController>();
        var parameters2 = fireBase2.GetAdmobParameter(12, new[]
        {
            new LogParameter("ad_response_id", responseInfo.GetResponseId()),
            new LogParameter("ad_source", adSource),
        }, AdTypeLog.banner).ToArray();
        fireBase2.LogEvent("admob_ad_close", parameters2);

        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            DestroyBanner();
            StartCoroutine(IeRequestBanner(.5f));
            StartCoroutine(InvokeBanner(false));
            if (logFail) UIDebugLog.Log0(nameof(HandleBannerAdClosed) + " event received",false, LogType.Ads);
            StartCoroutine(waitToBannerFalse());
        });
    }

    IEnumerator waitToBannerFalse()
    {
        yield return new WaitForSeconds(5);
        AdsManager.BannerOpened = false;
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            if (logFail) UIDebugLog.Log0(nameof(HandleBannerAdClosed) + " event received",false, LogType.Ads);
        });
    }

#endregion
}

#endif