using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorManager", menuName = "ScriptableObjects/ColorManager", order = 1)]
public class ColorManager : ScriptableObject
{
    [SerializeField] List<TupleSerialize<Color, Color, Color>> BackgroundColors;

    [Space]
    [SerializeField]
    private List<TupleSerialize<Color, Material, Color>> m_CoilColorMaterial;

    public List<TupleSerialize<Color, Material, Color>> coilColorMaterial => m_CoilColorMaterial;

    public Material GetCoilMaterial(Color coilColor)
    {
        return (from colorData in m_CoilColorMaterial where colorData.Value1 == coilColor select colorData.Value2).FirstOrDefault();
    }

    public int GetColorIndex(Color color)
    {
        return (from colorData in m_CoilColorMaterial where colorData.Value1 == color select m_CoilColorMaterial.IndexOf(colorData)).FirstOrDefault();
    }

#if UNITY_EDITOR
    public List<Color> newColors;
    [Button]
    public void AddToBgColors()
    {
        foreach (var color in newColors)
        {
            if (!BackgroundColors.Any(c => c.Value1 == color))
            {
                BackgroundColors.Add(new TupleSerialize<Color, Color, Color>(color, color, color));
            }
        }
    }
    [Button]
    public void AddToCoilColors()
    {
        foreach (var color in newColors)
        {
            if (!m_CoilColorMaterial.Any(c => c.Value1 == color))
            {
                m_CoilColorMaterial.Add(new TupleSerialize<Color, Material, Color>(color, null, color));
            }
        }
    }
    [Button]
    public void ClearNewColors()
    {
        newColors.Clear();
    }
#endif

}
