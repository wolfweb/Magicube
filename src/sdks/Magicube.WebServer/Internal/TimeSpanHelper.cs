using System;

namespace Magicube.WebServer.Internal {
    public static class TimeSpanHelper {
        public static string GetFormattedTime(this TimeSpan timeSpan) {
            string elapsedTime;

            if (timeSpan.Days > 0)
                elapsedTime = string.Format("{0:%d} d {0:%h} hrs {0:%m} min {0:%s} sec {0:%fff} ms", timeSpan);
            else if (timeSpan.Hours > 0)
                elapsedTime = string.Format("{0:%h} hrs {0:%m} min {0:%s} sec {0:%fff} ms", timeSpan);
            else if (timeSpan.Minutes > 0)
                elapsedTime = string.Format("{0:%m} min {0:%s} sec {0:%fff} ms", timeSpan);
            else if (timeSpan.Seconds > 0)
                elapsedTime = string.Format("{0:%s} sec {0:%fff} ms", timeSpan);
            else if (timeSpan.Milliseconds > 0)
                elapsedTime = string.Format("{0:%fff} ms", timeSpan);
            else
                elapsedTime = string.Format("{0} µs", timeSpan.TotalMilliseconds * 1000.0);

            return elapsedTime;
        }
    }
}
