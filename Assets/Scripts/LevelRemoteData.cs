using System.IO;
using UnityEngine;
using NaughtyAttributes;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "LevelRemoteData", menuName = "SOLevel/LevelRemoteData", order = 0)]
public class LevelRemoteData : ScriptableObject
{
    public LevelAsset _levelData;

#if UNITY_EDITOR
    [Button]
    void Serialize()
    {
        var stringData = JsonConvert.SerializeObject(_levelData.dataString);

        Debug.Log(stringData);

        string pathFileJson = Path.Combine(Application.dataPath, "Resources/LevelRemoteData.txt");
        File.WriteAllText(pathFileJson, stringData);

        UnityEditor.AssetDatabase.Refresh();
    }
#endif
}
