using System;
using Percas.UI;
using UnityEngine;

namespace Percas
{
    public class PopupInternetRequired : PopupBase
    {
        [SerializeField] ButtonClosePopup buttonClosePopup;

        protected override void Awake()
        {
            buttonClosePopup.onCompleted = Close;
        }

        private void Close()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.InternetRequired, null);
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
        }
        #endregion
    }
}
