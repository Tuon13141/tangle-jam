using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Percas.UI;
using Percas.Data;

namespace Percas
{
    public class PopupTutorialArgs
    {
        public TutorialDataSO tutorialData;
        public Action callback;
        public bool canPrev;
        public bool canNext;
        public bool isPreOpenTutorial;
        public bool showButtons;

        public PopupTutorialArgs(TutorialDataSO tutorialData, Action callback, bool canPrev, bool canNext, bool isPreOpenTutorial, bool showButtons = false)
        {
            this.tutorialData = tutorialData;
            this.callback = callback;
            this.canPrev = canPrev;
            this.canNext = canNext;
            this.isPreOpenTutorial = isPreOpenTutorial;
            this.showButtons = showButtons;
        }
    }

    public class PopupTutorial : PopupBase
    {
        [SerializeField] TMP_Text textTitle;
        [SerializeField] ButtonClosePopup buttonClosePopup;
        [SerializeField] ButtonTutorial buttonGotIt, buttonClaimBooster;
        [SerializeField] ButtonShowInter buttonContinuePreTutorial;
        [SerializeField] ButtonBase nextTutorial, previousTutorial;
        [SerializeField] PopupTutorial_Content tutorialContent;
        [SerializeField] Image iconInButton;

        [Header("Buttons")]
        [SerializeField] List<Sprite> buttonBGs = new();
        [SerializeField] Image prevButtonBG;
        [SerializeField] Image nextButtonBG;

        private TutorialDataSO tutorialData;

        private bool isShowButtons;
        private bool isPreOpenTutorial;

        private Action OnCallback;

        private void OnHide(Action onCompleted = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.Tutorial, () =>
            {
                TrackingManager.OnTutorialComplete?.Invoke(tutorialData.name, tutorialData.id);
                onCompleted?.Invoke();
            });
        }

        private void ShowTutorialButtons(bool canPrev, bool canNext)
        {
            if (canPrev) prevButtonBG.sprite = buttonBGs[1];
            else prevButtonBG.sprite = buttonBGs[0];

            if (canNext) nextButtonBG.sprite = buttonBGs[1];
            else nextButtonBG.sprite = buttonBGs[0];
        }

        private void ReloadData(TutorialDataSO data, bool canPrev, bool canNext)
        {
            if (data == null) return;
            textTitle.text = !string.IsNullOrEmpty(data.title) ? data.title : $"How To Play?";
            ShowTutorialButtons(canPrev, canNext);
            tutorialData = data;
            tutorialContent.SetContent(tutorialData);
        }

        private void OnStart(TutorialDataSO data)
        {
            tutorialData = data;
            nextTutorial.gameObject.SetActive(isShowButtons);
            previousTutorial.gameObject.SetActive(isShowButtons);
            buttonGotIt.gameObject.SetActive(!isShowButtons && !tutorialData.isBooster && !isPreOpenTutorial);
            buttonContinuePreTutorial.gameObject.SetActive(isPreOpenTutorial);
            buttonClaimBooster.gameObject.SetActive(!isShowButtons && tutorialData.isBooster);
            buttonClosePopup.gameObject.SetActive(!(!isShowButtons && tutorialData.isBooster) && !isPreOpenTutorial);
            if (tutorialData.isBooster)
            {
                iconInButton.sprite = tutorialData.iconInButton;
            }
            if (isShowButtons)
            {
                nextTutorial.SetPointerClickEvent(NextTutorial);
                previousTutorial.SetPointerClickEvent(PreviousTutorial);
            }
            buttonClosePopup.onCompleted = Close;
            buttonGotIt.onCompleted = GotIt;
            buttonContinuePreTutorial.onCompleted = PreTutorialContinue;
            buttonClaimBooster.onCompleted = ClaimBooster;
            tutorialContent.SetContent(tutorialData);
        }

        private void NextTutorial()
        {
            TutorialManager.OnNext?.Invoke((data, canPrev, canNext) =>
            {
                ReloadData(data, canPrev, canNext);
            });
        }

        private void PreviousTutorial()
        {
            TutorialManager.OnPrevious?.Invoke((data, canPrev, canNext) =>
            {
                ReloadData(data, canPrev, canNext);
            });
        }

        private void SetUserType(string value)
        {
            if (GameLogic.CurrentLevel >= 2) return;
            TrackingManager.OnSetUserType?.Invoke(value);
        }

        private void Close()
        {
            SetUserType(Const.USER_TYPE_EXP);
            OnHide();
        }

        private void GotIt()
        {
            SetUserType(Const.USER_TYPE_NEW);
            OnHide();
        }

        private void PreTutorialContinue()
        {
            OnHide(() =>
            {
                OnCallback?.Invoke();
            });
        }

        private void ClaimBooster()
        {
            PlayerDataManager.AddTutorialShown($"{tutorialData.level}_{tutorialData.wave}");
            OnHide(() =>
            {
                if (tutorialData.isBooster)
                {
                    if (tutorialData.key == "booster_undo")
                    {
                        ButtonUseBooster.OnFocus?.Invoke(BoosterType.Undo);
                    }

                    if (tutorialData.key == "booster_add_slots")
                    {
                        ButtonUseBooster.OnFocus?.Invoke(BoosterType.AddSlots);
                    }

                    if (tutorialData.key == "booster_clear")
                    {
                        ButtonUseBooster.OnFocus?.Invoke(BoosterType.Clear);
                    }
                }
            });
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            if (args is PopupTutorialArgs popupArgs)
            {
                tutorialData = popupArgs.tutorialData;
                OnCallback = popupArgs.callback;
                isPreOpenTutorial = popupArgs.isPreOpenTutorial;
                isShowButtons = popupArgs.showButtons;
                ShowTutorialButtons(popupArgs.canPrev, popupArgs.canNext);
            }
            TrackingManager.OnTutorialBegin?.Invoke(tutorialData.name, tutorialData.id);
            textTitle.text = !string.IsNullOrEmpty(tutorialData.title) ? tutorialData.title : $"How To Play?";
            OnStart(tutorialData);
        }
        #endregion
    }
}
