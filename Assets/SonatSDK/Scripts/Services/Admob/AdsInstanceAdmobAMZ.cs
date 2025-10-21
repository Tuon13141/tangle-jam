//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//public class AdsInstanceAdmobAMZ : AdsInstanceAdmob
//{
//    [Header("AMZ")] public string bannerId_AMZ;
//    public string interstitialId_AMZ;
//    public string videoId_AMZ;
//    public string openId_AMZ;
//    
//#if !((dummy || global_dummy) && !use_admob)
//#if UNITY_ANDROID
//#if AMZ
//    protected override string BannerId => testAds ? TestBannerId : bannerId_AMZ;
//    protected override  string InterstitialId => testAds ? TestInterstitialId : interstitialId_AMZ;
//    protected override  string VideoId => testAds ? TestVideoId : videoId_AMZ;
//    protected override  string OpenId => testAds ? TestOpenId : openId_AMZ;
//    #else
//    protected override string BannerId => testAds ? TestBannerId : bannerId;
//    protected override  string InterstitialId => testAds ? TestInterstitialId : interstitialId;
//    protected override  string VideoId => testAds ? TestVideoId : videoId;
//    protected override  string OpenId => testAds ? TestOpenId : openId;
//    #endif
//#endif
//#endif
//}
