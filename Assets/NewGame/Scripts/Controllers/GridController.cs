using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Rendering;

namespace Game.WoolSort.Controller
{
    using Game.WoolSort.Data;
    using Game.WoolSort.Element;

    public class GridController : MonoBehaviour
    {
        [SerializeField] ColorManager m_ColorManager;
        [SerializeField] PrefabManager m_PrefabManager;

        [Space]
        [SerializeField] List<SortingGroup> m_LayerGroups;

        public List<WoolElement> woolElements;

        public List<ShapeElement> shapeElements;

        public int currentMaxLayer;

        public List<SortingGroup> layerGroups => m_LayerGroups;

        public LevelData levelData;

        private void Awake()
        {
            //currentMaxLayer = m_LayerGroups.Count;
            //SetLayer();
        }

        public void Setup(LevelData levelData)
        {
            this.levelData = levelData;

            transform.ClearContent();
            CreateLayer(levelData.countLayer);

            foreach (var shapeData in levelData.shapeDatas)
            {
                var prefab = m_PrefabManager.GetShapeElement(shapeData.type);
                var shapeElement = Static.InstantiateUtility(prefab, m_LayerGroups[shapeData.layer - 1].transform);

                shapeElement.transform.position = shapeData.position;
                shapeElement.transform.eulerAngles = shapeData.rotation;
                shapeElement.ChangeColor(shapeData.color);

                foreach (var woolData in shapeData.woolDatas)
                {
                    var woolElement = Static.InstantiateUtility(m_PrefabManager.woolElement, shapeElement.holder);

                    woolElement.transform.localPosition = woolData.position;
                    woolElement.transform.eulerAngles = Vector3.zero;
                    woolElement.ChangeColor(woolData.color);
                }
            }

            currentMaxLayer = m_LayerGroups.Count;
            SetLayer();
        }

        public void CheckLayer()
        {
            if (m_LayerGroups[currentMaxLayer - 1].GetComponentsInChildren<ShapeElement>().Length == 0)
            {
                currentMaxLayer--;
                SetLayer();
            }
        }

        public void SetLayer()
        {
            int count = 3;
            for (int i = Mathf.Max(0, currentMaxLayer - 1); i >= Mathf.Max(0, currentMaxLayer - 3); i--)
            {
                if (!m_LayerGroups[i].gameObject.activeSelf)
                {
                    m_LayerGroups[i].gameObject.SetActive(true);
                }

                var shapes = m_LayerGroups[i].GetComponentsInChildren<ShapeElement>();
                foreach (var shape in shapes)
                {
                    shape.colliderShape.gameObject.layer = LayerMask.NameToLayer(string.Format("Layer{0}", count));
                }

                count--;
            }

            for (int j = Mathf.Max(-1, currentMaxLayer - 4); j >= 0; j--)
            {
                Debug.Log(m_LayerGroups[j].gameObject.name);
                m_LayerGroups[j].gameObject.SetActive(false);
            }
        }

        public List<ShapeElement> GetAllShapeOverLayer(int index)
        {
            var result = new List<ShapeElement>();

            if (index > 0 && index < currentMaxLayer)
            {
                for (int i = index; i < currentMaxLayer; i++)
                {
                    var shapes = m_LayerGroups[i].GetComponentsInChildren<ShapeElement>();
                    result.AddRange(shapes);
                }
            }

            return result;
        }

        [Button]
        void CreateLayer(int count)
        {
            m_LayerGroups = new();

            for (int i = count; i > 0; i--)
            {
                var layer = new GameObject(string.Format("Layer {0}", i), typeof(SortingGroup));
                layer.transform.SetParent(transform);
                layer.transform.SetPosition(z: -1 * i);

                layer.transform.SetAsFirstSibling();

                var sortingGroup = layer.GetComponent<SortingGroup>();
                sortingGroup.sortingOrder = i;
                m_LayerGroups.Insert(0, sortingGroup);
            }
        }
    }
}
