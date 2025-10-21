using System;
using Percas.Data;
using Percas.UI;
using TMPro;
using UnityEngine;

namespace Percas
{
    public class PopupLevelRetry : PopupBase
    {
        [SerializeField] GameObject liveMinus, liveInfinite;
        [SerializeField] TMP_Text textMessage;
        [SerializeField] ButtonShowInter buttonTryAgain;
        [SerializeField] ButtonBase buttonClose;

        protected override void Awake()
        {
            RegisterButtons();
        }

        private void RegisterButtons()
        {
            buttonTryAgain.onCompleted = OnCompletedTryAgain;
            buttonClose.SetPointerClickEvent(Close);
        }

        private void InitUI()
        {
            liveMinus.SetActive(GameLogic.CurrentLevel >= GameLogic.LevelUnlockHome && !GameLogic.IsInfiniteLive);
            liveInfinite.SetActive(GameLogic.CurrentLevel < GameLogic.LevelUnlockHome || GameLogic.IsInfiniteLive);
            textMessage.text = (GameLogic.CurrentLevel < GameLogic.LevelUnlockHome || GameLogic.IsInfiniteLive) ? $"You are infinite now!" : $"You will lose 1 live!";
        }

        private void OnHide(Action onCompleted = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.LevelRetry, onCompleted);
        }

        private void OnCompletedTryAgain()
        {
            if (!GameLogic.IsInfiniteLive && GameLogic.CurrentLive <= 0 && GameLogic.CurrentLevel >= GameLogic.LevelUnlockHome)
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.RefillLives);
            }
            else
            {
                PlayerDataManager.SetContinueWith("retry");
                TrackingManager.OnLevelEnd?.Invoke(false, "replay_level");
                if (LevelController.instance != null) LevelController.instance.ResetLevel();
                OnHide();
            }
        }

        private void Close()
        {
            OnHide();
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
