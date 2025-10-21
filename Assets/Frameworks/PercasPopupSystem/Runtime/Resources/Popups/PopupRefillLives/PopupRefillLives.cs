using System;
using Percas.Data;
using Percas.Live;
using Percas.UI;
using TMPro;
using UnityEngine;

namespace Percas
{
    public class PopupRefillLives : PopupBase
    {
        [SerializeField] ButtonClosePopup buttonClosePopup;
        [SerializeField] GameObject goBodyFull, goBodyNotFull;
        [SerializeField] TMP_Text textFullMessage;
        [SerializeField] TMP_Text textPrice;
        [SerializeField] TMP_Text textLiveAmount;
        [SerializeField] ButtonUseCoin buttonUseCoin;
        [SerializeField] ButtonWatchVideoAd buttonWatchVideoAd;

        private int lostLive = 0;
        private int pricePerLive = 60;

        protected override void Awake()
        {
            RegisterButtons();
        }

        private void RegisterButtons()
        {
            buttonClosePopup.onCompleted = Close;

            buttonUseCoin.onStart = OnStartAction;
            buttonUseCoin.onCompleted = OnCompletedUseCoin;
            buttonUseCoin.onError = OnErrorUseCoin;

            buttonWatchVideoAd.reward = new(IAR.RewardType.Live, 1, null);
            buttonWatchVideoAd.skipVideo = false;
            buttonWatchVideoAd.onStart = OnStartAction;
            buttonWatchVideoAd.onCompleted = OnCompletedWatchVideoAd;
        }

        protected override void OnSubscribeEvents()
        {
            TimeManager.OnTick += UpdateButtonUseCoin;
        }

        protected override void OnUnsubscribeEvents()
        {
            TimeManager.OnTick -= UpdateButtonUseCoin;
        }

        private void Close()
        {
            OnHide();
        }

        private void OnHide(Action onCompleted = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.RefillLives, () =>
            {
                onCompleted?.Invoke();
                GameLogic.AutoSalePopupClosed = true;
            });
        }

        private void SetLostLiveAmount()
        {
            lostLive = Math.Clamp(LiveManager.MaxLives - GameLogic.CurrentLive, 1, 5);
        }

        private void UpdateButtonUseCoin()
        {
            SetLostLiveAmount();
            textPrice.text = $"{lostLive * pricePerLive}";
            textLiveAmount.text = $"+{lostLive}";
            buttonUseCoin.SetCoinToSpend(lostLive * pricePerLive);
        }

        private void OnStart()
        {
            UpdateButtonUseCoin();

            goBodyFull.SetActive(GameLogic.IsFullLive || GameLogic.IsInfiniteLive);
            goBodyNotFull.SetActive(!GameLogic.IsFullLive && !GameLogic.IsInfiniteLive);

            if (GameLogic.IsInfiniteLive)
            {
                textFullMessage.text = $"You are infinite now!";
            }
        }

        private void OnStartAction(Action<bool> onCallback)
        {
            if (GameLogic.IsFullLive || GameLogic.IsInfiniteLive)
            {
                ActionEvent.OnShowToast?.Invoke($"You are full!");
                onCallback?.Invoke(false);
            }
            else
            {
                onCallback?.Invoke(true);
            }
        }

        private void OnCompletedUseCoin()
        {
            OnHide(() =>
            {
                LiveManager.OnRefillLives?.Invoke(lostLive, new LogCurrency("energy", "live", "refill_live", "non_iap", "feature", "use_coin_to_refill_live"));
                UICurrencyManager.OnShowLiveGain?.Invoke(false, lostLive);
            });
        }

        private void OnErrorUseCoin()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.Shop);
        }

        private void OnCompletedWatchVideoAd()
        {
            OnHide(() =>
            {
                LiveManager.OnRefillLives?.Invoke(1, new LogCurrency("energy", "live", "refill_live", "non_iap", "ads", "rwd_ads"));
                UICurrencyManager.OnShowLiveGain?.Invoke(false, 1);
            });
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            OnStart();
        }

        public override void Hide(Action callback = null)
        {
            base.Hide(callback);
        }
        #endregion
    }
}
