
using System;
using UnityEngine;

[Serializable]
public class KeyConditionBase
{
    public int key;
}


[Serializable]
public class IntKeyCondition : KeyConditionBase
{
    public int value;
}

[Serializable]
public class StringByCondition : KeyConditionBase
{
    public string value;
}

[Serializable]
public class IntByLevelCollection
{
//    public int defaultValue;
    public IntKeyCondition[] data;
}


[Serializable]
public class StringByLevelCollection
{
//    public int defaultValue;
    public StringByCondition[] data;
}

[Serializable]
public class IntByLevelCollectionRemote : IntByLevelCollection
{
    public RemoteConfigKey key;

    public IntByLevelCollectionRemote(RemoteConfigKey key)
    {
        this.key = key;
    }

    public void Load()
    {
        var remoteStr = key.GetValueString(false);
        if (remoteStr != String.Empty)
        {
            IntByLevelCollection collection = JsonUtility.FromJson<IntByLevelCollection>(remoteStr);
            data = collection.data;
        }
    }
}

[Serializable]
public class StringByLevelCollectionRemote: StringByLevelCollection
{
    public RemoteConfigKey key;
    public StringByLevelCollectionRemote(RemoteConfigKey key)
    {
        this.key = key;
    }
    
    public void Load()
    {
        var remoteStr = key.GetValueString(false);
        if (remoteStr != String.Empty)
        {
            StringByLevelCollection collection = JsonUtility.FromJson<StringByLevelCollection>(remoteStr);
            data = collection.data;
        }
        else
        {
            data = null;
        }
    }

    public string GetKey() => key.ToString();
    public string GetString() => key.GetValueString(false);
}