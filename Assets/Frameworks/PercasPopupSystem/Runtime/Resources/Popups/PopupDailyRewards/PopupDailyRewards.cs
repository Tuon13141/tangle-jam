using System;
using UnityEngine;
using Percas.Data;
using Percas.UI;

namespace Percas
{
    public class PopupDailyRewardsArgs
    {
        public bool isSalePopup;

        public PopupDailyRewardsArgs(bool isSalePopup)
        {
            this.isSalePopup = isSalePopup;
        }
    }

    public class PopupDailyRewards : PopupBase
    {
        [SerializeField] ButtonBase buttonClosePopup;

        bool isSalePopup = false;

        protected override void Awake()
        {
            buttonClosePopup.SetPointerClickEvent(Close);
        }

        protected override void OnSubscribeEvents()
        {
            ButtonDailyRewards.OnClosePopup += Close;
        }

        protected override void OnUnsubscribeEvents()
        {
            ButtonDailyRewards.OnClosePopup -= Close;
        }

        private void Close()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.DailyRewards, () =>
            {
                PlayerDataManager.PlayerData.IntroToDailyReward = true;
                PlayerDataManager.OnSave?.Invoke();
                if (isSalePopup)
                {
                    GameLogic.AutoSalePopupClosed = true;
                }
                else
                {
                    GameLogic.AutoIntroPopupClosed = true;
                }
            });
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            if (args is PopupDailyRewardsArgs popupArgs)
            {
                isSalePopup = popupArgs.isSalePopup;
            }
            base.Show(args, callback);
        }
        #endregion
    }
}
