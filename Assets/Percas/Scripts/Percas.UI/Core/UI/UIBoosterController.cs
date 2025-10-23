using Cysharp.Threading.Tasks;
using System;
using Tuon;
using UnityEngine;

namespace Percas.UI
{
    public class UIBoosterController : MonoBehaviour
    {
        [SerializeField] ButtonUseBooster buttonUseUndo;
        [SerializeField] ButtonUseBooster buttonUseAddSlots;
        [SerializeField] ButtonUseBooster buttonUseClear;

        private void Start()
        {
            buttonUseUndo.onStart = CheckUndo;
            buttonUseUndo.onCompleted = UndoAction;
            buttonUseUndo.onError = OnErrorUndo;

            buttonUseAddSlots.onStart = CheckAddSlot;
            buttonUseAddSlots.onCompleted = AddSlotAction;
            buttonUseAddSlots.onError = OnErrorAddSlots;

            buttonUseClear.onStart = CheckClear;
            buttonUseClear.onCompleted = ClearAction;
            buttonUseClear.onError = OnErrorClear;
        }

        // code connect logic gameplay
        public void CheckUndo(Action<bool> onCallback)
        {
            var result = LevelController.instance?.CheckShuffleBooster() ?? false;
            //Debug.Log(result);
            onCallback?.Invoke(result);
        }

        public void UndoAction()
        {
            LevelController.instance?.ShuffleBooster().Forget();
        }

        public void CheckAddSlot(Action<bool> onCallback)
        {
            var result = LevelController.instance?.CheckAddSlotBooster() ?? false;
            onCallback?.Invoke(result);
        }

        public void AddSlotAction()
        {
            LevelController.instance?.AddSlotBooster();
        }

        public void CheckClear(Action<bool> onCallback)
        {
            var result = LevelController.instance?.CheckRollCollectBooster() ?? false;
            onCallback?.Invoke(result);
        }

        public void ClearAction()
        {
            if (LevelController.instance?.isCollecting == true) return;
            LevelController.instance?.RollCollectBooster().Forget();
        }
        // end code

        private void OnErrorUndo()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.IAASalePack, new PopupIAASalePackArgs(IAA.IAASalePackID.BuyUndo));
        }

        private void OnErrorAddSlots()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.IAASalePack, new PopupIAASalePackArgs(IAA.IAASalePackID.BuyAddSlots));
        }

        private void OnErrorClear()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.IAASalePack, new PopupIAASalePackArgs(IAA.IAASalePackID.BuyClear));
        }
    }
}
