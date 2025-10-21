using System.Collections.Generic;

namespace Percas.IAR
{
    public class RewardData
    {
        public List<Reward> Rewards = new();

        public void AddReward(Reward reward)
        {
            this.Rewards.Add(reward);
        }

        public void Clear()
        {
            this.Rewards.Clear();
        }
    }
}
