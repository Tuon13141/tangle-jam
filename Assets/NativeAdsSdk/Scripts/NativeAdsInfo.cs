#if UNITY_GGNativeAds
using GoogleMobileAds.Api;
using System;

namespace Sdk.Google.NativeAds
{
    [System.Serializable]
    public class NativeAdsInfo
    {
        public int nativeAdsID;
        public NativeAd nativeAd;
        public DateTime dateTime;
        public bool recordImpression;
        public int countTimesFillToUI;
        public bool showing;

        public NativeAdsInfo(int nativeAdsID, NativeAd nativeAd, DateTime dateTime, bool recordImpression, int countTimesFillToUI, bool showing)
        {
            this.nativeAdsID = nativeAdsID;
            this.nativeAd = nativeAd;
            this.dateTime = dateTime;
            this.recordImpression = recordImpression;
            this.countTimesFillToUI = countTimesFillToUI;
            this.showing = showing;
        }

        public void DestroyNativeAds()
        {
            nativeAd?.Destroy();
        }

        public void SetRecordImpression()
        {
            recordImpression = true;
        }

        public void SetTimesFillToUI()
        {
            countTimesFillToUI++;
        }

        public void SetShowing(bool value)
        {
            showing = value;
        }

        public bool Old()
        {
            return recordImpression || countTimesFillToUI >= 2;
        }
    }
}
#endif