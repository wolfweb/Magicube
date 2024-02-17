using Magicube.Core.Runtime.Attributes;
using System;

namespace Magicube.Core {
#if !DEBUG
using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class DateTimeExtension {
        [RuntimeMethod]
        public static long ToUnixTimeSeconds(this DateTime value) {
            var date = value;
            if (value.Kind == DateTimeKind.Local) {
                date = value.ToUniversalTime();
            }
            var ticks = date.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
            var ts = ticks / TimeSpan.TicksPerSecond;
            return ts;
        }

        [RuntimeMethod]
        public static DateTime ToDateTime(this long value) {
            var timeInTicks = value * TimeSpan.TicksPerSecond;
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(timeInTicks);
        }

        [RuntimeMethod]
        public static string ToFriendly(this DateTime value) {
            DateTime now = DateTime.Now;
            if (now < value) return value.ToString("yyyy/MM/dd");

            TimeSpan dep = now - value;
            if (dep.TotalMinutes < 10)
                return "刚刚";
            else if (dep.TotalMinutes >= 10 && dep.TotalMinutes < 60)
                return (int)dep.TotalMinutes + " 分钟前";
            else if (dep.TotalHours < 24)
                return (int)dep.TotalHours + " 小时前";
            else if (dep.TotalDays < 5)
                return (int)dep.TotalDays + " 天前";
            else
                return value.ToString("yyyy/MM/dd");
        }
    }
}
