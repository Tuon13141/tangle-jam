using System;
using UnityEngine;

namespace Percas.UI
{
    public class ButtonClosePopup : ButtonBase
    {
        [HideInInspector]
        public Action onCompleted;

        protected override void Awake()
        {
            base.Awake();
            SetPointerClickEvent(ClosePopup);
        }

        private void ClosePopup()
        {
            onCompleted?.Invoke();
        }
    }
}
