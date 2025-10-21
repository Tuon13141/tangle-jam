using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Percas.IAR
{
    public class RewardGainController : MonoBehaviour
    {
        private readonly Queue<RewardGainBase> rewardGains = new();

        public static Action OnStartGaining;
        public static Action<RewardGainBase> OnAddRewardGain;

        private void Awake()
        {
            OnStartGaining += StartGaining;
            OnAddRewardGain += AddRewardGain;
        }

        private void OnDestroy()
        {
            OnStartGaining -= StartGaining;
            OnAddRewardGain -= AddRewardGain;
        }

        private void StartGaining()
        {
            StartCoroutine(OnGain());
        }

        private IEnumerator OnGain()
        {
            if (rewardGains.Count > 0)
            {
                GameLogic.RewardsGaining = true;
            }
            while (rewardGains.Count > 0)
            {
                rewardGains.Dequeue().GainReward();
                if (rewardGains.Count <= 0) GameLogic.RewardsGaining = false;
                yield return new WaitForSeconds(0.3f);
            }
        }

        private void AddRewardGain(RewardGainBase rewardGain)
        {
            if (rewardGain.Amount <= 0) return;
            foreach (var reward in rewardGains)
            {
                if (reward.Type == rewardGain.Type)
                {
                    reward.Amount += rewardGain.Amount;
                    return;
                }
            }
            rewardGains.Enqueue(rewardGain);
        }
    }
}
