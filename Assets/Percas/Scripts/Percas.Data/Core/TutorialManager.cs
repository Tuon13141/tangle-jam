using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Percas.UI;

namespace Percas.Data
{
    public enum TutorialShowType
    {
        Popup,
        Message,
    }

    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] int maxTutorialIndex = 7;
        [SerializeField] List<TutorialDataSO> tutorialData = new();

        public static Action OnOpenHowToPlay;
        public static Action<int> OnOpenTutorialByIndex;
        public static Action<string, Action> OnOpenTutorialByKey;
        public static Action<TutorialDataSO, Action> OnPreOpenTutorial;
        public static Action<Action<TutorialDataSO, bool, bool>> OnNext;
        public static Action<Action<TutorialDataSO, bool, bool>> OnPrevious;
        public static Action OnClaim;

        public static TutorialDataSO TutorialInNextLevel { get; private set; }
        public static TutorialDataSO TutorialLuxuryBasket { get; private set; }

        private TutorialDataSO tutorial;

        private int currentTutorialIndex = 0;

        private void Awake()
        {
            ActionEvent.OnAutoShowInGame += HandleTutorialDisplay;
            ActionEvent.OnNoCoinOnBoard += OpenTutorialNoCoilOnBoard;
            ActionEvent.OnReleasePin += UpdateTutorialLuxuryBasket;
            OnOpenHowToPlay += OpenHowToPlay;
            OnOpenTutorialByIndex += OpenTutorialByIndex;
            OnOpenTutorialByKey += OpenTutorialByKey;
            OnPreOpenTutorial += PreOpenTutorial;
            OnNext += NextTutorial;
            OnPrevious += PrevTutorial;
            OnClaim += Claim;
        }

        private void OnDestroy()
        {
            ActionEvent.OnAutoShowInGame -= HandleTutorialDisplay;
            ActionEvent.OnNoCoinOnBoard -= OpenTutorialNoCoilOnBoard;
            ActionEvent.OnReleasePin -= UpdateTutorialLuxuryBasket;
            OnOpenHowToPlay -= OpenHowToPlay;
            OnOpenTutorialByIndex -= OpenTutorialByIndex;
            OnOpenTutorialByKey -= OpenTutorialByKey;
            OnPreOpenTutorial -= PreOpenTutorial;
            OnNext -= NextTutorial;
            OnPrevious -= PrevTutorial;
            OnClaim -= Claim;

            claimTween?.Kill();
        }

        private void UpdateBoosterTutorialData()
        {
            foreach (TutorialDataSO tutorial in tutorialData)
            {
                // Boosters
                if (tutorial.key == "booster_undo")
                {
                    tutorial.level = GameLogic.LevelUnlockUndo;
                }

                if (tutorial.key == "booster_add_slots")
                {
                    tutorial.level = GameLogic.LevelUnlockAddSlots;
                }

                if (tutorial.key == "booster_clear")
                {
                    tutorial.level = GameLogic.LevelUnlockClear;
                }

                // Features
                if (tutorial.key == "feature_lucky_spin" && GameLogic.RemoteLevelConfig.TryGetValue("level_unlock_lucky_spin", out int resultLuckySpin))
                {
                    tutorial.level = resultLuckySpin;
                    tutorial.wave = 1;
                }

                // Obstacles
                if (tutorial.key == "ele_stack" && GameLogic.RemoteLevelConfig.TryGetValue("level_unlock_stack", out int resultStack))
                {
                    tutorial.level = resultStack;
                    tutorial.wave = 1;
                }

                if (tutorial.key == "ele_hidden_coil" && GameLogic.RemoteLevelConfig.TryGetValue("level_unlock_hidden_coil", out int resultHiddenCoil))
                {
                    tutorial.level = resultHiddenCoil;
                    tutorial.wave = 1;
                }

                if (tutorial.key == "ele_locked_pin" && GameLogic.RemoteLevelConfig.TryGetValue("level_unlock_locked_pin", out int resultLockedPin))
                {
                    tutorial.level = resultLockedPin;
                    tutorial.wave = 1;
                }

                if (tutorial.key == "ele_paired_coils" && GameLogic.RemoteLevelConfig.TryGetValue("level_unlock_paired_coils", out int resultPairedCoils))
                {
                    tutorial.level = resultPairedCoils;
                    tutorial.wave = 1;
                }

                if (tutorial.key == "ele_button_stack" && GameLogic.RemoteLevelConfig.TryGetValue("level_unlock_button_stack", out int resultButtonStack))
                {
                    tutorial.level = resultButtonStack;
                    tutorial.wave = 1;
                }
            }
        }

        private void OpenHowToPlay()
        {
            tutorial = GetTutorial(GameLogic.CurrentLevelPhase, true);
            if (tutorial == null)
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_SOMETHING_WRONG);
                return;
            }
            currentTutorialIndex = tutorialData.IndexOf(tutorial);
            //PopupTutorial.Instance.Show(tutorial, null, currentTutorialIndex > 0, currentTutorialIndex < maxTutorialIndex, false, true);
            ServiceLocator.PopupScene.ShowPopup(PopupName.Tutorial, new PopupTutorialArgs(tutorial, null, currentTutorialIndex > 0, currentTutorialIndex < maxTutorialIndex, false, true));
        }

        private void OpenTutorialByIndex(int index)
        {
            TutorialDataSO tutorial = GetTutorialByIndex(index);
            if (tutorial == null) return;
            currentTutorialIndex = tutorialData.IndexOf(tutorial);
            //PopupTutorial.Instance.Show(tutorial, null, currentTutorialIndex > 0, currentTutorialIndex < tutorialData.Count - 1, false);
            ServiceLocator.PopupScene.ShowPopup(PopupName.Tutorial, new PopupTutorialArgs(tutorial, null, currentTutorialIndex > 0, currentTutorialIndex < tutorialData.Count - 1, false));
        }

        private void OpenTutorialByKey(string key, Action onCallback)
        {
            TutorialDataSO tutorial = GetTutorialByKey(key);
            if (tutorial == null) return;
            currentTutorialIndex = tutorialData.IndexOf(tutorial);
            onCallback?.Invoke();
            //PopupTutorial.Instance.Show(tutorial, null, currentTutorialIndex > 0, currentTutorialIndex < tutorialData.Count - 1, false);
            ServiceLocator.PopupScene.ShowPopup(PopupName.Tutorial, new PopupTutorialArgs(tutorial, null, currentTutorialIndex > 0, currentTutorialIndex < tutorialData.Count - 1, false));
        }

        private void PreOpenTutorial(TutorialDataSO tutorial, Action callback)
        {
            if (tutorial == null) return;
            currentTutorialIndex = tutorialData.IndexOf(tutorial);
            //PopupTutorial.Instance.Show(tutorial, callback, currentTutorialIndex > 0, currentTutorialIndex < tutorialData.Count - 1, true);
            ServiceLocator.PopupScene.ShowPopup(PopupName.Tutorial, new PopupTutorialArgs(tutorial, callback, currentTutorialIndex > 0, currentTutorialIndex < tutorialData.Count - 1, true));
        }

        private void OpenTutorialNoCoilOnBoard()
        {
            OpenTutorialByIndex(maxTutorialIndex);
        }

        private TutorialDataSO GetTutorial(int wave, bool getFirstIfNull)
        {
            TutorialDataSO tut = null;
            try
            {
                tut = tutorialData.Find((item) => item.showInGame && item.level == GameLogic.CurrentLevel && item.wave == wave);
                if (tut == null && getFirstIfNull)
                {
                    tut = tutorialData[0];
                }
                if (tut != null) currentTutorialIndex = tutorialData.IndexOf(tut);
            }
            catch (Exception) { }
            return tut;
        }

        private TutorialDataSO GetTutorialInNextLevel()
        {
            TutorialDataSO tut = null;
            // try
            // {
            //     tut = tutorialData.Find((item) => item.level == GameLogic.CurrentLevel + 1 && item.preNotice);
            // }
            // catch (Exception) { }
            return tut;
        }

        private TutorialDataSO GetTutorialByKey(string keyName)
        {
            TutorialDataSO tut = null;
            try
            {
                tut = tutorialData.Find((item) => !string.IsNullOrEmpty(item.key) && item.key == keyName);
            }
            catch (Exception) { }
            return tut;
        }

        private TutorialDataSO GetTutorialByIndex(int index)
        {
            TutorialDataSO tut = null;
            try
            {
                tut = tutorialData[index];
            }
            catch (Exception) { }
            return tut;
        }

        private void PrevTutorial(Action<TutorialDataSO, bool, bool> onCompleted)
        {
            int newIndex = Math.Clamp(currentTutorialIndex - 1, 0, maxTutorialIndex);
            currentTutorialIndex = newIndex;
            TutorialDataSO newTutorial = GetTutorialByIndex(newIndex);
            onCompleted?.Invoke(newTutorial, currentTutorialIndex > 0, currentTutorialIndex < maxTutorialIndex);
        }

        private void NextTutorial(Action<TutorialDataSO, bool, bool> onCompleted)
        {
            int newIndex = Math.Clamp(currentTutorialIndex + 1, 0, maxTutorialIndex);
            TutorialDataSO newTutorial = GetTutorialByIndex(newIndex);
            if (GameLogic.CurrentLevel < newTutorial.level)
            {
                onCompleted?.Invoke(null, currentTutorialIndex > 0, false);
            }
            else
            {
                currentTutorialIndex = newIndex;
                onCompleted?.Invoke(newTutorial, currentTutorialIndex > 0, currentTutorialIndex < maxTutorialIndex);
            }
        }

        private Tween claimTween;
        private Action onCallback;

        private void HandleTutorialDisplay(int wave, Action callback)
        {
            UpdateBoosterTutorialData();
            tutorial = GetTutorial(wave, false);
            TutorialInNextLevel = GetTutorialInNextLevel();
            onCallback = callback;

            if (tutorial == null)
            {
                HideMessageThenShowBoosters(callback);
                return;
            }
            if (tutorial.showType == TutorialShowType.Popup)
            {
                if (!PlayerDataManager.IsTutorialShown($"{tutorial.level}_{tutorial.wave}"))
                {
                    currentTutorialIndex = tutorialData.IndexOf(tutorial);
                    UICurrencyManager.OnShowMaskWithAutoClose?.Invoke(0.25f, () =>
                    {
                        //PopupTutorial.Instance.Show(tutorial, null, currentTutorialIndex > 0, currentTutorialIndex < tutorialData.Count - 1, false);
                        ServiceLocator.PopupScene.ShowPopup(PopupName.Tutorial, new PopupTutorialArgs(tutorial, null, currentTutorialIndex > 0, currentTutorialIndex < tutorialData.Count - 1, false));
                    });
                }
                HideMessageThenShowBoosters(callback);
            }
            else
            {
                if (wave == tutorial.wave && !PlayerDataManager.IsTutorialShown($"{tutorial.level}_{tutorial.wave}"))
                {
                    TutorialMessage.OnShow?.Invoke(tutorial);
                    UIGameManager.OnDisplayBoosters?.Invoke(false, null);
                    if (tutorial.autoClose)
                    {
                        if (tutorial.isBooster) PlayerDataManager.AddTutorialShown($"{tutorial.level}_{tutorial.wave}");
                        claimTween = DOVirtual.DelayedCall((float)tutorial.autoCloseIn, () =>
                        {
                            HideMessageThenShowBoosters(callback);
                        });
                    }
                }
                else
                {
                    HideMessageThenShowBoosters(callback);
                }
            }
        }

        private void HideMessageThenShowBoosters(Action callback)
        {
            if (claimTween != null && claimTween.IsActive())
            {
                claimTween.Kill();
            }
            TutorialMessage.OnHide?.Invoke();
            UIGameManager.OnDisplayBoosters?.Invoke(true, callback);
        }

        private void UpdateTutorialLuxuryBasket()
        {
            // if (PlayerDataManager.PlayerData.PreIntroToLuxuryBasket) return;
            // if (GameLogic.TotalPin <= 3) return;
            // if (TutorialLuxuryBasket != null) return;
            // TutorialLuxuryBasket = GetTutorialByKey("luxury-basket");
        }

        private void Claim()
        {
            TutorialMessage.OnHide?.Invoke();
            UIGameManager.OnDisplayBoosters?.Invoke(true, onCallback);
            claimTween?.Kill();
        }
    }
}
