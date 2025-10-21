using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class PlayerPrefListInt
{
    [SerializeField] private List<int> defaultValue; //default
    [SerializeField] private string key;
    public List<int> Current { get; } //default
    
    private List<int> Get()
    {
        return  DuongSerializationExtensions
            .ListIntFromString(PlayerPrefs.GetString(key, DuongSerializationExtensions.ToString(defaultValue)))
            .ToList();
    }

    public int Count => Current.Count;

    public bool Contains(int value) => Current.Contains(value);
    
    public bool AddDistinct(int add)
    {
        if (!Current.Contains(add))
        {
            Current.Add(add);
            Save();
            return true;
        }

        return false;
    }
    
    public void Add(int add)
    {
        Current.Add(add);
        Save();
    }
    
    public void Remove(int remove)
    {
        Current.Remove(remove);
        Save();
    }
     
    public void RemoveAll(int remove)
    {
        Current.RemoveAll(x => x == remove);
        Save();
    }

    public void Clear()
	{
        Current.Clear();
        Save();
	}
    
    public void Save()
    {
        PlayerPrefs.SetString(key, DuongSerializationExtensions.ToString(Current));
    }


//    public void Set(IEnumerable<int> set)
//    {
//        current = set.ToList();
//        PlayerPrefs.SetString(key, DuongSerializationExtensions.ToString(current));
//    }
    
    public PlayerPrefListInt(string key,List<int> value)
    {
        this.key = key;
        defaultValue = value;
        Current = PlayerPrefs.HasKey(key) ? Get() : defaultValue.ToList();
    }
}


/// <summary>
/// value only can override not edit
/// </summary>
[Serializable]
public class PlayerPrefRemoteArrayInt
{
    [SerializeField] private int[] defaultValue; //default
    [SerializeField] private string key;

    private int[] SavedData => DuongSerializationExtensions.ListIntFromString(PlayerPrefs.GetString(key,DuongSerializationExtensions.ToString(defaultValue))).ToArray();

    public int[] Value
    {
        get
        {
            if (SharedRemoteConfigController.FetchStatus == ConfigFetchStatus.Fetched 
                &&  RemoteConfigController.RemoteHasValue(key)
                )
            {
                if (RemoteConfigHelper.GetValueString(key) != PlayerPrefs.GetString(key, DuongSerializationExtensions.ToString(defaultValue)))
                    PlayerPrefs.SetString(key, RemoteConfigHelper.GetValueString(key));
            }


            return SavedData;
        }
    }

    public PlayerPrefRemoteArrayInt(RemoteConfigKey key,int[] value)
    {
        this.key = key.ToString();
        defaultValue = value;
    }
    
    public PlayerPrefRemoteArrayInt(string key,int[] value)
    {
        this.key = key;
        defaultValue = value;
    }
}
