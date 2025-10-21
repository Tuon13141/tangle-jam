#define dummy
//#define use_admob
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using LogType = sonat_sdk.Scripts.LogType;
#if !((dummy || global_dummy) && !use_admob)
using GoogleMobileAds.Common;
using GoogleMobileAds.Api;
#endif


#if (dummy || global_dummy) && !use_admob
// no
#else
public partial class AdsInstanceAdmob
{
    private AdsState _collapsibleBannerState = AdsState.NotStart;

    private IEnumerator IeRequestCollapsibleBanner(float time)
    {
        UIDebugLog.Log0("IeRequestCollapsibleBanner", false, LogType.Ads);
        yield return new WaitForSeconds(time);
        RequestCollapsibleBanner();
    }

    public void CheckShowCollapsibleBanner()
    {
        switch (_collapsibleBannerState)
        {
            case AdsState.Requesting:
                break;
            case AdsState.Failed:
                RequestCollapsibleBanner();
                break;
            case AdsState.Loaded:
                break;
            case AdsState.Showing:
                break;
            case AdsState.Closed:
                RequestCollapsibleBanner();
                break;
            case AdsState.NotStart:
                RequestCollapsibleBanner();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RequestCollapsibleBanner()
    {
        if (string.IsNullOrEmpty(CollapsibleBannerId))
        {
            _collapsibleBannerState = AdsState.NotStart;
            UIDebugLog.Log0("RequestCollapsibleBanner Fail CollapsibleBannerId null", false, LogType.Ads);
            return;
        }

#if UNITY_EDITOR
        if (TestingSettings.TurnOffBanner)
        {
            UIDebugLog.Log0("CollapsibleBanner not show" + nameof(TestingSettings.TurnOffBanner), false, LogType.Ads);
            return;
        }
#endif
        if (!LoadCollapsible)
            return;

        if (!Kernel.IsInternetConnection())
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: RequestCollapsibleBanner failed no Internet");
            HandleCollapsibleBannerAdFailedToLoad(null);
            return;
        }

        if (!ConsentReady)
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: Consent is not set");
            HandleCollapsibleBannerAdFailedToLoad(null);
            return;
        }

        if (AdsManager.NoAds)
        {
            UIDebugLog.Log0("CollapsibleBanner not show " + nameof(AdsManager.NoAds), false, LogType.Ads);
            return;
        }

        if (AdsManager.NotShowBanner)
        {
            UIDebugLog.Log0("CollapsibleBanner not show" + nameof(AdsManager.NotShowBanner), false, LogType.Ads);
            return;
        }

        if (!showBanner)
        {
            _collapsibleBannerState = AdsState.NotStart;
            return;
        }

        if (_collapsibleBannerState == AdsState.Requesting)
            return;

        if (Initialized)
        {
            if (logFail) UIDebugLog.Log0("AdsInstanceAdmob: RequestCollapsibleBanner" + GetLast(CollapsibleBannerId), false, LogType.Ads);

            AdSize collapsibleSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

            _collapsibleBannerView?.Destroy();
            _collapsibleBannerView = null;
            _collapsibleBannerView = new BannerView(CollapsibleBannerId, collapsibleSize, AdPosition.Bottom);
            _collapsibleBannerView.Hide();

            _collapsibleBannerView.OnBannerAdLoaded += HandleCollapsibleBannerAdLoaded;
            _collapsibleBannerView.OnBannerAdLoadFailed += HandleCollapsibleBannerAdFailedToLoad;
            _collapsibleBannerView.OnAdFullScreenContentOpened += HandleCollapsibleBannerAdOpened;
            _collapsibleBannerView.OnAdFullScreenContentClosed += HandleCollapsibleBannerAdClosed;
            _collapsibleBannerView.OnAdClicked += HandleOnCollapsibleBannerAdClick;
            _collapsibleBannerView.OnAdPaid += adValue =>
            {
                HandleAdPaidEvent(adValue, AdTypeLog.collapsible_banner, collapsibleBannerAdapter, AdsPlatform.googleadmob);

                ResponseInfo responseInfo = _collapsibleBannerView.GetResponseInfo();
                string adRespondId = responseInfo.GetResponseId();
                string adSource = responseInfo.GetMediationAdapterClassName();
                var fireBase2 = Kernel.Resolve<FireBaseController>();
                var parameters2 = fireBase2.GetAdmobParameter(10, new[]
                {
                    new LogParameter("ad_source", adSource),
                    new LogParameter("ad_value", adValue.Value),
                    new LogParameter("ad_response_id", adRespondId),
                }, AdTypeLog.collapsible_banner).ToArray();
                fireBase2.LogEvent("admob_ad_open_success", parameters2);
            };

            // Create an empty ad request.
#if admob_9x_or_newer
            AdRequest request = new AdRequest();
#else
            AdRequest request = new AdRequest.Builder().Build();
#endif

            // Load the banner with the request.
            request.Extras.Add("collapsible", "bottom");
            _collapsibleBannerView.LoadAd(request);
            _collapsibleBannerState = AdsState.Requesting;

            var fireBase = Kernel.Resolve<FireBaseController>();
            var parameters = fireBase.GetAdmobParameter(3, new[]
            {
                new LogParameter(ParameterEnum.ad_id,GetLast(CollapsibleBannerId)),
            }, AdTypeLog.collapsible_banner).ToArray();
            fireBase.LogEvent(AdTypeLogStep.admob_ad_request.ToString(), parameters);
        }
    }

    public override bool IsCollapsibleBannerReady()
    {
        return _collapsibleBannerState == AdsState.Loaded || _collapsibleBannerState == AdsState.Hidden;
    }

    public override void HideCollapsibleBanner()
    {
        if (_collapsibleBannerState == AdsState.Loaded)
        {
            _collapsibleBannerView?.Hide();
            _collapsibleBannerState = AdsState.Hidden;

            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                DestroyCollapsibleBanner();
                StartCoroutine(IeRequestCollapsibleBanner(.5f));
                StartCoroutine(InvokeCollapsibleBanner(false));
                if (logFail) UIDebugLog.Log0(nameof(HandleCollapsibleBannerAdClosed) + " event received", false, LogType.Ads);
            });
        }
    }

    public override void ShowCollapsibleBanner()
    {
        if (_collapsibleBannerState == AdsState.Hidden)
        {
            OnBannerShow.Action.Invoke(true);
            _collapsibleBannerView?.Show();
            _collapsibleBannerState = AdsState.Loaded;
        }
    }

    public void DestroyCollapsibleBanner()
    {
        _collapsibleBannerState = AdsState.NotStart;
        OnBannerShow.Action.Invoke(false);
        if (_collapsibleBannerView == null)
            return;
        _collapsibleBannerView.Destroy();
        _collapsibleBannerView = null;
    }

    private void HandleCollapsibleBannerAdLoaded()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _collapsibleBannerState = AdsState.Loaded;
            UIDebugLog.Log0("on CollapsibleBanner Load:" + GetLast(CollapsibleBannerId) + " |Is collapsible: " + _collapsibleBannerView.IsCollapsible());
            collapsibleBannerAdapter = _collapsibleBannerView.GetResponseInfo().GetMediationAdapterClassName();
            _collapsibleBannerView.Hide();
            _collapsibleBannerState = AdsState.Hidden;

            try
            {
                ResponseInfo responseInfo = _collapsibleBannerView.GetResponseInfo();
                string adRespondId = responseInfo.GetResponseId();
                string adSource = responseInfo.GetMediationAdapterClassName();
                var fireBase = Kernel.Resolve<FireBaseController>();
                var parameters = fireBase.GetAdmobParameter(4, new[]
                {
                    new LogParameter("ad_source", adSource),
                    new LogParameter("ad_response_id", adRespondId),
                }, AdTypeLog.collapsible_banner).ToArray();
                fireBase.LogEvent("admob_ad_load_success", parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
    }

    #region CollapsibleBanner callback handlers
    int _retryCollapsibleBannerAttempt;

    private void HandleCollapsibleBannerAdFailedToLoad(AdError adError)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            _collapsibleBannerState = AdsState.Failed;
            StartCoroutine(InvokeCollapsibleBanner(false));

            _retryCollapsibleBannerAttempt++;
            double retryDelay = Math.Pow(2, Mathf.Clamp(_retryCollapsibleBannerAttempt, 3, 7));
            _collapsibleBannerView?.Destroy();
            _collapsibleBannerView = null;

            StartCoroutine(IeRequestCollapsibleBanner((float)retryDelay));
            if (logFail)
            {
#if UNITY_ANDROID || (UNITY_IOS && LOG_FAIL_IOS) // ios get mess crash
                var mess = adError == null ? "adError=null" : adError.GetMessage();
                UIDebugLog.Log0(
                    $"{nameof(HandleCollapsibleBannerAdFailedToLoad)} ({GetLast(CollapsibleBannerId)})  event received with message: {mess}",
                    false, LogType.Ads);
#endif
                UIDebugLog.Log0(
                    $"Reload CollapsibleBanner in {(float)retryDelay} seconds",
                    false, LogType.Ads);
            }

#if UNITY_ANDROID
            if (adError != null)
            {
                int errorCode = adError.GetCode();
                var fireBase = Kernel.Resolve<FireBaseController>();
                var parameters = fireBase.GetAdmobParameter(5, new[]
                {
                    new LogParameter("error_code", errorCode),
                }, AdTypeLog.collapsible_banner).ToArray();
                fireBase.LogEvent("admob_ad_load_fail", parameters);
            }
#else
            var fireBase = Kernel.Resolve<FireBaseController>();
            var parameters = fireBase.GetAdmobParameter(5, new[]
            {
                new LogParameter("error_code", "911"),
            }, AdTypeLog.collapsible_banner).ToArray();
            fireBase.LogEvent("admob_ad_load_fail", parameters);
#endif
        });
    }

    private IEnumerator InvokeCollapsibleBanner(bool value)
    {
        yield return new WaitForSeconds(0.1f);
    }

    private void HandleCollapsibleBannerAdOpened()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            StartCoroutine(InvokeCollapsibleBanner(false));
            if (logFail) UIDebugLog.Log0(nameof(HandleCollapsibleBannerAdOpened) + " event received", false, LogType.Ads);
        });
    }

    private void HandleCollapsibleBannerAdClosed()
    {
        ResponseInfo responseInfo = _collapsibleBannerView.GetResponseInfo();
        string adSource = responseInfo.GetMediationAdapterClassName();
        var fireBase2 = Kernel.Resolve<FireBaseController>();

        var parameters2 = fireBase2.GetAdmobParameter(12, new[]
        {
            new LogParameter("ad_response_id", responseInfo.GetResponseId()),
            new LogParameter("ad_source", adSource),
        }, AdTypeLog.collapsible_banner).ToArray();
        fireBase2.LogEvent("admob_ad_close", parameters2);


        //MobileAdsEventExecutor.ExecuteInUpdate(() =>
        //{
        //    DestroyCollapsibleBanner();
        //    StartCoroutine(IeRequestCollapsibleBanner(.5f));
        //    StartCoroutine(InvokeCollapsibleBanner(false));
        //    if (logFail) UIDebugLog.Log0(nameof(HandleCollapsibleBannerAdClosed) + " event received", false, LogType.Ads);
        //    StartCoroutine(waitToCollapsibleBannerFalse());
        //});
    }
    #endregion
}
#endif