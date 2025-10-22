using Cysharp.Threading.Tasks;
using DG.Tweening;
using Elements;
using NaughtyAttributes;
using Percas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tuon
{
    [DefaultExecutionOrder(200)]
    public class GridController : MonoBehaviour
    {
        [SerializeField] Grid m_Grid;
        [SerializeField] PrefabManager m_PrefabManager;
        [SerializeField] Transform m_ParentGrid;
        [SerializeField] Transform m_WallBottom;
        [SerializeField] Transform m_WallRight;
        [SerializeField] Transform m_WallLeft;
        public RollElement roll;
        public RollElement rollCollect;
        public ParticleSystem shuffleParticle;

        [HorizontalLine(2, EColor.Blue)]
        public Vector2Int size;
        public Matrix<GridElement> map;
        public bool generateGridOnAwake;

        [ReadOnly] public TutorialHand tutorial;
        [ReadOnly] public Vector3 offset;
        public Transform parentRotationRootTf;
        public void Setup(LevelAsset levelData)
        {
            this.levelData = levelData;
            FillMap();
        }

        void Start()
        {
            CheckMoveAll();

            if (LevelController.instance.gridController == this && stageData.tutorialType != LevelAsset.TutorialType.None)
            {
                tutorial = Instantiate(m_PrefabManager.GetTutorialHand(), Percas.UI.UIGameManager.SafeArea, true);
                tutorial.transform.localScale = Vector3.one;
                tutorial.Setup(this, stageData.tutorialType != LevelAsset.TutorialType.Begin);
            }

            shuffleParticle.transform.position = GetCenterPos();
        }

        private void OnDrawGizmos()
        {
            Vector3 center = transform.position + Vector3.forward * 2.2f;
            Vector3 direction = Vector3.up;
            Vector2 size = new Vector2(4, 4) * 1.1f;
            if (mapData != null)
            {
                size = new Vector2((mapData.size.x + 2) * m_Grid.cellSize.x, (mapData.size.y + 1) * m_Grid.cellSize.z) * ratio;
                center = transform.position + Vector3.forward * ratio * ((mapData.size.y + 1) * m_Grid.cellSize.z) / 2;
            }
            Kit.GizmosExtend.DrawSquare(center, direction, size, 0, Color.blue);
        }

        [Button]
        public void GenerateGrid()
        {
            var first = m_Grid.CellToLocal(new Vector3Int(0, 0, 0));
            var last = m_Grid.CellToLocal(new Vector3Int(size.x - 1, size.y - 1, 0));
            offset = new Vector3((last.x - first.x) / 2, 0, -1 * m_Grid.cellSize.z / 2);// ((last.z - first.z) + m_Grid.cellSize.z / 2));

            map.size = size;

            m_ParentGrid.ClearContent();
            for (int i = 0; i < size.x * size.y; i++)
            {
                var x = i % size.x;
                var y = i / size.x;

                var element = Static.InstantiateUtility(m_PrefabManager.GetGridElementPrefab(), m_ParentGrid);
                element.transform.localPosition = m_Grid.CellToLocal(new Vector3Int(x, y, 0)) - offset;
                element.indexGrid = new Vector2Int(x, y);
                element.Setup(StageData.CellType.Wall, this);
                map[x, y] = element;

#if UNITY_EDITOR
                var gridCreate = element.GetComponent<ElementCreate>();
                if (gridCreate != null)
                {
                    gridCreate.type = StageData.CellType.Wall;
                }
#endif
            }
        }

        public List<CoilElement> GetCoilListInStage()
        {
            List<CoilElement> coilList = new List<CoilElement>();

            for (int i = 0; i < size.x * size.y; i++)
            {
                var x = i % size.x;
                var y = i / size.x;

                if (map[x, y]?.coilElement != null)
                {
                    coilList.Add(map[x, y].coilElement);
                }
            }

            return coilList;
        }

        public Matrix<bool> matrix;
        public bool[,] GetMap()
        {
            bool[,] map = new bool[size.x, size.y];

            matrix = new Matrix<bool>();
            matrix.cellSize = new Vector2Int(20, 20);
            matrix.size = size;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    switch (this.map[x, y].cellType)
                    {
                        case StageData.CellType.Empty:
                            map[x, y] = true;
                            break;

                        case StageData.CellType.Coil:
                        case StageData.CellType.CoilLocked:
                        case StageData.CellType.CoilPair:
                            map[x, y] = this.map[x, y].coilElement?.coilStatus == CoilStatus.Hide;
                            break;

                        case StageData.CellType.ButtonStack:
                            map[x, y] = this.map[x, y].GetComponentInChildren<ButtonsElement>()?.gameObject.activeSelf != true;
                            break;

                        case StageData.CellType.PinControl:
                            map[x, y] = this.map[x, y].GetComponentInChildren<PinControlElement>()?.gameObject.activeSelf != true;
                            break;

                        case StageData.CellType.PinWall:
                            map[x, y] = this.map[x, y].GetComponentInChildren<PinWallElement>()?.gameObject.activeSelf != true;
                            break;

                        case StageData.CellType.Wall:
                        case StageData.CellType.Stack:
                            map[x, y] = false;
                            break;
                        case StageData.CellType.Key:
                            map[x, y] = this.map[x, y].GetComponentInChildren<KeyElement>()?.gameObject.activeSelf != true;
                            break;
                        case StageData.CellType.Lock:
                            map[x, y] = this.map[x, y].GetComponentInChildren<LockElement>()?.gameObject.activeSelf != true;
                            break;
                        default:
                            map[x, y] = true;
                            break;
                    }

                    matrix[x, y] = map[x, y];
                }
            }

            return map;
        }

        public GridElement GetGridElementTarget(Vector2Int currentIndex, StageData.Direction direction)
        {
            switch (direction)
            {
                case StageData.Direction.Up:
                    return map[currentIndex.x, currentIndex.y + 1];

                case StageData.Direction.Down:
                    return map[currentIndex.x, currentIndex.y - 1];


                case StageData.Direction.Left:
                    return map[currentIndex.x - 1, currentIndex.y];


                case StageData.Direction.Right:
                    return map[currentIndex.x + 1, currentIndex.y];
            }

            return null;
        }

        public Stack<Vector3> CreatePosMove(Stack<Vector2Int> gridPosMove)
        {
            var posMove = new Stack<Vector3>();

            foreach (var pos in gridPosMove)
            {
                //Debug.Log(pos);
                posMove.Push(m_Grid.CellToLocal(new Vector3Int(pos.x, 0, pos.y)) - offset);
            }

            return posMove;
        }


        [HorizontalLine(2, EColor.Blue)]
        public LevelAsset levelData;
        [Dropdown("dropdownValue")]
        public int stageIndex;
        int[] dropdownValue = new int[3] { 1, 2, 3 };
        Matrix<StageData.CellData> mapData;

        [ReadOnly] public List<CoilElement> coilElements = new List<CoilElement>();
        [ReadOnly] public List<TubeElement> tubeElements = new List<TubeElement>();
        [ReadOnly] public List<ButtonsElement> buttonsElements = new List<ButtonsElement>();
        [ReadOnly] public StageData stageData;
        [ReadOnly] public List<KeyElement> keyElements = new();
        [ReadOnly] public List<LockElement> lockElements = new();
        [ReadOnly] public List<CoilElement> coilTutorial = new List<CoilElement>();

        public void CheckMoveAll()
        {
            GetCoilListInStage().ForEach(x => x.CheckMove());
            buttonsElements.ForEach(x => x.CheckMove());
        }

        public void CoilElementCollect(CoilElement coilElement)
        {
            Debug.Log("!!CoilElementCollect");
            foreach (var buttons in buttonsElements)
            {
                buttons.CollectButtons();
            }
        }

        float ratio = 1;
        //float maxRatio = 1.128247f;
        [ShowNativeProperty] float maxRatio => GameLogic.CoilMaxSizeRatio;

        [Button]
        public void FillMap()
        {
            stageData = levelData.Stage;

            //gen grid
            mapData = new Matrix<StageData.CellData>();
            mapData.size = new Vector2Int(stageData.Width, stageData.Height);

            //size = new Vector2Int(mapData.size.x, size.y);
            if (Application.isPlaying)
            {
                size = mapData.size + new Vector2Int(2, 1);
            }
            else
            {
                size = mapData.size;
            }
            GenerateGrid();

            //set mapdata
            for (int i = 0; i < stageData.Data.Length; i++)
            {
                var x = i % stageData.Width;
                var y = i / stageData.Width;
                mapData[x, y] = stageData.Data[i];
            }

            //spawn map element
            int startRow = map.size.y - mapData.size.y; // Top row
            int startCol = (map.size.x - mapData.size.x) / 2; // Center horizontally

            coilElements.Clear();
            tubeElements.Clear();
            for (int x = 0; x < mapData.size.x; x++)
            {
                for (int y = 0; y < mapData.size.y; y++)
                {
                    var data = mapData[x, y];
                    var gridMap = map[startCol + x, startRow + y];

#if UNITY_EDITOR
                    var gridCreate = gridMap.GetComponent<ElementCreate>();
                    if (gridCreate != null)
                    {
                        gridCreate.type = data.Type;
                        gridCreate.value = data.Value;
                        gridCreate.direction = data.Direction;
                    }
#endif

                    gridMap.holder.ClearContent();
                    gridMap.Setup(data.Type, this);
                    switch (data.Type)
                    {
                        //case StageData.CellType.CoilPair:
                        case StageData.CellType.Coil:
                            var coilElement = Static.InstantiateUtility(m_PrefabManager.GetCoilPrefab(), gridMap.holder);
                            gridMap.coilElement = coilElement;
                            coilElement.gridElement = gridMap;

                            if (!string.IsNullOrEmpty(data.String)) Debug.LogError($"{gameObject.name} - {data.String}");
                            if (data.Value >= levelData.PixelData.Colors.Length)
                            {
                                Debug.LogFormat("{2}:{0} - {1}", levelData.PixelData.Colors.Count(), data.Value, gameObject.name);

                            }
                            else
                            {
                                coilElement.ChangeColor(levelData.PixelData.Colors[data.Value]);
                            }
                            coilElement.cellData = data;

                            coilElements.Add(coilElement);
                            break;

                        case StageData.CellType.CoilLocked:
                            var coilElementLock = Static.InstantiateUtility(m_PrefabManager.GetCoilPrefab(), gridMap.holder);
                            gridMap.coilElement = coilElementLock;
                            coilElementLock.gridElement = gridMap;

                            coilElementLock.ChangeColor(levelData.PixelData.Colors[data.Value]);
                            coilElementLock.ChangeStatus(CoilStatus.Mystery);
                            coilElementLock.cellData = data;

                            coilElements.Add(coilElementLock);
                            break;

                        case StageData.CellType.CoilPair:
                            var coilElementPair = Static.InstantiateUtility(m_PrefabManager.GetCoilPrefab(), gridMap.holder);
                            gridMap.coilElement = coilElementPair;
                            coilElementPair.gridElement = gridMap;

                            coilElementPair.ChangeColor(levelData.PixelData.Colors[data.Value]);
                            coilElementPair.cellData = data;
                            coilElementPair.isCoilPair = true;

                            var connectCoil = GetGridElementTarget(gridMap.indexGrid, data.Direction);
                            if (connectCoil?.coilElement != null)
                            {
                                coilElementPair.coilElementConnect = connectCoil.coilElement;
                                connectCoil.coilElement.coilElementConnect = coilElementPair;

                                //coilElementPair.SetFakeConnect(true, data.Direction);
                                //connectCoil.coilElement.SetFakeConnect(true, connectCoil.coilElement.cellData.Direction);

                                var ribbon = Static.InstantiateUtility(m_PrefabManager.GetRibbonEffect(), gridMap.holder);
                                ribbon.transform.position = (coilElementPair.transform.position + connectCoil.coilElement.transform.position) / 2f;
                                if (data.Direction == StageData.Direction.Up || data.Direction == StageData.Direction.Down) ribbon.transform.SetLocalEulerAngles(y: 90);
                                coilElementPair.ribbonEffect = ribbon;
                                connectCoil.coilElement.ribbonEffect = ribbon;
                                ribbon.isShow = true;
                            }

                            coilElements.Add(coilElementPair);
                            if (stageData.tutorialType == LevelAsset.TutorialType.CoilPair)
                            {
                                coilTutorial.Add(coilElementPair);
                            }
                            break;

                        case StageData.CellType.Stack:
                            var tubeElement = Static.InstantiateUtility(m_PrefabManager.GetTubePrefab(), gridMap.holder);
                            //gridMap.tubeElement = tubeElement;

                            tubeElement.Setup(data, gridMap, map);

                            tubeElements.Add(tubeElement);
                            break;

                        case StageData.CellType.ButtonStack:
                            var buttons = Static.InstantiateUtility(m_PrefabManager.GetButtonsElementPrefab(), gridMap.holder);
                            buttons.Setup(data, gridMap, map);
                            buttonsElements.Add(buttons);
                            break;

                        case StageData.CellType.PinControl:
                            var pinControl = Static.InstantiateUtility(m_PrefabManager.GetPinControlPrefab(), gridMap.holder);
                            pinControl.Setup(data, gridMap, map);
                            break;

                        case StageData.CellType.PinWall:
                            var pinWall = Static.InstantiateUtility(m_PrefabManager.GetPinWallPrefab(), gridMap.holder);
                            pinWall.Setup(data, gridMap, map);
                            break;
                        case StageData.CellType.Key:
                            var keyElement = Static.InstantiateUtility(m_PrefabManager.GetKeyElement(), gridMap.holder);
                            keyElement.Setup(data, gridMap, map);
                            keyElements.Add(keyElement);
                            break;
                        case StageData.CellType.Lock:
                            var lockElement = Static.InstantiateUtility(m_PrefabManager.GetLockElement(), gridMap.holder);
                            lockElement.Setup(data, gridMap, map);
                            lockElements.Add(lockElement);
                            break;
                    }
                }
            }

            //Setup lock to key
            foreach (var keyElement in keyElements)
            {
                foreach (var lockElement in lockElements.Where(lockElement => lockElement.cellData.Value == keyElement.cellData.Value))
                {
                    keyElement.lockElement = lockElement;
                }
            }

            //setup wall
            for (int i = 0; i < size.x * size.y; i++)
            {
                var x = i % size.x;
                var y = i / size.x;
                map[x, y].SetupWall();
            }

            //setup map element
            var colorRandomList = new List<int>();
            if (!levelData.DisableRandom)
            {
                colorRandomList = stageData.RandomCoils.Split(",").OrderBy(x => new System.Random().Next()).Select(x => int.Parse(x)).ToList();
                foreach (var coilElement in coilElements)
                {
                    coilElement.ChangeColor(levelData.PixelData.Colors[colorRandomList[0]]);
                    colorRandomList.RemoveAt(0);
                }

                foreach (var tubeElement in tubeElements)
                {
                    tubeElement.coilValues = colorRandomList.GetRange(0, tubeElement.countCoint).Select(x => levelData.PixelData.Colors[x]).ToList();
                    colorRandomList.RemoveRange(0, tubeElement.countCoint);
                }
            }
            else
            {
                foreach (var tubeElement in tubeElements)
                {
                    //set value on Editor
                    tubeElement.coilValues = tubeElement.cellData.String.Split(",").Select(x => levelData.PixelData.Colors[int.Parse(x)]).ToList();

                    //set value data
                    tubeElement.coilElementDatas = tubeElement.cellData.String.Split(",").Select(x => int.Parse(x)).ToList();
                }
            }

            var size1 = new Vector2((mapData.size.x + 1) * m_Grid.cellSize.x, (mapData.size.y + 1) * m_Grid.cellSize.z);
            var zone = CameraSizeHandler.instance.GetZoneView().Item2;

            var zone1 = new Vector2(zone.size.x, zone.size.y - (CameraSizeHandler.bottomOffset + SlotController.sizeControler) + CameraSizeHandler.topOffset);

            //Debug.Log(size1);
            //Debug.Log(zone1);
            float widthRatio = zone1.x / size1.x;
            float heightRatio = zone1.y / size1.y;
            ratio = Mathf.Min(widthRatio, heightRatio, maxRatio);

            if (zone1.y > (size1.y * ratio))
            {
                transform.SetLocalPosition(z: (zone1.y - size1.y * ratio) / 2f);
            }

            m_WallRight.transform.SetLossyScale(new Vector3(12f, 6.5f - ((map.size.x - 0.5f) * m_Grid.cellSize.x * ratio) / 2f, ratio));
            m_WallLeft.transform.SetLossyScale(new Vector3(12f, 6.5f - ((map.size.x - 0.5f) * m_Grid.cellSize.x * ratio) / 2f, ratio));

            m_WallBottom.transform.localPosition = new Vector3(0, ratio, 0.3f);
            m_WallRight.transform.localPosition = new Vector3(6.5f, 0, map.size.y * m_Grid.cellSize.z * ratio - 6.5f);
            m_WallLeft.transform.localPosition = new Vector3(-6.5f, 0, map.size.y * m_Grid.cellSize.z * ratio - 6.5f);
            //
            transform.localScale = Vector3.one * ratio;
            SetUpWallLeftRight();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
        private void SetUpWallLeftRight()
        {
            //Wall left Right
            var pos = m_Grid.CellToLocal(new Vector3Int(0, size.y - 1, 0)) - offset;
            var endWallCheck = m_ParentGrid.InverseTransformPoint(transform.position + Vector3.left * 6.5f);
            var posG = pos;
            posG.x = (endWallCheck.x + pos.x) / 2f;
            //
            var elementL = Static.InstantiateUtility(m_PrefabManager.GetGridElementPrefab(), m_ParentGrid);
            elementL.transform.localPosition = posG;
            elementL.indexGrid = new Vector2Int(-1, size.y - 1);
            elementL.Setup(StageData.CellType.Wall, this);
            elementL.transform.localScale = new Vector3(Mathf.Abs(endWallCheck.x - pos.x), 1, 1);
            elementL.SetActiveWallBeard(false);

            pos = m_Grid.CellToLocal(new Vector3Int(size.x - 1, size.y - 1, 0)) - offset;
            endWallCheck = m_ParentGrid.InverseTransformPoint(transform.position + Vector3.right * 6.5f);
            posG = pos;
            posG.x = (endWallCheck.x + pos.x) / 2f;
            var elementR = Static.InstantiateUtility(m_PrefabManager.GetGridElementPrefab(), m_ParentGrid);
            elementR.transform.localPosition = posG;
            elementR.indexGrid = new Vector2Int(size.x, size.y - 1);
            elementR.Setup(StageData.CellType.Wall, this);
            elementR.transform.localScale = new Vector3(Mathf.Abs(endWallCheck.x - pos.x), 1, 1);
            elementR.SetActiveWallBeard(false);
        }

        [Button]
        async void SetLevelDifficulty(int difficulty)
        {
            var coilChecked = new HashSet<CoilElement>();
            var random = new System.Random();

            var coilElementCheck = coilElements.OrderByDescending(x => x.gridElement.indexGrid.y).ToList();
            var coilElementRandom = coilElements.OrderBy(x => random.Next()).ToList();

            foreach (var coilElement in coilElementCheck)
            {
                if (coilChecked.Contains(coilElement)) continue;

                coilChecked.Add(coilElement);
                coilElement.transform.DOLocalMoveY(2, 0.2f);
                coilElement.transform.DOScale(1.25f, 0.2f);
                var index = coilElement.gridElement.indexGrid;
                if ((index.y - difficulty) > 0)
                {
                    //
                    var elmentSwap = coilElementCheck.Find(x => (!coilChecked.Contains(x) && x.coilColor == coilElement.coilColor));
                    if (elmentSwap != null)
                    {
                        var indexSwap = elmentSwap.gridElement.indexGrid;
                        if ((index.y - indexSwap.y) != difficulty)
                        {
                            var swapElement = coilElementRandom.Find(x => x.gridElement.indexGrid.y >= (index.y - difficulty) && x.gridElement.indexGrid.y <= index.y);

                            LevelController.instance.SwapCoil(elmentSwap, swapElement);
                            await UniTask.Delay(800);

                            coilChecked.Add(swapElement);
                        }
                        else
                        {
                            coilChecked.Add(elmentSwap);
                        }
                    }

                    //
                    var elmentSwap1 = coilElementCheck.Find(x => (!coilChecked.Contains(x) && x.coilColor == coilElement.coilColor));
                    if (elmentSwap1 != null)
                    {
                        var indexSwap = elmentSwap1.gridElement.indexGrid;
                        if ((index.y - indexSwap.y) > difficulty)
                        {
                            var swapElement = coilElementRandom.Find(x => x.gridElement.indexGrid.y >= (index.y - difficulty) && x.gridElement.indexGrid.y <= index.y);

                            LevelController.instance.SwapCoil(elmentSwap1, swapElement);
                            await UniTask.Delay(800);
                            coilChecked.Add(swapElement);
                        }
                        else
                        {
                            coilChecked.Add(elmentSwap1);

                        }
                    }
                }

                await UniTask.Delay(200);
                coilElement.transform.DOLocalMoveY(0, 0.2f);
                coilElement.transform.DOScale(1, 0.2f);
                await UniTask.Delay(200);

            }
            Debug.Log("Done");
        }

        void SwapCoilColor(CoilElement coil1, CoilElement coil2)
        {
            if (coil1 == null || coil2 == null) return;

            var color1 = coil1.coilColor;
            coil1.ChangeColor(coil2.coilColor);
            coil2.ChangeColor(color1);

        }

        public Vector3 GetCenterPos()
        {
            if (mapData == null) return Vector3.zero;
            return transform.position + new Vector3(0, 4f, (mapData.size.y + 1) * m_Grid.cellSize.z / 2f);
        }
    }
}

