using System;
using Core.Primitives;

namespace Core.Extensions
{
    public static class ArrayExtensions
    {
        // FLAT
        public static void Fill<T>(this T[] arr, T value) {
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        // MATRICES
        public static T[] Flat<T>(this T[,] arr)
        {
            var array = new T[arr.Length];
            var index = 0;
            for (var i = 0; i < arr.Rows(); i++)
            {
                for (var j = 0; j < arr.Columns(); j++)
                {
                    array[index] = arr[i, j];
                    index++;
                }
            }

            return array;
        }

        public static void Fill<T>(this T[,] arr, T value) {
            for (var i = 0; i < arr.GetLength(0); i++)
            {
                for (var j = 0; j < arr.GetLength(1); j++)
                {
                    arr[i, j] = value;
                }
            }
        }

        public static bool In<T>(this T[,] arr, int row, int column)
        {
            if (row < 0 || column < 0) return false;
            if (row >= arr.GetLength(0)) return false;
            if (column >= arr.GetLength(1)) return false;

            return true;
        }

        public static bool In<T>(this T[,] arr, Coords coords)
        {
            if (coords.Row < 0 || coords.Column < 0) return false;
            if (coords.Row >= arr.GetLength(0)) return false;
            if (coords.Column >= arr.GetLength(1)) return false;

            return true;
        }
        
        

        public static T[,] CopyPart<T>(this T[,] arr, Coords center, sbyte step)
        {
            T[,] copy = new T[step, step];

            for (sbyte row = (sbyte)-step; row <= step; row++)
            {
                for (var column = -step; column <= step; column++)
                {
                    if (arr.In(new Coords((sbyte)(center.Row + row), (sbyte)(center.Column + column))))
                    {
                        copy[center.Row + row, center.Column + column] = arr[center.Row + row, center.Column + column];
                    }
                }
            }
            return copy;
        }
        
        public static int Rows<T>(this T[,] arr)
        {
            return arr.GetLength(0);
        }

        public static int Columns<T>(this T[,] arr)
        {
            return arr.GetLength(1);
        }

        public static bool DifferBySize<TFirst, TSecond>(this TFirst[,] first, TSecond[,] second)
        {
            return first.Rows() != second.Rows() || 
                   first.Columns() != second.Columns();
        }

        // PRIVATE CASES COORDS MATRIX
        public static Coords[,] ToRect(this Coords[] arr)
        {
            var min = Coords.MAX;
            var max = Coords.MIN;

            foreach (var coords in arr)
            {
                if (coords.Row < min.Row) min.Row = coords.Row;
                if (coords.Row > max.Row) max.Row = coords.Row;

                if (coords.Column < min.Column) min.Column = coords.Column;
                if (coords.Column > max.Column) max.Column = coords.Column;
            }

            var rect = new Coords[max.Row - min.Row + 1, max.Column - min.Column + 1];
            rect.Fill(Coords.MIN);

            foreach (var coords in arr)
            {
                rect[coords.Row - min.Row, coords.Column - min.Column] = coords;
            }

            return rect;
        }

        //PRIVATE CASE ECS ENTITY GET BY COORDS
        public static ref T GetByCoords<T>(this T[,] arr, Coords coords)
        {
            if (!arr.In(coords.Row, coords.Column))
            {
                throw new ArgumentOutOfRangeException($"Try to get element by {coords}");
            }

            return ref arr[coords.Row, coords.Column];
        }

        public static int ElementsCount<T>(this T[,] array)
        {
            return array.Rows() * array.Columns();
        }
    }
}