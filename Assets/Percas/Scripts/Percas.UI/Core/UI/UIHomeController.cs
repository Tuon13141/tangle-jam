using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Percas.Data;
using Percas.IAR;
using Percas.IAA;

namespace Percas.UI
{
    public class UIHomeController : MonoBehaviour
    {
        [SerializeField] GameObject m_HomeCanvas, m_RoomCanvas;
        [SerializeField] List<RectTransform> leftMenus;
        [SerializeField] List<RectTransform> rightMenus;
        [SerializeField] RectTransform m_rectRoomNavigation;
        [SerializeField] RectTransform rtButtonPlay, /*rtQuote, */rtBottomMenus;
        [SerializeField] RectTransform m_iconCollections;
        [SerializeField] List<Vector2> leftPos;
        [SerializeField] List<Vector2> rightPos;
        [SerializeField] Vector2 bottomPos;

        public static Action<bool, bool> OnDisplay;
        public static Action OnCollectionScale;

        private Tween leftMenuTween;
        private Tween rightMenuTween;
        private Tween bottomMenuTween;
        private Tween scaleTween;

        private float bannerGap = 176f;

        private void Awake()
        {
            OnDisplay += Display;
            OnCollectionScale += ScaleCollection;
        }

        private void OnDestroy()
        {
            OnDisplay -= Display;
            OnCollectionScale -= ScaleCollection;

            leftMenuTween?.Kill();
            rightMenuTween?.Kill();
            bottomMenuTween?.Kill();
            scaleTween?.Kill();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.LogError($"BUILD");
                OnDisplay?.Invoke(false, true);
            }
        }

        private void Start()
        {
            Debug.LogError($"TotalCoil = {GameLogic.TotalCoil}");

            SetupUIPosition();

            if (GlobalSetting.IsFirstOpen)
            {
                if (!GlobalSetting.IsJustPlay && GameLogic.CanShowAppOpenWhenOpenGame && GameLogic.SessionStartCount >= GameConfig.Instance.SessionStartCountToShowAppOpen)
                {
                    IAAManager.ShowAppOpenAd(null, null);
                }
                OnDisplay?.Invoke(false, false);
                GlobalSetting.IsFirstOpen = false;
            }
        }

        private void SetupUIPosition()
        {
            bannerGap = GameLogic.IsNoAds ? 176f : 0f;
            m_rectRoomNavigation.anchoredPosition = new Vector2(m_rectRoomNavigation.anchoredPosition.x, m_rectRoomNavigation.anchoredPosition.y - bannerGap);
            rtButtonPlay.anchoredPosition = new Vector2(rtButtonPlay.anchoredPosition.x, rtButtonPlay.anchoredPosition.y - bannerGap);
            // rtQuote.anchoredPosition = new Vector2(rtQuote.anchoredPosition.x, rtQuote.anchoredPosition.y - bannerGap);
        }

        private void Display(bool autoPopups, bool autoBuildRoom)
        {
            UICurrencyManager.OnShowBlockMask?.Invoke(true);
            DisplayAsync(autoPopups, autoBuildRoom);
        }

        private void ScaleCollection()
        {
            Vector3 targetScale = new(1.1f, 1.1f, 1.1f);
            transform.localScale = Vector3.one;
            scaleTween = m_iconCollections.DOScale(targetScale, 0.2f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                m_iconCollections.localScale = Vector3.one;
            });
        }

        private async void DisplayAsync(bool autoPopups, bool autoBuildRoom)
        {
            try
            {
                if (GameLogic.OutOfRoom || GameLogic.TotalCoil <= 0 || !autoBuildRoom)
                {
                    GameLogic.IsShowingRoom = false;

                    RoomController.OnUpdateNoti?.Invoke();

                    //UICurrencyManager.OnShowBalance?.Invoke(true, true, true);
                    await ShowMenus();
                    await GainRewards();

                    if (GameLogic.RewardsGaining) return;

                    if (autoPopups)
                    {
                        await AutoShowIntroPopups();
                    }
                    else
                    {
                        await AutoShowSalePopups();
                    }
                    // await AutoShowEventPopups();

                    UICurrencyManager.OnShowBlockMask?.Invoke(false);

                    // if (canGainPicture && GameLogic.CurrentLevel >= GameLogic.LevelUnlockCollections && ((GameLogic.IsClassicMode && !PlayerDataManager.IsInPictureGained(GameLogic.CurrentLevel - 1)) || (GameLogic.IsHiddenPictureMode && !HiddenPictureManager.Data.IsCollectedPiece($"{HiddenPictureManager.Data.EventID}_{GlobalSetting.HiddenPictureLevelIndex}"))))
                    // {
                    //     UICurrencyManager.OnShowBlockMask?.Invoke(false);

                    //     if (hasIntroPopup)
                    //     {
                    //         await UniTask.WaitUntil(() => GameLogic.AutoIntroPopupClosed);
                    //     }

                    //     if (hasSalePopups)
                    //     {
                    //         await UniTask.WaitUntil(() => GameLogic.AutoSalePopupClosed);
                    //     }

                    //     if (hasEventPopups)
                    //     {
                    //         await UniTask.WaitUntil(() => GameLogic.AutoEventPopupClosed);
                    //     }

                    //     UICurrencyManager.OnShowBlockMask?.Invoke(true);

                    //     await UniTask.Delay(250);

                    //     if (GameLogic.IsClassicMode)
                    //     {
                    //         UICurrencyManager.OnShowPictureGain?.Invoke(false, GameLogic.CurrentLevel - 1, () =>
                    //         {
                    //             UICurrencyManager.OnShowBlockMask?.Invoke(false);
                    //         });
                    //     }
                    //     else if (GameLogic.IsHiddenPictureMode)
                    //     {
                    //         UICurrencyManager.OnShowPictureGain?.Invoke(true, GlobalSetting.HiddenPictureLevelIndex, () =>
                    //         {
                    //             UICurrencyManager.OnShowBlockMask?.Invoke(false);
                    //         });
                    //     }
                    // }
                    // else
                    // {
                    //     UICurrencyManager.OnShowBlockMask?.Invoke(false);
                    // }
                }
                else
                {
                    GameLogic.IsShowingRoom = true;

                    RoomController.OnUpdateNoti?.Invoke();

                    await HideMenus();
                    m_HomeCanvas.SetActive(false);
                    //UICurrencyManager.OnShowBalance?.Invoke(false, false, false);
                    await UniTask.Delay(200);
                    RoomController.OnUpdateUI?.Invoke(() =>
                    {
                        m_RoomCanvas.SetActive(true);
                        UICurrencyManager.OnShowBlockMask?.Invoke(false);
                    }, false);
                }
            }
            catch (Exception) { }
        }

        private async UniTask ShowMenus()
        {
            try
            {
                m_HomeCanvas.SetActive(true);
                m_RoomCanvas.SetActive(false);

                // Left Menus
                int leftMenuIndex = -1;
                int leftMenuInactiveCount = 0;
                foreach (RectTransform menu in leftMenus)
                {
                    if (menu.gameObject.activeSelf)
                    {
                        leftMenuIndex += 1;
                        leftMenuTween = menu.DOAnchorPosX(leftPos[leftMenuIndex].y, 0.5f).SetDelay(0.1f * leftMenuIndex).SetEase(Ease.OutBack);
                    }
                    else
                    {
                        leftMenuInactiveCount += 1;
                    }
                }

                // Right Menus
                int rightMenuIndex = -1;
                foreach (RectTransform menu in rightMenus)
                {
                    if (menu.gameObject.activeSelf)
                    {
                        rightMenuIndex += 1;
                        rightMenuTween = menu.DOAnchorPosX(rightPos[rightMenuIndex].y, 0.5f).SetDelay(0.1f * rightMenuIndex).SetEase(Ease.OutBack);
                    }
                }

                // Bottom Menus
                bottomMenuTween = rtBottomMenus.DOAnchorPosY(bottomPos.y - bannerGap, 0.5f).SetEase(Ease.OutQuart);

                await UniTask.Delay(200);

                // Button Play
                if (m_rectRoomNavigation != null) m_rectRoomNavigation.gameObject.SetActive(GameLogic.OutOfRoom);
                if (rtButtonPlay != null) rtButtonPlay.gameObject.SetActive(true);
            }
            catch (Exception) { }
        }

        private async UniTask GainRewards()
        {
            await UniTask.Delay(200);
            RewardGainController.OnStartGaining?.Invoke();
        }

        bool hasIntroPopup = false;
        private async UniTask AutoShowIntroPopups()
        {
            if (/*GameLogic.ShowBuildingIntro && */GameLogic.UnlockHome && !GameLogic.IntroToBuilding)
            {
                await UniTask.Delay(750);
                hasIntroPopup = true;
                ServiceLocator.PopupScene.ShowPopup(PopupName.BuildingTutorial);
            }
            // else if (GameLogic.IntroToBuilding && GameLogic.UnlockDailyRewards && !PlayerDataManager.PlayerData.IntroToDailyReward)
            // {
            //     await UniTask.Delay(750);
            //     hasIntroPopup = true;
            //     ServiceLocator.PopupScene.ShowPopup(PopupName.DailyRewards, new PopupDailyRewardsArgs(false));
            // }
            else if (GameLogic.CurrentLevel >= GameConfig.ConfigData.LevelUnlockLuckySpin && !PlayerDataManager.PlayerData.IntroToLuckySpin)
            {
                await UniTask.Delay(750);
                hasIntroPopup = true;
                ServiceLocator.PopupScene.ShowPopup(PopupName.LuckyWheel);
            }
            // else if (PlayerDataManager.PlayerData.PreIntroToLuxuryBasket && !PlayerDataManager.PlayerData.IntroToLuxuryBasket)
            // {
            //     await UniTask.Delay(750);
            //     hasIntroPopup = true;
            //     ServiceLocator.PopupScene.ShowPopup(PopupName.LuxuryBasket);
            // }
            else if (GameLogic.CurrentLevel >= GameLogic.LevelCallToRate && GameLogic.CurrentLevel % 10 == 0 && !PlayerDataManager.PlayerData.GameRated)
            {
                await UniTask.Delay(750);
                hasIntroPopup = true;
                ServiceLocator.PopupScene.ShowPopup(PopupName.CallToRate);
            }
        }

        bool hasSalePopups = false;
        private async UniTask AutoShowSalePopups()
        {
            if (GlobalSetting.IsJustPlay && GameLogic.CurrentLive <= 0)
            {
                if (hasIntroPopup)
                {
                    await UniTask.WaitUntil(() => GameLogic.AutoIntroPopupClosed);
                }
                else
                {
                    await UniTask.Delay(750);
                }
                if (GameLogic.IsShowingRoom) return;
                hasSalePopups = true;
                ServiceLocator.PopupScene.ShowPopup(PopupName.RefillLives);
            }
            // else if (GameLogic.AutoShowSalePopup && PlayerDataManager.PlayerData.IntroToDailyReward && !GlobalSetting.IsJustPlay && PlayerDataManager.PlayerData.DailyRewardIndex <= 0) // free collect
            // {
            //     if (hasIntroPopup)
            //     {
            //         await UniTask.WaitUntil(() => GameLogic.AutoIntroPopupClosed);
            //     }
            //     else
            //     {
            //         await UniTask.Delay(750);
            //     }
            //     if (GameLogic.IsShowingRoom) return;
            //     hasSalePopups = true;
            //     ServiceLocator.PopupScene.ShowPopup(PopupName.DailyRewards, new PopupDailyRewardsArgs(true));
            // }
            // else if (GameLogic.AutoShowSalePopup && GlobalSetting.IsJustPlay && !PlayerDataManager.PlayerData.IntroToHiddenPictureTutorial && GameLogic.CurrentLevel >= GameLogic.LevelUnlockHiddenPicture)
            // {
            //     if (hasIntroPopup)
            //     {
            //         await UniTask.WaitUntil(() => GameLogic.AutoIntroPopupClosed);
            //     }
            //     else
            //     {
            //         await UniTask.Delay(750);
            //     }
            //     if (GameLogic.IsShowingRoom) return;
            //     hasSalePopups = true;
            //     TutorialManager.OnOpenTutorialByKey?.Invoke("hidden-picture", () =>
            //     {
            //         if (!PlayerDataManager.PlayerData.IntroToHiddenPictureTutorial)
            //         {
            //             PlayerDataManager.PlayerData.IntroToHiddenPictureTutorial = true;
            //         }
            //     });
            // }
            // else if (GameLogic.AutoShowSalePopup && GlobalSetting.IsJustPlay && PlayerDataManager.PlayerData.IntroToLuxuryBasket)
            // {
            //     List<int> targets = DataManager.Instance.LuxuryBasketTargets;
            //     int targetValue = targets[Mathf.Clamp(PlayerDataManager.PlayerData.LuxuryBasketOpenedCount, 0, targets.Count - 1)];
            //     if (GameLogic.TotalPin >= targetValue)
            //     {
            //         if (hasIntroPopup)
            //         {
            //             await UniTask.WaitUntil(() => GameLogic.AutoIntroPopupClosed);
            //         }
            //         else
            //         {
            //             await UniTask.Delay(750);
            //         }
            //         if (GameLogic.IsShowingRoom) return;
            //         hasSalePopups = true;
            //         ServiceLocator.PopupScene.ShowPopup(PopupName.LuxuryBasket);
            //     }
            // }
            else if (GameLogic.AutoShowSalePopup && !GlobalSetting.IsJustPlay && GameLogic.IsFullPiggyBank)
            {
                if (hasIntroPopup)
                {
                    await UniTask.WaitUntil(() => GameLogic.AutoIntroPopupClosed);
                }
                else
                {
                    await UniTask.Delay(750);
                }
                if (GameLogic.IsShowingRoom) return;
                hasSalePopups = true;
                ServiceLocator.PopupScene.ShowPopup(PopupName.PiggyBank);
            }
            else if (GameLogic.AutoShowSalePopup && !GlobalSetting.IsJustPlay && !GameLogic.IsNoAds)
            {
                if (hasIntroPopup)
                {
                    await UniTask.WaitUntil(() => GameLogic.AutoIntroPopupClosed);
                }
                else
                {
                    await UniTask.Delay(750);
                }
                if (GameLogic.IsShowingRoom) return;
                hasSalePopups = true;
                ServiceLocator.PopupScene.ShowPopup(PopupName.RemoveAds);
            }
        }

        // bool hasEventPopups = false;
        // private async UniTask AutoShowEventPopups()
        // {
        //     if (GameLogic.UnlockStarRush && StarRushManager.Data.IsCompleted())
        //     {
        //         UICurrencyManager.OnShowBlockMask?.Invoke(false);
        //         if (hasSalePopups)
        //         {
        //             await UniTask.WaitUntil(() => GameLogic.AutoSalePopupClosed);
        //         }
        //         else
        //         {
        //             await UniTask.Delay(750);
        //         }
        //         if (isShowingRoom) return;
        //         hasEventPopups = true;
        //         ServiceLocator.PopupScene.ShowPopup(PopupName.StarRush, new PopupStarRushArgs(false));
        //     }
        // }

        private async UniTask HideMenus()
        {
            try
            {
                m_rectRoomNavigation.gameObject.SetActive(false);
                rtButtonPlay.gameObject.SetActive(false);

                // Left Menus
                int leftMenuIndex = -1;
                int leftMenuInactiveCount = 0;
                foreach (RectTransform menu in leftMenus)
                {
                    if (menu.gameObject.activeSelf)
                    {
                        leftMenuIndex += 1;
                        leftMenuTween = menu.DOAnchorPosX(leftPos[leftMenuIndex].x, 1f).SetDelay(0.1f * leftMenuIndex).SetEase(Ease.InOutBack);
                    }
                    else
                    {
                        leftMenuInactiveCount += 1;
                    }
                }

                // Right Menus
                int rightMenuIndex = -1;
                foreach (RectTransform menu in rightMenus)
                {
                    if (menu.gameObject.activeSelf)
                    {
                        rightMenuIndex += 1;
                        rightMenuTween = menu.DOAnchorPosX(rightPos[rightMenuIndex].x, 1f).SetDelay(0.1f * rightMenuIndex).SetEase(Ease.InOutBack);
                    }
                }

                await UniTask.Delay(200);

                // Bottom Menus
                bottomMenuTween = rtBottomMenus.DOAnchorPosY(bottomPos.x, 1f).SetDelay(0.1f).SetEase(Ease.InOutBack);
            }
            catch (Exception) { }
        }
    }
}
