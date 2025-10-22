using NaughtyAttributes;
using System.Linq;
using UnityEngine;

namespace Tuon
{
    [ExecuteAlways]
    public class GridColorPicker : MonoBehaviour
    {
        [ReadOnly, ShowAssetPreview] public Texture2D texture;
        public PictureAsset pictureAsset;

        public Color[] gridColors = new Color[1024];
        public Color[] currentGridColors = new Color[1024];

        public Renderer m_Renderer;

        void Awake()
        {
            texture = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
            m_Renderer.sharedMaterial.mainTexture = texture;
            ResetPicker();

            if (pictureAsset != null) gridColors = pictureAsset.GetColors();
        }

        public void ResetPicker()
        {
            currentGridColors = Enumerable.Repeat(new Color(0.75f, 0.75f, 0.75f, 0), 1024).ToArray();
            ApplyGridColors();
        }

        public void ApplyGridColors()
        {
            int blockSize = 32; // Calculate block size (32x32 for 1024x1024)

            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    // Fill the block for this color
                    var color = currentGridColors[32 * y + x];
                    for (int z = 0; z < blockSize; z++)
                    {
                        for (int w = 0; w < blockSize; w++)
                        {
                            texture.SetPixel(x * blockSize + z, y * blockSize + w, color);
                        }
                    }
                }
            }

            texture.Apply();
        }

        [Button]
        public void ViewPicture()
        {
            int blockSize = 32; // Calculate block size (32x32 for 1024x1024)

            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    // Fill the block for this color
                    var color = gridColors[32 * y + x];
                    for (int z = 0; z < blockSize; z++)
                    {
                        for (int w = 0; w < blockSize; w++)
                        {
                            texture.SetPixel(x * blockSize + z, y * blockSize + w, color);
                        }
                    }
                }
            }

            texture.Apply();
        }

        public void FillColor(Vector2Int index)
        {
            var blockSize = 32;
            var color = gridColors[32 * index.y + index.x];
            currentGridColors[32 * index.y + index.x] = color;

            for (int z = 0; z < blockSize; z++)
            {
                for (int w = 0; w < blockSize; w++)
                {
                    texture.SetPixel(index.x * blockSize + z, index.y * blockSize + w, color);
                }
            }

            texture.Apply();
        }
    }

}
