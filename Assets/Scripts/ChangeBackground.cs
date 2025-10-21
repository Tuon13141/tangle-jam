using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBackground : MonoBehaviour
{

    public Color color1 { get; set; }
    public Color color2 { get; set; }

    public void Apply()
    {
        LevelController.instance?.ChangeColorBackground(new TupleSerialize<Color, Color, Color>(color1, color2, color1));
    }
}
