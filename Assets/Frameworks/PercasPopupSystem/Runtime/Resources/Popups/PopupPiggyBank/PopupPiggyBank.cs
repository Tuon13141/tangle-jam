using System;
using UnityEngine;
using TMPro;
using Percas.UI;

namespace Percas
{
    public class PopupPiggyBank : PopupBase
    {
        [SerializeField] ButtonClosePopup buttonClosePopup;
        [SerializeField] ButtonPurchase buttonPurchase;
        [SerializeField] TMP_Text textMessage;
        [SerializeField] PopupLevelWin_PiggyBank PiggyAnim;

        protected override void Awake()
        {
            buttonClosePopup.onCompleted = Close;
            buttonPurchase.onStart = OnStart;
            buttonPurchase.onCallback = Close;
        }

        private void Close()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.PiggyBank);
        }

        private void OnStart(Action<bool> onCallback)
        {
            if (!GameLogic.IsFullPiggyBank)
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_NOT_FULL_PB);
                onCallback?.Invoke(false);
            }
            else
            {
                onCallback?.Invoke(true);
            }
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            textMessage.text = GameLogic.IsFullPiggyBank ? Const.LANG_KEY_FULL_PIGGY_BANK : Const.LANG_KEY_NOT_FULL_PIGGY_BANK;
            PiggyAnim.gameObject.SetActive(true);
            PiggyAnim.UpdateUI(false, null);
        }

        public override void Hide(Action callback = null)
        {
            PiggyAnim.gameObject.SetActive(false);
            base.Hide(() =>
            {
                GlobalSetting.NotiPiggyBankSeen = true;
                GameLogic.AutoSalePopupClosed = true;
                ButtonPiggyBank.OnUpdateNoti?.Invoke();
                ButtonPiggyBank.OnUpdateButtonText?.Invoke();
                callback?.Invoke();
            });
        }
        #endregion
    }
}
