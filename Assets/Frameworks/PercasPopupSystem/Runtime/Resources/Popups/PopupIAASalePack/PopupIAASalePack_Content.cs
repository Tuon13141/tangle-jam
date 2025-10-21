using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Percas.IAA;
using Percas.IAR;
using Percas.UI;
using System;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using Percas.Data;

namespace Percas
{
    public class PopupIAASalePack_Content : MonoBehaviour
    {
        [SerializeField] Image imagePack;
        [SerializeField] TMP_Text textDescription, textCoinPrice;
        [SerializeField] ButtonUseCoin buttonUseCoin;
        [SerializeField] ButtonWatchVideoAd buttonWatchVideoAd;
        [SerializeField] SkeletonGraphic tutorialAnimation;

        private IAASalePackDataSO packData;
        private Action OnCompleted;

        private void Awake()
        {
            RegisterButtons();
        }

        public void SetContent(IAASalePackDataSO data, Action onCompleted)
        {
            packData = data;
            OnCompleted = onCompleted;
            if (data.skeletonData != null)
            {
                tutorialAnimation.skeletonDataAsset = data.skeletonData;
                tutorialAnimation.AnimationState.SetAnimation(0, "idle", true);
                imagePack.gameObject.SetActive(false);
                tutorialAnimation.gameObject.SetActive(true);
            }
            else
            {
                imagePack.sprite = packData.image;
                imagePack.gameObject.SetActive(true);
                tutorialAnimation.gameObject.SetActive(false);
            }
            textDescription.text = $"{packData.description}";
            textCoinPrice.text = $"{packData.coinPrice}";
            buttonUseCoin.gameObject.SetActive(packData.coinPrice > 0);
            buttonUseCoin.SetCoinToSpend(packData.coinPrice);
        }

        private void RegisterButtons()
        {
            buttonUseCoin.onStart = OnStartAction;
            buttonUseCoin.onCompleted = OnCompletedUseCoin;
            buttonUseCoin.onError = OnErrorUseCoin;

            buttonWatchVideoAd.skipVideo = false;
            buttonWatchVideoAd.onStart = OnStartAction;
            buttonWatchVideoAd.onCompleted = OnCompletedWatchVideoAd;
        }

        private void OnStartAction(Action<bool> onCallback)
        {
            onCallback?.Invoke(true);
        }

        private void OnUseBooster(bool isUseCoin)
        {

            Debug.Log("OnUseBooster");
            if (packData.rewards.Count <= 0) return;
            if (packData.useImmediately)
            {
                Debug.Log("OnUseBooster123");
                bool result = false;
                foreach (Reward reward in packData.rewards)
                {
                    var name = reward.RewardType.ToString().ToLower().Replace("booster", "");
                    var logCurrency = new LogCurrency("booster", name, "buy_booster", "non_iap", isUseCoin ? "feature" : "ads", isUseCoin ? "use_coin" : "rwd_ads");
                    switch (reward.RewardType)
                    {
                        case RewardType.BoosterUndo:
                            buttonWatchVideoAd.reward = new(RewardType.BoosterUndo, 1, logCurrency);
                            //result = LevelController.instance?.UndoBooster() ?? false;
                            LevelController.instance?.ShuffleBooster().Forget();
                            TrackingManager.OnEarnBooster?.Invoke(logCurrency, 1);
                            TrackingManager.OnSpendBooster?.Invoke(logCurrency);
                            break;

                        case RewardType.BoosterAddSlots:
                            //logCurrency = new LogCurrency("booster", "add_slots", "buy_booster", "non_iap", isUseCoin ? "feature" : "ads", isUseCoin ? "use_coin" : "rwd_ads");
                            buttonWatchVideoAd.reward = new(RewardType.BoosterAddSlots, 1, logCurrency);
                            LevelController.instance?.AddSlotBooster();
                            TrackingManager.OnEarnBooster?.Invoke(logCurrency, 1);
                            TrackingManager.OnSpendBooster?.Invoke(logCurrency);
                            break;

                        case RewardType.BoosterClear:
                            //logCurrency = new LogCurrency("booster", "clear", "buy_booster", "non_iap", isUseCoin ? "feature" : "ads", isUseCoin ? "use_coin" : "rwd_ads");
                            buttonWatchVideoAd.reward = new(RewardType.BoosterClear, 1, logCurrency);
                            LevelController.instance?.RollCollectBooster();
                            TrackingManager.OnEarnBooster?.Invoke(logCurrency, 1);
                            TrackingManager.OnSpendBooster?.Invoke(logCurrency);
                            break;
                    }
                }
                OnCompleted?.Invoke();
            }
        }

        #region Button Use Coin
        private void OnCompletedUseCoin()
        {
            OnUseBooster(true);
        }

        private void OnErrorUseCoin()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.Shop);
        }
        #endregion

        #region Button Watch Video Ad
        private void OnCompletedWatchVideoAd()
        {
            OnUseBooster(false);
        }
        #endregion
    }
}
