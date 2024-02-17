using System;
using System.Threading.Tasks;
using System.Threading;

namespace Magicube.WebServer {
    public static class BackgroundTaskRunner {
        public static void Start(MiniWebConfiguration configuration) {
            foreach (BackgroundTask backgroundTask in configuration.BackgroundTasks) {
                BackgroundTask task = backgroundTask;

                Task.Run(() => {
                    while (true) {
                        var backgroundTaskContext = new BackgroundTaskContext { BackgroundTask = task, Configuration = configuration };

                        try {
                            if (backgroundTaskContext.BackgroundTask.AllowOverlappingRuns)
                                Task.Run(() => Run(backgroundTaskContext)); 
                            else
                                Run(backgroundTaskContext); 
                        } catch (Exception) { }

                        Thread.Sleep(backgroundTaskContext.BackgroundTask.MillisecondInterval);
                    } 
                });
            }
        }

        public static void Run(BackgroundTaskContext backgroundTaskContext) {
            try {
                backgroundTaskContext.StartDateTime = DateTime.Now;
                backgroundTaskContext.BackgroundTask.BackgroundTaskRunHistory[backgroundTaskContext.BackgroundTask.CurrentBackgroundTaskHistoryPosition] = backgroundTaskContext;
                backgroundTaskContext.BackgroundTask.CurrentBackgroundTaskHistoryPosition = (backgroundTaskContext.BackgroundTask.CurrentBackgroundTaskHistoryPosition + 1) % backgroundTaskContext.BackgroundTask.BackgroundTaskRunHistory.Length;
                backgroundTaskContext.BackgroundTask.BackgroundTaskEventHandler.InvokePreInvokeHandlers(backgroundTaskContext);
                backgroundTaskContext.TaskResult = backgroundTaskContext.BackgroundTask.Task.Invoke();
            } catch (Exception e) {
                backgroundTaskContext.TaskException = e;
                backgroundTaskContext.BackgroundTask.BackgroundTaskEventHandler.InvokeUnhandledExceptionHandlers(e, backgroundTaskContext);
            } finally {
                backgroundTaskContext.BackgroundTask.BackgroundTaskEventHandler.InvokePostInvokeHandlers(backgroundTaskContext);
                backgroundTaskContext.EndDateTime = DateTime.Now;
            }
        }
    }

}
