using Elements;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Tuon
{
    public class GridElement : MonoBehaviour
    {
        [SerializeField] Transform m_Holder;
        [SerializeField] SpriteRenderer m_SpriteRenderer;
        [SerializeField] GameObject m_Model3DParent;
        [SerializeField] Sprite m_SpriteGridHardLevel;

        [HorizontalLine(2, EColor.Blue)]
        [SerializeField] GameObject m_Shape_1;
        [SerializeField] GameObject m_Shape_2;
        [SerializeField] GameObject m_Shape_3;
        [SerializeField] GameObject m_Shape_4;
        [SerializeField] GameObject m_Shape_5;
        [SerializeField] GameObject m_Shape_6;
        [SerializeField] private GameObject[] m_WallBeards;
        public Transform holder => m_Holder;

        [HorizontalLine(2, EColor.Blue)]
        [ReadOnly] public GridController controller;
        [ReadOnly] public StageData.CellType cellType;
        [ReadOnly] public Vector2Int indexGrid;

        [ReadOnly] public CoilElement coilElement;
        [ReadOnly] public TubeElement tubeElement;
        [ReadOnly] public ButtonsElement buttonsElement;
        [ReadOnly] public List<PinControlElement> pinControlElements = new();
        [ReadOnly] public PinWallElement pinWallElement;
        [ReadOnly] public KeyElement keyElements;
        [ReadOnly] public LockElement lockElements;

        public bool isWall => cellType == StageData.CellType.Wall;

        public void SetKeyElement(KeyElement keyElement)
        {
            keyElements = keyElement;
        }

        public void SetLockElement(LockElement lockElement)
        {
            lockElements = lockElement;
        }

        public void Setup(StageData.CellType cellType, GridController controller)
        {
            this.controller = controller;
            this.cellType = cellType;
            //if (controller?.levelData?.IsHard == true) m_SpriteRenderer.sprite = m_SpriteGridHardLevel;

            switch (cellType)
            {
                case StageData.CellType.Empty:
                    m_SpriteRenderer.gameObject.SetActive(true);
                    m_Model3DParent.SetActive(false);
                    break;

                case StageData.CellType.ButtonStack:
                case StageData.CellType.Stack:
                case StageData.CellType.CoilLocked:
                case StageData.CellType.CoilPair:
                case StageData.CellType.Coil:
                case StageData.CellType.Lock:
                case StageData.CellType.Key:
                    m_SpriteRenderer.gameObject.SetActive(true);
                    m_Model3DParent.SetActive(false);
                    break;

                case StageData.CellType.Wall:
                    m_SpriteRenderer.gameObject.SetActive(false);
                    m_Model3DParent.SetActive(true);
                    SetupWall();
                    break;

                case StageData.CellType.PinWall:
                    m_SpriteRenderer.gameObject.SetActive(true);
                    m_Model3DParent.SetActive(false);
                    break;

                case StageData.CellType.PinControl:
                    m_SpriteRenderer.gameObject.SetActive(true);
                    m_Model3DParent.SetActive(false);
                    break;
            }
        }

        public void SetActiveWallBeard(bool isActive)
        {
            foreach (var beard in m_WallBeards)
            {
                beard.SetActive(isActive);
            }
        }

        [Button]
        public void SetupWall()
        {
            var gridUp = controller.map[indexGrid.x, indexGrid.y + 1];
            var gridDown = controller.map[indexGrid.x, indexGrid.y - 1];
            var gridLeft = controller.map[indexGrid.x - 1, indexGrid.y];
            var gridRight = controller.map[indexGrid.x + 1, indexGrid.y];

            var listShape = new List<int>();
            var sum = 0;

            if (gridUp?.isWall == true) { listShape.Add(1); sum += 1; }
            if (gridDown?.isWall ?? true) { listShape.Add(2); sum += 2; }
            if (gridLeft?.isWall ?? true) { listShape.Add(4); sum += 4; }
            if (gridRight?.isWall ?? true) { listShape.Add(8); sum += 8; }

            m_Shape_1.SetActive(false);
            m_Shape_2.SetActive(false);
            m_Shape_3.SetActive(false);
            m_Shape_4.SetActive(false);
            m_Shape_5.SetActive(false);
            m_Shape_6.SetActive(false);

            //Debug.LogFormat("{0} - {1}", listShape.Count, sum);
            switch (listShape.Count)
            {
                case 4:
                    m_Shape_1.SetActive(true);
                    break;

                case 3:
                    m_Shape_2.SetActive(true);
                    switch (sum)
                    {
                        case 7:
                            m_Shape_2.transform.localEulerAngles = new Vector3(0, 90, 0);
                            break;
                        case 11:
                            m_Shape_2.transform.localEulerAngles = new Vector3(0, -90, 0);
                            break;
                        case 13:
                            m_Shape_2.transform.localEulerAngles = new Vector3(0, 180, 0);
                            break;
                        case 14:
                            m_Shape_2.transform.localEulerAngles = new Vector3(0, 0, 0);
                            break;

                    }
                    break;

                case 2:
                    switch (sum)
                    {
                        case 3:
                            m_Shape_3.SetActive(true);
                            m_Shape_3.transform.localEulerAngles = new Vector3(0, 90, 0);
                            break;
                        case 12:
                            m_Shape_3.SetActive(true);
                            m_Shape_3.transform.localEulerAngles = new Vector3(0, 180, 0);
                            break;
                        case 5:
                            m_Shape_4.SetActive(true);
                            m_Shape_4.transform.localEulerAngles = new Vector3(0, 90, 0);
                            break;
                        case 9:
                            m_Shape_4.SetActive(true);
                            m_Shape_4.transform.localEulerAngles = new Vector3(0, 180, 0);
                            break;
                        case 6:
                            m_Shape_4.SetActive(true);
                            m_Shape_4.transform.localEulerAngles = new Vector3(0, 0, 0);
                            break;
                        case 10:
                            m_Shape_4.SetActive(true);
                            m_Shape_4.transform.localEulerAngles = new Vector3(0, -90, 0);
                            break;

                    }
                    break;

                case 1:
                    m_Shape_5.SetActive(true);
                    switch (sum)
                    {
                        case 1:
                            m_Shape_5.transform.localEulerAngles = new Vector3(0, 180, 0);
                            break;
                        case 2:
                            m_Shape_5.transform.localEulerAngles = new Vector3(0, 0, 0);
                            break;
                        case 4:
                            m_Shape_5.transform.localEulerAngles = new Vector3(0, 90, 0);
                            break;
                        case 8:
                            m_Shape_5.transform.localEulerAngles = new Vector3(0, -90, 0);
                            break;
                    }
                    break;

                case 0:
                    m_Shape_6.SetActive(true);
                    break;
            }
        }

        public void CheckCoilActive()
        {
            Debug.LogFormat("CoilInGridTap in: {0}, {1}", indexGrid.x, indexGrid.y);
            if (tubeElement != null && tubeElement.coilValues.Count > 0)
            {
                if (coilElement != null) coilElement.coilStatus = CoilStatus.Transfer;
                else cellType = StageData.CellType.Wall;
            }

            if (pinControlElements != null && (tubeElement?.coilValues.Count ?? 0) == 0)
            {
                pinControlElements.ForEach(x => x.DisablePin());
            }

            //check move all
            controller.CheckMoveAll();

            //check mystery coil
            var gridUp = controller.map[indexGrid.x, indexGrid.y + 1];
            var gridDown = controller.map[indexGrid.x, indexGrid.y - 1];
            var gridLeft = controller.map[indexGrid.x - 1, indexGrid.y];
            var gridRight = controller.map[indexGrid.x + 1, indexGrid.y];

            if (gridUp?.coilElement != null) gridUp.coilElement.ActiveMysteryCoil();
            if (gridDown?.coilElement != null) gridDown.coilElement.ActiveMysteryCoil();
            if (gridLeft?.coilElement != null) gridLeft.coilElement.ActiveMysteryCoil();
            if (gridRight?.coilElement != null) gridRight.coilElement.ActiveMysteryCoil();

            if (gridUp?.keyElements != null) gridUp.keyElements.DisableKey();
            if (gridDown?.keyElements != null) gridDown.keyElements.DisableKey();
            if (gridLeft?.keyElements != null) gridLeft.keyElements.DisableKey();
            if (gridRight?.keyElements != null) gridRight.keyElements.DisableKey();

            controller.CheckMoveAll();
        }

        public void CheckTubeElenentTarget()
        {
            if (tubeElement != null)
            {
                if (tubeElement.coilValues.Count > 0)
                {
                    if (coilElement == null)
                    {
                        coilElement = PoolingManager.instance.coilElementPool.Get();
                        coilElement.transform.SetParent(holder, false);
                    }
                    coilElement.ChangeColor(tubeElement.GetCoilActive());
                    coilElement.ChangeStatus(CoilStatus.Enable);
                    cellType = StageData.CellType.Coil;
                    controller.coilElements.Add(coilElement);
                    if (controller.stageData.tutorialType == LevelAsset.TutorialType.Stack)
                    {
                        controller.coilTutorial.Add(coilElement);
                    }
                }
            }
        }
    }
}

