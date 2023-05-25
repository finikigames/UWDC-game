using Core.Pools.Base;
using UnityEngine;

namespace Core.Pools {
    public class GameObjectsPool : BaseGamePool {
        public GameObjectsPool(GameObject go,
                               ExpandType expandType,
                               ExpandType expandPercent,
                               int size = 10) : base(go, expandType, expandPercent, size) {
            
        }
    }
}