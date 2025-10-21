using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
#if dummy_log
public static class SonatLogRecursive
{

}
#endif
#if !dummy_log
using AppsFlyerSDK;
using Firebase.Analytics;

// ReSharper disable InconsistentNaming

namespace Sonat
{
    [System.Serializable]
public class IntegerSerializableDictionary2 : BaseSerializableDictionary2
{
    private int _currentValue;
    private readonly int _defaultValue;
    public List<int> items = new List<int>();
    private Dictionary<int, int> _dictionary = new Dictionary<int, int>();

    public int Count => items.Count;

    public int CountValue => items.Sum();

    public int Max => keys.Count == 0 ? -1 : keys.Max();

    public string Key { get; set; }

    public void Save()
    {
        if (!string.IsNullOrEmpty(Key))
            PlayerPrefs.SetString(Key, JsonUtility.ToJson(this));
    }
    
    public static IntegerSerializableDictionary2 Load(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            try
            {
                var load = JsonUtility.FromJson<IntegerSerializableDictionary2>(PlayerPrefs.GetString(key));
                load.Key = key;
                return load;
            }
            catch
            {
            }
        }
        var newDict = new IntegerSerializableDictionary2();
        newDict.Key = key;
        return newDict;
    }
    
    public IntegerSerializableDictionary2()
    {
    }

    public IntegerSerializableDictionary2(int currValue, int defaultVal = -1)
    {
        _currentValue = currValue;
        _defaultValue = defaultVal;
    }

    public int Current
    {
        get => _currentValue;
        set
        {
            if (_currentValue != value)
            {
                _currentValue = value;
            }
        }
    }

    // id/index
    // value of subject index
    public int this[int key]
    {
        get
        {
            if (Exist(key))
                return items[_dictionary[key]];

            return _defaultValue;
        }
        set
        {
            if (Exist(key))
            {
                if (items[_dictionary[key]] != value)
                {
                    items[_dictionary[key]] = value;
                }
            }
            else
            {
                keys.Add(key);
                items.Add(value);
                _dictionary.Add(key, keys.IndexOf(key));
            }
        }
    }

    public override bool Exist(int key)
    {
        if (_dictionary.Count > items.Count)
        {
            Debug.LogError("What the fuck 2");
            //Debug.LogError(key + "_dictionary[key]" + _dictionary[key] + " Items" + Items.Count);
            items.ForEach(x => Debug.Log("Item" + x));
            keys.ForEach(x => Debug.Log("Key" + x));
            foreach (var keyValuePair in _dictionary)
            {
                Debug.Log(keyValuePair.Key + "/" + keyValuePair.Value);
            }

            Debug.Break();
        }

        if (_dictionary.ContainsKey(key))
            return true;
        // for existing
        var index = keys.IndexOf(key);
        if (index >= 0)
        {
            _dictionary.Add(key, index);
            return true;
        }

        return false;
    }


    public IntegerSerializableDictionary2 Clone()
    {
        return new IntegerSerializableDictionary2()
        {
            keys = keys.ToList(),
            items = items.ToList(),
        };
    }

    public override void ClearDict()
    {
        keys.Clear();
        items.Clear();
        _dictionary = new Dictionary<int, int>();
    }

    public int Sum() => items.Sum();

    public bool AnyValue()
    {
        return items.Sum() > 0;
    }
}

[System.Serializable]
public abstract class BaseSerializableDictionary2
{
    [SerializeField]
    protected List<int> keys = new List<int>();
    public abstract void ClearDict();

    public List<int> Keys => keys;

    public virtual bool Exist(int key)
    {
        return keys.Contains(key);
    }
}

    
    public static class SonatLogRecursive
    {
        private static PlayerPrefRemoteArrayInt logStartLevel;
        private static PlayerPrefListInt startLevelLogged;
    

        public static void Log(SonatLogLevelStart levelStart)
        {
            if (logStartLevel == null)
            {
                var fireBase = Kernel.Resolve<FireBaseController>();
                logStartLevel = fireBase.remoteConfigController.setting.LogLevelStart;
            }

            if (startLevelLogged == null)
                startLevelLogged = new PlayerPrefListInt("start_level_logged", new List<int>());

            var levelInt = levelStart.level;
            foreach (var level in logStartLevel.Value.ToArray())
            {
                if (levelInt == level && !startLevelLogged.Contains(levelInt))
                {
                    startLevelLogged.AddDistinct(levelInt);
                    var logEvent = $"{levelStart.mode}_start_level_{(levelInt):D4}";
                    new CustomSonatLog(logEvent, new List<LogParameter>()).Post(true);
                    
#if UNITY_EDITOR
                    Debug.LogError("Log "+logEvent);
#endif
                    break;
                }
            }
        }

        // count inters
        private static PlayerPrefRemoteArrayInt logStartInterstitial;
        private static PlayerPrefListInt startInterstitialLogged;
        private static readonly PlayerPrefInt countInterstitial = new PlayerPrefInt("count_interstitial_showed");
        public static void Log(SonatLogShowInterstitial interstitialLog)
        {
            if (logStartInterstitial == null)
            {
                var fireBase = Kernel.Resolve<FireBaseController>();
                logStartInterstitial = fireBase.remoteConfigController.setting.LogInterstitialAdsStart;
            }

            if (startInterstitialLogged == null)
                startInterstitialLogged = new PlayerPrefListInt("start_interstitial_logged", new List<int>());
            
            countInterstitial.Value++;
            foreach (var times in logStartInterstitial.Value.ToArray())
            {
                if (countInterstitial.Value == times && !startInterstitialLogged.Contains(countInterstitial.Value))
                {
                    startInterstitialLogged.AddDistinct(countInterstitial.Value);
                    var logEvent = $"show_interstitial_times_{(countInterstitial.Value):D4}";
                    new CustomSonatLog(logEvent, new List<LogParameter>()).Post(true);
                    
#if UNITY_EDITOR
                    Debug.LogError("Log "+logEvent);
#endif
                    break;
                }
            }
        }
        
        //count paidAd
        private static PlayerPrefRemoteArrayInt logStartPaidAd;
        private static PlayerPrefListInt startPaidAdLogged;
        private static readonly PlayerPrefInt countPaidAd = new PlayerPrefInt("count_paid_ad_showed");
        public static void LogPaidAd()
        {
            if (logStartPaidAd == null)
            {
                var fireBase = Kernel.Resolve<FireBaseController>();
                logStartPaidAd = fireBase.remoteConfigController.setting.LogPaidAd;
            }

            if (startPaidAdLogged == null)
                startPaidAdLogged = new PlayerPrefListInt("start_paid_ad_logged", new List<int>());
            
            countPaidAd.Value++;
            foreach (var times in logStartPaidAd.Value.ToArray())
            {
                if (countPaidAd.Value == times && !startPaidAdLogged.Contains(countPaidAd.Value))
                {
                    startPaidAdLogged.AddDistinct(countPaidAd.Value);
                    var logEvent = $"paid_ad_impression_{(countPaidAd.Value):D6}";
                    new CustomSonatLog(logEvent, new List<LogParameter>()).Post(true);
                    
#if UNITY_EDITOR
                    Debug.LogError("Log "+logEvent);
#endif
                    break;
                }
            }
        }
    }
}
// ReSharper disable InconsistentNaming
#endif