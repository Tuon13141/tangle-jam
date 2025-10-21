using System;
using System.Collections;
using System.Collections.Generic;
using SonatSdkUltilities;
using UnityEngine;
using UnityEngine.UI;

public class IntScriptSetSprite : IntScript
{
    [SerializeField] private int value;
    [SerializeField] private CompareType compareType;
    [SerializeField] private Image image;
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;

    public override void OnChanged(int val)
    {
        switch (compareType)
        {
            case CompareType.GreaterOrEqual:
                image.sprite = val >= value ? spriteOn : spriteOff;
                break;
            case CompareType.SmallerOrEqual:
                image.sprite = val <= value ? spriteOn : spriteOff;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}