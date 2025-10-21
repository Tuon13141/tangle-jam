using UnityEngine;
using UnityEngine.Purchasing;
using TMPro;

namespace Percas.IAP
{
    public class IAPButtonPurchase : MonoBehaviour
    {
        [SerializeField] string productID;
        [SerializeField] TMP_Text txtPrice;

        private IAPPack _pack;
        private Product _product;

        private void Start()
        {
            _pack = IAPManager.OnGetPack?.Invoke(productID);
            _product = IAPManager.OnGetProduct?.Invoke(productID.ToString());
            txtPrice.text = _product != null ? $"{_product.metadata.localizedPriceString}" : _pack != null ? $"${_pack.productPackPriceInUSD}" : $"$0.99";
        }

        public void Purchase()
        {
            if (!GameConfig.Instance.IsReachableInternet())
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_SOMETHING_WRONG);
                return;
            }
            IAPManager.OnPurchase?.Invoke(productID);
        }
    }
}
