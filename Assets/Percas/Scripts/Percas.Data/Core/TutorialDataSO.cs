using UnityEngine;
using TMPro;
using Spine.Unity;

namespace Percas.Data
{
    [CreateAssetMenu(fileName = "NewTutorialDataSO", menuName = "Game/Tutorial")]
    public class TutorialDataSO : ScriptableObject
    {
        public TutorialShowType showType = TutorialShowType.Popup;
        public bool showInGame = true;
        [Tooltip("If true, always shown as a popup")]
        public bool preNotice = false;
        public bool isBooster = false;
        public bool autoClose = false;
        public bool showCloseButton = true;
        public int autoCloseIn = 3;
        public int id;
        public int level;
        public int wave;
        public Sprite image;
        public Sprite iconInButton;
        public string key;
        public string message;
        public string messageWithoutImage;
        public string title = null;
        public TMP_SpriteAsset spriteAsset;
        public SkeletonDataAsset skeletonData;
    }
}
