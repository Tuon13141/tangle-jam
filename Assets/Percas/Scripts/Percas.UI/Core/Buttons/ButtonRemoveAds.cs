using System;
using Sonat;
using UnityEngine;

namespace Percas.UI
{
    public class ButtonRemoveAds : ButtonBase, IActivatable
    {
        [SerializeField] bool openSalePack;

        public static Action OnShow;

        protected override void Awake()
        {
            base.Awake();
            OnShow += Show;
        }

        private void OnDestroy()
        {
            OnShow -= Show;
        }

        private void OnEnable()
        {
            Show();
            SetPointerClickEvent(RemoveAds);
        }

        public void Activate()
        {
            Show();
            SetPointerClickEvent(RemoveAds);
        }

        public void Deactivate() { }

        private void Show()
        {
            gameObject.SetActive(!GameLogic.IsNoAds);
            if (placement == "home") gameObject.transform.parent.gameObject.SetActive(!GameLogic.IsNoAds);
        }

        private void RemoveAds()
        {
            if (openSalePack)
            {
                //PopupPackRemoveAds.Instance.Show();
                ServiceLocator.PopupScene.ShowPopup(PopupName.RemoveAds);
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
