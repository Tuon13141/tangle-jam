using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NaughtyAttributes;
using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "RemoteData", menuName = "SOLevel/RemoteData", order = 0)]
public class RemoteData : ScriptableObject
{
    [Serializable]
    public class RemoteLevelConfig
    {
        public string Key;
        public int Value;
    }

    public List<RemoteLevelConfig> remoteLevelConfig;
    public List<TupleSerialize<int, LevelAsset>> remoteLevelData;

#if UNITY_EDITOR
    [Button]
    void Serialize()
    {
        Dictionary<string, string> levelData = new();

        foreach (var element in remoteLevelData)
        {
            levelData.Add(string.Format("level_{0}", element.Value1), element.Value2.dataString);
        }

        var stringData = JsonConvert.SerializeObject(levelData);

        var remoteData = new
        {
            LevelConfig = remoteLevelConfig.ToDictionary(x => x.Key, x => x.Value),
            LevelData = stringData,
        };

        Debug.Log(JsonConvert.SerializeObject(remoteData));

        string pathFileJson = Path.Combine(Application.dataPath, "Resources/RemoteData.txt");
        File.WriteAllText(pathFileJson, JsonConvert.SerializeObject(remoteData));

        UnityEditor.AssetDatabase.Refresh();
    }
#endif
}
