using System;
using UnityEngine;
using Sonat;
using Percas.Data;

namespace Percas.UI
{
    public class ButtonDailyRewards : ButtonBase
    {
        [SerializeField] bool hasParent = false;
        [SerializeField] bool openSalePack;
        [SerializeField] GameObject noti;

        public static Action OnClosePopup;
        public static Action OnUpdateNoti;

        protected override void Awake()
        {
            base.Awake();
            OnUpdateNoti += HandleNoti;
            SetPointerClickEvent(DailyRewards);
        }

        private void OnDestroy()
        {
            OnUpdateNoti -= HandleNoti;
        }

        private void OnEnable()
        {
            HandleDisplay();
            HandleNoti();
        }

        private void HandleDisplay()
        {
            gameObject.SetActive(GameLogic.UnlockDailyRewards);
            if (hasParent) gameObject.transform.parent.gameObject.SetActive(GameLogic.UnlockDailyRewards);
        }

        private void HandleNoti()
        {
            if (noti == null) return;
            noti.SetActive(PlayerDataManager.PlayerData.DailyRewardIndex <= 4);
        }

        private void DailyRewards()
        {
            if (openSalePack)
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.DailyRewards, new PopupDailyRewardsArgs(false));
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