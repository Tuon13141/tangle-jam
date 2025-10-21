using System;
using System.Collections.Generic;
using Percas.Data;

namespace Percas.IAR
{
    [Serializable]
    public class Reward
    {
        public RewardType RewardType;
        public int RewardAmount = 0;
        public LogCurrency Log;

        public Reward() { }

        public Reward(RewardType rewardType, int rewardAmount, LogCurrency log)
        {
            this.RewardType = rewardType;
            this.RewardAmount = rewardAmount;
            this.Log = log;
        }

        public void SetAmount(int value)
        {
            this.RewardAmount = value;
        }

        public void AddAmount(int valueToAdd)
        {
            this.RewardAmount += valueToAdd;
        }

        public static implicit operator List<object>(Reward v)
        {
            throw new NotImplementedException();
        }
    }
}
