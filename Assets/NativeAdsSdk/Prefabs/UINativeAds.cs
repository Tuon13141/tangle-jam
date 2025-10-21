#if UNITY_GGNativeAds
using System;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using Percas;
using Sonat;

namespace Sdk.Google.NativeAds
{
    public class UINativeAds : NativeAdsUnitBase, IActivatable
    {
        [SerializeField] BoxCollider m_AdIcon, m_AdChoice, m_AdHeadline, m_AdAdvertiser, m_AdCallToAction;
        [SerializeField] private Image imgAdsIcon;
        [SerializeField] private Image imgAdsChoise;
        [SerializeField] private Text txtHeadline;
        [SerializeField] private Text txtAdsAdvertiser;
        [SerializeField] private Text txtCallToAction;
        [SerializeField] private NativeAdsSlowUpdate timeSlow;
        [SerializeField] private bool check;

        protected override void Start()
        {
            Fetch();
            BlockNativeAd(true);
        }

        protected override void OnTrackingRevenue(AdValueEventArgs e)
        {
            double revenue = e.AdValue.Value / ((double)1000000f);
            string placement_name = "native";
            SonatAnalyticTracker.LogRevenue(AdsPlatform.googleadmob, nativeAdsInfo.nativeAd.GetResponseInfo().GetMediationAdapterClassName(), revenue,
                e.AdValue.Precision.ToString(), AdTypeLog.native, Kernel.Resolve<FireBaseController>().FirebaseInstanceId,
                placement_name, e.AdValue.CurrencyCode);
        }

        public override void UpdateUI_NativeAds()
        {
            base.UpdateUI_NativeAds();

            #region Icon Texture
            Texture2D textureIcon;
            try
            {
                textureIcon = nativeAdsInfo.nativeAd.GetIconTexture();
            }
            catch (Exception)
            {
                textureIcon = null;
            }
            if (textureIcon != null && imgAdsIcon != null)
            {
                imgAdsIcon.sprite = Sprite.Create(textureIcon, new Rect(0, 0, textureIcon.width, textureIcon.height), new Vector2(textureIcon.width / 2, textureIcon.height / 2));
                nativeAdsInfo.nativeAd.RegisterIconImageGameObject(imgAdsIcon.gameObject);
            }
            #endregion

            #region Ad Choices Logo Texture
            Texture2D textureAdChoices;
            try
            {
                textureAdChoices = nativeAdsInfo.nativeAd.GetAdChoicesLogoTexture();
            }
            catch (Exception)
            {
                textureAdChoices = null;
            }
            if (textureAdChoices != null && imgAdsChoise != null)
            {
                imgAdsChoise.sprite = Sprite.Create(textureAdChoices, new Rect(0, 0, textureAdChoices.width, textureAdChoices.height), new Vector2(textureAdChoices.width / 2, textureAdChoices.height / 2));
                nativeAdsInfo.nativeAd.RegisterAdChoicesLogoGameObject(imgAdsChoise.gameObject);
            }
            #endregion

            #region Headline Text
            string headLineText;
            try
            {
                headLineText = nativeAdsInfo.nativeAd.GetHeadlineText();
            }
            catch (Exception)
            {
                headLineText = null;
            }
            if (!string.IsNullOrEmpty(headLineText) && txtHeadline != null)
            {
                txtHeadline.text = headLineText;
                nativeAdsInfo.nativeAd.RegisterHeadlineTextGameObject(txtHeadline.gameObject);
            }
            #endregion

            #region Advertiser Text
            string advertiserText;
            try
            {
                advertiserText = nativeAdsInfo.nativeAd.GetAdvertiserText();
            }
            catch (Exception)
            {
                advertiserText = null;
            }
            if (!string.IsNullOrEmpty(advertiserText) && txtAdsAdvertiser != null)
            {
                txtAdsAdvertiser.text = advertiserText;
                nativeAdsInfo.nativeAd.RegisterAdvertiserTextGameObject(txtAdsAdvertiser.gameObject);
            }
            #endregion

            #region Call To Action Text
            string ctaText;
            try
            {
                ctaText = nativeAdsInfo.nativeAd.GetCallToActionText();
            }
            catch (Exception)
            {
                ctaText = null;
            }
            if (!string.IsNullOrEmpty(ctaText) && txtCallToAction != null)
            {
                txtCallToAction.text = ctaText;
                nativeAdsInfo.nativeAd.RegisterCallToActionGameObject(txtCallToAction.gameObject);
            }
            #endregion
        }

        public new void Activate()
        {
            Fetch();
            BlockNativeAd(false);
        }

        public new void Deactivate()
        {
            BlockNativeAd(true);
        }

        public void BlockNativeAd(bool value)
        {
            m_AdIcon.enabled = !value;
            m_AdChoice.enabled = !value;
            m_AdHeadline.enabled = !value;
            m_AdAdvertiser.enabled = !value;
            m_AdCallToAction.enabled = !value;
        }
    }
}
#endif