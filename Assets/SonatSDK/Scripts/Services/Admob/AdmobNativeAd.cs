#if use_admob_native_ad
using GoogleMobileAds.Api;
#endif
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdmobNativeAd : INativeAd
{
#if use_admob_native_ad
    public NativeAd Ad
    {
        get;
        set;
    }

    public void ShowNativeAds(Text text, Image icon, GameObject clickableObj)
    {
        if (Ad == null)
        {
            return;
        }

        Texture2D iconTexture = Ad.GetIconTexture();
        string headline = Ad.GetHeadlineText();

        icon.sprite = Sprite.Create(iconTexture, new Rect(0.0f, 0.0f, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        text.text = headline;

        if (!Ad.RegisterIconImageGameObject(clickableObj))
        {
            // Handle failure to register ad asset.
            UIDebugLog.Log("RegisterIconImageGameObject Failed: " + clickableObj.name);
        }
    }

    public void ShowNativeAds(Text text, Image icon, GameObject installButton, GameObject iconObj)
    {
        if (Ad == null)
        {
            return;
        }

        Texture2D iconTexture = Ad.GetIconTexture();
        string headline = Ad.GetHeadlineText();

        icon.sprite = Sprite.Create(iconTexture, new Rect(0.0f, 0.0f, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        text.text = headline;

        if (!Ad.RegisterStoreGameObject(installButton))
        {
            // Handle failure to register ad asset.
            UIDebugLog.Log("RegisterStoreGameObject Failed: " + installButton.name);
        }

        if (!Ad.RegisterIconImageGameObject(iconObj))
        {
            // Handle failure to register ad asset.
            UIDebugLog.Log("RegisterIconImageGameObject Failed: " + iconObj.name);
        }
    }

    public void ShowNativeAds(TextMeshProUGUI text, Image icon, GameObject clickableObj)
    {
        if (Ad == null)
        {
            return;
        }

        Texture2D iconTexture = Ad.GetIconTexture();
        string headline = Ad.GetHeadlineText();

        icon.sprite = Sprite.Create(iconTexture, new Rect(0.0f, 0.0f, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        text.text = headline;

        if (!Ad.RegisterIconImageGameObject(clickableObj))
        {
            // Handle failure to register ad asset.
            UIDebugLog.Log("RegisterIconImageGameObject Failed: " + clickableObj.name);
        }
    }

    public void ShowNativeAds(TextMeshProUGUI text, Image icon, GameObject installButton, GameObject iconObj)
    {
        if (Ad == null)
        {
            return;
        }

        Texture2D iconTexture = Ad.GetIconTexture();
        string headline = Ad.GetHeadlineText();

        icon.sprite = Sprite.Create(iconTexture, new Rect(0.0f, 0.0f, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        text.text = headline;

        if (!Ad.RegisterStoreGameObject(installButton))
        {
            // Handle failure to register ad asset.
            UIDebugLog.Log("RegisterStoreGameObject Failed: " + installButton.name);
        }

        if (!Ad.RegisterIconImageGameObject(iconObj))
        {
            // Handle failure to register ad asset.
            UIDebugLog.Log("RegisterIconImageGameObject Failed: " + iconObj.name);
        }
    }

#else
    public void ShowNativeAds(TextMeshProUGUI text, Image icon, GameObject clickableObj)
    {      
    }

    public void ShowNativeAds(TextMeshProUGUI text, Image icon, GameObject installButton, GameObject iconObj)
    {        
    }
    public void ShowNativeAds(Text text, Image icon, GameObject clickableObj)
    { 
    }

    public void ShowNativeAds(Text text, Image icon, GameObject installButton, GameObject iconObj)
    { 
    }
#endif
}
