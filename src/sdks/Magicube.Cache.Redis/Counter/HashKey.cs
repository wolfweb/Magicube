using StackExchange.Redis;
using System;
using System.Text.RegularExpressions;

namespace Magicube.Cache.Redis.Counter {
    public class HashKey : IEquatable<HashKey> {
        public const string StringPattern = "^(?<n>\\w+)-(?<d>\\S+)$";

        public string Name { get; private set; }
        public string Id   { get; private set; }

        public HashKey(string name, string id) {
            Name    = name;
            Id      = id;
        }

        public override string ToString() {
            return string.Format("{0}-{1}", Name, Id);
        }

        public bool Equals(HashKey other) {
            return other != null && ((Name == null && other.Name == null) || Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                && ((Id == null && other.Id == null) || Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object other) {
            return other != null && other is HashKey && Equals((HashKey)other);
        }

        public override int GetHashCode() {
            return (Name != null ? Name.GetHashCode() : 0) ^ Id.GetHashCode();
        }

        public static HashKey Parse(string path) {
            HashKey value;
            if (!TryParse(path, out value)) {
                throw new ArgumentOutOfRangeException("path");
            }
            return value;
        }

        public static bool TryParse(string path, out HashKey value) {
            value = null;
            if (string.IsNullOrWhiteSpace(path)) {
                return false;
            }

            var match = Regex.Match(path, StringPattern);
            if (!match.Success) {
                return false;
            }

            value = new HashKey(
                match.Groups["n"].Value,
                match.Groups["d"].Value);
            return true;
        }

        public static bool operator ==(HashKey v1, HashKey v2) {
            return Object.Equals(v1, v2);
        }

        public static bool operator !=(HashKey v1, HashKey v2) {
            return !(Object.Equals(v1, v2));
        }

        public static implicit operator string(HashKey value) {
            if (value == null) {
                throw new ArgumentNullException();
            }

            return value.ToString();
        }

        public static implicit operator RedisKey(HashKey value) {
            return (RedisKey)value.ToString();
        }
    }
}
