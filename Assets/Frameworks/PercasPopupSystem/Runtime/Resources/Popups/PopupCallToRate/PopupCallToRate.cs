using System;
using UnityEngine;
using Percas.UI;
using Percas.Data;

namespace Percas
{
    public class PopupCallToRate : PopupBase
    {
        [SerializeField] ButtonBase buttonClosePopup;
        [SerializeField] ButtonBase buttonRate;
        [SerializeField] GameObject m_question;

        public static Action<int> OnUpdateStatus;
        public static Action<bool> OnShowButtonRate;
        public static int CurrentStar { get; set; }

        protected override void Awake()
        {
            RegisterButtons();
        }

        protected override void OnSubscribeEvents()
        {
            OnShowButtonRate += ShowButtonRate;
        }

        protected override void OnUnsubscribeEvents()
        {
            OnShowButtonRate -= ShowButtonRate;
        }

        private void RegisterButtons()
        {
            buttonClosePopup.SetPointerClickEvent(Close);
            buttonRate.SetPointerClickEvent(Rate);
        }

        private void OnHide(Action onCompleted = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.CallToRate, () =>
            {
                onCompleted?.Invoke();
                GameLogic.AutoIntroPopupClosed = true;
            });
        }

        private void ShowButtonRate(bool value)
        {
            m_question.SetActive(false);
            buttonRate.gameObject.SetActive(value);
        }

        private void InitUI()
        {
            CurrentStar = 0;
            buttonRate.gameObject.SetActive(false);
            m_question.SetActive(CurrentStar <= 0);
        }

        private void Close()
        {
            OnHide();
        }

        private void Rate()
        {
            OnHide(() =>
            {
                if (CurrentStar >= 5)
                {
                    ActionEvent.OnCallGoogleInAppReviews?.Invoke();
                }
                else
                {
                    ActionEvent.OnShowToast?.Invoke($"Thanks for your feedback!");
                }
                PlayerDataManager.PlayerData.SetGameRated(CurrentStar);
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
