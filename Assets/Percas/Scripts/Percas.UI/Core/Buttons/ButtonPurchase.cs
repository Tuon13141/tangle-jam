using System;
using TMPro;
using UnityEngine;
using Percas.IAP;
using Percas.Data;
using Sonat;
using Percas.IAA;

namespace Percas.UI
{
    public class ButtonPurchase : ButtonBase, IActivatable
    {
        [SerializeField] string productID;
        [SerializeField] TMP_Text txtPrice;
        [SerializeField] int shopItemKey;
        [SerializeField] string itemType;

        public Action<Action<bool>> onStart;
        public Action onCallback;

        protected override void Awake()
        {
            base.Awake();
            SetPointerClickEvent(Purchase);
        }

        private void OnEnable()
        {
            if (txtPrice != null)
            {
                txtPrice.text = $"{Kernel.Resolve<Purchaser>().GetPriceText(shopItemKey)}";
            }
        }

        public void Activate()
        {
            if (txtPrice != null)
            {
                txtPrice.text = $"{Kernel.Resolve<Purchaser>().GetPriceText(shopItemKey)}";
            }
        }

        public void Deactivate() { }

        private void Purchase()
        {
            if (!GameConfig.Instance.IsReachableInternet())
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_SOMETHING_WRONG);
                return;
            }

            if (onStart != null)
            {
                onStart?.Invoke((canStart) =>
                {
                    if (!canStart) return;
                    OnSonatPurchase();
                });
            }
            else
            {
                OnSonatPurchase();
            }
        }

        private void OnSonatPurchase()
        {
            if (GameConfig.Instance.CheatOn)
            {
                IAPPackFactory.GetPack(productID).BuyPack();
                PlayerDataManager.UpdatePurchaseHistory(productID);
                onCallback?.Invoke();
            }
            else
            {
                AdLoading.OnDisplay?.Invoke(true);
                IAAManager.CanShowAppOpen = false;
                var logInput = new SonatLogBuyShopItemIapInput(placement, itemType, GameLogic.CurrentLevelPhase)
                {
                    location = GameLogic.IsInGame ? "ingame" : "home",
                    screen = GlobalSetting.ScreenName,
                };
                Kernel.Resolve<Purchaser>().Buy(shopItemKey, (done) =>
                {
                    if (done)
                    {
                        IAPPackFactory.GetPack(productID).BuyPack();
                        PlayerDataManager.UpdatePurchaseHistory(productID);
                        onCallback?.Invoke();
                    }
                    else
                    {
                        ServiceLocator.PopupScene.ShowPopup(PopupName.Error, new PopupErrorArgs($"{Const.LANG_KEY_SOMETHING_WRONG}"));
                    }
                    AdLoading.OnDisplay?.Invoke(false);
                    IAAManager.OnInterstitialAdClosed?.Invoke();
                    IAAManager.CanShowAppOpen = true;
                }, logInput);
            }
        }
    }
}
