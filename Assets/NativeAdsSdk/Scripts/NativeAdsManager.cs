
#if UNITY_GGNativeAds
using System;
using UnityEngine;

namespace Sdk.Google.NativeAds
{
    public class NativeAdsManager : MonoBehaviour
    {
        public static NativeAdsManager Instance;
        private NativeAdsLoader_Cache nativeAdsLoader_Cache;
        private NativeAdsLoader_Realtime nativeAdsLoader_RealTime;
        [HideInInspector] public NativeAdsSettings NativeAdsSetting;
        [HideInInspector] public string nativeAdsTestID = "ca-app-pub-3940256099942544/2247696110";

        public static bool AdLoaded { get; set; }
        public static bool AdReady { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(gameObject);
            }

            LoadSetting();
            nativeAdsLoader_Cache = GetComponent<NativeAdsLoader_Cache>();
            nativeAdsLoader_RealTime = GetComponent<NativeAdsLoader_Realtime>();

            if (!Percas.GameLogic.CanShowNativeAds) return;

            nativeAdsLoader_Cache.Init(this);
            nativeAdsLoader_RealTime.Init(this);
        }

        public void LoadNative()
        {
            nativeAdsLoader_Cache.RequestAdsAddToCache();
        }

        void LoadSetting()
        {
            NativeAdsSetting = Resources.Load<NativeAdsSettings>("NativeAdsSettings");
            if (NativeAdsSetting == null)
            {
                Debug.LogError("Cant find Assets NativeAdsSettings in Resources/NativeAdsSettings");
            }
        }

        public void RequestNativeAdsByUnit(Action<NativeAdsInfo> OnComplete, Action OnFailed)
        {
            AdLoaded = false;
            AdReady = false;
            if (NativeAdsSetting.activeRequestRealTime)
            {
                nativeAdsLoader_RealTime.RequestNativeAd((nativeAdsLoaderState, nativeAd) =>
                {
                    if (nativeAd != null)
                    {
                        nativeAdsLoader_Cache.AddNativeAdsToCache(nativeAd);
                        NativeAdsInfo nativeAdsInfo = nativeAdsLoader_Cache.GetNativeAdsByUnit();
                        if (nativeAdsInfo != null)
                        {
                            AdLoaded = true;
                            AdReady = true;
                            OnComplete?.Invoke(nativeAdsInfo);
                        }
                        else
                        {
                            AdLoaded = true;
                            AdReady = false;
                            OnFailed?.Invoke();
                        }
                    }
                    else
                    {
                        NativeAdsInfo nativeAdsInfo = nativeAdsLoader_Cache.GetNativeAdsByUnit();
                        if (nativeAdsInfo != null)
                        {
                            AdLoaded = true;
                            AdReady = true;
                            OnComplete?.Invoke(nativeAdsInfo);
                        }
                        else
                        {
                            AdLoaded = true;
                            AdReady = false;
                            OnFailed?.Invoke();
                        }
                    }
                }, NativeAdsSetting.requestTimeOut != -1);
            }
            else
            {
                NativeAdsInfo nativeAdsInfo = nativeAdsLoader_Cache.GetNativeAdsByUnit();
                if (nativeAdsInfo != null)
                {
                    AdLoaded = true;
                    AdReady = true;
                    OnComplete?.Invoke(nativeAdsInfo);
                }
                else
                {
                    nativeAdsLoader_RealTime.RequestNativeAd((nativeAdsLoaderState, nativeAd) =>
                    {
                        if (nativeAd != null)
                        {
                            nativeAdsLoader_Cache.AddNativeAdsToCache(nativeAd);
                            NativeAdsInfo nativeAdsInfo = nativeAdsLoader_Cache.GetNativeAdsByUnit();
                            if (nativeAdsInfo != null)
                            {
                                AdLoaded = true;
                                AdReady = true;
                                OnComplete?.Invoke(nativeAdsInfo);
                            }
                            else
                            {
                                AdLoaded = true;
                                AdReady = false;
                                OnFailed?.Invoke();
                            }
                        }
                        else
                        {
                            Log("Callback:" + nativeAdsLoaderState.ToString());
                            AdLoaded = true;
                            AdReady = false;
                            OnFailed?.Invoke();
                        }
                    }, false);
                }
            }
        }

        public void CheckAddToCacheOrDestroyOldNativeAds(NativeAdsInfo nativeAdsInfo)
        {
            if (nativeAdsInfo != null)
            {
                if (nativeAdsInfo.Old())
                {
                    if (nativeAdsInfo.nativeAd != null)
                    {
                        nativeAdsInfo.DestroyNativeAds();
                        Log("Destroy Old Native Ads");
                    }
                }
                else
                {
                    nativeAdsLoader_Cache.AddNativeAdsToCache(nativeAdsInfo);
                    Log("Add To Cache");
                }
            }
        }

        public void Log(string content)
        {
            if (NativeAdsSetting.isLog)
            {
                Debug.Log(content);
            }
        }
        public void LogError(string content)
        {
            if (NativeAdsSetting.isLog)
            {
                Debug.LogError(content);
            }
        }
        public void LogWarning(string content)
        {
            if (NativeAdsSetting.isLog)
            {
                Debug.LogWarning(content);
            }
        }
    }
}

#endif
