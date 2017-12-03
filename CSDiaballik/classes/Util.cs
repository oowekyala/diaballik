using System;
using System.Collections.Generic;

namespace CSDiaballik
{
    public static class Util
    {
        public static List<T> ToList<T>(this ValueTuple<T, T> tuple)
        {
            return new List<T> {tuple.Item1, tuple.Item2};
        }


        public static ValueTuple<B, B> Map<A, B>(this ValueTuple<A, A> tuple, Func<A, B> f)
        {
            return (f(tuple.Item1), f(tuple.Item2));
        }
    }
}
