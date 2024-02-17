using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Core {
#if !DEBUG
using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class CollectionExtensions {
        static Random rng = new Random();
        public static ICollection<T> AddRange<T>(this ICollection<T> initial, IEnumerable<T> other) {
            if (other == null)
                return initial;

            var list = initial as List<T>;

            if (list != null) {
                list.AddRange(other);
                return initial;
            }
            
            other.ForEach(x => initial.Add(x));

            return initial;
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> source) {
            return (source == null || source.Count == 0);
        }

        public static bool AddIf<T>(this ICollection<T> source, Func<T, bool> predicate, T value) {
            if (predicate(value)) {
                source.Add(value);
                return true;
            }

            return false;
        }

        public static List<T> Shuffle<T>(this IEnumerable<T> datas) {
            var list = datas.ToList();
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }
    }
}
