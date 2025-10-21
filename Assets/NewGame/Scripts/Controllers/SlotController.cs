using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Game.WoolSort.Controller
{
    using Game.WoolSort.Element;
    using Game.WoolSort.Data;

    public class SlotController : MonoBehaviour
    {
        [SerializeField] SlotMatchElement[] m_SlotMatchElements;

        [SerializeField] SlotElement[] m_SlotElements;

        [SerializeField] Transform m_StartPoint;

        [Space]
        public LevelData levelData;
        public List<byte> orderSlot;

        int countSlotShow = 3;
        public async void Setup(LevelData levelData)
        {
            this.levelData = levelData;

            orderSlot = levelData.orderSlot.ToList();

            foreach (var slotMatch in m_SlotMatchElements)
            {
                if (orderSlot.Count == 0) break;

                var data = orderSlot[0];
                orderSlot.RemoveAt(0);

                slotMatch.color = (ColorType)data;
                slotMatch.ChangeColor(slotMatch.color);
                _ = slotMatch.transform.DOMove(slotMatch.transform.position, 0.3f).From(m_StartPoint.position).SetEase(Ease.OutBack);
                await UniTask.Delay(100);
            }
        }

        public void AddWoolToSlot(WoolElement woolElement)
        {
            if (CheckSlotMatch(woolElement.color, out var slotMatch))
            {
                woolElement.isComplete = true;
                slotMatch.AddWoolElement(woolElement);
            }
            else if (CheckSlot(out var slot))
            {
                woolElement.isComplete = true;
                slot.AddWoolElement(woolElement);
            }
            else
            {
                Debug.LogError("Do not have slot empty");
            }

            CheckGameLose();
        }

        public bool CheckSlotMatch(ColorType colorType, out SlotMatchElement slotMatch)
        {
            slotMatch = null;

            foreach (var slot in m_SlotMatchElements)
            {
                if (slot.isLock || slot.isFull || slot.isFillPicture || slot.isMove) continue;

                if (slot.color == colorType)
                {
                    if (slotMatch == null || slotMatch.countFill < slot.countFill)
                        slotMatch = slot;
                }
            }

            if (slotMatch != null) return true;

            return false;
        }

        public bool CheckSlot(out SlotElement slotEmpty)
        {
            foreach (var slot in m_SlotElements)
            {
                if (slot.isFull) continue;

                slotEmpty = slot;
                return true;
            }

            slotEmpty = null;
            return false;
        }

        public async void ReSpawnSlotMatch(SlotMatchElement slotMatch)
        {
            int data = -1;
            if (orderSlot.Count > 0)
            {
                data = orderSlot[0];
                orderSlot.RemoveAt(0);
                slotMatch.countFill = 0;
            }

            slotMatch.isMove = true;

            Vector3 rootPos = slotMatch.transform.position;
            await slotMatch.transform.DOMove(m_StartPoint.position, 0.3f).SetEase(Ease.InBack).ToUniTask();
            CheckGameWin();

            if (data != -1)
            {
                slotMatch.color = (ColorType)data;
                slotMatch.ChangeColor(slotMatch.color);
                await slotMatch.transform.DOMove(rootPos, 0.3f).From(m_StartPoint.position).SetEase(Ease.OutBack).ToUniTask();
                slotMatch.isMove = false;

                CheckGameLose();

                foreach (var slot in m_SlotElements)
                {
                    if (slotMatch.isFull) break;

                    if (!slot.isFull || slot.isFillSlotMatch) continue;
                    if (slot.colorFill == slotMatch.color)
                    {
                        slotMatch.AddWoolElementFromSlot(slot);
                        await UniTask.Delay(300);
                    }
                }
            }
        }

        public async void CheckGameLose()
        {
            if (!m_SlotElements.Any(x => !x.isFull && !x.isFillSlotMatch))
            {
                var count = m_SlotMatchElements.Count(x => x.isFull || x.isFillPicture || x.isMove);
                var colorInSlot = m_SlotElements.Select(x => (byte)x.colorFill);
                var check1 = orderSlot.Take(count).Any(x => colorInSlot.Contains(x));
                var check2 = m_SlotMatchElements.Where(x => !x.isFull).Select(x => (byte)x.color).Any(x => colorInSlot.Contains(x));

                if (check1 || check2) return;

                Debug.LogError("Game Lose!");

                LevelController.instance.isEndGame = true;

                await UniTask.Delay(500);
                LevelController.instance.ShowPopUpLose();
            }
        }

        public void CheckGameWin()
        {
            if (!m_SlotMatchElements.Any(x => x.transform.position != m_StartPoint.position))
            {
                Debug.LogError("Game Win!");
                LevelController.instance.isEndGame = true;

                LevelController.instance.ShowPopUpWin();
            }
        }
    }
}
