using Percas.Data;
using Percas.IAP;
using System;

namespace Tuon
{
    public static class TrackingManager
    {
        public static Action<LogCurrency> OnSpendBooster;
        public static Action<LogCurrency, int> OnEarnBooster;
        public static Action<string, int> OnTutorialBegin;
        public static Action<string, int> OnTutorialComplete;

        public static Action<string> OnSetUserType;
        public static Action<string> OnTrackScreenView;

        #region Gameplay
        public static Action OnLevelStart;
        public static Action<bool, string> OnLevelEnd;
        public static Action OnPhaseStart;
        public static Action<string> OnContinuePlaying;
        #endregion

        #region IAA
        public static Action OnInterstitialAdLoaded;
        public static Action<string> OnInterstitialAdDisplayed;
        public static Action OnAppOpenAdDisplayed;
        public static Action OnRewardedAdLoaded;
        public static Action<string> OnRewardedAdDisplayed;
        public static Action<string> OnRewardedAdReceivedReward;
        public static Action<MaxSdkBase.AdInfo> OnAdRevenuePaidEvent;
        public static Action<double, string, string, string> OnAdMobRevenueEvent;
        #endregion

        #region IAP
        public static Action<IAPPack> OnPurchaseCompleted;
        public static Action OnPurchaseCoinPack1;
        public static Action OnPurchaseCoinPack2;
        public static Action OnPurchaseCoinPack3;
        public static Action OnPurchaseCoinPack4;
        public static Action OnPurchaseCoinPack5;
        public static Action OnPurchaseCoinPack6;
        public static Action OnPurchaseRemoveAds;
        public static Action OnPurchaseStarterPack;
        public static Action OnPurchaseSmallPack;
        public static Action OnPurchaseMediumPack;
        public static Action OnPurchaseLargePack;
        public static Action OnPurchasePiggyBank;
        public static Action OnPurchaseHeartOffer;
        #endregion

        #region Retention
        public static Action<string> OnDailyRewardCompleted;
        public static Action OnWeeklyQuestCompleted1;
        public static Action OnWeeklyQuestCompleted2;
        public static Action OnWeeklyQuestCompleted3;
        public static Action OnWeeklyQuestCompleted4;
        public static Action OnWeeklyQuestCompleted5;
        public static Action OnWeeklyQuestCompleted6;
        public static Action<string> OnWeeklyQuestRewardReceived;
        public static Action<string> OnLuckyWheelSpin;
        public static Action<string> OnPushNotificationCount;
        public static Action<string> OnPushNotificationClicked;
        #endregion
    }
}

