using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomGetTrackingScript : MonoBehaviour
{
    public virtual IEnumerable<LogParameter> GetAdmobLog(int step, IEnumerable<LogParameter> input,AdTypeLog adType)
    {
        foreach (var logParameter in input)
        {
            yield return logParameter;
        }
    }
}
