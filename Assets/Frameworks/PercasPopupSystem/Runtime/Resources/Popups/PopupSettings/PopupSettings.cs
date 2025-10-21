using System;
using UnityEngine;
using Percas.UI;

namespace Percas
{
    public class PopupSettings : PopupBase
    {
        [SerializeField] ButtonClosePopup buttonClosePopup;
        [SerializeField] ButtonBase buttonSelectLanguage;
        [SerializeField] ButtonBase buttonRestore;
        [SerializeField] ButtonBase buttonPolicy;

        protected override void Awake()
        {
            RegisterButtons();
        }

        private void RegisterButtons()
        {
            buttonClosePopup.onCompleted = Close;
            buttonSelectLanguage.SetPointerClickEvent(SelectLanguage);
            buttonRestore.SetPointerClickEvent(Restore);
            buttonPolicy.SetPointerClickEvent(OpenPolicy);
        }

        protected override void OnSubscribeEvents()
        {
            ActionEvent.OnSettingClose += Close;
        }

        protected override void OnUnsubscribeEvents()
        {
            ActionEvent.OnSettingClose -= Close;
        }

        private void Close()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.Settings);
        }

        private void SelectLanguage()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.SelectLanguage);
        }

        private void Restore()
        {
#if UNITY_ANDROID
            Kernel.Resolve<Purchaser>().RestorePurchase();
#elif UNITY_IOS
        Kernel.Resolve<Purchaser>().RestorePurchasesIOS(null);
#endif
        }

        private void OpenPolicy()
        {
            string urlPolicy = "https://percas.vn/privacy-policy/";
            Application.OpenURL(urlPolicy);
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
        }
        #endregion
    }
}
