using Newtonsoft.Json;
using PercasSDK;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Percas
{
    public class GameConfig : SingletonMonoBehaviour<GameConfig>
    {
        [Serializable]
        public class LevelDataWithConfigs
        {
            public Dictionary<string, int> LevelConfig { get; set; }
            public string LevelData { get; set; }
        }
        
        public bool CheatOn;
        public bool DebugOn;

        [Header("Game Configs")]
        public int BuildNumber;
        public int MaxLevel = 50;
        public int SessionStartCountToShowAppOpen = 3;

        [Header("Remote Configs")]
        [Tooltip("Add your RemoteConfigValueSO assets here. Each asset contains its own remote key.")]
        [SerializeField] List<RemoteConfigValueSO> remoteConfigs;

        [Header("Coil Configs")]
        public float TimeCoilMoveToSpool = 0.2f;
        public float TimeCoilPlayAnim = 0.3f;
        public float TimeCoilFillPicture = 1f;

        [HideInInspector] public bool IsMarketingVersion;

        public static ConfigData ConfigData = new();

        public string NativeTemplateID { get; set; }
        public int NativeAdPosition { get; set; }

        public static bool LoadDataDone { get; private set; }

        public static Action OnSave;

        public bool IsReachableInternet()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        protected override void Awake()
        {
            InitData();

            OnSave += SaveData;
        }

        private void OnDestroy()
        {
            OnSave -= SaveData;
        }

        private void InitData()
        {
            if (PlayerPrefs.HasKey(Const.KEY_CONFIG_DATA))
            {
                LoadData();
            }
            LoadDataDone = true;
        }

        private void LoadData()
        {
            try
            {
                string encryptedJson = PlayerPrefs.GetString(Const.KEY_CONFIG_DATA);
                string jsonData = Helpers.Decrypt(encryptedJson);
                ConfigData data = JsonUtility.FromJson<ConfigData>(jsonData);
                ConfigData = data;
            }
            catch (Exception)
            {
                ConfigData = new();
            }
        }

        private void SaveData()
        {
            string jsonData = JsonUtility.ToJson(ConfigData);
            string encryptedJson = Helpers.Encrypt(jsonData);
            PlayerPrefs.SetString(Const.KEY_CONFIG_DATA, encryptedJson);
            DataManager.Instance.WriteToLocal(Const.KEY_CONFIG_DATA, jsonData);
        }

        public void SentNotiWelcome()
        {
            ConfigData.NotiWelcomeSent = 1;
            SaveData();
        }

        private int ConfigVersion
        {
            get
            {
                return PlayerPrefs.GetInt("PERCAS_CONFIG_VERSION", 0);
            }
            set
            {
                PlayerPrefs.SetInt("PERCAS_CONFIG_VERSION", value);
            }
        }

        public void LoadRemoteConfigs()
        {
            int configVersion = FirebaseManager.RemoteConfig.GetInt("config_version");
            Debug.LogError($"canFetch = {configVersion} > {ConfigVersion} = {configVersion > ConfigVersion}");
            bool canFetch = configVersion > ConfigVersion;
            if (canFetch)
            {
                ConfigVersion = configVersion;
            }

            // if (!ConfigData.IsKeyFetchedOnce("level_data_update") || canFetch)
            // {
            //     Debug.LogError($"Fetch level_data_update");
            //     var remoteLevelData = FirebaseManager.RemoteConfig.GetString("level_data_update");
            //     Debug.LogError($"remoteLevelData = {remoteLevelData}");
            //     if (!string.IsNullOrEmpty(remoteLevelData))
            //     {
            //         ConfigData.RemoteLevelData = remoteLevelData;
            //         ConfigData.AddKeyFetchedOnce("level_data_update");
            //     }
            // }

            // LoadRemoteLevelData();

            if (!ConfigData.IsKeyFetchedOnce("remote_data") || canFetch)
            {
                Debug.LogError($"Fetch remote_data");
                var remoteDataOnFirebase = FirebaseManager.RemoteConfig.GetString("remote_data");
                Debug.LogError($"remoteDataOnFirebase = {remoteDataOnFirebase}");
                if (!string.IsNullOrEmpty(remoteDataOnFirebase))
                {
                    ConfigData.RemoteData = remoteDataOnFirebase;
                    ConfigData.AddKeyFetchedOnce("remote_data");
                }
            }

            LoadRemoteData();

            //check and set data update static 
            // if (Static.levelsUpdateDic == null)
            // {
            //     var levelRemoteConfig = FirebaseManager.RemoteConfig.GetString("level_data_update");
            //     if (!string.IsNullOrEmpty(levelRemoteConfig))
            //     {
            //         Static.levelsUpdateDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(levelRemoteConfig);
            //     }
            //     else
            //     {
            //         Static.levelsUpdateDic = new Dictionary<string, string>();
            //     }
            // }

            //custom timeShowInter follow level remote
            // var timeShowInterLevels = FirebaseManager.RemoteConfig.GetString("time_show_inter_levels");
            // GameLogic.InterGapTime = timeShowInterLevels;

            if (!ConfigData.IsKeyFetchedOnce("time_show_inter_levels") || canFetch)
            {
                Debug.LogError($"Fetch time_show_inter_levels");
                var interGapTime = FirebaseManager.RemoteConfig.GetString("time_show_inter_levels");
                Debug.LogError($"interGapTime = {interGapTime}");
                if (!string.IsNullOrEmpty(interGapTime))
                {
                    ConfigData.InterGapTime = interGapTime;
                    ConfigData.AddKeyFetchedOnce("time_show_inter_levels");
                }
            }

            // active new mode for App Store review
            Static.isActiveNewMode = FirebaseManager.RemoteConfig.GetBool("active_new_mode");

            if (!ConfigData.IsKeyFetchedOnce("remote_configs_fetched") || canFetch)
            {
                // Bool
                ConfigData.ShowPhaseInterConfirm = FirebaseManager.RemoteConfig.GetBool("show_phase_inter_confirm");
                // ConfigData.UseRemoteLevelData = FirebaseManager.RemoteConfig.GetBool("use_remote_level_data");
                ConfigData.AutoShowSalePopup = FirebaseManager.RemoteConfig.GetBool("auto_show_sale_popup");
                ConfigData.CanShowInterBetweenLevelPhase = FirebaseManager.RemoteConfig.GetBool("show_inter_between_level_phase");
                ConfigData.CanShowNativeAds = FirebaseManager.RemoteConfig.GetBool("can_show_native_ads");
                ConfigData.ShowAppOpenWhenOpenGame = FirebaseManager.RemoteConfig.GetBool("show_app_open_when_open_game");
                ConfigData.ShowBuildingIntro = FirebaseManager.RemoteConfig.GetBool("show_building_intro");
                ConfigData.ConditionShowAdBreak = FirebaseManager.RemoteConfig.GetBool("condition_show_adbreak");
                ConfigData.ConditionShowInter = FirebaseManager.RemoteConfig.GetBool("condition_show_inter");

                // Int
                ConfigData.LogicSlotAmount = FirebaseManager.RemoteConfig.GetInt("slot_amount");
                ConfigData.CoinEarnWinLevel = FirebaseManager.RemoteConfig.GetInt("coin_earn_win_level");
                ConfigData.ThreadEarnWinLevel = FirebaseManager.RemoteConfig.GetInt("thread_earn_win_level");
                // ConfigData.LevelUnlockHome = FirebaseManager.RemoteConfig.GetInt("level_unlock_home");
                // ConfigData.LevelUnlockCollections = FirebaseManager.RemoteConfig.GetInt("level_unlock_collections");
                // ConfigData.LevelUnlockUndo = FirebaseManager.RemoteConfig.GetInt("level_unlock_booster_undo");
                // ConfigData.LevelUnlockAddSlots = FirebaseManager.RemoteConfig.GetInt("level_unlock_booster_add_slots");
                // ConfigData.LevelUnlockClear = FirebaseManager.RemoteConfig.GetInt("level_unlock_booster_clear");
                // ConfigData.LevelNeedsInternet = FirebaseManager.RemoteConfig.GetInt("level_needs_internet");
                ConfigData.PriceUndo = FirebaseManager.RemoteConfig.GetInt("price_booster_undo");
                ConfigData.PriceAddSlots = FirebaseManager.RemoteConfig.GetInt("price_booster_add_slots");
                ConfigData.PriceClear = FirebaseManager.RemoteConfig.GetInt("price_booster_clear");
                ConfigData.PriceRevive = FirebaseManager.RemoteConfig.GetInt("price_revive");
                // ConfigData.TimeRecoverHeart = FirebaseManager.RemoteConfig.GetInt("time_recover_heart");
                ConfigData.UnlimitedLifeTimeLimit = FirebaseManager.RemoteConfig.GetInt("unlimited_life_free_time_limit");
                ConfigData.UnlimitedLifeRewardAdsTimeLimit = FirebaseManager.RemoteConfig.GetInt("unlimited_life_free_reward_ads_time_limit");
                ConfigData.UnlimitedLifeIAPTimeLimit = FirebaseManager.RemoteConfig.GetInt("unlimited_life_iap_time_limit");
                ConfigData.WinRateOfRewardAd = FirebaseManager.RemoteConfig.GetInt("win_rate_of_reward_ad");
                ConfigData.DailyRewardRate = FirebaseManager.RemoteConfig.GetInt("daily_reward_rate");
                ConfigData.PiggyBankWinLevelEarn = FirebaseManager.RemoteConfig.GetInt("piggy_bank_win_level_earn");
                ConfigData.PiggyBankMaxCoin = FirebaseManager.RemoteConfig.GetInt("piggy_bank_max_coin");
                // ConfigData.LevelShowEventStarRush = FirebaseManager.RemoteConfig.GetInt("level_show_event_star_rush");
                // ConfigData.LevelUnlockEventStarRush = FirebaseManager.RemoteConfig.GetInt("level_unlock_event_star_rush");
                // ConfigData.LevelUnlockLuckySpin = FirebaseManager.RemoteConfig.GetInt("level_unlock_lucky_spin");
                // ConfigData.LevelShowEventHiddenPictureIcon = FirebaseManager.RemoteConfig.GetInt("level_show_hidden_picture");
                // ConfigData.LevelUnlockEventHiddenPicture = FirebaseManager.RemoteConfig.GetInt("level_unlock_hidden_picture");
                ConfigData.LevelShowInter = FirebaseManager.RemoteConfig.GetInt("level_show_inter");
                ConfigData.LevelShowAdBreak = FirebaseManager.RemoteConfig.GetInt("level_show_ad_break");
                ConfigData.TimeShowInter = FirebaseManager.RemoteConfig.GetInt("time_show_inter");
                ConfigData.TimeShowAdBreak = FirebaseManager.RemoteConfig.GetInt("time_show_ad_break");
                ConfigData.TimeShowAppOpen = FirebaseManager.RemoteConfig.GetInt("time_show_app_open");
                ConfigData.LevelShowNativeAdInPopupWin = FirebaseManager.RemoteConfig.GetInt("level_show_native_in_win");
                // ConfigData.LevelCallToRate = FirebaseManager.RemoteConfig.GetInt("level_call_to_rate");

                // ConfigData.FreeVideoWin = FirebaseManager.RemoteConfig.GetInt("free_video_win");

                // Double
                ConfigData.CoilMaxSizeRatio = (float)FirebaseManager.RemoteConfig.GetDouble("coil_max_size_ratio");

                ConfigData.AddKeyFetchedOnce("remote_configs_fetched");

                Debug.LogError($"Fetch remote configs done!!!");
            }

            SaveData();
        }

        // public void LoadRemoteLevelData()
        // {
        //     Debug.LogError($"LoadRemoteLevelData = {ConfigData.RemoteLevelData}");
        //     try
        //     {
        //         if (!string.IsNullOrEmpty(ConfigData.RemoteLevelData))
        //         {
        //             Static.levelsUpdateDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigData.RemoteLevelData) ?? new Dictionary<string, string>();
        //         }
        //     }
        //     catch (Exception) { }
        // }

        public void LoadRemoteData()
        {
            if (!string.IsNullOrEmpty(ConfigData.RemoteData))
            {
                try
                {
                    var levelDataWithConfigs = JsonConvert.DeserializeObject<LevelDataWithConfigs>(ConfigData.RemoteData);
                    if (levelDataWithConfigs != null)
                    {
                        GameLogic.RemoteLevelConfig = levelDataWithConfigs.LevelConfig ?? new Dictionary<string, int>();
                        if (!string.IsNullOrEmpty(levelDataWithConfigs.LevelData))
                        {
                            GameLogic.RemoteLevelData = JsonConvert.DeserializeObject<Dictionary<string, string>>(levelDataWithConfigs.LevelData) ?? new Dictionary<string, string>();
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[!] LoadRemoteData exception: {e}");
                }
            }
        }
    }
}
