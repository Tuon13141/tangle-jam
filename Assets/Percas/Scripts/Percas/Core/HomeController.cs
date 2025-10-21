using UnityEngine;
using Percas.UI;
using Sonat;

namespace Percas
{
    public class HomeController : MonoBehaviour
    {
        [SerializeField] ButtonBase buttonOpenSetting;
        [SerializeField] ButtonBase buttonProfile;
        [SerializeField] ButtonBase buttonOpenShop;
        [SerializeField] ButtonBase buttonOpenCollection;
        [SerializeField] ButtonBase buttonGoToPreviousRoom, buttonGoToNextRoom;

        private void OnEnable()
        {
            GameLogic.AutoIntroPopupClosed = false;
            GameLogic.AutoSalePopupClosed = false;
            GameLogic.AutoEventPopupClosed = false;
        }

        private void Start()
        {
            TrackingManager.OnTrackScreenView?.Invoke(ScreenName.SceneHome.ToString());
            GlobalSetting.ScreenName = ScreenName.SceneHome.ToString();
            buttonOpenSetting.SetPointerClickEvent(OpenSetting);
            buttonProfile.SetPointerClickEvent(OpenProfile);
            buttonOpenShop.SetPointerClickEvent(OpenShop);
            buttonOpenCollection.SetPointerClickEvent(OpenCollections);
            buttonGoToPreviousRoom.SetPointerClickEvent(GoToPreviousRoom);
            buttonGoToNextRoom.SetPointerClickEvent(GoToNextRoom);
        }

        private void OpenSetting()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.Settings);
            var log = new SonatLogClickIconShortcut()
            {
                mode = PlayMode.classic.ToString(),
                level = GameLogic.CurrentLevel,
                placement = "home",
                shortcut = "settings"
            };
            log.Post(logAf: true);
        }

        private void OpenProfile()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.Profile);
            var log = new SonatLogClickIconShortcut()
            {
                mode = PlayMode.classic.ToString(),
                level = GameLogic.CurrentLevel,
                placement = "home",
                shortcut = "profile"
            };
            log.Post(logAf: true);
        }

        private void OpenShop()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.Shop);
            var log = new SonatLogClickIconShortcut()
            {
                mode = PlayMode.classic.ToString(),
                level = GameLogic.CurrentLevel,
                placement = "home",
                shortcut = "shop"
            };
            log.Post(logAf: true);
        }

        private void OpenCollections()
        {
            ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_COMING_SOON);
            return;

            if (GameLogic.CurrentLevel < GameLogic.LevelUnlockCollections)
            {
                ActionEvent.OnShowToast?.Invoke(string.Format(Const.LANG_KEY_UNLOCK_AT_LEVEL, GameLogic.LevelUnlockCollections));
                return;
            }

            ServiceLocator.PopupScene.ShowPopup(PopupName.Collections);
            var log = new SonatLogClickIconShortcut()
            {
                mode = PlayMode.classic.ToString(),
                level = GameLogic.CurrentLevel,
                placement = "home",
                shortcut = "collections"
            };
            log.Post(logAf: true);
        }

        private void GoToPreviousRoom()
        {
            CollectionController.instance.Previous();
            var log = new SonatLogClickIconShortcut()
            {
                mode = PlayMode.classic.ToString(),
                level = GameLogic.CurrentLevel,
                placement = "home",
                shortcut = "view_previous_room"
            };
            log.Post(logAf: true);
        }

        private void GoToNextRoom()
        {
            CollectionController.instance.Next();
            var log = new SonatLogClickIconShortcut()
            {
                mode = PlayMode.classic.ToString(),
                level = GameLogic.CurrentLevel,
                placement = "home",
                shortcut = "view_next_room"
            };
            log.Post(logAf: true);
        }
    }
}
