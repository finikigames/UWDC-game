using System.Collections.Generic;

namespace Checkers.Structs
{
    public static class LinkedListExtensions
    {
        public static void AppendRange<T>(this LinkedList<T> linkedList, IEnumerable<T> elementsToAdd)
        {
            foreach (T element in elementsToAdd)
                linkedList.AddLast(element);
        }
    }
}