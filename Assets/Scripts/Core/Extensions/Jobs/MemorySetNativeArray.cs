using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Core.Extensions.Jobs
{
    [BurstCompile]
    public struct MemorySetNativeArray<T> : IJobParallelFor where T : struct
    {
        public NativeArray<T> Array;
        public T Value;
            
        public void Execute(int index)
        {
            Array[index] = Value;
        }
    }
}