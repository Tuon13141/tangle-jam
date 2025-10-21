using UnityEngine;
using Percas.Data;

namespace Percas.IAR
{
    public interface IRewardHandler
    {
        public int GetAmount();
        public void GetReward();
    }

    public class RewardBase : IRewardHandler
    {
        public RewardType Type;
        public int Amount;
        public LogCurrency Log;

        public RewardBase() { }

        public RewardBase(int amount)
        {
            this.Amount = amount;
        }

        public RewardBase(int amount, LogCurrency log)
        {
            this.Amount = amount;
            this.Log = log;
        }

        public int GetAmount()
        {
            return Amount;
        }

        public virtual void GetReward()
        {
            throw new System.NotImplementedException();
        }
    }
}
