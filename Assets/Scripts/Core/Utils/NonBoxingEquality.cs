using System.Collections.Generic;

namespace Core.Utils
{
    public static class NonBoxingEquality
    {
        public static bool NonBoxingGenericEqual<T>(this T value, T compare)
        {
            return EqualityComparer<T>.Default.Equals(value, compare);
        }
    }
}