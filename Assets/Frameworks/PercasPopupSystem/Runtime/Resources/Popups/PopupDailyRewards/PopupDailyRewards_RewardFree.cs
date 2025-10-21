using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Percas.Data;
using Percas.UI;
using Percas.IAR;

namespace Percas
{
    public class PopupDailyRewards_RewardFree : MonoBehaviour, IActivatable
    {
        [SerializeField] int index = 0;
        [SerializeField] int coinReward = 0;
        [SerializeField] TMP_Text textCoinReward;
        [SerializeField] GameObject buttonActive, buttonInactive;
        [SerializeField] ButtonBase buttonCollect;

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
            buttonCollect.SetPointerClickEvent(Collect);
        }

        public void Deactivate() { }

        private void UpdateUI()
        {
            textCoinReward.text = $"X{coinReward}";
            buttonActive.SetActive(GameLogic.CurrentDailyRewardIndex == index);
            buttonInactive.SetActive(GameLogic.CurrentDailyRewardIndex != index);
        }

        private void Collect()
        {
            List<Reward> rewards = new()
        {
            new Reward(RewardType.Coin, coinReward, new LogCurrency("currency", "coin", "daily_rewards", "non_iap", "feature", "collect_free")),
        };
            RewardManager.OnSetRewards?.Invoke(rewards);
            RewardManager.OnGetRewards?.Invoke((rwds) =>
            {
                PlayerDataManager.PlayerData.DailyRewardIndex.Set(index + 1);
                PlayerDataManager.OnSave?.Invoke();
                OnUpdateUI?.Invoke();
                PopupDailyRewards_RewardAd.OnUpdateUI?.Invoke();
            });
        }
    }
}
