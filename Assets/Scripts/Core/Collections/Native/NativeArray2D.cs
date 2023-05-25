using System;
using System.Collections;
using System.Collections.Generic;
using Core.Extensions;
using Unity.Collections;

namespace Core.Collections.Native
{
    public static class NativeArray2DintExtensions {
        public static bool Equals(NativeArray2D<int> s1, NativeArray2D<int> s2) {
            return s1.InnerArray == s2.InnerArray;
        }
    }
    
    public struct NativeArray2D<T> : IDisposable, IEnumerable<T> where T : struct
    {
        [NativeDisableParallelForRestriction] 
        public NativeArray<T> InnerArray;
        
        public readonly int Width;
        public readonly int Height;
            
        public NativeArray2D(int height, int width, Allocator allocator = Allocator.Temp, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
        {
            InnerArray = new NativeArray<T>(width * height, allocator, options);
            Width = width;
            Height = height;
        }
        
        public NativeArray2D(int height, int width, NativeArray<T> array)
        {
            InnerArray = array;
            Width = width;
            Height = height;
        }

        public static NativeArray2D<T> CreatePersistentUninitialized(int height, int width)
        {
            return new NativeArray2D<T>(height, width, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        }
 
        public void Dispose()
        {
            InnerArray.Dispose();
        }

        public IEnumerator<T> GetEnumerator() 
            => InnerArray.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}