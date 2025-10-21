using System;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Sonat;

namespace Percas.UI
{
    public class UICoinBalance : MonoBehaviour
    {
        [SerializeField] ButtonBase buttonBalance;
        [SerializeField] TMP_Text textCoinValue;
        [SerializeField] bool canOpenShop =  true;

        public static Action<int> OnUpdateUI;
        public static Action OnScaleLoop;

        private Tween scaleTween;

        private void Awake()
        {
            OnUpdateUI += UpdateCoinValue;
            OnScaleLoop += ScaleLoop;
        }

        private void OnDestroy()
        {
            OnUpdateUI -= UpdateCoinValue;
            OnScaleLoop -= ScaleLoop;
            scaleTween?.Kill();
            transform.localScale = Vector3.one;
        }

        private void OnEnable()
        {
            OnUpdateUI?.Invoke(GameLogic.CurrentCoin);
            buttonBalance.SetPointerClickEvent(OpenShop);
        }

        private void OnDisable()
        {
            scaleTween?.Kill();
            transform.localScale = Vector3.one;
        }

        private void UpdateCoinValue(int value)
        {
            textCoinValue.text = $"{value}";
        }

        private void OpenShop()
        {
            if (UICurrencyManager.BlockBalanceButton) return;
            if (!canOpenShop) return;
            if (GameLogic.CurrentLevel <= 2) return;

            ServiceLocator.PopupScene.ShowPopup(PopupName.Shop);

            string placement;
            if (GameLogic.IsInGame)
            {
                placement = "ingame";
            }
            else
            {
                placement = "home";
            }
            var log = new SonatLogClickIconShortcut()
            {
                mode = PlayMode.classic.ToString(),
                level = GameLogic.CurrentLevel,
                placement = placement,
                shortcut = "coin"
            };
            log.Post(logAf: true);
        }

        private void ScaleLoop()
        {
            Vector3 targetScale = new(1.1f, 1.1f, 1.1f);
            transform.localScale = Vector3.one;
            scaleTween = transform.DOScale(targetScale, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                transform.localScale = Vector3.one;
            });
        }
    }
}
