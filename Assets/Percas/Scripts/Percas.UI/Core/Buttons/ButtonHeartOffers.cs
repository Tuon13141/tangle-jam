using System;
using UnityEngine;
using Percas.Data;
using Sonat;

namespace Percas.UI
{
    public class ButtonHeartOffers : ButtonBase
    {
        [SerializeField] bool openSalePack;
        [SerializeField] GameObject noti;

        public static Action OnShow;
        public static Action OnUpdateNoti;

        protected override void Awake()
        {
            base.Awake();
            Show();
            OnShow += Show;
            OnUpdateNoti += HandleNoti;
            SetPointerClickEvent(HeartOffers);
        }

        private void OnDestroy()
        {
            OnShow -= Show;
            OnUpdateNoti -= HandleNoti;
        }

        private void Show()
        {
            gameObject.SetActive(GameLogic.CurrentLive <= 1 || GameLogic.CanShowHeartOffers);
            gameObject.transform.parent.gameObject.SetActive(GameLogic.CurrentLive <= 1 || GameLogic.CanShowHeartOffers);
            if (!GameLogic.CanShowHeartOffers && GameLogic.CurrentLive <= 1)
            {
                PlayerDataManager.PlayerData.CanShowHeartOffers = true;
                PlayerDataManager.OnSave?.Invoke();
            }
        }

        private void OnEnable()
        {
            HandleNoti();
        }

        private void HandleNoti()
        {
            if (noti == null) return;
            noti.SetActive(!GlobalSetting.NotiHeartOffersSeen);
        }

        private void HeartOffers()
        {
            if (openSalePack)
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.HeartOffers);
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
