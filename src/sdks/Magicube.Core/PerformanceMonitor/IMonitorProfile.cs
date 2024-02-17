using System;

namespace Magicube.Core.PerformanceMonitor {
    public interface IMonitorProfile : IDisposable {
        long ElapsedTicks { get; }
        string MethodName { get; }
    }
    public interface IMonitorProfile<T> : IMonitorProfile where T : class {        
        object Result     { get; set; }
    }

    sealed class MonitorProfile<T> : IMonitorProfile<T> where T : class {
        private readonly IStopwatch _stopwatch;
        private readonly IPerformanceMonitor _performanceMonitor;
        public MonitorProfile(string actionName, IStopwatch stopwatch, IPerformanceMonitor performanceMonitor) {
            MethodName          = actionName;
            _stopwatch          = stopwatch;
            _performanceMonitor = performanceMonitor;
        }

        public long   ElapsedTicks { get; private set; }
        public string MethodName   { get; private set; }
        public object Result       { get; set; }

        public void Dispose() {
            _stopwatch.Stop();
            ElapsedTicks = _stopwatch.ElapsedMilliseconds;
            _performanceMonitor.Deal(this);
        }
    }
}
