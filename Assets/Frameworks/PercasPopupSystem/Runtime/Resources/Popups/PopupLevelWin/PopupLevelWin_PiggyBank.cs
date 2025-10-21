using System;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using TMPro;
using Spine;

namespace Percas
{
    public class PopupLevelWin_PiggyBank : MonoBehaviour
    {
        [SerializeField] SkeletonGraphic skePiggyBank;
        [SerializeField] Slider slider;
        [SerializeField] TMP_Text textValue;

        private bool IsPopupWin;
        private Action OnCallback;

        private void Awake()
        {
            skePiggyBank.AnimationState.Event += HandleSpineEvent;
        }

        private void OnDestroy()
        {
            if (skePiggyBank != null)
            {
                skePiggyBank.AnimationState.Event -= HandleSpineEvent;
            }
        }

        private void UpdateValue(int value = -1)
        {
            if (GameLogic.IsInHome)
            {
                textValue.text = value == -1 ? $"{GameLogic.CurrentCoinInPiggyBank}/{GameLogic.PiggyBankMaxCoin}" : $"{value}/{GameLogic.PiggyBankMaxCoin}";
            }
            else
            {
                textValue.text = value == -1 ? $"{Math.Max(GameLogic.CurrentCoinInPiggyBank - GameLogic.PiggyBankWinLevelEarn, 0)}/{GameLogic.PiggyBankMaxCoin}" : $"{value}/{GameLogic.PiggyBankMaxCoin}";
            }
        }

        private void AnimPiggyBank()
        {
            if (skePiggyBank != null)
            {
                if (!GameLogic.IsFullPiggyBank)
                {
                    skePiggyBank.AnimationState.SetAnimation(0, "idle_win", false).Complete += (entry) =>
                    {
                        skePiggyBank.AnimationState.SetAnimation(0, "nhan_thuong", false).Complete += (trackEntry) =>
                        {
                            if (IsPopupWin)
                            {
                                OnCallback?.Invoke();
                            }
                            else
                            {
                                skePiggyBank.AnimationState.SetAnimation(0, "idle_win", true);
                            }
                        };
                    };
                }
                else
                {
                    skePiggyBank.AnimationState.SetAnimation(0, "idle_full", false).Complete += (entry) =>
                    {
                        skePiggyBank.AnimationState.SetAnimation(0, "idle_full_nhan_thuong", false).Complete += (trackEntry) =>
                        {
                            if (IsPopupWin)
                            {
                                OnCallback?.Invoke();
                            }
                            else
                            {
                                skePiggyBank.AnimationState.SetAnimation(0, "idle_full", true);
                            }
                        };
                    };
                }
            }
        }

        private void HandleSpineEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == "coin_drop")
            {
                AudioController.Instance.PlaySpawnCoins();
                if (GameLogic.IsInGame)
                {
                    Helpers.ChangeValueInt(Math.Max(GameLogic.CurrentCoinInPiggyBank - GameLogic.PiggyBankWinLevelEarn, 0), GameLogic.CurrentCoinInPiggyBank, 0.5f, 0.0f, (value) =>
                    {
                        UpdateValue(value);
                    });
                }
            }
        }

        #region Public Methods
        public void UpdateUI(bool isPopupWin, Action callback)
        {
            IsPopupWin = isPopupWin;
            OnCallback = callback;
            AnimPiggyBank();
            UpdateValue();
            slider.value = (float)GameLogic.CurrentCoinInPiggyBank / GameLogic.PiggyBankMaxCoin;
        }
        #endregion
    }
}
