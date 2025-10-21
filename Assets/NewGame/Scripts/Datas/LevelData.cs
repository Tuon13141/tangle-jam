using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.WoolSort.Data
{
    [System.Serializable]
    public class WoolData
    {
        public ColorType color;
        public Vector3 position;
    }

    [System.Serializable]
    public class ShapeData
    {
        public int type;
        public int layer;

        public ColorType color;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public WoolData[] woolDatas;
    }

    //[CreateAssetMenu]
    public class LevelData : ScriptableObject
    {
        //picture
        public int width = 32;
        public int height = 24;
        public Color[] colorsList;
        public ColorType[] colors;
        public byte[] data;

        public int[] stage;

        //slot
        public byte[] orderSlot;

        //grid
        public int countLayer;
        public ShapeData[] shapeDatas;

        public Color GetColor(int x, int y)
        {
            return colorsList[data[y * width + x]];
        }

        public Texture2D ToTexture2D()
        {
            Texture2D tex = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);

            int blockSize = tex.width / 32; // Calculate block size (32x32 for 1024x1024)

            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    // Fill the block for this color
                    var color = GetColor(x, y);
                    for (int z = 0; z < blockSize; z++)
                    {
                        for (int w = 0; w < blockSize; w++)
                        {
                            tex.SetPixel(x * blockSize + z, y * blockSize + w, color);
                        }
                    }
                }
            }

            tex.Apply(); // Apply the changes to the texture

            return tex;
        }
    }
}