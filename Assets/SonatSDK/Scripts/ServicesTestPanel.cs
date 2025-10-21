using System;
using System.Collections;
using System.Collections.Generic;
using Sonat;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;
#endif


public class ServicesTestPanel : KernelLoadedView
{
    public Text textCoin;
    public Text logText;
    public GameObject[] showGameObject;
    public RectTransform panelAds;
    public PlayerPrefInt coin;
    
    public Button noAdsButton;
    public Button watchAdsButton;
    public Button btnBannerShow;
    public Button btnBannerHide;
    public Button btnBannerSwitchNative;
    public Button btnBannerSwitchAdaptive;
    public Button btnShowInterstitial;
    public Button btnShowMrec;
    public Button btnHideMrec;
    public Button btnShowCollapsible;
    public Button btnHideCollapsible;
    public Button btnShowNativeAd;
    public Button btnHideNativeAd;

    public Button btnDeleteAllPref;
    public Button[] showButtons;
    public Button btnLogRemote;
    public Dropdown dropdownRemoteKey;

    //Native Ad
    public GameObject nativeAdView;
    public TextMeshProUGUI nativeAdText;
    public Image nativeAdIcon;
    public GameObject clickableAd;
    
    private void UpdateCoin(int value)
    {
        textCoin.text = value.ToString();
    }
    protected override void Start()
    {
        base.Start();
        coin = new PlayerPrefInt(nameof(coin));
        coin.OnChanged += UpdateCoin;
        UpdateCoin(coin.Value);
        
        dropdownRemoteKey.options = new List<Dropdown.OptionData>(new Dropdown.OptionData[]
        {
            new Dropdown.OptionData(((RemoteConfigKey) 0).ToString()), 
            new Dropdown.OptionData(((RemoteConfigKey) 1).ToString()), 
            new Dropdown.OptionData(((RemoteConfigKey) 2).ToString()), 
            new Dropdown.OptionData(((RemoteConfigKey) 3).ToString()), 
            new Dropdown.OptionData(((RemoteConfigKey) 4).ToString()), 
            new Dropdown.OptionData(((RemoteConfigKey) 5).ToString()), 
            new Dropdown.OptionData(((RemoteConfigKey) 6).ToString()), 
            new Dropdown.OptionData(((RemoteConfigKey) 7).ToString()), 
            new Dropdown.OptionData(((RemoteConfigKey) 8).ToString()), 
            new Dropdown.OptionData(((RemoteConfigKey) 9).ToString()), 
        });
        
        btnLogRemote.onClick.AddListener(() =>
        {
            var key = dropdownRemoteKey.options[dropdownRemoteKey.value].text;
            UIDebugLog.Log(key + ": "+Kernel.Resolve<FireBaseController>().remoteConfigController.GetDefault(key).GetDefaultString(true));
        });
        
        for (var i = 0; i < showButtons.Length; i++)
        {
            var i2 = i;
            showButtons[i].onClick.AddListener(() =>
            {
                for (var i1 = 0; i1 < showGameObject.Length; i1++)
                {
                    showGameObject[i1].SetActive(i1 == i2);
                }
            });
        }
        
        noAdsButton.onClick.AddListener(() => Kernel.Resolve<AdsManager>().EnableNoAds());
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        watchAdsButton.interactable = Kernel.Resolve<AdsManager>().IsVideoAdsReady();
        Kernel.Resolve<AdsManager>().onVideoLoaded.Action += data =>
        {
            watchAdsButton.interactable = data;
        };
        watchAdsButton.onClick.AddListener(() =>
        {
            Kernel.Resolve<AdsManager>().ShowVideoAds(GetReward,new SonatLogVideoRewarded());
        });
        
        btnBannerShow.onClick.AddListener(() =>
        {
            Kernel.Resolve<AdsManager>().ShowBanner();
        });
        
        btnBannerHide.onClick.AddListener(() =>
        {
            Kernel.Resolve<AdsManager>().HideBanner();
        });
        
        btnBannerSwitchNative.onClick.AddListener(() =>
        {
            Kernel.Resolve<AdsManager>().SwitchBanner(true);
        });
        
        btnBannerSwitchAdaptive.onClick.AddListener(() =>
        {
            Kernel.Resolve<AdsManager>().SwitchBanner(false);
        });
        
        btnShowInterstitial.onClick.AddListener(() =>
        {
            Kernel.Resolve<AdsManager>().ShowInterstitial("test".CreateDefaultLogInterstitial(),false,false, CallbackInter);
        });

        btnShowMrec.onClick.AddListener(() =>
        {
            Kernel.Resolve<AdsManager>().ShowMrecBanner();
        });

        btnHideMrec.onClick.AddListener(() =>
        {
            Kernel.Resolve<AdsManager>().HideMrecBanner();
        });

        btnShowCollapsible.onClick.AddListener(() =>
        {
            Kernel.Resolve<AdsManager>().ShowCollapsibleBanner();
        });

        btnHideCollapsible.onClick.AddListener(() =>
        {
            Kernel.Resolve<AdsManager>().HideCollapsibleBanner();
        });

        btnShowNativeAd.onClick.AddListener(() => 
        {
            if (Kernel.Resolve<AdsManager>().IsNativeAdReady())
            {
                INativeAd nativeAd = Kernel.Resolve<AdsManager>().GetNativeAd();

                if (nativeAd != null)
                {
                    nativeAdView.SetActive(true);
                    nativeAd.ShowNativeAds(nativeAdText, nativeAdIcon, clickableAd);
                }
            }
        });

        btnHideNativeAd.onClick.AddListener(() => 
        {
            nativeAdView.SetActive(false);
            Kernel.Resolve<AdsManager>().HideNativeAd();
        });

        btnDeleteAllPref.onClick.AddListener(() =>
        {
            PlayerPrefs.DeleteAll();
            TestingSettings.TurnOffBanner = false;
#if UNITY_EDITOR

#endif
        });


        Kernel.Resolve<BasePurchaser>().onInAppPurchased.Action += data =>
        {
            if (data == 1)
            {
                coin.Value += 100;
            }
        };
    }

    private void GetReward()
    {
        coin.Value += 100;
    }

    private void CallbackInter()
	{
        Debug.Log("Next game");
	}
}
