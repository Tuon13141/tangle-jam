#if use_admob
using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Ump.Api;


public class AdmobConsentHelper {

    public static string GetDeviceID()
    {
        try
        {
            // Get Android ID
            AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");

            string android_id = clsSecure.CallStatic<string>("getString", objResolver, "android_id");

            // Get bytes of Android ID
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(android_id);

            // Encrypt bytes with md5
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            string device_id = hashString.PadLeft(32, '0');

            return device_id.ToUpper();

        } catch(Exception e)
        {
            return "";
        }
    }

    public static void Acquired()
    {
        if (sharedPreference == null)
        {
            AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaClass clsPreferenceManager;
            try
            {
                clsPreferenceManager = new AndroidJavaClass("androidx.preference.PreferenceManager");

            }
            catch (Exception e)
            {
                clsPreferenceManager = new AndroidJavaClass("android.preference.PreferenceManager");
            }

            if (clsPreferenceManager != null)
            {
                AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
                sharedPreference = clsPreferenceManager.CallStatic<AndroidJavaObject>("getDefaultSharedPreferences", objActivity);
            }
        }
    }

    public static bool IsGDPR()
    {
        var gdpr = GetInt(IABTCF_GDPR_APPLIES, 0);
        return gdpr == 1;
    }

    public static String getConsentInfo()
    {
        var purposeConsent = GetString(IABTCF_PURPOSE_CONSENTS, "");
        var vendorConsent = GetString(IABTCF_VENDOR_CONSENTS, "");
        var vendorLI = GetString(IABTCF_VENDOR_LEGITIMATE_INTERESTS, "");
        var purposeLI = GetString(IABTCF_PURPOSE_LEGITIMATE_INTERESTS, "");

        return "";
    }

    public static bool CanShowAds() {

        //https://github.com/InteractiveAdvertisingBureau/GDPR-Transparency-and-Consent-Framework/blob/master/TCFv2/IAB%20Tech%20Lab%20-%20CMP%20API%20v2.md#in-app-details
        //https://support.google.com/admob/answer/9760862?hl=en&ref_topic=9756841

        var purposeConsent = GetString(IABTCF_PURPOSE_CONSENTS, "");
        var vendorConsent = GetString(IABTCF_VENDOR_CONSENTS, "");
        var vendorLI = GetString(IABTCF_VENDOR_LEGITIMATE_INTERESTS, "");
        var purposeLI = GetString(IABTCF_PURPOSE_LEGITIMATE_INTERESTS, "");

        var hasGoogleVendorConsent = HasAttribute(vendorConsent, GOOGLE_ID);
        var hasGoogleVendorLI = HasAttribute(vendorLI, GOOGLE_ID);

        // Minimum required for at least non-personalized ads
        return HasConsentFor(new int[] { DataUsagePurpose.storeAndAccessInformationOnADevice }, purposeConsent, hasGoogleVendorConsent)
                && HasConsentOrLegitimateInterestFor(
                    new int[] {
                        DataUsagePurpose.selectBasicAds,
                        DataUsagePurpose.measureAdPerformance,
                        DataUsagePurpose.applyMarketResearchToGenerateAudienceInsights,
                        DataUsagePurpose.developAndImproveProducts
                    }, 
                    purposeConsent,
                    purposeLI,
                    hasGoogleVendorConsent,
                    hasGoogleVendorLI
                );

    }

    public static bool CanShowPersonalizedAds() {

        //https://github.com/InteractiveAdvertisingBureau/GDPR-Transparency-and-Consent-Framework/blob/master/TCFv2/IAB%20Tech%20Lab%20-%20CMP%20API%20v2.md#in-app-details
        //https://support.google.com/admob/answer/9760862?hl=en&ref_topic=9756841

        var purposeConsent = GetString(IABTCF_PURPOSE_CONSENTS, "");
        var vendorConsent = GetString(IABTCF_VENDOR_CONSENTS, "");
        var vendorLI = GetString(IABTCF_VENDOR_LEGITIMATE_INTERESTS, "");
        var purposeLI = GetString(IABTCF_PURPOSE_LEGITIMATE_INTERESTS, "");

        var hasGoogleVendorConsent = HasAttribute(vendorConsent, GOOGLE_ID);
        var hasGoogleVendorLI = HasAttribute(vendorLI, GOOGLE_ID);

        return HasConsentFor(
                    new int[] { 
                        DataUsagePurpose.storeAndAccessInformationOnADevice,
                        DataUsagePurpose.createAPersonalisedAdsProfile,
                        DataUsagePurpose.selectPersonalisedAds
                    },
                    purposeConsent,
                    hasGoogleVendorConsent
                )
                && HasConsentOrLegitimateInterestFor(
                    new int[] {
                        DataUsagePurpose.selectBasicAds,
                        DataUsagePurpose.measureAdPerformance,
                        DataUsagePurpose.applyMarketResearchToGenerateAudienceInsights,
                        DataUsagePurpose.developAndImproveProducts
                    },
                    purposeConsent,
                    purposeLI,
                    hasGoogleVendorConsent,
                    hasGoogleVendorLI
                );
    }

    public static void Reset()
    {
        ConsentInformation.Reset();
    }


    private static String GetString(String key, String defaultValue)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Acquired();
            if (sharedPreference != null)
            {
                return sharedPreference.Call<String>("getString", key, defaultValue);
            } else
            {
                return defaultValue;
            }
        } else
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
        
    }

    private static object GetAll()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Acquired();
            if (sharedPreference != null)
            {
                return sharedPreference.Call<object>("getAll");
            }
            else
            {
                return new Dictionary<String, object>();
            }
        }
        else
        {
            return new Dictionary<String, object>();
        }

    }

    private static int GetInt(String key, int defaultValue)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Acquired();
            if (sharedPreference != null)
            {
                return sharedPreference.Call<int>("getInt", key, defaultValue);
            }
            else
            {
                return defaultValue;
            }
        }
        else
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
    }

    // Check if a binary string has a "1" at position "index" (1-based)    
    private static bool HasAttribute(String input, int index)
    {
        return input.Length >= index && input[index - 1] == '1';
    }

    // Check if consent is given for a list of purposes
    private static bool HasConsentFor(int[] purposes, String purposeConsent, bool hasVendorConsent)
    {
        return purposes.All(i => HasAttribute(purposeConsent, i)) && hasVendorConsent;
    }

    // Check if a vendor either has consent or legitimate interest for a list of purposes
    private static bool HasConsentOrLegitimateInterestFor(int[] purposes, String purposeConsent, String purposeLI, bool hasVendorConsent, bool hasVendorLI)
    {
        return purposes.All(i => (HasAttribute(purposeLI, i) && hasVendorLI) || (HasAttribute(purposeConsent, i) && hasVendorConsent));
    }

    private static readonly int GOOGLE_ID = 755;
    private static readonly String IABTCF_PURPOSE_CONSENTS = "IABTCF_PurposeConsents";
    private static readonly String IABTCF_VENDOR_CONSENTS = "IABTCF_VendorConsents";
    private static readonly String IABTCF_VENDOR_LEGITIMATE_INTERESTS = "IABTCF_VendorLegitimateInterests";
    private static readonly String IABTCF_PURPOSE_LEGITIMATE_INTERESTS = "IABTCF_PurposeLegitimateInterests";
    private static readonly String IABTCF_GDPR_APPLIES = "IABTCF_gdprApplies";

    private static AndroidJavaObject sharedPreference;

    public class DataUsagePurpose
    {
        public static readonly int storeAndAccessInformationOnADevice = 1;
        public static readonly int selectBasicAds = 2;
        public static readonly int createAPersonalisedAdsProfile = 3;
        public static readonly int selectPersonalisedAds = 4;
        public static readonly int createAPersonalisedContentProfile = 5;
        public static readonly int selectPersonalisedContent = 6;
        public static readonly int measureAdPerformance = 7;
        public static readonly int measureContentPerformance = 8;
        public static readonly int applyMarketResearchToGenerateAudienceInsights = 9;
        public static readonly int developAndImproveProducts = 10;
    }


}
#endif