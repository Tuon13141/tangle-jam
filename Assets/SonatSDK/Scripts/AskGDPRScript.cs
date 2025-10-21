using System;
using System.Collections;
using System.Collections.Generic;
using sonat_sdk.Scripts;
#if use_admob
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
#endif
using Sonat;
using UnityEngine;

public class AskGDPRScript : AskScriptBase
{
    [SerializeField] private bool showAttAfter;
    [SerializeField] private bool canStartIfGDPRLoadFail;
    #if use_admob
    public string[] testDeviceIds = new[] {"B15BB24370A69E2E02CE1EA2F2E5BD10"};
    private static bool ShouldLoad => true;

    public override IEnumerator IeAsk()
    {
        if (PlayerPrefs.GetInt(PlayerPrefEnum.cache_consent_admob.ToString()) == 1)
        {
           
            ConsentReady = true;
            OnConsent?.Invoke();
            AdsManager.AskAtt();
            yield break;
        }

        bool load = RemoteConfigKey.gdpr_force.GetValueBoolean() ||
                    ShouldLoad && !RemoteConfigKey.gdpr_ignore.GetValueBoolean();
        if (!load)
        {
            ConsentReady = true;
            OnConsent?.Invoke();
            if (showAttAfter)
                AdsManager.AskAtt();
            yield break;
        }

        Debug.Log("RemoteConfigKey.gdpr_force.GetValueBoolean()" + RemoteConfigKey.gdpr_force.GetValueBoolean());

        AdsManager.WaitAtt = true;
        LoadGdpr();
        while (!ConsentReady)
            yield return null;

        OnConsent?.Invoke();
        AdsManager.WaitAtt = false;
        Debug.Log("AdsManager.WaitAtt = false");

    }

    public override bool Ready => ConsentReady;
    public bool ConsentReady { get; set; }

    public void CallForm()
    {
        LoadGdpr();
    }

    private bool LoadGdpr()
    {
        AdsManager.GDPRRequested = true;
        if (PlayerPrefs.GetInt(PlayerPrefEnum.cache_consent_admob.ToString()) == 1)
        {
            UIDebugLog.Log0($" Consent cache_consent_admob cached",true);
            ConsentReady = true;
            if (showAttAfter)
                AdsManager.AskAtt();
            return ConsentReady;
        }

        List<String> deviceIds = new List<String>()
            {AdRequest.TestDeviceSimulator};
        //        deviceIds.Add("B15BB24370A69E2E02CE1EA2F2E5BD10");
        foreach (var testAdsId in testDeviceIds)
            deviceIds.Add(testAdsId);

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefEnum.test_device_id.ToString())))
            deviceIds.Add(PlayerPrefs.GetString(PlayerPrefEnum.test_device_id.ToString()));

        var debugSettings = new ConsentDebugSettings
        {
            // Geography appears as in EEA for debug devices.
            DebugGeography = DebugGeography.EEA,
            TestDeviceHashedIds = deviceIds
        };

        // Here false means users are not under age.
        ConsentRequestParameters request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
            ConsentDebugSettings = debugSettings,
        };

        // Check the current consent information status.
        UIDebugLog.Log($" ConsentInformation.Update",true);
        ConsentInformation.Update(request, OnConsentInfoUpdated);
        OnUpdateStatus?.Invoke(ConsentInformation.CanRequestAds());
        return false;
    }

    void OnConsentInfoUpdated(FormError error)
    {
        UIDebugLog.Log($"duong OnConsentInfoUpdated IsConsentFormAvailable:{ConsentInformation.IsConsentFormAvailable()} error{error}",true);
        if (!ConsentInformation.IsConsentFormAvailable())
        {
            ConsentReady = true;
            if (showAttAfter)
                AdsManager.AskAtt();
        }

        if (error != null)
        {
            // Handle the error.
            UIDebugLog.Log($"duong OnConsentInfoUpdated FormError:{error.Message}",true);
            return;
        }

        if (ConsentInformation.IsConsentFormAvailable())
        {
            UIDebugLog.Log($"duong OnConsentInfoUpdated IsConsentFormAvailable:{ConsentInformation.IsConsentFormAvailable()}",true);
            ConsentForm.Load(OnLoadConsentForm);
        }

        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
    }

    ConsentForm _consentForm;

    private bool consentFlag;

    void OnLoadConsentForm(ConsentForm consentForm, FormError error)
    {
        UIDebugLog.Log0($"duong OnLoadConsentForm ConsentInformation.ConsentStatus{ConsentInformation.ConsentStatus}",true);
        if (
            ConsentInformation.ConsentStatus == ConsentStatus.Obtained
//            || ConsentInformation.ConsentStatus == ConsentStatus.NotRequired
        )
        {
            if (ConsentInformation.ConsentStatus == ConsentStatus.Obtained && consentFlag)
            {
                PlayerPrefs.SetInt(PlayerPrefEnum.cache_consent_admob.ToString(), 1);
                new CustomSonatLog("confirm_consent", new List<Sonat.LogParameter>()).Post(false, 2);
                new CustomSonatLog(
                    AdmobConsentHelper.CanShowAds()
                        ? sonat_log_enum.cmp_user_consent
                        : sonat_log_enum.cmp_user_do_not_consent, new List<Sonat.LogParameter>()).Post(false, 2);

            

                Kernel.Resolve<FireBaseController>().SetUserProperty("consent_status",
                    AdmobConsentHelper.CanShowAds() ? " consent" : "do_not_consent");
                if (Kernel.kernel != null)
                {
                    Kernel.kernel.CheckConsent();
                }
            }
        }

        if (ConsentInformation.ConsentStatus == ConsentStatus.Obtained
            || ConsentInformation.ConsentStatus == ConsentStatus.NotRequired
            || ConsentInformation.ConsentStatus == ConsentStatus.Unknown)
        {
            ConsentReady = true;
            if (showAttAfter)
                AdsManager.AskAtt();
        }

        if (error != null)
        {
            // Handle the error.
            if (canStartIfGDPRLoadFail) ConsentReady = true;
            UnityEngine.Debug.LogError(error);
            return;
        }

        // The consent form was loaded.
        // Save the consent form for future requests.
        _consentForm = consentForm;

        // You are now ready to show the form.
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {
            _consentForm.Show(OnShowForm);
            new CustomSonatLog(sonat_log_enum.cmp_impression, null).Post(false, 2);
            consentFlag = true;
        }
    }


    private void OnShowForm(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }
        ConsentReady = true;
        OnUpdateStatus?.Invoke(ConsentInformation.CanRequestAds());
        AdsManager.FakeLastTimeShowBanner();
        // Handle dismissal by reloading form.
        ConsentForm.Load(OnLoadConsentForm);
    }
    #else
    public override IEnumerator IeAsk()
    {
        yield return null;
    }

    public override bool Ready => true;
    
    #endif
}