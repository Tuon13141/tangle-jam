using System;
using UnityEngine;
using Percas.UI;
using Sonat;

namespace Percas.Data
{
    public class BoosterManager : MonoBehaviour
    {
        public static BoosterData BoosterData = new();

        public static Action OnSave;

        public static Action<BoosterType, int, LogCurrency> OnEarnBooster;
        public static Action<BoosterType, int, LogCurrency> OnSpendBooster;
        public static Action<BoosterType, Action, Action, LogCurrency> OnUseBooster;

        public static Func<BoosterType, int> OnGetBoosterAmount;

        public static bool LoadDataDone { get; private set; }

        private void Awake()
        {
            InitData();

            OnSave += SaveData;

            OnEarnBooster += EarnBooster;
            OnSpendBooster += SpendBooster;
            OnUseBooster += UseBooster;

            OnGetBoosterAmount += GetBoosterAmount;
        }

        private void OnDestroy()
        {
            OnSave -= SaveData;

            OnEarnBooster -= EarnBooster;
            OnSpendBooster -= SpendBooster;
            OnUseBooster -= UseBooster;

            OnGetBoosterAmount -= GetBoosterAmount;
        }

        private void InitData()
        {
            if (!PlayerPrefs.HasKey(Const.KEY_BOOSTER_DATA))
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
                string encryptedJson = PlayerPrefs.GetString(Const.KEY_BOOSTER_DATA);
                string jsonData = Helpers.Decrypt(encryptedJson);
                BoosterData data = JsonUtility.FromJson<BoosterData>(jsonData);
                BoosterData = data;
            }
            catch (Exception)
            {
                BoosterData = new();
            }
        }

        private void SaveData()
        {
            string jsonData = JsonUtility.ToJson(BoosterData);
            string encryptedJson = Helpers.Encrypt(jsonData);
            PlayerPrefs.SetString(Const.KEY_BOOSTER_DATA, encryptedJson);
            DataManager.Instance.WriteToLocal(Const.KEY_BOOSTER_DATA, jsonData);
        }

        private int GetBoosterAmount(BoosterType boosterType)
        {
            int boosterAmount = 0;

            switch (boosterType)
            {
                case BoosterType.Undo:
                    boosterAmount = BoosterData.BoosterUndo;
                    break;

                case BoosterType.AddSlots:
                    boosterAmount = BoosterData.BoosterAddSlots;
                    break;

                case BoosterType.Shuffle:
                    boosterAmount = BoosterData.BoosterShuffle;
                    break;

                case BoosterType.Clear:
                    boosterAmount = BoosterData.BoosterClear;
                    break;
            }

            return boosterAmount;
        }

        private void EarnBooster(BoosterType boosterType, int valueToEarn, LogCurrency logCurrency)
        {
            switch (boosterType)
            {
                case BoosterType.Undo:
                    BoosterData.BoosterUndo += valueToEarn;
                    break;

                case BoosterType.AddSlots:
                    BoosterData.BoosterAddSlots += valueToEarn;
                    break;

                case BoosterType.Shuffle:
                    BoosterData.BoosterShuffle += valueToEarn;
                    break;

                case BoosterType.Clear:
                    BoosterData.BoosterClear += valueToEarn;
                    break;
            }
            if (logCurrency != null)
            {
                var log = new SonatLogEarnVirtualCurrency()
                {
                    virtual_currency_name = logCurrency.name,
                    virtual_currency_type = logCurrency.type,
                    value = valueToEarn,
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
            UICurrencyManager.OnShowBoosterGain?.Invoke(boosterType, valueToEarn);
            ButtonUseBooster.OnUpdateUI?.Invoke();
        }

        private void SpendBooster(BoosterType boosterType, int valueToSpend, LogCurrency logCurrency)
        {
            switch (boosterType)
            {
                case BoosterType.Undo:
                    BoosterData.BoosterUndo = Math.Max(0, BoosterData.BoosterUndo - valueToSpend);
                    break;

                case BoosterType.AddSlots:
                    BoosterData.BoosterAddSlots = Math.Max(0, BoosterData.BoosterAddSlots - valueToSpend);
                    break;

                case BoosterType.Shuffle:
                    BoosterData.BoosterShuffle = Math.Max(0, BoosterData.BoosterShuffle - valueToSpend);
                    break;

                case BoosterType.Clear:
                    BoosterData.BoosterClear = Math.Max(0, BoosterData.BoosterClear - valueToSpend);
                    break;
            }
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
            PlayerDataManager.OnUpdateLevelUseBoosterCount?.Invoke();
        }

        private bool CanUseBooster(BoosterType boosterType)
        {
            int boosterAmount = 0;

            switch (boosterType)
            {
                case BoosterType.Undo:
                    boosterAmount = BoosterData.BoosterUndo;
                    break;

                case BoosterType.AddSlots:
                    boosterAmount = BoosterData.BoosterAddSlots;
                    break;

                case BoosterType.Shuffle:
                    boosterAmount = BoosterData.BoosterShuffle;
                    break;

                case BoosterType.Clear:
                    boosterAmount = BoosterData.BoosterClear;
                    break;
            }

            return boosterAmount >= 1;
        }

        private void UseBooster(BoosterType boosterType, Action onCompleted, Action onError, LogCurrency logCurrency)
        {
            if (!CanUseBooster(boosterType))
            {
                onError?.Invoke();
                return;
            }
            OnSpendBooster?.Invoke(boosterType, 1, logCurrency);
            //TrackingManager.OnSpendBooster?.Invoke(boosterType.ToString());
            onCompleted?.Invoke();
        }
    }
}
