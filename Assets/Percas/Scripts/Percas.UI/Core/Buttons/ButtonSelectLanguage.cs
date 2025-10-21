using System;
using UnityEngine;
using Percas.UI;

namespace Percas
{
    public class ButtonSelectLanguage : ButtonBase
    {
        [SerializeField] string languageCode;

        public Action onCallback;

        protected override void Awake()
        {
            base.Awake();
            SetPointerClickEvent(SetLanguage);
        }

        private void SetLanguage()
        {
            LocalizationManager.Instance.SetLanguage(languageCode);
            onCallback?.Invoke();
        }
    }
}
