using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRemoteDatabaseScript : MonoBehaviour
{
    public abstract Type GetDatabaseType();
    public abstract ScriptableObject GetDatabase();
    public abstract void Clear();
    public abstract void LoadRemote();

    public abstract void Register(ScriptableObject origin);
}

public class BaseRemoteDatabase<T> : BaseRemoteDatabaseScript where T : ScriptableObject
{
    public override Type GetDatabaseType() => typeof(T);

    public override ScriptableObject GetDatabase()
    {
        return RemoteDatabase;
    }

    public T RemoteDatabase { get; set; }
    [SerializeField] private RemoteConfigDatabaseKey remoteKey;

    private PlayerPrefString _playerPrefString;

    [SerializeField]
    private PlayerPrefString PlayerPrefString => _playerPrefString ??= new PlayerPrefString(remoteKey.ToString(),"");

    public override void Clear()
    {
        PlayerPrefString.Clear();
    }

    public override void LoadRemote()
    {
        if (FireBaseController.FireBaseRemoteReady)
        {
            if (RemoteConfigController.RemoteHasValue(remoteKey.ToString()))
            {
                var remoteString = RemoteConfigHelper.GetValueString(remoteKey.ToString());
                if (remoteString != PlayerPrefString.Value)
                {
                    PlayerPrefString.Value = remoteString;
                    ResolveDatabase();
                }
                if (RemoteDatabase == null)
                    ResolveDatabase();
            }
            else
            {
                Clear();
                RemoteDatabase = null;
            }
        }
        else if(PlayerPrefString.Exist)
        {
            ResolveDatabase();
        }
        else
        {
            RemoteDatabase = null; 
        }
       
    }

    private ScriptableObject _originDatabase;

    public override void Register(ScriptableObject origin)
    {
        if (origin.GetType() == typeof(T))
            _originDatabase = origin;
    }

    private void ResolveDatabase()
    {
        if (PlayerPrefString.Exist)
        {
            try
            {
//                RemoteDatabase = ScriptableObject.CreateInstance<T>();
                RemoteDatabase = Instantiate(_originDatabase) as T;
                JsonUtility.FromJsonOverwrite(PlayerPrefString.Value, RemoteDatabase);
                //  RemoteDatabase = JsonUtility.FromJson<T>(PlayerPrefString.Value);
            }
            catch (Exception e)
            {
                // ignored
            }
        }
        else
        {
            RemoteDatabase = null;
        }
    }
}