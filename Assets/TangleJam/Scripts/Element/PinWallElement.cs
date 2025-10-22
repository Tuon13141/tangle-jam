using NaughtyAttributes;
using UnityEngine;

namespace Tuon
{
    public class PinWallElement : MonoBehaviour
    {
        [ReadOnly] public StageData.CellData cellData;

        public GridElement gridElement;
        public void Setup(StageData.CellData cellData, GridElement gridElement, Matrix<GridElement> map)
        {
            this.cellData = cellData;
            this.gridElement = gridElement;

            switch (cellData.Direction)
            {
                case StageData.Direction.Up:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(90);
                    for (int i = 0; i < 12; i++)
                    {
                        var gridTarget = map[gridElement.indexGrid.x, gridElement.indexGrid.y - i];
                        if (gridTarget == null) break;

                        if (gridTarget.cellType == StageData.CellType.PinControl)
                        {
                            var pinControl = gridTarget.GetComponentInChildren<PinControlElement>();
                            if (pinControl != null && !pinControl.pinWallElements.Contains(this))
                            {
                                pinControl.pinWallElements.Add(this);
                                break;
                            }
                        }
                    }
                    break;

                case StageData.Direction.Down:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(-90);
                    for (int i = 0; i < 12; i++)
                    {
                        var gridTarget = map[gridElement.indexGrid.x, gridElement.indexGrid.y + i];
                        if (gridTarget == null) break;

                        if (gridTarget.cellType == StageData.CellType.PinControl)
                        {
                            var pinControl = gridTarget.GetComponentInChildren<PinControlElement>();
                            if (pinControl != null && !pinControl.pinWallElements.Contains(this))
                            {
                                pinControl.pinWallElements.Add(this);
                                break;
                            }
                        }
                    }
                    break;

                case StageData.Direction.Left:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(0);
                    for (int i = 0; i < 12; i++)
                    {
                        var gridTarget = map[gridElement.indexGrid.x + i, gridElement.indexGrid.y];
                        if (gridTarget == null) break;

                        if (gridTarget.cellType == StageData.CellType.PinControl)
                        {
                            var pinControl = gridTarget.GetComponentInChildren<PinControlElement>();
                            if (pinControl != null && !pinControl.pinWallElements.Contains(this))
                            {
                                pinControl.pinWallElements.Add(this);
                                break;
                            }
                        }
                    }
                    break;

                case StageData.Direction.Right:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(180);
                    for (int i = 0; i < 12; i++)
                    {
                        var gridTarget = map[gridElement.indexGrid.x - i, gridElement.indexGrid.y];
                        if (gridTarget == null) break;

                        if (gridTarget.cellType == StageData.CellType.PinControl)
                        {
                            var pinControl = gridTarget.GetComponentInChildren<PinControlElement>();
                            if (pinControl != null && !pinControl.pinWallElements.Contains(this))
                            {
                                pinControl.pinWallElements.Add(this);
                                break;
                            }
                        }
                    }
                    break;
            }
        }

        public void SetupDirection(StageData.Direction direction)
        {
            switch (direction)
            {
                case StageData.Direction.Up:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(90);
                    break;

                case StageData.Direction.Down:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(-90);
                    break;

                case StageData.Direction.Left:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(0);
                    break;

                case StageData.Direction.Right:
                    transform.localEulerAngles = transform.localEulerAngles.SetY(180);
                    break;
            }
        }
        public void DisablePin()
        {
            if (!gameObject.activeSelf) return;

            gridElement.CheckTubeElenentTarget();
            gridElement.cellType = gridElement.coilElement != null ? StageData.CellType.Coil : StageData.CellType.Empty;
            gameObject.SetActive(false);
        }
    }
}
