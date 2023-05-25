using System;
using Core.Collections.Native;
using Core.Primitives;

namespace Core.Extensions
{
    public static class ReadOnlyNativeArray2DExtensions
    {
        public static ReadOnlyNativeArray2D<T> ToReadOnly<T>(this NativeArray2D<T> array) where T : struct
        {
            return new ReadOnlyNativeArray2D<T>(array.InnerArray, array.Width, array.Height);
        }

        public static T GetByCoords<T>(this ReadOnlyNativeArray2D<T> arr, Coords coords) where T : unmanaged
        {
            if (!arr.In(coords.Row, coords.Column))
            {
                throw new ArgumentOutOfRangeException($"Try to get element by {coords}");
            }

            return arr[coords.Row, coords.Column];
        }

        // Let you know coordinates from index
        public static JobTuple<int, int> GetBounds<T>(this ReadOnlyNativeArray2D<T> array, int index)
            where T : unmanaged
        {
            int column = index % array.Width;
            int row = index / array.Width;

            return new JobTuple<int, int>(row, column);
        }
        
        public static void GetBoundsInt<T>(this ReadOnlyNativeArray2D<T> array, int index, out int row, out int column)
            where T : unmanaged
        {
            column = index % array.Width;
            row = index / array.Width;
        }
        
        public static bool In<T>(this ReadOnlyNativeArray2D<T> arr, int row, int column) where T : struct
        {
            if (row < 0 || column < 0) return false;
            if (row >= arr.Height) return false;
            if (column >= arr.Width) return false;

            return true;
        }
    }
}