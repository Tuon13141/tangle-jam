using System;
using UnityEngine;
using TMPro;
using Percas.UI;
using Percas.Data;

namespace Percas
{
    public class PopupLuckyWheel : PopupBase
    {
        [SerializeField] LuckyWheel luckyWheel;
        [SerializeField] ButtonBase buttonClosePopup;
        [SerializeField] ButtonBase buttonPointer;
        [SerializeField] ButtonBase buttonSpinFree;
        [SerializeField] ButtonWatchVideoAd buttonWatchToSpin;
        [SerializeField] TMP_Text m_textButtonSpin;
        [SerializeField] ButtonBase buttonSkip;
        [SerializeField] GameObject labelSkip;

        protected override void Awake()
        {
            RegisterButtons();
        }

        private void RegisterButtons()
        {
            buttonClosePopup.SetPointerClickEvent(Close);
            buttonPointer.SetPointerClickEvent(Pointer);
            buttonSkip.SetPointerClickEvent(Skip);

            buttonSpinFree.SetPointerClickEvent(OnFreeSpin);

            buttonWatchToSpin.skipVideo = false;
            buttonWatchToSpin.onStart = OnStartAction;
            buttonWatchToSpin.onCompleted = OnWatchCompleted;
        }

        private void UpdateUI()
        {
            m_textButtonSpin.text = $"SPIN {PlayerDataManager.PlayerData.CurrentSpins}/{Const.MAX_DAILY_SPINS}";

            buttonSpinFree.gameObject.SetActive(PlayerDataManager.PlayerData.FreeSpin);
            buttonWatchToSpin.gameObject.SetActive(!PlayerDataManager.PlayerData.FreeSpin);
        }

        private void Close()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.LuckyWheel, () =>
            {
                PlayerDataManager.PlayerData.IntroToLuckySpin = true;
                PlayerDataManager.OnSave?.Invoke();
                GameLogic.AutoIntroPopupClosed = true;
            });
        }

        private void Pointer()
        {
            luckyWheel.ClickOnPointer();
        }

        private void Spin()
        {
            luckyWheel.Spin(OnSpinning, OnSpinStopped, OnSpinCompleted);
        }

        private void OnSpinning()
        {
            ActionEvent.OnPlaySFXLuckySpin?.Invoke();
            labelSkip.SetActive(true);
            buttonSkip.gameObject.SetActive(true);
            buttonClosePopup.gameObject.SetActive(false);
            buttonWatchToSpin.gameObject.SetActive(false);
        }

        private void OnSpinStopped()
        {
            labelSkip.SetActive(false);
            buttonSkip.gameObject.SetActive(false);
            buttonClosePopup.gameObject.SetActive(true);
            buttonWatchToSpin.gameObject.SetActive(true);
        }

        private void OnSpinCompleted()
        {
            Close();
        }

        private void Skip()
        {
            luckyWheel.Stop();
        }

        private void OnStartAction(Action<bool> onCallback)
        {
            if (!PlayerDataManager.PlayerData.CanSpin)
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_OUT_OF_SPINS);
                return;
            }
            onCallback?.Invoke(true);
        }

        private void OnWatchCompleted()
        {
            PlayerDataManager.PlayerData.UpdateDailySpins();
            PlayerDataManager.OnSave?.Invoke();
            UpdateUI();
            Spin();
        }

        private void OnFreeSpin()
        {
            Debug.LogError("OnFreeSpin");
            PlayerDataManager.PlayerData.FreeSpin = false;
            PlayerDataManager.OnSave?.Invoke();

            UpdateUI();
            Spin();
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            UpdateUI();
            luckyWheel.ClickOnPointer();
        }
        #endregion
    }
}
