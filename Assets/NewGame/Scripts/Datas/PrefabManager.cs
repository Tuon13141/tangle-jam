using Elements;
using UnityEngine;

namespace Game.WoolSort.Data
{
    using Game.WoolSort;
    using Game.WoolSort.Element;

    //[CreateAssetMenu(fileName = "PrefabManager", menuName = "ScriptableObjects/PrefabManager1", order = 1)]
    public class PrefabManager : ScriptableObject
    {
        [Header("Element")]
        [SerializeField] RopeWaveRenderer m_RopePrefab;

        [SerializeField] WoolElement m_WoolElement;
        [SerializeField] TupleSerialize<int, ShapeElement>[] m_ShapeElements;

        public RopeWaveRenderer ropePrefab => m_RopePrefab;
        public WoolElement woolElement => m_WoolElement;


        public ShapeElement GetShapeElement(int id)
        {
            foreach (var prefab in m_ShapeElements)
            {
                if (prefab.Value1 == id) return prefab.Value2;
            }

            return null;
        }
    }
}