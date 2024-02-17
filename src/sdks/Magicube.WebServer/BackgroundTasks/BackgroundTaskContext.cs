using System;

namespace Magicube.WebServer {
    public class BackgroundTaskContext {
        public object               TaskResult;
        public DateTime             EndDateTime;
        public DateTime             StartDateTime;
        public Exception            TaskException;
        public BackgroundTask       BackgroundTask;
        public MiniWebConfiguration Configuration;
    }
}
