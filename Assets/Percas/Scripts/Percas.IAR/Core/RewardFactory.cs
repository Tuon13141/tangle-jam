using System.Collections.Generic;
using Percas.Data;
using Percas.Live;

namespace Percas.IAR
{
    public static class RewardFactory
    {
        public static Dictionary<RewardType, RewardBase> RewardDict = new()
        {
            { RewardType.Coin, new RewardCoin() },
            { RewardType.RemoveAds, new RewardRemoveAds() },
            { RewardType.Live, new RewardLive() },
            { RewardType.InfiniteLive, new RewardInfiniteLive() },
            { RewardType.BoosterUndo, new RewardBoosterUndo() },
            { RewardType.BoosterAddSlots, new RewardBoosterAddSlots() },
            { RewardType.BoosterShuffle, new RewardBoosterShuffle() },
            { RewardType.BoosterClear, new RewardBoosterClear() },
            { RewardType.Coil, new RewardCoil() },
        };

        public static RewardBase GetRewardBase(RewardType rewardType)
        {
            if (RewardDict.TryGetValue(rewardType, out RewardBase reward))
            {
                return reward;
            }
            return null;
        }
    }

    public class RewardCoin : RewardBase
    {
        public RewardCoin() { }

        public RewardCoin(int amount) : base(amount)
        {
            this.Type = RewardType.Coin;
        }

        public RewardCoin(int amount, LogCurrency log) : base(amount, log)
        {
            this.Type = RewardType.Coin;
        }

        public override void GetReward()
        {
            PlayerDataManager.OnEarnCoin?.Invoke(Amount, Log);
        }
    }

    public class RewardCoil : RewardBase
    {
        public RewardCoil() { }

        public RewardCoil(int amount) : base(amount)
        {
            this.Type = RewardType.Coil;
        }

        public RewardCoil(int amount, LogCurrency log) : base(amount, log)
        {
            this.Type = RewardType.Coil;
        }

        public override void GetReward()
        {
            PlayerDataManager.OnEarnCoil?.Invoke(Amount, Log);
        }
    }

    public class RewardPin : RewardBase
    {
        public RewardPin() { }

        public RewardPin(int amount) : base(amount)
        {
            this.Type = RewardType.Pin;
        }

        public RewardPin(int amount, LogCurrency log) : base(amount, log)
        {
            this.Type = RewardType.Pin;
        }

        public override void GetReward()
        {
            PlayerDataManager.OnEarnPin?.Invoke(Amount, Log);
        }
    }

    public class RewardRemoveAds : RewardBase
    {
        public RewardRemoveAds(int amount) : base(amount)
        {
            this.Type = RewardType.RemoveAds;
        }

        public RewardRemoveAds() { }

        public override void GetReward()
        {
            IAA.IAAManager.RemoveAds();
        }
    }

    public class RewardLive : RewardBase
    {
        public RewardLive() { }

        public RewardLive(int amount) : base(amount)
        {
            this.Type = RewardType.Live;
        }

        public RewardLive(int amount, LogCurrency log) : base(amount, log)
        {
            this.Type = RewardType.Live;
        }

        public override void GetReward()
        {
            LiveManager.OnRefillLives?.Invoke(Amount, Log);
        }
    }

    public class RewardInfiniteLive : RewardBase
    {
        public RewardInfiniteLive(int amount) : base(amount)
        {
            this.Type = RewardType.InfiniteLive;
        }

        public RewardInfiniteLive(int amount, LogCurrency log) : base(amount, log)
        {
            this.Type = RewardType.InfiniteLive;
        }

        public RewardInfiniteLive() { }

        public override void GetReward()
        {
            LiveManager.OnEarnInfiniteLives?.Invoke(Amount, Log);
        }
    }

    public class RewardBoosterUndo : RewardBase
    {
        public RewardBoosterUndo() { }

        public RewardBoosterUndo(int amount) : base(amount)
        {
            this.Type = RewardType.BoosterUndo;
        }

        public RewardBoosterUndo(int amount, LogCurrency log) : base(amount, log)
        {
            this.Type = RewardType.BoosterUndo;
        }

        public override void GetReward()
        {
            BoosterManager.OnEarnBooster?.Invoke(BoosterType.Undo, Amount, Log);
        }
    }

    public class RewardBoosterAddSlots : RewardBase
    {
        public RewardBoosterAddSlots() { }

        public RewardBoosterAddSlots(int amount) : base(amount)
        {
            this.Type = RewardType.BoosterAddSlots;
        }

        public RewardBoosterAddSlots(int amount, LogCurrency log) : base(amount, log)
        {
            this.Type = RewardType.BoosterAddSlots;
        }

        public override void GetReward()
        {
            BoosterManager.OnEarnBooster?.Invoke(BoosterType.AddSlots, Amount, Log);
        }
    }

    public class RewardBoosterShuffle : RewardBase
    {
        public RewardBoosterShuffle() { }

        public RewardBoosterShuffle(int amount) : base(amount)
        {
            this.Type = RewardType.BoosterShuffle;
        }

        public RewardBoosterShuffle(int amount, LogCurrency log) : base(amount, log)
        {
            this.Type = RewardType.BoosterShuffle;
        }

        public override void GetReward()
        {
            BoosterManager.OnEarnBooster?.Invoke(BoosterType.Shuffle, Amount, Log);
        }
    }

    public class RewardBoosterClear : RewardBase
    {
        public RewardBoosterClear() { }

        public RewardBoosterClear(int amount) : base(amount)
        {
            this.Type = RewardType.BoosterClear;
        }

        public RewardBoosterClear(int amount, LogCurrency log) : base(amount, log)
        {
            this.Type = RewardType.BoosterClear;
        }

        public override void GetReward()
        {
            BoosterManager.OnEarnBooster?.Invoke(BoosterType.Clear, Amount, Log);
        }
    }
}
