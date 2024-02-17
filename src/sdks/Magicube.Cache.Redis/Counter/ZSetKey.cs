using StackExchange.Redis;
using System;
using System.Text.RegularExpressions;

namespace Magicube.Cache.Redis.Counter {
    public class ZSetKey : IEquatable<ZSetKey> {
        private const string StringPattern = "^(?<n>\\w+)(?:(?:\\.)(?<r>\\w+))?:(?<a>\\w+)-(?:-(?<d>\\S+))$";

        public string   Name     { get; private set; }
        public string   Resource { get; private set; }
        public string   Action   { get; private set; }
        public DateTime Date     { get; private set; }

        public ZSetKey(ZSetKey zsetKey) : this(zsetKey.Name, zsetKey.Resource, zsetKey.Action, zsetKey.Date) {
        }

        public ZSetKey(string name, string action, DateTime date) : this(name, null, action, date) {
        }

        public ZSetKey(string name, string resource, string action, DateTime date) {
            Name     = name;
            Resource = resource;
            Action   = action;
            Date     = date;
        }

        public override string ToString() {
            if (string.IsNullOrWhiteSpace(Resource)) {
                return string.Format("{0}:{1}-{2}", Name, Action, Date.DayOfYear);
            }
            return string.Format("{0}.{1}:{2}-{3}", Name, Resource, Action, Date.DayOfYear);
        }

        public string ToPath() {
            if (string.IsNullOrWhiteSpace(Resource)) {
                return string.Format("{0}:{1}", Name, Action);
            }
            return string.Format("{0}.{1}:{2}", Name, Resource, Action);
        }

        public bool Equals(ZSetKey other) {
            return other != null && ((Name == null && other.Name == null) || Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                && ((Action == null && other.Action == null) || Action.Equals(other.Action, StringComparison.OrdinalIgnoreCase))
                && Date.Date.Equals(other.Date.Date);
        }

        public override bool Equals(object other) {
            return other != null && other is ZSetKey && Equals((ZSetKey)other);
        }

        public override int GetHashCode() {
            var hash = (Name != null ? Name.GetHashCode() : 0)
                ^ (Action != null ? Action.GetHashCode() : 0)
                ^ Date.GetHashCode();

            if (!string.IsNullOrWhiteSpace(Resource)) {
                hash = hash ^ Resource.GetHashCode();
            }
            return hash;
        }

        public static bool operator ==(ZSetKey v1, ZSetKey v2) {
            return Object.Equals(v1, v2);
        }

        public static bool operator !=(ZSetKey v1, ZSetKey v2) {
            return !(Object.Equals(v1, v2));
        }

        public static implicit operator string(ZSetKey value) {
            if (value == null) {
                throw new ArgumentNullException();
            }
            return value.ToString();
        }

        public static implicit operator RedisKey(ZSetKey value) {
            return (RedisKey)value.ToString();
        }

        public static bool TryParse(string path, out ZSetKey key) {
            key = null;
            if (string.IsNullOrWhiteSpace(path)) {
                return false;
            }

            var match = Regex.Match(path, StringPattern);
            if (!match.Success) {
                return false;
            }

            var date = DateTime.UtcNow.Date;
            var day = int.Parse(match.Groups["d"].Value);
            date = new DateTime(DateTime.UtcNow.Year, 1, 1).AddDays(day - 1);

            key = new ZSetKey(
             match.Groups["n"].Value,
             match.Groups["r"].Success ? match.Groups["r"].Value : null,
             match.Groups["a"].Value,
             date);
            return true;
        }

        public static ZSetKey Parse(string path) {
            ZSetKey key;
            if (!TryParse(path, out key)) {
                throw new ArgumentOutOfRangeException();
            }
            return key;
        }
    }
}
