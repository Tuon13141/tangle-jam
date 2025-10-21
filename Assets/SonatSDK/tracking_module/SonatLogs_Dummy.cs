using System;
using System.Collections.Generic;
using UnityEngine;

#if dummy_log
namespace Sonat
{
    public static class SonatAnalyticTracker
    {
        public static string RewardedLogPlacement;
        public static string InterstitialLogName;
        public static bool FirebaseReady { get; set; }


        public static void LogRevenue(AdsPlatform platform, string adapter, double revenue, string precision,
            AdTypeLog adType, string fb_instance_id, string placement, string currencyCode = "USD")
        {
        }

        public static void LogFirebaseRevenue(AdsPlatform platform, string adapter, double revenue, string precision,
            string adType, string fb_instance_id, string placement, string currencyCode = "USD")
        {
        }
    }
    
    [Serializable]
    public class SonatLogBuyShopItemIapInput 
    {
        public string placement;
        public string item_type;
        public int phase;

        public SonatLogBuyShopItemIapInput(string placement, string itemType, int phase)
        {
            this.placement = placement;
            this.item_type = itemType;
            this.phase = phase;
        }
    }
  


    public abstract class BaseSonatAnalyticLog
    {
#if UNITY_EDITOR
        public static void ClearEditorLog() => eventsForEdiorWindow.Clear();
        public static IList<string> eventsForEdiorWindow = new List<string>();
        public static IList<string> events = new List<string>();
#endif
        
        protected virtual List<LogParameter> GetParameters() => new List<LogParameter>();
        public abstract string EventName { get; }


        private LogParameter[] _extra;

        public BaseSonatAnalyticLog SetExtraParameter(Sonat.LogParameter[] extra)
        {
            _extra = extra;
            return this;
        }

        public virtual void Post()
        {
        }
        
        public virtual void Post(bool af,int debug = 0)
        {
        }

        private network_connect_type GetConnectionType()
        {
            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable:
                    return network_connect_type.none;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    return network_connect_type.mobile;
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    return network_connect_type.wifi;
                default:
                    return network_connect_type.other;
            }
        }
    }

    [Serializable]
    public class SonatLogLevelStart : BaseSonatAnalyticLog
    {
        
        public static int UserPropertyLevel
        {
            get => PlayerPrefs.GetInt("user_property_level");
            set => PlayerPrefs.SetInt("user_property_level", value);
        }

        public static string UserPropertyMode
        {
            get => PlayerPrefs.GetString("user_property_mode","classic");
            set => PlayerPrefs.SetString("user_property_mode",value);
        }

        public override string EventName => EventNameEnum.level_start.ToString();
        public int phase { get; set; }
        public bool is_first_play { get; set; }
        public string continue_with { get; set; }
        public int continue_times { get; set; }

        public int level;
        public string mode;
        public bool setUserProperty = true;

        public override void Post()
        {
            base.Post();
            if (setUserProperty)
            {
                try
                {
                    UserPropertyLevel = level;
                    UserPropertyMode = mode;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }

    [Serializable]
    public class SonatPaidAdClick : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.paid_ad_click.ToString();
        public AdTypeLog ad_format;
        public string ad_placement;
        public string fb_instance_id;
    }

    [Serializable]
    public class SonatLogCustom : BaseSonatAnalyticLog
    {
        public override string EventName => _eventName;

        private string _eventName;

        public SonatLogCustom(string eventName)
        {
            _eventName = eventName;
        }
    }
    
    
    [Serializable]
    public class SonatLogOpenShop : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.open_shop.ToString();
        public string placement;
        public int level;
        public string mode;

     
    }


    [Serializable]
    public class SonatLogLevelEnd : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.level_end.ToString();
        public string continue_with { get; set; }
        public int continue_times { get; set; }

        public int level;
        public int phase;
        public string mode;
        public int use_booster_count;
        public int play_time;
        public int move_count;
        public bool is_first_play;
        public bool success;

        public string lose_cause;

        // optional
        public int score = int.MinValue;
        public int highest_score = int.MinValue;
        
        public SonatLogLevelEnd(int level, int phase, int play_time,bool success,bool is_first_play,string mode = "classic")
        {
 
        }

        public SonatLogLevelEnd()
		{

		}
    }
    
  

    [Serializable]
    public class SonatLogLevelUp : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.level_up.ToString();

        public int level;
        public string character;
    }

    [Serializable]
    public class SonatLogUseBooster : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.use_booster.ToString();

        public string name;
        public int level;
        public string mode;
        public int phase;
    }


    public class CustomSonatLog : BaseSonatAnalyticLog
    {
        public override string EventName => _name;

        protected override List<LogParameter> GetParameters() => _logs;

        private readonly List<LogParameter> _logs;
        private readonly string _name;

        public CustomSonatLog(string name, List<LogParameter> logs)
        {
            _name = name;
            _logs = logs;
        }
        
        public CustomSonatLog(sonat_log_enum name, List<LogParameter> logs)
        {
            _name = name.ToString();
            _logs = logs;
        }
    }

    [Serializable]
    public class SonatLogPostScore : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.post_score.ToString();

        public int score;
        public int level;
        public string mode;
    }

    [Serializable]
    public class SonatLogSpentTime : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.app_spent_time.ToString();

        public string placement;
        public long time_msec;
    }

    [Serializable]
    public class SonatLogTutorialBegin : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.tutorial_begin.ToString();

        public string placement;

        public SonatLogTutorialBegin(string placementValue, int stepValue)
        {
           
        }
    }

    [Serializable]
    public class SonatLogTutorialComplete : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.tutorial_complete.ToString();

        public string placement;

        public SonatLogTutorialComplete(string placementValue, int stepValue)
        {
        }
    }

    /// <summary>
    /// "Nhằm để thống kê được thời gian user ở từng giao diện ( screen class nếu ko đặt thì dùng luôn giá trị của screen_name )"
    /// </summary>
    [Serializable]
    public class SonatLogScreenView : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.screen_view.ToString();

        /// <summary>
        /// "Tên của screen trong game. Tuân theo quy tắc viết hoa mỗi  chữ cái của mỗi từ"
        /// </summary>
        public string screen_name;

        /// <summary>
        /// "Tên class của Screen  trong code(có thể dùng chính screen_name)"
        /// </summary>
        public string screen_class;

        public bool saveLastScreen = true;

        private string LastScreen
        {
            get => PlayerPrefs.GetString("last_screen");
            set => PlayerPrefs.SetString("last_screen", value);
        }

        private string LastScreenClass
        {
            get => PlayerPrefs.GetString("last_screen_class");
            set => PlayerPrefs.SetString("last_screen_class", value);
        }
    }

    public class SonatLogLastScreenView : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.screen_view.ToString();

        private string LastScreen
        {
            get => PlayerPrefs.GetString("last_screen");
        }

        private string LastScreenClass
        {
            get => PlayerPrefs.GetString("last_screen_class");
        }
    }

    [Serializable]
    public class SonatLogEarnVirtualCurrency : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.earn_virtual_currency.ToString();
        public string virtual_currency_type { get; set; }
        public string spend_item_type { get; set; }
        public string spend_item_id { get; set; }
        public int level { get; set; }
        public string mode { get; set; }

        public string virtual_currency_name;
        public double value;
        public string placement;
        public string item_type;
        public string item_id;
        public bool is_first_buy;
    }

    [Serializable]
    public class SonatLogShowUi : BaseSonatAnalyticLog
    {
        public override string EventName { get; }
        public string name { get; set; }
        public string open_by { get; set; }
        public string action { get; set; }
        public int level { get; set; }
        public string placement { get; set; }
    }

    [Serializable]
    public class SonatLogSpendVirtualCurrency : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.spend_virtual_currency.ToString();
        public string virtual_currency_type { get; set; }
        public string earn_item_type { get; set; }
        public string earn_item_id { get; set; }
        public int level { get; set; }
        public string mode { get; set; }

        public string virtual_currency_name;
        public double value;
        public string placement;
        public string item_type;
        public string item_id;
    }

    [Serializable]
    public class SonatLogShowInterstitial : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.show_interstitial.ToString();

        public string placement;
        public int level = int.MinValue;
        public string mode;
    }

    [Serializable]
    public class SonatLogVideoRewarded : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.video_rewarded.ToString();
        public int value { get; set; }

        public string placement;
        public int level;
        public string mode;
        public string item_type;
        public string item_id;
    }
    
    [Serializable]
    public class SonatLogBuyShopItem : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.video_rewarded.ToString();
        public int value { get; set; }
        public string currency { get; set; }
        public bool is_first_buy { get; set; }

        public string placement;
        public int level;
        public string mode;
        public string item_type;
        public string item_id;
    }


    [Serializable]
    public class SonatLogShowRate : BaseSonatAnalyticLog
    {
        public enum ShowRateOpenBy
        {
            user,
            auto,
        }

        public enum ShowRateAction
        {
            open,
            star_0,
            rate_now,
            other_star,
            star_5,
            star_4,
            close
        }

        private static readonly string[] convert =
        {
            "open",
            "0_star",
            "rate_now",
            "other_star",
            "5_star",
            "4_star",
            "close",
        };

        public override string EventName => EventNameEnum.show_rate.ToString();

        public ShowRateOpenBy open_by;
        public ShowRateAction action;
    }

    [Serializable]
    public class SonatLogClickIconShortcut : BaseSonatAnalyticLog
    {
        public override string EventName => EventNameEnum.click_icon_shortcut.ToString();
        public int level { get; set; }
        public string placement { get; set; }
        public string mode { get; set; }

        public string shortcut;

        protected override List<Sonat.LogParameter> GetParameters()
        {
            List<Sonat.LogParameter> parameters = new List<Sonat.LogParameter>();
            //  parameters.Add(new Parameter(ParameterEnum.shortcut.ToString(), shortcut.ToString()));
            return parameters;
        }
    }

    [Serializable]
    public class SonatLogInAppPurchaseAppflyer : BaseSonatAnalyticLogAppflyer
    {
        public string af_revenue;
        public int af_quantity = 1;
        public string af_content_type;
        public string af_content_id;
        public string af_order_id;
        public string af_receipt_id;
        public string af_currency;
        public override string EventName => EventNameEnum.af_purchase.ToString();

        protected override Dictionary<string, string> GetParameters()
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add(ParameterEnumForAf.af_revenue.ToString(), af_revenue);
            parameters.Add(ParameterEnumForAf.af_quantity.ToString(), af_quantity.ToString());
            parameters.Add(ParameterEnumForAf.af_content_type.ToString(), af_content_type);
            parameters.Add(ParameterEnumForAf.af_content_id.ToString(), af_content_id);
            parameters.Add(ParameterEnumForAf.af_order_id.ToString(), af_order_id);
            parameters.Add(ParameterEnumForAf.af_receipt_id.ToString(), af_receipt_id);
            parameters.Add(ParameterEnumForAf.af_currency.ToString(), af_currency);
            return parameters;
        }
    }

    public abstract class BaseSonatAnalyticLogAppflyer
    {
        protected abstract Dictionary<string, string> GetParameters();
        public abstract string EventName { get; }

        public void Post()
        {
        }
    }
}


#endif