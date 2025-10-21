using System;
using UnityEngine;
using Percas.UI;
using TMPro;
using NSubstitute;

namespace Percas
{
    public class PopupConfirmUseCoinArgs
    {
        public int coinValue;
        public Action<bool> onCompleted;

        public PopupConfirmUseCoinArgs(int coinValue, Action<bool> onCompleted)
        {
            this.coinValue = coinValue;
            this.onCompleted = onCompleted;
        }
    }

    public class PopupConfirmUseCoin : PopupBase
    {
        [SerializeField] ButtonBase buttonClosePopup;
        [SerializeField] ButtonBase buttonConfirm;
        [SerializeField] ButtonWatchVideoAd buttonWatchToUnlock;
        [SerializeField] TMP_Text m_textCoinValue;

        private int CoinValue;
        private Action<bool> OnCompleted;

        protected override void Awake()
        {
            RegisterButtons();
        }

        private void RegisterButtons()
        {
            buttonClosePopup.SetPointerClickEvent(Close);
            buttonConfirm.SetPointerClickEvent(Confirm);

            buttonWatchToUnlock.skipVideo = false;
            buttonWatchToUnlock.onStart = OnWatchStarted;
            buttonWatchToUnlock.onCompleted = OnWatchCompleted;
        }

        private void OnStart()
        {
            m_textCoinValue.text = $"<sprite=0> X{CoinValue}";
        }

        private void Close()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.ConfirmUseCoin, null);
        }

        private void Confirm()
        {
            if (GameLogic.CurrentCoin < CoinValue)
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.Shop);
            }
            else
            {
                ServiceLocator.PopupScene.HidePopup(PopupName.ConfirmUseCoin, () =>
                {
                    OnCompleted?.Invoke(true);
                });
            }
        }

        private void OnWatchStarted(Action<bool> onCallback)
        {
            onCallback?.Invoke(true);
        }

        private void OnWatchCompleted()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.ConfirmUseCoin, () =>
            {
                OnCompleted?.Invoke(false);
            });
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            if (args is PopupConfirmUseCoinArgs popupArgs)
            {
                CoinValue = popupArgs.coinValue;
                OnCompleted = popupArgs.onCompleted;
            }
            base.Show(args, callback);
            OnStart();
        }
        #endregion
    }
}
