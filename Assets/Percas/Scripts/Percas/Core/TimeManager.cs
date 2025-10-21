using System;
using UnityEngine;
using Percas.Data;

namespace Percas
{
    public class TimeManager : MonoBehaviour
    {
        public static Action OnTick;
        public static Action OnResetDailyTime;

        private void Awake()
        {
            OnResetDailyTime += ResetDailyTime;
        }

        private void OnDestroy()
        {
            OnResetDailyTime -= ResetDailyTime;
        }

        private void Start()
        {
            InvokeRepeating(nameof(OnCheckTimer), 0f, 1f);
        }

        private void OnCheckTimer()
        {
            OnTick?.Invoke();
        }

        private void ResetDailyTime()
        {
            DateTime endToday = DateTime.UtcNow.EndOfDay();
            if (!PlayerPrefs.HasKey(Const.KEY_DAILY_TIME_RESET))
            {
                ResetDailyServices();
                PlayerPrefs.SetString(Const.KEY_DAILY_TIME_RESET, endToday.ToString());
                return;
            }
            try
            {
                TimeSpan remainingTime = TimeHelper.ParseIsoString(PlayerPrefs.GetString(Const.KEY_DAILY_TIME_RESET)) - DateTime.UtcNow;
                if (remainingTime.TotalSeconds < 0)
                {
                    ResetDailyServices();
                    PlayerPrefs.SetString(Const.KEY_DAILY_TIME_RESET, endToday.ToString());
                }
            }
            catch (Exception) { }
        }

        private void ResetDailyServices()
        {
            PlayerDataManager.PlayerData.ResetDailyRewards();
            PlayerDataManager.PlayerData.ResetDailySpins();
            PlayerDataManager.OnSave?.Invoke();
            HiddenPictureManager.Data.ResetFreeKeys();
        }
    }
}
