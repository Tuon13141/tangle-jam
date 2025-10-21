using System;
using UnityEngine;
using Percas.UI;
using Percas.Data;
using Sonat;

namespace Percas.Live
{
    public class LiveManager : MonoBehaviour
    {
        [Header("Live Configs")]
        [SerializeField] int maxLives = 5;

        public static LiveData LiveData = new();

        public static Action OnSave;

        public static Action OnUseLive;
        public static Action OnRefillFullLives;
        public static Action<int, LogCurrency> OnRefillLives;
        public static Action<int, LogCurrency> OnEarnInfiniteLives;
        public static Action OnUpdateLastTimeWatchHeartOffer;

        public static int MaxLives;

        public static bool LoadDataDone { get; private set; }

        public static DateTime? NextLiveRefillTime => TimeHelper.ParseIsoString(LiveData.NextLiveRefillTime);
        public static DateTime? InfiniteLivesEndTime => TimeHelper.ParseIsoString(LiveData.InfiniteLivesEndTime);
        public static DateTime? LastTimeWatchHeartOffer => TimeHelper.ParseIsoString(LiveData.LastTimeWatchHeartOffer);

        private void Awake()
        {
            InitData();

            MaxLives = maxLives;

            OnSave += SaveData;

            TimeManager.OnTick += UpdateLiveRefill;
            TimeManager.OnTick += UpdateInfiniteLive;

            OnUseLive += UseLive;
            OnRefillFullLives += RefillFullLives;
            OnRefillLives += RefillLives;
            OnEarnInfiniteLives += EarnInfiniteLives;
            OnUpdateLastTimeWatchHeartOffer += UpdateLastTimeWatchHeartOffer;

            ActionEvent.OnLevelStart += UseLive;
            ActionEvent.OnLevelWin += WinLive;
        }

        private void OnDestroy()
        {
            OnSave -= SaveData;

            TimeManager.OnTick -= UpdateLiveRefill;
            TimeManager.OnTick -= UpdateInfiniteLive;

            OnUseLive -= UseLive;
            OnRefillFullLives -= RefillFullLives;
            OnRefillLives -= RefillLives;
            OnEarnInfiniteLives -= EarnInfiniteLives;
            OnUpdateLastTimeWatchHeartOffer -= UpdateLastTimeWatchHeartOffer;

            ActionEvent.OnLevelStart -= UseLive;
            ActionEvent.OnLevelWin -= WinLive;
        }

        private void InitData()
        {
            if (!PlayerPrefs.HasKey(Const.KEY_LIVE_DATA))
            {
                LiveData.CurrentLives = maxLives;
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
                string encryptedJson = PlayerPrefs.GetString(Const.KEY_LIVE_DATA);
                string jsonData = Helpers.Decrypt(encryptedJson);
                LiveData data = JsonUtility.FromJson<LiveData>(jsonData);
                LiveData = data;
            }
            catch (Exception)
            {
                LiveData = new();
            }
        }

        private void SaveData()
        {
            string jsonData = JsonUtility.ToJson(LiveData);
            string encryptedJson = Helpers.Encrypt(jsonData);
            PlayerPrefs.SetString(Const.KEY_LIVE_DATA, encryptedJson);
            DataManager.Instance.WriteToLocal(Const.KEY_LIVE_DATA, jsonData);
        }

        private void UpdateNextLiveRefillTime()
        {
            DateTime current = TimeHelper.ParseIsoString(LiveData.NextLiveRefillTime);
            LiveData.NextLiveRefillTime = TimeHelper.ToIsoString(current.AddSeconds(GameLogic.TimeRecoverHeart));
        }

        private void ResetNextLiveRefillTime()
        {
            LiveData.NextLiveRefillTime = TimeHelper.ToIsoString(DateTime.UtcNow.AddSeconds(GameLogic.TimeRecoverHeart));
        }

        private void EarnLive()
        {
            int newLives = LiveData.CurrentLives + 1;
            LiveData.CurrentLives = Math.Clamp(newLives, 0, maxLives); // refill 1 live
        }

        private void SpendLive()
        {
            int newLives = LiveData.CurrentLives - 1;
            LiveData.CurrentLives = Math.Clamp(newLives, 0, maxLives);
        }

        private void UpdateLiveRefill()
        {
            if (!LiveData.IsRefilling) return;

            if (LiveData.CurrentLives >= maxLives)
            {
                LiveData.IsRefilling = false;
                OnSave?.Invoke();
                return;
            }

            DateTime? nextRefillTime = TimeHelper.ParseIsoString(LiveData.NextLiveRefillTime);
            if (nextRefillTime.HasValue && DateTime.UtcNow >= nextRefillTime.Value)
            {
                EarnLive();
                UpdateNextLiveRefillTime();
                OnSave?.Invoke();
            }
        }

        private void UpdateInfiniteLive()
        {
            if (!LiveData.IsInfiniteLives) return;

            DateTime? infiniteEndTime = TimeHelper.ParseIsoString(LiveData.InfiniteLivesEndTime);
            if (infiniteEndTime.HasValue && DateTime.UtcNow >= infiniteEndTime.Value)
            {
                LiveData.IsInfiniteLives = false;
                OnSave?.Invoke();
            }
        }

        private void UseLive()
        {
            if (LiveData.IsInfiniteLives) return;
            if (GameLogic.CurrentLevel < GameLogic.LevelUnlockHome) return;
            if (GameLogic.IsHiddenPictureMode) return;
            if (LiveData.CurrentLives >= 1)
            {
                SpendLive();
                if (LiveData.CurrentLives == maxLives - 1) ResetNextLiveRefillTime();
                if (LiveData.CurrentLives < maxLives) LiveData.IsRefilling = true;
                OnSave?.Invoke();
                //StartLiveRefill();
            }
        }

        private void WinLive()
        {
            if (GameLogic.IsHiddenPictureMode) return;
            OnRefillLives?.Invoke(1, null);
            OnSave?.Invoke();
        }

        private void RefillLives(int amount, LogCurrency logCurrency)
        {
            int newLives = LiveData.CurrentLives + amount;
            LiveData.CurrentLives = Math.Clamp(newLives, 0, maxLives);
            if (GameLogic.IsFullLive)
            {
                LiveData.IsRefilling = false;
            }
            if (logCurrency != null)
            {
                var log = new SonatLogEarnVirtualCurrency()
                {
                    virtual_currency_name = logCurrency.name,
                    virtual_currency_type = logCurrency.type,
                    value = amount,
                    level = GameLogic.CurrentLevel,
                    location = GameLogic.LogLocation,
                    screen = logCurrency.screen,
                    source = logCurrency.source,
                    spend_item_type = logCurrency.item_type,
                    spend_item_id = logCurrency.item_id,
                };
                log.Post();
            }
            OnSave?.Invoke();
        }

        private void RefillFullLives()
        {
            LiveData.CurrentLives = maxLives;
            LiveData.IsRefilling = false;
            OnSave?.Invoke();
        }

        private void EarnInfiniteLives(int infiniteTimeInSeconds, LogCurrency logCurrency)
        {
            if (LiveData.IsInfiniteLives)
            {
                DateTime current = TimeHelper.ParseIsoString(LiveData.InfiniteLivesEndTime);
                LiveData.InfiniteLivesEndTime = TimeHelper.ToIsoString(current.AddSeconds(infiniteTimeInSeconds));
            }
            else
            {
                LiveData.InfiniteLivesEndTime = TimeHelper.ToIsoString(DateTime.UtcNow.AddSeconds(infiniteTimeInSeconds));
                LiveData.IsInfiniteLives = true;
            }
            if (logCurrency != null)
            {
                var log = new SonatLogEarnVirtualCurrency()
                {
                    virtual_currency_name = logCurrency.name,
                    virtual_currency_type = logCurrency.type,
                    value = infiniteTimeInSeconds,
                    level = GameLogic.CurrentLevel,
                    location = GameLogic.LogLocation,
                    screen = logCurrency.screen,
                    source = logCurrency.source,
                    spend_item_type = logCurrency.item_type,
                    spend_item_id = logCurrency.item_id,
                };
                log.Post();
            }
            OnSave?.Invoke();
            UICurrencyManager.OnShowLiveGain?.Invoke(true, infiniteTimeInSeconds);
        }

        private void UpdateLastTimeWatchHeartOffer()
        {
            LiveData.LastTimeWatchHeartOffer = DateTime.UtcNow.AddSeconds(GameLogic.UnlimitedLifeRewardAdsTimeLimit).ToString();
            OnSave?.Invoke();
        }
    }
}
