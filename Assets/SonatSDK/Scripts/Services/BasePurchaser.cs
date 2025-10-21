#define dummy
//#define use_iap

//#define SUBSCRIPTION_MANAGER

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
#if !((dummy || global_dummy) && !use_iap)
using Sonat;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

#endif

[Serializable]
public class RespondVerifyIap
{
    public int errorCode;
}

public class ShopItemKeyAction : BaseAction<int>
{
    public ShopItemKeyAction()
    {
        Action = x =>
            Console.WriteLine($"I just executed Event 6 with {x}");
    }
}

[Serializable]
public class StoreProductDescriptor
{
    [IndexAsEnumSonatSdk(nameof(BuiltInEnumType.ShopItemKey))]
    public int key;

    public bool active = true;

    public string StoreProductId
    {
        get
        {
#if UNITY_ANDROID
            return storeProductId;
#else
          if (string.IsNullOrEmpty(storeProductId_ios))
              return storeProductId;
          else
              return storeProductId_ios;
#endif
        }
    }

    [SerializeField] private string storeProductId;
    [SerializeField] private string storeProductId_ios;
    public float price;
    public string description;
    public ProductType productType = ProductType.Consumable;

    public void SetName(string defaultPackageName)
    {
        storeProductId = defaultPackageName;
        storeProductId_ios = defaultPackageName;
    }
}


#if (dummy || global_dummy) && !use_iap
public enum ProductType
{
    Consumable,
    NonConsumable,
    Subscription
}
public abstract class BasePurchaser : BaseService
{
    public abstract List<StoreProductDescriptor> StoreProductDescriptors { get; }
    public ShopItemKeyAction onInAppPurchased = new ShopItemKeyAction();
    
    public int[] OthersNoAdsKey => othersNoAdsKey;
    [SerializeField] private int[] othersNoAdsKey;
    [SerializeField] private bool purchaseConnector = true;

	
    public static float sn_ltv_iap
    {
        get => PlayerPrefs.GetFloat(nameof(sn_ltv_iap));
        set
        {
            PlayerPrefs.SetFloat(nameof(sn_ltv_iap),value);
        }
    }

    protected override void Register()
    {
        base.Register();
        Initialized = true;
      
    }

    public string GetPrice(int inAppKey)
    {
        return "dummy";
    }

    public object StoreController { get; set; }

    public void Buy(int id, Action<bool> onSuccess, Sonat.SonatLogBuyShopItemIapInput log)
    {
        Debug.Log("Buy(ShopItemKey ShopItemKey, Action action)");
        onSuccess.Invoke(true);
    }

    public virtual string GetPriceText(int shopItemKey)
    {
        return "dummy";
    }
    
    public bool Dummy { get; private set; }

    public void SetDummy(bool value)
    {
        Dummy = value;
    }
    

    public bool IsItemAvailable(int id)
    {
        return false;
    }    
    public bool IsInitialized()
    {
        return true;
    }

    public bool IsSubcribed(string storeProductId,out int expiredIn)
    {
        expiredIn = 0;
        return false;
    }

    public virtual bool RestorePurchase(bool isAuto = false)
    {
        return false;
    }
    
    protected virtual void OnInitializedSuccessfully()
    {
        
    }
    
    public Action onInitializedSuccess;


}
#else


public abstract class BasePurchaser : BaseService, IStoreListener, IDetailedStoreListener
{
    public abstract List<StoreProductDescriptor> StoreProductDescriptors { get; }
    public static IStoreController _mStoreController; // The Unity Purchasing system.
    public static IExtensionProvider _mStoreExtensionProvider; // The store-specific Purchasing subsystems.
    private KeyValuePair<int, Action<bool>> _onSuccess;
    public ShopItemKeyAction onInAppPurchased = new ShopItemKeyAction();

    public bool Dummy { get; private set; }
    [SerializeField] private bool autoRestore;
    [SerializeField] private float timeWaitStore = 10;

    public static float sn_ltv_iap
    {
        get => PlayerPrefs.GetFloat(nameof(sn_ltv_iap));
        set
        {
            var last = PlayerPrefs.GetFloat(nameof(sn_ltv_iap));
            if (Math.Abs(last - value) > 0.0001f)
            {
                Kernel.Resolve<FireBaseController>()
                    .SetUserProperty(nameof(sn_ltv_iap), value.ToString(CultureInfo.InvariantCulture));
                PlayerPrefs.SetFloat(nameof(sn_ltv_iap), value);
            }
        }
    }

    public void SetDummy(bool value)
    {
        Dummy = value;
    }

    private bool _isBuy;

    [SerializeField] private int[] othersNoAdsKey;

    public int[] OthersNoAdsKey => othersNoAdsKey;

    public virtual bool RestorePurchase(bool isAuto = false)
    {
        var noAdsItem = StoreProductDescriptors.Find(x => x.key == 0);
        if (_mStoreController.products.WithID(noAdsItem.StoreProductId).hasReceipt)
        {
            Debug.Log("duong restore " + noAdsItem.StoreProductId);
            Kernel.Resolve<AdsManager>().EnableNoAds();
            return true;
        }

        foreach (var key in othersNoAdsKey)
        {
            var otherNoAds = StoreProductDescriptors.Find(x => x.key == key);
            if (_mStoreController.products.WithID(otherNoAds.StoreProductId).hasReceipt)
            {
                Debug.Log("duong restore " + otherNoAds.StoreProductId);
                Kernel.Resolve<AdsManager>().EnableNoAds();
                return true;
            }
        }

        Debug.Log("not found no ads");
        return false;
    }

    public bool CheckHasPurchasedProductId(int id)
    {
        var product = StoreProductDescriptors.Find(x => x.key == id);
        foreach (var storeProductDescriptor in StoreProductDescriptors)
            if (storeProductDescriptor.key == id &&
                _mStoreController.products.WithID(product.StoreProductId).hasReceipt)
                return true;

        return false;
    }

    //    private Dictionary<string, string> _currency = new Dictionary<string, string>()
    //    {
    //        {"USD", "$ " },
    //        { "EUR", "€ " },
    //        { "GBP", "£ " },
    //        { "CNY", "¥ " },
    //        { "HKD", "$ " },
    //        { "JPY", "¥ " },
    //        { "MOP", "$ " },
    //        { "SGD", "$ " },
    //        { "KRW", "₩ " }, 
    //        {"VND", "₫ "},
    //    };

    // meaning move currency symbol to last of string text
    private readonly List<string> _currencyLast = new List<string>()
    {
        "₫",
    };


    public bool PurchasedUser
    {
        get => PlayerPrefs.GetInt(nameof(PurchasedUser)) == 1;
        set { PlayerPrefs.SetInt(nameof(PurchasedUser), value ? 1 : 0); }
    }

    protected override void Register()
    {
        base.Register();
        if (_mStoreController == null)
            InitializePurchasing();
        CheckLoadLoadingTransition();
    }

    private void CheckLoadLoadingTransition()
    {
        if (LoadingTransition.Instance == null)
        {
            var transition = FindObjectOfType<LoadingTransition>(true);
            if (transition != null)
                transition.CheckRegister();
        }
    }

    private void InitializePurchasing()
    {
        Debug.Log("InitializePurchasing times:" + tried);

#if UNITY_EDITOR
        foreach (var storeProductDescriptor in StoreProductDescriptors)
        {
            if (storeProductDescriptor.StoreProductId.Contains(" "))
                Debug.LogError("invalid id : contains space");
        }
#endif
        if (IsInitialized())
        {
            Debug.Log("Purchaser IsInitialized return");
            return;
        }

#if !AMZ
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
#else
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance( AppStore.AmazonAppStore));

#endif
        Initialized = true;
        foreach (var descriptor in StoreProductDescriptors)
        {
            if (descriptor.StoreProductId == "" || descriptor.StoreProductId == String.Empty)
                Debug.LogError("empty", gameObject);
            if (StoreProductDescriptors.Count(x =>
                    x.StoreProductId == descriptor.StoreProductId) > 1)
                Debug.LogError("duplicate" + descriptor.StoreProductId, gameObject);

            if (descriptor.active)
                builder.AddProduct(descriptor.StoreProductId, descriptor.productType);
        }


        UnityPurchasing.Initialize(this, builder);
    }


    public static CultureInfo GetCultureInfoFromIsoCurrencyCode(string code)
    {
        foreach (CultureInfo ci in CultureInfo.GetCultures(
            CultureTypes.SpecificCultures))
        {
            RegionInfo ri = new RegionInfo(ci.LCID);
            if (ri.ISOCurrencySymbol == code)
                return ci;
        }

        return null;
    }

    public virtual string GetPriceText(int itemId)
    {
        var item = StoreProductDescriptors.Find(x => x.key == itemId);

        #if UNITY_EDITOR
                return "$" + item.price;
        #endif
        if (_mStoreController != null)
        {
            var product = _mStoreController.products.WithID(item.StoreProductId);
            if (product != null && product.availableToPurchase)
            {
                if (_currencyLast.Contains(product.metadata.isoCurrencyCode))
                    return MoveAllSc(product.metadata.localizedPriceString);
                return product.metadata.localizedPriceString;
            }

            //  return "$" + item.price;
        }

        return "$" + item.price;
    }

    public virtual string GetPriceTextDefault(int itemId)
    {
        var item = StoreProductDescriptors.Find(x => x.key == itemId);
        return "$" + item.price;
    }

    public virtual bool IsItemAvailable(int itemId)
    {
        var item = StoreProductDescriptors.Find(x => x.key == itemId);
        if (_mStoreController != null)
        {
            var product = _mStoreController.products.WithID(item.StoreProductId);
            if (product != null && product.availableToPurchase)
                return true;
        }

        return false;
    }

    public static string GetCurrencySymbol(string code)
    {
        RegionInfo regionInfo =
            (from culture in CultureInfo.GetCultures(CultureTypes
                    .InstalledWin32Cultures)
             where culture.Name.Length > 0 && !culture.IsNeutralCulture
             let region = new RegionInfo(culture.LCID)
             where String.Equals(region.ISOCurrencySymbol, code, StringComparison.InvariantCultureIgnoreCase)
             select region).First();

        return regionInfo.CurrencySymbol;
    }

    private IEnumerator WaitForApplicationFocus()
    {
        Debug.Log("wait for application focus");
        _waitApplicationOutFocus = true;
        LoadingTransition.Instance.TurnOffForceHide = true;
        yield return new WaitForSeconds(0.25f);
        bool show = false;
        if (_waitApplicationOutFocus)
        {
            StartCoroutine(LoadingTransition.Instance.LoadingIn(0, true));
            show = true;
        }

        float t = 0f;
        while (_waitApplicationOutFocus && t < timeWaitStore)
        {
            yield return null;
            t += Time.deltaTime;
        }

        Debug.Log("close loading" + _waitApplicationOutFocus);
        if (show)
            yield return LoadingTransition.Instance.LoadingOut(() => { });
    }

    [ContextMenu("test")]
    private void TEst()
    {
        StartCoroutine(WaitForApplicationFocus());
    }

    private bool _waitApplicationOutFocus;

#if UNITY_ANDROID
    private void OnApplicationFocus(bool hasFocus)
    {
        if (_waitApplicationOutFocus && !hasFocus)
        {
            Debug.Log("_waitApplicationOutFocus set false");
            _waitApplicationOutFocus = false;
            if (LoadingTransition.Instance != null)
                LoadingTransition.Instance.Hide();
        }
    }
#endif

#if UNITY_IOS
    private void OnApplicationPause(bool hasFocus)
    {
        if (_waitApplicationOutFocus && hasFocus)
        {
            Debug.Log("_waitApplicationOutFocus set false");
            _waitApplicationOutFocus = false;
            LoadingTransition.Instance.Hide();
        }
    }
#endif


    public class BuyIapContent
    {
        public Action<bool> OnSuccess;
        public SonatLogBuyShopItemIapInput Log;
    }


    public void Buy(int id, Action<bool> onSuccess, SonatLogBuyShopItemIapInput log)
    {
        if (_isBuy) return;

        var content = new BuyIapContent()
        {
            OnSuccess = onSuccess,
            Log = log
        };

#if !UNITY_EDITOR
        if (IsInitialized() && IsItemAvailable(id) && timeWaitStore >= 1 && LoadingTransition.Instance != null)
            StartCoroutine(WaitForApplicationFocus());
#endif

        Debug.Log("Dummy" + Dummy);
        if (Dummy)
        {
            Debug.Log("Dummy buy key:" + id);

            onSuccess.Invoke(true);
            return;
        }
        else if (!IsInitialized())
        {
            Debug.Log("duong : BasePurchaser not IsInitialized");
            return;
        }

        UIDebugLog.Log("Try to buy key" + id);
        _onSuccess = new KeyValuePair<int, Action<bool>>(id, onSuccess);
        var item = StoreProductDescriptors.Find(x => x.key == id);
        if (_dictionary.ContainsKey(item.StoreProductId))
            _dictionary[item.StoreProductId] = content;
        else
            _dictionary.Add(item.StoreProductId, content);
        _isBuy = true;
        string storeProductId = StoreProductDescriptors.Find(x => x.key == id).StoreProductId;
        BuyProductId(storeProductId);
        AdsManager.AppLeaving = 30;
        Kernel.Resolve<FireBaseController>().LogEvent(EventNameEnum.buy_iap, new[]
        {
            new LogParameter(ParameterEnum.product_click_buy, storeProductId)
        });
    }

    private readonly Dictionary<string, BuyIapContent> _dictionary = new Dictionary<string, BuyIapContent>();


    private PlayerPrefListInt _listPurchased;

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        UIDebugLog.Log("OnPurchaseProcessingResult " + args.purchasedProduct.metadata.localizedTitle);
        var specificId = args.purchasedProduct.definition.storeSpecificId;
        if (_dictionary.ContainsKey(specificId))
        {
            BuyIapContent buyIapContent = _dictionary[specificId];
            //_dictionary[specificId] = null;
            _dictionary.Remove(specificId);
            //buyIapContent.OnSuccess.Invoke(true);
            var item = StoreProductDescriptors.Find(x => x.StoreProductId == specificId);
            //OnBuySuccessHandler(item.key);

            UIDebugLog.Log($"duong item.key:{item.key} specificId " + specificId);

            PurchasedUser = true;

            if (_listPurchased == null)
                _listPurchased = new PlayerPrefListInt("item_purchased", new List<int>());
            //            Kernel.Resolve<FireBaseController>().LogEvent("in_app_purchase_" + item.key);
            //            Kernel.Resolve<FireBaseController>().LogEvent("my_in_app_purchase", new[]
            //            {
            //                new LogParameter(ParameterEnum.in_app_key, item.key.ToString()),
            //                new LogParameter(ParameterEnum.store_product_id, item.StoreProductId),
            //                new LogParameter(ParameterEnum.price, item.price),
            //            });

            var log = new SonatLogBuyShopItemIap(buyIapContent.Log)
            {
                currency = args.purchasedProduct.metadata.isoCurrencyCode,
                item_id = item.StoreProductId,
                value = (float)args.purchasedProduct.metadata.localizedPrice,
                value_in_usd = item.price,
                //                is_first_buy = !_listPurchased.Contains(item.key),
                is_first_buy = _listPurchased.Count == 0,
                level = Kernel.ParameterValueRetriever.LevelDisplay,
                mode = Kernel.ParameterValueRetriever.Mode,
                buy_count = _listPurchased.Count + 1
            };

            _listPurchased.Add(item.key);
            _listPurchased.Save();
            buyIapContent.OnSuccess.Invoke(true);
            OnBuySuccessHandler(item.key);

#if UNITY_ANDROID
            VerifyIap(item.productType != ProductType.Subscription ? "product" : "subscription"
                , Application.identifier, item.StoreProductId, args.purchasedProduct.transactionID,
                () =>
                {
                    Log(args, item);
                    log.Post();
                });
#else
             Log(args,item);
                log.Post();
#endif
        }
        // in case of error
        else if (_onSuccess.Value != null)
        {
            UIDebugLog.Log("_onSuccess.Value.Invoke();");
            _onSuccess.Value.Invoke(true);
            OnBuySuccessHandler(_onSuccess.Key);
            PurchasedUser = true;

            var item = StoreProductDescriptors.Find(x => x.key == _onSuccess.Key);
            Kernel.Resolve<FireBaseController>().LogEvent("in_app_purchase_" + item.key);
            Kernel.Resolve<FireBaseController>().LogEvent("my_in_app_purchase", new[]
            {
                new LogParameter(ParameterEnum.in_app_key, item.key.ToString()),
                new LogParameter(ParameterEnum.store_product_id, item.StoreProductId),
                new LogParameter(ParameterEnum.price, item.price),
            });

            //        Kernel.Resolve<AppFlyerController>().SendEvent("af_purchase", new Dictionary<string, string>()
            //        {
            //            //{ "af_revenue",item.Price.ToString()},
            //            //{ "af_content_type",item.StoreProductId},
            //            //{ "af_content_id",((int)item.Key).ToString()},
            //            //{ "af_currency","USD"},
            //            {"af_revenue", args.purchasedProduct.metadata.localizedPriceString},
            //            {"af_quantity", "1"},
            //            {"af_content_type", item.storeProductId},
            //            {"af_content_id", item.key.ToString()},
            //            {"af_order_id", args.purchasedProduct.transactionID},
            //            {"af_receipt_id", args.purchasedProduct.transactionID},
            //            {"af_currency", args.purchasedProduct.metadata.isoCurrencyCode},
            //        });

#if UNITY_ANDROID
            VerifyIap(item.productType != ProductType.Subscription ? "product" : "subscription"
                , Application.identifier, item.StoreProductId, args.purchasedProduct.transactionID,
                () => Log(args, item));
#else
             Log(args,item);
#endif
        }

        _isBuy = false;

        return PurchaseProcessingResult.Complete;
    }

    private void Log(PurchaseEventArgs args, StoreProductDescriptor item)
    {

        var log = new SonatLogInAppPurchaseAppflyer()
        {
            af_revenue = decimal.ToDouble(args.purchasedProduct.metadata.localizedPrice),
            af_quantity = 1,
            af_content_type = item.StoreProductId,
            af_content_id = item.StoreProductId,
            af_order_id = args.purchasedProduct.transactionID,
            af_receipt_id = args.purchasedProduct.transactionID,
            af_currency = args.purchasedProduct.metadata.isoCurrencyCode,
        };
        UIDebugLog.Log("args.purchasedProduct.receipt:" + args.purchasedProduct.receipt);

#if use_af_purchase_connector
        //if (!Kernel.Resolve<AppFlyerController>().purchaseConnector)
        //log.Post();
#else
        log.Post();
#endif
        //        new CustomSonatLog("purchase_event_log", new List<Sonat.LogParameter>()
        //        {
        //            new Sonat.LogParameter("store_product_id", item.StoreProductId),
        //            new Sonat.LogParameter("currency", args.purchasedProduct.metadata.isoCurrencyCode),
        //            new Sonat.LogParameter("localized_price", args.purchasedProduct.metadata.localizedPriceString),
        //        }).Post();
        sn_ltv_iap += item.price;
    }


    public void VerifyIap(string kind, string packageName, string skuId, string purchaseToken, Action afterVerify)
    {
        WWWForm form = new WWWForm();
        form.AddField("kind", kind);
        form.AddField("package_name", packageName);
        form.AddField("sku_id", skuId);
        form.AddField("purchase_token", purchaseToken);
        StartCoroutine(IeVerify(form, afterVerify));
    }

    [SerializeField]
    private string verifyUrl = "https://us-central1-sonat-arm-358507.cloudfunctions.net/verify_inapp_purchase";

    public class ForceAcceptAll : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    private IEnumerator IeVerify(WWWForm form, Action afterVerify)
    {
        yield return new WaitForSeconds(30);
        using (UnityWebRequest www = UnityWebRequest.Post(verifyUrl, form))
        {
            var cert2 = new ForceAcceptAll();
            www.certificateHandler = cert2;
            yield return www.SendWebRequest();
            UIDebugLog.Log(www.responseCode.ToString());
            if (www.responseCode == 200)
            {
                var respond = JsonUtility.FromJson<RespondVerifyIap>(www.downloadHandler.text);
                if (respond == null || respond.errorCode != 0)
                {
                    UIDebugLog.LogError("not valid");
                }
                else
                {
                    afterVerify.Invoke();
                }
            }
            else
                afterVerify.Invoke();

            if (www.result != UnityWebRequest.Result.Success)
            {
                UIDebugLog.LogError(www.error);
            }
            else
            {
                UIDebugLog.Log(www.downloadHandler.text);
            }
        }
    }

    public void OnBuySuccessHandler(int id)
    {
        //DataManager.OnBuySuccess(id);
        onInAppPurchased.Action.Invoke(id);
        Debug.Log("OnBuySuccessHandler ShopItemKey:" + id);
    }


    public bool IsInitialized()
    {
        return _mStoreController != null && _mStoreExtensionProvider != null;
    }

    public Action onInitializedSuccess;

    public virtual void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _mStoreController = controller;
        _mStoreExtensionProvider = extensions;
        Debug.Log("Purchaser OnInitialized successfully");
        Initialized = true;

        if (autoRestore)
            RestorePurchase(true);

        onInitializedSuccess?.Invoke();
        onInitializedSuccess = null;

        Debug.Log("Available items:");
        foreach (var item in controller.products.all)
        {
            if (item.availableToPurchase)
            {
                Debug.Log(string.Join(" - ", item.metadata.localizedTitle, item.metadata.localizedDescription,
                    item.metadata.isoCurrencyCode, item.metadata.localizedPrice.ToString(CultureInfo.InvariantCulture), Decimal.ToDouble(item.metadata.localizedPrice).ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                    item.metadata.localizedPriceString, item.transactionID, item.receipt));
#if INTERCEPT_PROMOTIONAL_PURCHASES
                // Set all these products to be visible in the user's App Store according to Apple's Promotional IAP feature
                // https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/StoreKitGuide/PromotingIn-AppPurchases/PromotingIn-AppPurchases.html
                m_AppleExtensions.SetStorePromotionVisibility(item, AppleStorePromotionVisibility.Show);
#endif

#if SUBSCRIPTION_MANAGER
                // this is the usage of SubscriptionManager class
                if (item.receipt != null)
                {
                    if (item.definition.type == ProductType.Subscription)
                    {
                        if (checkIfProductIsAvailableForSubscriptionManager(item.receipt))
                        {
                            string intro_json =
                                (introductory_info_dict == null ||
                                 !introductory_info_dict.ContainsKey(item.definition.storeSpecificId))
                                    ? null
                                    : introductory_info_dict[item.definition.storeSpecificId];
                            SubscriptionManager p = new SubscriptionManager(item, intro_json);
                            SubscriptionInfo info = p.getSubscriptionInfo();
                            Debug.Log("product id is: " + info.getProductId());
                            Debug.Log("purchase date is: " + info.getPurchaseDate());
                            Debug.Log("subscription next billing date is: " + info.getExpireDate());
                            Debug.Log("is subscribed? " + info.isSubscribed().ToString());
                            Debug.Log("is expired? " + info.isExpired().ToString());
                            Debug.Log("is cancelled? " + info.isCancelled());
                            Debug.Log("product is in free trial peroid? " + info.isFreeTrial());
                            Debug.Log("product is auto renewing? " + info.isAutoRenewing());
                            Debug.Log("subscription remaining valid time until next billing date is: " +
                                      info.getRemainingTime());
                            Debug.Log("is this product in introductory price period? " +
                                      info.isIntroductoryPricePeriod());
                            Debug.Log("the product introductory localized price is: " + info.getIntroductoryPrice());
                            Debug.Log("the product introductory price period is: " + info.getIntroductoryPricePeriod());
                            Debug.Log("the number of product introductory price period cycles is: " +
                                      info.getIntroductoryPricePeriodCycles());
                        }
                        else
                        {
                            Debug.Log(
                                "This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                        }
                    }
                    else
                    {
                        Debug.Log("the product is not a subscription product");
                    }
                }
                else
                {
                    Debug.Log("the product should have a valid receipt");
                }
#endif
            }
        }

        OnInitializedSuccessfully();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log("OnPurchaseFailed " + product.transactionID + "/" + failureDescription.message);

        var specificId = product.definition.storeSpecificId;
        if (_dictionary.ContainsKey(specificId))
        {
            BuyIapContent buyIapContent = _dictionary[specificId];
            _dictionary.Remove(specificId);
            if (failureDescription.reason == PurchaseFailureReason.UserCancelled)
            {
                var item = StoreProductDescriptors.Find(x => x.StoreProductId == specificId);
                new SonatLogCancelShopItem()
                {
                    item_id = item.StoreProductId,
                    item_type = buyIapContent.Log.item_type,
                    placement = buyIapContent.Log.placement,
                    location = buyIapContent.Log.location,
                    screen = buyIapContent.Log.screen,
                    level = Kernel.ParameterValueRetriever.LevelDisplay,
                }.Post();
            }

            buyIapContent.OnSuccess.Invoke(false);
            
        }

        _isBuy = false;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("OnPurchaseFailed " + product.transactionID);

        _isBuy = false;
    }

    protected virtual void OnInitializedSuccessfully()
    {
    }


#if SUBSCRIPTION_MANAGER
    private bool checkIfProductIsAvailableForSubscriptionManager(string receipt)
    {
        var receipt_wrapper = (Dictionary<string, object>) MiniJson.JsonDecode(receipt);
        if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
        {
            Debug.Log("The product receipt does not contain enough information");
            return false;
        }

        var store = (string) receipt_wrapper["Store"];
        var payload = (string) receipt_wrapper["Payload"];

        if (payload != null)
        {
            switch (store)
            {
                case GooglePlay.Name:
                {
                    var payload_wrapper = (Dictionary<string, object>) MiniJson.JsonDecode(payload);
                    if (!payload_wrapper.ContainsKey("json"))
                    {
                        Debug.Log(
                            "The product receipt does not contain enough information, the 'json' field is missing");
                        return false;
                    }

                    var original_json_payload_wrapper =
                        (Dictionary<string, object>) MiniJson.JsonDecode((string) payload_wrapper["json"]);
                    if (original_json_payload_wrapper == null ||
                        !original_json_payload_wrapper.ContainsKey("developerPayload"))
                    {
                        Debug.Log(
                            "The product receipt does not contain enough information, the 'developerPayload' field is missing");
                        return false;
                    }

                    var developerPayloadJSON = (string) original_json_payload_wrapper["developerPayload"];
                    var developerPayload_wrapper =
                        (Dictionary<string, object>) MiniJson.JsonDecode(developerPayloadJSON);
                    if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") ||
                        !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                    {
                        Debug.Log(
                            "The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                        return false;
                    }

                    return true;
                }
                case AppleAppStore.Name:
                case AmazonApps.Name:
                case MacAppStore.Name:
                {
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        return false;
    }
#endif


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        UIDebugLog.Log("Purchaser OnInitialized failed " + error);
        Initialized = true;
        if (tried < 5)
        {
            tried++;
            StartCoroutine(IeWaitToReload());
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        UIDebugLog.Log("Purchaser OnInitialized failed " + error);
        UIDebugLog.Log("Purchaser OnInitialized failed " + message);
        Initialized = true;

        if (tried < 5)
        {
            tried++;
            StartCoroutine(IeWaitToReload());
        }
    }

    private IEnumerator IeWaitToReload()
    {
        yield return new WaitForSeconds(20);
        while (!Kernel.IsInternetConnection())
        {
            yield return new WaitForSeconds(20);
        }

        if (!IsInitialized())
            InitializePurchasing();
    }

    private int tried;

    void BuyProductId(string productId)
    {
        if (IsInitialized())
        {
            Debug.Log($" IsInitialized {IsInitialized()} BuyProductId:{productId}");
            var product = _mStoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                _mStoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.LogError($"Not available:{productId}");
            }
        }
        else
        {
            Debug.LogError($"Not IsInitialized {IsInitialized()} BuyProductId:{productId}");
        }
    }


    [ContextMenu("Test")]
    void Test()
    {
        Debug.Log(MoveAllSc("₫200,000"));
    }

    private static String MoveAllSc(String str)
    {
        // Take length of string 
        int len = str.Length;

        // regular expression to check  
        // char is special or not. 
        var regx = new Regex("[a-zA-Z0-9\\.\\,\\s+]");

        // traverse string 
        String res1 = "", res2 = "";
        for (int i = 0; i < len; i++)
        {
            char c = str[i];

            // check char at index i is a special char 
            if (regx.IsMatch(c.ToString()))
                res1 = res1 + c;
            else
                res2 = res2 + c;
        }

        return res1 + res2;
    }

    public object GetPriceTextSaleOriginal(int shopKey, float saleRatio)
    {
        var item = StoreProductDescriptors.Find(x => x.key == shopKey);
        //#if UNITY_EDITOR
        return "$" + (item.price / saleRatio);
        //#endif
        //if (_mStoreController != null)
        //{
        //    var product = _mStoreController.products.WithID(item.storeProductId);
        //    if (_currencyLast.Contains(product.metadata.isoCurrencyCode))
        //        return MoveAllSc(product.metadata.localizedPriceString);
        //    return product.metadata.localizedPriceString;
        //}

        //Debug.Log("final price = " + item.price + ", original price = " + item.price / saleRatio);
        //return "$" + (item.price / saleRatio);
    }

    public bool IsSubcribed(string storeProductId, out int expiredIn)
    {
        var subscriptionProduct = _mStoreController.products.WithID(storeProductId);
        Debug.Log("subscriptionProduct " + subscriptionProduct);
        expiredIn = 0;
        try
        {
            if (subscriptionProduct == null) return false;
            var isSubscribed = IsSubscribedTo(subscriptionProduct, out SubscriptionInfo subscriptionInfo);
            if (isSubscribed)
                expiredIn = SdkExtenstions.GetEpochDate(subscriptionInfo.getExpireDate()) -
                            SdkExtenstions.GetEpochDate();
            return isSubscribed;
        }
        catch (StoreSubscriptionInfoNotSupportedException)
        {
            return false;
        }

        return false;
    }

    bool IsSubscribedTo(UnityEngine.Purchasing.Product subscription, out SubscriptionInfo subscriptionInfo)
    {
        subscriptionInfo = null;
        Debug.Log("subscription" + subscription);
        // If the product doesn't have a receipt, then it wasn't purchased and the user is therefore not subscribed.
        if (subscription.receipt == null)
        {
            return false;
        }

        //The intro_json parameter is optional and is only used for the App Store to get introductory information.
        var subscriptionManager = new SubscriptionManager(subscription, null);
        Debug.Log("subscriptionManager" + subscriptionManager);

        // The SubscriptionInfo contains all of the information about the subscription.
        // Find out more: https://docs.unity3d.com/Packages/com.unity.purchasing@3.1/manual/UnityIAPSubscriptionProducts.html

        subscriptionInfo = subscriptionManager.getSubscriptionInfo();
        Debug.Log("subscriptionInfo" + subscriptionInfo);

        return subscriptionInfo.isSubscribed() == Result.True;
    }


    public bool PaidUser()
	{
        if (_listPurchased == null)
            _listPurchased = new PlayerPrefListInt("item_purchased", new List<int>());
        return _listPurchased.Count > 0;
    }

    public List<int> GetListPurchased()
	{
        if (_listPurchased == null)
            _listPurchased = new PlayerPrefListInt("item_purchased", new List<int>());
        return _listPurchased.Current;
    }

    public bool CheckPackBought(int id)
    {
        if (_listPurchased == null)
            _listPurchased = new PlayerPrefListInt("item_purchased", new List<int>());
        return _listPurchased.Current != null && _listPurchased.Current.FindIndex(e => e == id) >= 0;
    }

    public void RestorePurchasesIOS(Action onSuccess)
    {
#if UNITY_IOS
        if (!IsInitialized())
        {
            return;
        }

        var apple = _mStoreExtensionProvider.GetExtension<IAppleExtensions>();

        if (_onSuccess.Value != null)
        {
            _onSuccess = new KeyValuePair<int, Action<bool>>(0, null);
        }

        apple.RestoreTransactions((result, errorMsg) =>
        {
            Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");

            if (!result)
                Debug.Log("Restore error: " + errorMsg);
            else
            {
                RestorePurchase();
                onSuccess?.Invoke();
            }
        });
#endif
    }
}
#endif