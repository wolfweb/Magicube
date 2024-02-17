using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Magicube.Core.PerformanceMonitor {
    public interface IStopwatch {
        long ElapsedMilliseconds { get; }
        void Stop();
    }
    public class PerformanceMonitorFactory {
        private readonly IPerformanceMonitor _performanceMonitor;
        public PerformanceMonitorFactory(IPerformanceMonitor performanceMonitor) {
            _performanceMonitor = performanceMonitor;
        }
        public IDisposable Start(Expression<Action> expression) {
            var stopwatch = StopwatchWrapper.StartNew();
            expression.Compile().Invoke();
            return new MonitorProfile<object>(BuildActionName(expression.Body), stopwatch, _performanceMonitor);
        }

        public async Task<IMonitorProfile<T>> Start<T>(Expression<Func<Task<T>>> expression) where T : class{
            var _stopwatch = StopwatchWrapper.StartNew();
            var result = await expression.Compile().Invoke();
            return new MonitorProfile<T>(BuildActionName(expression.Body), _stopwatch, _performanceMonitor) { 
                Result = result
            };
        }

        private string BuildActionName(Expression expression) {
            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression == null) throw new ArgumentNullException(nameof(expression));
            return $"{methodCallExpression.Method.DeclaringType.FullName}.{methodCallExpression.Method.Name}";
        }

        sealed class StopwatchWrapper : IStopwatch {
            private readonly Stopwatch _stopwatch;
            public static StopwatchWrapper StartNew() => new StopwatchWrapper();

            private StopwatchWrapper() {
                _stopwatch = Stopwatch.StartNew();
            }

            public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

            public bool IsRunning => _stopwatch.IsRunning;
            public void Stop()    => _stopwatch.Stop();
        }
    }
}
