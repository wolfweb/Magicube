using System;
using System.Collections.Generic;

namespace Magicube.WebServer.Internal {
    public static class FileExtensionToContentTypeConverter {
        static FileExtensionToContentTypeConverter() {
            Mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                    { ".atom", "application/atom+xml" },
                    { ".avi", "video/x-msvideo" },
                    { ".bmp", "image/bmp" },
                    { ".chm", "application/octet-stream" },
                    { ".css", "text/css" },
                    { ".csv", "application/octet-stream" },
                    { ".doc", "application/msword" },
                    { ".docm", "application/vnd.ms-word.document.macroEnabled.12" },
                    { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                    { ".gif", "image/gif" },
                    { ".htm", "text/html" },
                    { ".html", "text/html" },
                    { ".ical", "text/calendar" },
                    { ".icalendar", "text/calendar" },
                    { ".ico", "image/x-icon" },
                    { ".ics", "text/calendar" },
                    { ".jar", "application/java-archive" },
                    { ".java", "application/octet-stream" },
                    { ".jpe", "image/jpeg" },
                    { ".jpeg", "image/jpeg" },
                    { ".jpg", "image/jpeg" },
                    { ".js", "application/javascript" },
                    { ".jsx", "text/jscript" },
                    { ".m3u", "audio/x-mpegurl" },
                    { ".m4a", "audio/mp4" },
                    { ".m4v", "video/mp4" },
                    { ".map", "text/plain" },
                    { ".mdb", "application/x-msaccess" },
                    { ".mid", "audio/mid" },
                    { ".midi", "audio/mid" },
                    { ".mov", "video/quicktime" },
                    { ".mp2", "video/mpeg" },
                    { ".mp3", "audio/mpeg" },
                    { ".mp4", "video/mp4" },
                    { ".mp4v", "video/mp4" },
                    { ".mpa", "video/mpeg" },
                    { ".mpe", "video/mpeg" },
                    { ".mpeg", "video/mpeg" },
                    { ".mpg", "video/mpeg" },
                    { ".mpp", "application/vnd.ms-project" },
                    { ".mpv2", "video/mpeg" },
                    { ".oga", "audio/ogg" },
                    { ".ogg", "video/ogg" },
                    { ".ogv", "video/ogg" },
                    { ".otf", "font/otf" },
                    { ".pdf", "application/pdf" },
                    { ".png", "image/png" },
                    { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                    { ".ps", "application/postscript" },
                    { ".qt", "video/quicktime" },
                    { ".rtf", "application/rtf" },
                    { ".svg", "image/svg+xml" },
                    { ".swf", "application/x-shockwave-flash" },
                    { ".tar", "application/x-tar" },
                    { ".tgz", "application/x-compressed" },
                    { ".tif", "image/tiff" },
                    { ".tiff", "image/tiff" },
                    { ".txt", "text/plain" },
                    { ".wav", "audio/wav" },
                    { ".wm", "video/x-ms-wm" },
                    { ".wma", "audio/x-ms-wma" },
                    { ".wmv", "video/x-ms-wmv" },
                    { ".xaml", "application/xaml+xml" },
                    { ".xap", "application/x-silverlight-app" },
                    { ".xbap", "application/x-ms-xbap" },
                    { ".xbm", "image/x-xbitmap" },
                    { ".xdr", "text/plain" },
                    { ".xls", "application/vnd.ms-excel" },
                    { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                    { ".xml", "text/xml" },
                    { ".zip", "application/x-zip-compressed" },
                };
        }

        public static IDictionary<string, string> Mappings { get; set; }

        public static string GetContentType(string extension) {
            string contentType;
            Mappings.TryGetValue(extension, out contentType);
            return contentType;
        }
    }
}
