using UnityEngine;
using UnityEngine.UI;

public class NativeAdsStatus : MonoBehaviour
{
    [SerializeField] private Image impressionNotice;
    [SerializeField] private Image ClickNotice;

    public void SetImpressionNotice(bool value)
    {
        if (value)
        {
            impressionNotice.color = Color.green;
        }
        else
        {
            impressionNotice.color = Color.red;
        }
    }

    public void SetClickNotice(bool value)
    {
        if (value)
        {
            ClickNotice.color = Color.green;
        }
        else
        {
            ClickNotice.color = Color.red;
        }
    }
}
