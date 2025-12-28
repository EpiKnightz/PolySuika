using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils.Pool
{

    public interface IGameObjectPool
    {
        public void Destroy(Poolable poolable);
    }

}
