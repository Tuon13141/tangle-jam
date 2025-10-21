using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.WoolSort.Controller
{
    public class PoolingController : MonoBehaviour
    {
        private static PoolingController _instance;

        public static PoolingController Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = (new GameObject("PoolingController", typeof(PoolingController)));
                    _instance = go.GetComponent<PoolingController>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        // prefabID => queue
        private Dictionary<int, Queue<Component>> pools = new();

        // instance => prefabID
        private Dictionary<Component, int> instanceToPrefabID = new();

        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            int id = prefab.gameObject.GetInstanceID();
            if (!pools.TryGetValue(id, out var queue))
            {
                queue = new Queue<Component>();
                pools[id] = queue;
            }

            T obj;
            if (queue.Count > 0)
            {
                obj = (T)queue.Dequeue();
            }
            else
            {
                obj = Instantiate(prefab, position, rotation);
                instanceToPrefabID[obj] = id;
            }

            obj.transform.SetPositionAndRotation(position, rotation);
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Release<T>(T obj) where T : Component
        {
            if (!instanceToPrefabID.TryGetValue(obj, out int id))
            {
                Debug.LogWarning($"Object {obj.name} not tracked by pool. Destroying.");
                Destroy(obj.gameObject);
                return;
            }

            obj.gameObject.SetActive(false);
            obj.transform.SetParent(transform);
            pools[id].Enqueue(obj);
        }
    }
}