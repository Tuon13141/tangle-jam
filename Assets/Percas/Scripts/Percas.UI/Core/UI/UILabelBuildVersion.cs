using UnityEngine;
using TMPro;
using Percas;

namespace PercasSDK
{
    public class UILabelBuildVersion : MonoBehaviour
    {
        [SerializeField] TMP_Text textLabel;

        private void OnEnable()
        {
            SetLabelText();
        }

        private void SetLabelText()
        {
            textLabel.text = $"Version {Application.version} - Build {GameConfig.Instance.BuildNumber} - Level {GameLogic.CurrentLevel}";
        }
    }
}
