using System;
using System.Collections.Generic;

namespace CSDiaballik {
    public static class ExtensionUtil {

        // Mainly extension methods for homogeneous pairs. Avoids repeating symmetrical code over and over.


        public static List<T> ToList<T>(this (T, T) tuple) => new List<T> {tuple.Item1, tuple.Item2};


        public static (B, B) Map<A, B>(this (A, A) tuple, Func<A, B> f) => (f(tuple.Item1), f(tuple.Item2));


        public static (B, B) Map<A, B, C>(this (A, A) tuple, C z, Func<A, C, B> f)
            => (f(tuple.Item1, z), f(tuple.Item2, z));


        public static (A, A) Pair<A>(Func<A> a) => (a.Invoke(), a.Invoke());


        public static void Foreach<A>(this(A, A) tuple, Action<A> f) {
            f(tuple.Item1);
            f(tuple.Item2);
        }


        public static (C, C) Zip<A, B, C>(this (A, A) t1, (B, B) t2, Func<A, B, C> f)
            => (f(t1.Item1, t2.Item1), f(t1.Item2, t2.Item2));


        public static ((A, B), (A, B)) Zip<A, B>(this (A, A) t1, (B, B) t2)
            => t1.Zip(t2, (a, b) => (a, b));

    }
}
