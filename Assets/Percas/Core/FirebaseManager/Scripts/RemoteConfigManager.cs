using System;
using System.Collections;
using Firebase.RemoteConfig;
using UnityEngine;
using Percas;

namespace PercasSDK
{
    public class RemoteConfigManager : MonoBehaviour
    {
        private const float fetchTimeoutSeconds = 10.0f;
        public static bool LoadDataDone;

        // Default values for Remote Config parameters
        //private readonly Dictionary<string, object> defaultConfigValues = new()
        //{
        //    { "native_template_id", "small" },
        //    { "native_ad_position", 1 },
        //    { "coin_earn_win_level", GameConfig.ConfigData.CoinEarnWinLevel },
        //    { "thread_earn_win_level", GameConfig.ConfigData.ThreadEarnWinLevel },
        //    { "level_unlock_booster_undo", GameConfig.ConfigData.LevelUnlockUndo },
        //    { "level_unlock_booster_add_slots", GameConfig.ConfigData.LevelUnlockAddSlots },
        //    { "level_unlock_booster_clear", GameConfig.ConfigData.LevelUnlockClear },
        //    { "price_booster_undo", GameConfig.ConfigData.PriceUndo },
        //    { "price_booster_add_slots", GameConfig.ConfigData.PriceAddSlots },
        //    { "price_booster_clear", GameConfig.ConfigData.PriceClear },
        //    { "price_revive", GameConfig.ConfigData.PriceRevive },
        //    { "time_recover_heart", GameConfig.ConfigData.TimeRecoverHeart },
        //    { "unlimited_life_time_limit", GameConfig.ConfigData.UnlimitedLifeTimeLimit },
        //    { "unlimited_life_reward_ads_time_limit", GameConfig.ConfigData.UnlimitedLifeRewardAdsTimeLimit },
        //    { "unlimited_life_iap_time_limit", GameConfig.ConfigData.UnlimitedLifeIAPTimeLimit },
        //    { "level_show_inter", GameConfig.ConfigData.LevelShowInter },
        //    { "time_show_inter", GameConfig.ConfigData.TimeShowInter },
        //};

        /// <summary>
        /// Initialize Remote Config with default values and fetch updates.
        /// </summary>
        public void Initialize()
        {
            // Set default values
            //FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaultConfigValues).ContinueWithOnMainThread(task =>
            //{
            //    // Fetch and activate Remote Config values
            //    FetchRemoteConfig();
            //});

            StartCoroutine(FetchAndActiveConfigs());
            Invoke(nameof(SetLoadDataDone), 2);
        }

        /// <summary>
        /// Fetch and activate the latest Remote Config values.
        /// </summary>
        private IEnumerator FetchAndActiveConfigs()
        {
            //FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).ContinueWithOnMainThread(fetchTask =>
            //{
            //    if (fetchTask.IsCompleted && fetchTask.Exception == null)
            //    {
            //        FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(activateTask =>
            //        {
            //            if (activateTask.IsCompleted)
            //            {
            //                Debug.Log("Remote Config fetched and activated!");
            //                GameConfig.Instance.LoadRemoteConfigs();
            //            }
            //        });
            //    }
            //    else
            //    {
            //        Debug.LogWarning($"Remote Config fetch failed: {fetchTask.Exception?.Message}");
            //    }
            //});

            //////////////////////////////////////

            // Fetch remote config from Firebase.
            // Cache expiration for remote config fetch. Set to TimeSpan.Zero to force a fetch every time.
            var fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);

            yield return new WaitUntil(() => fetchTask.IsCompleted);

            if (fetchTask.Exception != null)
            {
                Debug.LogError("Failed to fetch remote config: " + fetchTask.Exception);
                // GameConfig.Instance.LoadRemoteLevelData();
                GameConfig.Instance.LoadRemoteData();
                yield break;
            }

            // Activate the fetched config so values are available.
            var activateTask = FirebaseRemoteConfig.DefaultInstance.ActivateAsync();

            yield return new WaitUntil(() => activateTask.IsCompleted);

            Debug.LogError("Firebase Remote Config fetched and activated.");
            GameConfig.Instance.LoadRemoteConfigs();
            LoadDataDone = true;
        }

        private void SetLoadDataDone()
        {
            LoadDataDone = true;
        }

        private string GetConfigValueInString(string key)
        {
            return FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
        }

        private int GetDefaultInt(string key)
        {
            return key switch
            {
                "slot_amount" => GameConfig.ConfigData.LogicSlotAmount,
                "coin_earn_win_level" => GameConfig.ConfigData.CoinEarnWinLevel,
                "thread_earn_win_level" => GameConfig.ConfigData.ThreadEarnWinLevel,
                // "level_unlock_home" => GameConfig.ConfigData.LevelUnlockHome,
                // "level_unlock_collections" => GameConfig.ConfigData.LevelUnlockCollections,
                // "level_unlock_booster_undo" => GameConfig.ConfigData.LevelUnlockUndo,
                // "level_unlock_booster_add_slots" => GameConfig.ConfigData.LevelUnlockAddSlots,
                // "level_unlock_booster_clear" => GameConfig.ConfigData.LevelUnlockClear,
                // "level_needs_internet" => GameConfig.ConfigData.LevelNeedsInternet,
                "price_booster_undo" => GameConfig.ConfigData.PriceUndo,
                "price_booster_add_slots" => GameConfig.ConfigData.PriceAddSlots,
                "price_booster_clear" => GameConfig.ConfigData.PriceClear,
                "price_revive" => GameConfig.ConfigData.PriceRevive,
                // "time_recover_heart" => GameConfig.ConfigData.TimeRecoverHeart,
                "unlimited_life_free_time_limit" => GameConfig.ConfigData.UnlimitedLifeTimeLimit,
                "unlimited_life_free_reward_ads_time_limit" => GameConfig.ConfigData.UnlimitedLifeRewardAdsTimeLimit,
                "unlimited_life_iap_time_limit" => GameConfig.ConfigData.UnlimitedLifeIAPTimeLimit,
                "win_rate_of_reward_ad" => GameConfig.ConfigData.WinRateOfRewardAd,
                "daily_reward_rate" => GameConfig.ConfigData.DailyRewardRate,
                "piggy_bank_win_level_earn" => GameConfig.ConfigData.PiggyBankWinLevelEarn,
                "piggy_bank_max_coin" => GameConfig.ConfigData.PiggyBankMaxCoin,
                // "level_show_event_star_rush" => GameConfig.ConfigData.LevelShowEventStarRush,
                // "level_unlock_event_star_rush" => GameConfig.ConfigData.LevelUnlockEventStarRush,
                // "level_unlock_lucky_spin" => GameConfig.ConfigData.LevelUnlockLuckySpin,
                // "level_show_hidden_picture" => GameConfig.ConfigData.LevelShowEventHiddenPictureIcon,
                // "level_unlock_hidden_picture" => GameConfig.ConfigData.LevelUnlockEventHiddenPicture,
                "level_show_inter" => GameConfig.ConfigData.LevelShowInter,
                "level_show_ad_break" => GameConfig.ConfigData.LevelShowAdBreak,
                "time_show_inter" => GameConfig.ConfigData.TimeShowInter,
                "time_show_ad_break" => GameConfig.ConfigData.TimeShowAdBreak,
                "time_show_app_open" => GameConfig.ConfigData.TimeShowAppOpen,
                "level_show_native_in_win" => GameConfig.ConfigData.LevelShowNativeAdInPopupWin,
                // "level_call_to_rate" => GameConfig.ConfigData.LevelCallToRate,
                _ => 0,
            }; ;
        }

        private double GetDefaultDouble(string key)
        {
            return key switch
            {
                "coil_max_size_ratio" => GameConfig.ConfigData.CoilMaxSizeRatio,
                _ => (double)0f,
            };
        }

        public string GetString(string key)
        {
            string configValue = GetConfigValueInString(key);
            return configValue;
        }
        public int GetInt(string key)
        {
            string configValue = GetConfigValueInString(key);
            if (!string.IsNullOrEmpty(configValue) && long.TryParse(configValue, out long parsedValue))
            {
                return (int)parsedValue;
            }
            else
            {
                return GetDefaultInt(key);
            }
        }
        public double GetDouble(string key)
        {
            string configValue = GetConfigValueInString(key);
            if (!string.IsNullOrEmpty(configValue) && double.TryParse(configValue, out double parsedValue))
            {
                return parsedValue;
            }
            else
            {
                return GetDefaultDouble(key);
            }
        }

        public bool GetBool(string key)
        {
            bool value = FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue;
            return value;
        }
    }
}
