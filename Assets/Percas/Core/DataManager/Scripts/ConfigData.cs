using System.Collections.Generic;
using Percas;

public class ConfigData
{
    public int LogicSlotAmount = 5;
    // Base Game Economy
    public int CoinEarnWinLevel = 10;
    public int ThreadEarnWinLevel = 0;
    public int LevelUnlockHome = 6;
    // public int LevelUnlockDailyRewards = 6;
    public int LevelUnlockLuckySpin = 11;
    public int LevelUnlockCollections = 8;
    public int LevelShowEventStarRush = 25;
    public int LevelUnlockEventStarRush = 30;
    public int LevelShowEventHiddenPictureIcon = 45;
    public int LevelUnlockEventHiddenPicture = 55;
    public int LevelUnlockUndo = 5;
    public int LevelUnlockAddSlots = 10;
    public int LevelUnlockClear = 15;
    public int LevelNeedsInternet = 8;
    public int PriceUndo = 80;
    public int PriceAddSlots = 240;
    public int PriceClear = 160;
    public int PriceRevive = 100;
    public int TimeRecoverHeart = 1800;
    public int UnlimitedLifeTimeLimit = 600;
    public int UnlimitedLifeRewardAdsTimeLimit = 1800;
    public int UnlimitedLifeIAPTimeLimit = 3600;
    public int WinRateOfRewardAd = 2;
    public int DailyRewardRate = 2;
    public int PiggyBankWinLevelEarn = 100;
    public int PiggyBankMaxCoin = 2000;
    public int WinLayout = 1;
    public int LevelCallToRate = 12;
    public int FreeVideoWin = 1;

    public float CoilMaxSizeRatio = 1.11f;

    // public bool DataFetchedOnce = false;
    public bool AutoShowSalePopup = false;
    public bool ShowBuildingIntro = true;

    // Interstitial Ads
    public int LevelShowInter = 6;
    public int LevelShowAdBreak = 8;
    public int TimeShowInter = 60;
    public int TimeShowAdBreak = 60;
    public int TimeShowAppOpen = 30;
    public int LevelShowNativeAdInPopupWin = 2;
    public bool CanShowInterBetweenLevelPhase = true;
    public bool ShowAppOpenWhenOpenGame = true;
    public bool CanShowNativeAds = true;
    public bool ShowPhaseInterConfirm = true;
    public bool ConditionShowAdBreak = true;
    public bool ConditionShowInter = true;

    // Others
    public string UserLanguageCode = "en";
    public int NotiWelcomeSent = 0;
    // public bool UseRemoteLevelData = false;

    public void SetUserLanguageCode(string code)
    {
        UserLanguageCode = code;
        GameConfig.OnSave?.Invoke();
    }

    public List<string> KeysFetchedOnce = new();

    public bool IsKeyFetchedOnce(string key)
    {
        return KeysFetchedOnce.Contains(key);
    }

    public void AddKeyFetchedOnce(string key)
    {
        if (!IsKeyFetchedOnce(key))
        {
            KeysFetchedOnce.Add(key);
        }
    }

    public string RemoteLevelData;
    public string InterGapTime;
    public string RemoteData;
}
