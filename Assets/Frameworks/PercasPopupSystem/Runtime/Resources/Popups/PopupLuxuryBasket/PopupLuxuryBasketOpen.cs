using System;
using UnityEngine;
using Percas.Data;
using Percas.UI;
using Percas.IAR;

namespace Percas
{
    public class PopupLuxuryBasketOpenArgs
    {
        public int targetValue;

        public PopupLuxuryBasketOpenArgs(int targetValue)
        {
            this.targetValue = targetValue;
        }
    }

    public class PopupLuxuryBasketOpen : PopupBase
    {
        [Header("UI")]
        [SerializeField] ButtonBase buttonClosePopup;
        [SerializeField] ButtonBase buttonClaim;
        [SerializeField] ButtonWatchVideoAd buttonX2;

        private int targetValue = 50;

        private void OnStart()
        {
            buttonClosePopup.SetPointerClickEvent(Close);
            buttonClaim.SetPointerClickEvent(Claim);

            buttonX2.skipVideo = false;
            buttonX2.onStart = OnStartWatch;
            buttonX2.onCompleted = OnCompletedWatch;
        }

        private void OnHide(Action callback = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.LuxuryBasketOpen, callback);
        }

        private void Close()
        {
            OnHide();
        }

        private void Claim()
        {
            PlayerDataManager.OnSpendPin?.Invoke(targetValue, (n) =>
            {
                if (n)
                {
                    OnHide(() =>
                    {
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin(120, Vector3.zero, new LogCurrency("currency", "coin", "luxury_basket", "non_iap", "feature", "luxury_basket_completed")));
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterUndo(1, Vector3.zero, new LogCurrency("booster", $"{BoosterType.Undo}", "luxury_basket", "non_iap", "feature", "luxury_basket_completed")));
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterAddSlots(1, Vector3.zero, new LogCurrency("booster", $"{BoosterType.AddSlots}", "luxury_basket", "non_iap", "feature", "luxury_basket_completed")));
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterClear(1, Vector3.zero, new LogCurrency("booster", $"{BoosterType.Clear}", "luxury_basket", "non_iap", "feature", "luxury_basket_completed")));
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainInfiniteLive(900, Vector3.zero, new LogCurrency("energy", "infinite_live", "luxury_basket", "non_iap", "feature", "luxury_basket_completed"))); // 900 = 15m
                        RewardGainController.OnStartGaining?.Invoke();
                        PlayerDataManager.OnOpenBasket?.Invoke();
                        ButtonLuxuryBasket.OnUpdateNoti?.Invoke();
                        ButtonLuxuryBasket.OnUpdateButtonText?.Invoke();
                    });
                }
                else
                {
                    ActionEvent.OnShowToast?.Invoke($"Not Enough Pins");
                }
            }, "claim_luxury_basket", new LogCurrency("currency", "pin", "luxury_basket", "non_iap", "feature", "claim_luxury_basket"));
        }

        private void OnStartWatch(Action<bool> onCallback)
        {
            if (GameLogic.TotalPin < targetValue)
            {
                ActionEvent.OnShowToast?.Invoke($"Not Enough Pins");
                onCallback?.Invoke(false);
            }
            else
            {
                onCallback?.Invoke(true);
            }
        }

        private void OnCompletedWatch()
        {
            PlayerDataManager.OnSpendPin?.Invoke(targetValue, (n) =>
            {
                if (n)
                {
                    OnHide(() =>
                    {
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin(240, Vector3.zero, new LogCurrency("currency", "coin", "luxury_basket", "non_iap", "ads", "rwd_ads")));
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterUndo(2, Vector3.zero, new LogCurrency("booster", $"{BoosterType.Undo}", "luxury_basket", "non_iap", "ads", "rwd_ads")));
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterAddSlots(2, Vector3.zero, new LogCurrency("booster", $"{BoosterType.AddSlots}", "luxury_basket", "non_iap", "ads", "rwd_ads")));
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterClear(2, Vector3.zero, new LogCurrency("booster", $"{BoosterType.Clear}", "luxury_basket", "non_iap", "ads", "rwd_ads")));
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainInfiniteLive(1800, Vector3.zero, new LogCurrency("energy", "infinite_live", "luxury_basket", "non_iap", "ads", "rwd_ads"))); // 900 = 15m
                        RewardGainController.OnStartGaining?.Invoke();
                        PlayerDataManager.OnOpenBasket?.Invoke();
                        ButtonLuxuryBasket.OnUpdateNoti?.Invoke();
                        ButtonLuxuryBasket.OnUpdateButtonText?.Invoke();
                    });
                }
                else
                {
                    ActionEvent.OnShowToast?.Invoke($"Not Enough Pins");
                }
            }, "watch_to_open_luxury_basket", new LogCurrency("currency", "pin", "luxury_basket", "non_iap", "feature", "watch_to_open_luxury_basket"));
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            if (args is PopupLuxuryBasketOpenArgs popupArgs)
            {
                targetValue = popupArgs.targetValue;
            }
            base.Show(args, callback);
            OnStart();
        }
        #endregion
    }
}
