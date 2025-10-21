using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class LoadingTransition : MonoBehaviour
{
    public static LoadingTransition Instance;
    [SerializeField] private GameObject[] activeOnShows;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private PlayerPrefRemoteFloat delay_after_in_load_before_ads = new PlayerPrefRemoteFloat("delay_after_in_load_before_ads", 1f);
    [SerializeField] private PlayerPrefRemoteFloat delay_after_ads_to_action = new PlayerPrefRemoteFloat("delay_after_ads_to_action", .5f);
    private bool _register;
    [SerializeField] private bool fadeIn = true;
    [SerializeField] private bool fadeOut;
    [SerializeField] private GameObject loadingAds;
    [SerializeField] private GameObject loadingIap;
    [SerializeField] private bool showLoadingAds = true;
    [SerializeField] private bool showLoadingIap = true;

    private bool isShowing;

    public void CheckRegister()
    {
        if (!_register)
        {
            Register();
            _register = true;
        }
    }

    public void Register()
    {
        Instance = this;
        GetComponent<Canvas>().sortingOrder = 999;
        Hide();
        DontDestroyOnLoad(this);
    }

    private void Awake()
    {
        CheckRegister();
        if (loadingAds == null) loadingAds = transform.GetChild(0)?.gameObject;
    }

    public void Show(int index)
    {
        // var clamp = Mathf.Clamp(index, 0, activeOnShows.Length);
        isShowing = true;
        gameObject.SetActive(true);
        //for (var i = 0; i < activeOnShows.Length; i++)
        //    activeOnShows[i].gameObject.SetActive(i == clamp);
    }

    private void OnEnable()
    {
        if (!TurnOffForceHide)
            StartCoroutine(ForceHide());
    }

    private void OnDisable()
    {
        TurnOffForceHide = false;

    }


    public void Hide()
    {
        gameObject.SetActive(false);
        isShowing = false;
    }

    public bool TurnOffForceHide { get; set; }
    private bool isIap;

    public IEnumerator LoadingIn(int index, bool isIap = false)
    {
        this.isIap = isIap;
        if (isIap)
        {
            if (loadingAds) loadingAds.SetActive(false);
            if (loadingIap) loadingIap.SetActive(true);
        }
        else
        {
            if (loadingIap) loadingIap.SetActive(false);
            if (loadingAds) loadingAds.SetActive(true);
        }
        Show(index);
        canvasGroup.alpha = 0;
        graphicRaycaster.enabled = true;
        if (fadeIn && ((!isIap && showLoadingAds) || (isIap && showLoadingIap)))
        {
            float t = 0f;
            while (t < 0.3f)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
                canvasGroup.alpha = t / 0.3f;
            }

            yield return new WaitForSecondsRealtime(delay_after_in_load_before_ads.Value);
        }
    }

    public IEnumerator ForceHide()
    {
        yield return new WaitForSecondsRealtime(1 + delay_after_in_load_before_ads.Value + delay_after_in_load_before_ads.Value);
        Hide();
    }

    public IEnumerator LoadingOut(Action actionAfterShow)
    {

        if (fadeOut)
            yield return new WaitForSecondsRealtime(delay_after_ads_to_action.Value);
        else
            yield return new WaitForEndOfFrame();
        if (actionAfterShow != null)
            actionAfterShow.Invoke();
        graphicRaycaster.enabled = false;
        if (fadeOut && ((!isIap && showLoadingAds) || (isIap && showLoadingIap)))
        {
            float t = 0f;
            while (t < 0.3f)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
                canvasGroup.alpha = 1 - t / 0.3f;
            }
        }
        Hide();
    }
}
