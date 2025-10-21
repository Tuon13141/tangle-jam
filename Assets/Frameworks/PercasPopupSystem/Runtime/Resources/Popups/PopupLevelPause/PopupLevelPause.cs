using System;
using Percas.Data;
using Percas.UI;
using TMPro;
using UnityEngine;
using Sdk.Google.NativeAds;

namespace Percas
{
    public class PopupLevelPause : PopupBase
    {
        [SerializeField] ButtonBase buttonHowToPlay;
        [SerializeField] ButtonShowInter buttonQuitLevel;
        [SerializeField] ButtonClosePopup buttonClosePopup;
        [SerializeField] ButtonBase buttonRestore;
        [SerializeField] GameObject m_iconLive;
        [SerializeField] TMP_Text textHeader;
        [SerializeField] UINativeAds m_nativeAd;

        protected override void Awake()
        {
            RegisterButtons();
        }

        protected override void OnSubscribeEvents()
        {
            ActionEvent.OnBlockNativeAd += BlockNativeAd;
        }

        protected override void OnUnsubscribeEvents()
        {
            ActionEvent.OnBlockNativeAd -= BlockNativeAd;
        }

        private void RegisterButtons()
        {
            buttonHowToPlay.SetPointerClickEvent(OpenHowToPlay);
            buttonRestore.SetPointerClickEvent(Restore);
            buttonQuitLevel.onCompleted = OnCompletedQuitLevel;
            buttonClosePopup.onCompleted = Close;
        }

        private void InitUI()
        {
            m_iconLive.SetActive(GameLogic.IsClassicMode);
            buttonQuitLevel.gameObject.SetActive(GameLogic.CurrentLevel >= GameLogic.LevelUnlockHome);
            textHeader.text = $"LEVEL {GameLogic.CurrentLevel}";
        }

        private void OpenHowToPlay()
        {
            TutorialManager.OnOpenHowToPlay?.Invoke();
        }

        private void OnCompletedQuitLevel()
        {
            if (GameLogic.CurrentLevel < GameLogic.LevelUnlockHome) return;

            ServiceLocator.PopupScene.HidePopup(PopupName.LevelPause, () =>
            {
                TrackingManager.OnLevelEnd?.Invoke(false, "quit");
                GlobalSetting.OnGameToHome?.Invoke(null);
            });
        }

        private void Close()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.LevelPause, null);
        }

        private void Restore()
        {
#if UNITY_ANDROID
            Kernel.Resolve<Purchaser>().RestorePurchase();
#elif UNITY_IOS
        Kernel.Resolve<Purchaser>().RestorePurchasesIOS(null);
#endif
        }

        private void BlockNativeAd(bool value)
        {
            m_nativeAd.BlockNativeAd(value);
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            InitUI();
            BlockNativeAd(true);
        }
        #endregion
    }
}
