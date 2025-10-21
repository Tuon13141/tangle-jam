using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Percas.IAP
{
    [Serializable]
    public class IAPPack
    {
        public string productID;
        [SerializeField]
        public ProductType productType;
        public double productPackPriceInUSD;
    }
}
