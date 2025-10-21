using UnityEngine;

namespace Percas
{
    public enum ProfileItemType
    {
        Avatar,
        Frame,
    }

    [CreateAssetMenu(fileName = "NewAvatarDataSO", menuName = "Game/Avatar")]
    public class AvatarDataSO : ScriptableObject
    {
        public ProfileItemType type;
        public Sprite sprite;
        public int unlockByLevel = 0;
        public int unlockByVideoAd = 0;
        public int unlockByCoin = 0;
    }
}
