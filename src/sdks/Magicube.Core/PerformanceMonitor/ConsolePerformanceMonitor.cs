using Magicube.Core.IO;
using System.Diagnostics;

namespace Magicube.Core.PerformanceMonitor {
    public class ConsolePerformanceMonitor : IPerformanceMonitor {
        public void Deal(IMonitorProfile monitorProfile) {
            var msg = $"execute {monitorProfile.MethodName} used : {monitorProfile.ElapsedTicks} ms";
            Trace.WriteLine(msg);
            ConsoleUtil.Info(msg);
        }
    }
}
