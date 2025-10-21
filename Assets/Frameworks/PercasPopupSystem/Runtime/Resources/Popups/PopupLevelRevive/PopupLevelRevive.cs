using System;
using Percas.Data;
using Percas.IAR;
using Percas.UI;
using TMPro;
using UnityEngine;

namespace Percas
{
    public class PopupLevelRevive : PopupBase
    {
        [SerializeField] ButtonBase buttonClosePopup;
        [SerializeField] ButtonUseCoin buttonUseCoin;
        [SerializeField] ButtonWatchVideoAd buttonWatchVideoAd;
        [SerializeField] ButtonPurchase buttonPurchase;
        [SerializeField] TMP_Text textLevelCoil;
        [SerializeField] TMP_Text textMessage;
        [SerializeField] TMP_Text textRevivePrice;

        private int revivePrice;

        protected override void Awake()
        {
            RegisterButtons();
        }

        private void RegisterButtons()
        {
            buttonClosePopup.SetPointerClickEvent(MoveOn);

            buttonUseCoin.onStart = OnStartAction;
            buttonUseCoin.onCompleted = OnUseCoinCompleted;
            buttonUseCoin.onError = OnErrorUseCoin;

            buttonWatchVideoAd.skipVideo = false;
            buttonWatchVideoAd.onStart = OnStartAction;
            buttonWatchVideoAd.onCompleted = OnWatchVideoCompleted;

            buttonPurchase.onCallback = OnPurchaseCallback;
        }

        private void InitUI()
        {
            ActionEvent.OnLevelLose?.Invoke();

            ActionEvent.OnPlaySFXGameLose?.Invoke();

            revivePrice = GameLogic.PriceRevive;
            buttonUseCoin.SetCoinToSpend(revivePrice);

            textLevelCoil.text = $"x{Math.Max(9, GameLogic.LevelCoil)}";
            textMessage.text = LevelController.instance.CheckReviveBoosterAddSlot() ? $"{Const.LANG_KEY_REVIVE_ADD_SLOTS}" : $"{Const.LANG_KEY_REVIVE_UNDO}";
            textRevivePrice.text = $"{revivePrice}";
        }

        private void MoveOn()
        {
            if (GameLogic.IsClassicMode)
            {
                ServiceLocator.PopupScene.HidePopup(PopupName.LevelRevive, () =>
                {
                    ServiceLocator.PopupScene.ShowPopup(PopupName.LevelFailed);
                });
            }
            else if (GameLogic.IsHiddenPictureMode)
            {
                ServiceLocator.PopupScene.HidePopup(PopupName.LevelRevive, () =>
                {
                    GlobalSetting.OnGameToHome?.Invoke(null);
                });
            }
        }

        private void OnStartAction(Action<bool> onCallback)
        {
            onCallback?.Invoke(true);
        }

        private void OnUseCoinCompleted()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.LevelRevive, () =>
            {
                PlayerDataManager.OnUpdateContinueTimes?.Invoke();
                if (LevelController.instance != null) LevelController.instance.ReviveBooster();

                var logCurrency = new LogCurrency("booster", "revive", "buy_booster", "non_iap", "feature", "use_coin");
                TrackingManager.OnEarnBooster?.Invoke(logCurrency, 1);
                TrackingManager.OnSpendBooster?.Invoke(logCurrency);

                TrackingManager.OnLevelEnd?.Invoke(false, "replay_when_lose");
                TrackingManager.OnContinuePlaying?.Invoke("use_coin_to_revive");
            });
        }

        private void OnWatchVideoCompleted()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.LevelRevive, () =>
            {
                PlayerDataManager.PlayerData.AddVideoReviveCount();
                buttonWatchVideoAd.reward = new(RewardType.BoosterRevive, 1, null);
                PlayerDataManager.OnUpdateContinueTimes?.Invoke();
                if (LevelController.instance != null) LevelController.instance.ReviveBooster();

                var logCurrency = new LogCurrency("booster", "revive", "buy_booster", "non_iap", "ads", "rwd_ads");
                TrackingManager.OnEarnBooster?.Invoke(logCurrency, 1);
                TrackingManager.OnSpendBooster?.Invoke(logCurrency);

                TrackingManager.OnLevelEnd?.Invoke(false, "replay_when_lose");
                TrackingManager.OnContinuePlaying?.Invoke("watch_video_to_revive");
            });
        }

        private void OnErrorUseCoin()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.Shop);
        }

        private void OnPurchaseCallback()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.LevelRevive, () =>
            {
                PlayerDataManager.OnUpdateContinueTimes?.Invoke();
                if (LevelController.instance != null) LevelController.instance.ReviveBooster();

                var logCurrency = new LogCurrency("booster", "revive", "buy_booster", "iap", "pack", "starter_pack_purchased");
                TrackingManager.OnEarnBooster?.Invoke(logCurrency, 1);
                TrackingManager.OnSpendBooster?.Invoke(logCurrency);

                TrackingManager.OnLevelEnd?.Invoke(false, "replay_when_lose");
                TrackingManager.OnContinuePlaying?.Invoke("starter_pack_purchased");
            });
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            InitUI();
        }
        #endregion
    }
}
