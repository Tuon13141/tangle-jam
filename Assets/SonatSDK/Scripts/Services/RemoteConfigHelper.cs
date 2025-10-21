using System;
using UnityEngine;

public static class RemoteConfigHelper
{
    public static Double GetValueDouble(this RemoteConfigKey key)
    {
        if ((SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched ||
             SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.FetchedFail)
            && RemoteConfigController.RemoteHasValue(key))
            return RemoteConfigController.GetValue(key).DoubleValue;

        
        var defaultConfig = Kernel.Resolve<FireBaseController>().remoteConfigController.GetDefault(key);
        if (defaultConfig == null)
        {
#if DISPLAY_LOG
            UIDebugLog.LogError("err : not found this default config :" + key);
#endif
            return 0;
        }
        return defaultConfig.GetDefaultFloat(true);
    }

    public static string GetValueString(this RemoteConfigKey key, bool log = true)
    {
        if (SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched &&
            RemoteConfigController.RemoteHasValue(key))
            return RemoteConfigController.GetValue(key).StringValue;

        var defaultConfig = Kernel.Resolve<FireBaseController>().remoteConfigController.GetDefault(key, log);
        if (defaultConfig == null)
        {
#if DISPLAY_LOG
            UIDebugLog.LogError("err : not found this default config :" + key);
#endif
            return String.Empty;
        }
        return defaultConfig.GetDefaultString(true);
    }

    public static bool GetValueBoolean(this RemoteConfigKey key, bool defaultValue = false)
    {
        if ((SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched ||
             SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.FetchedFail)
            && RemoteConfigController.RemoteHasValue(key))
            return RemoteConfigController.GetValue(key).BooleanValue;

        var defaultConfig = Kernel.Resolve<FireBaseController>().remoteConfigController.GetDefault(key);
        if (defaultConfig == null)
        {
#if DISPLAY_LOG
            UIDebugLog.LogError("err : not found this default config :" + key);
#endif
            return defaultValue;
        }
        return defaultConfig.GetDefaultBoolean(true);
    }

    public static long GetValueInt(this RemoteConfigKey key, long defaultValue = 0)
    {
        if ((SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched ||
             SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.FetchedFail)
            && RemoteConfigController.RemoteHasValue(key))
            return RemoteConfigController.GetValue(key).LongValue;

        var defaultConfig = Kernel.Resolve<FireBaseController>().remoteConfigController.GetDefault(key);
        if (defaultConfig == null)
        {
#if DISPLAY_LOG
            UIDebugLog.LogError("err : not found this default config :" + key);
#endif
            return defaultValue;
        }
        return defaultConfig.GetDefaultInt(true);
    }

    //

    public static Double GetValueDouble(string key)
    {
        if ((SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched ||
             SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.FetchedFail)
            && RemoteConfigController.RemoteHasValue(key))
            return RemoteConfigController.GetValue(key).DoubleValue;

        var defaultConfig = Kernel.Resolve<FireBaseController>().remoteConfigController.GetDefault(key);
        if (defaultConfig == null)
        {
#if DISPLAY_LOG
            UIDebugLog.LogError("err : not found this default config :" + key);
#endif
            if (PlayerPrefs.HasKey($"remote_value_{key}"))
            {
                return PlayerPrefs.GetFloat($"remote_value_{key}");
            }
            return 0;
        }
        return defaultConfig.GetDefaultFloat(true);
    }

    public static string GetValueString(string key)
    {
        if ((SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched ||
             SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.FetchedFail)
            && RemoteConfigController.RemoteHasValue(key))
            return RemoteConfigController.GetValue(key).StringValue;

        var defaultConfig = Kernel.Resolve<FireBaseController>().remoteConfigController.GetDefault(key);
        if (defaultConfig == null)
        {
#if DISPLAY_LOG
            UIDebugLog.LogError("err : not found this default config :" + key);
#endif
            if (PlayerPrefs.HasKey($"remote_value_{key}"))
            {
                return PlayerPrefs.GetString($"remote_value_{key}");
            }
            return "";
        }
        return defaultConfig.GetDefaultString(true);
    }

    public static bool GetValueBoolean(string key)
    {
        if ((SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched ||
             SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.FetchedFail)
            && RemoteConfigController.RemoteHasValue(key))
            return RemoteConfigController.GetValue(key).BooleanValue;
        
        var defaultConfig = Kernel.Resolve<FireBaseController>().remoteConfigController.GetDefault(key);
        if (defaultConfig == null)
        {
#if DISPLAY_LOG
            UIDebugLog.LogError("err : not found this default config :" + key);
#endif
            return false;
        }
        return defaultConfig.GetDefaultBoolean(true);
    }

    public static long GetValueInt(string key,int defaultValue = 0)
    {
        if ((SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched ||
             SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.FetchedFail)
            && RemoteConfigController.RemoteHasValue(key))
            return RemoteConfigController.GetValue(key).LongValue;
        
        var defaultConfig = Kernel.Resolve<FireBaseController>().remoteConfigController.GetDefault(key);
        if (defaultConfig == null)
        {
#if DISPLAY_LOG
            UIDebugLog.LogError("err : not found this default config :" + key);
#endif
			if (PlayerPrefs.HasKey($"remote_value_{key}"))
			{
                return PlayerPrefs.GetInt($"remote_value_{key}");
			}
            return defaultValue;
        }
        return defaultConfig.GetDefaultInt(true);
    }

    public static bool HasKey(string key)
    {
        return (SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched ||
                SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.FetchedFail)
               && RemoteConfigController.RemoteHasValue(key);
    }
}