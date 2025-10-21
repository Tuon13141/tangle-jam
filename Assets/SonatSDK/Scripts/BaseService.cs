using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseService : MonoBehaviour
{
    public virtual Type GetThisType() => this.GetType();
    [SerializeField] private bool forceInitialized = true;

    public virtual bool Initialized { get; protected set; }
    
//    protected virtual void Awake()
//    {
//        CheckRegister();
//        if (Kernel.IsReady())
//            OnKernelLoaded();
//        else
//            StartCoroutine(WaitForOnKernelLoaded());
//    }

    private bool _register;
    public void CheckRegister()
    {
        if (!_register)
        {
            Register();
            _register = true;
        }
    }

    public virtual void ForceInit()
    {
        if (forceInitialized)
            Initialized = true;
    }

    protected virtual void Register()
    {
    }

    private IEnumerator WaitForOnKernelLoaded()
    {
        while (Kernel.IsReady())
            yield return null;
        OnKernelLoaded();
    }
    
    protected virtual void OnKernelLoaded()
    {
        
    }
}
