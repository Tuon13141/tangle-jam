using System;
using UnityEngine;
using Percas.UI;
using Percas.IAR;
using Percas.Data;
using Sonat;

namespace Percas.IAA
{
    public interface IIAAHandler
    {
        void Initialize();

        void LoadBannerAd();
        void ShowBannerAd();
        void HideBannerAd();
        void DestroyBannerAd();

        void LoadInterstitialAd();
        void ShowInterstitialAd(Action onAdClosed, string adPlacement);

        void LoadAppOpenAd();
        void ShowAppOpenAd();

        void LoadRewardedAd();
        void ShowRewardedAd(Action onAdRewarded, Action onAdClosed, string adPlacement);
    }

    public class IAAManager : MonoBehaviour
    {
        [SerializeField] AppLovinManager appLovinManager;
        [SerializeField] AdMobManager adMobManager;

        public static IAAData IAAData = new();

        private static IIAAHandler AdManager;
        private static AdMobManager AdMobManager;

        private Coroutine coroutineShowAppOpen;

        public static Action OnSave;
        public static Action OnInterstitialAdClosed;
        public static Func<bool> OnGetJustClosedInter;

        public static bool IsAdMobReady { get; set; }
        public static bool CanShowAppOpen { get; set; }
        public static bool LoadDataDone { get; private set; }

        public static bool IsAppOpenReady => AdMobManager.IsAppOpenReady;

        private void Awake()
        {
            InitData();

            OnSave += SaveData;
            OnInterstitialAdClosed += InterstitialAdClosed;
            ActionEvent.OnSetLevelPhase += ShowInterBetweenLevelPhase;
        }

        private void OnDestroy()
        {
            OnSave -= SaveData;
            OnInterstitialAdClosed -= InterstitialAdClosed;
            ActionEvent.OnSetLevelPhase -= ShowInterBetweenLevelPhase;

            if (coroutineShowAppOpen != null)
            {
                StopCoroutine(coroutineShowAppOpen);
                coroutineShowAppOpen = null;
            }
        }

#if use_max
        private void OnEnable()
        {
            InitializeAdManager();
        }
#endif

        private void ShowInterBetweenLevelPhase(int phase, LevelAsset levelData)
        {
            if (!GameLogic.CanShowInterBetweenLevelPhase) return;
            if (phase <= 1) return;
            bool interShown = false;
            if (GameLogic.CanShowAdBreak && GameLogic.ConditionShowAdBreak && Kernel.Resolve<AdsManager>().IsInterstitialAdsReady())
            {
                CanShowAppOpen = false;
                AdLoading.OnShow?.Invoke(() =>
                {
                    var log = new SonatLogShowInterstitial()
                    {
                        location = "ingame",
                        screen = AdPlacement.ChangeLevelPhase.ToString(),
                        placement = AdPlacement.ChangeLevelPhase.ToString(),
                        level = GameLogic.CurrentLevel,
                        mode = PlayMode.classic.ToString()
                    };
                    log.Post(logAf: true);
                    interShown = Kernel.Resolve<AdsManager>().ShowInterstitial(log, false, false, () =>
                    {
                        UpdateLastTimeAdBreakShown();
                        UpdateLastTimeAppOpenShown();
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin(5, Vector3.zero, new LogCurrency("currency", "coin", "ad_break", "non_iap", "ads", "inter_ads")));
                        RewardGainController.OnStartGaining?.Invoke();
                        OnInterstitialAdClosed?.Invoke();
                        CanShowAppOpen = true;
                    });
                }, true);
            }
            if (interShown) return;
        }

        private void InitializeAdManager()
        {
            AdMobManager = adMobManager;
            AdMobManager.InitSDK();
        }

        private void InitData()
        {
            if (!PlayerPrefs.HasKey(Const.KEY_IAA_DATA))
            {
                SaveData();
            }
            else
            {
                LoadData();
            }
            LoadDataDone = true;
        }

        private void LoadData()
        {
            try
            {
                string encryptedJson = PlayerPrefs.GetString(Const.KEY_IAA_DATA);
                string jsonData = Helpers.Decrypt(encryptedJson);
                IAAData data = JsonUtility.FromJson<IAAData>(jsonData);
                IAAData = data;
            }
            catch (Exception)
            {
                IAAData = new();
            }
        }

        private void SaveData()
        {
            string jsonData = JsonUtility.ToJson(IAAData);
            string encryptedJson = Helpers.Encrypt(jsonData);
            PlayerPrefs.SetString(Const.KEY_IAA_DATA, encryptedJson);
            DataManager.Instance.WriteToLocal(Const.KEY_IAA_DATA, jsonData);
        }

        #region Configs
        public static bool IsAdRemoved => IAAData.IsAdRemoved && !GameConfig.Instance.IsMarketingVersion;
        public static bool PassBannerInitialLevel => GameLogic.CurrentLevel >= IAAData.BannerInitialLevel;
        public static bool PassAdInitialLevel => GameLogic.CurrentLevel >= GameLogic.LevelShowInter;

        public static bool PassAdFrequency()
        {
            try
            {
                TimeSpan remainTime = DateTime.UtcNow - TimeHelper.ParseIsoString(IAAData.LastTimeAdShown);
                Debug.LogError($"PassAdFrequency.remainTime: {remainTime.TotalSeconds} - Required: {GameLogic.TimeShowInter}/Last Time = {IAAData.LastTimeAdShown}/Level Show = {GameConfig.ConfigData.LevelShowInter}");
                return remainTime.TotalSeconds >= GameLogic.TimeShowInter;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static bool PassAdBreakFrequency()
        {
            try
            {
                TimeSpan remainTime = DateTime.UtcNow - TimeHelper.ParseIsoString(IAAData.LastTimeAdBreakShown);
                Debug.LogError($"PassAdBreakFrequency.remainTime: {remainTime.TotalSeconds} - Required: {GameLogic.TimeShowAdBreak}/Last Time = {IAAData.LastTimeAdBreakShown}/Level Show = {GameConfig.ConfigData.LevelShowAdBreak}");
                return remainTime.TotalSeconds >= GameLogic.TimeShowAdBreak;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static bool PassAppOpenFrequency()
        {
            try
            {
                TimeSpan remainTime = DateTime.UtcNow - TimeHelper.ParseIsoString(IAAData.LastTimeAppOpenShown);
                Debug.LogError($"PassAppOpenFrequency.remainTime: {remainTime.TotalSeconds} - Required: {GameLogic.TimeShowAppOpen}/Last Time = {IAAData.LastTimeAppOpenShown}/Level Show = {GameConfig.ConfigData.LevelShowInter}");
                return remainTime.TotalSeconds >= GameLogic.TimeShowAppOpen;
            }
            catch (Exception)
            {
                return true;
            }
        }
        #endregion

        #region Ads Controller
        // --- Banner Ad Methods ---
        public static void LoadBannerAd()
        {
            //if (IsAdRemoved) return;
            //AdManager.LoadBannerAd();
        }

        public static void ShowBannerAd()
        {
            //if (IsAdRemoved) return;
            //if (!PassBannerInitialLevel) return;
            //AdManager.ShowBannerAd();

            if (!GameLogic.IsNoAds)
            {
                Kernel.Resolve<AdsManager>().ShowBanner();
            }
        }

        public static void CheckShowBanner()
        {
            Kernel.Resolve<AdsManager>().CheckShowBanner();
        }

        public static void HideBannerAd()
        {
            //AdManager.HideBannerAd();
            Kernel.Resolve<AdsManager>().HideBanner();
        }

        public static void DestroyBannerAd()
        {
            AdManager.DestroyBannerAd();
        }

        // --- Interstitial Ad Methods ---
        public static void LoadInterstitialAd()
        {
            if (IsAdRemoved) return;
            AdManager.LoadInterstitialAd();
        }

        public static void ShowInterstitialAd(Action onAdClosed, string adPlacement)
        {
            if (IsAdRemoved)
            {
                onAdClosed?.Invoke();
                return;
            }
            if (!PassAdInitialLevel || !PassAdFrequency())
            {
                onAdClosed?.Invoke();
                return;
            }
            AdManager.ShowInterstitialAd(onAdClosed, adPlacement);
        }

        // --- App Open Ad Methods ---
        public static void LoadAppOpenAd()
        {
            if (IsAdRemoved) return;
            AdManager.LoadAppOpenAd();
        }

        public static void ShowAppOpenAd(Action onCompleted, Action onFailed)
        {
#if use_max
            if (GameLogic.CanShowAppOpen && AdMobManager.IsAppOpenReady)
            {
                AdMobManager.ShowAppOpenAd();
                UpdateLastTimeAppOpenShown();
                onCompleted?.Invoke();
            }
            else
            {
                onFailed?.Invoke();
            }
#endif

#if use_admob
            if (GameLogic.CanShowAppOpen && Kernel.Resolve<AdsManager>().IsAppOpenAdsReady())
            {
                Kernel.Resolve<AdsManager>().ShowAppOpenAds();
                UpdateLastTimeAppOpenShown();
                onCompleted?.Invoke();
            }
            else
            {
                onFailed?.Invoke();
            }
#endif
        }

        // --- Rewarded Ad Methods ---
        public static void LoadRewardedAd()
        {
            AdManager.LoadRewardedAd();
        }

        public static void ShowRewardedAd(Action onAdRewarded, Action onAdClosed, string adPlacement)
        {
            if (!GameConfig.Instance.IsReachableInternet())
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_NO_INTERNET);
                return;
            }
            AdManager.ShowRewardedAd(onAdRewarded, onAdClosed, adPlacement);
        }
        #endregion

        public static void RemoveAds()
        {
            IAAData.IsAdRemoved = true;
            Kernel.Resolve<AdsManager>().EnableNoAds();
            Kernel.Resolve<AdsManager>().DestroyBanner();
            UIBannerArea.OnShow?.Invoke();
            UIBottomMenus.OnShow?.Invoke(true);
            ButtonRemoveAds.OnShow?.Invoke();
            OnSave?.Invoke();
        }

        public static void UpdateLastTimeAdShown()
        {
            IAAData.LastTimeAdShown = TimeHelper.ToIsoString(DateTime.UtcNow);
            OnSave?.Invoke();
        }

        public static void UpdateLastTimeAdBreakShown()
        {
            IAAData.LastTimeAdBreakShown = TimeHelper.ToIsoString(DateTime.UtcNow);
            OnSave?.Invoke();
        }

        public static void UpdateLastTimeAppOpenShown()
        {
            IAAData.LastTimeAppOpenShown = TimeHelper.ToIsoString(DateTime.UtcNow);
            OnSave?.Invoke();
        }

        public static void UpdateInterCount()
        {
            IAAData.InterCount += 1;
            OnSave?.Invoke();
        }

        public static void UpdateVideoCount()
        {
            IAAData.VideoCount += 1;
            OnSave?.Invoke();
        }

        public static void UpdateUserAdRevenue(double valueToAdd)
        {
            IAAData.UserAdRevenue += valueToAdd;
            OnSave?.Invoke();
        }

        private bool justClosedInterstitial = false;
        private void InterstitialAdClosed()
        {
            justClosedInterstitial = true;
            // Cancel any scheduled App Open ad invocation.
            CancelInvoke(nameof(ShowAppOpenAdDelayed));
            // Reset the flag after a short delay.
            Invoke(nameof(ResetInterstitialFlag), 0.5f); // Adjust the delay as needed.
        }

        private void ResetInterstitialFlag()
        {
            justClosedInterstitial = false;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!CanShowAppOpen) return;
            if (hasFocus)
            {
                //Debug.LogError($"hasFocus");
                // Schedule the App Open ad to show after a brief delay.
                Invoke(nameof(ShowAppOpenAdDelayed), 0.2f);
            }
            else
            {
                //Debug.LogError($"NOT hasFocus");
                // Cancel if focus is lost.
                CancelInvoke(nameof(ShowAppOpenAdDelayed));
            }
        }

        private void ShowAppOpenAdDelayed()
        {
            // Only show the App Open ad if an interstitial wasn't just closed.
            if (!justClosedInterstitial)
            {
                ShowAppOpenAd(null, null);
            }
        }
    }
}
