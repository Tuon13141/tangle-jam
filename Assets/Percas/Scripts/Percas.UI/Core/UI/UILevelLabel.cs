using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Percas.UI
{
    public class UILevelLabel : MonoBehaviour
    {
        [SerializeField] RectTransform rect;
        [SerializeField] TMP_Text textLabel;
        [SerializeField] Image imgLabel;
        [SerializeField] List<Sprite> images;
        [SerializeField] List<Color> colors;
        [SerializeField] GameObject iconHardLevel;

        public static Action<bool> OnUpdateLabel;
        public static Action<int> OnSetupPhaseItem;
        public static Action<int> OnUpdatePhaseStatus;

        private void Awake()
        {
            OnUpdateLabel += UpdateLabel;
        }

        private void OnDestroy()
        {
            OnUpdateLabel -= UpdateLabel;
        }

        private void UpdateLabel(bool isHardLevel)
        {
            if (GameLogic.IsClassicMode)
            {
                if (isHardLevel)
                {
                    textLabel.text = $"HARD<br>LEVEL {GameLogic.CurrentLevel}";
                    imgLabel.sprite = images[1];
                    textLabel.color = colors[1];
                }
                else
                {
                    textLabel.text = $"LEVEL {GameLogic.CurrentLevel}";
                    imgLabel.sprite = images[0];
                    textLabel.color = colors[0];
                }
            }
            else if (GameLogic.IsHiddenPictureMode)
            {
                textLabel.text = $"{DataManager.Instance.GetCurrentHiddenPictureName()} {GlobalSetting.HiddenPictureLevelIndex + 1}";
                imgLabel.sprite = images[0];
                textLabel.color = colors[0];
            }
        }
    }
}
