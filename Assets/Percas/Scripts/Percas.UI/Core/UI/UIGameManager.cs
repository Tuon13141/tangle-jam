using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Percas.Data;

namespace Percas.UI
{
    public class UIGameManager : MonoBehaviour
    {
        [SerializeField] List<Sprite> settingUIs;
        [SerializeField] List<Sprite> replayUIs;
        [SerializeField] Image spriteButtonSetting, spriteButtonReplay;
        [SerializeField] RectTransform m_safeArea;
        [SerializeField] RectTransform rectButtonSetting;
        [SerializeField] RectTransform rectButtonReplay;
        [SerializeField] RectTransform rectBalanceCoin;
        [SerializeField] RectTransform rectLevelLabel;
        //[SerializeField] RectTransform rectIconHardLevel;
        [SerializeField] GameObject boosterController;
        [SerializeField] List<RectTransform> rectBoosters;

        public static Action<bool> OnUpdateButtonUI;
        public static Action OnUpdateIconHardLevelPos;
        public static Action<bool> OnShowIconHardLevel;
        public static Action<bool, Action> OnDisplayBoosters;
        public static bool CanShowTutorial { get; set; }
        public static bool CompleteBoosterShown { get; set; }

        //private readonly float labelShortPosY = 0f; // 76f

        private Tween buttonSettingsTween;
        private Tween buttonReplayTween;
        private Tween balanceCoinTween;
        private Tween levelLabelTween;
        //private Tween iconHardTween;
        private Tween boosterTween;

        public static RectTransform SafeArea;

        private void Awake()
        {
            SafeArea = m_safeArea;
            OnUpdateButtonUI += UpdateButtonUI;
            ActionEvent.OnSetLevelPhase += Display;
            OnDisplayBoosters += DisplayBoosters;
            //OnUpdateIconHardLevelPos += UpdateIconHardLevelPos;
            //OnShowIconHardLevel += ShowIconHardLevel;
        }

        private void OnDestroy()
        {
            OnUpdateButtonUI -= UpdateButtonUI;
            ActionEvent.OnSetLevelPhase -= Display;
            OnDisplayBoosters -= DisplayBoosters;
            //OnUpdateIconHardLevelPos -= UpdateIconHardLevelPos;
            //OnShowIconHardLevel -= ShowIconHardLevel;

            buttonSettingsTween?.Kill();
            buttonReplayTween?.Kill();
            balanceCoinTween?.Kill();
            levelLabelTween?.Kill();
            //iconHardTween?.Kill();
            boosterTween?.Kill();
        }

        //private void OnEnable()
        //{
        //    UICurrencyManager.OnShowBalance?.Invoke(true, false, false);
        //}

        private void UpdateButtonUI(bool isHardLevel)
        {
            if (isHardLevel)
            {
                spriteButtonSetting.sprite = settingUIs[1];
                spriteButtonReplay.sprite = replayUIs[1];
            }
            else
            {
                spriteButtonSetting.sprite = settingUIs[0];
                spriteButtonReplay.sprite = replayUIs[0];
            }
        }

        private void DisplayBoosters(bool value, Action callback)
        {
            boosterController.SetActive(value);
            callback?.Invoke();
        }

        //private void UpdateIconHardLevelPos()
        //{
        //    rectIconHardLevel.anchoredPosition = new Vector2(439f, -136f);
        //}

        //private void ShowIconHardLevel(bool value)
        //{
        //    if (value)
        //    {
        //        iconHardTween = rectIconHardLevel.DOAnchorPosX(-61f, 0.5f).SetDelay(0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        //        {
        //            rectIconHardLevel.DOScale(1.25f, 0.3f).SetLoops(10, LoopType.Yoyo).SetEase(Ease.InOutSine);
        //        });
        //    }
        //    else
        //    {
        //        iconHardTween = rectIconHardLevel.DOAnchorPosX(439f, 0.5f).SetEase(Ease.InBack);
        //    }
        //}

        private void Display(int phase, LevelAsset levelData)
        {
            UICurrencyManager.OnShowBlockMask?.Invoke(true);
            DisplayAsync(phase, levelData);
        }

        private async void DisplayAsync(int phase, LevelAsset levelData)
        {
            try
            {
                if (phase == 1)
                {
                    await ShowMenus();
                    await UniTask.Delay(GameLogic.CurrentLevel < GameLogic.LevelUnlockHome ? 0 : 1200);
                    await ShowPopups(phase, levelData);
                }
                else if (phase == 2)
                {
                    await HideMenus();
                    await ShowPopups(phase, null);
                }
                else
                {
                    await ShowPopups(phase, null);
                }
            }
            catch (Exception) { }
            finally
            {
                UICurrencyManager.OnShowBlockMask?.Invoke(false);
            }
        }

        private async UniTask HideMenus()
        {
            try
            {
                await UniTask.Delay(200);
                // Level Label
                levelLabelTween = rectLevelLabel.DOAnchorPosY(500, 0.5f).SetDelay(0.3f).SetEase(Ease.InBack);
            }
            catch (Exception) { }
        }

        private async UniTask ShowMenus()
        {
            try
            {
                // Config
                float delayTime;
                if (GlobalSetting.IsFirstOpen)
                {
                    delayTime = 0.2f;
                    GlobalSetting.IsFirstOpen = false;
                }
                else
                {
                    delayTime = 1.5f;
                }

                // Button Setting
                Vector2 initialButtonSettingPos = rectButtonSetting.anchoredPosition;
                rectButtonSetting.anchoredPosition = new Vector2(initialButtonSettingPos.x - 360, initialButtonSettingPos.y);
                rectButtonSetting.gameObject.SetActive(true);
                buttonSettingsTween = rectButtonSetting.DOAnchorPosX(initialButtonSettingPos.x, 0.5f).SetDelay(delayTime).SetEase(Ease.OutBack);

                // Button Replay
                if (GameLogic.IsClassicMode)
                {
                    rectButtonReplay.gameObject.SetActive(true);
                    Vector2 initialButtonReplayPos = rectButtonReplay.anchoredPosition;
                    rectButtonReplay.anchoredPosition = new Vector2(initialButtonReplayPos.x - 360, initialButtonReplayPos.y);
                    rectButtonReplay.gameObject.SetActive(GameLogic.CurrentLevel >= 2);
                    if (rectButtonReplay.gameObject.activeSelf) buttonReplayTween = rectButtonReplay.DOAnchorPosX(initialButtonReplayPos.x, 0.5f).SetDelay(delayTime + 0.1f).SetEase(Ease.OutBack);
                }
                else if (GameLogic.IsHiddenPictureMode)
                {
                    rectButtonReplay.gameObject.SetActive(false);
                }

                // Balance Coin
                rectBalanceCoin.anchoredPosition = new Vector2(380, 0);
                rectBalanceCoin.gameObject.SetActive(GameLogic.CurrentLevel >= 1);

                await UniTask.Delay(100);

                // Level Label
                rectLevelLabel.anchoredPosition = new Vector2(0, 500);
                rectLevelLabel.gameObject.SetActive(true);
                levelLabelTween = rectLevelLabel.DOAnchorPosY(0, 0.5f).SetDelay(GameLogic.CurrentLevel < GameLogic.LevelUnlockHome ? 0.5f : 1.5f).SetEase(Ease.OutBack);

                // Balance Coin
                if (rectBalanceCoin.gameObject.activeSelf) balanceCoinTween = rectBalanceCoin.DOAnchorPosX(-120, 0.5f).SetDelay(GameLogic.CurrentLevel < GameLogic.LevelUnlockHome ? 0.6f : 1.6f).SetEase(Ease.OutBack);
            }
            catch (Exception) { }
        }

        private async UniTask ShowPopups(int phase, LevelAsset levelData)
        {
            if (levelData != null)
            {
                if (levelData.IsHard)
                {
                    CanShowTutorial = false;
                    ServiceLocator.PopupScene.ShowPopup(PopupName.HardLevelWarning);
                    await UniTask.WaitUntil(() => CanShowTutorial);
                }
            }

            await UniTask.Delay(100);

            ActionEvent.OnAutoShowInGame?.Invoke(phase, ShowBoosters);

            if (levelData.IsHard)
            {
                float timeOut = 5f;
                await UniTask.WaitUntil(() => CompleteBoosterShown || (timeOut -= Time.deltaTime) <= 0);
                if (GameLogic.CurrentLevel >= GameLogic.LevelUnlockClear)
                {
                    int rand = UnityEngine.Random.Range(0, 2);
                    if (rand == 0) ButtonUseBooster.OnFocus?.Invoke(BoosterType.AddSlots);
                    else ButtonUseBooster.OnFocus?.Invoke(BoosterType.Clear);
                }
                else if (GameLogic.CurrentLevel >= GameLogic.LevelUnlockAddSlots)
                {
                    ButtonUseBooster.OnFocus?.Invoke(BoosterType.AddSlots);
                }
            }
        }

        private void ShowBoosters()
        {
            // Boosters
            for (int i = 0; i <= rectBoosters.Count - 1; i++)
            {
                RectTransform rect = rectBoosters[i];
                Vector2 initPos = rect.anchoredPosition;
                float delayTime = 0f + 0.1f * i;
                rect.anchoredPosition = new Vector2(initPos.x, initPos.y - 600);
                boosterTween = rect.DOAnchorPosY(initPos.y, 0.5f).SetDelay(delayTime).SetEase(Ease.OutBack);
                if (i == rectBoosters.Count - 1)
                {
                    boosterTween.OnComplete(() =>
                    {
                        CompleteBoosterShown = true;
                    });
                }
            }
        }
    }
}
