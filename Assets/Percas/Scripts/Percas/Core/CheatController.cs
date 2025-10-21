using UnityEngine;

namespace Percas
{
    public class CheatController : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.C))
            //{
            //    if (GameLogic.IsInHome)
            //    {
            //        UICurrencyManager.OnShowPictureGain?.Invoke(false, 10, null);
            //    }
            //}

            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    HiddenPictureManager.Data.Reset();
            //}

            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    HiddenPictureManager.Data.Keys += 1;
            //}

            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    HiddenPictureManager.Data.EndTime = System.DateTime.Parse(HiddenPictureManager.Data.EndTime).AddDays(-1).ToString();
            //}

            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    PlayerDataManager.PlayerData.IntroToHiddenPictureTutorial = false;
            //}

            if (Input.GetKeyDown(KeyCode.C))
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.BuildingTutorial);
            }
        }
#endif
    }
}
