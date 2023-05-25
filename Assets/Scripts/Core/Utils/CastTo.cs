using System;
using System.Linq.Expressions;

namespace Core.Utils
{
    /// <summary>
    /// Class to cast to type <see cref="TOut"/>
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <see cref="https://bit.ly/3CLwwgz"/>
    public static class CastTo<TOut>
    {
        /// <summary>
        /// Casts <see cref="TIn"/> to <see cref="TOut"/>.
        /// This does not cause boxing for value types.
        /// Useful in generic methods.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        public static TOut From<TIn>(TIn s)
        {
            return Cache<TIn>.Caster(s);
        }    

        private static class Cache<TIn>
        {
            public static readonly Func<TIn, TOut> Caster = Get();

            private static Func<TIn, TOut> Get()
            {
                var p = Expression.Parameter(typeof(TIn));
                var c = Expression.ConvertChecked(p, typeof(TOut));
                return Expression.Lambda<Func<TIn, TOut>>(c, p).Compile();
            }
        }
    }
}