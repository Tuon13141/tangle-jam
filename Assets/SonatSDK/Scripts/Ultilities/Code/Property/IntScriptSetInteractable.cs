using System;
using System.Collections;
using System.Collections.Generic;
using SonatSdkUltilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntScriptSetInteractable : IntScript
{
    [SerializeField] private Button btn;
    [SerializeField] private Image[] imgs;
    [SerializeField] private TMP_Text[] texts;
    [SerializeField] private int value;
    [SerializeField] private Type type;
    [SerializeField] private Color colorOn;
    [SerializeField] private Color colorOff;
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
                btn.interactable = (val >= value);
                foreach (var image in imgs)
                    image.color = val >= value ? colorOn : colorOff;
                foreach (var image in texts)
                    image.color = val >= value ? colorOn : colorOff;
                break;
            case Type.SmallerOrEqual:
                btn.interactable = (val <= value);
                foreach (var image in imgs)
                    image.color = val <= value ? colorOn : colorOff;
                foreach (var image in texts)
                    image.color = val >= value ? colorOn : colorOff;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}