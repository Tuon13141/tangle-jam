using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Percas.IAR;

namespace Percas.IAA
{
    [CreateAssetMenu(fileName = "NewIAASalePack", menuName = "Game/IAA Sale Pack")]
    public class IAASalePackDataSO : ScriptableObject
    {
        public IAASalePackID packID;
        public string packName;
        public Sprite image;
        public int coinPrice; // coinPrice = 0 then Coin Button not shown
        public string description;
        public List<Reward> rewards;
        public bool useImmediately;
        public SkeletonDataAsset skeletonData;
    }
}
