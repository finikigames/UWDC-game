using System;
using System.Collections.Generic;
using Core.Primitives;

namespace Core.Extensions
{
    public static class ListExtensions
    {
        public static Coords[,] ToRect(this List<Coords> arr)
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
    }
}