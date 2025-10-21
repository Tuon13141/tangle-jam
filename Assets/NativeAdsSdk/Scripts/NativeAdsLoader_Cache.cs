#if UNITY_GGNativeAds
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using UnityEngine;
using Percas;

namespace Sdk.Google.NativeAds
{
    public class NativeAdsLoader_Cache : MonoBehaviour
    {
        private NativeAdsManager nativeAdsManager;
        private List<NativeAdsInfo> lstNativeAdInfo;

        [SerializeField] private NativeAdsSlowUpdate slowUpdate_CheckAdsOutOfDate;
        [SerializeField] private NativeAdsSlowUpdate slowUpdate_CheckEmptyAndRequestAdsAddToCache;
        [SerializeField] private bool requesting;

        private int timeCheckAdsOutOfDate = 30;
        private int nativeAdsID;

        private void Awake()
        {
            TimeManager.OnTick += OnUpdate;
        }

        private void OnDestroy()
        {
            TimeManager.OnTick -= OnUpdate;
        }

        public void Init(NativeAdsManager nativeAdsManager)
        {
            this.nativeAdsManager = nativeAdsManager;
            lstNativeAdInfo = new List<NativeAdsInfo>();
            slowUpdate_CheckAdsOutOfDate = new NativeAdsSlowUpdate(timeCheckAdsOutOfDate, timeCheckAdsOutOfDate, CheckAndRemoveCacheOutOfDate, true);
            slowUpdate_CheckAdsOutOfDate.ResetTime();

            //float timeRequestAdsAddToCacheWhenEmpty = amobearNativeAdsManager.AmobearNativeAdsSetting.timeRequestAdsAddToCacheWhenEmpty;
            float timeRequestAdsAddToCacheWhenEmpty = 30;
            slowUpdate_CheckEmptyAndRequestAdsAddToCache = new NativeAdsSlowUpdate(timeRequestAdsAddToCacheWhenEmpty, timeRequestAdsAddToCacheWhenEmpty, OnCheckAdsEmptyAndRequestAds, true);
            slowUpdate_CheckEmptyAndRequestAdsAddToCache.ResetTime();
        }

        public void RequestAdsAddToCache()
        {
            RequestNativeAd();
        }

        public NativeAdsInfo GetNativeAdsByUnit()
        {
            CheckAndRemoveCacheIfNull();
            for (int i = lstNativeAdInfo.Count - 1; i >= 0; i--)
            {
                NativeAdsInfo nativeAdsInfo = lstNativeAdInfo[i];
                lstNativeAdInfo.RemoveAt(i);

                nativeAdsManager.Log("Get: id=" + nativeAdsInfo.nativeAdsID + " _ Count=" + lstNativeAdInfo.Count);
                OnCheckAdsEmptyAndRequestAds();
                return nativeAdsInfo;
            }
            return null;
        }

        void RequestNativeAd()
        {
            if (requesting)
            {
                nativeAdsManager.Log("Cache Requesting");
                return;
            }
            nativeAdsManager.Log("Load Ads Cache");
            requesting = true;
            AdLoader adLoader = null;
            var id_native = "";
            if (NativeAdsManager.Instance.NativeAdsSetting.isTest)
            {
                adLoader = new AdLoader.Builder(NativeAdsManager.Instance.nativeAdsTestID.Trim())
                    .ForNativeAd()
                    .Build();
            }
            else
            {
                id_native = NativeAdsManager.Instance.NativeAdsSetting.nativeAdsID;
                adLoader = new AdLoader.Builder(id_native.Trim())
                    .ForNativeAd()
                    .Build();
            }
            adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
            adLoader.OnAdFailedToLoad += this.HandleNativeAdFailedToLoad;

            AdRequest adRequest = new()
            {
                //Keywords = new HashSet<string> { "gaming", "action", "multiplayer", "finance", "ecommerce" },
            };
            adLoader.LoadAd(adRequest);
        }

        void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
        {
            AddNativeAdsToCache(args.nativeAd);
            requesting = false;
            nativeAdsManager.Log("Native Ads Cache: Loaded");
        }

        void HandleNativeAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            requesting = false;
            nativeAdsManager.Log("Native Ads Cache: Failed");
        }

        public void AddNativeAdsToCache(NativeAd nativeAd)
        {
            lstNativeAdInfo.Add(new NativeAdsInfo(nativeAdsID, nativeAd, DateTime.Now, false, 0, false));
            nativeAdsID++;
        }

        public void AddNativeAdsToCache(NativeAdsInfo nativeAdsInfo)
        {
            lstNativeAdInfo.Add(nativeAdsInfo);
        }

        void CheckAndRemoveCacheOutOfDate()
        {
            DateTime now = DateTime.Now;
            for (int i = lstNativeAdInfo.Count - 1; i >= 0; i--)
            {
                TimeSpan timeSpan = now - lstNativeAdInfo[i].dateTime;
                if (timeSpan.Days > 0 || timeSpan.Hours > 1 || timeSpan.Minutes > 30)
                {
                    NativeAdsInfo nativeAdsInfo = lstNativeAdInfo[i];
                    if (!nativeAdsInfo.showing)
                    {
                        lstNativeAdInfo[i].DestroyNativeAds();
                        lstNativeAdInfo.RemoveAt(i);
                    }
                }
            }
            slowUpdate_CheckAdsOutOfDate.ResetTime();
        }

        void CheckAndRemoveCacheIfNull()
        {
            for (int i = lstNativeAdInfo.Count - 1; i >= 0; i--)
            {
                if (lstNativeAdInfo[i].nativeAd == null)
                {
                    lstNativeAdInfo.RemoveAt(i);
                }
            }
        }

        void OnCheckAdsEmptyAndRequestAds()
        {
            if (lstNativeAdInfo.Count <= 0)
            {
                RequestAdsAddToCache();
            }
            slowUpdate_CheckEmptyAndRequestAdsAddToCache.ResetTime();
        }

        private void OnUpdate()
        {
            slowUpdate_CheckAdsOutOfDate.Update();
            slowUpdate_CheckEmptyAndRequestAdsAddToCache.Update();
        }
    }
}
#endif
