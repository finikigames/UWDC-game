using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Core.Collections.Native
{
    public struct ReadOnlyNativeArray2D<T> where T : struct
    {
        [ReadOnly] 
        [NativeDisableParallelForRestriction]
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<T>.ReadOnly InnerArray;

        public readonly int Width;
        public readonly int Height;

        public ReadOnlyNativeArray2D(NativeArray<T> array, int width, int height)
        {
            InnerArray = array.AsReadOnly();
            Width = width;
            Height = height;
        }
            
        public T this[int row, int column] 
            => InnerArray[row * Width + column];
    }
}