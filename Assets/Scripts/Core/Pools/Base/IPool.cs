using System;
using UnityEngine;

namespace Core.Pools.Base {
    public interface IPool {
        int NumTotal { get; }
        int NumActive { get; }
        int NumInactive { get; }

        IPool SetParentContainer(Transform poolContainer);
        
        // Pool usage
        void Release(MonoBehaviour go);
        void Release(GameObject go);
        void InitialFill();
        void Instantiate(int expandCount);

        T Take<T>(bool worldPositionStays = true) where T : MonoBehaviour;
        T GetFirst<T>() where T : MonoBehaviour;
        void Dispose();
        IPool SetResetProcess(Action<GameObject> process);
        IPool SetInstantiateProcess(Action<GameObject> instantiateProcess);
    }
}