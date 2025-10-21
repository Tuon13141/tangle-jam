using System;
using UnityEngine;

namespace Percas.UI
{
    public class ButtonTutorial : ButtonBase
    {
        [HideInInspector]
        public Action onCompleted;

        protected override void Awake()
        {
            base.Awake();
            SetPointerClickEvent(OnClick);
        }

        private void OnClick()
        {
            onCompleted?.Invoke();
        }
    }
}
