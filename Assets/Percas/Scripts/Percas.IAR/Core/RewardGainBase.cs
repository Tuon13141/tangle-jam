using UnityEngine;
using Percas.Data;

namespace Percas.IAR
{
    public interface IRewardGainHandler
    {
        public void GainReward();
    }

    public abstract class RewardGainBase : IRewardGainHandler
    {
        public RewardType Type;
        public int Amount;
        public Vector3 SpawnPos;
        public LogCurrency Log;

        public RewardGainBase() { }

        public RewardGainBase(int amount, Vector3 spawnPos)
        {
            this.Amount = amount;
            this.SpawnPos = spawnPos;
        }

        public RewardGainBase(int amount, Vector3 spawnPos, LogCurrency log)
        {
            this.Amount = amount;
            this.SpawnPos = spawnPos;
            this.Log = log;
        }

        public RewardGainBase(int amount)
        {
            this.Amount = amount;
        }

        public abstract void GainReward();
    }
}
