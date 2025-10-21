using UnityEngine;
using Percas.IAA;
using System;
using Sonat;
using Percas.IAR;
using Percas.Data;

namespace Percas.UI
{
    public class ButtonWatchVideoAd : ButtonBase
    {
        [SerializeField] AdPlacement adPlacement;

        [HideInInspector]
        public bool skipVideo = false;
        public Action<Action<bool>> onStart;
        public Action onCompleted;
        public Action onNotCompleted;
        public Action onError;
        public Reward reward = new();

        protected override void Awake()
        {
            base.Awake();
            SetPointerClickEvent(WatchVideoAd);
        }

        private string GetItemType(RewardType rewardType)
        {
            return rewardType switch
            {
                RewardType.Coin or RewardType.Coil or RewardType.CoinAndCoil or RewardType.Live => "currency",
                RewardType.BoosterUndo or RewardType.BoosterAddSlots or RewardType.BoosterShuffle or RewardType.BoosterClear or RewardType.BoosterRevive => "booster",
                _ => "others",
            };
        }

        private void WatchVideoAd()
        {
            if (skipVideo)
            {
                onStart?.Invoke((canStart) =>
                {
                    if (canStart)
                    {
                        onCompleted?.Invoke();
                    }
                });
            }
            else
            {
                onStart?.Invoke((canStart) =>
                {
                    if (canStart)
                    {
#if UNITY_EDITOR && use_admob
                        onCompleted?.Invoke();
#else
                        if (Kernel.Resolve<AdsManager>().IsVideoAdsReady())
                        {
                            IAAManager.CanShowAppOpen = false;
                            var log = new SonatLogVideoRewarded()
                            {
                                mode = PlayMode.classic.ToString(),
                                level = GameLogic.CurrentLevel,
                                phase = GameLogic.CurrentLevelPhase,
                                location = "ingame",
                                placement = adPlacement.ToString(),
                                item_type = GetItemType(reward.RewardType),
                                item_id = reward.RewardType.ToString()
                            };
                            log.Post(logAf: true);
                            bool isRewarded = false;
                            Kernel.Resolve<AdsManager>().ShowVideoAds(() =>
                            {
                                isRewarded = true;
                                PlayerDataManager.PlayerData.UpdateCountRewardAds();
                                ButtonRewardAds.OnUpdateNoti?.Invoke();
                                onCompleted?.Invoke();
                                IAAManager.UpdateLastTimeAdShown();
                                IAAManager.OnInterstitialAdClosed?.Invoke();
                                IAAManager.CanShowAppOpen = true;
                            }, log);
                            if (!isRewarded) onNotCompleted?.Invoke();
                        }
                        else
                        {
                            ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_VIDEO_NOT_READY);
                            onNotCompleted?.Invoke();
                        }

#endif
                    }
                });
            }
        }
    }
}
