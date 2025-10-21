using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if use_admob
using GoogleMobileAds.Ump;
using GoogleMobileAds.Ump.Api;

#endif

public class GDPRScript : MonoBehaviour
{
#if use_admob
    ConsentForm _consentForm;

    // Start is called before the first frame update
    void Start()
    {
        var debugSettings = new ConsentDebugSettings
        {
            // Geography appears as in EEA for debug devices.
            DebugGeography = DebugGeography.EEA,
            TestDeviceHashedIds = new List<string>
            {
                "257361E8D32009A3D78044F9A0E9B843"
            }
        };

        // Here false means users are not under age.
        ConsentRequestParameters request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
            ConsentDebugSettings = debugSettings,
        };

        // Check the current consent information status.
        Debug.Log($" ConsentInformation.Update");
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    void OnConsentInfoUpdated(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError("duong " + error.Message);
            return;
        }

        Debug.Log($"duong ConsentInformation.IsConsentFormAvailable(){ConsentInformation.IsConsentFormAvailable()}");
        if (ConsentInformation.IsConsentFormAvailable())
        {
            LoadConsentForm();
        }

        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
    }

    void LoadConsentForm()
    {
        // Loads a consent form.
        ConsentForm.Load(OnLoadConsentForm);
    }

    void OnLoadConsentForm(ConsentForm consentForm, FormError error)
    {
        if (error != null)
        {
            // Handle the error.
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
        }
    }


    void OnShowForm(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        AdsManager.FakeLastTimeShowBanner();
        // Handle dismissal by reloading form.
        LoadConsentForm();
    }
#endif
}