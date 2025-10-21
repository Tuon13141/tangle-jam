using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Percas.Data;
using Percas.UI;
using Percas.IAR;
using DG.Tweening;

namespace Percas
{
    public class PopupHiddenPicture : PopupBase
    {
        [Header("UI")]
        [SerializeField] ButtonBase buttonClosePopup;
        [SerializeField] ButtonBase buttonOpenHelp;
        [SerializeField] ButtonBase buttonClaim;
        [SerializeField] ButtonWatchVideoAd buttonWatchVideoAd;
        [SerializeField] ButtonUseCoin buttonLastDay;
        [SerializeField] ButtonBase buttonCollect;
        [SerializeField] GameObject m_congrat;
        [SerializeField] MoveAnim moveAnim;
        [SerializeField] Image m_imageIcon, m_imageBackground;
        [SerializeField] TMP_Text m_textTitle, m_textKeyAmount, m_textTimer, m_textButtonClaim, m_textButtonWatch, m_textCongrat;
        [SerializeField] Slider m_sliderProgress;
        [SerializeField] RectTransform m_rectKeyAmount;
        [SerializeField] List<PopupHiddenPicture_Piece> picturePieces;
        [SerializeField] GameObject m_mask;

        public static Action<Action> OnClosePopup;
        public static Action OnUpdateUI;
        public static Action OnScaleKeyAmount;

        private HiddenPictureDataSO data;
        private Action tutorialCallback;
        private Tween scaleTween;

        private bool loadPictureOnCompleted = false;

        private void OnDestroy()
        {
            scaleTween?.Kill();
        }

        protected override void OnSubscribeEvents()
        {
            OnClosePopup += OnHide;
            OnUpdateUI += UpdateUI;
            OnScaleKeyAmount += ScaleKeyAmount;
            TimeManager.OnTick += HandleButtonClaim;
            TimeManager.OnTick += HandleButtonWatch;
        }

        protected override void OnUnsubscribeEvents()
        {
            OnClosePopup -= OnHide;
            OnUpdateUI -= UpdateUI;
            OnScaleKeyAmount -= ScaleKeyAmount;
            TimeManager.OnTick -= HandleButtonClaim;
            TimeManager.OnTick -= HandleButtonWatch;
        }

        private void OnHide(Action callback = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.HiddenPicture, callback);
        }

        private void InitUI()
        {
            tutorialCallback = null;
            HandleButtonClaim();
            HandleButtonWatch();
            OnShown();
        }

        private void OnShown()
        {
            if (!PlayerDataManager.PlayerData.IntroToHiddenPictureHelp)
            {
                m_mask.SetActive(true);
                tutorialCallback = ShowPictureGrid;
                moveAnim.SetOnCompleted(ShowTutorial);
            }
            else
            {
                moveAnim.SetOnCompleted(ShowPictureGrid);
            }
        }

        private void OnStart()
        {
            m_mask.SetActive(false);
            SetupButtons();
            SetupUI();
        }

        private void ShowTutorial()
        {
            StartCoroutine(OnShowTutorial());
        }

        private IEnumerator OnShowTutorial()
        {
            //yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => loadPictureOnCompleted);
            m_mask.SetActive(false);
            ServiceLocator.PopupScene.ShowPopup(PopupName.HiddenPictureTutorial, null, tutorialCallback);
        }

        private void ShowPictureGrid()
        {
            m_mask.SetActive(true);
            StartCoroutine(SetupPictureGrid());
        }

        private void UpdateUI()
        {
            SetupUI();
            StartCoroutine(SetupPictureGrid());
        }

        private void SetupButtons()
        {
            buttonClosePopup.SetPointerClickEvent(Close);
            buttonOpenHelp.SetPointerClickEvent(OpenHelp);
            buttonClaim.SetPointerClickEvent(Claim);

            buttonWatchVideoAd.reward = new(RewardType.HiddenPictureKey, 1, null);
            buttonWatchVideoAd.skipVideo = false;
            buttonWatchVideoAd.onStart = OnStartWatch;
            buttonWatchVideoAd.onCompleted = OnCompletedWatch;

            buttonLastDay.onStart = OnStartAction;
            buttonLastDay.onCompleted = OnCompletedUseCoin;
            buttonLastDay.onError = OnErrorUseCoin;

            buttonCollect.SetPointerClickEvent(Collect);

            m_imageIcon.sprite = data.EventIcon;
            m_imageBackground.sprite = data.HiddenBackground;
        }

        private void SetupUI()
        {
            m_textTitle.text = $"{data.EventName}";

            m_textKeyAmount.text = $"{Math.Max(HiddenPictureManager.Data.Keys, 0)}";

            bool isLastDay = false;
            try
            {
                TimeSpan remainTime = TimeHelper.ParseIsoString(HiddenPictureManager.Data.EndTime) - DateTime.UtcNow;
                isLastDay = remainTime.TotalDays <= 1;
                m_textTimer.text = $"{Helpers.ConvertTimeToText(remainTime)}";
                if (HiddenPictureManager.Data.UnlockedPieces.Count >= data.PictureSize * data.PictureSize && HiddenPictureManager.Data.PictureCollected)
                {
                    m_textCongrat.text = $"<size=56>CONGRATULATIONS!</size><br>Come back after <color=#E22E3C>{Helpers.ConvertTimeToText(remainTime)}</color> for new painting.";
                }
            }
            catch (Exception) { }

            m_sliderProgress.value = (float)HiddenPictureManager.Data.UnlockedPieces.Count / data.PictureSize * data.PictureSize;

            buttonClaim.gameObject.SetActive(!isLastDay && HiddenPictureManager.Data.UnlockedPieces.Count < data.PictureSize * data.PictureSize);
            buttonWatchVideoAd.gameObject.SetActive(!isLastDay && HiddenPictureManager.Data.UnlockedPieces.Count < data.PictureSize * data.PictureSize);
            buttonLastDay.gameObject.SetActive(isLastDay && HiddenPictureManager.Data.UnlockedPieces.Count < data.PictureSize * data.PictureSize);
            buttonCollect.gameObject.SetActive(HiddenPictureManager.Data.UnlockedPieces.Count >= data.PictureSize * data.PictureSize && !HiddenPictureManager.Data.PictureCollected);
            m_congrat.SetActive(HiddenPictureManager.Data.UnlockedPieces.Count >= data.PictureSize * data.PictureSize && HiddenPictureManager.Data.PictureCollected);
        }

        private IEnumerator SetupPictureGrid()
        {
            List<LevelAsset> levelAssets = data.LevelDatas;
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < levelAssets.Count; i++)
            {
                //picturePieces[i].UpdateUI(Static.GetSpriteHiddenPicture(levelAssets[i]), HiddenPictureManager.Data.IsUnlocked(i));
                picturePieces[i].UpdateUI(DataManager.Instance.GetCurrentHiddenPicturePiece(i), HiddenPictureManager.Data.IsUnlocked(i));
                if (i >= levelAssets.Count - 1)
                {
                    m_mask.SetActive(false);
                    loadPictureOnCompleted = true;
                }
                yield return null;
            }
        }

        private void HandleButtonClaim()
        {
            if (HiddenPictureManager.Data.FreeKeys > 0)
            {
                m_textButtonClaim.text = $"CLAIM";
                return;
            }

            try
            {
                string textRemainTime;
                TimeSpan? remainTime = TimeHelper.ParseIsoString(GameLogic.NextDailyTimeReset) - DateTime.UtcNow;
                if (remainTime?.TotalSeconds > 0)
                {
                    if (remainTime?.TotalHours <= 0)
                    {
                        textRemainTime = string.Format("{0:D2}:{1:D2}", remainTime?.Minutes, remainTime?.Seconds);
                    }
                    else
                    {
                        textRemainTime = string.Format("{0:D2}:{1:D2}:{2:D2}", remainTime?.Hours, remainTime?.Minutes, remainTime?.Seconds);
                    }
                }
                else
                {
                    textRemainTime = $"00:00";
                }
                m_textButtonClaim.text = $"FREE IN<br><size=32>{textRemainTime}</size>";
            }
            catch (Exception) { }
        }

        private void HandleButtonWatch()
        {
            if (HiddenPictureManager.Data.FreeAdKeys > 0)
            {
                m_textButtonWatch.text = $"FREE";
                return;
            }

            try
            {
                string textRemainTime;
                TimeSpan? remainTime = TimeHelper.ParseIsoString(GameLogic.NextDailyTimeReset) - DateTime.UtcNow;
                if (remainTime?.TotalSeconds > 0)
                {
                    if (remainTime?.TotalHours <= 0)
                    {
                        textRemainTime = string.Format("{0:D2}:{1:D2}", remainTime?.Minutes, remainTime?.Seconds);
                    }
                    else
                    {
                        textRemainTime = string.Format("{0:D2}:{1:D2}:{2:D2}", remainTime?.Hours, remainTime?.Minutes, remainTime?.Seconds);
                    }
                }
                else
                {
                    textRemainTime = $"00:00";
                }
                m_textButtonWatch.text = $"FREE IN<br><size=32>{textRemainTime}</size>";
            }
            catch (Exception) { }
        }

        private void Close()
        {
            OnHide();
        }

        private void OpenHelp()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.HiddenPictureTutorial);
        }

        private void Claim()
        {
            PopupHiddenPicture_ProcessReward.OnHidePreviewReward?.Invoke();

            if (HiddenPictureManager.Data.FreeKeys <= 0) return;

            HiddenPictureManager.Data.UpdateFreeKeys(-1);
            HiddenPictureManager.Data.UpdateKeys(1);
            OnScaleKeyAmount?.Invoke();
            SetupUI();
        }

        private void OnStartWatch(Action<bool> onCallback)
        {
            PopupHiddenPicture_ProcessReward.OnHidePreviewReward?.Invoke();

            onCallback?.Invoke(HiddenPictureManager.Data.FreeAdKeys > 0);
        }

        private void OnCompletedWatch()
        {
            HiddenPictureManager.Data.UpdateFreeAdKeys(-1);
            HiddenPictureManager.Data.UpdateKeys(1);
            OnScaleKeyAmount?.Invoke();
            SetupUI();
        }

        private void OnStartAction(Action<bool> onCallback)
        {
            onCallback?.Invoke(true);
        }

        private void OnCompletedUseCoin()
        {
            HiddenPictureManager.Data.UpdateKeys(6);
            OnScaleKeyAmount?.Invoke();
            SetupUI();
        }

        private void OnErrorUseCoin()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.Shop);
        }

        private void Collect()
        {
            OnHide(() =>
            {
                HiddenPictureManager.Data.AddCollectedEvents(DataManager.Instance.GetCurrentHiddenPictureData().ID);
                OnUpdateUI?.Invoke();
                ButtonHiddenPicture.OnUpdateUI?.Invoke();
                ButtonHiddenPicture.OnUpdateNoti?.Invoke();
                // UICurrencyManager.OnShowPictureGain?.Invoke(true, -1, null);
            });
        }

        private void ScaleKeyAmount()
        {
            scaleTween?.Kill();
            Vector3 targetScale = new(1.1f, 1.1f, 1.1f);
            m_rectKeyAmount.localScale = Vector3.one;
            scaleTween = m_rectKeyAmount.DOScale(targetScale, 0.25f).SetLoops(6, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                m_rectKeyAmount.localScale = Vector3.one;
            });
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            data = DataManager.Instance.GetCurrentHiddenPictureData();
            InitUI();
            OnStart();
        }
        #endregion
    }
}
