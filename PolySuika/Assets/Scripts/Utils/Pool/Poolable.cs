using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Utils.Pool
{

    public interface IPoolable
    {
        public void OnCreate();
        public void OnReturnedToPool();
    }

    public class Poolable : MonoBehaviour
    {
        /* NOTE
         * If a Poolable Object prefab is disable, Awake() and OnEnabled() will NOT be called when creating the pool.
         */

        /* BEST PRACTICES
         * - Keep the Poolable object enabled in the prefab
         * - Register the OnCreate, OnReturnedToPool on OnEnable(). Unregister them on OnDisable()
         * - Create the Poolable object with activation param equals to 'true' in order to property register events
         */

        /// <summary>
        /// <b>OnCreate</b> will be invoked right after the Poolable is instantiate or reused,<br></br>
        /// this is the good place for you to reset this object's data before using.
        /// </summary>
        //public UnityAction OnCreate = null;

        /// <summary>
        /// <b>OnReturnedToPool</b> will be invoked right before the Poolable becomes Disabled.<br></br>
        /// Make sure you unregistered OnCreate and OnReturnedToPool on OnDisabled() function of MonoBehaviour.
        /// </summary>
        //public UnityAction OnReturnedToPool = null;

        /// <summary>
        /// instance id of the GameObject that implements IGameObjectPool, mostly for intergrity check
        /// </summary>
        [HideInInspector]
        public int GOPoolId;

        /// <summary>
        /// in case the pool can instantiate multiple object types, this variable will containts the
        /// index of the pool in the pool list
        /// </summary>
        [HideInInspector]
        public int PoolIndex;

        /// <summary>
        /// reference to the GameObject that holds the pool
        /// </summary>
        [HideInInspector]
        public IGameObjectPool GOPool;

        /// <summary>
        /// index of this object in the pool
        /// </summary>
        [HideInInspector]
        public int Index;

        [HideInInspector]
        public IPoolable PoolableListener = null;

        [System.NonSerialized]
        public Vector3 OrgPosition = Vector3.zero;
        [System.NonSerialized]
        public Vector3 OrgScale = Vector3.zero;
        [System.NonSerialized]
        public Quaternion OrgRotation = Quaternion.identity;

        private void Awake()
        {
            OrgPosition = transform.position;
            OrgScale = transform.localScale;
            OrgRotation = transform.rotation;
        }

        public void ReturnToPool()
        {
            if (GOPool != null)
            {
                GOPool.Destroy(this);
            }
            else
            {
                Debug.LogWarning($"No pool to return, destroying object '{gameObject.name}' for real!");
                Destroy(gameObject);
            }
        }
    }

}
