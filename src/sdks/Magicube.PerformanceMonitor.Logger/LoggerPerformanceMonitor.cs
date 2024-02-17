using Magicube.Core.PerformanceMonitor;
using Microsoft.Extensions.Logging;

namespace Magicube.PerformanceMonitor.Logger {
    public class LoggerPerformanceMonitor : IPerformanceMonitor {
        private readonly ILogger _logger;
        public LoggerPerformanceMonitor(ILogger<LoggerPerformanceMonitor> logger) {
            _logger = logger;
        }
        public void Deal(IMonitorProfile monitorProfile) {
            _logger.LogDebug($"execute {monitorProfile.MethodName} used : {monitorProfile.ElapsedTicks} ms");
        }
    }
}
