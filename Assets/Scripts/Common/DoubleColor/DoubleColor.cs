using System;
using UnityEngine;

[Serializable]
public class DoubleColor
{
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;

    public DoubleColor() { }

    public DoubleColor(Color color1, Color color2)
    {
        this.color1 = color1;
        this.color2 = color2;
    }

    public Color Color1 => color1;
    public Color Color2 => color2;
}

