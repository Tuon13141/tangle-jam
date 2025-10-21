using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UnityEngine;
using UnityEngine.UI;

public enum BuiltInFunctionTestSdk
{
    FillTestAds,
    EnableNoAds,
    DisableNoAds,
    ShowAdsIds,
    ShowStatistics,
    Vibrate,
    AdmobInspector,
    ShowInters,
    LogStartLevel1000,
    OnScreenDebugLog,
    OnScreenUiDebugLog,
    DummyPurchase,
}

[RequireComponent(typeof(Button))]
public class BuiltInButtonTestSdk : MonoBehaviour
{
    [SerializeField] private BuiltInFunctionTestSdk Enum;
    [SerializeField] private int value;

    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    
    protected  void OnClick()
    {
        Handler(Enum);
    }

    public void Handler(BuiltInFunctionTestSdk testSdk)
    {
        switch (testSdk)
        {
            case BuiltInFunctionTestSdk.FillTestAds:
                Kernel.Resolve<AdsManager>().EnableTestAdsThenClearAds();
                break;
            case BuiltInFunctionTestSdk.EnableNoAds:
                Kernel.Resolve<AdsManager>().EnableNoAds();
                break;
            case BuiltInFunctionTestSdk.DisableNoAds:
                Kernel.Resolve<AdsManager>().DisableNoAds();
                break;
            case BuiltInFunctionTestSdk.ShowAdsIds:
                Kernel.Resolve<AdsManager>().ShowAdsIds();
                break;
            case BuiltInFunctionTestSdk.ShowStatistics:
                ShowStatistic();
                break;
            case BuiltInFunctionTestSdk.AdmobInspector:
                Kernel.Resolve<AdsManager>().ShowDebugInspector();
                break;
            case BuiltInFunctionTestSdk.Vibrate:
                break;
            case BuiltInFunctionTestSdk.ShowInters:
                Kernel.Resolve<AdsManager>().ShowInterstitial("test".CreateDefaultLogInterstitial(),false,true);
                break;
            case BuiltInFunctionTestSdk.LogStartLevel1000:
                new SonatLogLevelStart()
                {
                    level = 1000
                }.Post();
                break;
            case BuiltInFunctionTestSdk.OnScreenDebugLog:
                Kernel.kernel.AddOnScreenDebugLog();
                break;
            case BuiltInFunctionTestSdk.DummyPurchase:
                Kernel.Resolve<BasePurchaser>().SetDummy(true);
                break;
            case BuiltInFunctionTestSdk.OnScreenUiDebugLog:
                Kernel.kernel.AddOnScreenDebugLog2();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void ShowStatistic()
    {
        UIDebugLog.Log("refreshRate:" + Screen.currentResolution.refreshRate);
        UIDebugLog.Log("Screen.safeArea.height:" + Screen.safeArea.height);
        UIDebugLog.Log("Screen.height:" + Screen.height);
    }
}