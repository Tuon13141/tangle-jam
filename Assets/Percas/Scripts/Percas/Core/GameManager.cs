using Percas.UI;
using UnityEngine;

namespace Percas
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] ButtonBase buttonGamePause;
        [SerializeField] ButtonBase buttonGameReplay;

        private void Start()
        {
            TrackingManager.OnTrackScreenView?.Invoke(ScreenName.SceneGame.ToString());
            GlobalSetting.ScreenName = ScreenName.SceneGame.ToString();
            buttonGamePause.SetPointerClickEvent(OpenSetting);
            buttonGameReplay.SetPointerClickEvent(Replay);
        }

        private void OpenSetting()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.LevelPause);
        }

        private void Replay()
        {
            if (!GameLogic.IsInfiniteLive && GameLogic.CurrentLive <= 0)
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.RefillLives);
            }
            else
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.LevelRetry);
            }
        }
    }
}
