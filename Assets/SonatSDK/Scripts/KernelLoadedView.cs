using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KernelLoadedView : MonoBehaviour
{
    
    protected virtual void Start()
    {
        if (Kernel.IsReady())
            OnKernelLoaded();
        else
            StartCoroutine(WaitForOnKernelLoaded());
    }
    
    private IEnumerator WaitForOnKernelLoaded()
    {
        while (!Kernel.IsReady())
            yield return null;
        OnKernelLoaded();
    }
    
    protected virtual void OnKernelLoaded()
    {
        // put your init code her
    }
}
