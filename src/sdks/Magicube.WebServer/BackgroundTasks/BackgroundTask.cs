using System;

namespace Magicube.WebServer {
    public class BackgroundTask {
        public string                     Name;
        public Func<object>               Task;
        public int                        MillisecondInterval;
        public bool                       AllowOverlappingRuns;
        public BackgroundTaskEventHandler BackgroundTaskEventHandler;
        public BackgroundTaskContext[]    BackgroundTaskRunHistory = new BackgroundTaskContext[10];
        public int                        CurrentBackgroundTaskHistoryPosition;
    }

}
