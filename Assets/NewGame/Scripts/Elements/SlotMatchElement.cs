using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game.WoolSort.Element
{
    using DG.Tweening;
    using Game.WoolSort.Data;
    using Game.WoolSort.Controller;

    public class SlotMatchElement : MonoBehaviour
    {
        [SerializeField] ColorManager m_ColorManager;
        [SerializeField] PrefabManager m_PrefabManager;

        [Space]
        [SerializeField] SpriteRenderer m_SlotRenderer;
        [SerializeField] SpriteRenderer[] m_WoolRenderers;

        [Space]
        [Header("ReadOnly")]
        public bool isLock;
        public bool isFillPicture;
        public bool isMove;

        public int countFill;
        public ColorType color;

        public bool isFull => countFill >= 3;
        public SpriteRenderer[] woolRenderers => m_WoolRenderers;

        private void OnValidate()
        {
            ChangeColor(color);
        }

        float timeAdd = 0.4f;
        public async void AddWoolElement(WoolElement woolElement)
        {
            woolElement.status = WoolStatus.Transfer;
            countFill++;

            var isFillPicture = false;
            if (countFill == 3) isFillPicture = true;
            if (countFill > 3) Debug.LogError("Error Add WoolElement");

            _ = woolElement.pivot.transform.DOScale(0, timeAdd).From(1).SetEase(Ease.InBack);

            var rope = PoolingController.Instance.Spawn(m_PrefabManager.ropePrefab, transform.position, Quaternion.identity);
            rope.SetWaveRange(woolElement.pivot.position, woolElement.pivot.position);
            rope.SetColor(m_ColorManager.GetRopeColor(color));

            int startIndex = (countFill - 1) * (m_WoolRenderers.Length / 3);
            int endIndex = countFill * (m_WoolRenderers.Length / 3);
            float timeFade = (timeAdd * 0.8f) / (m_WoolRenderers.Length / 3);
            _ = rope.endPoint.DOMove(m_WoolRenderers[startIndex].transform.position, timeAdd * 0.2f).OnComplete(async () =>
                {
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        var renderer = m_WoolRenderers[i];
                        renderer.gameObject.SetActive(true);
                        renderer.sprite = m_ColorManager.GetThreadWool2Sprite(color);
                        _ = rope.endPoint.DOMove(renderer.transform.position, timeFade);

                        await renderer.DOFade(1, timeFade).From(0).ToUniTask();
                    }
                });

            await UniTask.Delay(Mathf.CeilToInt(timeAdd * 1000));
            await rope.startPoint.DOMove(m_WoolRenderers[endIndex - 1].transform.position, timeAdd * 0.2f).ToUniTask();

            woolElement.status = WoolStatus.Hide;
            woolElement.gameObject.SetActive(false);
            PoolingController.Instance.Release(rope);

            if (isFillPicture)
            {
                this.isFillPicture = isFillPicture;
                await UniTask.DelayFrame(1);
                //fill picture
                Debug.Log("Fill to Picture");
                LevelController.instance.pictureController.Fill(this, color);

            }
        }

        public async void AddWoolElementFromSlot(SlotElement slotElement)
        {
            countFill++;
            var isFillPicture = false;
            slotElement.isFillSlotMatch = true;

            if (countFill == 3) isFillPicture = true;
            if (countFill > 3) { Debug.LogError($"Error Add WoolElement: {countFill}"); return; }

            var rope = PoolingController.Instance.Spawn(m_PrefabManager.ropePrefab, transform.position, Quaternion.identity);
            rope.SetWaveRange(slotElement.woolRenderers[^1].transform.position, slotElement.woolRenderers[^1].transform.position);
            rope.SetColor(m_ColorManager.GetRopeColor(color));

            int startIndex = (countFill - 1) * (m_WoolRenderers.Length / 3);
            int endIndex = countFill * (m_WoolRenderers.Length / 3);
            float timeFade = (timeAdd * 0.8f) / (m_WoolRenderers.Length / 3);
            await rope.endPoint.DOMove(m_WoolRenderers[startIndex].transform.position, timeAdd * 0.2f).OnComplete(async () =>
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    var renderer = m_WoolRenderers[i];
                    renderer.gameObject.SetActive(true);
                    renderer.sprite = m_ColorManager.GetThreadWool2Sprite(color);
                    _ = rope.endPoint.DOMove(renderer.transform.position, timeFade);

                    await renderer.DOFade(1, timeFade).From(0).ToUniTask();
                }
            }).ToUniTask();

            var timeFade2 = (timeAdd * 0.8f) / slotElement.woolRenderers.Length;
            for (int i = slotElement.woolRenderers.Length - 1; i >= 0; i--)
            {
                var renderer = slotElement.woolRenderers[i];
                renderer.gameObject.SetActive(true);
                _ = rope.startPoint.DOMove(renderer.transform.position, timeFade2);
                await renderer.DOFade(0, timeFade2).From(1).ToUniTask();
            }

            await UniTask.Delay(Mathf.CeilToInt(timeAdd * 100));
            await rope.startPoint.DOMove(m_WoolRenderers[endIndex - 1].transform.position, timeAdd * 0.2f).ToUniTask();

            slotElement.isFull = false;
            slotElement.isFillSlotMatch = false;
            PoolingController.Instance.Release(rope);

            if (isFillPicture)
            {
                this.isFillPicture = isFillPicture;
                await UniTask.DelayFrame(1);
                //fill picture
                Debug.Log("Fill to Picture");
                LevelController.instance.pictureController.Fill(this, color);

            }
        }

        public void ChangeColor(ColorType color)
        {
            //if (this.color == color) return;

            this.color = color;

            m_SlotRenderer.sprite = m_ColorManager.GetSlotMatchSprite(color);
        }
    }
}