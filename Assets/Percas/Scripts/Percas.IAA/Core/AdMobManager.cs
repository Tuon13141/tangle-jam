using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using Sdk.Google.NativeAds;
using Percas.Data;
using Percas.IAA;

namespace Percas
{
    public class AdMobManager : MonoBehaviour
    {
        [Header("Android")]
        [SerializeField] string m_androidBannerID;
        [SerializeField] string m_androidInterID;
        [SerializeField] string m_androidVideoID;
        [SerializeField] string m_androidAppOpenID;

        [Header("iOS")]
        [SerializeField] string m_iosBannerID;
        [SerializeField] string m_iosInterID;
        [SerializeField] string m_iosVideoID;
        [SerializeField] string m_iosAppOpenID;

        private string bannerID;
        private string interID;
        private string videoID;
        private string appOpenID;

        private AppOpenAd appOpenAd;

        private void Awake()
        {
#if UNITY_ANDROID
            bannerID = m_androidBannerID;
            interID = m_androidInterID;
            videoID = m_androidVideoID;
            appOpenID = m_androidAppOpenID;
#elif UNITY_IOS
            bannerID = m_iosBannerID;
            interID = m_iosInterID;
            videoID = m_iosVideoID;
            appOpenID = m_iosAppOpenID;
#else
            bannerID = "unexpected_platform";
            interID = "unexpected_platform";
            videoID = "unexpected_platform";
            appOpenID = "unexpected_platform";
#endif
        }

        public void InitSDK()
        {
#if use_max
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                Debug.LogError("This callback is called once the MobileAds SDK is initialized.");
                LoadAppOpenAd();
                IAAManager.IsAdMobReady = true;
                try
                {
                    NativeAdsManager.Instance.LoadNative();
                }
                catch (Exception) { }
            });
#endif
        }

        #region App Open
        /// <summary>
        /// Loads the app open ad.
        /// </summary>
        private void LoadAppOpenAd()
        {
            // Clean up the old ad before loading a new one.
            if (appOpenAd != null)
            {
                DestroyAppOpen();
                appOpenAd = null;
            }

            Debug.Log("Loading the app open ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            AppOpenAd.Load(appOpenID, adRequest,
                (AppOpenAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("app open ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("App open ad loaded with response : "
                              + ad.GetResponseInfo());

                    appOpenAd = ad;
                    RegisterAppOpenEventHandlers(appOpenAd);
                });
        }

        /// <summary>
        /// Shows the app open ad.
        /// </summary>
        public void ShowAppOpenAd()
        {
            if (appOpenAd is null)
            {
                LoadAppOpenAd();
            }
            else
            {
                if (IsAppOpenReady)
                {
                    Debug.Log("Showing app open ad.");
                    appOpenAd.Show();
                }
                else
                {
                    Debug.LogError("App open ad is not ready yet.");
                    LoadAppOpenAd();
                }
            }
        }

        private void RegisterAppOpenEventHandlers(AppOpenAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                //Debug.Log(string.Format("App open ad paid {0} {1}.",
                //    adValue.Value,
                //    adValue.CurrencyCode));

                //Tracking.OnSendAppOpenRevenueToAppsFlyer?.Invoke(ad, adValue);
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                // Debug.Log("App open ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("App open ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                //  Debug.Log("App open ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                //  Debug.Log("App open ad full screen content closed.");
                //PlayerDataManager.Data.UpdateLastTimeAppOpenShown();
                LoadAppOpenAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("App open ad failed to open full screen content " +
                               "with error : " + error);
                LoadAppOpenAd();
            };
        }

        private void DestroyAppOpen()
        {
            appOpenAd.Destroy();
        }

        public bool IsAppOpenReady
        {
            get
            {
                return appOpenAd != null && appOpenAd.CanShowAd();
            }
        }
        #endregion
    }
}
