using System.Collections.Generic;
using UnityEngine;

namespace Percas
{
    [CreateAssetMenu(fileName = "NewHiddenPictureDataSO", menuName = "Game/Hidden Picture")]
    public class HiddenPictureDataSO : ScriptableObject
    {
        public int ID;
        public string EventName;
        public string Desctiption;
        public int PictureSize = 3; // 3x3
        public Sprite EventCoverImage;
        public Sprite EventIcon;
        public Sprite HiddenPicture;
        public Sprite HiddenBackground;
        public List<Sprite> PiecePictures = new();
        public List<LevelAsset> LevelDatas = new();
    }
}
