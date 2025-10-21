using System;
using System.Collections;
using System.Collections.Generic;
using SonatSdkUltilities;
using UnityEngine;

public class IntScriptSetActive : IntScript
{
    [SerializeField] private int value;
    [SerializeField] private  Type type;
    public enum Type
    {
        GreaterOrEqual,
        SmallerOrEqual
    }
   
    public override void OnChanged(int val)
    {
        switch (type)
        {
            case Type.GreaterOrEqual:
                gameObject.SetActive(val >= value);
                break;
            case Type.SmallerOrEqual:
                gameObject.SetActive(val <= value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
