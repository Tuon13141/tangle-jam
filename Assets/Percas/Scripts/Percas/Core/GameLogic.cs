using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Percas.Data;
using Percas.Live;
using Percas.UI;
using Percas.IAA;

namespace Percas
{
    public static class GameLogic
    {
        public static PopupBase CurrentPopup { get; set; }

        public static bool IsShowingRoom { get; set; } = false;
        public static bool RewardsGaining { get; set; }
        public static bool AutoIntroPopupClosed { get; set; }
        public static bool AutoSalePopupClosed { get; set; }
        public static bool AutoEventPopupClosed { get; set; }

        public static int CoilEarned { get; set; } = 0;
        
        public static string CheatLevelData { get; set; }

        public static Dictionary<string, int> RemoteLevelConfig { get; set; } = new Dictionary<string, int>();
        public static Dictionary<string, string> RemoteLevelData { get; set; } = new Dictionary<string, string>();

        public static string InterGapTime => GameConfig.ConfigData.InterGapTime;

        // Game
        public static bool InternetReachability => Application.internetReachability != NetworkReachability.NotReachable;

        public static string LogLocation => IsInGame ? "ingame" : "home";

        public static bool IsClassicMode => GlobalSetting.PlayMode == PlayMode.classic;
        public static bool IsHiddenPictureMode => GlobalSetting.PlayMode == PlayMode.hidden_picture;

        public static bool IsActiveHiddenPictureEvent => DataManager.Instance.GetCurrentHiddenPictureData() != null && HiddenPictureManager.Data.HasActiveEvent();

        public static string NextDailyTimeReset => PlayerPrefs.GetString(Const.KEY_DAILY_TIME_RESET);
        public static bool IsNullUserType => string.IsNullOrEmpty(PlayerDataManager.PlayerData.UserTyper);
        public static int CurrentLevel => (int)PlayerDataManager.PlayerData.HighestLevel + 1;
        public static int WinStreak => PlayerDataManager.PlayerData.WinStreak;
        public static int LoseStreak => PlayerDataManager.PlayerData.LoseStreak;
        public static int CurrentLevelPhase => PlayerDataManager.LevelPhase;
        public static string ContinueWith => PlayerDataManager.PlayerData.ContinueWith;
        public static int ContinueTimes => PlayerDataManager.ContinueTimes;
        public static int LevelAttempts => PlayerDataManager.PlayerData.LevelAttempts;
        public static int LevelCoil => PlayerDataManager.LevelCoil;
        public static int LevelPin => PlayerDataManager.LevelPin;
        public static int LevelStar => PlayerDataManager.LevelStar;
        public static int LogicSlotAmount => GameConfig.ConfigData.LogicSlotAmount;
        public static int LevelUnlockLuckySpin => GameConfig.ConfigData.LevelUnlockLuckySpin;
        public static int LevelShowStarRush => GameConfig.ConfigData.LevelShowEventStarRush;
        public static int LevelUnlockStarRush => GameConfig.ConfigData.LevelUnlockEventStarRush;
        public static int LevelShowHiddenPictureIcon => GameConfig.ConfigData.LevelShowEventHiddenPictureIcon;
        public static int LevelUnlockHiddenPicture => GameConfig.ConfigData.LevelUnlockEventHiddenPicture;
        public static int LevelUseBoosterCount => PlayerDataManager.LevelUseBoosterCount;
        public static int LevelMoveCount => PlayerDataManager.LevelMoveCount;
        public static int TotalCoil => PlayerDataManager.PlayerData.Coil;
        public static int TotalPin => PlayerDataManager.PlayerData.Pin;
        public static int LuxuryBasketOpenedCount => PlayerDataManager.PlayerData.LuxuryBasketOpenedCount;
        public static int FreePictureLevelCoin => 10;
        public static float LevelPlayingTime => PlayerDataManager.LevelPlayingTime;
        public static float PhasePlayingTime => PlayerDataManager.PhasePlayingTime;
        public static bool IsStarterPackPurchased => PlayerDataManager.PlayerData.IsStarterPackPurchased;
        public static bool OutOfRoom => CollectionController.instance?.collectionFillDoneAll ?? false;
        public static bool ShowPhaseInterConfirm => GameConfig.ConfigData.ShowPhaseInterConfirm;
        public static bool UnlockStarRush => CurrentLevel >= LevelUnlockStarRush;

        public static bool AutoShowSalePopup => GameConfig.ConfigData.AutoShowSalePopup;
        public static bool ShowBuildingIntro => GameConfig.ConfigData.ShowBuildingIntro;

        public static bool IsInGame => SceneManager.GetActiveScene().name == Const.SCENE_GAME;
        public static bool IsInHome => SceneManager.GetActiveScene().name == Const.SCENE_HOME;

        public static int CurrentDailyRewardIndex => PlayerDataManager.PlayerData.DailyRewardIndex;

        public static void UpdateLevelLabel(bool isHardLevel) => UILevelLabel.OnUpdateLabel?.Invoke(isHardLevel);
        public static void UpdateButtonUI(bool isHardLevel) => UIGameManager.OnUpdateButtonUI?.Invoke(isHardLevel);
        public static void UpdateLevelImage(Sprite sprite) => UILevelImage.OnUpdateImage?.Invoke(sprite);

        public static string GetLocalization(string key, params object[] args) => LocalizationManager.Instance.GetFormattedValue(key, args);

        // IAA
        public static bool IsNoAds => IAAManager.IAAData.IsAdRemoved || Kernel.Resolve<AdsManager>().IsNoAds();
        public static bool CanShowAdBreak => !IsNoAds && CurrentLevel >= GameConfig.ConfigData.LevelShowAdBreak && IAAManager.PassAdBreakFrequency();
        public static bool ConditionShowAdBreak => GameConfig.ConfigData.ConditionShowAdBreak;
        public static bool CanShowInter => !IsNoAds && CurrentLevel > LevelShowInter && IAAManager.PassAdFrequency();
        public static bool ConditionShowInter => GameConfig.ConfigData.ConditionShowInter;
        public static bool CanShowAppOpen => !IsNoAds && CurrentLevel > LevelShowInter && IAAManager.PassAppOpenFrequency();
        public static bool CanShowNativeAdInPopupWin => CurrentLevel > GameConfig.ConfigData.LevelShowNativeAdInPopupWin;
        public static bool CanShowInterBetweenLevelPhase => GameConfig.ConfigData.CanShowInterBetweenLevelPhase;
        public static bool CanShowAppOpenWhenOpenGame => GameConfig.ConfigData.ShowAppOpenWhenOpenGame;
        public static bool CanShowNativeAds => GameConfig.ConfigData.CanShowNativeAds && !IsNoAds;

        public static int SessionStartCount => PlayerDataManager.PlayerData.SessionStartCount;
        public static int VideoWinCount => PlayerDataManager.PlayerData.VideoWinCount;
        public static int VideoReviveCount => PlayerDataManager.PlayerData.VideoReviveCount;
        public static int FreeVideoWin => GameConfig.ConfigData.FreeVideoWin;

        // Game Configs
        public static int CoinEarnWinLevel => GameConfig.ConfigData.CoinEarnWinLevel;
        public static int ThreadEarnWinLevel => GameConfig.ConfigData.ThreadEarnWinLevel;
        public static int LevelUnlockHome => RemoteLevelConfig.TryGetValue("level_unlock_home", out int result) ? result : GameConfig.ConfigData.LevelUnlockHome;
        public static int LevelUnlockCollections => GameConfig.ConfigData.LevelUnlockCollections;
        public static int LevelUnlockUndo => RemoteLevelConfig.TryGetValue("level_unlock_shuffle", out int result) ? result : GameConfig.ConfigData.LevelUnlockUndo;
        public static int LevelUnlockAddSlots => RemoteLevelConfig.TryGetValue("level_unlock_addslots", out int result) ? result : GameConfig.ConfigData.LevelUnlockAddSlots;
        public static int LevelUnlockClear => RemoteLevelConfig.TryGetValue("level_unlock_clear", out int result) ? result : GameConfig.ConfigData.LevelUnlockClear;
        public static int PriceUndo => GameConfig.ConfigData.PriceUndo;
        public static int PriceAddSlots => GameConfig.ConfigData.PriceAddSlots;
        public static int PriceClear => GameConfig.ConfigData.PriceClear;
        public static int PriceRevive => GameConfig.ConfigData.PriceRevive;
        public static int LevelNeedsInternet => RemoteLevelConfig.TryGetValue("level_requires_internet", out int result) ? result : GameConfig.ConfigData.LevelNeedsInternet;
        public static int TimeRecoverHeart => GameConfig.ConfigData.TimeRecoverHeart;
        public static int UnlimitedLifeTimeLimit => GameConfig.ConfigData.UnlimitedLifeTimeLimit;
        public static int UnlimitedLifeRewardAdsTimeLimit => GameConfig.ConfigData.UnlimitedLifeRewardAdsTimeLimit;
        public static int UnlimitedLifeIAPTimeLimit => GameConfig.ConfigData.UnlimitedLifeIAPTimeLimit;
        public static int LevelShowInter => GameConfig.ConfigData.LevelShowInter;
        public static int TimeShowInter => GameConfig.ConfigData.TimeShowInter;
        public static int TimeShowAdBreak => GameConfig.ConfigData.TimeShowAdBreak;
        public static int TimeShowAppOpen => GameConfig.ConfigData.TimeShowAppOpen;
        public static int WinRateOfRewardAd => GameConfig.ConfigData.WinRateOfRewardAd;
        public static int DailyRewardRate => GameConfig.ConfigData.DailyRewardRate;
        public static float CoilMaxSizeRatio => GameConfig.ConfigData.CoilMaxSizeRatio;
        public static int WinLayout => GameConfig.ConfigData.WinLayout;
        public static int LevelCallToRate => RemoteLevelConfig.TryGetValue("level_call_to_rate", out int result) ? result : GameConfig.ConfigData.LevelCallToRate;

        public static bool IntroToBuilding => PlayerDataManager.PlayerData.IntroToBuilding;

        public static bool UnlockHome => CurrentLevel >= LevelUnlockHome;
        public static bool UnlockAdButton => CurrentLevel >= LevelUnlockHome + 1;
        public static bool UnlockDailyRewards => false/*CurrentLevel >= LevelUnlockHome*/;

        // Coins
        public static int CurrentCoin => PlayerDataManager.PlayerData.Coin;
        public static int CurrentCoinInPiggyBank => PlayerDataManager.PlayerData.CoinInPiggyBank;
        public static int PiggyBankWinLevelEarn => GameConfig.ConfigData.PiggyBankWinLevelEarn;
        public static int PiggyBankMaxCoin => GameConfig.ConfigData.PiggyBankMaxCoin;
        public static bool IsFullPiggyBank => CurrentCoinInPiggyBank >= PiggyBankMaxCoin;

        // Lives
        public static int CurrentLive => LiveManager.LiveData.CurrentLives;
        public static bool IsFullLive => CurrentLive >= LiveManager.MaxLives;
        public static bool IsInfiniteLive => LiveManager.LiveData.IsInfiniteLives;
        public static bool CanShowHeartOffers => false/*PlayerDataManager.PlayerData.CanShowHeartOffers*/;

        // Boosters
        public static int GetBoosterAmount(BoosterType boosterType) => BoosterManager.OnGetBoosterAmount.Invoke(boosterType);
    }
}
