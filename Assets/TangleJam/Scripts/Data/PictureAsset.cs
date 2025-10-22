using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tuon
{
    public sealed class PictureAsset : ScriptableObject
    {
        //fields
        [ReadOnly] public int Width = 32;

        [ReadOnly] public int Height = 32;

        public Color[] Colors;

        [ReadOnly] public byte[] Data;

        //functions
        public PixelData GetPixelData()
        {
            var pixelData = new PixelData();
            pixelData.Width = Width;
            pixelData.Height = Height;
            pixelData.Colors = Colors;
            pixelData.Data = Data;

            return pixelData;
        }

        public byte GetId(int x, int y)
        {
            return Data[y * Width + x];
        }

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

        [HorizontalLine(2, EColor.Blue)]
        [ShowAssetPreview(512, 512)] public Texture2D sourceTexture;
        [Button]
        public void GenMap()
        {
            var gridColors = Extract64Colors();
            var grouped = gridColors.GroupBy(x => ColorUtility.ToHtmlStringRGB(x)).ToList();
            Debug.Log(grouped.Count());
            //ColorUtility.ToHtmlStringRGB(color);
            var colorList = grouped.Select(x => x.Key).ToList();
            Colors = grouped.Select(x => x.First()).ToArray();

            Data = gridColors.Select(x => ((byte)colorList.IndexOf(ColorUtility.ToHtmlStringRGB(x)))).ToArray();
            // m_GridColorPicker.ApplyGridColors();
            Debug.Log("Gen Done!");
        }

        public List<Color> Extract64Colors()
        {
            if (sourceTexture == null)
            {
                Debug.LogError("Source texture is not assigned!");
                return null;
            }

            int gridWidth = 32; // Fixed 8x8 grid
            int gridHeight = 32;
            int cellWidth = sourceTexture.width / gridWidth;
            int cellHeight = sourceTexture.height / gridHeight;

            List<Color> colors = new List<Color>();

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    Color averageColor = GetAverageColorInCell(x, y, cellWidth, cellHeight);
                    colors.Add(averageColor);
                }
            }

            return colors;
        }
        private Color GetAverageColorInCell(int cellX, int cellY, int cellWidth, int cellHeight)
        {
            int startX = cellX * cellWidth;
            int startY = cellY * cellHeight;

            float r = 0, g = 0, b = 0, a = 0;
            int pixelCount = 0;

            // Loop through each pixel in the cell
            for (int y = startY; y < startY + cellHeight; y++)
            {
                for (int x = startX; x < startX + cellWidth; x++)
                {
                    if (x >= sourceTexture.width || y >= sourceTexture.height) continue;

                    Color pixelColor = sourceTexture.GetPixel(x, y);
                    r += pixelColor.r;
                    g += pixelColor.g;
                    b += pixelColor.b;
                    a += pixelColor.a;
                    pixelCount++;
                }
            }

            // Return the average color
            return new Color(r / pixelCount, g / pixelCount, b / pixelCount, a / pixelCount);
        }
    }
}
