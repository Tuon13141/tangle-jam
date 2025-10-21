using System;
using System.Collections.Generic;
using Percas.IAR;

namespace Percas.IAP
{
    public class IAPPackBase
    {
        public List<RewardBase> Rewards = new();

        public Action OnCompleted;

        public void AddReward(RewardBase reward)
        {
            this.Rewards.Add(reward);
        }

        public void SetCompletedAction(Action onCompleted)
        {
            this.OnCompleted = onCompleted;
        }

        public virtual void BuyPack()
        {
            List<Reward> rewards = new();
            foreach (var item in Rewards)
            {
                rewards.Add(new Reward(item.Type, item.Amount, item.Log));
            }
            RewardManager.OnSetRewards?.Invoke(rewards);
            RewardManager.OnGetRewards?.Invoke((rewards) =>
            {
                OnCompleted?.Invoke();
            });
        }
    }
}
