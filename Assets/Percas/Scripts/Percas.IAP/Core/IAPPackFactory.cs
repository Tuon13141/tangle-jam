using System.Collections.Generic;
using Percas.IAR;
using Percas.Data;
using Percas.UI;

namespace Percas.IAP
{
    public static class IAPPackFactory
    {
        public static Dictionary<string, IAPPackBase> IAPPackDict = new()
        {
            { "tf_pack_coin_1", new Coin1() },
            { "tf_pack_coin_2", new Coin2() },
            { "tf_pack_coin_3", new Coin3() },
            { "tf_pack_coin_4", new Coin4() },
            { "tf_pack_coin_5", new Coin5() },
            { "tf_pack_coin_6", new Coin6() },
            { "tf_remove_ads", new RemoveAds() },
            { "tf_pack_starter", new StarterPack() },
            { "tf_pack_small", new SmallPack() },
            { "tf_pack_medium", new MediumPack() },
            { "tf_pack_large", new LargePack() },
            { "tf_pack_piggy_bank", new PiggyBank() },
            { "tf_pack_heart_offer", new HeartOffer() },
        };

        public static IAPPackBase GetPack(string name)
        {
            if (IAPPackDict.TryGetValue(name, out IAPPackBase pack))
            {
                return pack;
            }
            return null;
        }
    }

    public class Coin1 : IAPPackBase
    {
        public Coin1()
        {
            AddReward(new RewardCoin(300, new LogCurrency("currency", "coin", "shop", "iap", "pack", "coin_1")));
        }

        public override void BuyPack()
        {
            SetCompletedAction(() =>
            {
                TrackingManager.OnPurchaseCoinPack1?.Invoke();
            });

            base.BuyPack();
        }
    }

    public class Coin2 : IAPPackBase
    {
        public Coin2()
        {
            AddReward(new RewardCoin(1300, new LogCurrency("currency", "coin", "shop", "iap", "pack", "coin_2")));
        }

        public override void BuyPack()
        {
            SetCompletedAction(() =>
            {
                TrackingManager.OnPurchaseCoinPack2?.Invoke();
            });

            base.BuyPack();
        }
    }

    public class Coin3 : IAPPackBase
    {
        public Coin3()
        {
            AddReward(new RewardCoin(3600, new LogCurrency("currency", "coin", "shop", "iap", "pack", "coin_3")));
        }

        public override void BuyPack()
        {
            SetCompletedAction(() =>
            {
                TrackingManager.OnPurchaseCoinPack3?.Invoke();
            });

            base.BuyPack();
        }
    }

    public class Coin4 : IAPPackBase
    {
        public Coin4()
        {
            AddReward(new RewardCoin(8000, new LogCurrency("currency", "coin", "shop", "iap", "pack", "coin_4")));
        }

        public override void BuyPack()
        {
            SetCompletedAction(() =>
            {
                TrackingManager.OnPurchaseCoinPack4?.Invoke();
            });

            base.BuyPack();
        }
    }

    public class Coin5 : IAPPackBase
    {
        public Coin5()
        {
            AddReward(new RewardCoin(16000, new LogCurrency("currency", "coin", "shop", "iap", "pack", "coin_5")));
        }

        public override void BuyPack()
        {
            SetCompletedAction(() =>
            {
                TrackingManager.OnPurchaseCoinPack5?.Invoke();
            });

            base.BuyPack();
        }
    }

    public class Coin6 : IAPPackBase
    {
        public Coin6()
        {
            AddReward(new RewardCoin(36000, new LogCurrency("currency", "coin", "shop", "iap", "pack", "coin_6")));
        }

        public override void BuyPack()
        {
            SetCompletedAction(() =>
            {
                TrackingManager.OnPurchaseCoinPack6?.Invoke();
            });

            base.BuyPack();
        }
    }

    public class RemoveAds : IAPPackBase
    {
        public RemoveAds()
        {
            AddReward(new RewardRemoveAds(1));
        }

        public override void BuyPack()
        {
            SetCompletedAction(() =>
            {
                TrackingManager.OnPurchaseRemoveAds?.Invoke();
            });

            base.BuyPack();
        }
    }

    public class StarterPack : IAPPackBase
    {
        public StarterPack()
        {
            AddReward(new RewardBoosterUndo(1, new LogCurrency("booster", $"{BoosterType.Undo}", "shop", "iap", "pack", "starter")));
            AddReward(new RewardBoosterAddSlots(1, new LogCurrency("booster", $"{BoosterType.AddSlots}", "shop", "iap", "pack", "starter")));
            AddReward(new RewardBoosterShuffle(1, new LogCurrency("booster", $"{BoosterType.Shuffle}", "shop", "iap", "pack", "starter")));
            AddReward(new RewardBoosterClear(1, new LogCurrency("booster", $"{BoosterType.Clear}", "shop", "iap", "pack", "starter")));

            AddReward(new RewardCoin(400, new LogCurrency("currency", "coin", "shop", "iap", "pack", "starter")));

            AddReward(new RewardInfiniteLive(7200, new LogCurrency("energy", "infinite_live", "shop", "iap", "pack", "starter"))); // 2h = 120m = 7200s
        }

        public override void BuyPack()
        {
            SetCompletedAction(() =>
            {
                TrackingManager.OnPurchaseStarterPack?.Invoke();
                PlayerDataManager.OnPurchaseStarterPack?.Invoke();
                UIStarterPack.OnShow?.Invoke();
            });

            base.BuyPack();
        }
    }

    public class SmallPack : IAPPackBase
    {
        public SmallPack()
        {
            AddReward(new RewardBoosterUndo(3, new LogCurrency("booster", $"{BoosterType.Undo}", "shop", "iap", "pack", "small")));
            AddReward(new RewardBoosterAddSlots(1, new LogCurrency("booster", $"{BoosterType.AddSlots}", "shop", "iap", "pack", "small")));
            AddReward(new RewardBoosterShuffle(5, new LogCurrency("booster", $"{BoosterType.Shuffle}", "shop", "iap", "pack", "small")));
            AddReward(new RewardBoosterClear(1, new LogCurrency("booster", $"{BoosterType.Clear}", "shop", "iap", "pack", "small")));

            AddReward(new RewardCoin(1600, new LogCurrency("currency", "coin", "shop", "iap", "pack", "small")));

            AddReward(new RewardInfiniteLive(21600, new LogCurrency("energy", "infinite_live", "shop", "iap", "pack", "small"))); // 6h = 360m = 21600s

            AddReward(new RewardRemoveAds(1));
        }

        public override void BuyPack()
        {
            SetCompletedAction(() =>
            {
                TrackingManager.OnPurchaseSmallPack?.Invoke();
            });

            base.BuyPack();
        }
    }

    public class MediumPack : IAPPackBase
    {
        public MediumPack()
        {
            AddReward(new RewardBoosterUndo(6, new LogCurrency("booster", $"{BoosterType.Undo}", "shop", "iap", "pack", "medium")));
            AddReward(new RewardBoosterAddSlots(2, new LogCurrency("booster", $"{BoosterType.AddSlots}", "shop", "iap", "pack", "medium")));
            AddReward(new RewardBoosterShuffle(10, new LogCurrency("booster", $"{BoosterType.Shuffle}", "shop", "iap", "pack", "medium")));
            AddReward(new RewardBoosterClear(2, new LogCurrency("booster", $"{BoosterType.Clear}", "shop", "iap", "pack", "medium")));

            AddReward(new RewardCoin(3600, new LogCurrency("currency", "coin", "shop", "iap", "pack", "medium")));

            AddReward(new RewardInfiniteLive(43200, new LogCurrency("energy", "infinite_live", "shop", "iap", "pack", "medium"))); // 12h

            AddReward(new RewardRemoveAds(1));
        }

        public override void BuyPack()
        {
            SetCompletedAction(() =>
            {
                TrackingManager.OnPurchaseMediumPack?.Invoke();
            });

            base.BuyPack();
        }
    }

    public class LargePack : IAPPackBase
    {
        public LargePack()
        {
            AddReward(new RewardBoosterUndo(12, new LogCurrency("booster", $"{BoosterType.Undo}", "shop", "iap", "pack", "large")));
            AddReward(new RewardBoosterAddSlots(8, new LogCurrency("booster", $"{BoosterType.AddSlots}", "shop", "iap", "pack", "large")));
            AddReward(new RewardBoosterShuffle(24, new LogCurrency("booster", $"{BoosterType.Shuffle}", "shop", "iap", "pack", "large")));
            AddReward(new RewardBoosterClear(8, new LogCurrency("booster", $"{BoosterType.Clear}", "shop", "iap", "pack", "large")));

            AddReward(new RewardCoin(8000, new LogCurrency("currency", "coin", "shop", "iap", "pack", "large")));

            AddReward(new RewardInfiniteLive(86400, new LogCurrency("energy", "infinite_live", "shop", "iap", "pack", "large"))); // 24h

            AddReward(new RewardRemoveAds(1));
        }

        public override void BuyPack()
        {
            SetCompletedAction(() =>
            {
                TrackingManager.OnPurchaseLargePack?.Invoke();
            });

            base.BuyPack();
        }
    }

    public class PiggyBank : IAPPackBase
    {
        public PiggyBank()
        {
            AddReward(new RewardCoin(GameLogic.PiggyBankMaxCoin, new LogCurrency("currency", "coin", "piggy_bank", "iap", "pack", "piggy_bank")));
        }

        public override void BuyPack()
        {
            SetCompletedAction(() =>
            {
                TrackingManager.OnPurchasePiggyBank?.Invoke();
            });

            base.BuyPack();

            PlayerDataManager.OnResetPiggyBank?.Invoke();
            ButtonPiggyBank.OnUpdateButtonText?.Invoke();
        }
    }

    public class HeartOffer : IAPPackBase
    {
        public HeartOffer()
        {
            AddReward(new RewardInfiniteLive(GameLogic.UnlimitedLifeIAPTimeLimit, new LogCurrency("energy", "infinite_live", "heart_offers", "iap", "pack", "heart_offers")));
        }

        public override void BuyPack()
        {
            SetCompletedAction(() =>
            {
                TrackingManager.OnPurchaseHeartOffer?.Invoke();
            });

            base.BuyPack();

            UICurrencyManager.OnShowLiveGain?.Invoke(true, GameLogic.UnlimitedLifeIAPTimeLimit);
        }
    }
}
