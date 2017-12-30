using System;
using System.Collections.Generic;
using System.Linq;

namespace Diaballik.Core.Util {
    public static class ExtensionUtil {
        // Mainly extension methods for homogeneous pairs. Avoids repeating symmetrical code over and over.


        public static List<T> ToList<T>(this (T, T) tuple) => new List<T> {tuple.Item1, tuple.Item2};

        public static (A, A) ToTuple<A>(this List<A> l) => (l[0], l[1]);

        public static (A, A) ToTuple<A>(this IEnumerable<A> l) {
            var it = l.GetEnumerator();
            it.MoveNext();
            var a0 = it.Current;
            it.MoveNext();
            var a1 = it.Current;
            it.Dispose();
            return (a0, a1);
        }

        public static (B, B) Map<A, B>(this (A, A) tuple, Func<A, B> f) => (f(tuple.Item1), f(tuple.Item2));

        public static (B, B) ZipWithPair<A, B, C>(this (A, A) tuple, C z, Func<A, C, B> f)
            => tuple.Zip(Pair(z), f);


        public static (A, A) Pair<A>(A a) => (a, a);


        public static (A, A) Foreach<A>(this(A, A) tuple, Action<A> f) {
            f(tuple.Item1);
            f(tuple.Item2);
            return tuple;
        }


        public static bool Forall<A>(this(A, A) tuple, Func<A, bool> pred)
            => pred(tuple.Item1) && pred(tuple.Item2);


        public static (C, C) Zip<A, B, C>(this (A, A) t1, (B, B) t2, Func<A, B, C> f)
            => (f(t1.Item1, t2.Item1), f(t1.Item2, t2.Item2));


        public static ((A, B), (A, B)) Zip<A, B>(this (A, A) t1, (B, B) t2)
            => t1.Zip(t2, (a, b) => (a, b));


        public static IEnumerable<(A, int)> ZipWithIndex<A>(this IEnumerable<A> l) => ZipWithIndex(l, (a, i) => (a, i));


        public static IEnumerable<B> ZipWithIndex<A, B>(this IEnumerable<A> l, Func<A, int, B> f) {
            var i = 0;
            foreach (var a in l) {
                yield return f(a, i++);
            }
        }

        public static IEnumerable<A> SortAndUnzip<A>(this IEnumerable<(A, int)> l) {
            return l.OrderBy(t => t.Item2).Select(t => t.Item1);
        }


        private static readonly Random Rng = new Random();

        public static IEnumerable<T> Shuffle<T>(this IList<T> list) {
            for (var i = 0; i < list.Count; i++) {
                var j = Rng.Next(i, list.Count);
                yield return list[j];

                list[j] = list[i];
            }
        }
    }
}