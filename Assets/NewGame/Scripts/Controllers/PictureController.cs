using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using DG.Tweening;

namespace Game.WoolSort.Controller
{
    using Game.WoolSort.Data;
    using Game.WoolSort.Element;

    public class PictureController : MonoBehaviour
    {
        [SerializeField] ColorManager m_ColorManager;
        [SerializeField] PrefabManager m_PrefabManager;

        [Space]
        [SerializeField] Transform m_Container;
        [SerializeField] Transform m_ParentGrid;
        [SerializeField] ParticleSystem m_ParticleFill;

        [Space]
        [SerializeField] BoxCollider2D m_BoxCollider;
        [SerializeField] Grid m_Grid;

        [Space]
        public LevelData levelData;

        public byte[] colorsInPicture;
        public byte[] colorInGrid;

        Dictionary<byte, float> colorRatios;

        public int currentStage;

        public void Setup(LevelData levelData)
        {
            this.levelData = levelData;
            colorsInPicture = levelData.data.Select(x => (byte)levelData.colors[x]).ToArray();
            colorInGrid = levelData.shapeDatas.SelectMany(x => x.woolDatas.Select(x => (byte)x.color)).ToArray();

            Debug.Log(colorsInPicture.Length);
            Debug.Log(colorInGrid.Length);
            //var groupPixel = colorsInPicture.GroupBy(x => x);
            //var groupWool = colorInGrid.GroupBy(x => x);

            var pixelCounts = colorsInPicture.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            var woolCounts = colorInGrid.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

            //var allColors = pixelCounts.Keys.Union(woolCounts.Keys);

            colorRatios = pixelCounts.ToDictionary(
                x => x.Key,
                x =>
                {
                    woolCounts.TryGetValue(x.Key, out int woolCount);
                    pixelCounts.TryGetValue(x.Key, out int pixelCount);
                    return woolCount == 0 ? 0f : pixelCount / (woolCount / 3f);
                }
            );

            foreach (var e in colorRatios)
            {
                Debug.Log($"color: {e.Key}: {e.Value}");
            }

            currentStage = 1;
        }

        float timeFill = 1f;
        public async void Fill(SlotMatchElement slotMatch, ColorType color)
        {
            var sprite = m_ColorManager.GetPixelSprite(color);

            var listFill = GetListPixelFill(color);
            if (listFill.Count == 0)
            {
                Debug.LogError($"Error Fill Picture: Not have color - {color.ToString()}");
                return;
            }

            var rope = PoolingController.Instance.Spawn(m_PrefabManager.ropePrefab, transform.position, Quaternion.identity);
            var lastWoolPos = slotMatch.woolRenderers[^1].transform.position;
            var firstPixelPos = m_ParentGrid.TransformPoint(listFill[0].Item2);
            var lastPixelPos = m_ParentGrid.TransformPoint(listFill[^1].Item2);

            var ropeColor = m_ColorManager.GetRopeColor(color);
            rope.SetWaveRange(lastWoolPos, lastWoolPos);
            rope.SetColor(ropeColor);

            var timeShow = (timeFill * 0.9f) / listFill.Count;

            await rope.endPoint.DOMove(firstPixelPos, timeFill * 0.1f).OnComplete(async () =>
            {
                float countTime = 0;
                for (int i = 0; i < listFill.Count; i++)
                {
                    var pixel = (new GameObject("pixel", typeof(SpriteRenderer))).GetComponent<SpriteRenderer>();
                    pixel.sprite = sprite;
                    pixel.sortingOrder = 3;
                    pixel.transform.SetParent(m_ParentGrid);
                    pixel.transform.localPosition = listFill[i].Item2;

                    _ = rope.endPoint.DOMove(pixel.transform.position, timeShow);
                    var tweenDelay = pixel.transform.DOScale(Vector3.one * 1.045f, timeShow).From(0).SetEase(Ease.OutBack);

                    ParticleSystem.EmitParams emitParams = new();
                    emitParams.position = pixel.transform.position;
                    emitParams.startColor = ropeColor;
                    m_ParticleFill.Emit(emitParams, 1);

                    countTime += timeShow;
                    if (countTime >= Time.deltaTime)
                    {
                        countTime = 0;
                        await tweenDelay.ToUniTask();
                    }
                }
            }).ToUniTask();

            var timeFade = (timeFill * 0.8f) / slotMatch.woolRenderers.Length;
            for (int i = slotMatch.woolRenderers.Length - 1; i >= 0; i--)
            {
                var renderer = slotMatch.woolRenderers[i];
                renderer.gameObject.SetActive(true);
                _ = rope.startPoint.DOMove(renderer.transform.position, timeFade);
                await renderer.DOFade(0, timeFade).From(1).ToUniTask();
            }

            await UniTask.Delay(Mathf.CeilToInt(timeFill * 100));
            await rope.startPoint.DOMove(lastPixelPos, timeFill * 0.1f).ToUniTask();
            PoolingController.Instance.Release(rope);
            slotMatch.isFillPicture = false;

            LevelController.instance.slotController.ReSpawnSlotMatch(slotMatch);
        }

        public List<(ColorType, Vector3)> GetListPixelFill(ColorType color)
        {
            var result = new List<(ColorType, Vector3)>();

            var rowCheck = 0;
            var currentCountFill = 0;

            var a1 = 32 - levelData.stage.Take(currentStage - 1).Sum();
            var a2 = 32 - levelData.stage.Take(currentStage).Sum();

            for (int y = a1 - 1; y >= a2; y--)
            {
                for (int x = 0; x < 32; x++)
                {
                    var xIndex = (rowCheck % 2 == 0) ? x : (31 - x);
                    var index = 32 * y + xIndex;

                    if (colorRatios.TryGetValue((byte)color, out var value) && result.Count >= value) return result;

                    if (colorsInPicture[index] == (byte)color)
                    {
                        var pos = m_Grid.GetCellCenterLocal(new Vector3Int(xIndex, y) - new Vector3Int(16, 15, 0));
                        result.Add((color, pos));
                        colorsInPicture[index] = byte.MaxValue;
                    }
                }

                if (currentCountFill != result.Count)
                {
                    currentCountFill = result.Count;
                    rowCheck += 1;
                }
            }
            return result;
        }
    }
}