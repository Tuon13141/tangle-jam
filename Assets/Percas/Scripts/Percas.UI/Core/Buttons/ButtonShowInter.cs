using System;
using Percas.IAA;
using Sonat;
using UnityEngine;

namespace Percas.UI
{
    public class ButtonShowInter : ButtonBase
    {
        [SerializeField] AdPlacement adPlacement;

        [HideInInspector]
        public Action onStarted;
        public Action onCompleted;
        public Action onRewarded;

        protected override void Awake()
        {
            base.Awake();
            SetPointerClickEvent(ShowInter);
        }

        private void ShowInter()
        {
            bool interShown = false;
            if (GameLogic.CanShowInter && GameLogic.ConditionShowInter && Kernel.Resolve<AdsManager>().IsInterstitialAdsReady())
            {
                IAAManager.CanShowAppOpen = false;
                onStarted?.Invoke();
                var log = new SonatLogShowInterstitial()
                {
                    location = "ingame",
                    screen = adPlacement.ToString(),
                    placement = adPlacement.ToString(),
                    level = GameLogic.CurrentLevel,
                    mode = PlayMode.classic.ToString()
                };
                log.Post(logAf: true);
                interShown = Kernel.Resolve<AdsManager>().ShowInterstitial(log, false, false, () =>
                {
                    IAAManager.UpdateLastTimeAdShown();
                    onRewarded?.Invoke();
                    onCompleted?.Invoke();
                    IAAManager.OnInterstitialAdClosed?.Invoke();
                    IAAManager.CanShowAppOpen = true;
                });
            }
            if (interShown) return;
            onStarted?.Invoke();
            onCompleted?.Invoke();
        }
    }
}
