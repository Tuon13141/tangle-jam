using System;
using System.Collections.Generic;
using Percas.Data;
using Percas.IAA;
using Percas.IAR;
using Percas.Live;
using Percas.UI;
using TMPro;
using UnityEngine;

namespace Percas
{
    public class PopupCheat : PopupBase
    {
        [SerializeField] TMP_Text textBoosterUndo;
        [SerializeField] TMP_Text textBoosterAddSlots;
        [SerializeField] TMP_Text textBoosterShuffle;
        [SerializeField] TMP_Text textBoosterClear;
        [SerializeField] TMP_InputField inputLevel, inputLevelData;
        [SerializeField] TMP_InputField inputAddCoin;
        [SerializeField] TMP_InputField inputAddCoil;

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                PlayerDataManager.PlayerData.HighestLevel += 1;
                base.Hide(() =>
                {
                    GlobalSetting.OnHomeToGame?.Invoke(null);
                });
            }
        }
#endif

        private void OnUpdateBoosterButtons()
        {
            textBoosterUndo.text = $"{BoosterManager.OnGetBoosterAmount?.Invoke(BoosterType.Undo)} Undo";
            textBoosterAddSlots.text = $"{BoosterManager.OnGetBoosterAmount?.Invoke(BoosterType.AddSlots)} Add Slots";
            textBoosterShuffle.text = $"{BoosterManager.OnGetBoosterAmount?.Invoke(BoosterType.Shuffle)} Shuffle";
            textBoosterClear.text = $"{BoosterManager.OnGetBoosterAmount?.Invoke(BoosterType.Clear)} Clear";
        }

        public override void Show(object args = null, Action callback = null)
        {
            OnUpdateBoosterButtons();
            inputLevel.text = $"0";
            inputAddCoin.text = $"0";
            inputAddCoil.text = $"0";
            base.Show(callback);
        }

        public void Close()
        {
            base.Hide();
        }

        public void EarnCoil()
        {
            base.Hide(() =>
            {
                RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoil(100, Vector3.zero, null));
                RewardGainController.OnStartGaining?.Invoke();
            });
        }

        public void EarnPin()
        {
            base.Hide(() =>
            {
                RewardGainController.OnAddRewardGain?.Invoke(new RewardGainPin(100, Vector3.zero, null));
                RewardGainController.OnStartGaining?.Invoke();
            });
        }

        public void EarnCoin()
        {
            List<Reward> rewards = new()
        {
            new Reward(RewardType.Coin, 100, null),
        };
            RewardManager.OnSetRewards?.Invoke(rewards);
            RewardManager.OnGetRewards?.Invoke((rewards) =>
            {
                foreach (Reward reward in rewards)
                {
                    Debug.LogError($"Earn {reward.RewardAmount} {reward.RewardType}s!");
                }
            });
        }

        public void SpendCoin()
        {
            PlayerDataManager.OnSpendCoin?.Invoke(200, (value) =>
            {
                if (value)
                {
                    Debug.LogError($"Spend 200 Coins!");
                }
                else
                {
                    ActionEvent.OnShowToast?.Invoke($"Not Enough Coins!");
                }
            }, "cheat", null);
        }

        public void ShowInter()
        {
            IAAManager.ShowInterstitialAd(() =>
            {
                ActionEvent.OnShowToast?.Invoke($"Interstitial Closed!");
            }, null);
        }

        public void ShowVideo()
        {
            IAAManager.ShowRewardedAd(() =>
            {
                ActionEvent.OnShowToast?.Invoke($"Rewarded!");
            }, () =>
            {
                ActionEvent.OnShowToast?.Invoke($"Video Closed!");
            }, null);
        }

        public void AddLive()
        {
            // add 1 live
            LiveManager.OnRefillLives?.Invoke(1, null);
        }

        public void AddInfiniteLive()
        {
            // add infinite in 1 minute
            LiveManager.OnEarnInfiniteLives?.Invoke(1, null);
        }

        public void AddAllBoosters()
        {
            BoosterManager.OnEarnBooster?.Invoke(BoosterType.Undo, 2, null);
            BoosterManager.OnEarnBooster?.Invoke(BoosterType.AddSlots, 2, null);
            BoosterManager.OnEarnBooster?.Invoke(BoosterType.Shuffle, 2, null);
            BoosterManager.OnEarnBooster?.Invoke(BoosterType.Clear, 2, null);
            OnUpdateBoosterButtons();
        }

        public void UseBooster(int boosterIndex)
        {
            BoosterType boosterType = (BoosterType)boosterIndex;
            BoosterManager.OnUseBooster?.Invoke(boosterType, () =>
            {
                ActionEvent.OnShowToast?.Invoke($"Use {boosterType}!");
                OnUpdateBoosterButtons();
            }, () =>
            {
                ActionEvent.OnShowToast?.Invoke($"Oops! Lack of {boosterType}!");
            }, null);
        }

        public void ToggleUI()
        {
            GameConfig.Instance.IsMarketingVersion = !GameConfig.Instance.IsMarketingVersion;
            UIMarketingHandler.OnShow?.Invoke();
            if (GameConfig.Instance.IsMarketingVersion)
            {
                Kernel.Resolve<AdsManager>().HideBanner();
            }
            else
            {
                Kernel.Resolve<AdsManager>().ShowBanner();
            }
        }

        public void ToggleDebugConsole()
        {
            GlobalSetting.ToggleDebugConsole();
        }

        public void ResetDailyRewards()
        {
            PlayerDataManager.PlayerData.DailyRewardIndex = 0;
            PlayerDataManager.OnSave?.Invoke();
            ButtonDailyRewards.OnUpdateNoti?.Invoke();
        }

        public void Play()
        {
            int levelToPlay = int.Parse(inputLevel.text);
            PlayerDataManager.PlayerData.HighestLevel = levelToPlay - 1;
            base.Hide(() =>
            {
                GlobalSetting.OnHomeToGame?.Invoke(null);
            });
        }

        public void PlayWithData()
        {
            GameLogic.CheatLevelData = inputLevelData.text;
            base.Hide(() =>
            {
                GlobalSetting.OnHomeToGame?.Invoke(null);
            });
        }

        public void AddCoin()
        {
            int addCoin = int.Parse(inputAddCoin.text);
            base.Hide(() =>
            {
                if (addCoin > 0)
                {
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin(addCoin, Vector3.zero, null));
                    RewardGainController.OnStartGaining?.Invoke();
                }
            });
        }

        public void AddCoil()
        {
            int addCoil = int.Parse(inputAddCoil.text);
            base.Hide(() =>
            {
                if (addCoil > 0)
                {
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoil(addCoil, Vector3.zero, null));
                    RewardGainController.OnStartGaining?.Invoke();
                }
            });
        }
    }
}
