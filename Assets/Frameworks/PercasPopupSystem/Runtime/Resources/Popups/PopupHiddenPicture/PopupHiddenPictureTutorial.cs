using System;
using UnityEngine;
using Percas.UI;
using Percas.Data;

namespace Percas
{
    public class PopupHiddenPictureTutorial : PopupBase
    {
        [Header("UI")]
        [SerializeField] ButtonBase buttonSkip;
        [SerializeField] ScaleAnim m_animButtonSkip;

        private Action onCallback;

        protected override void Awake()
        {
            RegisterButtons();
        }

        private void InitUI()
        {
            buttonSkip.gameObject.SetActive(false);
        }

        private void RegisterButtons()
        {
            buttonSkip.SetPointerClickEvent(Close);
            m_animButtonSkip.SetOnCompleted(ShowButtonClose);
        }

        private void OnHide(Action callback = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.HiddenPictureTutorial, () =>
            {
                callback?.Invoke();
                onCallback?.Invoke();
                onCallback = null;
            });
        }

        private void ShowButtonClose()
        {
            buttonSkip.gameObject.SetActive(true);
        }

        private void Close()
        {
            OnHide(() =>
            {
                if (!PlayerDataManager.PlayerData.IntroToHiddenPictureHelp)
                {
                    PlayerDataManager.PlayerData.IntroToHiddenPictureHelp = true;
                    PlayerDataManager.OnSave?.Invoke();
                }
            });
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            onCallback = callback;
            base.Show(args, callback);
            InitUI();
        }
        #endregion
    }
}
