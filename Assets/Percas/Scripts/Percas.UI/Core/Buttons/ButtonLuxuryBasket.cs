using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Percas.Data;
using Sonat;

namespace Percas.UI
{
    public class ButtonLuxuryBasket : ButtonBase
    {
        [SerializeField] bool blockButton = false;
        [SerializeField] bool openSalePack;
        [SerializeField] GameObject noti;
        [SerializeField] RectTransform m_icon;
        [SerializeField] TMP_Text m_textButton;

        public static Action OnUpdateNoti;
        public static Action OnUpdateButtonText;
        public static Action OnScaleLoop;

        private Tween scaleTween;

        private int targetValue = 50;
        private List<int> targets;

        protected override void Awake()
        {
            base.Awake();
            Show();
            OnUpdateNoti += HandleNoti;
            OnUpdateButtonText += HandleButtonText;
            OnScaleLoop += ScaleLoop;
            SetPointerClickEvent(OpenLuxuryBasket);
        }

        private void OnDestroy()
        {
            OnUpdateNoti -= HandleNoti;
            OnUpdateButtonText -= HandleButtonText;
            OnScaleLoop -= ScaleLoop;
            scaleTween?.Kill();
            m_icon.localScale = Vector3.one;
        }

        private void Show()
        {
            gameObject.SetActive(false/*PlayerDataManager.PlayerData.PreIntroToLuxuryBasket*/);
            gameObject.transform.parent.gameObject.SetActive(false/*PlayerDataManager.PlayerData.PreIntroToLuxuryBasket*/);
        }

        private void OnEnable()
        {
            HandleButtonText();
            HandleNoti();
        }

        private void OnDisable()
        {
            scaleTween?.Kill();
            m_icon.localScale = Vector3.one;
        }

        private void HandleButtonText()
        {
            targets = DataManager.Instance.LuxuryBasketTargets;
            targetValue = targets[Mathf.Clamp(PlayerDataManager.PlayerData.LuxuryBasketOpenedCount, 0, targets.Count - 1)];
            m_textButton.text = $"{GameLogic.TotalPin}/{targetValue}";
        }

        private void HandleNoti()
        {
            if (noti == null) return;
            noti.SetActive(CanOpen());
        }

        private bool CanOpen()
        {
            if (PlayerDataManager.PlayerData.LuxuryBasketOpenedCount == 0) return GameLogic.TotalPin >= 50;
            else if (PlayerDataManager.PlayerData.LuxuryBasketOpenedCount == 1) return GameLogic.TotalPin >= 100;
            else return GameLogic.TotalPin >= 200;
        }

        private void ScaleLoop()
        {
            Vector3 targetScale = new(1.1f, 1.1f, 1.1f);
            m_icon.localScale = Vector3.one;
            scaleTween = m_icon.DOScale(targetScale, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                m_icon.localScale = Vector3.one;
            });
        }

        private void OpenLuxuryBasket()
        {
            if (blockButton) return;

            if (openSalePack)
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.LuxuryBasket);
            }
            var log = new SonatLogClickIconShortcut()
            {
                mode = PlayMode.classic.ToString(),
                level = GameLogic.CurrentLevel,
                placement = placement,
                shortcut = shortcut
            };
            log.Post(logAf: true);
        }
    }
}
