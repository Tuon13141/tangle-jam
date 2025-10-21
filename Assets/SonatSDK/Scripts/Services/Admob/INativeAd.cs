using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface INativeAd
{
    public void ShowNativeAds(TextMeshProUGUI text, Image icon, GameObject clickableObj);

    public void ShowNativeAds(TextMeshProUGUI text, Image icon, GameObject installButton, GameObject iconObj);

    public void ShowNativeAds(Text text, Image icon, GameObject clickableObj);

    public void ShowNativeAds(Text text, Image icon, GameObject installButton, GameObject iconObj);
}
