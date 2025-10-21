using UnityEngine;
using Percas.UI;

namespace Percas
{
    public class PopupRemoveAds : PopupBase
    {
        [SerializeField] ButtonClosePopup buttonClosePopup;
        [SerializeField] ButtonPurchase buttonPurchase;

        protected override void Awake()
        {
            buttonClosePopup.onCompleted = Close;
            buttonPurchase.onCallback = Close;
        }

        private void Close()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.RemoveAds, () =>
            {
                GameLogic.AutoSalePopupClosed = true;
            });
        }
    }
}
