using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Percas.Data;
using Percas.UI;

namespace Percas
{
    public class PopupHiddenPicture_Piece : MonoBehaviour
    {
        [SerializeField] int index;
        [SerializeField] Image m_image;
        [SerializeField] CanvasGroup m_canvasGroup;
        [SerializeField] GameObject m_locked;
        [SerializeField] ButtonBase buttonPlay;
        [SerializeField] bool canClick = true;

        private void Awake()
        {
            buttonPlay.SetPointerClickEvent(Play);
        }

        private void Play()
        {
            if (!canClick) return;

            PopupHiddenPicture_ProcessReward.OnHidePreviewReward?.Invoke();

            if (!GameLogic.InternetReachability && GameLogic.CurrentLevel >= GameLogic.LevelNeedsInternet)
            {
                ServiceLocator.PopupScene.ShowPopup(PopupName.InternetRequired);
                return;
            }

            try
            {
                if (HiddenPictureManager.Data.IsUnlocked(index)) return;

                if (HiddenPictureManager.Data.Keys >= 1)
                {
                    HiddenPictureDataSO currentData = DataManager.Instance.GetCurrentHiddenPictureData();
                    GlobalSetting.HiddenPictureLevelIndex = index;
                    GlobalSetting.HiddenPictureLevelData = currentData.LevelDatas[index];
                    HiddenPictureManager.Data.UpdateKeys(-1);

                    // Play
                    PopupHiddenPicture.OnClosePopup?.Invoke(null);
                    GlobalSetting.SetPlayMode(PlayMode.hidden_picture);
                    PlayerDataManager.SetContinueWith(null);
                    PlayerDataManager.OnResetContinueTimes?.Invoke();
                    GlobalSetting.OnHomeToGame?.Invoke(null);
                }
                else
                {
                    ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_NOT_ENOUGH_KEY);
                    PopupHiddenPicture.OnScaleKeyAmount?.Invoke();
                    return;
                }
            }
            catch (Exception)
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_SOMETHING_WRONG);
                return;
            }
        }

        public void UpdateUI(Sprite sprite, bool isUnlocked)
        {
            m_image.sprite = sprite;
            m_locked.SetActive(!isUnlocked);
        }
    }
}