using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Tuon
{
    public class PinControlElement : MonoBehaviour
    {
        [ReadOnly] public StageData.CellData cellData;
        [ReadOnly] public List<PinWallElement> pinWallElements = new List<PinWallElement>();

        public GridElement gridElement;

        public void Setup(StageData.CellData cellData, GridElement gridElement, Matrix<GridElement> map)
        {
            this.cellData = cellData;
            this.gridElement = gridElement;

            GridElement gridTarget = null;

            switch (cellData.Direction)
            {
                case StageData.Direction.Up:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(-90);
                    gridTarget = map[gridElement.indexGrid.x, gridElement.indexGrid.y - 1];
                    for (int i = 0; i < 12; i++)
                    {
                        var grid = map[gridElement.indexGrid.x, gridElement.indexGrid.y + i];
                        if (grid == null) break;

                        if (grid.cellType == StageData.CellType.PinWall)
                        {
                            var pinWall = grid.GetComponentInChildren<PinWallElement>();
                            if (pinWall != null && !pinWallElements.Contains(pinWall)) pinWallElements.Add(pinWall);
                        }
                    }
                    break;

                case StageData.Direction.Down:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(90);
                    gridTarget = map[gridElement.indexGrid.x, gridElement.indexGrid.y + 1];
                    for (int i = 0; i < 12; i++)
                    {
                        var grid = map[gridElement.indexGrid.x, gridElement.indexGrid.y - i];
                        if (grid == null) break;

                        if (grid.cellType == StageData.CellType.PinWall)
                        {
                            var pinWall = grid.GetComponentInChildren<PinWallElement>();
                            if (pinWall != null && !pinWallElements.Contains(pinWall)) pinWallElements.Add(pinWall);
                        }
                    }
                    break;

                case StageData.Direction.Left:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(180);
                    gridTarget = map[gridElement.indexGrid.x + 1, gridElement.indexGrid.y];
                    for (int i = 0; i < 12; i++)
                    {
                        var grid = map[gridElement.indexGrid.x - i, gridElement.indexGrid.y];
                        if (grid == null) break;

                        if (grid.cellType == StageData.CellType.PinWall)
                        {
                            var pinWall = grid.GetComponentInChildren<PinWallElement>();
                            if (pinWall != null && !pinWallElements.Contains(pinWall)) pinWallElements.Add(pinWall);
                        }
                    }
                    break;

                case StageData.Direction.Right:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(0);
                    gridTarget = map[gridElement.indexGrid.x - 1, gridElement.indexGrid.y];
                    for (int i = 0; i < 12; i++)
                    {
                        var grid = map[gridElement.indexGrid.x + i, gridElement.indexGrid.y];
                        if (grid == null) break;

                        if (grid.cellType == StageData.CellType.PinWall)
                        {
                            var pinWall = grid.GetComponentInChildren<PinWallElement>();
                            if (pinWall != null && !pinWallElements.Contains(pinWall)) pinWallElements.Add(pinWall);
                        }
                    }
                    break;
            }

            if (gridTarget != null) gridTarget.pinControlElements.Add(this);
        }

        public void SetupDirection(StageData.Direction direction)
        {
            switch (direction)
            {
                case StageData.Direction.Up:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(-90);
                    break;

                case StageData.Direction.Down:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(90);
                    break;

                case StageData.Direction.Left:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(180);
                    break;

                case StageData.Direction.Right:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(0);
                    break;
            }
        }

        public void DisablePin()
        {
            if (!gameObject.activeSelf) return;

            foreach (var pinWall in pinWallElements)
            {
                pinWall.DisablePin();
            }

            Percas.ActionEvent.OnReleasePin?.Invoke();

            gridElement.CheckTubeElenentTarget();
            gridElement.cellType = gridElement.coilElement != null ? StageData.CellType.Coil : StageData.CellType.Empty;
            gridElement.pinControlElements.Remove(this);
            gameObject.SetActive(false);
        }
    }
}

