#define dummy
//#define use_appflyer

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sonat;
#if !((dummy || global_dummy) && !use_appflyer)
using Firebase.Analytics;
using AppsFlyerSDK;
//#if use_af_purchase_connector
using AppsFlyerConnector;
//#endif
using Firebase.Crashlytics;
#endif
using UnityEngine;
using UnityEngine.Serialization;

#if (dummy || global_dummy) && !use_appflyer
public class AppFlyerController : BaseService
{
    public static bool FirebaseReady { get; set; }
    public string devKey;
    public string appID_IOS;
    public bool isDebug;
    public bool getConversionData;
    //public bool splitCampaignName;
    
    protected override void Register()
    {
        base.Register();
        Initialized = true;
    }


    public void SendEvent(string eventName, Dictionary<string, string> eventValues)
    {
    }
    
    public void SendEvent(string eventName)
    {
    }

    public void LogAppsFlyerAdRevenue(string interstitialAdapter, double adValueValue, string banner)
    {
        
    }

    public bool purchaseConnector = true;
        public static string GetUserId()
	{
        return "";
    }
}

#else

public class AppFlyerController : BaseService, IAppsFlyerConversionData
{
    //public bool purchaseConnector = true;

    public class Util
    {
        private static readonly string AUTO_ID_ALPHABET =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private static readonly int AUTO_ID_LENGTH = 24;
        private static readonly System.Random rand = new System.Random();

        public static string generateSonatARMUserID()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds();

            return unixTimeMilliseconds.ToString() + "-" + _AutoId();
        }


        private static string _AutoId()
        {
            StringBuilder builder = new StringBuilder();
            int maxRandom = AUTO_ID_ALPHABET.Length;
            for (int i = 0; i < AUTO_ID_LENGTH; i++)
            {
                builder.Append(AUTO_ID_ALPHABET[rand.Next(maxRandom)]);
            }

            return builder.ToString();
        }
    }

    public static string GetUserId()
	{
        return AppsFlyer.getAppsFlyerId();
    }


    public string devKey = "Ry3zd9x7iEihJdcAdRb7PX";
    public string appID_GP;
    public string appID_IOS;
    public string UWPAppID;
    public bool isDebug;
    //public bool splitCampaignName;
    
    public bool getConversionData => true;
    //******************************//

    public static string CustomerUserId
    {
        get
        {
            if (!PlayerPrefs.HasKey(nameof(CustomerUserId)))
                PlayerPrefs.SetString(nameof(CustomerUserId), Util.generateSonatARMUserID());
            return PlayerPrefs.GetString(nameof(CustomerUserId));
        }
        set => PlayerPrefs.SetString(nameof(CustomerUserId), value);
    }

    public static bool ConversionLogged
    {
        get => PlayerPrefs.GetInt(nameof(ConversionLogged)) == 1;
        set => PlayerPrefs.SetInt(nameof(ConversionLogged), value ? 1 : 0);
    }

    public static bool Firebase
    {
        get => PlayerPrefs.GetInt(nameof(ConversionLogged)) == 1;
        set => PlayerPrefs.SetInt(nameof(ConversionLogged), value ? 1 : 0);
    }

    protected override void Register()
    {
        base.Register();
        AppsFlyer.setCustomerUserId(CustomerUserId);
        // These fields are set from the editor so do not modify!
        //******************************//
        AppsFlyer.setIsDebug(isDebug);
#if UNITY_WSA_10_0 && !UNITY_EDITOR
        AppsFlyer.initSDK(devKey.RemoveWhiteSpace(), UWPAppID, getConversionData ? this : null);
#else
#if UNITY_IOS
        AppsFlyer.initSDK(devKey.RemoveWhiteSpace(), appID_IOS.RemoveWhiteSpace(), getConversionData ? this : null);
#endif
#if UNITY_ANDROID
        AppsFlyer.initSDK(devKey.RemoveWhiteSpace(), appID_GP.RemoveWhiteSpace(), getConversionData ? this : null);
#endif
#endif
        //******************************/

//#if use_af_purchase_connector
        //if (purchaseConnector)
        //{
            AppsFlyerPurchaseConnector.init(this, AppsFlyerConnector.Store.GOOGLE);
//        AppsFlyerPurchaseConnector.setIsSandbox(true);
            AppsFlyerPurchaseConnector.setAutoLogPurchaseRevenue(AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsAutoRenewableSubscriptions
                , AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsInAppPurchases);
            AppsFlyerPurchaseConnector.setPurchaseRevenueValidationListeners(true);
            AppsFlyerPurchaseConnector.build();
            AppsFlyerPurchaseConnector.startObservingTransactions();
            UIDebugLog.Log($"duong AppsFlyerPurchaseConnector");
        //}
//#endif
        var gdprUserConsent = AppsFlyerConsent.ForGDPRUser(true, true);
        AppsFlyer.setConsentData(gdprUserConsent);
        AppsFlyer.startSDK();
#if appsflyer_6_15_or_older
        AppsFlyerAdRevenue.start();
#endif


        Invoke(nameof(Initialize), 2f);
        StartCoroutine(Wait2());
    }

    public void ReloadGdprConsent()
    {
        AppsFlyer.stopSDK(true);
        var gdprUserConsent = AppsFlyerConsent.ForGDPRUser(true, true);
        AppsFlyer.setConsentData(gdprUserConsent);
        AppsFlyer.startSDK();
    }

    public void didReceivePurchaseRevenueValidationInfo(string validationInfo)
    {
        UIDebugLog.Log($"duong didReceivePurchaseRevenueValidationInfo {validationInfo}");
        AppsFlyer.AFLog("didReceivePurchaseRevenueValidationInfo", validationInfo);
        // deserialize the string as a dictionnary, easy to manipulate
        Dictionary<string, object> dictionary =
            AFMiniJSON.Json.Deserialize(validationInfo) as Dictionary<string, object>;

        // if the platform is Android, you can create an object from the dictionnary 
        // deserialize the string as a dictionnary, easy to manipulate
        if (dictionary == null)
            return;
        // if the platform is Android, you can create an object from the dictionnary 
#if UNITY_ANDROID
        if (dictionary.ContainsKey("productPurchase") && dictionary["productPurchase"] != null)
        {
            // Create an object from the JSON string.
//            InAppPurchaseValidationResult iapObject = JsonUtility.FromJson<InAppPurchaseValidationResult>(validationInfo);
            if (dictionary.ContainsKey("success"))
                UIDebugLog.Log($"duong iapObject.success:{dictionary["success"]}");
            if (dictionary.ContainsKey("token"))
                UIDebugLog.Log($"duong iapObject.token:{dictionary["token"]}");
        }
        else if (dictionary.ContainsKey("subscriptionPurchase") && dictionary["subscriptionPurchase"] != null)
        {
            //  SubscriptionValidationResult iapObject = JsonUtility.FromJson<SubscriptionValidationResult>(validationInfo);
        }
#endif
    }

    void Initialize()
    {
        Initialized = true;
    }

    public void onConversionDataSuccess(string conversionData)
    {
        AppsFlyer.AFLog("didReceiveConversionData", conversionData);
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
        // if (!ConversionLogged)
        {
            Debug.Log("duong first conversionData: " + conversionData);
            StartCoroutine(Wait(conversionData));
        }

        // add deferred deeplink logic here
    }

    public string str1;
    public string str2;

    [ContextMenu("test1")]
    public void Test1()
    {
        onConversionDataSuccess(str1);
    }
    [ContextMenu("test2")]
    public void Test2()
    {
        onConversionDataSuccess(str2);
    }

    IEnumerator Wait2()
    {
        while (!SonatAnalyticTracker.FirebaseReady)
            yield return null;

        yield return new WaitForSeconds(3);
        FirebaseAnalytics.SetUserId(CustomerUserId);
        Crashlytics.SetUserId(CustomerUserId);
    }

    IEnumerator Wait(string conversionData)
    {
        while (!SonatAnalyticTracker.FirebaseReady)
            yield return null;

        yield return new WaitForSeconds(3);
//        if (!ConversionLogged)
        {
//            ConversionLogged = true;
            Debug.Log("duong: ConversionDataSuccess" + conversionData);
            var objDict = MiniJSON.Json.Deserialize(conversionData) as Dictionary<string, object>;

            if (objDict != null)
            {
                List<Parameter> parameters = new List<Parameter>();
                string campaignType = null;
                foreach (var keyValuePair in objDict)
                {
                    if (!(keyValuePair.Value == null || keyValuePair.Value.ToString().Length > 100))
                    {
                        var strValue = keyValuePair.Value.ToString();
                        parameters.Add(new Parameter(keyValuePair.Key, strValue, GetPrior(strValue)));
                    }
                    
                    if (keyValuePair is { Value: not null, Key: "campaign" })
                    {
                        string[] regex = keyValuePair.Value.ToString().Split(new [] {"_", "-", " - "}, StringSplitOptions.None);
                        if (regex.Length > 2)
                        {
                            parameters.Add(new Parameter("ad_network", regex[0], GetPrior("ad_network")));
                            parameters.Add(new Parameter("campaign_type", regex[^2], GetPrior("campaign_type")));
                            campaignType = regex[^2];
#if use_campaign_segment
                            UserSegment.SetUserSegment(regex[^2], keyValuePair.Value.ToString());
#endif
                        }
                    }
                }

                var logs = parameters.OrderBy(pr => pr.order).Take(25).ToArray();
                Debug.Log("duong: " + string.Join(",", logs.Select(x => x.log)));

                if (!ConversionLogged)
                {
                    FirebaseAnalytics.LogEvent("af_conversion_data", logs.Select(x => x.Param).ToArray());
                    ConversionLogged = true;
                }

                var last = JsonUtility.FromJson<DictStringString2>(
                    PlayerPrefs.GetString("last_conversation_set_user_property"));
                var setDict = GetListPropertySet(objDict);
                if (!string.IsNullOrEmpty(campaignType))
                {
                    setDict.Set("campaign_type", campaignType);
                }
                UIDebugLog.Log("duong setDict "+setDict.GetString());
                if(last != null)
                    UIDebugLog.Log("duong last = "+last.GetString());

                bool update = setDict.Count > 0;
                if (update && (last == null || last.Count == 0 ||  last.GetString() != setDict.GetString()))
                {
                    UIDebugLog.Log("duong ConversionDataSuccess set user property");
                    FirebaseAnalytics.LogEvent("sdk_tracking", new Firebase.Analytics.Parameter("data","af_conversion_data"), new Firebase.Analytics.Parameter("type","update"));
                    PlayerPrefs.SetString("last_conversation_set_user_property", JsonUtility.ToJson(setDict));
                    for (int i = 0; i < setDict.Count; i++)
                    {
                        FirebaseAnalytics.SetUserProperty(setDict.keys[i], setDict.values[i]);
                    }
                }
                else
                {
                    UIDebugLog.Log("duong no conversation data changed");
                }
            }
            else
            {
                Debug.LogError(this.name + " obj2 == null");
            }
        }

        DictStringString2 GetListPropertySet(Dictionary<string, object> dict)
        {
            DictStringString2 get = new DictStringString2();

            bool campaign_id = dict.ContainsKey("campaign_id");
            foreach (var keyValuePair in dict)
                if (_updateProperty.Contains(keyValuePair.Key))
                    get.Set(keyValuePair.Key, keyValuePair.Value == null ? "null" : keyValuePair.Value.ToString());
            if (!campaign_id)
                foreach (var keyValuePair in dict)
                    if (keyValuePair.Key == "af_c_id")
                        get.Set(keyValuePair.Key, keyValuePair.Value == null ? "null" : keyValuePair.Value.ToString());

            return get;
        }
    }


    private readonly string[] _updateProperty =
    {
        "media_source",
        "campaign_id",
        //  "af_c_id",
        "campaign",
        "af_adset_id",
        "af_adset",
        "af_ad_id",
        "af_ad",
        "af_channel",
        "af_cpi",
    };


    public class Parameter
    {
        public Firebase.Analytics.Parameter Param;
        public int order { get; private set; }

        public string log { get; private set; }

        public Parameter(string name, string value, int order = 0)
        {
            this.order = order;
            Param = new Firebase.Analytics.Parameter(name, value);
            log = name + "=" + value;
        }
    }

    private int GetPrior(string compare)
    {
        if (compare == "ad_network"
            || compare == "af_ad"
            || compare == "af_ad_format"
            || compare == "af_ad_id"
            || compare == "af_ad_type"
            || compare == "af_adset_id"
            || compare == "af_adset"
            || compare == "af_cpi"
            || compare == "af_status"
            || compare == "af_user_id"
            || compare == "campaign"
            || compare == "campaign_id"
            || compare == "campaign_type"
            || compare == "click_time"
            || compare == "install_time"
            || compare == "media_source"
            || compare == "dma")
            return 0;

        if (compare.StartsWith("af_sub")
            || compare.StartsWith("iscache")
            || compare.StartsWith("af_r")
            || compare.StartsWith("is_universal_link")
            || compare.StartsWith("af_click_lookback")
            || compare.StartsWith("is_incentivized")
        )
            return 1;

        if (compare == "clickid"
            || compare == "match_type"
            || compare == "is_branded_link"
            || compare == "af_r"
            || compare == "http_referrer "
        )
            return 2;
        return 3;
    }


    public void onConversionDataFail(string error)
    {
        AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
    }

    public void onAppOpenAttribution(string attributionData)
    {
        AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
        Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
        // add direct deeplink logic here
    }

    public void onAppOpenAttributionFailure(string error)
    {
        AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
    }


    public void SendEvent(string eventName, Dictionary<string, string> eventValues)
    {
        AppsFlyer.sendEvent(eventName, eventValues);
    }

    public void SendEvent(string eventName)
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        AppsFlyer.sendEvent(eventName, eventValues);
    }

//
//    public void LogAppsFlyerAdRevenue(string networkName, double revenue, string adType, string currencyCode = "USD",
//        AppsFlyerAdRevenueMediationNetworkType networkType = AppsFlyerAdRevenueMediationNetworkType
//            .AppsFlyerAdRevenueMediationNetworkTypeGoogleAdMob)
//    {
//        SonatAnalyticTracker.LogAppsFlyerAdRevenue(SonatTrackingHelper.GetNetworkName(adapter,platform), revenue, adType, currencyCode, networkType);
//    }
}


[System.Serializable]
public class DictStringString2
{
    public List<string> keys = new List<string>();
    public List<string> values = new List<string>();

    public string GetString()
    {
        string str = "";
        for (var i = 0; i < keys.Count; i++)
        {
            str += keys[i] + ":" + values[i];
            if (i < keys.Count - 1)
                str += ",";
        }

        return str;
    }

    public int Count => keys.Count;


    public virtual bool Exist(string key)
    {
        return keys.Contains(key);
    }

    public virtual string Get(string key)
    {
        for (var i = 0; i < keys.Count; i++)
        {
            if (keys[i] == key)
                return values[i];
        }

        Debug.LogError("not found");
        return string.Empty;
    }

    public virtual void Set(string key, string value)
    {
        for (var i = 0; i < keys.Count; i++)
        {
            if (keys[i] == key)
                values[i] = value;
        }

        keys.Add(key);
        values.Add(value);
    }

    public bool Equal(DictStringString2 other)
    {
        if (other.Count != Count)
            return false;
        for (var i = 0; i < other.keys.Count; i++)
        {
            if (!Exist(other.keys[i]))
                return false;
            if (Get(other.keys[i]) != other.values[i])
                return false;
        }

        return true;
    }
}


#endif