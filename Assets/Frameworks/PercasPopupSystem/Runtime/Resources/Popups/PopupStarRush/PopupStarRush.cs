using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Percas.UI;
using Percas.Data;
using Percas.IAR;

namespace Percas
{
    public class PopupStarRushArgs
    {
        public bool callToPlay;

        public PopupStarRushArgs(bool callToPlay)
        {
            this.callToPlay = callToPlay;
        }
    }

    public class PopupStarRush : PopupBase
    {
        [SerializeField] ButtonClosePopup buttonClosePopup;
        [SerializeField] ButtonBase buttonOpenHelp;
        [SerializeField] ButtonBase buttonStart;
        [SerializeField] ButtonBase buttonPreviewRewards;
        [SerializeField] ButtonBase buttonContinue;
        [SerializeField] RectTransform m_previewRewards;
        [SerializeField] GameObject m_contentStart, m_contentInProcess, m_contentCompleted;
        [SerializeField] GameObject m_mask;

        [Header("In Process")]
        [SerializeField] TMP_Text m_textTimer;
        [SerializeField] List<StarRushEntry> entries;

        [Header("Completed")]
        [SerializeField] List<GameObject> resultRanks;
        [SerializeField] TMP_Text m_textResultRank;
        [SerializeField] GameObject m_gift, m_nogift;

        private int yourPosition = -1;
        bool callToPlay = false;

        protected override void Awake()
        {
            RegisterButtons();
        }

        private void RegisterButtons()
        {
            buttonClosePopup.onCompleted = Close;
            buttonOpenHelp.SetPointerClickEvent(OpenHelp);
            buttonStart.SetPointerClickEvent(StartEvent);
            buttonPreviewRewards.SetPointerClickEvent(PreviewRewards);
            buttonContinue.SetPointerClickEvent(Continue);
        }

        protected override void OnSubscribeEvents()
        {
            TimeManager.OnTick += HandleStatus;
        }

        protected override void OnUnsubscribeEvents()
        {
            TimeManager.OnTick -= HandleStatus;
        }

        private void InitUI()
        {
            yourPosition = -1;
            ShowTutorial();
            HandleStatus();
        }

        private void ShowTutorial()
        {
            if (!PlayerDataManager.PlayerData.IntroToStarRushTutorial)
            {
                m_mask.SetActive(true);
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    m_mask.SetActive(false);
                    ServiceLocator.PopupScene.ShowPopup(PopupName.StarRushTutorial);
                });
            }
        }

        private void OnStart()
        {
            m_mask.SetActive(false);
            m_previewRewards.gameObject.SetActive(false);
            HandleInProcessEntries();
            HandleCompleted();
        }

        private void OnHide(Action onCompleted = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.StarRush, () =>
            {
                GameLogic.AutoEventPopupClosed = true;
                onCompleted?.Invoke();
                if (callToPlay)
                {
                    if (!StarRushManager.Data.CallToPlay)
                    {
                        StarRushManager.Data.CallToPlay = true;
                        StarRushManager.OnSave?.Invoke();
                    }
                }
            });
        }

        private void Close()
        {
            OnHide();
        }

        private void OpenHelp()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.StarRushTutorial);
        }

        private void StartEvent()
        {
            OnHide(() =>
            {
                StarRushManager.Data.Start();
                ButtonStarRush.OnUpdateNoti?.Invoke();
                AdLoading.OnLoad?.Invoke(1.0f, () =>
                {
                    Show();
                });
            });
        }

        private void PreviewRewards()
        {
            m_previewRewards.gameObject.SetActive(!m_previewRewards.gameObject.activeSelf);
        }

        private void HandleStatus()
        {
            m_contentStart.SetActive(StarRushManager.Data.CanStart());
            m_contentInProcess.SetActive(StarRushManager.Data.IsRunning());
            m_contentCompleted.SetActive(StarRushManager.Data.IsCompleted());
            buttonContinue.gameObject.SetActive(StarRushManager.Data.IsRunning() || StarRushManager.Data.IsCompleted());

            HandleTimer();
        }

        private void Continue()
        {
            if (StarRushManager.Data.IsCompleted())
            {
                if (yourPosition == 0)
                {
                    OnHide(() =>
                    {
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin(50, Vector3.zero, new LogCurrency("currency", "coin", "event_star_rush", "non_iap", "feature", "star_rush_top_1")));
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterUndo(1, Vector3.zero, new LogCurrency("booster", $"{BoosterType.Undo}", "event_star_rush", "non_iap", "feature", "star_rush_top_1")));
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterAddSlots(1, Vector3.zero, new LogCurrency("booster", $"{BoosterType.AddSlots}", "event_star_rush", "non_iap", "feature", "star_rush_top_1")));
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterClear(1, Vector3.zero, new LogCurrency("booster", $"{BoosterType.Clear}", "event_star_rush", "non_iap", "feature", "star_rush_top_1")));
                        RewardGainController.OnAddRewardGain?.Invoke(new RewardGainInfiniteLive(900, Vector3.zero, new LogCurrency("energy", "infinite_live", "event_star_rush", "non_iap", "feature", "star_rush_top_1"))); // 900 = 15m
                        RewardGainController.OnStartGaining?.Invoke();
                        StarRushManager.Data.UpdateWinCount();
                        StarRushManager.Data.Reset();
                    });
                }
                else
                {
                    OnHide(() =>
                    {
                        StarRushManager.Data.Reset();
                    });
                }
            }
            else if (StarRushManager.Data.IsRunning())
            {
                OnHide();
            }
            ButtonStarRush.OnUpdateUI?.Invoke();
        }

        private void HandleTimer()
        {
            if (StarRushManager.Data.CanStart()) return;

            if (StarRushManager.Data.IsCompleted())
            {
                m_textTimer.text = $"Completed";
                HandleCompleted();
                return;
            }

            try
            {
                string textRemainTime;
                TimeSpan? remainTime = TimeHelper.ParseIsoString(StarRushManager.Data.EndTime) - DateTime.UtcNow;
                if (remainTime?.TotalSeconds > 0)
                {
                    textRemainTime = string.Format("{0:D2}:{1:D2}", remainTime?.Minutes, remainTime?.Seconds);
                }
                else
                {
                    textRemainTime = $"---";

                }
                m_textTimer.text = textRemainTime;
            }
            catch (Exception) { }
        }

        private async UniTask ScoreAdjustment()
        {
            if (!(!StarRushManager.Data.IsCompleted() && (!StarRushManager.Data.Scored || StarRushManager.Data.AutoScoredCount() >= 1))) return;
            StarRushManager.Data.AdjustScore();
            await UniTask.Delay(0);
        }

        private async void HandleInProcessEntries()
        {
            if (!StarRushManager.Data.IsRunning()) return;

            await ScoreAdjustment();

            try
            {
                bool sorted = false;
                List<StarRushPlayer> players = StarRushManager.Data.Players;
                players = players.OrderByDescending(item => item.Score).ToList();
                sorted = true;

                await UniTask.WaitUntil(() => sorted);

                for (int i = 0; i < players.Count; i++)
                {
                    StarRushPlayer player = players[i];
                    StarRushEntry entry = entries[i];
                    entry.Init(player.IsYou ? PlayerDataManager.PlayerData.PlayerProfile : player.PlayerProfile, player.Score, i, player.IsYou);
                }
            }
            catch (Exception) { }
        }

        private async void HandleCompleted()
        {
            if (!StarRushManager.Data.IsCompleted() || yourPosition != -1) return;

            buttonContinue.gameObject.SetActive(StarRushManager.Data.IsCompleted());

            try
            {
                bool sorted = false;
                List<StarRushPlayer> players = StarRushManager.Data.Players;
                players = players.OrderByDescending(item => item.Score).ToList();
                sorted = true;

                await UniTask.WaitUntil(() => sorted);

                for (int i = 0; i < players.Count; i++)
                {
                    StarRushPlayer player = players[i];
                    if (player.IsYou)
                    {
                        yourPosition = i;
                        break;
                    }
                }

                if (yourPosition < 0 || yourPosition >= 5) return;

                resultRanks.ForEach((pos) => pos.SetActive(false));
                int positionIndex = Mathf.Clamp(yourPosition, 0, 3);
                resultRanks[positionIndex].SetActive(true);
                m_textResultRank.text = $"{yourPosition + 1}";
                m_gift.SetActive(yourPosition == 0);
                m_nogift.SetActive(yourPosition > 0);
            }
            catch (Exception) { }
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            if (args is PopupStarRushArgs popupArgs)
            {
                callToPlay = popupArgs.callToPlay;
            }
            base.Show(args, callback);
            InitUI();
            OnStart();
        }
        #endregion
    }
}
