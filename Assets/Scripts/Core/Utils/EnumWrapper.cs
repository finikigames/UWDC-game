using System;
using System.Collections.Generic;

namespace Core.Utils
{
    /// <summary>
    /// Used for list of enums to use common list API such as Contains().
    /// Does not provide garbage because EqualityComparer doesn't cause boxing enum value on the heap
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <see cref="https://bit.ly/3KqJODq"/>
    public struct EnumWrapper<TEnum> : IEquatable<EnumWrapper<TEnum>> where TEnum : Enum
    {
        public TEnum EnumValue;
            
        public bool Equals(EnumWrapper<TEnum> other)
        {
            return EqualityComparer<TEnum>.Default.Equals(EnumValue, other.EnumValue);
        }

        public static EnumWrapper<TEnum> Wrap(TEnum enumValue) => new EnumWrapper<TEnum> {EnumValue = enumValue};
    }
}