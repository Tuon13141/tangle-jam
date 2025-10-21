using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class MobileNotification : MonoBehaviour
{
    private readonly int morningNotificationID = 0;
    private readonly int middayNotificationID = 1;
    private readonly int eveningNotificationID = 2;

    private readonly string channelGroupID = "percas_thread_frenzy_channel_group_id";
    private readonly string channelGroupName = "Thread Frenzy Notifications";
    private readonly string channelID = "percas_thread_frenzy_channel_id";
    private readonly string channelName = "percas_thread_frenzy_channel_name";
    private readonly string channelDesc = "percas_thread_frenzy_channel_desc";
    private readonly string iOSID = "percas_thread_frenzy_ios_id";
    private readonly string iOSCategoryID = "percas_thread_frenzy_ios_category_id";
    private readonly string iOSThreadID = "percas_thread_frenzy_ios_thread_id";

    private readonly List<string> notificationTitles = new()
    {
        "Morning Start",
        "Midday Break",
        "Evening Relax"
    };

    private readonly List<string> notificationTexts = new()
    {
        "Good morning! Grab your yarn and color your first picture of the day!",
        "Relax at lunch with a fresh yarn puzzle to weave.",
        "Wrap up your day—fill your canvas with colorful yarn!",
    };

    private void Start()
    {
#if UNITY_ANDROID
        CreateNotificationChannel();
#elif UNITY_IOS
        StartCoroutine(IOSRequestAuthorization());
#endif
        SendAllNotifications();
    }

    // Gợi ý: tạo identifier riêng cho iOS theo ID
    private string GetIOSIdentifier(int id)
    {
        return $"{iOSID}_{id}";
    }

#if UNITY_ANDROID
    private void CreateNotificationChannel()
    {
        var group = new AndroidNotificationChannelGroup()
        {
            Id = channelGroupID,
            Name = channelGroupName,
        };
        AndroidNotificationCenter.RegisterNotificationChannelGroup(group);
        var channel = new AndroidNotificationChannel()
        {
            Id = channelID,
            Name = channelName,
            Importance = Importance.High,
            Description = channelDesc,
            Group = channelGroupID,
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }
#endif

#if UNITY_IOS
    private IEnumerator IOSRequestAuthorization()
    {
        // Có thể thêm Sound để chắc chắn:
        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound;
        using var req = new AuthorizationRequest(authorizationOption, true);
        while (!req.IsFinished) yield return null;
        string res = "\n RequestAuthorization:";
        res += "\n finished: " + req.IsFinished;
        res += "\n granted :  " + req.Granted;
        res += "\n error:  " + req.Error;
        res += "\n deviceToken:  " + req.DeviceToken;
        Debug.Log(res);
    }
#endif

    private void OnSendNotification(int notificationID, int notificationFireTime)
    {
#if UNITY_ANDROID
        AndroidNotification androidNotification = new()
        {
            SmallIcon = "small_icon",
            LargeIcon = "large_icon",
            Title = notificationTitles[notificationID],
            Text = notificationTexts[notificationID],
        };
        DateTime fireTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, notificationFireTime, 0, 0);
        if (fireTime < DateTime.Now)
        {
            androidNotification.FireTime = fireTime + new TimeSpan(24, 0, 0);
        }
        else
        {
            androidNotification.FireTime = fireTime;
        }
        androidNotification.RepeatInterval = new TimeSpan(24, 0, 0);
        NotificationStatus notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationID);
        if (notificationStatus == NotificationStatus.Scheduled)
        {
            AndroidNotificationCenter.UpdateScheduledNotification(notificationID, androidNotification, channelID);
        }
        else if (notificationStatus == NotificationStatus.Delivered)
        {
            AndroidNotificationCenter.CancelNotification(notificationID);
        }
        else
        {
            AndroidNotificationCenter.SendNotificationWithExplicitID(androidNotification, channelID, notificationID);
        }
#elif UNITY_IOS
        // Tạo noti iOS với Identifier RIÊNG
        iOSNotification iOSNotification = new()
        {
            Identifier = GetIOSIdentifier(notificationID),
            Title = notificationTitles[notificationID],
            Body = notificationTexts[notificationID],
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = iOSCategoryID,
            ThreadIdentifier = iOSThreadID,
        };
        iOSNotificationCalendarTrigger calendarTrigger = new()
        {
            Hour = notificationFireTime,
            Minute = 0,
            Second = 0,
            Repeats = true,
        };
        iOSNotification.Trigger = calendarTrigger;
        iOSNotificationCenter.ScheduleNotification(iOSNotification);
#endif
    }

    private void SendAllNotifications()
    {
        SendMorningNotification();
        SendMiddayNotification();
        SendEveningNotification();
    }

    private void SendMorningNotification()
    {
        OnSendNotification(morningNotificationID, 8);
    }

    private void SendMiddayNotification()
    {
        OnSendNotification(middayNotificationID, 12);
    }

    private void SendEveningNotification()
    {
        OnSendNotification(eveningNotificationID, 20);
    }
}
