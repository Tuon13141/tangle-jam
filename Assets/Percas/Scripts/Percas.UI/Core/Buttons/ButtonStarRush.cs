using System;
using UnityEngine;
using TMPro;
using Sonat;

namespace Percas.UI
{
    public class ButtonStarRush : ButtonBase
    {
        [SerializeField] bool blockButton = false;
        [SerializeField] bool openSalePack;
        [SerializeField] GameObject noti;
        [SerializeField] TMP_Text m_textButton;

        public static Action OnClosePopup;
        public static Action OnUpdateUI;
        public static Action OnUpdateNoti;

        protected override void Awake()
        {
            base.Awake();
            OnUpdateNoti += HandleNoti;
            OnUpdateUI += UpdateTextButton;
            TimeManager.OnTick += UpdateTextButton;
            UpdateTextButton();
            SetPointerClickEvent(OpenStarRush);
        }

        private void OnDestroy()
        {
            OnUpdateNoti -= HandleNoti;
            OnUpdateUI -= UpdateTextButton;
            TimeManager.OnTick -= UpdateTextButton;
        }

        private void OnEnable()
        {
            SetupButton();
            HandleNoti();
        }

        private void SetupButton()
        {
            gameObject.SetActive(GameLogic.CurrentLevel >= GameLogic.LevelShowStarRush);
        }

        private void HandleNoti()
        {
            if (noti == null) return;
            noti.SetActive(GameLogic.CurrentLevel >= GameLogic.LevelShowStarRush && (!StarRushManager.Data.Scored || StarRushManager.Data.IsCompleted()));
        }

        private void UpdateTextButton()
        {
            if (!GameLogic.UnlockStarRush)
            {
                m_textButton.text = $"UNLOCK AT LEVEL {GameLogic.LevelUnlockStarRush}";
                return;
            }

            try
            {
                string textRemainTime;
                TimeSpan? remainTime = TimeHelper.ParseIsoString(StarRushManager.Data.EndTime) - DateTime.UtcNow;
                if (remainTime?.TotalSeconds > 0)
                {
                    textRemainTime = string.Format("{0:D2}:{1:D2}", remainTime?.Minutes, remainTime?.Seconds);
                }
                else
                {
                    textRemainTime = $"COMPLETED";

                }
                m_textButton.text = textRemainTime;
            }
            catch (Exception) { }
        }

        private void OpenStarRush()
        {
            if (!GameLogic.InternetReachability && GameLogic.CurrentLevel >= GameLogic.LevelNeedsInternet)
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.InternetRequired);
                return;
            }

            if (blockButton) return;

            if (!GameLogic.UnlockStarRush)
            {
                ActionEvent.OnShowToast?.Invoke(string.Format(Const.LANG_KEY_UNLOCK_AT_LEVEL, GameLogic.LevelUnlockStarRush));
                return;
            }

            if (openSalePack)
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.StarRush, new PopupStarRushArgs(false));
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