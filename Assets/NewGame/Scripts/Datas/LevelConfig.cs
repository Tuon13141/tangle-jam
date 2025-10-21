using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.WoolSort.Data
{
    //[CreateAssetMenu]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] LevelData[] LevelDatas;

        public LevelData GetLevelData(int levelId)
        {
            return LevelDatas[(levelId - 1) % LevelDatas.Length];
        }
    }
}
