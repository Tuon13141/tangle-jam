using System;
using System.Collections;
using AssetKits.ParticleImage;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Percas.Data;
using Percas.IAR;
using Percas.UI;
using Sdk.Google.NativeAds;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Percas
{
    public class PopupLevelWinArgs
    {
        public TutorialDataSO nextTutorial;

        public PopupLevelWinArgs(TutorialDataSO nextTutorial)
        {
            this.nextTutorial = nextTutorial;
        }
    }

    public class PopupLevelWin : PopupBase
    {
        [SerializeField] GameObject iconCoilInButton, iconCoinInInfo, iconPinInInfo, iconStarInInfo, m_uiBlock;
        [SerializeField] UILevelImage levelPicture;
        [Header("Buttons")]
        [SerializeField] ButtonShowInter buttonNext;
        [SerializeField] ButtonBase buttonNextWithPreTutorial;
        [SerializeField] ButtonWatchVideoAd buttonX2, buttonStopMeter;
        [SerializeField] RectTransform rectButtonNext, rectButtonNextWithPreTutorial, rectButtonX2, m_rectButtonStopMeter;
        [SerializeField] TMP_Text textButtonX2, m_textButtonNext, m_textButtonNextWithPreTutorial;
        [SerializeField] LuckyBar m_stopMeter;

        [Header("Animations")]
        [SerializeField] SkeletonGraphic skeWellDone;
        [SerializeField] RectTransform rectLevelImage,/* rectPictureEffect,*/ rtPiggyBank, rtNativeAd, m_rectStopMeter;
        [SerializeField] PopupLevelWin_PiggyBank PiggyAnim;
        [SerializeField] ParticleSystem fireworks;
        [SerializeField] private ParticleImage topEffect;

        //[SerializeField] float fromY = 632f;
        //[SerializeField] float toY = -288f;
        [SerializeField] float duration = 0.5f;
        [SerializeField] [Range(0, 1)] float m_RatioActive = 0.5f;
        [SerializeField] AnimationCurve m_AnimLevelImageMove;
        [SerializeField] AnimationCurve m_AnimLevelImageScale;

        private Coroutine coroutineUpdateNativeUI;

        private TutorialDataSO nextTutorial;

        private Tween pictureMoveTween;
        private Tween pictureScaleTween;
        private Tween pictureEffectScaleTween;
        private Tween pictureEffectRotateTween;
        private Tween buttonNextTween;
        private Tween buttonNextWithPreTutorialTween;
        private Tween buttonX2Tween;
        private Tween piggyBankTween;
        private Tween nativeAdTween;
        private Tween stopMeterTween;
        private Tween buttonStopMeterTween;

        private float meterValue = 1;
        private bool isRewarded = false;

        protected override void OnPopupActivated()
        {
            base.OnPopupActivated();
            isRewarded = false;
            UICurrencyManager.BlockBalanceButton = true;
            m_uiBlock.SetActive(false);
            rectButtonNext.localScale = Vector2.zero;
            rectButtonNextWithPreTutorial.localScale = Vector2.zero;
            rectButtonX2.localScale = Vector2.zero;
            m_rectButtonStopMeter.localScale = Vector2.zero;
            m_rectStopMeter.localScale = Vector2.zero;
            rtPiggyBank.localScale = Vector2.one;
            //rectLevelImage.anchoredPosition = new Vector2(rectLevelImage.anchoredPosition.x, fromY);
            //rectLevelImage.localScale = new Vector3(0.0f, 0.0f, 1.0f);
            SetupImage();
        }

        protected override void OnPopupDeactivated()
        {
            base.OnPopupDeactivated();
            if (coroutineUpdateNativeUI != null)
            {
                StopCoroutine(coroutineUpdateNativeUI);
                coroutineUpdateNativeUI = null;
            }
            pictureMoveTween?.Kill();
            pictureScaleTween?.Kill();
            pictureEffectScaleTween?.Kill();
            pictureEffectRotateTween?.Kill();
            buttonNextTween?.Kill();
            buttonNextWithPreTutorialTween?.Kill();
            buttonX2Tween?.Kill();
            piggyBankTween?.Kill();
            nativeAdTween?.Kill();
            stopMeterTween?.Kill();
            buttonStopMeterTween?.Kill();
        }

        private void SetupImage()
        {
            levelPicture.DisplayPicture(true);
        }

        private void OnHide(Action callback = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.LevelWin, () =>
            {
                UICurrencyManager.BlockBalanceButton = false;
                callback?.Invoke();
            });
        }

        private void OnStart()
        {
            if (!PercasSDK.FirebaseManager.RemoteConfig.GetBool("active_new_mode") || Static.isPlayDoneNewMode)
            {
                ActionEvent.OnLevelWin?.Invoke();
            }

            UIWinLevelInfo.OnUpdateUI?.Invoke();

            ActionEvent.OnPlaySFXGameWin?.Invoke();

            if (GameLogic.CanShowNativeAdInPopupWin)
            {
                rtPiggyBank.anchoredPosition = new Vector2(0, rtPiggyBank.anchoredPosition.y);
                rtNativeAd.anchoredPosition = new Vector2(-1600, rtNativeAd.anchoredPosition.y);
            }

            rtNativeAd.gameObject.SetActive(GameLogic.WinLayout == (int)WinLayout.WithNativeAd);
            m_rectStopMeter.gameObject.SetActive(GameLogic.WinLayout == (int)WinLayout.WithStopMeter);

            iconCoinInInfo.SetActive(GameLogic.UnlockHome);
            iconCoilInButton.SetActive(GameLogic.UnlockHome);

            // iconPinInInfo.SetActive(GameLogic.LevelPin > 0);
            // iconStarInInfo.SetActive(GameLogic.LevelStar > 0);

            m_textButtonNext.text = $"<sprite=0> {GameLogic.CoinEarnWinLevel}";
            m_textButtonNextWithPreTutorial.text = $"<sprite=0> {GameLogic.CoinEarnWinLevel}";

            buttonNext.gameObject.SetActive(!(nextTutorial != null || (TutorialManager.TutorialLuxuryBasket != null && !PlayerDataManager.PlayerData.PreIntroToLuxuryBasket)));
            buttonNextWithPreTutorial.gameObject.SetActive(nextTutorial != null || (TutorialManager.TutorialLuxuryBasket != null && !PlayerDataManager.PlayerData.PreIntroToLuxuryBasket));

            buttonNext.onCompleted = OnNextCompleted;

            buttonNextWithPreTutorial.SetPointerClickEvent(OnNextWithPreTutorial);

            if (GameLogic.WinLayout == (int)WinLayout.WithNativeAd)
            {
                textButtonX2.text = $"<sprite=0> {(int)(GameLogic.CoinEarnWinLevel * GameLogic.WinRateOfRewardAd)}";
                buttonStopMeter.gameObject.SetActive(false);
                buttonX2.gameObject.SetActive(GameLogic.UnlockAdButton);
                buttonX2.reward = new(RewardType.CoinAndCoil, 1, null);
                buttonX2.skipVideo = /*GameLogic.VideoWinCount <= GameLogic.FreeVideoWin - 1*/false;
                buttonX2.onStart = OnX2Started;
                buttonX2.onCompleted = OnX2Completed;
                buttonX2.onNotCompleted = OnX2NotCompleted;
            }
            else
            {
                buttonX2.gameObject.SetActive(false);
                buttonStopMeter.gameObject.SetActive(GameLogic.UnlockAdButton);
                buttonStopMeter.reward = new(RewardType.CoinAndCoil, 1, null);
                buttonStopMeter.skipVideo = /*GameLogic.VideoWinCount <= GameLogic.FreeVideoWin - 1*/false;
                buttonStopMeter.onStart = OnStopMeterStarted;
                buttonStopMeter.onCompleted = OnStopMeterCompleted;
                buttonStopMeter.onNotCompleted = OnStopMeterNotCompleted;
            }

            AnimWellDone();
            AnimLevelImage();
            fireworks.Play();
            PiggyAnim.UpdateUI(true, () =>
            {
                if (GameLogic.WinLayout == (int)WinLayout.WithNativeAd)
                {
                    if (GameLogic.CanShowNativeAdInPopupWin)
                    {
                        UpdateAdUI();
                    }
                    else
                    {
                        ShowButtons();
                    }
                }
                else
                {
                    UpdateStopMeterUI();
                }
            });
        }

        private void UpdateAdUI()
        {
            coroutineUpdateNativeUI ??= StartCoroutine(UpdateAdUIAsync());
        }

        private IEnumerator UpdateAdUIAsync()
        {
            float timeOut = 0.5f;
            yield return new WaitUntil(() => (NativeAdsManager.AdLoaded && NativeAdsManager.AdReady) || (timeOut -= Time.deltaTime) <= 0);
            if (NativeAdsManager.AdLoaded && NativeAdsManager.AdReady)
            {
                piggyBankTween = rtPiggyBank.DOAnchorPosX(rtPiggyBank.anchoredPosition.x + 1600f, 0.5f).SetDelay(0.2f).SetEase(Ease.InBack);
                nativeAdTween = rtNativeAd.DOAnchorPosX(0, 0.5f).SetDelay(0.5f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    ShowButtons();
                });
            }
            else
            {
                ShowButtons();
            }
        }

        private void UpdateStopMeterUI()
        {
            if (GameLogic.UnlockAdButton)
            {
                piggyBankTween = rtPiggyBank.DOScale(Vector2.zero, 0.3f).SetDelay(0.2f).SetEase(Ease.InBack).OnComplete(() => m_stopMeter.RunCursor());
                stopMeterTween = m_rectStopMeter.DOScale(Vector2.one, 0.5f).SetDelay(0.6f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    ShowButtons();
                });
            }
            else
            {
                piggyBankTween = rtPiggyBank.DOScale(Vector2.zero, 0.3f).SetDelay(0.2f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    ShowButtons();
                });
            }
        }

        private void ShowButtons()
        {
            // next button
            if (nextTutorial != null || (TutorialManager.TutorialLuxuryBasket != null && !PlayerDataManager.PlayerData.PreIntroToLuxuryBasket))
            {
                buttonNextWithPreTutorialTween = rectButtonNextWithPreTutorial.DOScale(Vector2.one, 0.5f).SetDelay(0.1f).SetEase(Ease.OutBack);
            }
            else
            {
                buttonNextTween = rectButtonNext.DOScale(Vector2.one, 0.5f).SetDelay(0.1f).SetEase(Ease.OutBack);
            }

            // watch button
            if (GameLogic.WinLayout == (int)WinLayout.WithNativeAd)
            {
                buttonX2Tween = rectButtonX2.DOScale(Vector2.one, 0.5f).SetDelay(0.0f).SetEase(Ease.OutBack);
            }
            else
            {
                buttonStopMeterTween = m_rectButtonStopMeter.DOScale(Vector2.one, 0.5f).SetDelay(0.0f).SetEase(Ease.OutBack);
            }
        }

        private void AnimWellDone()
        {
            if (skeWellDone != null)
            {
                skeWellDone.AnimationState.SetAnimation(0, "animation", false).Complete += (trackEntry) =>
                {
                    skeWellDone.AnimationState.SetAnimation(0, "idle", true);
                };
            }
        }



        [NaughtyAttributes.Button]
        private void AnimLevelImage()
        {
            Vector3 rootPos = rectLevelImage.transform.position;
            Transform rootParent = rectLevelImage.transform.parent;
            int rootOrder = rectLevelImage.transform.GetSiblingIndex();

            rectLevelImage.transform.SetParent(transform);
            rectLevelImage.transform.SetAsFirstSibling();
            SetupPointer(LevelController.instance.pictureController.boxCollider, LevelController.instance.mainCamera);
            Sequence sequence = DOTween.Sequence();

            sequence.Append(pictureMoveTween = rectLevelImage.transform.DOMove(rootPos, duration).SetEase(m_AnimLevelImageMove));
            sequence.Insert(0, pictureScaleTween = rectLevelImage.DOScale(1, duration).SetEase(m_AnimLevelImageScale));
            sequence.InsertCallback(m_RatioActive * duration, () =>
            {
                rectLevelImage.transform.SetParent(rootParent);
                rectLevelImage.transform.SetSiblingIndex(rootOrder);
            });

            sequence.InsertCallback(0.75f * duration, () =>
            {
                topEffect.Play();
            });

            sequence.SetDelay(0.3f).Play();
        }

        private void SetupPointer(BoxCollider target, Camera camera)
        {
            Canvas canvas = rectLevelImage.GetComponentInParent<Canvas>();
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            Vector3 worldPos = target.transform.position;
            Vector3 size = target.transform.lossyScale.Multiplication(target.size);

            target.gameObject.SetActive(false);

            Vector3[] screenCorners = new Vector3[4];
            rectLevelImage.GetWorldCorners(screenCorners);

            var scrennPos = new Vector3[4];

            scrennPos[0] = camera.WorldToScreenPoint(worldPos + Vector3.left * size.x * 0.5f);
            scrennPos[1] = camera.WorldToScreenPoint(worldPos + Vector3.right * size.x * 0.5f);
            scrennPos[2] = canvas.worldCamera.WorldToScreenPoint(screenCorners[1]);
            scrennPos[3] = canvas.worldCamera.WorldToScreenPoint(screenCorners[2]);

            var width1 = Mathf.Abs(scrennPos[0].x - scrennPos[1].x);
            var width2 = Mathf.Abs(scrennPos[2].x - scrennPos[3].x);

            var pos = camera.WorldToScreenPoint(worldPos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                pos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out var localPos);


            rectLevelImage.localPosition = localPos;


            if (width1 == 0)
            {
                Debug.LogError("Error1");
                return;
            }
            if (width2 == 0)
            {
                Debug.LogError("Error2");
                return;
            }

            rectLevelImage.transform.localScale = Vector3.one * (width1 / width2);
        }

        private void OnNextCompleted()
        {
            OnHide(() =>
            {
                OnNextRewards();
            });
        }

        private void OnNextWithPreTutorial()
        {
            OnHide(() =>
            {
                if (nextTutorial != null)
                {
                    TutorialManager.OnPreOpenTutorial?.Invoke(nextTutorial, () =>
                    {
                        OnNextRewards();
                    });
                }
                else if (TutorialManager.TutorialLuxuryBasket != null && !PlayerDataManager.PlayerData.PreIntroToLuxuryBasket)
                {
                    PlayerDataManager.PlayerData.PreIntroToLuxuryBasket = true;
                    PlayerDataManager.OnSave?.Invoke();
                    TutorialManager.OnPreOpenTutorial?.Invoke(TutorialManager.TutorialLuxuryBasket, () =>
                    {
                        OnNextRewards();
                    });
                }
                else
                {
                    OnNextRewards();
                }
            });
        }

        private void OnNextRewards()
        {
            m_uiBlock.SetActive(true);
            if (PercasSDK.FirebaseManager.RemoteConfig.GetBool("active_new_mode") && !Static.isPlayDoneNewMode)
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                GameLoading.OnShow?.Invoke(null, false);
            }
            else if (!GameLogic.UnlockHome)
            {
                GlobalSetting.OnHomeToGame?.Invoke(() =>
                {
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin(GameLogic.CoinEarnWinLevel, Vector3.zero, new LogCurrency("currency", "coin", "level_win", "non_iap", "feature", "level_win")));
                    RewardGainController.OnStartGaining?.Invoke();
                });
            }
            else
            {
                SetupRewardGain(1);
                GlobalSetting.OnGameToHome?.Invoke(null);
            }
        }

        #region Button X2
        private void OnX2Started(Action<bool> onCallback)
        {
            m_uiBlock.SetActive(true);
            onCallback?.Invoke(true);
        }

        private void OnX2Completed()
        {
            OnHide(() =>
            {
                if (nextTutorial != null)
                {
                    TutorialManager.OnPreOpenTutorial?.Invoke(nextTutorial, () =>
                    {
                        OnX2Rewards();
                    });
                }
                else if (TutorialManager.TutorialLuxuryBasket != null && !PlayerDataManager.PlayerData.PreIntroToLuxuryBasket)
                {
                    PlayerDataManager.PlayerData.PreIntroToLuxuryBasket = true;
                    PlayerDataManager.OnSave?.Invoke();
                    TutorialManager.OnPreOpenTutorial?.Invoke(TutorialManager.TutorialLuxuryBasket, () =>
                    {
                        OnX2Rewards();
                    });
                }
                else
                {
                    OnX2Rewards();
                }
            });
        }

        private void OnX2NotCompleted()
        {
            m_uiBlock.SetActive(false);
        }

        private void OnX2Rewards()
        {
            PlayerDataManager.PlayerData.AddVideoWinCount();
            if (!GameLogic.UnlockHome)
            {
                GlobalSetting.OnHomeToGame?.Invoke(() =>
                {
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin(GameLogic.CoinEarnWinLevel * GameLogic.WinRateOfRewardAd, Vector3.zero, new LogCurrency("currency", "coin", "level_win", "non_iap", "ads", "rwd_ads")));
                    RewardGainController.OnStartGaining?.Invoke();
                });
            }
            else
            {
                SetupRewardGain(GameLogic.WinRateOfRewardAd);
                GlobalSetting.OnGameToHome?.Invoke(null);
            }
        }
        #endregion

        #region Button Stop Meter
        private void OnStopMeterStarted(Action<bool> onCallback)
        {
            Debug.LogError($"OnStopMeterStarted");
            m_uiBlock.SetActive(true);
            meterValue = m_stopMeter.StopCursor();
            onCallback?.Invoke(true);
        }

        private void OnStopMeterCompleted()
        {
            Debug.LogError($"OnStopMeterCompleted");
            OnHide(() =>
            {
                if (nextTutorial != null)
                {
                    TutorialManager.OnPreOpenTutorial?.Invoke(nextTutorial, () =>
                    {
                        OnStopMeterRewards();
                    });
                }
                else if (TutorialManager.TutorialLuxuryBasket != null && !PlayerDataManager.PlayerData.PreIntroToLuxuryBasket)
                {
                    PlayerDataManager.PlayerData.PreIntroToLuxuryBasket = true;
                    PlayerDataManager.OnSave?.Invoke();
                    TutorialManager.OnPreOpenTutorial?.Invoke(TutorialManager.TutorialLuxuryBasket, () =>
                    {
                        OnStopMeterRewards();
                    });
                }
                else
                {
                    OnStopMeterRewards();
                }
            });

        }

        private void OnStopMeterNotCompleted()
        {
            Debug.LogError($"OnStopMeterNotCompleted");
            m_uiBlock.SetActive(false);
            m_stopMeter.RunCursor();
        }

        private void OnStopMeterRewards()
        {
            PlayerDataManager.PlayerData.AddVideoWinCount();
            if (!GameLogic.UnlockHome)
            {
                GlobalSetting.OnHomeToGame?.Invoke(() =>
                {
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin((int)(GameLogic.CoinEarnWinLevel * meterValue), Vector3.zero, new LogCurrency("currency", "coin", "level_win", "non_iap", "ads", "rwd_ads")));
                    RewardGainController.OnStartGaining?.Invoke();
                });
            }
            else
            {
                SetupRewardGain(meterValue);
                GlobalSetting.OnGameToHome?.Invoke(null);
            }
        }
        #endregion

        private void SetupRewardGain(float earnRate)
        {
            if (GameLogic.UnlockHome && !isRewarded)
            {
                isRewarded = true;
                RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin((int)(GameLogic.CoinEarnWinLevel * earnRate), Vector3.zero, new LogCurrency("currency", "coin", "level_win", "non_iap", earnRate == 1 ? "feature" : "ads", earnRate == 1 ? "level_win" : "rwd_ads")));
                if (GameLogic.ThreadEarnWinLevel == 0)
                {
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoil((int)(GameLogic.LevelCoil), Vector3.zero, new LogCurrency("currency", "coil", "level_win", "non_iap", "feature", "level_win")));
                }
                else
                {
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoil((int)(GameLogic.ThreadEarnWinLevel), Vector3.zero, new LogCurrency("currency", "coil", "level_win", "non_iap", "feature", "level_win")));
                }
                RewardGainController.OnAddRewardGain?.Invoke(new RewardGainPin(GameLogic.LevelPin, Vector3.zero, new LogCurrency("currency", "pin", "level_win", "non_iap", "feature", "level_win")));
            }
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            if (args is PopupLevelWinArgs popupArgs)
            {
                nextTutorial = popupArgs.nextTutorial;
            }
            base.Show(args, callback);
            PiggyAnim.gameObject.SetActive(true);
            OnStart();
        }

        public override void Hide(Action onHidden = null)
        {
            PiggyAnim.gameObject.SetActive(false);
            base.Hide(onHidden);
        }
        #endregion
    }
}
