using System;
using UnityEngine;
using Percas.UI;
using Percas.Data;

namespace Percas
{
    public class PopupBuildingTutorial : PopupBase
    {
        [Header("UI")]
        [SerializeField] ButtonBase buttonSkip;
        [SerializeField] ScaleAnim m_animButtonSkip;

        protected override void Awake()
        {
            RegisterButtons();
        }

        private void RegisterButtons()
        {
            buttonSkip.SetPointerClickEvent(Close);
            m_animButtonSkip.SetOnCompleted(ShowButtonClose);
        }

        private void OnHide(Action callback = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.BuildingTutorial, callback);
        }

        private void ShowButtonClose()
        {
            buttonSkip.gameObject.SetActive(true);
        }

        private void ResetUI()
        {
            buttonSkip.gameObject.SetActive(false);
        }

        private void Close()
        {
            OnHide(() =>
            {
                if (!PlayerDataManager.PlayerData.IntroToBuilding)
                {
                    PlayerDataManager.PlayerData.IntroToBuilding = true;
                    PlayerDataManager.OnSave?.Invoke();
                    // if (GameLogic.UnlockDailyRewards && !PlayerDataManager.PlayerData.IntroToDailyReward)
                    // {
                    //     ServiceLocator.PopupScene.ShowPopup(PopupName.DailyRewards, new PopupDailyRewardsArgs(false));
                    // }
                    // else
                    // {
                    //     GameLogic.AutoIntroPopupClosed = true;
                    // }
                    GameLogic.AutoIntroPopupClosed = true;
                }
            });
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
        }

        public override void Hide(Action onHidden = null)
        {
            ResetUI();
            base.Hide(onHidden);
        }
        #endregion
    }
}
