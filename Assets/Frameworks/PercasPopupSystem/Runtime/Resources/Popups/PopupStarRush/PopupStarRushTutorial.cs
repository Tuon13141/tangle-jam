using System;
using UnityEngine;
using Percas.UI;
using Percas.Data;

namespace Percas
{
    public class PopupStarRushTutorial : PopupBase
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

        private void InitUI()
        {
            buttonSkip.gameObject.SetActive(false);
        }

        private void OnHide(Action callback = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.StarRushTutorial, callback);
        }

        private void ShowButtonClose()
        {
            buttonSkip.gameObject.SetActive(true);
        }

        private void Close()
        {
            OnHide(() =>
            {
                if (!PlayerDataManager.PlayerData.IntroToStarRushTutorial)
                {
                    PlayerDataManager.PlayerData.IntroToStarRushTutorial = true;
                    PlayerDataManager.OnSave?.Invoke();
                }
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
