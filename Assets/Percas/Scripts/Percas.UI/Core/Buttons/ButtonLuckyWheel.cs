using System;
using UnityEngine;
using Sonat;

namespace Percas.UI
{
    public class ButtonLuckyWheel : ButtonBase
    {
        [SerializeField] bool openSalePack;
        [SerializeField] GameObject noti;

        public static Action OnClosePopup;
        public static Action OnUpdateNoti;

        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(GameLogic.CurrentLevel > GameLogic.LevelUnlockClear);
            gameObject.transform.parent.gameObject.SetActive(GameLogic.CurrentLevel > GameLogic.LevelUnlockClear);
            OnUpdateNoti += HandleNoti;
            SetPointerClickEvent(DailyRewards);
        }

        private void OnDestroy()
        {
            OnUpdateNoti -= HandleNoti;
        }

        private void OnEnable()
        {
            HandleNoti();
        }

        private void HandleNoti()
        {
            if (noti == null) return;
            noti.SetActive(true);
        }

        private void DailyRewards()
        {
            if (openSalePack)
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.LuckyWheel);
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
