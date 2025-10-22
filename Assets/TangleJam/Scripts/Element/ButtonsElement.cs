using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Tuon
{
    public class ButtonsElement : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text m_CountText;
        [SerializeField] ParticleSystem m_ParticleCollect;
        [SerializeField] List<Renderer> m_Renderers;

        [Space()]
        [SerializeField] Material m_MaterialActive;
        [SerializeField] Material m_MaterialInActive;

        [HorizontalLine(2, EColor.Blue)]

        [ReadOnly] public StageData.CellData cellData;
        [ReadOnly] public GridElement gridElement;
        [ReadOnly] public bool isLock;

        public TMPro.TMP_Text countText => m_CountText;

        int currentButtons;
        public void Setup(StageData.CellData cellData, GridElement gridElement, Matrix<GridElement> map)
        {
            this.cellData = cellData;
            this.gridElement = gridElement;

            currentButtons = cellData.Value;
            UpdateCountText();
        }

        public async void CollectButtons()
        {
            if (currentButtons == 0 || isLock) return;

            currentButtons--;
            m_ParticleCollect.Play();

            UpdateCountText();

            if (currentButtons == 0)
            {
                gridElement.cellType = StageData.CellType.Empty;
                gridElement.CheckCoilActive();

                await UniTask.Delay(500);
                gridElement.CheckTubeElenentTarget();
                gameObject.SetActive(false);
            }
            else
            {
            }
        }

        public void UpdateCountText()
        {
            m_CountText.text = string.Format("{0}", currentButtons);
            if (currentButtons == 0)
                m_CountText.transform.DOScale(0, 0.5f).SetEase(Ease.InBack);
            else
                m_CountText.transform.DOScale(1.1f, 0.25f).SetEase(Ease.OutCubic).SetLoops(2, LoopType.Yoyo);

        }

        public void CheckMove()
        {
            isLock = false;
            //if (gridElement == null) gridElement = GetComponentInParent<GridElement>();

            //var map = gridElement.controller.GetMap();
            //var stackOut = new Stack<Vector2Int>();
            ////Debug.LogFormat("CheckMove {1}: {0}", gridElement.indexGrid, gridElement.transform.parent.parent.gameObject.name);
            //var checkisLock = !Utils.CanMoveToBusStopCells(map, gridElement.indexGrid, out stackOut);
            //if (isLock == checkisLock) return;

            //isLock = checkisLock;
            //if (isLock)
            //{
            //    m_Renderers.ForEach(x => x.sharedMaterial = m_MaterialInActive);
            //}
            //else
            //{
            //    m_Renderers.ForEach(x => x.sharedMaterial = m_MaterialActive);
            //}

            //Debug.LogFormat("CheckMove {2}: {0} - {1}", gridElement.indexGrid, result, gridElement.transform.parent.parent.gameObject.name);

            //Debug.Log("CheckMove ButtonStack: " + isLock);
        }
    }
}

