namespace Magicube.WebServer {
    using Magicube.WebServer.Internal;

    public static class CorsHelper {
        public static void EnableCors(this MiniWebConfiguration configuration, string allowedOrigin = "*", bool allowCredentials = true) {
            configuration.GlobalEventHandler.EnableCors(allowedOrigin, allowCredentials);
        }

        public static void EnableCors(this EventHandler eventHandler, string allowedOrigin = "*", bool allowCredentials = true) {
            eventHandler.PreInvokeHandlers.Add(context => {
                if (allowCredentials) {
                    context.Response.HeaderParameters.Add("Access-Control-Allow-Credentials", "true");

                    if (allowedOrigin == "*")
                        allowedOrigin = context.Request.Url.SiteBase;
                }

                context.Response.HeaderParameters.Add("Access-Control-Allow-Origin", allowedOrigin);

                if (context.Request.HttpMethod == "OPTIONS") {
                    context.Response.HeaderParameters.Add("Access-Control-Allow-Methods", "GET, PUT, POST, DELETE, HEAD, OPTIONS");
                    context.Response.HeaderParameters.Add("Access-Control-Max-Age", "86400"); 
                    context.Response.HttpStatusCode = Constants.HttpStatusCode.NoContent.ToInt();
                    context.Response.ContentType = "text/plain";
                    context.Handled = true;
                }
            });
        }
    }

}
