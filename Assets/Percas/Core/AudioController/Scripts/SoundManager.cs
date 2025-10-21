using UnityEngine;

namespace Percas
{
    public class SoundManager : MonoBehaviour
    {
        private void Awake()
        {
            ActionEvent.OnPlaySFXButtonClickOn += OnPlaySFXButtonClickOn;
            ActionEvent.OnPlaySFXButtonClickOut += OnPlaySFXButtonClickOut;

            ActionEvent.OnPlaySFXCoilClickOn += OnPlaySFXCoilClickOn;
            ActionEvent.OnPlaySFXCoilMerge += OnPlaySFXCoilMerge;
            ActionEvent.OnPlaySFXCoilHide += OnPlaySFXCoilHide;
            ActionEvent.OnPlaySFXBoosterUndo += OnPlaySFXBoosterUndo;
            ActionEvent.OnPlaySFXBoosterAddSlots += OnPlaySFXBoosterAddSlots;
            ActionEvent.OnPlaySFXBoosterClear += OnPlaySFXBoosterClear;
            ActionEvent.OnPlaySFXGameLose += OnPlaySFXGameLose;
            ActionEvent.OnPlaySFXGameWin += OnPlaySFXGameWin;
            ActionEvent.OnPlaySFXCoinDrop += OnPlaySFXCoinDrop;

            ActionEvent.OnPlaySFXCollectionClickOn += OnPlaySFXCollectionClickOn;
            ActionEvent.OnPlaySFXCollectionFill += OnPlaySFXCollectionFill;
            ActionEvent.OnPlaySFXCollectionFillCompleted += OnPlaySFXCollectionFillCompleted;
            ActionEvent.OnPlaySFXCollectionCompleted += OnPlaySFXCollectionCompleted;

            ActionEvent.OnPlaySFXLuckySpin += OnPlaySFXLuckySpin;
        }

        private void OnDestroy()
        {
            ActionEvent.OnPlaySFXButtonClickOn -= OnPlaySFXButtonClickOn;
            ActionEvent.OnPlaySFXButtonClickOut -= OnPlaySFXButtonClickOut;

            ActionEvent.OnPlaySFXCoilClickOn -= OnPlaySFXCoilClickOn;
            ActionEvent.OnPlaySFXCoilMerge -= OnPlaySFXCoilMerge;
            ActionEvent.OnPlaySFXCoilHide -= OnPlaySFXCoilHide;
            ActionEvent.OnPlaySFXBoosterUndo -= OnPlaySFXBoosterUndo;
            ActionEvent.OnPlaySFXBoosterAddSlots -= OnPlaySFXBoosterAddSlots;
            ActionEvent.OnPlaySFXBoosterClear -= OnPlaySFXBoosterClear;
            ActionEvent.OnPlaySFXGameLose -= OnPlaySFXGameLose;
            ActionEvent.OnPlaySFXGameWin -= OnPlaySFXGameWin;
            ActionEvent.OnPlaySFXCoinDrop -= OnPlaySFXCoinDrop;

            ActionEvent.OnPlaySFXCollectionClickOn -= OnPlaySFXCollectionClickOn;
            ActionEvent.OnPlaySFXCollectionFill -= OnPlaySFXCollectionFill;
            ActionEvent.OnPlaySFXCollectionFillCompleted -= OnPlaySFXCollectionFillCompleted;
            ActionEvent.OnPlaySFXCollectionCompleted -= OnPlaySFXCollectionCompleted;

            ActionEvent.OnPlaySFXLuckySpin -= OnPlaySFXLuckySpin;
        }

        private AudioClip GetSFX(string audioName)
        {
            return Resources.Load<AudioClip>("SFX/" + audioName);
        }

        private void OnPlaySFXButtonClickOn()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_BUTTON_CLICK_ON));
        }

        private void OnPlaySFXButtonClickOut()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_BUTTON_CLICK_OUT));
        }

        private void OnPlaySFXCoilClickOn()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_COIL_CLICK_ON));
        }

        private void OnPlaySFXCoilMerge()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_COIL_MERGE));
        }

        private void OnPlaySFXCoilHide()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_COIL_HIDE));
        }

        private void OnPlaySFXBoosterUndo()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_BOOSTER_UNDO));
        }

        private void OnPlaySFXBoosterAddSlots()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_BOOSTER_ADD_SLOTS));
        }

        private void OnPlaySFXBoosterClear()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_BOOSTER_CLEAR));
        }

        private void OnPlaySFXGameLose()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_GAME_LOSE));
        }

        private void OnPlaySFXGameWin()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_GAME_WIN));
        }

        private void OnPlaySFXCoinDrop()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_COIN_DROP));
        }

        private void OnPlaySFXCollectionClickOn()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_COLLECTION_CLICK_ON));
        }

        private void OnPlaySFXCollectionFill()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_COLLECTION_FILL));
        }

        private void OnPlaySFXCollectionFillCompleted()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_COLLECTION_FILL_COMPLETED));
        }

        private void OnPlaySFXCollectionCompleted()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_COLLECTION_COMPLETED));
        }

        private void OnPlaySFXLuckySpin()
        {
            AudioController.Instance.SFXOverride(GetSFX(Const.SFX_LUCKY_SPIN));
        }
    }
}
