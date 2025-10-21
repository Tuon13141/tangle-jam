using System;
using UnityEngine;
using DG.Tweening;
using Percas.Data;
using Percas.UI;

namespace Percas
{
    public class PopupCollectionsDemo : PopupBase
    {
        [Header("UI")]
        [SerializeField] ButtonBase buttonClosePopup;
        [SerializeField] ButtonBase buttonSell;
        [SerializeField] ScaleAnim m_animButtonSell;
        [SerializeField] RectTransform m_buttonBack, m_buttonSell, m_textQuote;
        [SerializeField] UILevelImage levelImage;
        [SerializeField] GameObject m_collectHand, m_closeHand;

        private Sequence sequence;

        protected override void Awake()
        {
            buttonClosePopup.SetPointerClickEvent(Close);
            buttonSell.SetPointerClickEvent(OnSell);
            m_animButtonSell.SetOnCompleted(ShowTutorial);
        }

        private void OnDestroy()
        {
            sequence?.Kill();
        }

        private void OnHide(Action callback = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.CollectionsDemo, () =>
            {
                callback?.Invoke();
                if (!PlayerDataManager.PlayerData.IntroToCollectionDemo)
                {
                    PlayerDataManager.PlayerData.IntroToCollectionDemo = true;
                    PlayerDataManager.OnSave?.Invoke();
                }
            });
        }

        private void InitUI()
        {
            buttonClosePopup.gameObject.SetActive(false);
        }

        private void OnStart()
        {
            m_buttonBack.anchoredPosition = new Vector2(-444, 0);
            m_textQuote.localScale = Vector2.zero;
            UpdateLevelPicture();
        }

        private void ShowCollectHand(bool value)
        {
            m_collectHand.SetActive(value);
        }

        private void ShowCloseHand(bool value)
        {
            m_closeHand.SetActive(value);
        }

        private void ShowTutorial()
        {
            ShowCollectHand(true);
        }

        private void UpdateLevelPicture()
        {
            levelImage.ReloadClassic(GameConfig.Instance.MaxLevel, () =>
            {
                levelImage.DisplayPicture(false);
            }, () =>
            {
                levelImage.DisplayPicture(true);
            });
        }

        private void Close()
        {
            OnHide(() =>
            {
                if (!PlayerDataManager.PlayerData.IntroToCollectionTutorial)
                {
                    ServiceLocator.PopupScene.ShowPopup(PopupName.CollectionsTutorial);
                }
            });
        }

        private void OnSell()
        {
            ShowCollectHand(false);
            sequence = DOTween.Sequence();
            sequence.Append(m_buttonSell.DOScale(Vector2.zero, 0.3f).SetEase(Ease.InOutQuart));
            sequence.AppendInterval(0.25f);
            sequence.Append(m_textQuote.DOScale(Vector2.one, 0.3f).SetEase(Ease.OutBack));
            sequence.AppendInterval(0.5f);
            sequence.Append(m_buttonBack.DOAnchorPosX(56, 0.5f).SetEase(Ease.OutBack));
            sequence.AppendCallback(() =>
            {
                ShowCloseHand(true);
                buttonClosePopup.gameObject.SetActive(true);
            });
            sequence.Play();
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            InitUI();
            ShowCollectHand(false);
            ShowCloseHand(false);
            OnStart();
        }
        #endregion
    }
}
