using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Tuon
{
    public class TubeElement : MonoBehaviour
    {
        [SerializeField] PrefabManager m_PrefabManager;

        [HorizontalLine(2, EColor.Blue)]
        [SerializeField] Transform m_ModeRoot;
        [SerializeField] TextMeshPro m_CountText;

        [HorizontalLine(2, EColor.Blue)]
        [ReadOnly] public List<Color> coilValues;

        [HorizontalLine(2, EColor.Blue)]
        public List<int> coilElementDatas;
        public GridElement gridTarget;

        public StageData.CellData cellData;
        public int countCoint => cellData.Value;
        public TextMeshPro countText => m_CountText;
        public void UpdateCountText()
        {
            m_CountText.text = string.Format("{0}", coilValues.Count);
        }

        public void Setup(StageData.CellData cellData, GridElement gridElement, Matrix<GridElement> map)
        {
            if (cellData.Type != StageData.CellType.Stack) return;

            this.cellData = cellData;
            m_CountText.text = string.Format("{0}", countCoint);

            gridTarget = null;
            switch (cellData.Direction)
            {
                case StageData.Direction.Up:
                    m_ModeRoot.transform.localEulerAngles = m_ModeRoot.transform.localEulerAngles.SetY(90);
                    gridTarget = map[gridElement.indexGrid.x, gridElement.indexGrid.y + 1];
                    if (gridTarget != null)
                    {
                        gridTarget.tubeElement = this;
                    }

                    break;
                case StageData.Direction.Down:
                    m_ModeRoot.transform.localEulerAngles = m_ModeRoot.transform.localEulerAngles.SetY(-90);
                    gridTarget = map[gridElement.indexGrid.x, gridElement.indexGrid.y - 1];
                    if (gridTarget != null)
                    {
                        gridTarget.tubeElement = this;
                    }
                    break;
                case StageData.Direction.Left:
                    m_ModeRoot.transform.localEulerAngles = m_ModeRoot.transform.localEulerAngles.SetY(0);
                    gridTarget = map[gridElement.indexGrid.x - 1, gridElement.indexGrid.y];
                    if (gridTarget != null)
                    {
                        gridTarget.tubeElement = this;
                    }
                    break;
                case StageData.Direction.Right:
                    m_ModeRoot.transform.localEulerAngles = m_ModeRoot.transform.localEulerAngles.SetY(180);
                    gridTarget = map[gridElement.indexGrid.x + 1, gridElement.indexGrid.y];
                    if (gridTarget != null)
                    {
                        gridTarget.tubeElement = this;
                    }
                    break;
            }
        }

        public void SetupDirection(StageData.Direction direction)
        {
            switch (direction)
            {
                case StageData.Direction.Up:
                    m_ModeRoot.transform.localEulerAngles = m_ModeRoot.transform.localEulerAngles.SetY(90);
                    break;
                case StageData.Direction.Down:
                    m_ModeRoot.transform.localEulerAngles = m_ModeRoot.transform.localEulerAngles.SetY(-90);
                    break;
                case StageData.Direction.Left:
                    m_ModeRoot.transform.localEulerAngles = m_ModeRoot.transform.localEulerAngles.SetY(0);
                    break;
                case StageData.Direction.Right:
                    m_ModeRoot.transform.localEulerAngles = m_ModeRoot.transform.localEulerAngles.SetY(180);
                    break;
            }
        }

        private void Start()
        {
            if (gridTarget.cellType == StageData.CellType.Empty)
            {
                var coilElement = Static.InstantiateUtility(m_PrefabManager.GetCoilPrefab(), gridTarget.holder);
                gridTarget.coilElement = coilElement;
                gridTarget.cellType = StageData.CellType.Coil;

                var color = GetCoilActive();
                coilElement.ChangeColor(color);
                gridTarget.controller.coilElements.Add(coilElement);
                if (gridTarget.controller.stageData.tutorialType == LevelAsset.TutorialType.Stack)
                {
                    gridTarget.controller.coilTutorial.Add(coilElement);
                }
            }
        }

        public Color GetCoilActive()
        {
            var color = coilValues[0];
            coilValues.RemoveAt(0);
            UpdateCountText();

            return color;
        }

        public CoilElement GetTempCoilElementInTube(Color colorMatch, ref List<CoilElement> listCoilElemet)
        {
            foreach (var color in coilValues.ToList())
            {
                if (color == colorMatch)
                {
                    var coilElementTemp = PoolingManager.instance.coilElementPool.Get();
                    coilElementTemp.transform.SetParent(transform, false);
                    coilElementTemp.transform.localPosition = new Vector3(0, 1, 0);

                    coilElementTemp.ChangeColor(colorMatch);
                    coilElementTemp.ChangeStatus(CoilStatus.Disable);

                    listCoilElemet.Add(coilElementTemp);
                    coilValues.Remove(colorMatch);
                    UpdateCountText();

                    return coilElementTemp;
                }
            }

            return null;
        }

        public void ChangeColorWithIndex(int index, Color colorTemp)
        {
            if (index < 0 || index >= coilValues.Count) return;
            coilValues[index] = colorTemp;
            UpdateCountText();
        }
    }
}

