using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils.Pool
{
    public class GameObjectMultiplePool : MonoBehaviour, IGameObjectPool
    {
#if UNITY_EDITOR
        public class ObjectRegistry
#else
        private class ObjectRegistry
#endif
        {
            public Poolable PooledObject;
            public bool InUse;
        }

#if UNITY_EDITOR
        public class PoolRegistry
#else
        private class PoolRegistry
#endif
        {
            public Poolable PrefabTemplate;
            public List<ObjectRegistry> Pool;
        }

#if UNITY_EDITOR
        public List<PoolRegistry> m_Pools;
#else
        private List<PoolRegistry> m_Pools;
#endif

        public void Awake()
        {
            m_Pools = new List<PoolRegistry>();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < m_Pools.Count; i++)
            {
                PoolRegistry thisSubPool = m_Pools[i];
                for (int j = 0; j < thisSubPool.Pool.Count; j++)
                {
                    thisSubPool.Pool[j].PooledObject.GOPool = null;
                }
            }
        }

        private Poolable CreateNewObject(Poolable prefabTemplate, List<ObjectRegistry> pool)
        {
            Poolable newObj = Instantiate(prefabTemplate);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            int numIPoolable = newObj.GetComponents<IPoolable>().Length;
            if (numIPoolable > 1)
            {
                Debug.LogWarning($"Prefab {prefabTemplate.name} has more than 1 IPoolable interface, please recheck");
            }
#endif
            newObj.PoolableListener = newObj.GetComponent<IPoolable>();
            newObj.gameObject.SetActive(false);
            newObj.transform.SetParent(transform);
            newObj.transform.localPosition = Vector3.zero;
            newObj.transform.localRotation = Quaternion.identity;
            newObj.transform.localScale = Vector3.one;
            newObj.GOPoolId = GetInstanceID();
            newObj.PoolIndex = -1;
            newObj.GOPool = this;
            newObj.Index = pool.Count;
            newObj.name = string.Format("pooled_{0}_{1}", prefabTemplate.name, newObj.Index);
            return newObj;
        }

        public GameObject Create(Poolable prefabTemplate, bool activation = true)
        {
            int poolIndex = -1;
            // find pool
            int numPools = m_Pools.Count;
            List<ObjectRegistry> pool = null;
            for (int i = 0; i < numPools; i++)
            {
                if (m_Pools[i].PrefabTemplate == prefabTemplate)
                {
                    poolIndex = i;
                    pool = m_Pools[i].Pool;
                    break;
                }
            }

            if (pool == null)
            {
                pool = new List<ObjectRegistry>();
                PoolRegistry poolRegistry = new PoolRegistry
                {
                    PrefabTemplate = prefabTemplate,
                    Pool = pool
                };
                poolIndex = m_Pools.Count;
                m_Pools.Add(poolRegistry);
                Debug.LogFormat("Added new Pool for template '{0}'.", prefabTemplate.name);
            }

            Debug.Assert(poolIndex >= 0);

            // find object
            Poolable newObj = null;
            int numObjects = pool.Count;
            for (int i = 0; i < numObjects; i++)
            {
                ObjectRegistry registry = pool[i];
                if (!pool[i].InUse)
                {
                    registry.InUse = true;
                    newObj = registry.PooledObject;
                    newObj.gameObject.SetActive(activation);
                    break;
                }
            }

            if (newObj == null)
            {
                Poolable poolable = CreateNewObject(prefabTemplate, pool);
                poolable.PoolIndex = poolIndex;

                ObjectRegistry registry = new ObjectRegistry
                {
                    PooledObject = poolable,
                    InUse = true
                };
                pool.Add(registry);
                newObj = registry.PooledObject;
                newObj.gameObject.SetActive(activation);
            }

            if (newObj.PoolableListener != null)
            {
                newObj.PoolableListener.OnCreate();
            }
            return newObj.gameObject;
        }

        public void Destroy(Poolable poolable)
        {
            Debug.Assert(poolable.GOPoolId == GetInstanceID());
            if (poolable.PoolableListener != null)
            {
                poolable.PoolableListener.OnReturnedToPool();
            }
            poolable.gameObject.SetActive(false);
            poolable.transform.SetParent(transform);
            var pool = m_Pools[poolable.PoolIndex].Pool;
            pool[poolable.Index].InUse = false;
        }
    }

}
