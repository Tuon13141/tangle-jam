#define dummy
//#define use_firebase

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;
#if !((dummy || global_dummy) && !use_firebase)
using Firebase;
using Firebase.RemoteConfig;
using Firebase.Extensions;
using Sonat;

#endif
// ReSharper disable InconsistentNaming

public abstract class SharedRemoteConfigController : MonoBehaviour
{
    public static ConfigFetchStatus FetchStatus = ConfigFetchStatus.NotStarted;
    public BooleanAction Ready = new BooleanAction();
    public static readonly BooleanAction OnInitialized = new BooleanAction();

    [SerializeField] protected DefaultRemoteDatabase defaultRemoteDatabase;

    [ArrayElementTitleSonatSdk(nameof(RemoteConfigDefaultByKey.key))] 
    [SerializeField]  private List<RemoteConfigDefaultByKey> defaultConfigs = new List<RemoteConfigDefaultByKey>();

    [ArrayElementTitleSonatSdk(nameof(RemoteConfigDefaultByKey.key))] [SerializeField]
    private List<RemoteConfigDefaultByString> defaultConfigs2 = new List<RemoteConfigDefaultByString>();

    private List<BaseRemoteConfigDefault> _allDefaultConfigs;

    public FireBaseDefaultSetting setting => defaultRemoteDatabase.setting;

    protected List<BaseRemoteConfigDefault> AllDefaultConfigs
    {
        get
        {
            if (_allDefaultConfigs == null)
            {
                _allDefaultConfigs = new List<BaseRemoteConfigDefault>();
                _allDefaultConfigs.AddRange(defaultConfigs);
                _allDefaultConfigs.AddRange(GetDistinctDefaultByKernelConfig());
                _allDefaultConfigs.AddRange(GetDefaultConfig2CheckNull());

#if UNITY_EDITOR
                for (var i = 0; i < _allDefaultConfigs.Count; i++)
                for (var i1 = i + 1; i1 < _allDefaultConfigs.Count; i1++)
                    if (_allDefaultConfigs[i].GetKey() == _allDefaultConfigs[i1].GetKey())
                    {
                        Debug.LogError("duplicate " + _allDefaultConfigs[i].GetKey());
                    }
#endif
            }

            return _allDefaultConfigs;
        }
    }

    private IEnumerable<RemoteConfigDefaultByString> GetDefaultConfig2CheckNull()
    {
        foreach (var remoteConfigDefaultByString in defaultConfigs2)
        {
            if (remoteConfigDefaultByString.dataType == DataType.Json)
            {
                if (remoteConfigDefaultByString.jsonTextAsset != null)
                   yield return  remoteConfigDefaultByString;
            }
            else
                yield return  remoteConfigDefaultByString;
        }
    }

    private IntByLevelCollectionRemote[] listIntCollection;
    private StringByLevelCollectionRemote[] listStringCollection;


    public BaseRemoteConfigDefault GetDefault(RemoteConfigKey key,bool log = true)
    {
        var find = AllDefaultConfigs.Find(x => x.GetKey() == key.ToString());
        if (find == null && log)
            UIDebugLog.LogWarning("not found remote config " + key + "(so it  also cannot  receive playerpref value from remote, please set it)");

        return find;
    }

    public BaseRemoteConfigDefault GetDefault(string key)
    {
        BaseRemoteConfigDefault find = AllDefaultConfigs.Find(x => x.GetKey() == key);
        if (find == null)
        {
            find = defaultConfigs2.Find(x => x.key.ToString() == key);
            if (find == null)
                UIDebugLog.LogWarning("not found remote config " + key);
        }

        return find;
    }

    private IEnumerable<BaseRemoteConfigDefault> GetDistinctDefaultByKernelConfig()
    {
        if (defaultRemoteDatabase != null)
            foreach (var defaultInKernel in defaultRemoteDatabase.defaultConfigs)
            {
                bool found = false;
                foreach (var defaultByProject in defaultConfigs)
                    if (defaultByProject.key == defaultInKernel.key)
                    {
                        found = true;
                        break;
                    }

                if (!found)
                    yield return defaultInKernel;
            }
    }

    public virtual void ResolveDataByLevel(RemoteConfigKey[] intKeys, RemoteConfigKey[] stringKeys)
    {
        listIntCollection = new IntByLevelCollectionRemote[intKeys.Length];
        for (var i = 0; i < listIntCollection.Length; i++)
        {
            listIntCollection[i] = new IntByLevelCollectionRemote(intKeys[i]);
            try
            {
                listIntCollection[i].Load();
            }
            catch (Exception e)
            {
                Kernel.Resolve<FireBaseController>().LogCrashException(e);
                Debug.LogError(e);
                Debug.LogError(listStringCollection[i].GetKey() + ":"+listStringCollection[i].GetString());
            }
        }

        listStringCollection = new StringByLevelCollectionRemote[stringKeys.Length];
        for (var i = 0; i < listStringCollection.Length; i++)
        {
            listStringCollection[i] = new StringByLevelCollectionRemote(stringKeys[i]);
            try
            {
                listStringCollection[i].Load();
            }
            catch (Exception e)
            {
                Kernel.Resolve<FireBaseController>().LogCrashException(e);
                Debug.LogError(e);
                Debug.LogError(listStringCollection[i].GetKey() + ":"+listStringCollection[i].GetString());
            }
        }
    }

    public int GetValueByLevel(RemoteConfigKey collectionKey, int defaultValue)
    {
        int value = defaultValue;
        var collection = GetIntCollectionByKey(collectionKey);
        if (collection?.data != null)
            for (var i = 0; i < collection.data.Length; i++)
            {
                if (FireBaseController.UserPropertyLevel >= collection.data[i].key)
                    value = collection.data[i].value;
                else
                    break;
            }

        return value;
    }

    public int GetValueByLevel(RemoteConfigKey collectionKey, int level, int defaultValue)
    {
        int value = defaultValue;
        var collection = GetIntCollectionByKey(collectionKey);
        if (collection?.data != null)
            for (var i = 0; i < collection.data.Length; i++)
            {
                if (level >= collection.data[i].key)
                    value = collection.data[i].value;
                else
                    break;
            }

        return value;
    }


    public string GetValueByLevel(RemoteConfigKey collectionKey, string defaultValue)
    {
        string value = defaultValue;
        var collection = GetStringCollectionByKey(collectionKey);
        if (collection?.data != null)
            for (var i = 0; i < collection.data.Length; i++)
                if (FireBaseController.UserPropertyLevel >= collection.data[i].key)
                    value = collection.data[i].value;
                else
                    break;

        return value;
    }

    public string GetValueByLevel(RemoteConfigKey collectionKey, int level, string defaultValue)
    {
        string value = defaultValue;
        var collection = GetStringCollectionByKey(collectionKey);
        if (collection?.data != null)
            for (var i = 0; i < collection.data.Length; i++)
                if (level >= collection.data[i].key)
                    value = collection.data[i].value;
                else
                    break;

        return value;
    }

    private StringByLevelCollectionRemote GetStringCollectionByKey(RemoteConfigKey key)
    {
        if(listStringCollection != null)
            foreach (var collection in listStringCollection)
                if (collection.key == key)
                    return collection;

        return null;
    }

    private IntByLevelCollectionRemote GetIntCollectionByKey(RemoteConfigKey key)
    {
        if(listStringCollection != null)
            foreach (var collection in listIntCollection)
                if (collection.key == key)
                    return collection;

        return null;
    }
}

#if !((dummy || global_dummy) && !use_firebase)

public class RemoteConfigController : SharedRemoteConfigController
{

    protected static FirebaseRemoteConfig firebaseRemote => FirebaseRemoteConfig.DefaultInstance;

    private void SetDefaults(Dictionary<string, object> injectDict)
    {
        var dict = injectDict ?? new Dictionary<string, object>();


        for (var index = 0; index < AllDefaultConfigs.Count; index++)
        {
            var item = AllDefaultConfigs[index];
            var key = item.GetKey();
            if (dict.ContainsKey(key))
            {
                Debug.LogError("duplicate key " + key + " at "+index);
                Debug.Break();
            }

            switch (item.dataType)
            {
                case DataType.String:
                    dict.Add(key, item.DefaultStringValue);
                    break;
                case DataType.Int:
                    dict.Add(key, item.DefaultLongValue);
                    break;
                case DataType.Float:
                    dict.Add(key, item.DefaultDoubleValue);
                    break;
                case DataType.Boolean:
                    dict.Add(key, item.DefaultBooleanValue);
                    break;
                case DataType.Json:
                    dict.Add(key, item.jsonTextAsset.text);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        firebaseRemote.SetDefaultsAsync(dict);
    }


    public void InitRemoteConfig(Dictionary<string, object> defaultConfig = null)
    {
        SetDefaults(defaultConfig);
        FetchData(true);
    }

    private void FetchData(bool first)
    {
        FetchDataAsync(first);
    }

//    public void FetchDataAgain()
//    {
//        if (FireBaseController.FireBaseRemoteReady)
//        {
//            FetchData(false);
//            UIDebugLog.Log("Fetch Remote Agian");
//        }
//    }

    private int fetchTime;

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            int seg = Mathf.FloorToInt(Time.realtimeSinceStartup / 300f);
            if (seg > fetchTime)
            {
                fetchTime = seg;
                FetchData(!FireBaseController.FireBaseRemoteReady);
            }
        }
    }


    Task FetchDataAsync(bool first)
    {
        if (first)
            FetchStatus = ConfigFetchStatus.Fetching;
        Task fetchTask = firebaseRemote.FetchAsync(
            TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }


    void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {
            Debug.Log("Fetch canceled.");
        }
        else if (fetchTask.IsFaulted)
        {
            Debug.Log("Fetch encountered an error.");
            FetchStatus = ConfigFetchStatus.FetchedFail;
        }
        else if (fetchTask.IsCompleted)
        {
            Debug.Log("Fetch completed successfully!");
            FetchStatus = ConfigFetchStatus.Fetched;
        }

        var info = firebaseRemote.Info;

        switch (info.LastFetchStatus)
        {
            case LastFetchStatus.Success:
                firebaseRemote.ActivateAsync();
                FireBaseController.FireBaseRemoteReady = true;
             
                OnInitialized.Action.Invoke(true);
                OnInitialized.Destroy();
                foreach (var baseRemoteConfigDefault in AllDefaultConfigs)
                    baseRemoteConfigDefault.SavePlayerPref();
                Debug.Log($"Remote data loaded and ready (last fetch time {info.FetchTime}).");
                ResolveDataByLevel(defaultRemoteDatabase.listIntCollectionKeys,
                    defaultRemoteDatabase.listStringCollectionKeys);
//                Debug.LogError(firebaseRemote.GetValue(RemoteConfigKey.display_list_view_ads.ToString()).StringValue);
//                Debug.LogError(firebaseRemote.GetValue(RemoteConfigKey.display_list_view_ads.ToString()).BooleanValue);
//                Debug.LogError(FireBaseController.GetBooleanValue(RemoteConfigKey.display_list_view_ads));
                break;
            case LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");
                        break;
                    case FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }

                break;
            case LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }


        Ready.Action.Invoke(true);
        Kernel.Resolve<AppFlyerController>().SendEvent("LaunchApp");
        Kernel.Resolve<FireBaseController>().LogEvent("app_loading");
    }

    public static bool RemoteHasValue(RemoteConfigKey key)
    {
        if (!(FetchStatus == ConfigFetchStatus.Fetched || FetchStatus == ConfigFetchStatus.FetchedFail))
            return false;
        var config = GetValue(key);
        if (string.IsNullOrEmpty(config.StringValue))
            return false;
        return true;
    }

    public static bool RemoteHasValue(string key)
    {
        if (!(FetchStatus == ConfigFetchStatus.Fetched || FetchStatus == ConfigFetchStatus.FetchedFail))
            return false;
        var config = GetValue(key);
        if (string.IsNullOrEmpty(config.StringValue))
            return false;
        return true;
    }

    public static ConfigValue GetValue(RemoteConfigKey key)
    {
        if (!(FetchStatus == ConfigFetchStatus.Fetched || FetchStatus == ConfigFetchStatus.FetchedFail))
        {
            Debug.LogError("don't call if not initialized'");
        }

        return GetValue(key.ToString());
    }

    public static ConfigValue GetValue(string key)
    {
        return firebaseRemote.GetValue(key);
    }
}

#else
public class RemoteConfigController : SharedRemoteConfigController
{
    public static bool RemoteHasValue(RemoteConfigKey key)
    {
        return false;
    }

    public static bool RemoteHasValue(string key)
    {
        return false;
    }

    public static BaseRemoteConfigDefault GetValue(RemoteConfigKey key)
    {
        return Kernel.Resolve<FireBaseController>().remoteConfigController.GetDefault(key);
    }

    public static BaseRemoteConfigDefault GetValue(string key)
    {
        return Kernel.Resolve<FireBaseController>().remoteConfigController.GetDefault(key);
    }

}
#endif