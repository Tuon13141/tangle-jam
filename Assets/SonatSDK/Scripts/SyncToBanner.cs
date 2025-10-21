using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SyncToBanner : KernelLoadedView
{
    [SerializeField] private RectTransform bannerUiBackground;
    [SerializeField] private float addHeight;
    [SerializeField] private float iosHomeButtonHeight = 100;

    public float AddHeight
    {
#if UNITY_EDITOR
        get => addHeight + 130;
#else
 get => addHeight;
#endif
    }

    protected override void Start()
    {
        base.Start();
//        gameObject.SetActive(Kernel.IsReady() && Kernel.Resolve<AdsManager>().IsBannerShowing());
        UpdateSize();
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        SetActive(Kernel.Resolve<AdsManager>().IsBannerShowing());
        Kernel.Resolve<AdsManager>().onBannerShow.Action += data =>
        {
            UIDebugLog.Log("SyncToBanner: " + data);
            SetActive(data);
            UpdateSize();
        };
    }

    private void SetActive(bool value)
    {
        gameObject.SetActive(value);
        bannerUiBackground.gameObject.SetActive(value);
    }

    [MyButton(nameof(Check))] [SerializeField]
    private int test;

    public void Check()
    {
        SetActive(Kernel.Resolve<AdsManager>().IsBannerShowing());
        UpdateSize();
    }

    void UpdateSize()
    {
        if (bannerUiBackground != null)
        {
#if UNITY_ANDROID
            var size = bannerUiBackground.sizeDelta;
            size.y = AdsManager.BannerHeight + AddHeight;
            bannerUiBackground.sizeDelta = size;
#endif

#if UNITY_IOS 
            var size = bannerUiBackground.sizeDelta;
            size.y = AdsManager.BannerHeight + (!HaveNotch () ? AddHeight : iosHomeButtonHeight + addHeight);
            bannerUiBackground.sizeDelta = size;
            Debug.Log($"AdsManager.BannerHeight {AdsManager.BannerHeight}" +
                                $" HaveNotch{HaveNotch()} AddHeight{AddHeight} iosHomeButtonHeight{iosHomeButtonHeight} Screen.height{Screen.height} ");
#endif
        }
    }

    public static bool HaveNotch()
    {
        return Screen.safeArea.height < Screen.height;
    }
}