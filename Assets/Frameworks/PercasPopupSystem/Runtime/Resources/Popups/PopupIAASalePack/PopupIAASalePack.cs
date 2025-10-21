using System;
using UnityEngine;
using TMPro;
using Percas.IAA;
using Percas.UI;

namespace Percas
{
    public class PopupIAASalePackArgs
    {
        public IAASalePackID packID;

        public PopupIAASalePackArgs(IAASalePackID packID)
        {
            this.packID = packID;
        }
    }

    public class PopupIAASalePack : PopupBase
    {
        [SerializeField] ButtonClosePopup buttonClosePopup;
        [SerializeField] TMP_Text textPackName;
        [SerializeField] PopupIAASalePack_Content content;

        private IAASalePackDataSO packData;

        protected override void Awake()
        {
            buttonClosePopup.onCompleted = Close;
        }

        private void InitUI()
        {
            textPackName.text = $"{packData.packName}";
            content.SetContent(packData, Close);
        }

        private void Close()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.IAASalePack, null);
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            if (args is PopupIAASalePackArgs popupArgs)
            {
                packData = DataManager.GetPackData(popupArgs.packID) ?? DataManager.GetPackData(IAASalePackID.BuyUndo);
            }
            base.Show(args, callback);
            InitUI();
        }
        #endregion
    }
}
