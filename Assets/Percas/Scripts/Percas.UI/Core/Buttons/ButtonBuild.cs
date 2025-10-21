using System;
using UnityEngine;
using TMPro;
using Sonat;

namespace Percas.UI
{
    public class ButtonBuild : ButtonBase
    {
        [SerializeField] bool hasParent = false;
        [SerializeField] GameObject noti;
        [SerializeField] TMP_Text m_textButton;

        public static Action OnClosePopup;
        public static Action OnUpdateUI;

        protected override void Awake()
        {
            base.Awake();
            OnUpdateUI += UpdateUI;
            SetPointerClickEvent(BuildRoom);
        }

        private void OnDestroy()
        {
            OnUpdateUI -= UpdateUI;
        }

        private void OnEnable()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            HandleDisplay();
            HandleNoti();
        }

        private void HandleDisplay()
        {
            gameObject.SetActive(GameLogic.UnlockHome);
            if (hasParent) gameObject.transform.parent.gameObject.SetActive(GameLogic.UnlockHome);
            Helpers.ChangeValueInt(GameLogic.TotalCoil - GameLogic.CoilEarned, GameLogic.TotalCoil, 0.5f, 0.0f, (value) =>
            {
                m_textButton.text = $"<sprite=0> {value}";
            });
        }

        private void HandleNoti()
        {
            if (noti == null) return;
            noti.SetActive(GameLogic.TotalCoil > 0);
        }

        private void BuildRoom()
        {
            var log = new SonatLogClickIconShortcut()
            {
                mode = PlayMode.classic.ToString(),
                level = GameLogic.CurrentLevel,
                placement = placement,
                shortcut = shortcut
            };
            log.Post(logAf: true);

            if (!GameLogic.IsShowingRoom && GameLogic.TotalCoil > 0)
            {
                UIHomeController.OnDisplay?.Invoke(false, true);
                return;
            }

            if (!GameLogic.IsShowingRoom && GameLogic.TotalCoil <= 0)
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_LACK_COIL);
                ButtonPlay.OnScaleLoop?.Invoke();
            }
        }
    }
}