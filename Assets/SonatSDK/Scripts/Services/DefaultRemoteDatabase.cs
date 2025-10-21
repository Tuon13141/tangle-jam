using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = nameof(DefaultRemoteDatabase) + ".asset",
    menuName = "Sonat/Database/" + nameof(DefaultRemoteDatabase))]
public class DefaultRemoteDatabase : ScriptableObject
{
    public FireBaseDefaultSetting setting;

    [ArrayElementTitle(nameof(RemoteConfigDefaultByKey.key))] 
    public List<RemoteConfigDefaultByKey> defaultConfigs = new List<RemoteConfigDefaultByKey>();
    
    public RemoteConfigKey[] listIntCollectionKeys;
    public RemoteConfigKey[] listStringCollectionKeys;

    [SerializeField] private DefaultRemoteDatabase other;
    [ContextMenu("copy")]
    private void Copy()
    {
        for (var i = 0; i < other.defaultConfigs.Count; i++)
        {
            if(defaultConfigs.All(x => x.key != other.defaultConfigs[i].key))
                defaultConfigs.Add(other.defaultConfigs[i]);
        }
    }
    
    [ContextMenu("Log")]
    private void Log()
    {
      Debug.Log(setting.LogPaidAd.Value);
    }
}