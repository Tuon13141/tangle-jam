using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Game.WoolSort.Controller
{
    using Game.WoolSort.Data;
    using Game.WoolSort.Element;
    using Percas;

    public class LevelController : MonoBehaviour
    {
        public static LevelController instance;

        [SerializeField] LevelConfig m_LevelConfig;
        [SerializeField] bool m_AutoLoadLevel;

        [Space]
        [SerializeField] Camera m_MainCamera;
        [SerializeField] SpriteRenderer m_Background;

        [Space]
        [SerializeField] PictureController m_PictureController;
        [SerializeField] SlotController m_SlotController;
        [SerializeField] GridController m_GridConroller;

        [Space]
        [SerializeField] CanvasGroup m_WinPupup;
        [SerializeField] CanvasGroup m_LosePupup;
        [SerializeField] GameObject m_GameplayUI;

        public Camera mainCamera => m_MainCamera;

        public void ShowPopUpWin()
        {
            var current = PlayerPrefs.GetInt("CURRENT_NEW_MODE", 1);
            PlayerPrefs.SetInt("CURRENT_NEW_MODE", current + 1);
            m_GameplayUI.SetActive(false);

            Percas.ServiceLocator.PopupScene.ShowPopup(PopupName.LevelWin);
            Texture2D tex = levelData.ToTexture2D();
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            GameLogic.UpdateLevelImage(sprite);
        }

        public void ShowPopUpLose()
        {
            m_GameplayUI.SetActive(false);
            m_LosePupup.DOFade(1, 0.3f).From(0).OnComplete(() =>
            {
                m_LosePupup.interactable = true;
                m_LosePupup.blocksRaycasts = true;
            });
        }

        [Space]
        public LevelData levelData;

        public PictureController pictureController => m_PictureController;
        public SlotController slotController => m_SlotController;
        public GridController gridController => m_GridConroller;

        public bool isEndGame;

        private void Awake()
        {
            instance = this;

            Input.multiTouchEnabled = false;
            Application.targetFrameRate = 60;

            SetupLevel();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Break();
            }

            if (Input.GetMouseButtonDown(0) && !isEndGame && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 mousePos = m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 100, LayerMask.GetMask("Coil"));
                if (hit.collider != null)
                {
                    Debug.Log("Clicked: " + hit.collider.gameObject.name);
                    var wool = hit.collider.GetComponent<WoolElement>();
                    wool.OnPointerClick(null);
                }
                else
                {
                    Debug.Log("Clicked: Null");

                }
            }
        }

        public void ResetLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void NextScene()
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }

        [Button("Setup Level")]
        private void SetupLevel()
        {
            var current = PlayerPrefs.GetInt("CURRENT_NEW_MODE", 1);

            levelData = m_LevelConfig.GetLevelData(current);

            m_PictureController.Setup(levelData);
            m_SlotController.Setup(levelData);
            m_GridConroller.Setup(levelData);
        }

        [Button("Overwrite Level")]
        public void OverwriteLevel()
        {
            //levelData
        }

        [Button("Analytics")]
        public void Analytics(bool randomColor)
        {
            var shapeElements = m_GridConroller.GetComponentsInChildren<ShapeElement>();
            var woolElements = new List<WoolElement>();
            foreach (var shape in shapeElements)
            {
                if (randomColor)
                {
                    shape.color = (ColorType)UnityEngine.Random.Range(1, 11);
                    shape.ChangeColor(shape.color);

#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(shape);
#endif
                }

                var wools = shape.GetComponentsInChildren<WoolElement>();
                woolElements.AddRange(wools);
            }

            Debug.LogFormat("Shape count: {0}", shapeElements.Length);
            Debug.LogFormat("Wool count: {0}", woolElements.Count);

            var availableColors = levelData.colors.Select(x => (int)x).ToList();

            if (availableColors.Count == 0 || woolElements.Count % 3 != 0)
            {
                Debug.LogError("Invalid color list or element count must be multiple of 3.");
                return;
            }

            int groupCount = woolElements.Count / 3;
            List<int> colorPool = new List<int>();

            // Lặp lại màu sao cho tổng số nhóm là groupCount
            int fullRepeats = groupCount / availableColors.Count;
            int remainder = groupCount % availableColors.Count;

            foreach (var color in availableColors)
            {
                for (int i = 0; i < fullRepeats; i++)
                    colorPool.Add(color);
            }

            List<int> shuffledColors = new List<int>(availableColors);
            Shuffle(shuffledColors);
            for (int i = 0; i < remainder; i++)
                colorPool.Add(shuffledColors[i]);

            // Shuffle lại colorPool để màu phân bố ngẫu nhiên
            Shuffle(colorPool);

            // Gán màu cho mỗi nhóm 3 phần tử
            Shuffle(woolElements); // Nếu bạn muốn thứ tự phần tử được trộn
            for (int i = 0; i < groupCount; i++)
            {
                int color = colorPool[i];
                for (int j = 0; j < 3; j++)
                {
                    int index = i * 3 + j;
                    AssignColor(woolElements[index], color);
                }
            }

            var data = new List<ShapeData>();
            foreach (var shape in shapeElements)
            {
                var shapeData = new ShapeData();
                shapeData.type = shape.id;
                shapeData.layer = shape.GetComponentInParent<SortingGroup>().sortingOrder;
                shapeData.color = shape.color;
                shapeData.position = shape.transform.position;
                shapeData.rotation = shape.transform.eulerAngles;
                shapeData.scale = Vector3.one;


                var wools = shape.GetComponentsInChildren<WoolElement>();
                shapeData.woolDatas = wools.Select(x =>
                {
                    var data = new WoolData();
                    data.color = x.color;
                    data.position = x.transform.localPosition;
                    return data;
                }).ToArray();
                data.Add(shapeData);
            }

            levelData.countLayer = m_GridConroller.layerGroups.Count;
            levelData.shapeDatas = data.ToArray();

            SaveSlotOrder(colorPool);
        }

        void AssignColor(WoolElement obj, int color)
        {
            obj.color = (ColorType)color;

#if UNITY_EDITOR
            obj.OnValidate();
            UnityEditor.EditorUtility.SetDirty(obj);
#endif
        }

        void SaveSlotOrder(List<int> orders)
        {
            levelData.orderSlot = orders.Select(x => (byte)x).ToArray();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(levelData);
#endif
        }

        void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = UnityEngine.Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }
    }
}