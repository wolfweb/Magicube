using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magicube.Core {
#if !DEBUG
using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class EnumerableExtensions {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> func) {
            foreach (T item in source) {
                func(item);
            }
            return source;
        }

        public static Task ForEach<T>(this IEnumerable<T> source, Func<T, Task> body) {
            var partitionCount = System.Environment.ProcessorCount;

            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(partitionCount)
                select Task.Run(async delegate {
                    using (partition) {
                        while (partition.MoveNext()) {
                            await body(partition.Current);
                        }
                    }
                }));
        }
    }
}
