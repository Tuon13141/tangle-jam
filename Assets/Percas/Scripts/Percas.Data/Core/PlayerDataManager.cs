using System;
using System.Collections.Generic;
using UnityEngine;
using Percas.UI;
using DG.Tweening;
using Sonat;

namespace Percas.Data
{
    [Serializable]
    public class LogCurrency
    {
        public string type;
        public string name;
        public string screen;
        public string source;
        public string item_type;
        public string item_id;

        public LogCurrency(string type, string name, string screen, string source, string item_type, string item_id)
        {
            this.type = type;
            this.name = name;
            this.screen = screen;
            this.source = source;
            this.item_type = item_type;
            this.item_id = item_id;
        }
    }

    public class PlayerDataManager : MonoBehaviour
    {
        public static PlayerData PlayerData = new();

        public static Action OnSave;
        public static Action OnResetLevel;
        public static Action<string> OnSetUserType;

        public static Action OnResetLevelUseBoosterCount;
        public static Action OnUpdateLevelUseBoosterCount;
        public static Action OnResetContinueTimes;
        public static Action OnUpdateContinueTimes;

        public static Action<int, LogCurrency> OnEarnCoin;
        public static Action<int, Action<bool>, string, LogCurrency> OnSpendCoin;

        public static Action<int, LogCurrency> OnEarnCoil;
        public static Action<int, Action<bool>, string, LogCurrency> OnSpendCoil;

        public static Action<int, LogCurrency> OnEarnPin;
        public static Action<int, Action<bool>, string, LogCurrency> OnSpendPin;
        public static Action OnOpenBasket;

        public static Action OnResetPiggyBank;
        public static Action OnPurchaseStarterPack;

        public static Action<float> OnPlayingTimeCalculated; // Event to notify playing time

        public static bool LoadDataDone { get; private set; }

        public static int LevelStar { get; set; }
        public static int LevelPin { get; set; }
        public static int LevelCoil { get; set; }
        public static int LevelPhase { get; set; }
        public static int LevelUseBoosterCount { get; set; }
        public static int LevelMoveCount { get; set; }
        public static int ContinueTimes { get; set; }
        public static float LevelPlayingTime { get; set; }
        public static float PhasePlayingTime { get; set; }

        private float levelStartTime; // Time when the level started
        private float levelPausedTime; // Total time the app was paused

        private float phaseStartTime;
        private float phasePausedTime;

        private float pauseStartTime; // Time when the app was last paused
        private bool isPaused = false;

        private void Awake()
        {
            InitData();

            OnSave += SaveData;
            OnResetLevel += ResetLevel;
            OnSetUserType += SetUserType;

            OnResetLevelUseBoosterCount += ResetLevelUseBoosterCount;
            OnUpdateLevelUseBoosterCount += UpdateLevelUseBoosterCount;
            OnResetContinueTimes += ResetContinueTimes;
            OnUpdateContinueTimes += UpdateContinueTimes;

            OnEarnCoin += EarnCoin;
            OnSpendCoin += SpendCoin;

            OnEarnCoil += EarnCoil;
            OnSpendCoil += SpendCoil;

            OnEarnPin += EarnPin;
            OnSpendPin += SpendPin;
            OnOpenBasket += OpenBasket;

            OnResetPiggyBank += ResetPiggyBank;
            OnPurchaseStarterPack += PurchaseStarterPack;

            ActionEvent.OnLevelStart += LevelStart;
            ActionEvent.OnLevelStart += UpdateLevelAttempts;

            ActionEvent.OnPhaseStart += PhaseStart;
            ActionEvent.OnPhaseEnd += PhaseEnd;

            ActionEvent.OnSetLevelPhase += SetLevelPhase;

            ActionEvent.OnItemsMatched += ItemsMatched;

            ActionEvent.OnCoilTap += UpdateMoveCount;
            ActionEvent.OnReleasePin += ReleasePin;

            ActionEvent.OnLevelWin += LevelWin;
            ActionEvent.OnLevelLose += LevelLose;
        }

        private void OnDestroy()
        {
            OnSave -= SaveData;
            OnResetLevel -= ResetLevel;
            OnSetUserType -= SetUserType;

            OnResetLevelUseBoosterCount -= ResetLevelUseBoosterCount;
            OnUpdateLevelUseBoosterCount -= UpdateLevelUseBoosterCount;
            OnResetContinueTimes -= ResetContinueTimes;
            OnUpdateContinueTimes -= UpdateContinueTimes;

            OnEarnCoin -= EarnCoin;
            OnSpendCoin -= SpendCoin;

            OnEarnCoil -= EarnCoil;
            OnSpendCoil -= SpendCoil;

            OnEarnPin -= EarnPin;
            OnSpendPin -= SpendPin;
            OnOpenBasket -= OpenBasket;

            OnResetPiggyBank -= ResetPiggyBank;
            OnPurchaseStarterPack -= PurchaseStarterPack;

            ActionEvent.OnLevelStart -= LevelStart;
            ActionEvent.OnLevelStart -= UpdateLevelAttempts;

            ActionEvent.OnPhaseStart -= PhaseStart;
            ActionEvent.OnPhaseEnd -= PhaseEnd;

            ActionEvent.OnSetLevelPhase -= SetLevelPhase;

            ActionEvent.OnItemsMatched -= ItemsMatched;

            ActionEvent.OnCoilTap -= UpdateMoveCount;
            ActionEvent.OnReleasePin -= ReleasePin;

            ActionEvent.OnLevelWin -= LevelWin;
            ActionEvent.OnLevelLose -= LevelLose;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // App is being paused
                isPaused = true;
                pauseStartTime = Time.time;
            }
            else
            {
                // App is being resumed
                if (isPaused)
                {
                    isPaused = false;
                    levelPausedTime += Time.time - pauseStartTime;
                    phasePausedTime += Time.time - pauseStartTime;
                }
            }
        }

        private void InitData()
        {
            if (!PlayerPrefs.HasKey(Const.KEY_PLAYER_DATA))
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
                string encryptedJson = PlayerPrefs.GetString(Const.KEY_PLAYER_DATA);
                string jsonData = Helpers.Decrypt(encryptedJson);
                PlayerData data = JsonUtility.FromJson<PlayerData>(jsonData);
                PlayerData = data;
            }
            catch (Exception)
            {
                PlayerData = new();
            }
        }

        private void SaveData()
        {
            string jsonData = JsonUtility.ToJson(PlayerData);
            string encryptedJson = Helpers.Encrypt(jsonData);
            PlayerPrefs.SetString(Const.KEY_PLAYER_DATA, encryptedJson);
            DataManager.Instance.WriteToLocal(Const.KEY_PLAYER_DATA, jsonData);
        }

        private void ResetLevel()
        {
            PlayerData.HighestLevel = 0;
            PlayerData.CoinInPiggyBank = 0;
            PlayerData.Coil = 0;
            OnSave?.Invoke();
        }

        private void SetUserType(string value)
        {
            if (!GameLogic.IsNullUserType) return;
            PlayerData.UserTyper = value;
            OnSave?.Invoke();
        }

        private void ResetLevelUseBoosterCount()
        {
            LevelUseBoosterCount = 0;
        }

        private void UpdateLevelUseBoosterCount()
        {
            LevelUseBoosterCount += 1;
        }

        private void ResetContinueTimes()
        {
            ContinueTimes = 0;
        }

        private void UpdateContinueTimes()
        {
            ContinueTimes += 1;
        }

        // source
        // item_type
        // item_id
        // screen
        private void EarnCoin(int valueToEarn, LogCurrency logCurrency)
        {
            PlayerData.Coin += valueToEarn;
            if (logCurrency != null)
            {
                var log = new SonatLogEarnVirtualCurrency()
                {
                    virtual_currency_name = logCurrency.name,
                    virtual_currency_type = logCurrency.type,
                    value = valueToEarn,
                    level = logCurrency.source == "level_win" ? GameLogic.CurrentLevel - 1 : GameLogic.CurrentLevel,
                    location = GameLogic.LogLocation,
                    screen = logCurrency.screen,
                    source = logCurrency.source,
                    spend_item_type = logCurrency.item_type,
                    spend_item_id = logCurrency.item_id,
                };
                log.Post();
            }
            OnSave?.Invoke();
            UICurrencyManager.OnShowCoinGain?.Invoke(valueToEarn);
        }

        private void SpendCoin(int valueToSpend, Action<bool> onCompleted, string spendLocation, LogCurrency logCurrency)
        {
            if (PlayerData.Coin < valueToSpend)
            {
                onCompleted?.Invoke(false);
                return;
            }
            PlayerData.Coin = Math.Max(0, PlayerData.Coin - valueToSpend);
            if (logCurrency != null)
            {
                var log = new SonatLogSpendVirtualCurrency()
                {
                    virtual_currency_name = logCurrency.name,
                    virtual_currency_type = logCurrency.type,
                    value = valueToSpend,
                    level = GameLogic.CurrentLevel,
                    location = GameLogic.LogLocation,
                    screen = logCurrency.screen,
                    earn_item_type = logCurrency.item_type,
                    earn_item_id = logCurrency.item_id,
                };
                log.Post();
            }
            OnSave?.Invoke();
            onCompleted?.Invoke(true);
            Helpers.ChangeValueInt(PlayerData.Coin + valueToSpend, PlayerData.Coin, 0.5f, 0.0f, (value) =>
            {
                UICoinBalance.OnUpdateUI?.Invoke(value);
            }).OnStart(() =>
            {
                AudioController.Instance.PlaySpawnCoins();
            });
        }

        private void EarnCoil(int valueToEarn, LogCurrency logCurrency)
        {
            PlayerData.Coil += valueToEarn;
            if (logCurrency != null)
            {
                var log = new SonatLogEarnVirtualCurrency()
                {
                    virtual_currency_name = logCurrency.name,
                    virtual_currency_type = logCurrency.type,
                    value = valueToEarn,
                    level = logCurrency.source == "level_win" ? GameLogic.CurrentLevel - 1 : GameLogic.CurrentLevel,
                    location = GameLogic.LogLocation,
                    screen = logCurrency.screen,
                    source = logCurrency.source,
                    spend_item_type = logCurrency.item_type,
                    spend_item_id = logCurrency.item_id,
                };
                log.Post();
            }
            OnSave?.Invoke();
            UICurrencyManager.OnShowCoilGain?.Invoke(valueToEarn);
        }

        private void SpendCoil(int valueToSpend, Action<bool> onCompleted, string spendLocation, LogCurrency logCurrency)
        {
            if (PlayerData.Coil < valueToSpend)
            {
                onCompleted?.Invoke(false);
                return;
            }
            PlayerData.Coil = Math.Max(0, PlayerData.Coil - valueToSpend);
            if (logCurrency != null)
            {
                var log = new SonatLogSpendVirtualCurrency()
                {
                    virtual_currency_name = logCurrency.name,
                    virtual_currency_type = logCurrency.type,
                    value = valueToSpend,
                    level = GameLogic.CurrentLevel,
                    location = GameLogic.LogLocation,
                    screen = logCurrency.screen,
                    earn_item_type = logCurrency.item_type,
                    earn_item_id = logCurrency.item_id,
                };
                log.Post();
            }
            OnSave?.Invoke();
            onCompleted?.Invoke(true);
        }

        private void EarnPin(int valueToEarn, LogCurrency logCurrency)
        {
            PlayerData.Pin += valueToEarn;
            if (logCurrency != null)
            {
                var log = new SonatLogEarnVirtualCurrency()
                {
                    virtual_currency_name = logCurrency.name,
                    virtual_currency_type = logCurrency.type,
                    value = valueToEarn,
                    level = logCurrency.source == "level_win" ? GameLogic.CurrentLevel - 1 : GameLogic.CurrentLevel,
                    location = GameLogic.LogLocation,
                    screen = logCurrency.screen,
                    source = logCurrency.source,
                    spend_item_type = logCurrency.item_type,
                    spend_item_id = logCurrency.item_id,
                };
                log.Post();
            }
            OnSave?.Invoke();
            if (PlayerData.PreIntroToLuxuryBasket) UICurrencyManager.OnShowPinGain?.Invoke(valueToEarn);
        }

        private void SpendPin(int valueToSpend, Action<bool> onCompleted, string spendLocation, LogCurrency logCurrency)
        {
            if (PlayerData.Pin < valueToSpend)
            {
                onCompleted?.Invoke(false);
                return;
            }
            PlayerData.Pin = Math.Max(0, PlayerData.Pin - valueToSpend);
            if (logCurrency != null)
            {
                var log = new SonatLogSpendVirtualCurrency()
                {
                    virtual_currency_name = logCurrency.name,
                    virtual_currency_type = logCurrency.type,
                    value = valueToSpend,
                    level = GameLogic.CurrentLevel,
                    location = GameLogic.LogLocation,
                    screen = logCurrency.screen,
                    earn_item_type = logCurrency.item_type,
                    earn_item_id = logCurrency.item_id,
                };
                log.Post();
            }
            OnSave?.Invoke();
            onCompleted?.Invoke(true);
        }

        private void OpenBasket()
        {
            PlayerData.LuxuryBasketOpenedCount += 1;
            OnSave?.Invoke();
        }

        private void ResetPiggyBank()
        {
            PlayerData.CoinInPiggyBank = 0;
            OnSave?.Invoke();
        }

        private void PurchaseStarterPack()
        {
            PlayerData.IsStarterPackPurchased = true;
            OnSave?.Invoke();
        }

        private void LevelStart()
        {
            LevelStar = 0;
            LevelPin = 0;
            LevelCoil = 0;
            LevelUseBoosterCount = 0;
            LevelMoveCount = 0;
            levelStartTime = Time.time;
            LevelPlayingTime = 0f;
            levelPausedTime = 0f;
        }

        private void PhaseStart()
        {
            phaseStartTime = Time.time;
            PhasePlayingTime = 0f;
            phasePausedTime = 0f;
        }

        private void UpdateLevelAttempts()
        {
            PlayerData.LevelAttempts += 1;
            PlayerData.LoseStreak += 1;
            OnSave?.Invoke();
        }

        private void SetLevelPhase(int phase, LevelAsset levelData)
        {
            LevelPhase = phase;
        }

        private void ItemsMatched()
        {
            LevelCoil += 3;
        }

        private void UpdateMoveCount()
        {
            LevelMoveCount += 1;
        }

        private void ReleasePin()
        {
            LevelPin += 1;
        }

        private void LevelWin()
        {
            OnLevelEnd();
            SetContinueWith(null);
            TrackingManager.OnLevelEnd?.Invoke(true, "none");
            if (GameLogic.IsClassicMode)
            {
                PlayerData.HighestLevel += 1;
                if (PlayerData.LoseStreak > 0) PlayerData.WinStreak = 1;
                else PlayerData.WinStreak += 1;
                PlayerData.LoseStreak = 0;
            }
            else if (GameLogic.IsHiddenPictureMode)
            {
                HiddenPictureManager.Data.AddUnlockedPieces(GlobalSetting.HiddenPictureLevelIndex);
                PopupHiddenPicture.OnUpdateUI?.Invoke();
            }
            PlayerData.LevelAttempts = 0;
            PlayerData.CoinInPiggyBank = Math.Clamp(PlayerData.CoinInPiggyBank + GameLogic.PiggyBankWinLevelEarn, 0, GameLogic.PiggyBankMaxCoin);
            OnSave?.Invoke();
        }

        private void LevelLose()
        {
            OnLevelEnd();
            TrackingManager.OnLevelEnd?.Invoke(false, "out_of_space");
        }

        private void OnLevelEnd()
        {
            LevelPlayingTime = Time.time - levelStartTime - levelPausedTime;
            if (GameLogic.UnlockStarRush && StarRushManager.Data.IsRunning())
            {
                LevelStar = (int)(LevelCoil * 100 / LevelPlayingTime);
                StarRushManager.Data.UpdateYourLastScore(LevelStar);
            }
        }

        private void PhaseEnd()
        {
            OnPhaseEnd();
            TrackingManager.OnPhaseEnd?.Invoke(true, "none");
        }

        private void OnPhaseEnd()
        {
            PhasePlayingTime = Time.time - phaseStartTime - phasePausedTime;
        }

        public static void SetContinueWith(string value)
        {
            PlayerData.ContinueWith = value;
            OnSave?.Invoke();
        }

        public static int GetCollectionProcess(int index) //index start 0
        {
            if (index < 0) return 0;

            if (PlayerData.CollectionProgress == null || PlayerData.CollectionProgress.Count == 0)
            {
                PlayerData.CollectionProgress = new List<int>(new int[index + 1]);
                return 0;
            }
            else if (index < PlayerData.CollectionProgress.Count)
            {
                return PlayerData.CollectionProgress[index];
            }
            else
            {
                var tempList = new List<int>(new int[index - PlayerData.CollectionProgress.Count + 1]);
                PlayerData.CollectionProgress.AddRange(tempList);
                return 0;
            }
        }

        public static void SetCollectionProcess(int index, int value)
        {
            if (index < 0 || value < 0) return;

            var currentValue = GetCollectionProcess(index);
            if (currentValue != value)
            {
                PlayerData.CollectionProgress[index] = value;
                OnSave?.Invoke();
            }
        }

        public static bool IsFirstPurchase()
        {
            return PlayerData.PurchaseHistory.Count <= 0;
        }

        public static void UpdatePurchaseHistory(string productID)
        {
            PlayerData.PurchaseHistory.Add(productID);
            OnSave?.Invoke();
        }

        public static void AddTutorialShown(string tutorial)
        {
            PlayerData.TutorialShown.Add(tutorial);
            OnSave?.Invoke();
        }

        public static bool IsTutorialShown(string tutorial)
        {
            return PlayerData.TutorialShown.Contains(tutorial);
        }

        public static void AddPictureCoin(int pictureLevel)
        {
            PlayerData.PictureCoinReceived.Add(pictureLevel);
            OnSave?.Invoke();
        }

        public static bool IsInPictureCoinReceived(int pictureLevel)
        {
            return PlayerData.PictureCoinReceived.Contains(pictureLevel);
        }

        public static void AddPictureGain(int pictureLevel)
        {
            PlayerData.PictureGained.Add(pictureLevel);
            OnSave?.Invoke();
        }

        public static bool IsInPictureGained(int pictureLevel)
        {
            return PlayerData.PictureGained.Contains(pictureLevel);
        }

        private static int GetRandomAvailableQuoteIndex()
        {
            int result = 0;
            bool searching = true;
            while (searching)
            {
                int randomQuoteIndex = DataManager.Instance.GetRandomQuoteIndex();
                if (GameLogic.CurrentLevel <= DataManager.Instance.GetQuoteCount())
                {
                    string existingQuote = PlayerData.PictureQuotes.Find((i) => i.Contains($"_{randomQuoteIndex}"));
                    if (existingQuote == null)
                    {
                        searching = false;
                        result = randomQuoteIndex;
                    }
                }
                else
                {
                    result = randomQuoteIndex;
                }
            }
            return result;
        }

        public static int GetPictureQuoteByLevel(int level)
        {
            string existingQuote = PlayerData.PictureQuotes.Find((i) => i.Contains($"{level}_"));
            if (existingQuote != null)
            {
                string[] parts = existingQuote.Split('_');
                return int.Parse(parts[1]);
            }
            string newQuote;
            int randomQuote = GetRandomAvailableQuoteIndex();
            newQuote = $"{level}_{randomQuote}";
            PlayerData.PictureQuotes.Add(newQuote);
            OnSave?.Invoke();
            return randomQuote;
        }
    }
}
