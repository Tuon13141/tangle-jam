using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Percas.Data;
using Percas.UI;
using Percas.IAR;

namespace Percas
{
    public class PopupDailyRewards_RewardAd : MonoBehaviour, IActivatable
    {
        [SerializeField] int index = 0;
        [SerializeField] int coinReward = 0;
        [SerializeField] TMP_Text textCoinReward, textButtonInactive;
        [SerializeField] GameObject buttonActive, buttonInactive;
        [SerializeField] ButtonWatchVideoAd buttonWatchVideoAd;

        public static Action OnUpdateUI;

        private void Awake()
        {
            OnUpdateUI += UpdateUI;
        }

        private void OnDestroy()
        {
            OnUpdateUI -= UpdateUI;
        }

        public void Activate()
        {
            UpdateUI();
            buttonWatchVideoAd.skipVideo = false;
            buttonWatchVideoAd.onStart = OnStartAction;
            buttonWatchVideoAd.onCompleted = OnCompleted;
        }

        public void Deactivate() { }

        private void UpdateUI()
        {
            textCoinReward.text = index == 1 ? $"15m" : $"X{coinReward * GameLogic.DailyRewardRate}";
            textButtonInactive.text = GameLogic.CurrentDailyRewardIndex > index ? $"RECEIVED" : $"FREE";
            buttonActive.SetActive(GameLogic.CurrentDailyRewardIndex == index);
            buttonInactive.SetActive(GameLogic.CurrentDailyRewardIndex != index);
        }

        private void OnStartAction(Action<bool> onCallback)
        {
            onCallback?.Invoke(true);
        }

        private void OnCompleted()
        {
            List<Reward> rewards = new();
            if (index == 1)
            {
                rewards.Add(new Reward(RewardType.InfiniteLive, 900, new LogCurrency("energy", "infinite_live", "daily_rewards", "non_iap", "ads", "rwd_ads")));
            }
            else
            {
                rewards.Add(new Reward(RewardType.Coin, coinReward * GameLogic.DailyRewardRate, new LogCurrency("currency", "coin", "daily_rewards", "non_iap", "ads", "rwd_ads")));
                if (index == 4)
                {
                    rewards.Add(new Reward(RewardType.Coil, 100, new LogCurrency("currency", "coil", "daily_rewards", "non_iap", "ads", "rwd_ads")));
                    rewards.Add(new Reward(RewardType.InfiniteLive, 900, new LogCurrency("energy", "infinite_live", "daily_rewards", "non_iap", "ads", "rwd_ads")));
                }
            }
            RewardManager.OnSetRewards?.Invoke(rewards);
            RewardManager.OnGetRewards?.Invoke((rwds) =>
            {
                PlayerDataManager.PlayerData.DailyRewardIndex.Set(index + 1);
                PlayerDataManager.OnSave?.Invoke();
                PopupDailyRewards_RewardFree.OnUpdateUI?.Invoke();
                OnUpdateUI?.Invoke();
                ButtonDailyRewards.OnUpdateNoti?.Invoke();
                if (index == 4)
                {
                    ButtonDailyRewards.OnClosePopup?.Invoke();
                }
            });
        }
    }
}
