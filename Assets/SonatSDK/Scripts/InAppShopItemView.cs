using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InAppShopItemView : BaseInAppView
{
    protected override void Claim(bool onSuccess)
    {
        if(!onSuccess) return;
        if (key == 0)
        {
            Kernel.Resolve<AdsManager>().EnableNoAds();
            Kernel.LogFirebase(EventNameEnum.remove_ads_success);
        }
        if (Kernel.ResolveDirty<BasePurchaser>().OthersNoAdsKey.Contains(key))
        {
            Kernel.Resolve<AdsManager>().EnableNoAds();
            Kernel.LogFirebase(EventNameEnum.remove_ads_success);
        }
        
        Debug.Log("buy success" + key);
    }
}
