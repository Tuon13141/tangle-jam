using System;
using Percas.Data;
using Percas.IAR;
using Percas.UI;
using UnityEngine;

namespace Percas
{
    public class PopupLevelFailed : PopupBase
    {
        [SerializeField] ButtonShowInter buttonTryAgain;
        [SerializeField] ButtonWatchVideoAd buttonWatchVideoAd;
        [SerializeField] ButtonShowInter buttonClose;

        protected override void Awake()
        {
            RegisterButtons();
        }

        private void RegisterButtons()
        {
            buttonClose.onCompleted = OnCompletedClose;

            buttonTryAgain.onCompleted = OnCompletedTryAgain;

            buttonWatchVideoAd.skipVideo = false;
            buttonWatchVideoAd.onStart = OnStartAction;
            buttonWatchVideoAd.onCompleted = OnWatchVideoCompleted;
        }

        private void InitUI()
        {
            buttonClose.gameObject.SetActive(GameLogic.CurrentLevel >= GameLogic.LevelUnlockHome);
        }

        private void OnCompletedTryAgain()
        {
            if (PercasSDK.FirebaseManager.RemoteConfig.GetBool("active_new_mode") && !Static.isPlayDoneNewMode)
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                GameLoading.OnShow?.Invoke(null, false);
            }
            else if (!GameLogic.IsInfiniteLive && GameLogic.CurrentLive <= 0)
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.RefillLives);
            }
            else
            {
                PlayerDataManager.SetContinueWith("try_again");
                TrackingManager.OnLevelEnd?.Invoke(false, "level_try_again");
                if (LevelController.instance != null) LevelController.instance.ResetLevel();
                ServiceLocator.PopupScene.HidePopup(PopupName.LevelFailed, null);
            }
        }

        private void OnCompletedClose()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.LevelFailed, () =>
            {
                GlobalSetting.OnGameToHome?.Invoke(null);
            });
        }

        private void OnStartAction(Action<bool> onCallback)
        {
            onCallback?.Invoke(true);
        }

        private void OnWatchVideoCompleted()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.LevelFailed, () =>
            {
                PlayerDataManager.PlayerData.AddVideoReviveCount();
                buttonWatchVideoAd.reward = new(RewardType.BoosterRevive, 1, null);
                PlayerDataManager.OnUpdateContinueTimes?.Invoke();
                if (LevelController.instance != null) LevelController.instance.ReviveBooster();

                var logCurrency = new LogCurrency("booster", "revive", "buy_booster", "non_iap", "ads", "rwd_ads");
                TrackingManager.OnEarnBooster?.Invoke(logCurrency, 1);
                TrackingManager.OnSpendBooster?.Invoke(logCurrency);

                TrackingManager.OnLevelEnd?.Invoke(false, "replay_when_lose");
                TrackingManager.OnContinuePlaying?.Invoke("watch_video_to_revive");
            });
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            InitUI();
        }
        #endregion
    }
}
