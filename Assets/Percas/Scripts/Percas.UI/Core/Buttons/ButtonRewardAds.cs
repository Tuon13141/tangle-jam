using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Percas.Data;
using Sonat;

namespace Percas.UI
{
    public class ButtonRewardAds : ButtonBase
    {
        [SerializeField] GameObject noti;
        [SerializeField] RectTransform m_icon;
        [SerializeField] TMP_Text m_textButton;

        public static Action OnUpdateNoti;
        public static Action OnScaleLoop;

        private Tween scaleTween;

        protected override void Awake()
        {
            base.Awake();
            Show();
            OnUpdateNoti += HandleNoti;
            OnScaleLoop += ScaleLoop;
            SetPointerClickEvent(OpenRewardAds);
        }

        private void OnDestroy()
        {
            OnUpdateNoti -= HandleNoti;
            OnScaleLoop -= ScaleLoop;
            scaleTween?.Kill();
            m_icon.localScale = Vector3.one;
        }

        private void Show()
        {
            gameObject.SetActive(true);
            gameObject.transform.parent.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            HandleNoti();
        }

        private void OnDisable()
        {
            scaleTween?.Kill();
            m_icon.localScale = Vector3.one;
        }

        private void HandleNoti()
        {
            if (m_textButton != null) m_textButton.text = $"{Math.Clamp(PlayerDataManager.PlayerData.GetCountRewardAds(), 0, Const.MAX_DAILY_WATCH_ADS)}/{Const.MAX_DAILY_WATCH_ADS}";
            if (noti == null) noti.SetActive(CanOpen());
        }

        private bool CanOpen()
        {
            return PlayerDataManager.PlayerData.CheckHaveGiftReward();
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

        private void OpenRewardAds()
        {
            Debug.Log(PopupName.RewardAds);
            ServiceLocator.PopupScene.ShowPopup(PopupName.RewardAds);

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
