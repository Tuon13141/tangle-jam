using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AskScriptBase : MonoBehaviour
{
    public Action OnConsent;
    public Action<bool> OnUpdateStatus;
    public virtual void Ask(Action onComplete, Action<bool> onUpdateStatus) 
    {
        this.OnConsent = onComplete;
        this.OnUpdateStatus = onUpdateStatus;
        StartCoroutine(IeAsk());
    }
   public abstract IEnumerator IeAsk();

   public abstract bool Ready { get; }
}
