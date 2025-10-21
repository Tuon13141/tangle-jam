using UnityEngine;

namespace Sdk.Google.NativeAds
{
    [CreateAssetMenu(menuName = "NativeAdsSDK/NativeAdsSettings")]
    public class NativeAdsSettings : ScriptableObject
    {
        public bool isTest;
        public bool isLog;
        public string nativeAdsID;
        public bool activeRequestRealTime;
        public float requestTimeOut = 3f;
        public int timeRequestAdsAddToCacheWhenEmpty = 20;
    }
}
