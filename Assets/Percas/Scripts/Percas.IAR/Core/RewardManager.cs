using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using PercasSDK;

namespace Percas.IAR
{
    public class RewardManager : MonoBehaviour
    {
        public static RewardData RewardData = new();

        public static Action OnSave;
        public static Action<List<Reward>> OnSetRewards;
        public static Action<Action<List<Reward>>> OnGetRewards;

        public static bool LoadDataDone { get; private set; }

        private void Awake()
        {
            InitData();

            OnSave += SaveData;
            OnSetRewards += SetRewards;
            OnGetRewards += GetRewards;
        }

        private void OnDestroy()
        {
            OnSave -= SaveData;
            OnSetRewards -= SetRewards;
            OnGetRewards -= GetRewards;
        }

        private void InitData()
        {
            if (!PlayerPrefs.HasKey(Const.KEY_REWARD_DATA))
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
                string encryptedJson = PlayerPrefs.GetString(Const.KEY_REWARD_DATA);
                string jsonData = Helpers.Decrypt(encryptedJson);
                RewardData data = JsonUtility.FromJson<RewardData>(jsonData);
                RewardData = data;
            }
            catch (Exception)
            {
                RewardData = new();
            }
        }

        private void SaveData()
        {
            string jsonData = JsonUtility.ToJson(RewardData);
            string encryptedJson = Helpers.Encrypt(jsonData);
            PlayerPrefs.SetString(Const.KEY_REWARD_DATA, encryptedJson);
            DataManager.Instance.WriteToLocal(Const.KEY_REWARD_DATA, jsonData);
        }

        private void SetRewards(List<Reward> rewards)
        {
            if (rewards == null) return;
            if (rewards.Count <= 0) return;
            foreach (Reward reward in rewards)
            {
                RewardData.AddReward(reward);
            }
            OnSave?.Invoke();
        }

        private void GetRewards(Action<List<Reward>> onCompleted)
        {
            if (RewardData.Rewards == null) return;
            if (RewardData.Rewards.Count <= 0) return;
            //List<Reward> groupAndSumRewards = RewardData.Rewards
            //    .GroupBy(item => item.RewardType)
            //    .Select(group => new Reward(group.Key, group.Sum(item => item.RewardAmount)))
            //    .ToList();
            foreach (Reward reward in RewardData.Rewards)
            {
                try
                {
                    RewardBase rewardBase = RewardFactory.GetRewardBase(reward.RewardType);
                    rewardBase.Amount = reward.RewardAmount;
                    rewardBase.Log = reward.Log;
                    rewardBase.GetReward();
                }
                catch (Exception)
                {
                    continue;
                }
            }
            onCompleted?.Invoke(RewardData.Rewards);
            RewardData.Clear();
            OnSave?.Invoke();
        }
    }
}
