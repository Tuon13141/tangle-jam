using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;
using Percas.IAP;

public class AppsFlyerManager : MonoBehaviour
{
    private void Awake()
    {
        TrackingManager.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        TrackingManager.OnAdMobRevenueEvent += OnAdMobRevenueEvent;
        TrackingManager.OnPurchaseCompleted += OnPurchaseRevenueEvent;

        AppsFlyer.setCustomerUserId(SystemInfo.deviceUniqueIdentifier);
    }

    private void OnDestroy()
    {
        TrackingManager.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent;
        TrackingManager.OnAdMobRevenueEvent -= OnAdMobRevenueEvent;
        TrackingManager.OnPurchaseCompleted -= OnPurchaseRevenueEvent;
    }

    private void OnAdRevenuePaidEvent(MaxSdkBase.AdInfo adInfo)
    {
        double revenue = adInfo.Revenue;
        Dictionary<string, string> additionalParams = new()
        {
            { AdRevenueScheme.COUNTRY, MaxSdk.GetSdkConfiguration().CountryCode },
            { AdRevenueScheme.AD_UNIT, adInfo.AdUnitIdentifier },
            { AdRevenueScheme.AD_TYPE, adInfo.AdFormat },
            { AdRevenueScheme.PLACEMENT, adInfo.Placement }
        };
        var logRevenue = new AFAdRevenueData(adInfo.NetworkName, MediationNetwork.ApplovinMax, "USD", revenue);
        AppsFlyer.logAdRevenue(logRevenue, additionalParams);
    }

    private void OnAdMobRevenueEvent(double value, string adNetwork, string adUnit, string adFormat)
    {
        double revenue = value;
        Dictionary<string, string> additionalParams = new()
        {
            { AdRevenueScheme.COUNTRY, MaxSdk.GetSdkConfiguration().CountryCode },
        };
        var logRevenue = new AFAdRevenueData("MonetizationGoogleAdMob", MediationNetwork.GoogleAdMob, "USD", revenue);
        AppsFlyer.logAdRevenue(logRevenue, additionalParams);
    }

    private void OnPurchaseRevenueEvent(IAPPack pack)
    {
        Dictionary<string, string> eventValues = new();
        decimal revenue = (decimal)pack.productPackPriceInUSD;
        eventValues.Add(AFInAppEvents.CURRENCY, "USD");
        eventValues.Add(AFInAppEvents.REVENUE, GetRecordedRevenue(revenue));
        eventValues.Add("af_quantity", "1");
        AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, eventValues);
    }

    private string GetRecordedRevenue(decimal amount)
    {
        decimal val = decimal.Multiply(amount, 0.63m);
        return val.ToString();
    }
}
