using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Collections.Native
{
    public struct JobTuple<T1, T2>
        : IStructuralComparable, 
            IStructuralEquatable,
            IComparable,
            IComparable<JobTuple<T1, T2>>,
            IEquatable<JobTuple<T1, T2>>
    {
        private static readonly int RandomSeed = new Unity.Mathematics.Random().NextInt(
            int.MinValue,
            int.MaxValue);

        public readonly T1 Item1;

        public readonly T2 Item2;

        public JobTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public int CompareTo(JobTuple<T1, T2> other)
        {
            if (Comparer<T1>.Default.Compare(Item1, other.Item1) != 0)
            {
                return Comparer<T1>.Default.Compare(Item1, other.Item1);
            }
            
            return Comparer<T2>.Default.Compare(Item2, other.Item2);
        }

        public bool Equals(JobTuple<T1, T2> other)
        {
            return EqualityComparer<JobTuple<T1, T2>>.Equals(
                       Item1,
                       other.Item1)
                   && EqualityComparer<JobTuple<T1, T2>>.Equals(
                       Item2,
                       other.Item2);
        }

        private static int Combine(int h1, int h2)
        {
            // RyuJIT optimizes this to use the ROL instruction
            // Related GitHub pull request: dotnet/coreclr#1830
            uint rol5 = ((uint) h1 << 5) | ((uint) h1 >> 27);
            return ((int) rol5 + h1) ^ h2;
        }
     
        private static int CombineHashCodes(int h1, int h2)
        {
            return Combine(Combine(RandomSeed, h1), h2);
        }

        public override int GetHashCode()
        {
            return CombineHashCodes(
                Item1?.GetHashCode() ?? 0,
                Item2?.GetHashCode() ?? 0);
        }
     
        int IStructuralComparable.CompareTo(object obj, IComparer comparer)
        {
            if (obj == null)
            {
                return 1;
            }
     
            if (!(obj is JobTuple<T1, T2>))
            {
                throw new ArgumentException("Incorrect type", "obj");
            }
     
            JobTuple<T1, T2> other = (JobTuple<T1, T2>) obj;
     
            if (Comparer<T1>.Default.Compare(Item1, other.Item1) != 0)
            {
                return Comparer<T1>.Default.Compare(Item1, other.Item1);
            }
            
            return Comparer<T2>.Default.Compare(Item2, other.Item2);
        }
     
        bool IStructuralEquatable.Equals(object obj, IEqualityComparer comparer)
        {
            if (!(obj is JobTuple<T1, T2>))
            {
                return false;
            }
     
            JobTuple<T1, T2> other = (JobTuple<T1, T2>) obj;
            return comparer.Equals(
                       Item1,
                       other.Item1)
                   && comparer.Equals(
                       Item2,
                       other.Item2);
        }
     
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return CombineHashCodes(
                Item1?.GetHashCode() ?? 0,
                Item2?.GetHashCode() ?? 0);
        }
     
        int IComparable.CompareTo(object other)
        {
            if (other == null)
            {
                return 1;
            }
     
            if (!(other is JobTuple<T1, T2> tuple))
            {
                throw new ArgumentException("Incorrect type", "other");
            }
     
            return CompareTo(tuple);
        }
     
        public override string ToString()
        {
            return $"({Item1}, {Item2})";
        }
    }
}