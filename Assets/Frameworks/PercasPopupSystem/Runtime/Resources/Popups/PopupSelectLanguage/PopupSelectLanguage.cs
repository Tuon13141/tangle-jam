using System;
using System.Collections.Generic;
using UnityEngine;
using Percas.UI;

namespace Percas
{
    public class PopupSelectLanguage : PopupBase
    {
        [SerializeField] ButtonBase buttonClosePopup;
        [SerializeField] List<ButtonSelectLanguage> languageButtons;

        private void OnStart()
        {
            buttonClosePopup.SetPointerClickEvent(Close);
            foreach (ButtonSelectLanguage button in languageButtons)
            {
                button.onCallback = Select;
            }
        }

        private void OnHide(Action onCompleted = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.SelectLanguage, onCompleted);
        }

        private void Close()
        {
            OnHide();
        }

        private void Select()
        {
            OnHide(() =>
            {
                ActionEvent.OnSettingClose?.Invoke();
                GlobalSetting.OnGameToHome?.Invoke(null);
            });
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            OnStart();
        }
        #endregion
    }
}
