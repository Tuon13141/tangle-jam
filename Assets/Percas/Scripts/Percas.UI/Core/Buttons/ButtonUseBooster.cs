using System;
using UnityEngine;
using DG.Tweening;
using Percas.Data;
using TMPro;

namespace Percas.UI
{
    public class ButtonUseBooster : ButtonBase
    {
        [SerializeField] BoosterType boosterType;

        [Header("References")]
        [SerializeField] RectTransform m_rectBooster;
        [SerializeField] TMP_Text textAmount;
        [SerializeField] TMP_Text textPrice;
        [SerializeField] TMP_Text textLevelUnlock;
        [SerializeField] GameObject goPrice, goAmount, goActive, goInactive;

        [HideInInspector]
        public Action<Action<bool>> onStart;
        public Action onCompleted;
        public Action onError;

        public static Action OnUpdateUI;
        public static Action<BoosterType> OnFocus;

        private int price;
        private int levelUnlock;

        private Tween scaleTween;

        protected override void Awake()
        {
            base.Awake();
            SetBoosterPrice();
            SetLevelUnlockBoosterPrice();
            SetPointerClickEvent(UseBooster);

            OnUpdateUI += UpdateUI;
            OnFocus += FocusOn;
        }

        private void OnDestroy()
        {
            OnUpdateUI -= UpdateUI;
            OnFocus -= FocusOn;
            scaleTween?.Kill();
        }

        private void OnEnable()
        {
            UpdateUI();
        }

        private void OnDisable()
        {
            scaleTween?.Kill();
        }

        private void SetBoosterPrice()
        {
            switch (boosterType)
            {
                case BoosterType.Undo:
                    price = GameLogic.PriceUndo;
                    break;

                case BoosterType.AddSlots:
                    price = GameLogic.PriceAddSlots;
                    break;

                case BoosterType.Clear:
                    price = GameLogic.PriceClear;
                    break;
            }
        }

        private void SetLevelUnlockBoosterPrice()
        {
            switch (boosterType)
            {
                case BoosterType.Undo:
                    levelUnlock = GameLogic.LevelUnlockUndo;
                    break;

                case BoosterType.AddSlots:
                    levelUnlock = GameLogic.LevelUnlockAddSlots;
                    break;

                case BoosterType.Clear:
                    levelUnlock = GameLogic.LevelUnlockClear;
                    break;
            }
        }

        private void UseBooster()
        {
            if (GameLogic.CurrentLevel < levelUnlock)
            {
                ActionEvent.OnShowToast?.Invoke(string.Format(Const.LANG_KEY_UNLOCK_AT_LEVEL, levelUnlock));
                return;
            }

            onStart?.Invoke((canStart) =>
            {
                if (!canStart) return;
                if (GameLogic.GetBoosterAmount(boosterType) > 0)
                {
                    BoosterManager.OnUseBooster?.Invoke(boosterType, () =>
                    {
                        onCompleted?.Invoke();
                        UpdateUI();
                    }, () =>
                    {
                        onError?.Invoke();
                    }, new LogCurrency("booster", $"{boosterType}", "gameplay", null, "feature", $"Booster_{boosterType}"));
                }
                else
                {
                    onError?.Invoke();
                    //PlayerDataManager.OnSpendCoin?.Invoke(price, (value) =>
                    //{
                    //    if (value)
                    //    {
                    //        onCompleted?.Invoke();
                    //    }
                    //    else
                    //    {
                    //        onError?.Invoke();
                    //    }
                    //}, $"Booster_{boosterType}", new LogCurrency("currency", "coin", "gameplay", null, "feature", $"Booster_{boosterType}"));
                }
            });
        }

        private void UpdateUI()
        {
            if (GameLogic.CurrentLevel < levelUnlock)
            {
                textLevelUnlock.text = $"Level {levelUnlock}";
                goActive.SetActive(false);
                goInactive.SetActive(true);
                goPrice.SetActive(false);
                goAmount.SetActive(false);
            }
            else
            {
                textAmount.text = $"{GameLogic.GetBoosterAmount(boosterType)}";
                textPrice.text = $"{price}";
                goActive.SetActive(true);
                goInactive.SetActive(false);
                goPrice.SetActive(GameLogic.GetBoosterAmount(boosterType) <= 0);
                goAmount.SetActive(GameLogic.GetBoosterAmount(boosterType) > 0);
            }
        }

        private void FocusOn(BoosterType boosterType)
        {
            if (this.boosterType != boosterType || m_rectBooster == null) return;
            scaleTween = m_rectBooster.DOScale(1.25f, 0.3f).SetLoops(10, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
    }
}
