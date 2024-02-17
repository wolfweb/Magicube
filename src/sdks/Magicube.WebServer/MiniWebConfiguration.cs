using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Magicube.WebServer.Metadata;
using Magicube.WebServer.RequestHandlers;
using Magicube.WebServer.Serialization;

namespace Magicube.WebServer {
    public class MiniWebConfiguration {
        public string ApplicationRootFolderPath;

        public EventHandler DefaultEventHandler = new EventHandler();
        public MethodRequestHandlerMetadataProvider DefaultMethodRequestHandlerMetadataProvider = new MethodRequestHandlerMetadataProvider();
        public BackgroundTaskEventHandler DefaultBackgroundTaskEventHandler = new BackgroundTaskEventHandler();
        public EventHandler GlobalEventHandler = new EventHandler();
        public dynamic Host;
        public IList<IRequestHandler> RequestHandlers = new List<IRequestHandler>();
        public IRequestHandler UnhandledRequestHandler;
        public IList<BackgroundTask> BackgroundTasks = new List<BackgroundTask>();
        public ISerializationService SerializationService = new JsonNetSerializer();
        public Func<string, string> GetDefaultMethodUrlPath = path => "/api/" + path;
        public bool LogErrorsToEventLog = true;
        public bool EnableVerboseErrors = false;
        public string ApplicationName = AppDomain.CurrentDomain.FriendlyName;
        public DateTime ApplicationStartDateTime = DateTime.Now;
        public int RequestParameterLimit = 1000;
        public List<string> MetadataNamespaces = new List<string>();

        public MiniWebConfiguration() {
            RequestHandlers.Add(new MetadataRequestHandler("/metadata/GetMetadata", DefaultEventHandler));
            this.EnableCorrelationId();
            this.DisableKeepAlive();
            this.EnableElapsedMillisecondsResponseHeader();
            this.EnableRequestCounter();
            this.EnableErrorCounter();
        }

        public IList<MethodRequestHandler> AddMethods(Type type, string urlPath = null, EventHandler eventHandler = null, IApiMetaDataProvider metadataProvider = null) {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);

            string path = type.Name;

            if (string.IsNullOrWhiteSpace(urlPath)) {
                while (type.DeclaringType != null) {
                    path = type.DeclaringType.Name + "/" + path;
                    type = type.DeclaringType;
                }
            }

            urlPath = string.IsNullOrWhiteSpace(urlPath) == false ? urlPath.TrimStart('/').TrimEnd('/') : GetDefaultMethodUrlPath(path).TrimStart('/').TrimEnd('/');

            IList<MethodRequestHandler> handlers = new List<MethodRequestHandler>();

            foreach (MethodInfo methodInfo in methods) {
                string caseSensitiveUrlPath = string.Format("/{0}/{1}", urlPath, methodInfo.Name);
                string methodUrlPath = caseSensitiveUrlPath.ToLower();
                var requestHandler = new MethodRequestHandler(methodUrlPath, eventHandler ?? DefaultEventHandler, methodInfo) { MetadataProvider = metadataProvider ?? DefaultMethodRequestHandlerMetadataProvider, CaseSensitiveUrlPath = caseSensitiveUrlPath };
                RequestHandlers.Add(requestHandler);
                handlers.Add(requestHandler);
            }

            return handlers;
        }

        public IList<MethodRequestHandler> AddMethods<T>(string urlPath = null, EventHandler eventHandler = null, IApiMetaDataProvider metadataProvider = null) {
            return AddMethods(typeof(T), urlPath, eventHandler, metadataProvider);
        }

        public FuncRequestHandler<T> AddFunc<T>(string urlPath, Func<MiniWebContext, T> func, EventHandler eventHandler = null, IApiMetaDataProvider metadataProvider = null) {
            urlPath = "/" + urlPath.TrimStart('/').TrimEnd('/');
            var handler = new FuncRequestHandler<T>(urlPath, eventHandler ?? DefaultEventHandler, func);
            RequestHandlers.Add(handler);
            return handler;
        }

        public FileRequestHandler AddFile(string urlPath, string filePath, EventHandler eventHandler = null) {
            var handler = new FileRequestHandler(urlPath, eventHandler ?? DefaultEventHandler, filePath);
            RequestHandlers.Add(handler);
            return handler;
        }

        public DirectoryRequestHandler AddDirectory(string urlPath, string directoryPath, EventHandler eventHandler = null, bool returnHttp404WhenFileWasNotFound = false, IList<string> defaultDocuments = null) {
            var handler = new DirectoryRequestHandler(urlPath, eventHandler ?? DefaultEventHandler, directoryPath, returnHttp404WhenFileWasNotFound, defaultDocuments);
            RequestHandlers.Add(handler);
            return handler;
        }

        public BackgroundTask AddBackgroundTask(string taskName, int millisecondInterval, Func<object> task, bool allowOverlappingRuns = false, BackgroundTaskEventHandler backgroundTaskEventHandler = null) {
            var backgroundTask = new BackgroundTask { Name = taskName, MillisecondInterval = millisecondInterval, Task = task, AllowOverlappingRuns = allowOverlappingRuns, BackgroundTaskEventHandler = backgroundTaskEventHandler ?? DefaultBackgroundTaskEventHandler };
            BackgroundTasks.Add(backgroundTask);
            return backgroundTask;
        }

		public override string ToString() {
            var builder = new StringBuilder();
            builder.AppendLine("[MiniWeb Configuration]");

            foreach (IRequestHandler handler in RequestHandlers)
                builder.AppendFormat("Handler [{0}] -> Path: {1}{2}", handler.GetType().Name, handler.UrlPath, Environment.NewLine);

            return builder.ToString();
        }
    }

}
