using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using NaughtyAttributes;

public class PoolingObject<T> where T : MonoBehaviour
{
    public ObjectPool<T> pool;

    public PoolingObject(T prefab, int maxSize)
    {
        pool = new ObjectPool<T>(() => CreateElement(prefab), OnGetPoolObject, OnReleasePoolObject, OnDestroyPoolObject, maxSize: maxSize);
    }

    public void Release(T element)
    {
        pool.Release(element);
    }

    public T Get()
    {
        return pool.Get();
    }

    private T CreateElement(T prefab)
    {
        return Static.InstantiateUtility(prefab, null);
    }

    private void OnGetPoolObject(T poolObject)
    {
        poolObject.gameObject.SetActive(true);
    }

    private void OnReleasePoolObject(T poolObject)
    {
        poolObject.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(T poolObject)
    {
        GameObject.Destroy(poolObject.gameObject);
    }
}

public class PoolingManager : Kit.Common.Singleton<PoolingManager>
{
    [SerializeField] PrefabManager m_PrefabManager;

    public PoolingObject<CoilElement> coilElementPool;
    [ShowNativeProperty]
    public string coilElementPoolInfo => string.Format("All: {0} -> Active: {1} - Inactive: {2}",
                                                                        coilElementPool?.pool.CountAll ?? 0,
                                                                        coilElementPool?.pool.CountActive ?? 0,
                                                                        coilElementPool?.pool.CountInactive ?? 0);

    public PoolingObject<MergeEffect> mergeParticlePool;
    [ShowNativeProperty]
    public string mergeParticlePoolInfo => string.Format("All: {0} -> Active: {1} - Inactive: {2}",
                                                                        mergeParticlePool?.pool.CountAll ?? 0,
                                                                        mergeParticlePool?.pool.CountActive ?? 0,
                                                                        mergeParticlePool?.pool.CountInactive ?? 0);
    

    public PoolingObject<RopeElement> ropePool;
    [ShowNativeProperty]
    public string ropePoolInfo => string.Format("All: {0} -> Active: {1} - Inactive: {2}",
                                                                        ropePool?.pool.CountAll ?? 0,
                                                                        ropePool?.pool.CountActive ?? 0,
                                                                        ropePool?.pool.CountInactive ?? 0);
    
    
    public PoolingObject<CoilRollEffect> coilEffectPool;
    [ShowNativeProperty]
    public string coilEffectInfo => string.Format("All: {0} -> Active: {1} - Inactive: {2}",
        coilEffectPool?.pool.CountAll ?? 0,
        coilEffectPool?.pool.CountActive ?? 0,
        coilEffectPool?.pool.CountInactive ?? 0);


    protected override void Awake()
    {
        base.Awake();
        coilElementPool = new PoolingObject<CoilElement>(m_PrefabManager.GetCoilPrefab(), 50);
        mergeParticlePool = new PoolingObject<MergeEffect>(m_PrefabManager.GetCoilFillCollectParticle(), 5);
        ropePool = new PoolingObject<RopeElement>(m_PrefabManager.GetRopePrefab(), 10);
        coilEffectPool = new PoolingObject<CoilRollEffect>(m_PrefabManager.GetCoilEffectPrefab(), 10);
    }
}
