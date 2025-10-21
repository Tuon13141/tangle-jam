using System;
using UnityEngine;
using TMPro;
using Percas.UI;

namespace Percas
{
    public class PopupErrorArgs
    {
        public string message;

        public PopupErrorArgs(string message)
        {
            this.message = message;
        }
    }

    public class PopupError : PopupBase
    {
        [SerializeField] ButtonClosePopup buttonClosePopup;
        [SerializeField] TMP_Text textMessage;

        private string Message;

        private void OnStart()
        {
            buttonClosePopup.SetPointerClickEvent(Close);
            if (!string.IsNullOrEmpty(Message))
                textMessage.text = $"{Message}";
        }

        private void Close()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.Error, null);
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            if (args is PopupErrorArgs popupArgs)
            {
                Message = popupArgs.message;
            }
            base.Show(args, callback);
            OnStart();
        }
        #endregion
    }
}
