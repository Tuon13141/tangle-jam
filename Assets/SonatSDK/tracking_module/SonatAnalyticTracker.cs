using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using UnityEngine;



#if dummy_log

public static class SonatTrackingHelper
{

      


    public static bool TryToPostQueues()
    {
       return false;
    }
}

#endif

#if !dummy_log
using AppsFlyerSDK;
using Firebase.Analytics;
// ReSharper disable InconsistentNaming

namespace Sonat
{
    public static class SonatTrackingHelper
    {

        public static readonly List<BaseSonatAnalyticLog> NotReadyQueues = new List<BaseSonatAnalyticLog>();

        public static bool TryToPostQueues()
        {
            if (SonatAnalyticTracker.FirebaseReady)
            {
                foreach (var log in NotReadyQueues)
                    log.Post(log.PostAf);
                NotReadyQueues.Clear();
                return true;
            }

            return false;
        }
        
        public static void CrossAppPromotionClick(string appId,string campaign,Dictionary<string,string> parameters,MonoBehaviour go)
        {
            AppsFlyer.attributeAndOpenStore(appId,campaign,parameters,go);
            Debug.Log("AppsFlyer.attributeAndOpenStore(appId,campaign,parameters,go)");
        }

        public static void CrossAppPromotionImpression(string promotedAppID, string campaign,Dictionary<string,string> parameters)
        {
            AppsFlyer.recordCrossPromoteImpression(promotedAppID,campaign, parameters);
    //        Debug.Log("AppsFlyer.recordCrossPromoteImpression(appId,campaign,parameters,go)");
        }
        
        private static string GetDefault(AdsPlatform platform)
        {
            switch (platform)
            {
                case AdsPlatform.applovinmax:
                    return platform.ToString();
                case AdsPlatform.googleadmob:
                    return "admob";
                case AdsPlatform.ironsource:
                    return platform.ToString();
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }
        }

        private static readonly Dictionary<string,string>  mediationDict = new Dictionary<string, string>()
        {
            {"googleadmanager","googleadmanager"},
            {"admob","admob"},
            {"applovin","applovinmax"},
            {"max","applovinmax"},
            {"fyber","fyber"},
            {"appodeal","appodeal"},
            {"inmobi","inmobi"},
            {"vungle","vungle"},
            {"admost","admost"},
            {"topon","topon"},
            {"tradplus","tradplus"},
            {"chartboost","chartboost"},
            {"facebook","facebook"},
            {"meta","facebook"},
            {"mintegral","mintegral"},
            {"mtg","mintegral"},
            {"ironsource","ironsource"},
            {"unity","unity"},
            {"pangle","pangle"},
            {"bytedance","bytedance"},
            {"bidmachine","bidmachine"},
            {"liftoff","liftoff"},
            {"mytarget","mytarget"},
            {"smaato","smaato"},
            {"tapjoy","tapjoy"},
            {"verve","verve"},
            {"yahoo","yahoo"},
            {"yandex","yandex"},
            {"google","admob"},
        };

        public static string FindNetworkName(string splitLower)
        {
            foreach (var keyValuePair in mediationDict)
                if (splitLower.Contains(keyValuePair.Key))
                    return keyValuePair.Value;

            return null;
        }


        public static string GetNetworkName(string fullNetworkName, AdsPlatform platform)
        {
            if (string.IsNullOrEmpty(fullNetworkName))
                return GetDefault(platform);

            var split = fullNetworkName.Split('.');
            var lower = split[split.Length-1].ToLower();
            return FindNetworkName(lower) ?? GetDefault(platform);
        }
    }

    public static class SonatAnalyticTracker
    {
        public static string RewardedLogPlacement;
        public static string InterstitialLogName;
        public static bool FirebaseReady { get; set; }

        public static float sn_ltv_iaa
        {
            get => PlayerPrefs.GetFloat(nameof(sn_ltv_iaa));
            set
            {
               var last = PlayerPrefs.GetFloat(nameof(sn_ltv_iaa));
               if (Math.Abs(last - value) > 0.0001f)
               {
                   Kernel.Resolve<FireBaseController>().SetUserProperty(nameof(sn_ltv_iaa),value.ToString(CultureInfo.InvariantCulture));
                   PlayerPrefs.SetFloat(nameof(sn_ltv_iaa),value);
               }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"> admob, max app lovin or iron source</param>
        /// <param name="adapter"></param>
        /// <param name="revenue">revenue in USD not microUSD</param>
        /// <param name="precision">float string</param>
        /// <param name="adType">banner, inter, video</param>
        /// <param name="currencyCode">usd maybe</param>
        public static void LogRevenue(AdsPlatform platform, string adapter, double revenue, string precision,
            AdTypeLog adType,string fb_instance_id,string placement, string currencyCode = "USD")
        {
            LogFirebaseRevenue(platform,adapter,revenue,precision,adType.ToString(),fb_instance_id,placement,currencyCode);
            LogAppsFlyerAdRevenue(platform,adapter,revenue,adType.ToString(),fb_instance_id,placement,currencyCode);
            sn_ltv_iaa += (float)revenue;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"> admob, max app lovin or iron source</param>
        /// <param name="adapter"></param>
        /// <param name="revenue">revenue in USD not microUSD</param>
        /// <param name="precision">float string</param>
        /// <param name="adType">banner, inter, video</param>
        /// <param name="currencyCode">usd maybe</param>
        public static void LogFirebaseRevenue(AdsPlatform platform, string adapter, double revenue, string precision,
            string adType, string fb_instance_id,string placement, string currencyCode = "USD")
        {
            if (!FirebaseReady) return;

            Parameter[] parameters =
            {
                new Parameter("valuemicros", revenue * 1000000f),
                new Parameter("value", (float) revenue),
                // These values below won’t be used in ROAS recipe.
                // But log for purposes of debugging and future reference.
                new Parameter("currency", currencyCode),
                new Parameter("precision", precision),
                new Parameter(ParameterEnum.ad_format.ToString(), adType),
                new Parameter(ParameterEnum.fb_instance_id.ToString(), fb_instance_id),
                new Parameter(ParameterEnum.ad_placement .ToString(), placement),
                new Parameter(ParameterEnum.ad_source.ToString(), SonatTrackingHelper.GetNetworkName(adapter, platform)),
                new Parameter(ParameterEnum.ad_platform.ToString(), platform.ToString()),
            };
            FirebaseAnalytics.LogEvent(EventNameEnum.paid_ad_impression.ToString(), parameters);

            if (platform != AdsPlatform.googleadmob)
            {
                FirebaseAnalytics.LogEvent(EventNameEnum.ad_impression.ToString(), parameters);
            }
        }

        public static void LogAppsFlyerAdRevenue(AdsPlatform platform,string adapter, double revenue, string adType,string firebase_instance_id,string placement, 
            string currencyCode = "USD")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("af_quantity", "1");
            dic.Add("ad_type", adType);
            dic.Add("ad_unit", adType);
            dic.Add("placement", placement);
            dic.Add("segment", placement);

            UIDebugLog.Log($"duong logAdRevenue adapter:{adapter},platform:{platform},revenue{revenue}");
#if appsflyer_6_15_or_older
            AppsFlyerAdRevenue.logAdRevenue(SonatTrackingHelper.GetNetworkName(adapter, platform),
                GetNetworkType(platform), revenue, currencyCode, dic);
#else
            var logRevenue = new AFAdRevenueData(SonatTrackingHelper.GetNetworkName(adapter, platform), GetNetworkType(platform), currencyCode, revenue);
            AppsFlyer.logAdRevenue(logRevenue, dic);
#endif
        }

#if appsflyer_6_15_or_older
        private static AppsFlyerAdRevenueMediationNetworkType GetNetworkType(AdsPlatform platform)
        {
            switch (platform)
            {
                case AdsPlatform.applovinmax:
                    return AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax;
                case AdsPlatform.googleadmob:
                    return AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeGoogleAdMob;
                case AdsPlatform.ironsource:
                    return AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeIronSource;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }
        }
#else
        private static MediationNetwork GetNetworkType(AdsPlatform platform)
        {
            switch (platform)
            {
                case AdsPlatform.applovinmax:
                    return MediationNetwork.ApplovinMax;
                case AdsPlatform.googleadmob:
                    return MediationNetwork.GoogleAdMob;
                case AdsPlatform.ironsource:
                    return MediationNetwork.IronSource;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }
        }
#endif
    }

    public class CustomSonatLog : BaseSonatAnalyticLog
    {
        public override string EventName => _name;

        protected override List<LogParameter> GetParameters() => _logs;

        private readonly List<LogParameter> _logs;
        private readonly string _name;
        public CustomSonatLog(string name,List<LogParameter> logs)
        {
            _name = name;
            _logs = logs;
        }
        
        public CustomSonatLog(sonat_log_enum log,List<LogParameter> logs)
        {
            _name = log.ToString();
            _logs = logs;
        }
    }
    
    public abstract class BaseSonatAnalyticLog
    {
        protected abstract List<LogParameter> GetParameters();
        public abstract string EventName { get; }


        public bool PostAf { get; set; }
        private LogParameter[] _extra;
        

        public BaseSonatAnalyticLog SetExtraParameter(LogParameter[] extra)
        {
            _extra = extra;
            return this;
        }

        private void OnScreenLog(string eventName, List<LogParameter> logs)
        {
            if (OnScreenDebugLog.Instance != null)
            {
                var pr = string.Join("-",logs.Select(x => x.GetDisplayMinify()));
                OnScreenDebugLog.Log("LogEvent: "+eventName +"\t " +pr);
            }
            
#if UNITY_EDITOR
            var pr2 = string.Join("-",logs.Select(x => x.GetDisplayMinify()));
            eventsForEdiorWindow.Insert(0,"LogEvent: "+eventName +"\t " +pr2);
#endif
        }
        
#if UNITY_EDITOR
        public static void ClearEditorLog() => eventsForEdiorWindow.Clear();
        public static IList<string> eventsForEdiorWindow = new List<string>();
#endif
        
        public virtual void Post(bool logAf = false,int debugLog = 0)
        {
            if (SonatAnalyticTracker.FirebaseReady)
            {
                var listParameters = GetParameters();

                if(listParameters == null)
                    listParameters = new List<LogParameter>();
                listParameters.Add(new LogParameter(nameof(network_connect_type), GetConnectionType().ToString()));

                if (_extra != null)
                {
                    listParameters.AddRange(_extra);
#if !UNITY_STANDALONE_WIN && !UNITY_EDITOR && use_firebase
                    FirebaseAnalytics.LogEvent(EventName, listParameters.Select(x => x.Param).ToArray());
#endif
                }
#if !UNITY_STANDALONE_WIN && !UNITY_EDITOR && use_firebase
                else
                    FirebaseAnalytics.LogEvent(EventName, listParameters.Select(x => x.Param).ToArray());
#endif

                if (logAf)
                    LogAf(listParameters);
                
                OnScreenLog(EventName, listParameters);
            }
            else
            {
                OnScreenLog("firebase_not_ready",new List<LogParameter>()
                {
                   new LogParameter("message", "Firebase not ready : SonatAnalyticTracker.FirebaseReady, push to queued")
                });
                SonatTrackingHelper.NotReadyQueues.Add(this);
            }

     
//#if UNITY_EDITOR
            if (debugLog > 0)
            {
                var listParameters = GetParameters();
                if(listParameters == null)
                    listParameters = new List<LogParameter>();
                
                var mess = "Log: " +EventName + "\n";
                mess += string.Join( '\n',listParameters.Select(x => x.GetDisplay()));
                Debug.Log("Sonat "+mess);
                if(debugLog == 1)
                    UIDebugLog.LogError(mess);
                else if(debugLog == 2)
                    UIDebugLog.LogError(mess,true);
            }
//#endif
        }

        protected virtual void LogAf(List<LogParameter> listParameters)
        {
            var dict = new Dictionary<string, string>();
            foreach (var parameter in listParameters)
            {
                if (!dict.ContainsKey(parameter.stringKey))
                    dict.Add(parameter.stringKey, parameter.GetValueAsString());
            }
            AppsFlyer.sendEvent(EventName, dict);
        }

        private network_connect_type GetConnectionType()
        {
            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable:
                    return network_connect_type.none;
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    return network_connect_type.mobile;
                    break;
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    return network_connect_type.wifi;
                    break;
                default:
                    return network_connect_type.other;
            }
        }

        public static bool IsInternetConnection()
        {
#if UNITY_EDITOR

            return true;
#endif
            return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
                   Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }
    }

    public abstract class BaseSonatAnalyticLogAppflyer
    {
        protected abstract Dictionary<string, string> GetParameters();
        public abstract string EventName { get; }

        public void Post()
        {
            AppsFlyer.sendEvent(EventName, GetParameters());
            var pr = string.Join("-",GetParameters().Select(x => x.Key +":"+x.Value));
            if (OnScreenDebugLog.Instance != null)
                OnScreenDebugLog.Log("LogEvent:"+EventName +"\t " +pr);
            UIDebugLog.Log(EventName);
            UIDebugLog.Log(pr);
        }
    }
}
// ReSharper disable InconsistentNaming
#endif