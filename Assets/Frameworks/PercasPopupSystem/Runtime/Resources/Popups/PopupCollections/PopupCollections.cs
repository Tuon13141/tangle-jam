using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Percas.Data;
using Percas.UI;
using Percas.IAR;

namespace Percas
{
    public class PopupCollections : PopupBase
    {
        [Header("UI")]
        [SerializeField] ButtonBase buttonClosePopup;
        [SerializeField] ButtonBase buttonHelp;
        [SerializeField] ButtonBase buttonMenuClassic, buttonMenuEvent;
        [SerializeField] Image m_imageMenuClassic, m_imageMenuEvent;
        [SerializeField] List<Sprite> buttonBackgrounds;

        [SerializeField] ButtonBase buttonPrev, buttonNext;
        [SerializeField] ButtonBase buttonSell;
        [SerializeField] ScaleAnim m_animButtonSell;

        [SerializeField] ButtonBase buttonClaim;
        [SerializeField] ButtonWatchVideoAd buttonWatchVideoAd;
        [SerializeField] ButtonBase buttonCancel;

        [SerializeField] UILevelImage levelImage;
        [SerializeField] TMP_Text textQuote;
        [SerializeField] GameObject m_bars, m_nativeAd;
        [SerializeField] LuckyBar luckyBar;

        [SerializeField] RectTransform m_rectSell, m_rectTabs;
        [SerializeField] RectTransform rectPicture;
        [SerializeField] float targetScaleMultiplier = 1.48f; // 1.48 = 888 / 600
        [SerializeField] float animationDuration = 0.5f;

        private int currentLevel = -1;
        private float adRewardRate = 1;

        private Tween moveTween;
        private Tween scaleTween;

        private Vector2 originalPicturePosition;

        bool canShowQuote = false;

        bool isClassicPictures = true;

        private void OnDestroy()
        {
            moveTween?.Kill();
            scaleTween?.Kill();
        }

        private void OnHide(Action callback = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.Collections, callback);
        }

        private void ResetUI()
        {
            rectPicture.anchoredPosition = originalPicturePosition;
            rectPicture.localScale = Vector2.one;
            m_rectSell.anchoredPosition = new Vector2(0, 0);
        }

        private void OnStart()
        {
            isClassicPictures = true;
            m_imageMenuClassic.sprite = buttonBackgrounds[1];
            m_imageMenuEvent.sprite = buttonBackgrounds[0];
            m_rectTabs.gameObject.SetActive(true);
            buttonMenuClassic.gameObject.SetActive(GameLogic.CurrentLevel >= GameLogic.LevelUnlockHiddenPicture && (DataManager.Instance.GetActiveHiddenPictureEventID() >= 0 || DataManager.Instance.GetLastestCollectedHiddenPictureEventID() >= 0));
            buttonMenuEvent.gameObject.SetActive(GameLogic.CurrentLevel >= GameLogic.LevelUnlockHiddenPicture && (DataManager.Instance.GetActiveHiddenPictureEventID() >= 0 || DataManager.Instance.GetLastestCollectedHiddenPictureEventID() >= 0));

            currentLevel = Math.Max(1, GameLogic.CurrentLevel - 1);

            DisplayUIView(true);
            DisplayUISell(false);

            buttonClosePopup.SetPointerClickEvent(Close);
            buttonHelp.SetPointerClickEvent(OpenHelp);
            buttonMenuClassic.SetPointerClickEvent(ShowClassic);
            buttonMenuEvent.SetPointerClickEvent(ShowEvent);

            SetupViewButtons();
            SetupSellButtons();
            originalPicturePosition = rectPicture.anchoredPosition;

            m_animButtonSell.SetOnCompleted(ShowTutorial);
        }

        private void SetupViewButtons()
        {
            buttonPrev.SetPointerClickEvent(OnPrev);
            buttonNext.SetPointerClickEvent(OnNext);
            buttonSell.SetPointerClickEvent(OnSell);
        }

        private void SetupSellButtons()
        {
            buttonCancel.SetPointerClickEvent(OnCancel);
            buttonClaim.SetPointerClickEvent(OnClaim);
            buttonWatchVideoAd.skipVideo = false;
            buttonWatchVideoAd.onStart = OnWatchStarted;
            buttonWatchVideoAd.onCompleted = OnWatchCompleted;
            buttonWatchVideoAd.onNotCompleted = OnWatchNotCompleted;
        }

        private void ShowTutorial()
        {
            if (!PlayerDataManager.PlayerData.IntroToCollectionTutorial)
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.CollectionsTutorial);
            }
        }

        private void Close()
        {
            OnHide();
        }

        private void OpenHelp()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.CollectionsTutorial);
        }

        private void ShowClassic()
        {
            if (isClassicPictures == true) return;
            isClassicPictures = true;
            m_imageMenuClassic.sprite = buttonBackgrounds[1];
            m_imageMenuEvent.sprite = buttonBackgrounds[0];
            UpdateLevelPicture();
        }

        private void ShowEvent()
        {
            if (DataManager.Instance.GetActiveHiddenPictureEventID() < 0 && DataManager.Instance.GetLastestCollectedHiddenPictureEventID() < 0)
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_COMING_SOON);
                return;
            }

            if (isClassicPictures == false) return;
            isClassicPictures = false;
            m_imageMenuClassic.sprite = buttonBackgrounds[0];
            m_imageMenuEvent.sprite = buttonBackgrounds[1];
            UpdateLevelPicture();
        }

        private void UpdateLevelPicture()
        {
            if (isClassicPictures)
            {
                levelImage.ReloadClassic(currentLevel, () =>
                {
                    levelImage.DisplayPicture(false);
                }, () =>
                {
                    levelImage.DisplayPicture(true);
                });
            }
            else
            {
                int eventID = DataManager.Instance.GetActiveHiddenPictureEventID() >= 0 ? DataManager.Instance.GetActiveHiddenPictureEventID() : DataManager.Instance.GetLastestCollectedHiddenPictureEventID();
                levelImage.ReloadHiddenPicture(eventID, () =>
                {
                    levelImage.DisplayPicture(false);
                }, () =>
                {
                    levelImage.DisplayPicture(true);
                });
            }
            UpdateUIView();
        }

        private void UpdateQuote()
        {
            if (!canShowQuote) return;
            int pictureQuote = PlayerDataManager.GetPictureQuoteByLevel(currentLevel);
            textQuote.text = $"\"{DataManager.Instance.GetQuoteByIndex(pictureQuote)}\"";
        }

        private void OnPrev()
        {
            if (currentLevel == 1) return;
            currentLevel = Math.Clamp(currentLevel - 1, 1, GameLogic.CurrentLevel - 1);
            UpdateLevelPicture();
            DisplayUIViewButtons(true);
        }

        private void OnNext()
        {
            if (currentLevel == GameLogic.CurrentLevel - 1)
            {
                ActionEvent.OnShowToast?.Invoke($"Pass Level {currentLevel + 1} To Unlock");
                return;
            }
            currentLevel = Math.Clamp(currentLevel + 1, 1, GameLogic.CurrentLevel - 1);
            UpdateLevelPicture();
            DisplayUIViewButtons(true);
        }

        private void UpdateUIView()
        {
            if (isClassicPictures)
            {
                DisplayUIView(true);
            }
            else
            {
                buttonPrev.gameObject.SetActive(false);
                buttonNext.gameObject.SetActive(false);
                buttonSell.gameObject.SetActive(false);
                textQuote.text = $"\"{DataManager.Instance.GetQuoteByIndex(HiddenPictureManager.Data.EventQuoteID)}\"";
                textQuote.gameObject.SetActive(true);
            }
        }

        private void DisplayUIView(bool value)
        {
            buttonPrev.gameObject.SetActive(value);
            buttonNext.gameObject.SetActive(value);
            DisplayUIViewButtons(value);
            m_nativeAd.SetActive(value);
        }

        private void DisplayUIViewButtons(bool value)
        {
            textQuote.gameObject.SetActive(value && PlayerDataManager.IsInPictureCoinReceived(currentLevel));
            buttonSell.gameObject.SetActive(value && !PlayerDataManager.IsInPictureCoinReceived(currentLevel));
            canShowQuote = value && PlayerDataManager.IsInPictureCoinReceived(currentLevel);
            UpdateQuote();
        }

        private void DisplayUISell(bool value)
        {
            buttonClaim.gameObject.SetActive(value);
            buttonWatchVideoAd.gameObject.SetActive(value);
            buttonCancel.gameObject.SetActive(value);
            m_bars.SetActive(value);
        }

        private async void OnSell()
        {
            await ZoomIn();
        }

        private async UniTask ZoomOut()
        {
            // move to original position
            m_rectSell.anchoredPosition = new Vector2(0, 0);
            m_rectTabs.gameObject.SetActive(true);

            DisplayUISell(false);
            moveTween = rectPicture.DOAnchorPos(originalPicturePosition, animationDuration).SetEase(Ease.OutQuad);
            scaleTween = rectPicture.DOScale(Vector2.one, animationDuration).SetEase(Ease.OutQuad);
            await UniTask.Delay(300);
            DisplayUIView(true);
        }

        private async UniTask ZoomIn()
        {
            // move to new position
            m_rectSell.anchoredPosition = new Vector2(0, -164f);
            m_rectTabs.gameObject.SetActive(false);

            originalPicturePosition = rectPicture.anchoredPosition;
            DisplayUIView(false);
            moveTween = rectPicture.DOAnchorPos(new Vector2(0, 100f), animationDuration).SetEase(Ease.OutQuad); // 264f
            scaleTween = rectPicture.DOScale(Vector2.one * targetScaleMultiplier, animationDuration).SetEase(Ease.OutQuad);
            await UniTask.Delay(300);
            luckyBar.RunCursor();
            DisplayUISell(true);
        }

        private async void OnCancel()
        {
            await ZoomOut();
        }

        private void OnClaim()
        {
            RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin(GameLogic.FreePictureLevelCoin, Vector3.zero, new LogCurrency("currency", "coin", "collection", "non_iap", "feature", "collect_picture_coin")));
            RewardGainController.OnStartGaining?.Invoke();
            PlayerDataManager.AddPictureCoin(currentLevel);
            OnCancel();
        }

        private void OnWatchStarted(Action<bool> onCallback)
        {
            adRewardRate = luckyBar.StopCursor();
            onCallback?.Invoke(true);
        }

        private void OnWatchCompleted()
        {
            RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin((int)(GameLogic.FreePictureLevelCoin * adRewardRate), Vector3.zero, new LogCurrency("currency", "coin", "collection", "non_iap", "ads", "rwd_ads")));
            RewardGainController.OnStartGaining?.Invoke();
            PlayerDataManager.AddPictureCoin(currentLevel);
            OnCancel();
        }

        private void OnWatchNotCompleted()
        {
            luckyBar.RunCursor();
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            OnStart();
            UpdateLevelPicture();
        }

        public override void Hide(Action onHidden = null)
        {
            ResetUI();
            base.Hide(onHidden);
        }
        #endregion
    }
}
