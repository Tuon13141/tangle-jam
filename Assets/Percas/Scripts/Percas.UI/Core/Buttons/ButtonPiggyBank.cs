using System;
using UnityEngine;
using TMPro;
using Sonat;

namespace Percas.UI
{
    public class ButtonPiggyBank : ButtonBase
    {
        [SerializeField] bool openSalePack;
        [SerializeField] GameObject noti;
        [SerializeField] TMP_Text m_textButton;

        public static Action OnUpdateNoti;
        public static Action OnUpdateButtonText;

        protected override void Awake()
        {
            base.Awake();
            OnUpdateNoti += HandleNoti;
            OnUpdateButtonText += HandleButtonText;
            SetPointerClickEvent(PiggyBank);
        }

        private void OnDestroy()
        {
            OnUpdateNoti -= HandleNoti;
            OnUpdateButtonText -= HandleButtonText;
        }

        private void OnEnable()
        {
            HandleButtonText();
            HandleNoti();
        }

        private void HandleButtonText()
        {
            m_textButton.text = GameLogic.IsFullPiggyBank ? $"BREAK" : $"{GameLogic.CurrentCoinInPiggyBank}";
        }

        private void HandleNoti()
        {
            if (noti == null) return;
            noti.SetActive(!GlobalSetting.NotiPiggyBankSeen || GameLogic.IsFullPiggyBank);
        }

        private void PiggyBank()
        {
            if (openSalePack)
            {
                //PopupPackPiggyBank.Instance.Show();
                ServiceLocator.PopupScene.ShowPopup(PopupName.PiggyBank);
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