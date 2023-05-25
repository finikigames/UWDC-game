using Core.Pools.Base;
using UnityEngine;

namespace Core.Pools.PoolFactory.Base {
    public interface IGameObjectsPoolFactory {
        IPool Create(GameObject gameObject, ExpandType type, ExpandType expandType, int size);
    }
}