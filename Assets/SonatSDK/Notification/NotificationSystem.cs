#define dummy
//#define use_notify

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
#if !((dummy || global_dummy) && !use_notify)
using Assets.SimpleAndroidNotifications;
#endif

using MiniJSON;

public enum NotificationType
{
    Once,
    Repeat
}

public enum NotificationTimeType
{
    DelayTime,
    DelayDate,
    DelayDateInWeek,
    SpecificDate
}

[System.Serializable]
public class NotificationItem
{
    public int id;
    public string title;
    public string[] content;
    public string small_icon;
    public string big_icon;
    public bool hasSound;
    public bool hasVibrate;
    public NotificationType notificationType;
    public NotificationTimeType notificationTimeType;
    public int delayTime;
    public int delayDate;
    public DayOfWeek dateInWeek;
    public int exactHour;
    public int exactMinute;
    public int exactSecond;
    public string exactDate;
    public int repeatTimes;
}

public class NotifyItemInfo
{
    public string title;
    public string content;
}

#if (dummy || global_dummy) && !use_notify
public class NotificationSystem : MonoBehaviour
{
}
#else
    public class NotificationSystem : MonoBehaviour
    {
        #region Handle recently
        int almostHours = System.DateTime.Now.Hour;

        private int[] startup_recently = new int[20];

        private void LoadStartupRecently()
        {
            for (int i = 0; i < startup_recently.Length; i++)
            {
                startup_recently[i] = PlayerPrefs.GetInt("startup_ago_session_" + i.ToString(), -1);

                //Debug.Log("startup_ago_session_" + i.ToString() + ": " + startup_recently[i]);
            }
        }

        private void SetStartupRecently(int lastRecently = 20)
        {
            //Debug.Log("lastRecently: " + lastRecently);

            LoadStartupRecently();

            PlayerPrefs.SetInt("startup_ago_session_0", lastRecently);
            for (int i = 1; i < startup_recently.Length; i++)
            {
                PlayerPrefs.SetInt("startup_ago_session_" + i.ToString(), startup_recently[i - 1]);
            }

            LoadStartupRecently();
        }

        private int GetAlmostRecently()
        {
            int[] number_with_hours = new int[24];
            for (int i = 0; i < 24; i++)
            {
                number_with_hours[i] = 0;
            }

            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < startup_recently.Length; j++)
                {
                    if (startup_recently[j] == (i + 1))
                    {
                        number_with_hours[i]++;
                    }
                }
            }

            int max = 0;
            int almost = 20;
            for (int i = 0; i < 24; i++)
            {
                if (number_with_hours[i] > max)
                {
                    max = number_with_hours[i];
                    almost = (i + 1);
                }
            }

            return almost;
        }
        #endregion

        [SerializeField]
        private bool _enableNotify;
        [SerializeField]
        private int repeatitionID;
        [SerializeField]
        private NotificationItem[] notifyItems;

        public string notifications_data = "";
        public List<NotifyItemInfo> notifyItemInfoList = new List<NotifyItemInfo>();

        #region Get Info
        public const string notifications_key = "notifications";
        public const string title_key = "title";
        public const string content_key = "content";

        public void ParseJsonData(ref List<NotifyItemInfo> notifyItemInfoList, string jsonData)
        {
            try
            {
                //Debug.Log("JsonData: " + JsonData);
                if(string.IsNullOrEmpty(jsonData))
                {
                    return;
                }

                var dict = Json.Deserialize(jsonData) as Dictionary<string, object>;

                //Debug.Log("dict: " + dict.ToString());

                if(dict.ContainsKey(notifications_key))
                {
                    //Debug.Log("dict[LIST_VIEW_ADS_KEY]: " + dict[LIST_VIEW_ADS_KEY].ToString());

                    var list_notification = dict[notifications_key] as IList;

                    notifyItemInfoList = new List<NotifyItemInfo>(list_notification.Count);

                    foreach(Dictionary<string, object> item in list_notification)
                    {
                        NotifyItemInfo notifyItemInfo = new NotifyItemInfo();
                        if(item.ContainsKey(title_key))
                        {
                            notifyItemInfo.title = item[title_key].ToString();
                        }

                        if(item.ContainsKey(content_key))
                        {
                            notifyItemInfo.content = item[content_key].ToString();
                        }

                        notifyItemInfoList.Add(notifyItemInfo);
                    }
                }
            }
            catch(System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void GetNotificationData()
        {
            if(string.IsNullOrEmpty(RemoteConfigKey.notifications.GetValueString()))
            {
                //return;
            }
            else
            {
                notifications_data = RemoteConfigKey.notifications.GetValueString();
            }
            
            //Debug.Log("notifications_data: " + notifications_data);

            ParseJsonData(ref notifyItemInfoList, notifications_data);
        }
        #endregion

        public bool enableNotify
        {
            get
            {
                return _enableNotify;
            }
        }

        public static NotificationSystem instance;

        void Awake()
        {
            //if (instance == null)
            //{
            //    Init();
            //}

            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this);

            Invoke(nameof(Init),10);
        }

        public void Init()
        {
            Debug.Log("-- Init Notification --" + enableNotify);
            DateTime date = System.DateTime.Now;
            int hours = date.Hour;

            if (date.Minute > 30)
            {
                hours = hours + 1;
            }

            if (hours > 24)
            {
                hours = 1;
            }

            //Debug.Log("hours: " + hours);

            SetStartupRecently(hours);

            almostHours = GetAlmostRecently();

            GetNotificationData();

            //Debug.Log("almostHours: " + almostHours);

            //if (instance != null)
            //{
            //    Destroy(gameObject);
            //    return;
            //}

            //instance = this;
            //DontDestroyOnLoad(this);
            //Debug.Log("try to create notification crossplatform");

            //#if !UNITY_EDITOR
#if UNITY_ANDROID
            if (enableNotify)
            {
                Debug.Log("enable notification");

                CancelAllNotification();
                SetupNewNotification();

                // For test
                //NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds(5), "Notification", "Notification with app icon", new Color(0f, 0.6f, 1f), NotificationIcon.Message);
                //TestNotify();

                Debug.Log("Created notification crossplatform android");
            }
#endif
#if UNITY_IOS
            RegisterForNotificationIOS();
            Debug.Log("Register notification crossplatform on IOS");
#endif
            //#endif
        }

        void TestNotify()
        {
            ScheduleLocalNotification("Title", "Content", 15);
        }

        public void SendNotification(int id, string title, string content, string small_icon, string big_icon,
            long delay, bool hasSound = false, bool hasVibrate = false)
        {
            if (!enableNotify)
            {
                return;
            }

            // For test
            if (delay <= 0)
            {
                Debug.Log("can't start a notification with time <=0");
                return;
            }

            ScheduleLocalNotification(title, content, delay);

            //NotificationIcon icon = NotificationIcon.Bell;
            //TimeSpan timespan = new TimeSpan(delay * TimeSpan.TicksPerMillisecond);
            //NotificationManager.SendWithAppIcon(
            //    timespan,
            //    title,
            //    content,
            //    new Color(0, 0.6f, 1),
            //    icon);
        }

        public void ScheduleLocalNotification(string title, string content, double delay)
        {
#if UNITY_ANDROID
            //Debug.Log("ScheduleLocalNotification 1");

            NotificationIcon[] icons = new []
            {
                NotificationIcon.Bell,
                NotificationIcon.Star,
                NotificationIcon.Event,
                NotificationIcon.Heart,
            };

            var icon = icons[UnityEngine.Random.Range(0, icons.Length)];
            int id = NotificationManager.SendWithAppIcon(
                TimeSpan.FromSeconds(delay),
                title,
                content,
                new Color(0, 0.6f, 1),
                icon);

            //Debug.Log("ScheduleLocalNotification 2 : id : " + id.ToString());
#endif
        }

        public void CancelNotification(int id)
        {

        }

        public void SetupNewNotification()
        {
            foreach (NotificationItem item in notifyItems)
            {
                SetupNotify(item);
            }
        }

        public void SetupNotify(NotificationItem item)
        {
            switch (item.notificationType)
            {
                case NotificationType.Once:
                    SetupNotificationPlayOnce(item);
                    break;
                case NotificationType.Repeat:
                    try
                    {
                        SetupNotificationPlayMultipleTime(item);
                    }
                    catch { };
                    break;
            }

        }

        private void SetupNotificationPlayOnce(NotificationItem item)
        {
            long time = 0;
            TimeSpan span;
            DateTime today = DateTime.Today;

            switch (item.notificationTimeType)
            {
                case NotificationTimeType.DelayDate:
                    //DateTime currentDate = DateTime.Today;
                    //span = new TimeSpan(item.delayDate, item.exactHour, item.exactMinute, item.exactSecond);				
                    today =
 today.AddDays(item.delayDate).AddHours(almostHours).AddMinutes(item.exactMinute).AddSeconds(item.exactSecond);
                    time = (long)today.Subtract(DateTime.Now).TotalSeconds;
                    Debug.Log(time);
                    break;
                case NotificationTimeType.DelayTime:
                    time = item.delayTime;
                    break;
                case NotificationTimeType.SpecificDate:
                    DateTime targetDate = DateTime.ParseExact(item.exactDate, "dd/mm/yyyy", CultureInfo.CurrentCulture);
                    span = new TimeSpan(0, almostHours, item.exactMinute, item.exactSecond);
                    targetDate = targetDate.Add(span);
                    span = targetDate.Subtract(DateTime.Now);
                    time = (long)span.TotalSeconds;
                    break;
                case NotificationTimeType.DelayDateInWeek:
                    DateTime currentDate = DateTime.Today;
                    int diff = item.dateInWeek - currentDate.DayOfWeek;
                    if (diff < 0)
                    {
                        diff += 7;
                    }
                    today =
 today.AddDays(diff).AddHours(almostHours).AddMinutes(item.exactMinute).AddSeconds(item.exactSecond);
                    span = today.Subtract(DateTime.Now);
                    time = (long)span.TotalSeconds;
                    break;
            }
#if UNITY_ANDROID
            //SendNotification(item.id, item.title, item.content[UnityEngine.Random.Range(0, item.content.Length)], 
            //    item.small_icon, item.big_icon, time, item.hasSound);

            SendNotification(item.id, 
                (RemoteConfigKey.notify_day_title).GetValueString()
                ,(RemoteConfigKey.notify_day_content).GetValueString(),
                item.small_icon, item.big_icon, time, item.hasSound);
#endif


#if UNITY_IOS
            DateTime dateTime = DateTime.Now.AddSeconds(time); // new DateTime(time * 10L * 1000L);
            ScheduleNotificationIOS((RemoteConfigKey.notify_day_title).StringValue, (RemoteConfigKey.notify_day_content).StringValue,
                                    dateTime, (item.hasSound && item.hasVibrate));
#endif
        }



        private void SetupNotificationPlayMultipleTime(NotificationItem item)
        {
            long time = 0;
            TimeSpan span;
            DateTime today = DateTime.Today;

            switch (item.notificationTimeType)
            {
                case NotificationTimeType.DelayDate:
                    for (int i = 0; i < item.repeatTimes; i++)
                    {
                        today = DateTime.Today;

                        //if ((int)((int)(today.DayOfWeek + i + 1) % 7) == (int)DayOfWeek.Saturday)
                        //{
                        //    continue;
                        //}

                        //today = today.AddDays(item.delayDate * (i + 1)).AddHours(almostHours).AddMinutes(item.exactMinute).AddSeconds(item.exactSecond);

                        today = today.AddDays(item.delayDate * (i + 1));
                        if((today.DayOfWeek == DayOfWeek.Saturday) || (today.DayOfWeek == DayOfWeek.Sunday))
                        {
                            today = today.AddHours(16); // 16h
                        }
                        else
                        {
                            int rand = UnityEngine.Random.Range(0, 2);
                            if(rand == 1)
                            {
                                today = today.AddHours(8).AddMinutes(30);
                            }
                            else
                            {
                                today = today.AddHours(20).AddMinutes(30);
                            }
                        }

                        NotifyItemInfo info = notifyItemInfoList[UnityEngine.Random.Range(0, notifyItemInfoList.Count)];

                        time = (long)today.Subtract(DateTime.Now).TotalSeconds;
#if UNITY_ANDROID
                        SendNotification(item.id + repeatitionID * i, info.title, info.content,
                            item.small_icon, item.big_icon, time, item.hasSound, item.hasVibrate);
#endif

#if UNITY_IOS
                        DateTime dateTime = DateTime.Now.AddSeconds(time); // new DateTime(time * 10L * 1000L);
                        //Debug.Log("dateTime: " + dateTime.ToString());
                        ScheduleNotificationIOS(info.title, info.content, 
                            dateTime, (item.hasSound && item.hasVibrate));
#endif
                    }
                    break;
                case NotificationTimeType.DelayTime:
                    for (int i = 0; i < item.repeatTimes; i++)
                    {
                        time = item.delayTime * (i + 1);

                        //Debug.Log("item.content[UnityEngine.Random.Range(0, item.content.Length)]: " + item.content[UnityEngine.Random.Range(0, item.content.Length)]);

#if UNITY_ANDROID
                        SendNotification(item.id + repeatitionID * i,
                            (RemoteConfigKey.notify_day_title).GetValueString()
                            ,(RemoteConfigKey.notify_day_content).GetValueString(),
                            item.small_icon, item.big_icon, time, item.hasSound);
#endif

#if UNITY_IOS
                        DateTime dateTime = DateTime.Now.AddSeconds(time); // new DateTime(time * 10L * 1000L);

                        ScheduleNotificationIOS(
     (RemoteConfigKey.notify_day_title).StringValue
     , (RemoteConfigKey.notify_day_content).StringValue,
    dateTime, (item.hasSound && item.hasVibrate));

                        //Debug.Log("dateTime: " + dateTime.ToString());
              //          ScheduleNotificationIOS(item.title, item.content[UnityEngine.Random.Range(0, item.content.Length)], 
              //              dateTime, (item.hasSound && item.hasVibrate));
#endif
                    }
                    break;
                case NotificationTimeType.DelayDateInWeek:
                    DateTime currentDate = DateTime.Today;
                    int diff = item.dateInWeek - currentDate.DayOfWeek;
                    int remain = diff;
                    if (diff < 0)
                    {
                        // For next week
                        diff += 7;
                    }
                    for (int i = 0; i < item.repeatTimes; i++)
                    {
                        // For next week
                        //if ((remain % 3) == 0)
                        //{
                        //    remain += 7;
                        //    continue;
                        //}

                        today = DateTime.Today;
                        today =
 today.AddDays(diff + i * 7).AddHours(almostHours).AddMinutes(item.exactMinute).AddSeconds(item.exactSecond);
                        span = today.Subtract(DateTime.Now);
                        time = (long)span.TotalSeconds;

#if UNITY_ANDROID
                        SendNotification(item.id + repeatitionID * i, 
                            (RemoteConfigKey.notify_week_title).GetValueString()
                            ,(RemoteConfigKey.notify_week_content).GetValueString(),
                            item.small_icon, item.big_icon, time, item.hasSound);
#endif

#if UNITY_IOS
                        DateTime dateTime = DateTime.Now.AddSeconds (time); // new DateTime(time * 10L * 1000L);


                        ScheduleNotificationIOS(
             (RemoteConfigKey.notify_week_title).StringValue
             , (RemoteConfigKey.notify_week_content).StringValue,
            dateTime, (item.hasSound && item.hasVibrate));

                        //Debug.Log("dateTime: " + dateTime.ToString());
                //        ScheduleNotificationIOS(item.title, item.content[UnityEngine.Random.Range(0, item.content.Length)], 
               //             dateTime, (item.hasSound && item.hasVibrate));
#endif
                    }
                    break;
            }
        }

        public void OnApplicationPause(bool paused)
        {
            //Debug.Log("paused is: " + paused);

            // Comment for test
            //#if UNITY_ANDROID
            //            if(notification != null)
            //            {
            //                notification.OnApplicationPause(paused);
            //            }
            //#endif

#if UNITY_IOS
            if (paused)
            {
                if (enableNotify)
                {
                    CancelAllNotificationIOS();
                    SetupNewNotification
                      ();
                }
            }
            else
            {
                CancelAllNotificationIOS();
            }
#endif
        }

        public void CancelAllNotification()
        {
#if UNITY_ANDROID
            try
            {
                NotificationManager.CancelAll();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
#elif UNITY_IOS
		UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
#endif
        }

        #region For IOS
#if UNITY_IOS
        void RegisterForNotificationIOS()
        {
            UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Badge);
        }

        void ScheduleNotificationIOS(string title, string content, DateTime dateTime,
            bool hasAction = false, bool hasSound = false, bool hasVibrate = false)
        {

            Debug.Log(dateTime);
            UnityEngine.iOS.LocalNotification notif = new UnityEngine.iOS.LocalNotification();

            notif.alertAction = title;
            notif.alertBody = content;
            notif.fireDate = dateTime;
            notif.hasAction = hasAction;

            UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notif);
        }

        void CancelAllNotificationIOS()
        {
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();

            UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
        }
#endif
        #endregion
    }


#endif