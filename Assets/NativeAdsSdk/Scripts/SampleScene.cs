#if UNITY_GGNativeAds
using Sdk.Google.NativeAds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScene : MonoBehaviour
{
    [SerializeField] private NativeAdsUnitBase nativeAdsUnitN1;
    [SerializeField] private NativeAdsUnitBase nativeAdsUnitN2;

    private void Start()
    {
        HideN1();
        HideN2();
    }
    public void ShowN1()
    {
        nativeAdsUnitN1.gameObject.SetActive(true);
    }
    public void ShowN2()
    {
        nativeAdsUnitN2.gameObject.SetActive(true);
    }

    public void HideN1()
    {
        nativeAdsUnitN1.gameObject.SetActive(false);
    }
    public void HideN2()
    {
        nativeAdsUnitN2.gameObject.SetActive(false);
    }
}
#endif