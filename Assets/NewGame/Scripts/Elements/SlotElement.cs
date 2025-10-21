using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Game.WoolSort.Element
{
    using Game.WoolSort.Controller;
    using Game.WoolSort.Data;

    public class SlotElement : MonoBehaviour
    {
        [SerializeField] ColorManager m_ColorManager;
        [SerializeField] PrefabManager m_PrefabManager;

        [Space]
        [SerializeField] SpriteRenderer[] m_WoolRenderers;

        [Space]
        [Header("ReadOnly")]
        public bool isFull;
        public bool isFillSlotMatch;
        public ColorType colorFill;

        public SpriteRenderer[] woolRenderers => m_WoolRenderers;

        float timeAdd = 0.4f;
        Vector3 offset = new Vector3(-0.15f, 0, 0);
        public async void AddWoolElement(WoolElement woolElement)
        {
            woolElement.status = WoolStatus.Transfer;
            isFull = true;
            colorFill = woolElement.color;

            _ = woolElement.pivot.transform.DOScale(0, timeAdd).From(1).SetEase(Ease.InBack);
            var rope = PoolingController.Instance.Spawn(m_PrefabManager.ropePrefab, transform.position, Quaternion.identity);
            rope.SetWaveRange(woolElement.pivot.position, woolElement.pivot.position);
            rope.SetColor(m_ColorManager.GetRopeColor(colorFill));

            float timeFade = (timeAdd * 0.8f) / m_WoolRenderers.Length;
            _ = rope.endPoint.DOMove(transform.position + offset, timeAdd * 0.2f).OnComplete(async () =>
                {
                    foreach (var renderer in m_WoolRenderers)
                    {
                        renderer.gameObject.SetActive(true);
                        renderer.sprite = m_ColorManager.GetThreadWool1Sprite(colorFill);
                        _ = rope.endPoint.DOMove(renderer.transform.position, timeFade);

                        await renderer.DOFade(1, timeFade).From(0).ToUniTask();
                    }
                });

            await UniTask.Delay(Mathf.CeilToInt(timeAdd * 1000));
            await rope.startPoint.DOMove(transform.position - offset, timeAdd * 0.2f).ToUniTask();

            woolElement.status = WoolStatus.Hide;
            woolElement.gameObject.SetActive(false);

            PoolingController.Instance.Release(rope);
        }
    }
}
