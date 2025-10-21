using System;
using UnityEngine;

namespace Percas.IAA
{
    public class AppLovinManager : MonoBehaviour, IIAAHandler
    {
        [Header("Android Ad Unit ID")]
        [SerializeField] string android_banner;
        [SerializeField] string android_inter;
        [SerializeField] string android_appOpen;
        [SerializeField] string android_reward;
        [Header("iOS Ad Unit ID")]
        [SerializeField] string ios_banner;
        [SerializeField] string ios_inter;
        [SerializeField] string ios_appOpen;
        [SerializeField] string ios_reward;

        private string bannerAdUnitId;
        private string interstitialAdUnitId;
        private string appOpenAdUnitId;
        private string rewardedAdUnitId;

        private int interstitialRetryAttempt;
        private int appOpenRetryAttempt;
        private int rewardedRetryAttempt;

        private Action _onAdClosed;
        private Action _onAdRewarded;

        private bool _isRewarded;

        //private void Awake()
        //{
        //    InitializeAdUnits();
        //    InitializeCallbacks();
        //}

        private void InitializeAdUnits()
        {
#if UNITY_IOS
            bannerAdUnitId = ios_banner;
            interstitialAdUnitId = ios_inter;
            appOpenAdUnitId = ios_appOpen;
            rewardedAdUnitId = ios_reward;
#else
            bannerAdUnitId = android_banner;
            interstitialAdUnitId = android_inter;
            appOpenAdUnitId = android_appOpen;
            rewardedAdUnitId = android_reward;
#endif
        }

        private void InitializeCallbacks()
        {
            // Banner Ad callbacks
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoaded;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailed;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClicked;

            // Interstitial Ad callbacks
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialAdLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialAdLoadFailed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialAdDisplayed;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialAdClicked;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialAdHidden;

            // App Open Ad callbacks
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppOpenAdLoaded;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAppOpenAdLoadFailed;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAppOpenAdDisplayed;
            MaxSdkCallbacks.AppOpen.OnAdClickedEvent += OnAppOpenAdClicked;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenAdHidden;

            // Rewarded Ad callbacks
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayed;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClicked;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHidden;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedReward;

            // Attach callbacks based on the ad format(s) you are using
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        }

        private void OnDestroy()
        {
            // Banner Ad callbacks
            MaxSdkCallbacks.Banner.OnAdLoadedEvent -= OnBannerAdLoaded;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent -= OnBannerAdLoadFailed;
            MaxSdkCallbacks.Banner.OnAdClickedEvent -= OnBannerAdClicked;

            // Interstitial Ad callbacks
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnInterstitialAdLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnInterstitialAdLoadFailed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent -= OnInterstitialAdDisplayed;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent -= OnInterstitialAdClicked;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnInterstitialAdHidden;

            // App Open Ad callbacks
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent -= OnAppOpenAdLoaded;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent -= OnAppOpenAdLoadFailed;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent -= OnAppOpenAdDisplayed;
            MaxSdkCallbacks.AppOpen.OnAdClickedEvent -= OnAppOpenAdClicked;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent -= OnAppOpenAdHidden;

            // Rewarded Ad callbacks
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRewardedAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedAdLoadFailed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnRewardedAdDisplayed;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent -= OnRewardedAdClicked;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedAdHidden;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedAdReceivedReward;

            // Attach callbacks based on the ad format(s) you are using
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent;
        }

        // --- Banner Ad Callbacks ---
        private void OnBannerAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Banner Ad Loaded.");
        }

        private void OnBannerAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.LogError("Banner Ad Load Failed: " + errorInfo.Message);
        }

        private void OnBannerAdClicked(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Banner Ad Clicked.");
        }

        // --- Interstitial Ad Callbacks ---
        private void OnInterstitialAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Interstitial Ad Loaded.");
            TrackingManager.OnInterstitialAdLoaded?.Invoke();
        }

        private void OnInterstitialAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.LogError("Interstitial Ad Load Failed: " + errorInfo.Message);
            interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
            Invoke(nameof(LoadInterstitialAd), (float)retryDelay);
        }

        private void OnInterstitialAdDisplayed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Interstitial Ad Displayed.");
            AudioController.Instance.Mute();
            IAAManager.UpdateLastTimeAdShown();
            IAAManager.UpdateLastTimeAppOpenShown();
            IAAManager.UpdateInterCount();
            TrackingManager.OnInterstitialAdDisplayed?.Invoke(adInfo.Placement);
        }

        private void OnInterstitialAdClicked(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Interstitial Ad Clicked.");
        }

        private void OnInterstitialAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Interstitial Ad Hidden.");
            AudioController.Instance.Unmute();
            IAAManager.UpdateLastTimeAppOpenShown();
            LoadInterstitialAd(); // Preload next ad
            _onAdClosed?.Invoke();
        }

        // --- App Open Ad Callbacks ---
        private void OnAppOpenAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("App Open Ad Loaded.");
        }

        private void OnAppOpenAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.LogError("App Open Ad Load Failed: " + errorInfo.Message);
            appOpenRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, appOpenRetryAttempt));
            Invoke(nameof(LoadAppOpenAd), (float)retryDelay);
        }

        private void OnAppOpenAdDisplayed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("App Open Ad Displayed.");
            AudioController.Instance.Mute();
            IAAManager.UpdateLastTimeAppOpenShown();
            TrackingManager.OnAppOpenAdDisplayed?.Invoke();
        }

        private void OnAppOpenAdClicked(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("App Open Ad Clicked.");
        }

        private void OnAppOpenAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("App Open Ad Hidden.");
            AudioController.Instance.Unmute();
            LoadAppOpenAd(); // Preload next ad
        }

        // --- Rewarded Ad Callbacks ---
        private void OnRewardedAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded Ad Loaded.");
            TrackingManager.OnRewardedAdLoaded?.Invoke();
        }

        private void OnRewardedAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.LogError("Rewarded Ad Load Failed: " + errorInfo.Message);
            rewardedRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));
            Invoke(nameof(LoadRewardedAd), (float)retryDelay);
        }

        private void OnRewardedAdDisplayed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded Ad Displayed.");
            AudioController.Instance.Mute();
            IAAManager.UpdateLastTimeAppOpenShown();
            TrackingManager.OnRewardedAdDisplayed?.Invoke(adInfo.Placement);
        }

        private void OnRewardedAdClicked(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded Ad Clicked.");
        }

        private void OnRewardedAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded Ad Hidden.");
            AudioController.Instance.Unmute();
            IAAManager.UpdateLastTimeAppOpenShown();
            LoadRewardedAd(); // Preload next ad
            if (_isRewarded) return;
            _onAdClosed?.Invoke();
        }

        private void OnRewardedAdReceivedReward(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Rewarded Ad Reward Received: {reward.Amount} {reward.Label}");
            // Grant reward here
            _isRewarded = true;
            _onAdRewarded?.Invoke();
            IAAManager.UpdateVideoCount();
            TrackingManager.OnRewardedAdReceivedReward?.Invoke(adInfo.Placement);
        }

        private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            TrackingManager.OnAdRevenuePaidEvent?.Invoke(adInfo);
        }

        // Initialize
        public void Initialize()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
                // AppLovin SDK is initialized, start loading ads
                Debug.LogError($"[Percas] AppLovin SDK is initialized!");
                //LoadAllAds();
            };
            MaxSdk.InitializeSdk();
        }

        private void LoadAllAds()
        {
            //IAAManager.LoadBannerAd();
            //IAAManager.LoadAppOpenAd();
            //IAAManager.LoadInterstitialAd();
            //IAAManager.LoadRewardedAd();
        }

        // --- Banner Ad Methods ---
        public void LoadBannerAd()
        {
            // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black);
        }

        public void ShowBannerAd()
        {
            MaxSdk.ShowBanner(bannerAdUnitId);
        }

        public void HideBannerAd()
        {
            MaxSdk.HideBanner(bannerAdUnitId);
        }

        public void DestroyBannerAd()
        {
            MaxSdk.DestroyBanner(bannerAdUnitId);
        }

        // --- Interstitial Ad Methods ---
        public void LoadInterstitialAd()
        {
            MaxSdk.LoadInterstitial(interstitialAdUnitId);
        }

        public void ShowInterstitialAd(Action onAdClosed, string adPlacement)
        {
            _onAdClosed = onAdClosed;
            if (MaxSdk.IsInterstitialReady(interstitialAdUnitId))
            {
                UI.AdLoading.OnShow?.Invoke(() =>
                {
                    MaxSdk.ShowInterstitial(interstitialAdUnitId, adPlacement);
                }, false);
            }
            else
            {
                LoadInterstitialAd();
                _onAdClosed?.Invoke();
            }
        }

        // --- App Open Ad Methods ---
        public void LoadAppOpenAd()
        {
            MaxSdk.LoadAppOpenAd(appOpenAdUnitId);
        }

        public void ShowAppOpenAd()
        {
            if (MaxSdk.IsAppOpenAdReady(appOpenAdUnitId))
            {
                MaxSdk.ShowAppOpenAd(appOpenAdUnitId);
            }
            else
            {
                LoadAppOpenAd();
            }
        }

        // --- Rewarded Ad Methods ---
        public void LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(rewardedAdUnitId);
        }

        public void ShowRewardedAd(Action onAdRewarded, Action onAdClosed, string adPlacement)
        {
            _isRewarded = false;
            _onAdRewarded = onAdRewarded;
            _onAdClosed = onAdClosed;
            if (MaxSdk.IsRewardedAdReady(rewardedAdUnitId))
            {
                MaxSdk.ShowRewardedAd(rewardedAdUnitId, adPlacement);
            }
            else
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_VIDEO_NOT_READY);
            }
        }
    }
}
