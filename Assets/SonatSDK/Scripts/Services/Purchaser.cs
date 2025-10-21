using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Test
{
    public List<StoreProductDescriptor> StoreProductDescriptors;
}

public class Purchaser : BasePurchaser
{
    [ArrayElementTitleSonatSdk("storeProductId")]
    [SerializeField] private List<StoreProductDescriptor> storeProductDescriptors;
    public override List<StoreProductDescriptor> StoreProductDescriptors => storeProductDescriptors;

    public override Type GetThisType()
    {
        return typeof(BasePurchaser);
    }

//    [SerializeField] private Test test;
//    [ContextMenu(nameof(Load))]
//    public void Load()
//    {
//        test = JsonUtility.FromJson<Test>(System.IO.File.ReadAllText("Assets/save.txt"));
//        Debug.Log(test.StoreProductDescriptors.Count);
//    }
}

