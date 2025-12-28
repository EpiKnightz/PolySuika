using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils.Pool
{

    /// <summary>
    /// ObjectPool of a single prefab template
    /// </summary>
    public class GameObjectPool : MonoBehaviour, IGameObjectPool
    {
        public Poolable PrefabTemplate;
        public int InitialNumber = 4;

        private class ObjectRegistry
        {
            public Poolable PooledObject;
            public bool InUse;
        }

        private List<ObjectRegistry> m_Pool;

        private Poolable CreateNewObject()
        {
            Poolable newObj = Instantiate(PrefabTemplate);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            int numIPoolable = newObj.GetComponents<IPoolable>().Length;
            if (numIPoolable > 1)
            {
                Debug.LogWarning($"Prefab {PrefabTemplate.name} has more than 1 IPoolable interface, please recheck");
            }
#endif
            newObj.PoolableListener = newObj.GetComponent<IPoolable>();
            newObj.gameObject.SetActive(false);
            newObj.transform.SetParent(transform);
            newObj.transform.localPosition = Vector3.zero;
            newObj.transform.rotation = newObj.OrgRotation;
            newObj.transform.localScale = newObj.OrgScale;
            newObj.GOPoolId = GetInstanceID();
            newObj.PoolIndex = -1;
            newObj.GOPool = this;
            newObj.Index = m_Pool.Count;
            newObj.name = string.Format("pooled_{0}_{1}", PrefabTemplate.name, newObj.Index);
            return newObj;
        }

        public void Awake()
        {
            if (PrefabTemplate != null)
            {
                m_Pool = new List<ObjectRegistry>();
                for (int i = 0; i < InitialNumber; i++)
                {
                    m_Pool.Add(new ObjectRegistry
                    {
                        PooledObject = CreateNewObject(),
                        InUse = false
                    });
                }
            }
            else
            {
                Debug.LogWarningFormat("PrefabTemplate of '{0}' is null, please recheck!", name);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < m_Pool.Count; i++)
            {
                m_Pool[i].PooledObject.GOPool = null;
            }
        }

        public GameObject Create(bool activation = true)
        {
            int numObjectsInPool = m_Pool.Count;
            Poolable newObj = null;
            for (int i = 0; i < numObjectsInPool; i++)
            {
                ObjectRegistry registry = m_Pool[i];
                if (!m_Pool[i].InUse)
                {
                    registry.InUse = true;
                    newObj = registry.PooledObject;
                    newObj.gameObject.SetActive(activation);
                    break;
                }
            }

            if (newObj == null)
            {
                ObjectRegistry registry = new ObjectRegistry
                {
                    PooledObject = CreateNewObject(),
                    InUse = true
                };
                m_Pool.Add(registry);
                newObj = registry.PooledObject;
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
            //poolable.transform.localPosition = Vector3.zero;
            //poolable.transform.localRotation = Quaternion.identity;
            //poolable.transform.localScale = Vector3.one;
            m_Pool[poolable.Index].InUse = false;
        }
    }

}
