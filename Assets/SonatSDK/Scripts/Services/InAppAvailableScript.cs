using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAppAvailableScript : MonoBehaviour
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    private int itemKey;

    private void Start()
    {
        var purchaser = Kernel.ResolveDirty<BasePurchaser>();
        Debug.Log(nameof(InAppAvailableScript) + "(purchaser.IsInitialized() && purchaser.IsItemAvailable(itemKey):"+(purchaser.IsInitialized() && purchaser.IsItemAvailable(itemKey)));
        if (purchaser.IsInitialized() && purchaser.IsItemAvailable(itemKey))
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
            purchaser.onInitializedSuccess += () =>
                gameObject.SetActive(purchaser.IsInitialized() && purchaser.IsItemAvailable(itemKey));
        }
    }
}