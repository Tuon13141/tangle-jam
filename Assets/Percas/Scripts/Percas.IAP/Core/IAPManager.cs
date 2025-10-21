using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

namespace Percas.IAP
{
    public class IAPManager : MonoBehaviour, IDetailedStoreListener
    {
        public static Action<string> OnPurchase;
        public static Action OnRestore;

        public static Func<string, IAPPack> OnGetPack;
        public static Func<string, Product> OnGetProduct;

        IStoreController m_StoreController;

        IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;
        IAppleExtensions m_AppleExtensions;

        CrossPlatformValidator m_Validator = null;

        [SerializeField] private IAPPack[] packs;

        private string _productID;

        private void Awake()
        {
            OnPurchase += Purchase;
            OnRestore += Restore;

            OnGetPack += GetPack;
            OnGetProduct += GetProduct;
        }

        private void OnDestroy()
        {
            OnPurchase -= Purchase;
            OnRestore -= Restore;

            OnGetPack -= GetPack;
            OnGetProduct -= GetProduct;
        }

        void Start()
        {
            InitializePurchasing();
        }

        static bool IsCurrentStoreSupportedByValidator()
        {
            //The CrossPlatform validator only supports the GooglePlayStore and Apple's App Stores.
            return IsGooglePlayStoreSelected() || IsAppleAppStoreSelected();
        }

        static bool IsGooglePlayStoreSelected()
        {
            var currentAppStore = StandardPurchasingModule.Instance().appStore;
            return currentAppStore == AppStore.GooglePlay;
        }

        static bool IsAppleAppStoreSelected()
        {
            var currentAppStore = StandardPurchasingModule.Instance().appStore;
            return currentAppStore == AppStore.AppleAppStore ||
                currentAppStore == AppStore.MacAppStore;
        }

        void InitializePurchasing()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            //Add products that will be purchasable and indicate its type.
            foreach (IAPPack pack in packs)
            {
                builder.AddProduct(pack.productID.ToString(), pack.productType);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("In-App Purchasing successfully initialized");
            m_StoreController = controller;
            InitializeValidator();
        }

        void InitializeValidator()
        {
            if (IsCurrentStoreSupportedByValidator())
            {
#if !UNITY_EDITOR
            var appleTangleData = AppleTangle.Data();
            m_Validator = new CrossPlatformValidator(GooglePlayTangle.Data(), appleTangleData, Application.identifier);
#endif
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            OnInitializeFailed(error, null);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

            if (message != null)
            {
                errorMessage += $" More details: {message}";
            }

            Debug.Log(errorMessage);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
            ServiceLocator.PopupScene.ShowPopup(PopupName.Error, new PopupErrorArgs($"{failureReason}"));
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
                $" Purchase failure reason: {failureDescription.reason}," +
                $" Purchase failure details: {failureDescription.message}");
            ServiceLocator.PopupScene.ShowPopup(PopupName.Error, new PopupErrorArgs($"{failureDescription.reason}"));
        }

        private void Purchase(string productID)
        {
            if (GetPack(productID) == null || m_StoreController == null)
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_SOMETHING_WRONG);
                return;
            }

            _productID = productID;

            m_StoreController.InitiatePurchase(productID.ToString());
        }

        private IAPPack GetPack(string productID)
        {
            try
            {
                for (int i = 0; i < packs.Length; i++)
                {
                    if (packs[i].productID == productID) return packs[i];
                }
            }
            catch (Exception) { }
            return null;
        }

        private Product GetProduct(string _productID)
        {
            try
            {
                return m_StoreController.products.WithID(_productID);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GetProduct.Error] {ex.Message}");
            }
            return null;
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            //Retrieve the purchased product
            var product = args.purchasedProduct;

            var isPurchaseValid = IsPurchaseValid(product);

            if (isPurchaseValid)
            {
                //Add the purchased product to the players inventory
                PurchaseCompleted(product);
                Debug.Log("Valid receipt, unlocking content.");
            }
            else
            {
                Debug.Log("Invalid receipt, not unlocking content.");
            }

            //We return Complete, informing Unity IAP that the processing on our side is done and the transaction can be closed.
            return PurchaseProcessingResult.Complete;
        }

        bool IsPurchaseValid(Product product)
        {
            //If we the validator doesn't support the current store, we assume the purchase is valid
            if (IsCurrentStoreSupportedByValidator())
            {
                try
                {
                    var result = m_Validator.Validate(product.receipt);

                    //The validator returns parsed receipts.
                    LogReceipts(result);
                }

                //If the purchase is deemed invalid, the validator throws an IAPSecurityException.
                catch (IAPSecurityException reason)
                {
                    Debug.Log($"Invalid receipt: {reason}");
                    return false;
                }
            }

            return true;
        }

        private void Restore()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            m_GooglePlayStoreExtensions.RestoreTransactions(GoRestore);
#elif UNITY_IOS && !UNITY_EDITOR
            m_AppleExtensions.RestoreTransactions(GoRestore);
#else
            ActionEvent.OnShowToast?.Invoke($"This store is not supported!");
#endif
        }

        private void GoRestore(bool success, string error)
        {
            string restoreMessage;
            if (success)
            {
                // This does not mean anything was restored,
                // merely that the restoration process succeeded.
                restoreMessage = "Restore Successful";
            }
            else
            {
                // Restoration failed.
                restoreMessage = $"Restore Failed with error: {error}";
            }

            Debug.Log(restoreMessage);
            ActionEvent.OnShowToast?.Invoke(restoreMessage);
        }

        void PurchaseCompleted(Product product)
        {
            Debug.LogError($"PurchaseCompleted = {product.definition.id}");
            IAPPackFactory.GetPack(product.definition.id).BuyPack();
            TrackingManager.OnPurchaseCompleted?.Invoke(GetPack(_productID));
        }

        static void LogReceipts(IEnumerable<IPurchaseReceipt> receipts)
        {
            Debug.Log("Receipt is valid. Contents:");
            foreach (var receipt in receipts)
            {
                LogReceipt(receipt);
            }
        }

        static void LogReceipt(IPurchaseReceipt receipt)
        {
            Debug.Log($"Product ID: {receipt.productID}\n" +
                $"Purchase Date: {receipt.purchaseDate}\n" +
                $"Transaction ID: {receipt.transactionID}");

            if (receipt is GooglePlayReceipt googleReceipt)
            {
                Debug.Log($"Purchase State: {googleReceipt.purchaseState}\n" +
                    $"Purchase Token: {googleReceipt.purchaseToken}");
            }

            if (receipt is AppleInAppPurchaseReceipt appleReceipt)
            {
                Debug.Log($"Original Transaction ID: {appleReceipt.originalTransactionIdentifier}\n" +
                    $"Subscription Expiration Date: {appleReceipt.subscriptionExpirationDate}\n" +
                    $"Cancellation Date: {appleReceipt.cancellationDate}\n" +
                    $"Quantity: {appleReceipt.quantity}");
            }
        }
    }
}
