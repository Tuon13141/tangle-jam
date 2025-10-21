using UnityEngine;
using Percas.Data;
using Percas.Live;

namespace Percas.IAR
{
    public class RewardGainCoin : RewardGainBase
    {
        public RewardGainCoin() { }

        public RewardGainCoin(int amount, Vector3 spawnPos) : base(amount, spawnPos)
        {
            this.Type = RewardType.Coin;
        }

        public RewardGainCoin(int amount, Vector3 spawnPos, LogCurrency log) : base(amount, spawnPos, log)
        {
            this.Type = RewardType.Coin;
        }

        public override void GainReward()
        {
            PlayerDataManager.OnEarnCoin?.Invoke(Amount, Log);
        }
    }

    public class RewardGainCoil : RewardGainBase
    {
        public RewardGainCoil() { }

        public RewardGainCoil(int amount, Vector3 spawnPos) : base(amount, spawnPos)
        {
            this.Type = RewardType.Coil;
        }

        public RewardGainCoil(int amount, Vector3 spawnPos, LogCurrency log) : base(amount, spawnPos, log)
        {
            this.Type = RewardType.Coil;
        }

        public override void GainReward()
        {
            PlayerDataManager.OnEarnCoil?.Invoke(Amount, Log);
        }
    }

    public class RewardGainPin : RewardGainBase
    {
        public RewardGainPin() { }

        public RewardGainPin(int amount, Vector3 spawnPos) : base(amount, spawnPos)
        {
            this.Type = RewardType.Pin;
        }

        public RewardGainPin(int amount, Vector3 spawnPos, LogCurrency log) : base(amount, spawnPos, log)
        {
            this.Type = RewardType.Pin;
        }

        public override void GainReward()
        {
            PlayerDataManager.OnEarnPin?.Invoke(Amount, Log);
        }
    }

    public class RewardGainInfiniteLive : RewardGainBase
    {
        public RewardGainInfiniteLive() { }

        public RewardGainInfiniteLive(int amount, Vector3 spawnPos) : base(amount, spawnPos)
        {
            this.Type = RewardType.InfiniteLive;
        }

        public RewardGainInfiniteLive(int amount, Vector3 spawnPos, LogCurrency log) : base(amount, spawnPos, log)
        {
            this.Type = RewardType.InfiniteLive;
        }

        public override void GainReward()
        {
            LiveManager.OnEarnInfiniteLives?.Invoke(Amount, Log);
        }
    }

    public class RewardGainBoosterUndo : RewardGainBase
    {
        public RewardGainBoosterUndo() { }

        public RewardGainBoosterUndo(int amount, Vector3 spawnPos) : base(amount, spawnPos)
        {
            this.Type = RewardType.BoosterUndo;
        }

        public RewardGainBoosterUndo(int amount, Vector3 spawnPos, LogCurrency log) : base(amount, spawnPos, log)
        {
            this.Type = RewardType.BoosterUndo;
        }

        public override void GainReward()
        {
            BoosterManager.OnEarnBooster?.Invoke(BoosterType.Undo, Amount, Log);
        }
    }

    public class RewardGainBoosterAddSlots : RewardGainBase
    {
        public RewardGainBoosterAddSlots() { }

        public RewardGainBoosterAddSlots(int amount, Vector3 spawnPos) : base(amount, spawnPos)
        {
            this.Type = RewardType.BoosterAddSlots;
        }

        public RewardGainBoosterAddSlots(int amount, Vector3 spawnPos, LogCurrency log) : base(amount, spawnPos, log)
        {
            this.Type = RewardType.BoosterAddSlots;
        }

        public override void GainReward()
        {
            BoosterManager.OnEarnBooster?.Invoke(BoosterType.AddSlots, Amount, Log);
        }
    }

    public class RewardGainBoosterClear : RewardGainBase
    {
        public RewardGainBoosterClear() { }

        public RewardGainBoosterClear(int amount, Vector3 spawnPos) : base(amount, spawnPos)
        {
            this.Type = RewardType.BoosterClear;
        }

        public RewardGainBoosterClear(int amount, Vector3 spawnPos, LogCurrency log) : base(amount, spawnPos, log)
        {
            this.Type = RewardType.BoosterClear;
        }

        public override void GainReward()
        {
            BoosterManager.OnEarnBooster?.Invoke(BoosterType.Clear, Amount, Log);
        }
    }
}
