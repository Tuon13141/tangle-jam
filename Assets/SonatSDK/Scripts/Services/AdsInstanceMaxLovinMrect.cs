#define dummy
using System.Collections;
using Sonat;
using UnityEngine;
using LogType = sonat_sdk.Scripts.LogType;

public partial class AdsInstanceMaxLovin
{
#if !((dummy || global_dummy) && !use_max)

    public float mrecPosX;
    public float mrecPosY;
    [SerializeField]
    private MaxSdkBase.AdViewPosition mrecPos = MaxSdkBase.AdViewPosition.BottomCenter;

    private void RequestMrecBanner(bool register = false)
    {
        if (!LoadMrec || string.IsNullOrEmpty(MRECBannerId))
            return;

        UIDebugLog.Log(nameof(RequestMrecBanner));
        if (_initialized && !AdsManager.NoAds)
        {
            //MaxSdk.CreateMRec(MRECBannerId, MaxSdkBase.AdViewPosition.BottomCenter);
            if (register)
            {
                MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMrecBannerAdLoadedEvent;
                MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMrecBannerAdLoadFailedEvent;
                MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMrecBannerAdClickedEvent;
                MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMrecBannerAdRevenuePaidEvent;
            }

            if (mrecPos == MaxSdkBase.AdViewPosition.BottomCenter && mrecPosY != 0)
            {
                //MRECs are sized to 300x250 on phones and tablets
                float density = MaxSdkUtils.GetScreenDensity();
                Vector2 bannerSize = new Vector2(300.0f, 250.0f);

                float dpWidth = Screen.safeArea.width / density;
                float dpheight = Screen.safeArea.height / density;

                //Banners are automatically sized to 320×50 on phones and 728×90 on tablets
                float offsetHeight = (mrecPosY * (Screen.height / 1920.0f)) + (MaxSdkUtils.IsTablet() ? 90.0f : 50.0f);

                MaxSdk.CreateMRec(MRECBannerId, (dpWidth - bannerSize.x) / 2, (dpheight - bannerSize.y - offsetHeight));
            }
            else if (mrecPosX == 0 && mrecPosY == 0)
            {
                MaxSdk.CreateMRec(MRECBannerId, mrecPos);
            }
            else
            {
                MaxSdk.CreateMRec(MRECBannerId, mrecPosX, mrecPosY);
            }

            _mrecBannerState = AdsState.Requesting;
        }
    }

    private void OnMrecBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log(nameof(OnMrecBannerAdClickedEvent));
    }

    private void OnMrecBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo info)
    {
        Debug.Log(nameof(OnMrecBannerAdLoadedEvent));

        if (_nativeBannerState == AdsState.Hidden)
        {
            HideMrecBanner();
            return;
        }

        _mrecBannerState = AdsState.Loaded;
        mrecAdapter = info.NetworkName; // _bannerView.GetResponseInfo().GetMediationAdapterClassName();
        //OnBannerShow.Action.Invoke(true);
    }

    private void OnMrecBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.Log(nameof(OnMrecBannerAdLoadFailedEvent));
        Debug.Log($"MrecFailed : {errorInfo.Message}");
    }

    private void OnMrecBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        SonatAnalyticTracker.LogRevenue(AdsPlatform.applovinmax, mrecAdapter, adInfo.Revenue, adInfo.RevenuePrecision, AdTypeLog.mrec,
            Kernel.Resolve<FireBaseController>().FirebaseInstanceId,
            "mrec_banner");
    }

    private IEnumerator IeRequestMrecBanner(float time, bool init = false)
    {
        UIDebugLog.Log("IeRequestMrecBanner", false, LogType.Ads);
        yield return new WaitForSeconds(time);
        RequestMrecBanner(init); ;
    }

    public override bool IsMrecBannerReady()
    {
#if UNITY_EDITOR
        return true;
#endif
        return _mrecBannerState == AdsState.Loaded || _mrecBannerState == AdsState.Hidden;
    }

    public override void HideMrecBanner()
    {
        //UIDebugLog.Log($"{nameof(HideMrecBanner)} {_mrecBannerState}");
        //if (_mrecBannerState == AdsState.Showing)
        //{
        MaxSdk.HideMRec(MRECBannerId);
        _mrecBannerState = AdsState.Hidden;
        //}
    }

    public override void ShowMrecBanner()
    {
#if UNITY_EDITOR
        MaxSdk.ShowMRec(MRECBannerId);
        _mrecBannerState = AdsState.Loaded;

        return;
#endif

        UIDebugLog.Log($"{nameof(ShowMrecBanner)} {_mrecBannerState}");
        if (_mrecBannerState == AdsState.Hidden || _mrecBannerState == AdsState.Loaded)
        {
            MaxSdk.ShowMRec(MRECBannerId);
            _mrecBannerState = AdsState.Showing;
        }
    }

    public void DestroyMrecBanner()
    {
        _mrecBannerState = AdsState.NotStart;
        OnBannerShow.Action.Invoke(false);
        if (!string.IsNullOrEmpty(MRECBannerId))
            MaxSdk.DestroyMRec(MRECBannerId);
    }
#endif
}