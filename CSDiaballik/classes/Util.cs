using System;
using System.Collections.Generic;

namespace CSDiaballik {
    public static class Util {

        public static List<T> ToList<T>(this ValueTuple<T, T> tuple) => new List<T> {tuple.Item1, tuple.Item2};


        public static ValueTuple<B, B> Select<A, B>(this ValueTuple<A, A> tuple, Func<A, B> f)
            => (f(tuple.Item1), f(tuple.Item2));


        public static void Foreach<A>(this ValueTuple<A, A> tuple, Action<A> f) {
            f(tuple.Item1);
            f(tuple.Item2);
        }


        public static (C, C) Merge<A, B, C>(this (A, A) t1, (B, B) t2, Func<A, B, C> f)
            => (f(t1.Item1, t2.Item1), f(t1.Item2, t2.Item2));

    }
}
