using DG.Tweening;
using NaughtyAttributes;
using Percas.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tuon
{
    [DefaultExecutionOrder(200)]
    public class PictureController : Kit.Common.Singleton<PictureController>
    {
        [SerializeField] BoxCollider m_BoxCollider;
        [SerializeField] GridColorPicker m_GridColorPicker;

        [SerializeField] Color m_ColorPinActive;
        [SerializeField] Color m_ColorPinDone;

        [HorizontalLine(2, EColor.Blue)]
        [ReadOnly, ShowAssetPreview(512, 512)] public Texture2D texturePreview;
        public Texture GetTextureLevel() => m_GridColorPicker.texture;

        public BoxCollider boxCollider => m_BoxCollider;

        public LevelAsset levelData;
        public PictureAsset pictureAsset;

        public List<Color> colorsInStage;
        public List<Color> pixelColorsInStage = new List<Color>();
        private List<TupleSerialize<Color, int, int>> _pixelPerColor = new();

        public void Setup(LevelAsset levelData)
        {
            this.levelData = levelData;

            UILevelLabel.OnSetupPhaseItem?.Invoke(1);
            TrackingManager.OnLevelStart?.Invoke();
            GlobalSetting.IsJustPlay = true;

            //pictureAsset = levelData.PictureAsset;
            //texturePreview = pictureAsset.ToTexture2D();

            m_GridColorPicker.gridColors = levelData.PixelData.GetColors();
            m_GridColorPicker.ResetPicker();

            //
            //Debug.Log(levelData.Stages[currentStage - 1].RandomCoils);
            var randomDataList = levelData.Stage.RandomCoils.Split(",").Select(x => int.Parse(x)).ToList();
            colorsInStage = randomDataList.Select(x => levelData.PixelData.Colors[x]).ToList();

            var a1 = 32;
            var a2 = 0;

            pixelColorsInStage.Clear();
            for (int y = a1 - 1; y >= a2; y--)
            {
                for (int x = 0; x < 32; x++)
                {
                    pixelColorsInStage.Add(m_GridColorPicker.gridColors[32 * y + x]);
                }
            }

            //
            transform.DOLocalMoveY(a2 * (-10f / 32f), 0.5f).From(-10);
            _pixelPerColor.Clear();
            var colors = colorsInStage.Distinct().ToList();
            foreach (var t in colors)
            {
                var totalPoint = pixelColorsInStage.Count(x => x == t);
                var totalColor = colorsInStage.Count(x => x == t);
                _pixelPerColor.Add(new TupleSerialize<Color, int, int>(t, totalPoint, totalColor));
            }
        }

        private Tween _tweenShakePicture;

        public void ShakePicture()
        {
            // transform.DOScale(1.01f, 0.1f).From(1f);
            // if (_tweenShakePicture != null) return;

            // _tweenShakePicture = transform.DOScale(1.01f, 0.1f).From(1f).SetEase(Ease.InQuart).OnComplete(() =>
            // {
            //     _tweenShakePicture?.Kill();
            //     _tweenShakePicture = null;
            // });
        }

        public void ResetShakePicture()
        {
            // transform.localScale = Vector3.one;
            // _tweenShakePicture?.Kill();
            // _tweenShakePicture = null;
        }

        public void NextStage()
        {
            ActionEvent.OnPhaseEnd?.Invoke();


            ActionEvent.OnPhaseStart?.Invoke();
            TrackingManager.OnPhaseStart?.Invoke();

            colorsInStage = levelData.Stage.RandomCoils.Split(",").Select(x => levelData.PixelData.Colors[int.Parse(x)]).ToList();

            var a1 = 32;
            var a2 = 0;

            pixelColorsInStage.Clear();
            for (int y = a1 - 1; y >= a2; y--)
            {
                for (int x = 0; x < 32; x++)
                {
                    pixelColorsInStage.Add(m_GridColorPicker.gridColors[32 * y + x]);
                }
            }

            transform.DOLocalMoveY(a2 * (-10f / 32f), 0.5f);

            _pixelPerColor.Clear();
            var colors = colorsInStage.Distinct().ToList();
            foreach (var t in colors)
            {
                var totalPoint = pixelColorsInStage.Count(x => x == t);
                var totalColor = colorsInStage.Count(x => x == t);
                _pixelPerColor.Add(new TupleSerialize<Color, int, int>(t, totalPoint, totalColor));
            }
        }

        public void CompleteLevel()
        {

        }

        public List<int> CurrenPointFill { get; set; }
        public List<(Vector3, Vector2Int)> GetLevelPointFill(Color colorMatch)
        {
            var rowCheck = 0;
            var currentCountFill = 0;
            var maxPoint = 0;
            foreach (var t in _pixelPerColor)
            {
                if (t.Value1 == colorMatch)
                {
                    if (t.Value3 <= 3)
                    {
                        maxPoint = t.Value2;
                        t.Value2 = 0;
                        t.Value3 = 0;
                    }
                    else
                    {
                        maxPoint = Mathf.RoundToInt(t.Value2 / (t.Value3 / 3.0f));
                        t.Value2 -= maxPoint;
                        t.Value3 -= 3;
                    }
                }
            }
            var vectorList = new List<(Vector3, Vector2Int)>();

            CurrenPointFill ??= new List<int>();

            var a1 = 32;
            var a2 = 0;
            //Debug.LogFormat("Check {0},{1}", a1, a2);

            for (int y = a1 - 1; y >= a2; y--)
            {
                for (int x = 0; x < 32; x++)
                {
                    if (vectorList.Count >= maxPoint)
                    {
                        goto return_value;
                    }
                    var xIndex = (rowCheck % 2 == 0) ? x : (31 - x);
                    var index = 32 * y + xIndex;

                    //Debug.LogFormat("Check in {0},{1} = {2}", xIndex, y, m_GridColorPicker.gridColors[index]);
                    if (m_GridColorPicker.gridColors[index] != colorMatch) continue;
                    if (m_GridColorPicker.gridColors[index] == m_GridColorPicker.currentGridColors[index]) continue;
                    if (CurrenPointFill != null && CurrenPointFill.Contains(index)) continue;

                    vectorList.Add(new(m_GridColorPicker.transform.TransformPoint(GetPosition(xIndex, y)), new Vector2Int(xIndex, y)));
                    CurrenPointFill.Add(index);
                }

                if (currentCountFill != vectorList.Count)
                {
                    currentCountFill = vectorList.Count;
                    rowCheck += 1;
                }
            }

        return_value:
            return vectorList;
        }

        Vector2Int size = new Vector2Int(32, 32);
        float offset = 1.286f;
        public Vector3 GetPosition(int x, int y)
        {
            var sizeX = (m_BoxCollider.bounds.size.x / size.x) / transform.lossyScale.x;
            var sizeY = offset * (m_BoxCollider.bounds.size.y / size.y) / transform.lossyScale.y;

            var positionX = x * sizeX;
            var positionY = (31 - y) * sizeY;

            return new Vector3(positionX, positionY, 0) + new Vector3(-15.5f * sizeX, -15.5f * sizeY, 0);
        }

        private int CheckSameColorInStage(int colorIndex)
        {
            return levelData.Stage.Data.Count(t => t.isCoil && t.Value == colorIndex);
        }

        public void FillColor(Vector2Int index)
        {
            m_GridColorPicker.FillColor(index);
        }

        //[Button]
        //public void GenMap()
        //{
        //    m_GridColorPicker.gridColors = Extract64Colors().ToArray();
        //    m_GridColorPicker.ApplyGridColors();
        //}

        [Button]
        public void GenMap1()
        {
            m_GridColorPicker.pictureAsset = pictureAsset;
            m_GridColorPicker.ViewPicture();

            texturePreview = pictureAsset.ToTexture2D();
        }

        //public List<Color> Extract64Colors()
        //{
        //    if (sourceTexture == null)
        //    {
        //        Debug.LogError("Source texture is not assigned!");
        //        return null;
        //    }

        //    int gridWidth = 32; // Fixed 8x8 grid
        //    int gridHeight = 32;
        //    int cellWidth = sourceTexture.width / gridWidth;
        //    int cellHeight = sourceTexture.height / gridHeight;

        //    List<Color> colors = new List<Color>();

        //    for (int y = 0; y < gridHeight; y++)
        //    {
        //        for (int x = 0; x < gridWidth; x++)
        //        {
        //            Color averageColor = GetAverageColorInCell(x, y, cellWidth, cellHeight);
        //            colors.Add(averageColor);
        //        }
        //    }

        //    return colors;
        //}

        //private Color GetAverageColorInCell(int cellX, int cellY, int cellWidth, int cellHeight)
        //{
        //    int startX = cellX * cellWidth;
        //    int startY = cellY * cellHeight;

        //    float r = 0, g = 0, b = 0, a = 0;
        //    int pixelCount = 0;

        //    // Loop through each pixel in the cell
        //    for (int y = startY; y < startY + cellHeight; y++)
        //    {
        //        for (int x = startX; x < startX + cellWidth; x++)
        //        {
        //            if (x >= sourceTexture.width || y >= sourceTexture.height) continue;

        //            Color pixelColor = sourceTexture.GetPixel(x, y);
        //            r += pixelColor.r;
        //            g += pixelColor.g;
        //            b += pixelColor.b;
        //            a += pixelColor.a;
        //            pixelCount++;
        //        }
        //    }

        //    // Return the average color
        //    return new Color(r / pixelCount, g / pixelCount, b / pixelCount, a / pixelCount);
        //}

    }
}

