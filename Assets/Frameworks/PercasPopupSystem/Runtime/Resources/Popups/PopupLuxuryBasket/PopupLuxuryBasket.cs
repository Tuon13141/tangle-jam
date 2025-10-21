using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Percas.UI;
using Percas.Data;

namespace Percas
{
    public class PopupLuxuryBasket : PopupBase
    {
        [Header("UI")]
        [SerializeField] ButtonBase buttonClosePopup;
        [SerializeField] ButtonBase buttonOpenHelp;
        [SerializeField] ButtonBase buttonPreviewRewards;
        [SerializeField] ButtonBase buttonOpen;
        [SerializeField] Slider sliderProgress;
        [SerializeField] RectTransform reviewRewards;
        [SerializeField] TMP_Text textProressValue;
        [SerializeField] GameObject iconBasket;

        private int targetValue = 50;
        private List<int> targets;

        private void SetTargetValue()
        {
            targetValue = targets[Mathf.Clamp(PlayerDataManager.PlayerData.LuxuryBasketOpenedCount, 0, targets.Count - 1)];
        }

        private void OnStart()
        {
            if (!PlayerDataManager.PlayerData.IntroToLuxuryBasket)
            {
                PlayerDataManager.PlayerData.IntroToLuxuryBasket = true;
                PlayerDataManager.OnSave?.Invoke();
            }
            iconBasket.SetActive(GameLogic.TotalPin < targetValue);
            buttonOpen.gameObject.SetActive(GameLogic.TotalPin >= targetValue);
            buttonOpenHelp.gameObject.SetActive(PlayerDataManager.PlayerData.IntroToLuxuryBasketTutorial);
            textProressValue.text = $"{GameLogic.TotalPin}/{targetValue}";
            sliderProgress.value = (float)GameLogic.TotalPin / (float)targetValue;
            buttonClosePopup.SetPointerClickEvent(Close);
            buttonOpenHelp.SetPointerClickEvent(OpenHelp);
            buttonOpen.SetPointerClickEvent(OpenBasket);
            buttonPreviewRewards.SetPointerClickEvent(PreviewRewards);
            reviewRewards.gameObject.SetActive(false);
        }

        private void OnHide(Action callback = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.LuxuryBasket, () =>
            {
                callback?.Invoke();
                GameLogic.AutoIntroPopupClosed = true;
            });
        }

        private void Close()
        {
            OnHide(() =>
            {
                if (!PlayerDataManager.PlayerData.IntroToLuxuryBasketTutorial)
                {
                    ServiceLocator.PopupScene.ShowPopup(PopupName.LuxuryBasketTutorial);
                }
                else
                {
                    GameLogic.AutoSalePopupClosed = true;
                }
            });
        }

        private void OpenHelp()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.LuxuryBasketTutorial);
        }

        private void PreviewRewards()
        {
            reviewRewards.gameObject.SetActive(!reviewRewards.gameObject.activeSelf);
        }

        private void OpenBasket()
        {
            if (GameLogic.TotalPin < targetValue)
            {
                ActionEvent.OnShowToast?.Invoke($"Not Enough Pins");
                return;
            }
            OnHide(() =>
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.LuxuryBasketOpen, new PopupLuxuryBasketOpenArgs(targetValue));
            });
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            targets = DataManager.Instance.LuxuryBasketTargets;
            SetTargetValue();
            OnStart();
        }
        #endregion
    }
}
