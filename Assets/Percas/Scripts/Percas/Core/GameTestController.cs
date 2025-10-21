using UnityEngine;
using Percas.UI;

namespace Percas
{
    public class GameTestController : MonoBehaviour
    {
        [SerializeField] ButtonBase buttonGamePause;
        [SerializeField] ButtonBase buttonGameWin;
        [SerializeField] ButtonBase buttonGameLose;

        private void Start()
        {
            buttonGamePause.SetPointerClickEvent(OpenSetting);
            buttonGameWin.SetPointerClickEvent(GameWin);
            buttonGameLose.SetPointerClickEvent(GameLose);
        }

        private void OpenSetting()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.LevelPause);
        }

        private void GameWin()
        {
            ActionEvent.OnLevelWin?.Invoke();
            ServiceLocator.PopupScene.ShowPopup(PopupName.LevelWin);
        }

        private void GameLose()
        {
            ActionEvent.OnLevelLose?.Invoke();
            ServiceLocator.PopupScene.ShowPopup(PopupName.LevelRevive);
        }
    }
}
