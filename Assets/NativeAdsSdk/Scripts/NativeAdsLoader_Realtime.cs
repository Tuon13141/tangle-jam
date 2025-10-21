#if UNITY_GGNativeAds
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using UnityEngine;
using Percas;

namespace Sdk.Google.NativeAds
{
    public class NativeAdsLoader_Realtime : MonoBehaviour
    {
        public enum NativeAdsLoaderState
        {
            Loaded,
            Failed,
            RequestTimeOut
        }

        private NativeAdsManager nativeAdsManager;
        private Action<NativeAdsLoaderState, NativeAd> Callback; // Loaded//Failed --- RequestTimeOut
        [SerializeField] private NativeAdsSlowUpdate slowUpdate_RequestTimeOut;
        [SerializeField] private bool requesting;

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
            float requestTimeOut = nativeAdsManager.NativeAdsSetting.requestTimeOut;
            slowUpdate_RequestTimeOut = new NativeAdsSlowUpdate(requestTimeOut, requestTimeOut, OnRequestTimeOut);
            slowUpdate_RequestTimeOut.Cancel();
        }

        public void RequestNativeAd(Action<NativeAdsLoaderState, NativeAd> Callback, bool useTimeOut)
        {
            this.Callback = Callback;
            if (requesting)
            {
                nativeAdsManager.Log("Real-Time Requesting");
                return;
            }
            requesting = true;
            nativeAdsManager.Log("Load Ads Realtime");
            if (useTimeOut)
            {
                slowUpdate_RequestTimeOut.ResetTime();
            }
            AdLoader adLoader = null;
            if (NativeAdsManager.Instance.NativeAdsSetting.isTest)
            {
                adLoader = new AdLoader.Builder(NativeAdsManager.Instance.nativeAdsTestID.Trim())
                    .ForNativeAd()
                    .Build();
            }
            else
            {
                var id_native = NativeAdsManager.Instance.NativeAdsSetting.nativeAdsID;
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

        private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
        {
            requesting = false;
            SetCallBack(NativeAdsLoaderState.Loaded, args.nativeAd);

            nativeAdsManager.Log("Native Ads RealTime: Loaded");
        }

        private void HandleNativeAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            requesting = false;
            SetCallBack(NativeAdsLoaderState.Failed, null);

            nativeAdsManager.Log("Native Ads RealTime: Failed");
        }

        void SetCallBack(NativeAdsLoaderState nativeAdsLoaderState, NativeAd nativeAd)
        {
            Callback?.Invoke(nativeAdsLoaderState, nativeAd);
        }

        private void OnUpdate()
        {
            slowUpdate_RequestTimeOut.Update();
        }

        void OnRequestTimeOut()
        {
            SetCallBack(NativeAdsLoaderState.RequestTimeOut, null);
        }
    }
}
#endif
