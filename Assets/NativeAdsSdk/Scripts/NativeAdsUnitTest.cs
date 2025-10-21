#if UNITY_GGNativeAds
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

namespace Sdk.Google.NativeAds
{
    public class NativeAdsUnitTest : NativeAdsUnitBase
    {
        [SerializeField] private Image imgAdsIcon;
        [SerializeField] private Image imgAdsLogo;
        [SerializeField] private Image imgAdsChoise;
        [SerializeField] private Text txtHeadline;
        [SerializeField] private Text txtAdsAdvertiser;
        [SerializeField] private Text txtCallToAction;
        [SerializeField] private Text txtStar;
        [SerializeField] private NativeAdsSlowUpdate timeSlow;
        [SerializeField] private bool check;

        protected override void Start()
        {
            Fetch();
        }

        protected override void OnTrackingRevenue(AdValueEventArgs e)
        {
            //TrackingRevenueConnector.SendRevenue_ToAppflyer_Admob(e.AdValue);
            //TrackingRevenueConnector.SendRevenue_ToFirebase_Admob(e.AdValue);
        }

        public override void UpdateUI_NativeAds()
        {
            base.UpdateUI_NativeAds();

            Texture2D textureIcon = nativeAdsInfo.nativeAd.GetIconTexture();
            if (textureIcon != null)
                imgAdsIcon.sprite = Sprite.Create(textureIcon, new Rect(0, 0, textureIcon.width, textureIcon.height), new Vector2(textureIcon.width / 2, textureIcon.height / 2));

            Texture2D textureChoise = nativeAdsInfo.nativeAd.GetAdChoicesLogoTexture();
            if (textureChoise != null)
                imgAdsChoise.sprite = Sprite.Create(textureChoise, new Rect(0, 0, textureChoise.width, textureChoise.height), new Vector2(textureChoise.width / 2, textureChoise.height / 2));

            txtHeadline.text = nativeAdsInfo.nativeAd.GetHeadlineText();
            txtAdsAdvertiser.text = nativeAdsInfo.nativeAd.GetAdvertiserText();
            txtCallToAction.text = nativeAdsInfo.nativeAd.GetCallToActionText();

            //Register
            nativeAdsInfo.nativeAd.RegisterIconImageGameObject(imgAdsIcon.gameObject);
            nativeAdsInfo.nativeAd.RegisterAdChoicesLogoGameObject(imgAdsChoise.gameObject);
            nativeAdsInfo.nativeAd.RegisterHeadlineTextGameObject(txtHeadline.gameObject);
            nativeAdsInfo.nativeAd.RegisterAdvertiserTextGameObject(txtAdsAdvertiser.gameObject);
            nativeAdsInfo.nativeAd.RegisterCallToActionGameObject(txtCallToAction.gameObject);
        }
    }
}
#endif
