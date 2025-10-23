using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Percas;
using Percas.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tuon
{
    [DefaultExecutionOrder(100)]
    public class LevelController : Kit.Common.Singleton<LevelController>
    {
        [SerializeField] LevelsConfig m_LevelsConfig;
        [SerializeField] private Transform m_EventSystem;

        [HorizontalLine(2, EColor.Blue)]
        [SerializeField] Camera m_MainCamera;

        [SerializeField] SlotController m_SlotController;
        [SerializeField] GridController m_GridController;
        //[SerializeField] RopeController m_RopeController;
        [SerializeField] PictureController m_PictureController;
        //[SerializeField] Animation m_WinAnimation;

        [SerializeField] Material m_MaterialBackground1;
        [SerializeField] Material m_MaterialBackground2;
        [SerializeField] Material m_MaterialBackground3;

        [SerializeField] SpriteRenderer m_Background;
        [SerializeField] Sprite m_PatternHardLevel;
        [SerializeField] List<Sprite> m_PatternLevels;

        [HorizontalLine(2, EColor.Blue)]
        public bool autoLoadLevelIndex;
        public LevelAsset levelData;
        [ReadOnly] public int levelIndex;
        [ReadOnly] public bool endGame;
        public GridController gridController => m_GridController;
        [ReadOnly] public bool isUsingBooster;
        [ReadOnly] public bool isCollecting;
        [SerializedDictionary] public List<StageData.CellType> cellTypeList;
        public Camera mainCamera => m_MainCamera;
        public PictureController pictureController => m_PictureController;
        public bool isPlaying => (m_SlotController.coilElementAdds?.Count ?? 0) > 0;
        protected override void Awake()
        {
            Application.targetFrameRate = 60;

            SetupLevel();
        }

#if UNITY_EDITOR
        // [Cheat]
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                FinishLevel();
            }

            if (Input.GetKeyDown(KeyCode.K)) Debug.Break();
        }
#endif

        public void ChangeColorBackground(TupleSerialize<Color, Color, Color> colors, bool isHard = false)
        {
            if (m_Background != null)
            {
                if (isHard)
                {
                    m_Background.sprite = m_PatternHardLevel;
                }
                else
                {
                    m_Background.sprite = m_PatternLevels[(GameLogic.CurrentLevel / m_PatternLevels.Count) % m_PatternLevels.Count];
                }
            }

            //color 1
            if (m_MaterialBackground1 != null)
            {
                if (isHard)
                    m_MaterialBackground1.color = Static.defaultColor1LevelHard;
                else if (colors.Value1 != Color.clear)
                    m_MaterialBackground1.color = colors.Value1;
                else
                    m_MaterialBackground1.color = Static.defaultColor1;
            }

            //color 2
            if (m_MaterialBackground2 != null)
            {
                if (isHard)
                    m_MaterialBackground2.color = Static.defaultColor2LevelHard;
                else if (colors.Value2 != Color.clear)
                    m_MaterialBackground2.color = colors.Value2;
                else
                    m_MaterialBackground2.color = Static.defaultColor2;
            }

            //color 3
            if (m_MaterialBackground3 != null)
            {
                if (isHard)
                    m_MaterialBackground3.color = Static.defaultColor3LevelHard;
                else if (colors.Value3 != Color.clear)
                    m_MaterialBackground3.color = colors.Value3;
                else
                    m_MaterialBackground3.color = Static.defaultColor3;
            }
        }

        protected override void OnDestroy()
        {
#if UNITY_EDITOR
            m_MaterialBackground1.color = Static.defaultColor1;
            m_MaterialBackground2.color = Static.defaultColor2;
            m_MaterialBackground3.color = Static.defaultColor3;
#endif
        }

        [Button]
        public void SetupLevel()
        {
            //Debug.LogError($"SetupLevel {GameLogic.LevelAttempts}");
            ActionEvent.OnLevelStart?.Invoke();

            Debug.LogError($"LevelUnlockHome = {GameLogic.LevelUnlockHome}");

            //setup level data
            if (autoLoadLevelIndex && GameLogic.IsClassicMode)
            {
                // get default level data
                levelIndex = GameLogic.CurrentLevel; //value start by 1, not 0
                levelData = m_LevelsConfig.GetLevelData(levelIndex);

                // get remote group level data
                try
                {
                    Debug.LogError($"RemoteLevelData = {JsonConvert.SerializeObject(GameLogic.RemoteLevelData)}");
                    if (GameLogic.RemoteLevelData != null)
                    {
                        if (GameLogic.RemoteLevelData.TryGetValue(string.Format("level_{0}", levelIndex), out string levelDataString))
                        {
                            if (levelDataString != null)
                            {
                                //convert string to scriptableObject
                                var level = ScriptableObject.CreateInstance<LevelAsset>();
                                JsonUtility.FromJsonOverwrite(Static.DecompressString(levelDataString), level);
                                levelData = level;
                            }
                        }
                    }
                }
                catch (Exception) { }

                // get remote level data
                try
                {
                    var remoteLevelData = PercasSDK.FirebaseManager.RemoteConfig.GetString($"remote_level_{levelIndex}");
                    Debug.LogError($"remote_level_{levelIndex} = {remoteLevelData}");
                    if (!string.IsNullOrEmpty(remoteLevelData))
                    {
                        var level = ScriptableObject.CreateInstance<LevelAsset>();
                        JsonUtility.FromJsonOverwrite(Static.DecompressString(remoteLevelData), level);
                        levelData = level;
                    }
                }
                catch (Exception) { }

                // hack level data
                if (GameConfig.Instance.DebugOn && !string.IsNullOrEmpty(GameLogic.CheatLevelData))
                {
                    var level = ScriptableObject.CreateInstance<LevelAsset>();
                    JsonUtility.FromJsonOverwrite(Static.DecompressString(GameLogic.CheatLevelData), level);
                    levelData = level;
                    // reset cheat data
                    GameLogic.CheatLevelData = null;
                }
            }
            else if (GameLogic.IsHiddenPictureMode)
            {
                levelData = GlobalSetting.HiddenPictureLevelData;
            }
            else
            {
                // check event system

                if (EventSystem.current == null)
                {
                    m_EventSystem.gameObject.SetActive(true);
                }

            }

            //custom inter frequency
            Debug.LogError($"InterGapTime: {GameLogic.InterGapTime}");
            if (!string.IsNullOrEmpty(GameLogic.InterGapTime))
            {
                JObject obj = JObject.Parse(GameLogic.InterGapTime);
                JArray data = (JArray)obj["data"];

                var closestItem = data
                    .Where(item => (int)item["key"] <= levelIndex + 1)
                    .OrderByDescending(item => (int)item["key"])
                    .FirstOrDefault();

                var result = closestItem?["value"]?.Value<int>();
                if (result.HasValue)
                {
                    Debug.LogError($"TimeShowInter Old - New: {GameConfig.ConfigData.TimeShowInter} - {result.Value}");
                    GameConfig.ConfigData.TimeShowInter = result.Value;
                }
            }

            //setup hardlevel
            if (GameLogic.IsClassicMode)
            {
                if (levelData.IsHard)
                {
                    GameLogic.UpdateTangleJamLevelLabel(true);
                    GameLogic.UpdateButtonUI(true);
                }
                else
                {
                    GameLogic.UpdateTangleJamLevelLabel(false);
                    GameLogic.UpdateButtonUI(false);
                }
            }
            else if (GameLogic.IsHiddenPictureMode)
            {
                GameLogic.UpdateTangleJamLevelLabel(false);
                GameLogic.UpdateButtonUI(false);
            }
            //UIGameManager.OnUpdateIconHardLevelPos?.Invoke();

            //setup picture
            if (levelData.PictureAsset != null) levelData.ConvertPixelData();

            m_PictureController.Setup(levelData);

            //setup grid
            m_GridController.Setup(levelData);

            if (Application.isPlaying)
            {
                //setup slot
                m_SlotController.ResetSlot();

                //setup background
                ChangeColorBackground(levelData.BackgroundColor, levelData.IsHard);
            }
            this.cellTypeList = new List<StageData.CellType>();

            var cellTypeList = new List<StageData.CellType>();
            foreach (var cell in levelData.Stage.Data)
            {
                if (!cellTypeList.Contains(cell.Type))
                {
                    cellTypeList.Add(cell.Type);
                }
            }
            this.cellTypeList.AddRange(cellTypeList);
        }

        public void ResetLevel()
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Reset Level");
            GlobalSetting.OnHomeToGame?.Invoke(null);
        }

        [HorizontalLine(2, EColor.Blue)]
        [ReadOnly] public List<CoilElement> coilFillList = new List<CoilElement>();

        //public UnityEngine.UI.Image demo;
        public void CheckStage()
        {
            if (coilFillList.Count > 0) return;
            Debug.Log("CheckStage");

            var coilList = gridController.GetCoilListInStage();
            var coilListInSlot = m_SlotController.GetListCoilInBar(false);
            var coilListInSlotExtra = m_SlotController.GetListCoilActiveInSlotExtra();

            if (!coilList.Any(x => x.coilStatus != CoilStatus.Hide))
            {
                if (coilListInSlotExtra.Count > 0)
                {
                    if (!showOnNoCoinOnBoard && !PlayerDataManager.PlayerData.IntroNoCoinOnBoard)
                    {
                        showOnNoCoinOnBoard = true;
                        PlayerDataManager.PlayerData.SaveIntroNoCoinOnBoard();
                        Debug.Log("OnNoCoinOnBoard!");
                        ActionEvent.OnNoCoinOnBoard?.Invoke();
                    }
                }
                else if (!coilListInSlot.Any(x => x.coilStatus != CoilStatus.Hide))
                {
                    FinishLevel();
                }
            }
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void FinishLevel()
        {
            if (!endGame)
            {
                //win
                Debug.Log("!Win game");

                endGame = true;
                Static.curentLevel += 1;

                m_PictureController.CompleteLevel();

                Percas.ServiceLocator.PopupScene.ShowPopup(PopupName.LevelWin, new PopupLevelWinArgs(Percas.Data.TutorialManager.TutorialInNextLevel));

                Texture2D tex = m_PictureController.GetTextureLevel() as Texture2D;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                GameLogic.UpdateLevelImage(sprite);
            }
        }

        bool showOnNoCoinOnBoard = false;
        public void CheckLost()
        {
            var coilList = gridController.GetCoilListInStage();
            var coilListInSlot = m_SlotController.GetListCoilInBar(false);
            var coilListInSlotExtra = m_SlotController.GetListCoilActiveInSlotExtra();

            if (m_SlotController.CheckFullSlot())
            {
                Debug.Log("lost");
                if (!endGame)
                {
                    endGame = true;

                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        Percas.ServiceLocator.PopupScene.ShowPopup(PopupName.LevelRevive);
                    });
                }
            }

            if (!coilList.Any(x => x.coilStatus != CoilStatus.Hide) && coilListInSlotExtra.Count > 0)
            {
                if (!showOnNoCoinOnBoard && !PlayerDataManager.PlayerData.IntroNoCoinOnBoard)
                {
                    showOnNoCoinOnBoard = true;
                    PlayerDataManager.PlayerData.SaveIntroNoCoinOnBoard();
                    Debug.Log("OnNoCoinOnBoard!");
                    ActionEvent.OnNoCoinOnBoard?.Invoke();
                }
            }
        }

        private void Start()
        {
            //start level
            endGame = false;
        }

#if UNITY_EDITOR
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void ButtonUndoBoosterTap()
        {
            var check = CheckUndoBoosterToSlotExtra();
            Debug.LogFormat("UndoBooster: {0}", check);
            if (check) UndoBoosterToSlotExtra();
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void ButtonAddSlotBoosterTap()
        {
            var check = CheckAddSlotBooster();
            Debug.LogFormat("AddSlotBooster: {0}", check);

            if (check) AddSlotBooster();
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void ButtonCollectBoosterTap()
        {
            var check = CheckRollCollectBooster();
            Debug.LogFormat("CollectBooster: {0}", check);

            if (check && !isCollecting) RollCollectBooster().Forget();
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void ButtonShuffleBoosterTap()
        {
            var check = CheckShuffleBooster();
            Debug.LogFormat("ShuffleBooster: {0}", check);

            if (check) ShuffleBooster().Forget();
        }
#endif

        (CoilElement, CoilElement) lastCoilElementAdd;
        public bool CheckUndoBooster()
        {
            lastCoilElementAdd = m_SlotController.GetLastCoilElementAdd();
            if (lastCoilElementAdd.Item1 == null || lastCoilElementAdd.Item2 == null)
            {
                //ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_CANNOT_UNDO);
                return false;
            }

            return true;
        }

        public bool UndoBooster()
        {
            //Debug.Log($"UndoBooster: {lastCoilElementAdd}");

            if (lastCoilElementAdd.Item1 == null || lastCoilElementAdd.Item2 == null) return false;
            m_SlotController.coilElementAdds.Remove(lastCoilElementAdd);

            if (lastCoilElementAdd.Item2.coilStatus == CoilStatus.Enable)
            {
                var gridElement = lastCoilElementAdd.Item2.GetComponentInParent<GridElement>();
                if (gridElement.tubeElement != null)
                {
                    gridElement.tubeElement.coilValues.Add(lastCoilElementAdd.Item2.coilColor);
                    gridElement.tubeElement.UpdateCountText();
                }
            }

            var targetCoilElemet = lastCoilElementAdd;
            targetCoilElemet.Item2.coilStatus = CoilStatus.Enable;
            targetCoilElemet.Item1.MoveOtherSpool(targetCoilElemet.Item2).ContinueWith(() =>
            {
                targetCoilElemet.Item2.ChangeStatus(CoilStatus.Enable);
                gridController.CheckMoveAll();
                m_SlotController.ReOrderSlot();
            }).Forget();

            Percas.ActionEvent.OnPlaySFXBoosterUndo?.Invoke();
            return true;
        }

        #region UndoBooster

        private (CoilElement, CoilElement) _lastCoilElementAddSlotExtra;
        public bool CheckUndoBoosterToSlotExtra()
        {
            var check = CheckUndoBooster();
            if (!check)
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_CANNOT_UNDO);
                return false;
            }
            var otherCoilList = m_SlotController.GetListCoilHideInSlotExtra();
            Debug.Log(otherCoilList.Count);
            if (otherCoilList.Count == 0)
            {
                _lastCoilElementAddSlotExtra = new ValueTuple<CoilElement, CoilElement>(lastCoilElementAdd.Item1, null);

                ActionEvent.OnShowToast?.Invoke("Cannot add more slot!");
                return true;
            }
            _lastCoilElementAddSlotExtra = new ValueTuple<CoilElement, CoilElement>(lastCoilElementAdd.Item1, otherCoilList.First());
            return true;

        }
        public bool UndoBoosterToSlotExtra()
        {
            if (_lastCoilElementAddSlotExtra.Item1 == null || _lastCoilElementAddSlotExtra.Item2 == null)
            {
                return UndoBooster();
            }
            AddCoilToSlotExtra(_lastCoilElementAddSlotExtra);
            if (!m_SlotController.parentSlotExtra.gameObject.activeSelf) m_SlotController.parentSlotExtra.gameObject.SetActive(true);
            ActionEvent.OnPlaySFXBoosterUndo?.Invoke();
            return true;
        }
        #endregion

        List<(CoilElement, CoilElement)> coilElementAddSlotList;
        public bool CheckAddSlotBooster()
        {
            coilElementAddSlotList = new List<(CoilElement, CoilElement)>();

            var coilElementsInBar = m_SlotController.GetListCoilInBar();
            var coilElementActive = coilElementsInBar.Where(x => x.coilStatus == CoilStatus.InBar).ToList();

            int count = 0;
            if (coilElementActive.Count > 0)
            {
                var otherCoilList = m_SlotController.GetListCoilHideInSlotExtra();
                Debug.Log(otherCoilList.Count);
                if (otherCoilList.Count == 0)
                {
                    Percas.ActionEvent.OnShowToast?.Invoke("Cannot add more slot!");
                    return false;
                }

                foreach (var coilElement in coilElementActive)
                {
                    if (count >= 3) break;
                    if (count >= otherCoilList.Count) break;

                    coilElementAddSlotList.Add(new(coilElement, otherCoilList[count]));
                    count += 1;
                }
            }
            else
            {
                Percas.ActionEvent.OnShowToast?.Invoke("No more coils to add slots to!");
                return false;
            }

            return true;
        }

        private void AddCoilToSlotExtra((CoilElement, CoilElement) coilElement)
        {
            if (coilElement.Item1 == null || coilElement.Item2 == null) return;
            m_SlotController.coilElementAdds.Remove(coilElement);
            coilElement.Item2.coilStatus = CoilStatus.Transfer;
            coilElement.Item1.MoveOtherSpool(coilElement.Item2).ContinueWith(() =>
            {
                coilElement.Item2.ChangeStatus(CoilStatus.Enable);
                m_SlotController.ReOrderSlot();
            }).Forget();
        }

        public bool AddSlotBooster()
        {
            Debug.Log($"AddSlotBooster: {coilElementAddSlotList?.Count}");
            if (coilElementAddSlotList == null || coilElementAddSlotList.Count == 0) return false;

            foreach (var element in coilElementAddSlotList)
            {
                AddCoilToSlotExtra(element);
            }

            if (!m_SlotController.parentSlotExtra.gameObject.activeSelf) m_SlotController.parentSlotExtra.gameObject.SetActive(true);
            Percas.ActionEvent.OnPlaySFXBoosterAddSlots?.Invoke();
            return true;
        }

        List<CoilElement> collectList;
        public bool CheckCollectBooster()
        {
            collectList = new List<CoilElement>();
            var matchList = m_SlotController.GetCoilElementListMatch();
            var coilElementsInStage = gridController.GetCoilListInStage();
            var tubeElementInStage = gridController.tubeElements;
            var coilListInSlotExtra = m_SlotController.GetListCoilActiveInSlotExtra();
            var coilListInTubeWillAdd = new List<(TubeElement, CoilElement)>();

            var coilElementActive = coilElementsInStage.Where(x => (x.coilStatus == CoilStatus.Enable || x.coilStatus == CoilStatus.Disable || x.coilStatus == CoilStatus.Mystery)).ToList();
            coilElementActive.AddRange(coilListInSlotExtra);
            Debug.Log(matchList.Count);

            //get list collect
            if (matchList.Count > 0)
            {
                collectList = coilElementActive.Where(x => x.coilColor == matchList[0].coilColor).ToList();
                if (collectList.Count + matchList.Count < 3)
                {
                    foreach (var tubeElement in tubeElementInStage)
                    {
                        foreach (var color in tubeElement.coilValues.ToList())
                        {
                            var coilElement = tubeElement.GetTempCoilElementInTube(matchList[0].coilColor, ref collectList);
                            coilListInTubeWillAdd.Add(new(tubeElement, coilElement));
                            if (collectList.Count + matchList.Count == 3) break;
                        }

                        if (collectList.Count + matchList.Count == 3) break;
                    }

                    if (collectList.Count + matchList.Count < 3) return false;
                }
                else
                {
                    collectList = collectList.GetRange(0, 3 - matchList.Count);
                }
            }
            else
            {
                var grouped = coilElementActive
                    .GroupBy(o => o.coilColor)
                    .OrderByDescending(g => g.Count())
                    .ToList();

                if (grouped.Count == 0) return false;

                var topGroup = grouped.First().ToList();
                if (topGroup.Count < 3)
                {
                    foreach (var tubeElement in tubeElementInStage)
                    {
                        foreach (var color in tubeElement.coilValues.ToList())
                        {
                            var coilElement = tubeElement.GetTempCoilElementInTube(topGroup[0].coilColor, ref topGroup);
                            coilListInTubeWillAdd.Add(new(tubeElement, coilElement));
                            if (topGroup.Count == 3) break;
                        }

                        if (topGroup.Count == 3) break;
                    }

                    if (topGroup.Count != 3) return false;

                    collectList = new List<CoilElement>(topGroup);
                }
                else
                {
                    collectList = topGroup.GetRange(0, 3);
                }
            }
            Debug.Log(collectList.Count);

            //check count slot empty
            var slotEmpty = m_SlotController.GetSlotEmpty();
            if (slotEmpty.Count < collectList.Count)
            {
                ActionEvent.OnShowToast?.Invoke("Not enough slot to clean!");
                foreach (var element in coilListInTubeWillAdd)
                {
                    if (element.Item2 == null) continue;

                    element.Item1.coilValues.Add(element.Item2.coilColor);
                    PoolingManager.instance.coilElementPool.Release(element.Item2);
                    //Debug.LogError("Release Coil in Stack");
                    //Debug.Break();
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CollectBooster()
        {
            if (collectList == null || collectList.Count == 0) return false;

            foreach (var coilElement in collectList)
            {
                coilElement.AddCoilToSlot(true);
            }

            Percas.ActionEvent.OnPlaySFXBoosterClear?.Invoke();
            return true;
        }

        public bool CheckRollCollectBooster()
        {
            if (isCollecting) return false;
            collectList = new List<CoilElement>();
            var matchList = m_SlotController.GetCoilElementListMatch();
            var coilElementsInStage = gridController.GetCoilListInStage();
            var tubeElementInStage = gridController.tubeElements;
            var coilListInSlotExtra = m_SlotController.GetListCoilActiveInSlotExtra();
            var coilListInTubeWillAdd = new List<(TubeElement, CoilElement)>();

            var coilElementActive = coilElementsInStage.Where(x => (x.coilStatus == CoilStatus.Enable || x.coilStatus == CoilStatus.Disable || x.coilStatus == CoilStatus.Mystery)).ToList();
            coilElementActive.AddRange(coilListInSlotExtra);
            Debug.Log(matchList.Count);

            //get list collect
            if (matchList.Count > 0)
            {
                collectList = coilElementActive.Where(x => x.coilColor == matchList[0].coilColor).ToList();
                if (collectList.Count + matchList.Count < 3)
                {
                    foreach (var tubeElement in tubeElementInStage)
                    {
                        foreach (var color in tubeElement.coilValues.ToList())
                        {
                            var coilElement = tubeElement.GetTempCoilElementInTube(matchList[0].coilColor, ref collectList);
                            coilListInTubeWillAdd.Add(new(tubeElement, coilElement));
                            if (collectList.Count + matchList.Count == 3) break;
                        }

                        if (collectList.Count + matchList.Count == 3) break;
                    }

                    if (collectList.Count + matchList.Count < 3) return false;
                }
                else
                {
                    collectList = collectList.GetRange(0, 3 - matchList.Count);
                }
                foreach (var coil in matchList)
                {
                    collectList.Add(coil);
                }
            }
            else
            {
                var grouped = coilElementActive
                    .GroupBy(o => o.coilColor)
                    .OrderByDescending(g => g.Count())
                    .ToList();

                if (grouped.Count == 0) return false;

                var topGroup = grouped.First().ToList();
                if (topGroup.Count < 3)
                {
                    foreach (var tubeElement in tubeElementInStage)
                    {
                        foreach (var color in tubeElement.coilValues.ToList())
                        {
                            var coilElement = tubeElement.GetTempCoilElementInTube(topGroup[0].coilColor, ref topGroup);
                            coilListInTubeWillAdd.Add(new(tubeElement, coilElement));
                            if (topGroup.Count == 3) break;
                        }

                        if (topGroup.Count == 3) break;
                    }

                    if (topGroup.Count != 3) return false;

                    collectList = new List<CoilElement>(topGroup);
                }
                else
                {
                    collectList = topGroup.GetRange(0, 3);
                }
            }
            Debug.Log(collectList.Count);

            //check count slot empty
            var slotEmpty = m_SlotController.GetSlotEmpty();
            if (slotEmpty.Count <= 0)
            {
                ActionEvent.OnShowToast?.Invoke("Not enough slot to clean!");
                foreach (var element in coilListInTubeWillAdd)
                {
                    if (element.Item2 == null) continue;

                    element.Item1.coilValues.Add(element.Item2.coilColor);
                    PoolingManager.instance.coilElementPool.Release(element.Item2);
                    //Debug.LogError("Release Coil in Stack");
                    //Debug.Break();
                }
                return false;
            }

            return true;
        }

        public async UniTask RollCollectBooster()
        {
            var matchList = m_SlotController.GetCoilElementListMatch();
            if (collectList == null || collectList.Count == 0) return;
            isCollecting = true;
            ScaleRollCollect(collectList[0].coilColor).Forget();
            foreach (var coilElement in collectList)
            {
                if (coilElement.coilStatus == CoilStatus.Mystery)
                {
                    coilElement.ActiveMysteryCoil();
                }
                coilElement.animation1.PlayAsync("Coil Enable", 0.1f).ContinueWith(() =>
                {
                    coilElement.SetFakeShadow(false);
                    coilElement.SetOutline(false);
                }).Forget();
                coilElement.transform.DOLocalMoveY(2f, 0.1f);
                if (!collectList.Contains(coilElement.coilElementConnect))
                {
                    if (coilElement.isCoilPair && coilElement.coilElementConnect != null)
                    {
                        coilElement.coilElementConnect.isCoilPair = false;
                        coilElement.isCoilPair = false;
                        if (coilElement.coilElementConnect.coilStatus != CoilStatus.Hide) coilElement.coilElementConnect.coilStatus = CoilStatus.Transfer;
                        coilElement.ribbonEffect.PlayAnimation().ContinueWith(() =>
                        {
                            coilElement.ribbonEffect.gameObject.SetActive(false);
                            if (coilElement.coilElementConnect.coilStatus != CoilStatus.Hide) coilElement.coilElementConnect.AddCoilToSlot(true);
                        }).Forget();
                    }
                }
                else
                {
                    coilElement.ribbonEffect.PlayAnimation().ContinueWith(() =>
                    {
                        coilElement.ribbonEffect.gameObject.SetActive(false);
                    }).Forget();
                }
                coilElement.FillToRollCollect(1f, gridController.rollCollect.transform).ContinueWith(() =>
                {
                    var grid = coilElement.gridElement;
                    var tube = grid?.tubeElement;
                    var tubeAmount = tube?.coilValues.Count ?? 0;
                    if (grid != null)
                    {
                        coilElement.gridElement.CheckCoilActive();
                        coilElement.gridElement.CheckTubeElenentTarget();
                        coilElement.gridElement.controller.CoilElementCollect(coilElement);
                    }
                    var slot = coilElement.GetComponentInParent<SlotElement>();
                    coilElement.SetupMergeCoil();
                    if (coilElement.coilStatus == CoilStatus.InBar)
                    {
                        slot.SpawnCoil();
                        SlotController.instance.ReOrderSlot();
                    }
                    if (tube != null && tubeAmount > 0)
                    {
                        coilElement.transform.DOLocalMoveY(0, 0.2f);

                    }
                    else
                    {
                        coilElement.gameObject.SetActive(false);
                        coilElement.ResetSlotStatus();
                        PoolingManager.instance.coilElementPool.Release(coilElement);
                    }
                }).Forget();
            }
            await UniTask.Delay(1400);
            await collectList[0].RollCollectToPicture(1f, gridController.rollCollect.transform);
            isCollecting = false;
            ActionEvent.OnPlaySFXBoosterClear?.Invoke();
        }

        private List<CoilElement> _listCoilShuffle = new();
        private List<CoilElement> _listCoilShuffleEnable = new();
        private List<CoilElement> _coilInSlot = new();
        private List<CoilElement> _coilInSlotSameColor = new();
        [ReadOnly] public Dictionary<Color, int> colorCountInSlot = new();

        private TubeElement _tubeElementChange;
        private TubeElement _tubeElementChange1;
        private Color _colorChange;
        private CoilElement _coilElementChange;
        private CoilElement _coilElementChange1;
        private int _indexColor;
        private int _indexColor1;

        public bool CheckShuffleBooster()
        {
            if (isUsingBooster) return false;
            _coilInSlot = m_SlotController.GetCoilElementListMatch();
            var coilElementsInStage = gridController.GetCoilListInStage();
            var tubeElementInStage = gridController.tubeElements;
            _coilInSlotSameColor.Clear();
            _listCoilShuffleEnable.Clear();
            _listCoilShuffle.Clear();
            _listCoilShuffle = coilElementsInStage.Where(x => x.coilStatus is CoilStatus.Enable or CoilStatus.Disable or CoilStatus.Mystery).OrderBy(x => new System.Random().Next()).ToList();
            if (_listCoilShuffle.Count < 2)
            {
                ActionEvent.OnShowToast?.Invoke("Not enough coil to shuffle!");
                return false;
            }
            if (_coilInSlot.Count <= 0) return true;
            _colorChange = _coilInSlot[0].coilColor;
            _coilInSlotSameColor = _listCoilShuffle.Where(x => x.coilColor == _colorChange).ToList();
            _listCoilShuffle.RemoveAll(x => x.coilColor == _colorChange);
            _listCoilShuffleEnable = _listCoilShuffle.Where(x => x.coilStatus == CoilStatus.Enable).ToList();
            _listCoilShuffle.RemoveAll(x => x.coilStatus == CoilStatus.Enable);
            if (_coilInSlot.Count + _coilInSlotSameColor.Count == 2)
            {
                foreach (var tube in tubeElementInStage)
                {
                    foreach (var color in tube.coilValues.ToList())
                    {
                        if (color != _colorChange) continue;
                        _tubeElementChange = tube;
                        _indexColor = tube.coilValues.IndexOf(color);
                        goto TubeFound;
                    }
                }

            TubeFound:
                if (_tubeElementChange == null) return false;
                _coilElementChange = _listCoilShuffleEnable[0];
                _listCoilShuffleEnable.RemoveAt(0);
            }
            else if (_coilInSlot.Count + _coilInSlotSameColor.Count == 1)
            {
                var countCoil = 0;
                foreach (var tube in tubeElementInStage)
                {
                    foreach (var color in tube.coilValues.ToList())
                    {
                        if (color != _colorChange) continue;
                        if (countCoil == 0)
                        {
                            _tubeElementChange = tube;
                            _indexColor = tube.coilValues.IndexOf(color);
                        }
                        else
                        {
                            _tubeElementChange1 = tube;
                            _indexColor1 = tube.coilValues.IndexOf(color);
                        }
                        countCoil += 1;
                        if (countCoil == 2) goto TubeFound1;
                    }
                }

            TubeFound1:
                if (_listCoilShuffleEnable.Count <= 1) return false;
                if (_tubeElementChange == null) return false;
                _coilElementChange = _listCoilShuffleEnable[0];
                _listCoilShuffleEnable.RemoveAt(0);
                if (_tubeElementChange1 == null) return false;
                _coilElementChange1 = _listCoilShuffleEnable[0];
                _listCoilShuffleEnable.RemoveAt(0);
            }
            return true;
        }
        public bool CheckShuffleBoosterNew()
        {
            var coilElementsInStage = gridController.GetCoilListInStage();
            var tubeElementInStage = gridController.tubeElements;
            _coilInSlot = m_SlotController.GetCoilElementListMatch();
            _coilInSlotSameColor.Clear();
            _listCoilShuffleEnable.Clear();
            _listCoilShuffle = coilElementsInStage.Where(x => x.coilStatus is CoilStatus.Enable or CoilStatus.Disable or CoilStatus.Mystery).OrderBy(x => new System.Random().Next()).ToList();
            if (_listCoilShuffle.Count < 2)
            {
                ActionEvent.OnShowToast?.Invoke("Not enough coil to shuffle!");
                return false;
            }
            if (_coilInSlot.Count <= 0) return true;
            foreach (var coil in _coilInSlot)
            {
                if (!colorCountInSlot.TryAdd(coil.coilColor, 1)) colorCountInSlot[coil.coilColor] += 1;
            }

            foreach (var coil in coilElementsInStage)
            {
                if (!colorCountInSlot.TryAdd(coil.coilColor, 1)) colorCountInSlot[coil.coilColor] += 1;
            }

            var colorTemp = Color.clear;
            foreach (var dict in colorCountInSlot)
            {
                if (dict.Value < 3) continue;
                colorTemp = dict.Key;
                Debug.Log("======0");
                break;
            }

            if (colorTemp == Color.clear)
            {
                Debug.Log("======1");
                foreach (var dict in colorCountInSlot)
                {
                    if (dict.Value < 2) continue;
                    colorTemp = dict.Key;
                    break;
                }
                Debug.Log("======2");
                _coilElementChange = _listCoilShuffle.Where(coil => coil.coilStatus == CoilStatus.Enable).ToList()[0];
                _listCoilShuffle.Remove(_coilElementChange);
                foreach (var tub in tubeElementInStage)
                {
                    foreach (var color in tub.coilValues)
                    {
                        if (color == colorTemp)
                        {
                            _tubeElementChange = tub;
                            _colorChange = color;
                            _indexColor = tub.coilValues.IndexOf(color);
                            Debug.Log("======3");
                            goto TubeFound;
                        }
                    }
                }
            }

        TubeFound:

            foreach (var coil in _listCoilShuffle.Where(coil => coil.coilStatus is CoilStatus.Enable or CoilStatus.Disable or CoilStatus.Mystery &&
                                                                coil.coilColor == colorTemp && coil != _coilElementChange).ToList())
            {
                _coilInSlotSameColor.Add(coil);
                _listCoilShuffle.Remove(coil);
            }
            foreach (var coil in _listCoilShuffle.ToList().Where(coil => coil.coilStatus == CoilStatus.Enable && coil != _coilElementChange))
            {
                _listCoilShuffleEnable.Add(coil);
                _listCoilShuffle.Remove(coil);
            }
            if (_coilInSlotSameColor.Count > 0)
            {
                for (var i = _coilInSlotSameColor.Count - 1; i >= 0; i--)
                {
                    for (var j = _listCoilShuffleEnable.Count - 1; j >= 0; j--)
                    {
                        if (_coilInSlotSameColor[i].coilColor == _listCoilShuffleEnable[j].coilColor)
                        {
                            _listCoilShuffle.Add(_listCoilShuffleEnable[j]);
                            _listCoilShuffleEnable.RemoveAt(j);
                        }
                    }
                }
            }
            else
            {
                for (var i = _coilInSlotSameColor.Count - 1; i >= 0; i--)
                {
                    _listCoilShuffle.Add(_coilInSlotSameColor[i]);
                    _coilInSlotSameColor.RemoveAt(i);
                }
                for (var i = _listCoilShuffleEnable.Count - 1; i >= 0; i--)
                {
                    _listCoilShuffle.Add(_listCoilShuffleEnable[i]);
                    _listCoilShuffleEnable.RemoveAt(i);
                }
            }
            return true;
        }

        public async UniTask ShuffleBooster()
        {
            if (isUsingBooster) return;
            isUsingBooster = true;
            if (_coilInSlotSameColor.Count > 0)
            {
                for (var i = _listCoilShuffleEnable.Count - 1; i >= 0; i--)
                {
                    if (i >= _coilInSlotSameColor.Count) continue;
                    if (_coilInSlotSameColor[i].coilColor == _listCoilShuffleEnable[i].coilColor) continue;
                    var coil1 = _listCoilShuffleEnable[i];
                    var coil2 = _coilInSlotSameColor[i];
                    var st1 = coil1.coilStatus;
                    var st2 = coil2.coilStatus;
                    SwapCoil(coil1, coil2);
                    UniTask.Delay(TimeSpan.FromSeconds(0.1f)).ContinueWith(() =>
                    {
                        MoveAllCoilToBoard(coil1, coil2, st1, st2).Forget();
                    }).Forget();
                    _listCoilShuffleEnable.RemoveAt(i);
                    _coilInSlotSameColor.RemoveAt(i);
                }
                for (var i = _listCoilShuffleEnable.Count - 1; i >= 0; i--)
                {
                    _listCoilShuffle.Add(_listCoilShuffleEnable[i]);
                    _listCoilShuffleEnable.RemoveAt(i);
                }
                for (var i = _coilInSlotSameColor.Count - 1; i >= 0; i--)
                {
                    _listCoilShuffle.Add(_coilInSlotSameColor[i]);
                    _coilInSlotSameColor.RemoveAt(i);
                }

                if (_listCoilShuffle.Count <= 0)
                {
                    ShuffleEffect().Forget();
                    await UniTask.Delay(TimeSpan.FromSeconds(1f));
                    isUsingBooster = false;
                }
            }

            if (_listCoilShuffle == null || _listCoilShuffle.Count < 2) return;

            var a = Enumerable
                .Range(0, _listCoilShuffle.Count / 2)
                .Select(i => (_listCoilShuffle.Skip(i * 2).Take(1).First(), _listCoilShuffle.Skip(i * 2 + 1).Take(1).First()))
                .ToList();
            if (_tubeElementChange != null)
            {
                _tubeElementChange.ChangeColorWithIndex(_indexColor, _coilElementChange.coilColor);
                SwapCoil(_coilElementChange, _colorChange);
                _tubeElementChange = null;
                _coilElementChange = null;
            }

            if (_tubeElementChange1 != null)
            {
                _tubeElementChange1.ChangeColorWithIndex(_indexColor1, _coilElementChange1.coilColor);
                SwapCoil(_coilElementChange1, _colorChange);
                _tubeElementChange1 = null;
                _coilElementChange1 = null;
            }
            ShuffleEffect().Forget();
            var status = a.Select(x => (x.Item1.coilStatus, x.Item2.coilStatus)).ToList();
            foreach (var t in a)
            {
                SwapCoil(t.Item1, t.Item2);
                await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
            }

            for (var index = 0; index < a.Count; index++)
            {
                var t = a[index];
                var status1 = status[index];
                MoveAllCoilToBoard(t.Item1, t.Item2, status1.Item1, status1.Item2).Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            isUsingBooster = false;
        }

        public Vector3 swapOffset = new Vector3(0, 2, 0);
        public int delayDespawn = 3600;
        public async void SwapCoil(CoilElement coil1, CoilElement coil2)
        {
            //var coil1Status = coil1.coilStatus;
            //var coil2Status = coil2.coilStatus;
            coil1.ChangeStatus(CoilStatus.Transfer);
            coil1.SetOutline(false);
            coil1.SetFakeShadow(false);
            // coil1.animation1.PlayAsync("Coil Enable", 0.1f).ContinueWith(() =>
            // {
            //     coil1.SetOutline(false);
            //     coil1.SetFakeShadow(false);
            // }).Forget();

            coil2.ChangeStatus(CoilStatus.Transfer);
            coil2.SetOutline(false);
            coil2.SetFakeShadow(false);
            // coil2.animation1.PlayAsync("Coil Enable", 0.1f).ContinueWith(() =>
            // {
            //     coil2.SetOutline(false);
            //     coil2.SetFakeShadow(false);
            // }).Forget();
            //coil1.transform.DOLocalMoveY(2, 0.2f);
            //coil2.transform.DOLocalMoveY(2, 0.2f);
            //await UniTask.Delay(200);
            //coil1.FillToRoll(1.4f, currentStage.roll.transform,swapOffset, coil1Status).Forget();
            //coil2.FillToRoll(1.4f, currentStage.roll.transform,swapOffset, coil2Status).Forget();
            coil1.MoveToShuffle(0.5f, gridController.shuffleParticle.transform).Forget();
            coil2.MoveToShuffle(0.5f, gridController.shuffleParticle.transform).Forget();
            await UniTask.Delay(500);
            var colorTemp = coil1.coilColor;
            coil1.ChangeColor(coil2.coilColor);
            coil2.ChangeColor(colorTemp);
            //coil1.RollToCoil(1.5f, currentStage.roll.transform,swapOffset, coil1Status).Forget();
            //coil2.RollToCoil(1.5f, currentStage.roll.transform,swapOffset, coil2Status).Forget();
            //coil1.MoveToBoard(0.5f).Forget();
            //coil2.MoveToBoard(0.5f).Forget();
            //await UniTask.Delay(1500);
            //coil1.transform.DOLocalMoveY(0, 0.2f);
            //coil2.transform.DOLocalMoveY(0, 0.2f);
            //await UniTask.Delay(500);
            //coil1.transform.localPosition = Vector3.zero;
            //coil2.transform.localPosition = Vector3.zero;
            //coil1.ChangeStatus(coil1Status == CoilStatus.Mystery ? CoilStatus.Mystery : CoilStatus.Disable);
            //coil1.CheckMove();
            //coil2.ChangeStatus(coil2Status == CoilStatus.Mystery ? CoilStatus.Mystery : CoilStatus.Disable);
            //coil2.CheckMove();
        }

        public async UniTask MoveAllCoilToBoard(CoilElement coil1, CoilElement coil2, CoilStatus coil1Status, CoilStatus coil2Status)
        {
            coil1.MoveToBoard(0.5f).Forget();
            coil2.MoveToBoard(0.5f).Forget();
            await UniTask.Delay(500);
            coil1.transform.localPosition = Vector3.zero;
            coil2.transform.localPosition = Vector3.zero;
            coil1.ChangeStatus(coil1Status == CoilStatus.Mystery ? CoilStatus.Mystery : CoilStatus.Disable);
            coil1.CheckMove();
            coil2.ChangeStatus(coil2Status == CoilStatus.Mystery ? CoilStatus.Mystery : CoilStatus.Disable);
            coil2.CheckMove();
        }

        public async void SwapCoil(CoilElement coil1, Color colorSwap)
        {
            // var coil1Status = coil1.coilStatus;
            // coil1.ChangeStatus(CoilStatus.Transfer);
            // coil1.animation1.PlayAsync("Coil Enable", 0.1f).ContinueWith(() =>
            // {
            //     coil1.SetOutline(false);
            //     coil1.SetFakeShadow(false);
            // }).Forget();
            // coil1.transform.DOLocalMoveY(2, 0.2f);
            // await UniTask.Delay(200);
            // coil1.FillToRoll(1.4f, currentStage.roll.transform,swapOffset, coil1Status).Forget();
            // await UniTask.Delay(2500);
            // coil1.ChangeColor(colorSwap);
            // coil1.RollToCoil(1.5f, currentStage.roll.transform,swapOffset, coil1Status).Forget();
            // await UniTask.Delay(1500);
            // coil1.transform.DOLocalMoveY(0, 0.2f);
            // await UniTask.Delay(200);
            // coil1.transform.localPosition = Vector3.zero;
            // coil1.ChangeStatus(coil1Status == CoilStatus.Mystery ? CoilStatus.Mystery : CoilStatus.Disable);
            // coil1.CheckMove();
            var coil1Status = coil1.coilStatus;
            coil1.ChangeStatus(CoilStatus.Transfer);
            coil1.SetOutline(false);
            coil1.SetFakeShadow(false);
            coil1.MoveToShuffle(0.5f, gridController.shuffleParticle.transform).Forget();
            await UniTask.Delay(500);
            coil1.ChangeColor(colorSwap);
            coil1.MoveToBoard(0.5f).Forget();
            await UniTask.Delay(500);
            coil1.transform.localPosition = Vector3.zero;
            coil1.ChangeStatus(coil1Status == CoilStatus.Mystery ? CoilStatus.Mystery : CoilStatus.Disable);
            coil1.CheckMove();
        }

        private async UniTask ShuffleEffect()
        {
            gridController.shuffleParticle.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            gridController.shuffleParticle.Stop();
        }

        private async UniTask ScaleRoll()
        {
            await UniTask.Delay(470);
            gridController.roll.gameObject.SetActive(true);
            gridController.roll.SetAutoStart(true);
            gridController.roll.SetSpeedAnim(1f);
            var effect = PoolingManager.instance.mergeParticlePool.Get();
            effect.transform.position = gridController.roll.transform.position;
            UniTask.Delay(500).ContinueWith(() => PoolingManager.instance.mergeParticlePool.Release(effect)).Forget();
            //currentStage.roll.RevertRotate(1f);
            await UniTask.Delay(delayDespawn);
            var effect1 = PoolingManager.instance.mergeParticlePool.Get();
            effect1.transform.position = gridController.roll.transform.position;
            UniTask.Delay(500).ContinueWith(() => PoolingManager.instance.mergeParticlePool.Release(effect)).Forget();
            gridController.roll.gameObject.SetActive(false);
        }

        private async UniTask ScaleRollCollect(Color color)
        {
            gridController.rollCollect.ChangeColor(color);
            await UniTask.Delay(450);
            gridController.rollCollect.gameObject.SetActive(true);
            gridController.rollCollect.SetSpeedAnim(1f);
            var effect = PoolingManager.instance.mergeParticlePool.Get();
            effect.transform.position = gridController.rollCollect.transform.position;
            UniTask.Delay(500).ContinueWith(() => PoolingManager.instance.mergeParticlePool.Release(effect)).Forget();
            //currentStage.roll.RevertRotate(1f);
            await UniTask.Delay(2200);
            gridController.rollCollect.gameObject.SetActive(false);
        }

        public bool CheckReviveBoosterAddSlot()
        {
            return m_SlotController.CheckAddSlotExtraBooster(3);
        }

        public void ReviveBooster()
        {
            Debug.Log("ReviveBooster");
            if (CheckReviveBoosterAddSlot())
            {
                CheckAddSlotBooster();
                AddSlotBooster();
            }
            else
            {
                for (int i = 1; i < 4; i++)
                {
                    UniTask.Delay(i * 100).ContinueWith(() =>
                    {
                        CheckUndoBoosterToSlotExtra();
                        UndoBoosterToSlotExtra();
                    }).Forget();
                }
            }
            endGame = false;
        }
    }
}


