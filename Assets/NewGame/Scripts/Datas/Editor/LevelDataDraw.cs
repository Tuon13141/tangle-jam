using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.WoolSort.Data
{
    [CustomEditor(typeof(LevelData))]
    public class LevelDataDraw : UnityEditor.Editor
    {
        private LevelData _levelData;
        private Texture2D _texture;
        private string _analyticPicture;
        private List<ColorList> _colorsInStage = new List<ColorList>();

        private void OnEnable()
        {
            _levelData = (LevelData)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ImportPicture();
        }

        private void ImportPicture()
        {
            _texture = (Texture2D)EditorGUILayout.ObjectField("Enter Texture:", _texture, typeof(Texture2D), false);

            GUI.enabled = false;
            EditorGUILayout.TextArea(_analyticPicture);
            GUI.enabled = true;

            if (GUILayout.Button("Setup Picture"))
            {
                var gridColors = ExtractGridColors(_texture);
                var grouped = gridColors.GroupBy(ColorUtility.ToHtmlStringRGB).ToList();
                Debug.Log(grouped.Count());
                var colorList = grouped.Select(x => x.Key).ToList();
                _levelData.colorsList = grouped.Select(x => x.First()).ToArray();

                //_levelData.types = new();
                //foreach (Color color in _levelData.colorsList)
                //{
                //    _levelData.types.Add(_levelData.colorManager.GetEditorType(color));
                //}
                _levelData.data = gridColors.Select(x => (byte)colorList.IndexOf(ColorUtility.ToHtmlStringRGB(x))).ToArray();

                CheckColor();
                Debug.Log("Gen Done!");
            }
        }

        private void CheckColor()
        {
            if (_levelData?.colorsList == null) return;

            _colorsInStage = new List<ColorList>();

            var colorList = _levelData.colorsList.ToList();
            var pixelColorsInStage = new List<Color>();
            var s = string.Empty;

            pixelColorsInStage.Clear();
            for (var y = _levelData.height - 1; y >= 0; y--)
            {
                for (var x = 0; x < _levelData.width; x++)
                {
                    pixelColorsInStage.Add(colorList[_levelData.data[_levelData.width * y + x]]);
                }
            }

            var group = pixelColorsInStage.GroupBy(x => x);

            s = string.Format("{0}\n - slice {1}: {4} | {2} -> {3}", s, 0, group.Count(),
                string.Join(",", group.Select(x => colorList.IndexOf(x.First())).OrderBy(x => x).ToArray()),
                pixelColorsInStage.Count);
            _colorsInStage.Add(new ColorList(group.Select(x => x.First()).ToList()));

            _analyticPicture = string.Format("Count Slices: {0}\n {1} ", 1, s);
        }

        private List<Color> ExtractGridColors(Texture2D sourceTexture)
        {
            if (sourceTexture == null)
            {
                Debug.LogError("Source texture is not assigned!");
                return null;
            }

            var gridWidth = _levelData.width;
            var gridHeight = _levelData.height;
            var cellWidth = sourceTexture.width / gridWidth;
            var cellHeight = sourceTexture.height / gridHeight;

            var colors = new List<Color>();
            for (var y = 0; y < gridHeight; y++)
            {
                for (var x = 0; x < gridWidth; x++)
                {
                    var averageColor = GetAverageColorInCell(sourceTexture, x, y, cellWidth, cellHeight);
                    colors.Add(averageColor);
                }
            }

            return colors;
        }

        private Color GetAverageColorInCell(Texture2D sourceTexture, int cellX, int cellY, int cellWidth,
            int cellHeight)
        {
            var startX = cellX * cellWidth;
            var startY = cellY * cellHeight;

            float r = 0, g = 0, b = 0, a = 0;
            var pixelCount = 0;

            for (var y = startY; y < startY + cellHeight; y++)
            {
                for (var x = startX; x < startX + cellWidth; x++)
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

            return new Color(r / pixelCount, g / pixelCount, b / pixelCount, a / pixelCount);
        }
    }
}