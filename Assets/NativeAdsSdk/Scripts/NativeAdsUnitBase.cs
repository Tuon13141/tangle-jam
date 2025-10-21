#if UNITY_GGNativeAds
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using Percas;
using Percas.UI;

namespace Sdk.Google.NativeAds
{
    public class NativeAdsUnitBase : MonoBehaviour, IActivatable
    {
        [Header("Status")]
        [SerializeField] protected NativeAdsStatus NativeAdsStatus;

        // [SerializeField] bool inactiveWhenRequesting;
        [SerializeField] bool hideParent;
        [SerializeField] GameObject adContainer;
        [SerializeField] MoveAnimController moveContainer;

        [Header("Content")]
        [SerializeField] protected bool autoFetchAdsInOnEnable = true;

        [SerializeField] private int refreshTimeAfterImpression = -1;

        protected NativeAdsInfo nativeAdsInfo;
        private float curTimeImpression;
        private bool isRecordImpression;

        protected virtual void Start() { }

        public void Activate()
        {
            if (!GameLogic.CanShowNativeAds)
            {
                adContainer.SetActive(false);
                if (hideParent) adContainer.transform.parent.gameObject.SetActive(false);
                NativeAdsStatus.gameObject.SetActive(false);
                return;
            }

            UpdateUI_Status();

            if (isRecordImpression)
            {
                Debug.LogError("Check Refresh Ads: " + (Time.time - curTimeImpression) + "/" + refreshTimeAfterImpression);
                if (Time.time - curTimeImpression > refreshTimeAfterImpression)
                {
                    curTimeImpression = Time.time;
                    isRecordImpression = false;
                    Fetch();
                }
            }
        }

        public void Deactivate()
        {
            if (nativeAdsInfo != null)
            {
                nativeAdsInfo.SetShowing(false);
                if (nativeAdsInfo.nativeAd != null)
                    nativeAdsInfo.nativeAd.OnPaidEvent -= OnPaidEvent;
            }
            NativeAdsManager.Instance.CheckAddToCacheOrDestroyOldNativeAds(nativeAdsInfo);
        }

        public void Fetch()
        {
            if (!GameLogic.CanShowNativeAds) return;

            // if (inactiveWhenRequesting)
            // {
                adContainer.SetActive(false);
                if (moveContainer != null) moveContainer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            // }
            NativeAdsManager.Instance.RequestNativeAdsByUnit((nativeAdsInfo) =>
            {
                adContainer.SetActive(true);
                if (moveContainer != null) moveContainer.Show(isBack: false);
                this.nativeAdsInfo?.DestroyNativeAds();
                this.nativeAdsInfo = nativeAdsInfo;
                UpdateUI_NativeAds();
            }, () =>
            {
                if (/*inactiveWhenRequesting && */nativeAdsInfo == null)
                {
                    adContainer.SetActive(false);
                    if (moveContainer != null) moveContainer.Show(isBack: true);
                }
            });
        }

        void UpdateUI_Status()
        {
            if (NativeAdsManager.Instance.NativeAdsSetting != null)
            {
                if (NativeAdsManager.Instance.NativeAdsSetting.isTest)
                {
                    NativeAdsStatus.gameObject.SetActive(true);
                    NativeAdsStatus.SetImpressionNotice(false);
                    NativeAdsStatus.SetClickNotice(false);
                }
                else
                {
                    NativeAdsStatus.gameObject.SetActive(false);
                }
            }
        }

        public virtual void UpdateUI_NativeAds()
        {
            if (nativeAdsInfo == null || nativeAdsInfo.nativeAd == null)
            {
                return;
            }
            nativeAdsInfo.nativeAd.OnPaidEvent += OnPaidEvent;
            nativeAdsInfo.SetTimesFillToUI();
            nativeAdsInfo.SetShowing(true);
            isRecordImpression = false;
        }

        void OnPaidEvent(object sender, AdValueEventArgs e)
        {
            StartCoroutine(DelayEndOfFrame(() =>
            {
                nativeAdsInfo.SetRecordImpression();
                NativeAdsStatus.SetImpressionNotice(true);
                OnTrackingRevenue(e);
                if (refreshTimeAfterImpression > 0)
                {
                    curTimeImpression = Time.time;
                    isRecordImpression = true;
                }
            }));
            NativeAdsManager.Instance.Log("ad_impresstion Success");
        }

        protected void ConvertTextureToSpriteAndShow(Image image, Texture2D texture2D)
        {
            if (texture2D == null)
            {
                return;
            }
            image.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(texture2D.width / 2, texture2D.height / 2));
        }

        IEnumerator DelayEndOfFrame(Action callback)
        {
            yield return new WaitForEndOfFrame();
            callback?.Invoke();
        }

        protected virtual void OnTrackingRevenue(AdValueEventArgs e) { }
    }
}
#endif