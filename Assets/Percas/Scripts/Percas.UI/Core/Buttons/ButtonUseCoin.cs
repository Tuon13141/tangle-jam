using System;
using UnityEngine;
using Percas.Data;

namespace Percas.UI
{
    public class ButtonUseCoin : ButtonBase
    {
        [SerializeField] int coinToSpend = 0;
        [SerializeField] string screen;
        [SerializeField] SpendCoinLocation location;

        [HideInInspector]
        public Action<Action<bool>> onStart;
        public Action onCompleted;
        public Action onError;

        protected override void Awake()
        {
            base.Awake();
            SetPointerClickEvent(OnUseCoin);
        }

        public void SetCoinToSpend(int value)
        {
            coinToSpend = value;
        }

        private void OnUseCoin()
        {
            if (coinToSpend <= 0)
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_SOMETHING_WRONG);
                return;
            }

            onStart?.Invoke((canStart) =>
            {
                if (!canStart) return;
                PlayerDataManager.OnSpendCoin?.Invoke(coinToSpend, (value) =>
                {
                    if (value)
                    {
                        onCompleted?.Invoke();
                    }
                    else
                    {
                        onError?.Invoke();
                    }
                }, location.ToString(), new LogCurrency("currency", "coin", screen, null, "feature", location.ToString()));
            });
        }
    }
}
