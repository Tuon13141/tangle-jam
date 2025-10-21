using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PixelData
{
    //fields
    public int Width = 32;

    public int Height = 32;

    public Color[] Colors;

    public byte[] Data;

    //functions
    public Color GetColor(int x, int y)
    {
        return Colors[Data[y * Width + x]];
    }

    public Color[] GetColors()
    {
        var size = Width * Height;
        var colors = new Color[size];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                colors[y * Width + x] = GetColor(x, y);
            }
        }

        return colors;
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
