#define dummy
//#define use_facebook

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (dummy || global_dummy) && !use_facebook
public class FacebookController : BaseService
{
    protected override void Register()
    {
        base.Register();
        Initialized = true;
    }
}
#else
using Facebook.Unity;

public class FacebookController : BaseService
{
    [SerializeField] private bool login;

    // Include Facebook namespace
    public Action<string> loggedReadyCallback;
    public Action<string> onLoggingCallback;
    public Action loggedOutCallback;

    protected override void Register()
    {
        base.Register();
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
            Initialized = true;
            if(login)
                CheckPlayFab();
        }
    }

    public void CheckPlayFab()
    {
        if (FB.IsLoggedIn)
            loggedReadyCallback.Invoke(AccessToken.CurrentAccessToken.TokenString);
        else
            if(loggedReadyCallback != null)
                loggedReadyCallback.Invoke(string.Empty);
    }


    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            Initialized = true;
            CheckPlayFab();
        }
        else
        {
            if(loggedReadyCallback != null)
                loggedReadyCallback.Invoke(string.Empty);
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public Action<bool> loggedEvent;

    public void LoginWithFacebook()
    {
        Debug.Log(nameof(LoginWithFacebook));
        AdsManager.AppLeaving = 10;

//        AdsManager.AppLeaving = true;
        var perms = new List<string>() {"public_profile"};
//        var perms = new List<string>() {"public_profile", "email"};
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    public void LogOut()
    {
        Debug.Log(nameof(LogOut));
        if (FB.IsLoggedIn)
        {
            FB.LogOut();
            StopCoroutine("CheckForSuccussfulLogout");
            StartCoroutine("CheckForSuccussfulLogout");
        }
    }

    IEnumerator CheckForSuccussfulLogout()
    {
        while (FB.IsLoggedIn)
        {
            yield return new WaitForSeconds(0.1f);
        }
        loggedEvent?.Invoke(false);
        loggedOutCallback.Invoke();
    }

    private ILoginResult _result;

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            loggedEvent?.Invoke(true);
            _result = result;
            // AccessToken class will have session details
            var aToken = AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);

            UIDebugLog.Log("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString +
                           "\nLogging into PlayFab...");

            onLoggingCallback.Invoke(AccessToken.CurrentAccessToken.TokenString);
        }
        else
        {
            UIDebugLog.Log("User cancelled login");
        }
    }
}
#endif