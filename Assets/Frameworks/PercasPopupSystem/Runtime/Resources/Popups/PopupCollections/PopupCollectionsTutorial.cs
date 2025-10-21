using System;
using UnityEngine;
using Percas.Data;
using Percas.UI;

namespace Percas
{
    public class PopupCollectionsTutorial : PopupBase
    {
        [Header("UI")]
        [SerializeField] ButtonBase buttonSkip;
        [SerializeField] ScaleAnim m_animButtonSkip;

        protected override void Awake()
        {
            buttonSkip.SetPointerClickEvent(Close);
            m_animButtonSkip.SetOnCompleted(ShowButtonClose);
        }

        private void OnHide(Action callback = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.CollectionsTutorial, callback);
        }

        private void InitUI()
        {
            buttonSkip.gameObject.SetActive(false);
        }

        private void Close()
        {
            OnHide(() =>
            {
                if (!PlayerDataManager.PlayerData.IntroToCollectionTutorial)
                {
                    PlayerDataManager.PlayerData.IntroToCollectionTutorial = true;
                    PlayerDataManager.OnSave?.Invoke();
                }
            });
        }

        private void ShowButtonClose()
        {
            buttonSkip.gameObject.SetActive(true);
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
