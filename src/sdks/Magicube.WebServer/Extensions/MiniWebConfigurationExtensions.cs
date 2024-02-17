using Magicube.WebServer.Internal;
namespace Magicube.WebServer {
    public static class MiniWebConfigurationExtensions {
        public static bool WriteErrorToEventLog(this MiniWebConfiguration configuration, string errorMessage) {
            return EventLogHelper.UploadEventLog(configuration.ApplicationName, errorMessage);
        }

        public static MiniWebConfiguration AddRedirect(this MiniWebConfiguration configuration, string urlPath, string targetUrlPath, EventHandler eventHandler = null) {
            configuration.AddFunc(urlPath, context => {
                context.Response.Redirect(targetUrlPath);
                return (object)null;
            }, eventHandler);

            return configuration;
        }
    }
}
