using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sonat;

namespace Percas.UI
{
    public class ButtonHiddenPicture : ButtonBase
    {
        [SerializeField] bool blockButton = false;
        [SerializeField] bool openSalePack;
        [SerializeField] Image m_imageIcon;
        [SerializeField] GameObject noti;
        [SerializeField] TMP_Text m_textButton;

        public static Action OnUpdateUI;
        public static Action OnUpdateNoti;

        private bool hasAvailableEvent = false;
        private bool canShow = false;

        protected override void Awake()
        {
            base.Awake();

            OnUpdateUI += UpdateUI;
            OnUpdateNoti += HandleNoti;

            SetPointerClickEvent(OpenHiddenPicture);
        }

        private void OnDestroy()
        {
            OnUpdateUI -= UpdateUI;
            OnUpdateNoti -= HandleNoti;
        }

        private void OnEnable()
        {
            SetupButton();
            HandleNoti();
        }

        private async void SetupButton()
        {
            await Show();
            await HandleButtonUI();
        }

        private async UniTask Show()
        {
            hasAvailableEvent = HiddenPictureManager.Data.HasAvailableEvent();
            if (GameLogic.CurrentLevel < GameLogic.LevelShowHiddenPictureIcon)
            {
                canShow = false;
            }
            else
            {
                canShow = hasAvailableEvent || GameLogic.IsActiveHiddenPictureEvent;
            }
            gameObject.SetActive(canShow);
            gameObject.transform.parent.gameObject.SetActive(canShow);
            await UniTask.Delay(0);
        }

        private async UniTask HandleButtonUI()
        {
            if (!canShow) return;

            if (GameLogic.CurrentLevel < GameLogic.LevelUnlockHiddenPicture)
            {
                m_textButton.text = $"UNLOCK AT LEVEL {GameLogic.LevelUnlockHiddenPicture}";
            }
            else
            {
                Sprite eventIcon = DataManager.Instance.GetCurrentHiddenPictureIcon();
                if (eventIcon != null)
                {
                    m_imageIcon.sprite = eventIcon;
                }

                try
                {
                    if (!HiddenPictureManager.Data.IsCompleted())
                    {
                        TimeSpan remainTime = TimeHelper.ParseIsoString(HiddenPictureManager.Data.EndTime) - DateTime.UtcNow;
                        m_textButton.text = $"{Helpers.ConvertTimeToText(remainTime)}";
                    }
                    else
                    {
                        m_textButton.text = $"{DataManager.Instance.GetCurrentHiddenPictureName()}";
                    }
                }
                catch (Exception) { }
            }
            await UniTask.Delay(0);
        }

        private async void SetupEvent()
        {
            await StartEvent();
        }

        private async UniTask StartEvent()
        {
            if (!canShow) return;

            if (GameLogic.CurrentLevel < GameLogic.LevelUnlockHiddenPicture) return;

            if (hasAvailableEvent)
            {
                HiddenPictureManager.Data.Start();
            }
            else if (HiddenPictureManager.Data.IsCompleted())
            {
                HiddenPictureManager.Data.Reset();
            }
            await UniTask.Delay(0);
        }

        private void HandleNoti()
        {
            if (noti == null) return;
            bool hasNoti = GameLogic.CurrentLevel >= GameLogic.LevelUnlockHiddenPicture && HiddenPictureManager.Data.HasActiveEvent() && HiddenPictureManager.Data.Keys >= 1;
            noti.SetActive(hasNoti);
        }

        private void UpdateUI()
        {
            Sprite eventIcon = DataManager.Instance.GetCurrentHiddenPictureIcon();
            if (eventIcon != null)
            {
                m_imageIcon.sprite = eventIcon;
            }

            try
            {
                if (!HiddenPictureManager.Data.IsCompleted())
                {
                    TimeSpan remainTime = TimeHelper.ParseIsoString(HiddenPictureManager.Data.EndTime) - DateTime.UtcNow;
                    m_textButton.text = $"{Helpers.ConvertTimeToText(remainTime)}";
                }
                else
                {
                    m_textButton.text = $"{DataManager.Instance.GetCurrentHiddenPictureName()}";
                }
            }
            catch (Exception) { }
        }

        private void OpenHiddenPicture()
        {
            if (!GameLogic.InternetReachability && GameLogic.CurrentLevel >= GameLogic.LevelNeedsInternet)
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.InternetRequired);
                return;
            }

            if (blockButton) return;

            if (GameLogic.CurrentLevel < GameLogic.LevelUnlockHiddenPicture)
            {
                ActionEvent.OnShowToast?.Invoke($"UNLOCK AT LEVEL {GameLogic.LevelUnlockHiddenPicture}");
                return;
            }

            if (openSalePack)
            {
                if (GameLogic.IsActiveHiddenPictureEvent)
                {
                    ServiceLocator.PopupScene.ShowPopup(PopupName.HiddenPicture);
                }
                else
                {
                    SetupButton();
                    SetupEvent();
                    AdLoading.OnLoad?.Invoke(1.0f, () =>
                    {
                        ServiceLocator.PopupScene.ShowPopup(PopupName.HiddenPicture);
                        OnUpdateUI?.Invoke();
                        OnUpdateNoti?.Invoke();
                    });
                }
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