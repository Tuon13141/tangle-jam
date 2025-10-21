using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.WoolSort.Data
{
    public enum ColorType
    {
        White = 0,
        Blue = 1,
        Brow = 2,
        Cyan = 3,
        Green = 4,
        LightBlue = 5,
        Orange = 6,
        Pink = 7,
        Purple = 8,
        Red = 9,
        Yellow = 10,
    }

    //[CreateAssetMenu]
    public class ColorManager : ScriptableObject
    {
        [SerializeField] TupleSerialize<ColorType, Sprite>[] m_WoolSprites;
        [SerializeField] TupleSerialize<ColorType, Sprite>[] m_SlotMatchSprites;
        [SerializeField] TupleSerialize<ColorType, Sprite>[] m_PixelMatchSprites;

        [SerializeField] TupleSerialize<ColorType, Sprite, Sprite>[] m_ThreadWools;
        [SerializeField] TupleSerialize<ColorType, Color>[] m_RopeColors;
        [SerializeField] TupleSerialize<ColorType, Material>[] m_ShapeMaterial;

        public Sprite GetWoolSprite(ColorType colorType)
        {
            foreach (var element in m_WoolSprites)
            {
                if (element.Value1 == colorType) return element.Value2;
            }

            return m_WoolSprites[0].Value2;
        }

        public Sprite GetSlotMatchSprite(ColorType colorType)
        {
            foreach (var element in m_SlotMatchSprites)
            {
                if (element.Value1 == colorType) return element.Value2;
            }

            return m_SlotMatchSprites[0].Value2;
        }

        public Sprite GetThreadWool1Sprite(ColorType colorType)
        {
            foreach (var element in m_ThreadWools)
            {
                if (element.Value1 == colorType) return element.Value2;
            }

            return m_ThreadWools[0].Value2;
        }

        public Sprite GetThreadWool2Sprite(ColorType colorType)
        {
            foreach (var element in m_ThreadWools)
            {
                if (element.Value1 == colorType) return element.Value3;
            }

            return m_ThreadWools[0].Value3;
        }

        public Color GetRopeColor(ColorType colorType)
        {
            foreach (var element in m_RopeColors)
            {
                if (element.Value1 == colorType) return element.Value2;
            }

            return m_RopeColors[0].Value2;
        }

        public Sprite GetPixelSprite(ColorType colorType)
        {
            foreach (var element in m_PixelMatchSprites)
            {
                if (element.Value1 == colorType) return element.Value2;
            }

            return m_PixelMatchSprites[0].Value2;
        }

        public Material GetShapeMaterial(ColorType colorType)
        {
            foreach (var element in m_ShapeMaterial)
            {
                if (element.Value1 == colorType) return element.Value2;
            }

            return m_ShapeMaterial[0].Value2;
        }
    }
}
