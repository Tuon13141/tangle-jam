using System;
using System.Collections;
using System.Collections.Generic;
using sonat_sdk.Scripts;
using UnityEngine;


[Serializable]
public class PlayerPrefRemoteInt
{
    [SerializeField] private string key;
    [SerializeField] private int value; //default

    private int SavedData
    {
        get => PlayerPrefs.GetInt(key, value);
        set => PlayerPrefs.SetInt(key, value);
    }

    public int Value
    {
        get
        {
            if (SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched ||
                SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.FetchedFail)
                if (RemoteConfigController.RemoteHasValue(key) &&
                    (int) RemoteConfigHelper.GetValueInt(key) != SavedData)
                    SavedData = (int) RemoteConfigHelper.GetValueInt(key);

            return SavedData;
        }
    }

    public PlayerPrefRemoteInt(string key, int value)
    {
        this.key = key;
        this.value = value;
    }


    public PlayerPrefRemoteInt(RemoteConfigKey key, int value)
    {
        this.key = key.ToString();
        this.value = value;
    }
}

[Serializable]
public class PlayerPrefRemoteString
{
    [SerializeField] private string key;
    [SerializeField] private string value; //default

    private string SavedData
    {
        get => PlayerPrefs.GetString(key, value);
        set => PlayerPrefs.SetString(key, value);
    }

    public string DefaultValueWhenEmpty() => string.IsNullOrEmpty(Value) ? value : Value;

    public string Value
    {
        get
        {
            if (SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched ||
                SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.FetchedFail)
                if (RemoteConfigController.RemoteHasValue(key) && RemoteConfigHelper.GetValueString(key) != SavedData)
                    SavedData = RemoteConfigHelper.GetValueString(key);

            return SavedData;
        }
    }

    public PlayerPrefRemoteString(RemoteConfigKey key, string value)
    {
        this.key = key.ToString();
        this.value = value;
    }

    public PlayerPrefRemoteString(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}


[Serializable]
public class PlayerPrefRemoteBool
{
    [SerializeField] private string key;
    [SerializeField] private bool value; //default

    private bool SavedData
    {
        get => PlayerPrefs.GetInt(key, value ? 1 : 0) != 0;
        set => PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public bool Value
    {
        get
        {
            if (SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched ||
                SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.FetchedFail)
                if (RemoteConfigController.RemoteHasValue(key) && RemoteConfigHelper.GetValueBoolean(key) != SavedData)
                    SavedData = RemoteConfigHelper.GetValueBoolean(key);

            return SavedData;
        }
    }


    public PlayerPrefRemoteBool(string key, bool value)
    {
        this.key = key;
        this.value = value;
    }

    public PlayerPrefRemoteBool(RemoteConfigKey key, bool value)
    {
        this.key = key.ToString();
        this.value = value;
    }
}


[Serializable]
public class PlayerPrefRemoteFloat
{
    [SerializeField] private string key;
    [SerializeField] private float value; //default

    private float SavedData
    {
        get => PlayerPrefs.GetFloat(key, value);
        set => PlayerPrefs.SetFloat(key, value);
    }

    public float Value
    {
        get
        {
            if (SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched ||
                SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.FetchedFail)
                if (RemoteConfigController.RemoteHasValue(key) &&
                    Math.Abs((float) RemoteConfigHelper.GetValueDouble(key) - SavedData) > 0.001f)
                    SavedData = (float) RemoteConfigHelper.GetValueDouble(key);

            return SavedData;
        }
    }


    public PlayerPrefRemoteFloat(RemoteConfigKey key, float value)
    {
        this.key = key.ToString();
        this.value = value;
    }

    public PlayerPrefRemoteFloat(string key, float value)
    {
        this.key = key;
        this.value = value;
    }
}

public class PlayerPrefInt
{
    public Action<int> OnChanged;

    public int Value
    {
        get => PlayerPrefs.GetInt(_name);
        set
        {
            if (value != Value)
            {
                PlayerPrefs.SetInt(_name, value);
                OnChanged?.Invoke(value);
            }
        }
    }

    public bool BoolValue
    {
        get => PlayerPrefs.GetInt(_name) != 0;
        set
        {
            if (value == (Value != 0)) return;
            PlayerPrefs.SetInt(_name, value ? 1 : 0);
            OnChanged?.Invoke(value ? 1 : 0);
        }
    }

    private readonly string _name;

    public PlayerPrefInt(string name)
    {
        _name = name;
    }
    
    public PlayerPrefInt(PlayerPrefEnum name)
    {
        _name = "extra_"+name;
    }

    public bool HasKey() => PlayerPrefs.HasKey(_name);
    
    public PlayerPrefInt(string name,int defaultValue)
    {
        if(!PlayerPrefs.HasKey(name))
            PlayerPrefs.SetInt(name, defaultValue);
        _name = name;
    }
}


public class PlayerPrefLong
{
    public Action<long> OnChanged;

    private long _currentValue;
    public long Value
    {
        get => _currentValue;
        set
        {
            if (value != Value)
            {
                _currentValue = value;
                PlayerPrefs.SetString(_name, value.ToString());
                OnChanged?.Invoke(value);
            }
        }
    }


    private readonly string _name;

    public PlayerPrefLong(string name)
    {
        _name = name;
        _currentValue = 0;
        try
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(_name)))
            {
                _currentValue = long.Parse(PlayerPrefs.GetString(_name));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}



public class PlayerPrefString
{
    public bool Exist => PlayerPrefs.HasKey(_name) && !string.IsNullOrEmpty(PlayerPrefs.GetString(_name,_default));

    public string Value
    {
        get => PlayerPrefs.GetString(_name,_default);
        set
        {
            if (value != Value)
                PlayerPrefs.SetString(_name, value);
        }
    }

    private readonly string _name;

    private string _default;
    public PlayerPrefString(string name, string defaultValue)
    {
        _name = name;
        _default = defaultValue;
    }

    public void Clear()
    {
        PlayerPrefs.DeleteKey(_name);
    }
}