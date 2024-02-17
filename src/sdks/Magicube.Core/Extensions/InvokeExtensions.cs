using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.Core {
#if !DEBUG
    using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class InvokeExtensions {
        public static void Invoke<TEvents>(this IEnumerable<TEvents> events, Action<TEvents> dispatch, ILogger logger) {
            foreach (var sink in events) {
                try {
                    dispatch(sink);
                } catch (Exception ex) {
                    HandleException(ex, logger, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }
        }

        public static void Invoke<TEvents, T1>(this IEnumerable<TEvents> events, Action<TEvents, T1> dispatch, T1 arg1, ILogger logger) {
            foreach (var sink in events) {
                try {
                    dispatch(sink, arg1);
                } catch (Exception ex) {
                    HandleException(ex, logger, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }
        }

        public static IEnumerable<TResult> Invoke<TEvents, TResult>(this IEnumerable<TEvents> events, Func<TEvents, TResult> dispatch, ILogger logger) {
            var results = new List<TResult>();

            foreach (var sink in events) {
                try {
                    var result = dispatch(sink);
                    results.Add(result);
                } catch (Exception ex) {
                    HandleException(ex, logger, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }

            return results;
        }

        public static IEnumerable<TResult> Invoke<TEvents, T1, TResult>(this IEnumerable<TEvents> events, Func<TEvents, T1, TResult> dispatch, T1 arg1, ILogger logger) {
            var results = new List<TResult>();

            foreach (var sink in events) {
                try {
                    var result = dispatch(sink, arg1);
                    results.Add(result);
                } catch (Exception ex) {
                    HandleException(ex, logger, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }

            return results;
        }

        public static IEnumerable<TResult> Invoke<TEvents, TResult>(this IEnumerable<TEvents> events, Func<TEvents, IEnumerable<TResult>> dispatch, ILogger logger) {
            var results = new List<TResult>();

            foreach (var sink in events) {
                try {
                    var result = dispatch(sink);
                    results.AddRange(result);
                } catch (Exception ex) {
                    HandleException(ex, logger, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }

            return results;
        }

        public static async Task InvokeAsync<TEvents>(this IEnumerable<TEvents> events, Func<TEvents, Task> dispatch, ILogger logger) {
            foreach (var sink in events) {
                try {
                    await dispatch(sink);
                } catch (Exception ex) {
                    HandleException(ex, logger, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }
        }

        public static async Task InvokeAsync<TEvents, T1>(this IEnumerable<TEvents> events, Func<TEvents, T1, Task> dispatch, T1 arg1, ILogger logger) {
            foreach (var sink in events) {
                try {
                    await dispatch(sink, arg1);
                } catch (Exception ex) {
                    HandleException(ex, logger, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }
        }

        public static async Task InvokeAsync<TEvents, T1, T2>(this IEnumerable<TEvents> events, Func<TEvents, T1, T2, Task> dispatch, T1 arg1, T2 arg2, ILogger logger) {
            foreach (var sink in events) {
                try {
                    await dispatch(sink, arg1, arg2);
                } catch (Exception ex) {
                    HandleException(ex, logger, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }
        }

        public static async Task<IEnumerable<TResult>> InvokeAsync<TEvents, TResult>(this IEnumerable<TEvents> events, Func<TEvents, Task<TResult>> dispatch, ILogger logger) {
            var results = new List<TResult>();

            foreach (var sink in events) {
                try {
                    var result = await dispatch(sink);
                    results.Add(result);
                } catch (Exception ex) {
                    HandleException(ex, logger, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }

            return results;
        }

        public static async Task<IEnumerable<TResult>> InvokeAsync<TEvents, T1, TResult>(this IEnumerable<TEvents> events, Func<TEvents, T1, Task<TResult>> dispatch, T1 arg1, ILogger logger) {
            var results = new List<TResult>();

            foreach (var sink in events) {
                try {
                    var result = await dispatch(sink, arg1);
                    results.Add(result);
                } catch (Exception ex) {
                    HandleException(ex, logger, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }

            return results;
        }

        public static async Task<IEnumerable<TResult>> InvokeAsync<TEvents, TResult>(this IEnumerable<TEvents> events, Func<TEvents, Task<IEnumerable<TResult>>> dispatch, ILogger logger) {
            var results = new List<TResult>();

            foreach (var sink in events) {
                try {
                    var result = await dispatch(sink);
                    results.AddRange(result);
                } catch (Exception ex) {
                    HandleException(ex, logger, typeof(TEvents).Name, sink.GetType().FullName);
                }
            }

            return results;
        }

        public static void HandleException(Exception ex, ILogger logger, string sourceType, string method) {
            if (IsLogged(ex)) {
                logger.LogError(ex, "{Type} thrown from {Method} by {Exception}",
                    sourceType,
                    method,
                    ex.GetType().Name);
            }

            if (ex.IsFatal()) {
                throw ex;
            }
        }
        private static bool IsLogged(Exception ex) {
            return !ex.IsFatal();
        }
    }
}
