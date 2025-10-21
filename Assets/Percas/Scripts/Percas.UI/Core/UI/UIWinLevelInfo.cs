using System;
using UnityEngine;
using TMPro;

namespace Percas.UI
{
    public class UIWinLevelInfo : MonoBehaviour, IActivatable
    {
        [SerializeField] TMP_Text textLevelPassed;
        [SerializeField] TMP_Text textWinCoin;
        [SerializeField] TMP_Text textWinCoil;
        [SerializeField] TMP_Text textWinPin;
        [SerializeField] TMP_Text textWinStar;

        public static Action OnUpdateUI;

        public void Activate()
        {
            OnUpdateUI += UpdateLevelInfo;

            UpdateLevelInfo();
        }

        public void Deactivate()
        {
            OnUpdateUI -= UpdateLevelInfo;
        }

        private void UpdateLevelInfo()
        {
            if (GameLogic.IsClassicMode)
            {
                textLevelPassed.text = $"Level {Math.Max(GameLogic.CurrentLevel - 1, 1)} Passed";
            }
            else if (GameLogic.IsHiddenPictureMode)
            {
                textLevelPassed.text = $"{DataManager.Instance.GetCurrentHiddenPictureName()} {GlobalSetting.HiddenPictureLevelIndex + 1} Passed";
            }

            textWinCoin.text = $"{GameLogic.CoinEarnWinLevel}";

            if (GameLogic.ThreadEarnWinLevel == 0)
            {
                textWinCoil.text = $"{GameLogic.LevelCoil}";
            }
            else
            {
                textWinCoil.text = $"{GameLogic.ThreadEarnWinLevel}";
            }

            textWinPin.text = $"{GameLogic.LevelPin}";

            textWinStar.text = $"{GameLogic.LevelStar}";
        }
    }
}
