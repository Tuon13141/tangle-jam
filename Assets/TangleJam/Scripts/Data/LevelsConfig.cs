using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tuon
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "SOLevel/LevelsConfig", order = 0)]
    public sealed class LevelsConfig : ScriptableObject
    {
        public LevelAsset[] Loop;
        public Texture2D[] levelImages;

        public LevelAsset GetLevelData(int index)
        {
            if (Loop.Length == 0) return null;

            if (index <= Loop.Length)
            {
                return Loop[index - 1];
            }

            //var loopIndex = 10 + ((index - 1 - Loop.Length) % (Loop.Length - 10));

            //Debug.Log("Index : " + index + " -> Loop Index : " + loopIndex);

            //return Loop[loopIndex];
            int realIndex = (index - 1) % Loop.Length;

            Debug.Log($"Index: {index} -> Loop Index: {realIndex}");

            return Loop[realIndex];
        }

        public void Analytic()
        {

        }

        [Button]
        public void LoadAllImages()
        {
            levelImages = Resources.LoadAll<Texture2D>("LevelImagesUpdate");
        }

        [Button]
        public void SetupNewImageToLevel()
        {
            var s = "Level_";
            foreach (var texture in levelImages)
            {
                var index = texture.name[s.Length..];
                if (int.TryParse(index, out var level))
                {
                    var levelBase = GetLevelData(level);
                    if (level > 104)
                    {
                        levelBase = GetLevelData(level - 2);
                    }
                    var gridColors = Extract64Colors(texture);
                    var grouped = gridColors.GroupBy(x => ColorUtility.ToHtmlStringRGB(x)).ToList();
                    Debug.Log(grouped.Count());
                    //ColorUtility.ToHtmlStringRGB(color);
                    var colorList = grouped.Select(x => x.Key).ToList();
                    levelBase.PixelData.Colors = grouped.Select(x => x.First()).ToArray();
                    levelBase.PixelData.Data = gridColors.Select(x => ((byte)colorList.IndexOf(ColorUtility.ToHtmlStringRGB(x)))).ToArray();
#if UNITY_EDITOR
                    EditorUtility.SetDirty(levelBase);
#endif
                }
            }
        }

        private List<Color> Extract64Colors(Texture2D sourceTexture)
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
                    Color averageColor = GetAverageColorInCell(sourceTexture, x, y, cellWidth, cellHeight);
                    colors.Add(averageColor);
                }
            }

            return colors;
        }

        Color GetAverageColorInCell(Texture2D sourceTexture, int cellX, int cellY, int cellWidth, int cellHeight)
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

#if UNITY_EDITOR
        [Header("EDITOR")]
        public string targetFolder = "Assets/ThreadPuzzle/LevelDatas";
        [Button]
        void Move()
        {
            foreach (var myScriptableObject in Loop)
            {
                // Get the asset path of the ScriptableObject
                string sourcePath = AssetDatabase.GetAssetPath(myScriptableObject);

                if (string.IsNullOrEmpty(sourcePath))
                {
                    Debug.LogError("Could not find the asset path for the selected ScriptableObject.");
                    return;
                }

                // Make sure the folder exists, or create it
                if (!AssetDatabase.IsValidFolder(targetFolder))
                {
                    AssetDatabase.CreateFolder("Assets", "TargetFolder");
                }

                // Get the asset's file name
                string fileName = System.IO.Path.GetFileName(sourcePath);

                // Specify the target path
                string targetPath = System.IO.Path.Combine(targetFolder, fileName);

                // Move the asset
                string result = AssetDatabase.MoveAsset(sourcePath, targetPath);

                // Check for errors
                if (string.IsNullOrEmpty(result))
                {
                    Debug.Log($"Successfully moved {myScriptableObject.name} to {targetPath}");
                }
                else
                {
                    Debug.LogError($"Failed to move asset: {result}");
                }

                // Refresh the AssetDatabase to update the changes
                AssetDatabase.Refresh();
            }
        }
#endif
    }
}


