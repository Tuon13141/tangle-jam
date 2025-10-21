using System;
using UnityEngine;
using TMPro;
using Percas.UI;
using Percas.Live;
using Percas.IAR;
using Percas.Data;

namespace Percas
{
    public class PopupHeartOffers : PopupBase
    {
        [SerializeField] ButtonClosePopup buttonClosePopup;
        [SerializeField] ButtonWatchVideoAd buttonWatchVideoAd;
        [SerializeField] ButtonPurchase buttonPurchase;
        [SerializeField] TMP_Text textIAATimeValue;
        [SerializeField] TMP_Text textIAAButton;
        [SerializeField] TMP_Text textIAPTimeValue;

        private bool canWatch;

        protected override void OnSubscribeEvents()
        {
            TimeManager.OnTick += UpdateTextButtons;
        }

        protected override void OnUnsubscribeEvents()
        {
            TimeManager.OnTick -= UpdateTextButtons;
        }

        private void OnHide(Action onCompleted = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.HeartOffers, () =>
            {
                GlobalSetting.NotiHeartOffersSeen = true;
                ButtonHeartOffers.OnUpdateNoti?.Invoke();
                onCompleted?.Invoke();
            });
        }

        private void OnStart()
        {
            UpdateTextButtons();

            textIAATimeValue.text = $"{Helpers.ConvertSecondToText(GameLogic.UnlimitedLifeTimeLimit)}";
            textIAPTimeValue.text = $"{Helpers.ConvertSecondToText(GameLogic.UnlimitedLifeIAPTimeLimit)}";

            buttonClosePopup.onCompleted = Close;

            buttonPurchase.onCallback = Close;

            buttonWatchVideoAd.reward = new(RewardType.InfiniteLive, 1, null);
            buttonWatchVideoAd.skipVideo = false;
            buttonWatchVideoAd.onStart = OnStartWatch;
            buttonWatchVideoAd.onCompleted = OnCompletedWatch;
        }

        private void Close()
        {
            OnHide();
        }

        private void UpdateTextButtons()
        {
            // IAA Button
            try
            {
                TimeSpan? remainTime = LiveManager.LastTimeWatchHeartOffer - DateTime.UtcNow;
                if (remainTime?.TotalSeconds <= 0)
                {
                    canWatch = true;
                    textIAAButton.text = $"FREE";
                }
                else
                {
                    canWatch = false;
                    string textButtonWatch = string.Format("{0:D2}:{1:D2}", remainTime?.Minutes, remainTime?.Seconds);
                    textIAAButton.text = textButtonWatch;
                }
            }
            catch (Exception)
            {
                textIAAButton.text = $"---";
            }
        }

        private void OnStartWatch(Action<bool> onCallback)
        {
            if (!canWatch)
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_WAIT_FOR_SHORT_TIME);
                onCallback?.Invoke(false);
            }
            else
            {
                onCallback?.Invoke(true);
            }
        }

        private void OnCompletedWatch()
        {
            OnHide(() =>
            {
                RewardGainController.OnAddRewardGain?.Invoke(new RewardGainInfiniteLive(GameLogic.UnlimitedLifeTimeLimit, Vector3.zero, new LogCurrency("energy", "infinite_live", "heart_offers", "non_iap", "ads", "rwd_ads")));
                RewardGainController.OnStartGaining?.Invoke();
                LiveManager.OnUpdateLastTimeWatchHeartOffer?.Invoke();
            });
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            UpdateTextButtons();
            OnStart();
        }
        #endregion
    }
}
