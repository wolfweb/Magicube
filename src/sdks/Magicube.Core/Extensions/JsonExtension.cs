using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Magicube.Core {
#if !DEBUG
	using System.Diagnostics;
	[DebuggerStepThrough]
#endif
	public static class JsonExtension {
        public static IEnumerable<T> GetArray<T>(this JObject data, string path) {
            var paths = path.Split('.');
            var curObject = data;
            for (int i = 0; i < paths.Length; i++) {
                var pos = paths[i];
                var node = curObject.GetValue(pos);
                if (node == null) break;
                if (i == paths.Length - 1) {
                    return node.ToObject<JArray>().Select(x => x.ToObject<T>());
                }
                curObject = node.ToObject<JObject>();
            }
            return Enumerable.Empty<T>();
        }

        public static T GetValue<T>(this JObject data, string path) {
            var paths = path.Split('.');
            var curObject = data;
            for (int i = 0; i < paths.Length; i++) {
                var pos = paths[i];
                var node = curObject.GetValue(pos);
                if (node == null) break;
                if (i == paths.Length - 1) {
                    return node.ToObject<T>();
                }
                curObject = node.ToObject<JObject>();
            }
            return default(T);
        }

        public static List<JObjectCarrier> GetsByKey(this JArray data, Func<string, bool> predicate) {
			var result = new List<JObjectCarrier>();

			Gets(result, data, predicate, SearchType.Key);
			return result;
		}

		public static List<JObjectCarrier> GetsByValue(this JArray data, Func<string, bool> predicate) {
			var result = new List<JObjectCarrier>();

			Gets(result, data, predicate, SearchType.Value);
			return result;
		}

		public static List<JObjectCarrier> GetsByKey(this JObject data, Func<string, bool> predicate) {
			var result = new List<JObjectCarrier>();

			Gets(result, data, predicate, SearchType.Key);
			return result;
		}

		public static List<JObjectCarrier> GetsByValue(this JObject data, Func<string, bool> predicate) {
			var result = new List<JObjectCarrier>();

			Gets(result, data, predicate, SearchType.Value);
			return result;
		}

		private static void Gets(List<JObjectCarrier> result, JObject data, Func<string, bool> predicate, SearchType searchType) {
			foreach (var item in data) {
				if (searchType == SearchType.Key) {
					if (predicate(item.Key)) {
						if (item.Value.GetType().Equals(typeof(JObject))) {
							result.Add(new JObjectCarrier { Carrier = (JObject)item.Value });
						} else if (item.Value.GetType().Equals(typeof(JArray))) {
							result.Add(new JObjectCarrier { Array = (JArray)item.Value });
						} else {
							result.Add(new JObjectCarrier { Value = item.Value });
						}
					}
				}

				if (item.Value.GetType() == typeof(JObject)) {
					Gets(result, (JObject)item.Value, predicate, searchType);
				} else if (item.Value.GetType() == typeof(JArray)) {
					Gets(result, (JArray)item.Value, predicate, searchType);
				} else if (item.Value.GetType() == typeof(JValue)) {
					if (predicate(item.Value.ToString())) {
						var it = result.Find(x => x.Carrier == data);
						if (it == null) {
							it = new JObjectCarrier { Carrier = data };
							result.Add(it);
						}
						it.Fields.Add(item.Key);
					}
				} else {
					//
				}
			}
		}

		private static void Gets(List<JObjectCarrier> links, JArray data, Func<string, bool> predicate, SearchType searchType) {
			foreach (var item in data) {
				if (item.GetType() == typeof(JObject)) {
					Gets(links, (JObject)item, predicate, searchType);
				} else if (item.GetType() == typeof(JArray)) {
					Gets(links, (JArray)item, predicate, searchType);
				} else if (item.GetType() == typeof(JValue) && searchType == SearchType.Value) {
					if (item.Parent.GetType() == typeof(JArray)) {
						if (predicate(item.ToString())) {
							var path = item.Parent.Parent.Path;
							var pathArr = path.Split(".".ToCharArray());
							var it = links.Find(x => x.ArrayPath == path);
							if (it == null) {
								var carrier = new JObjectCarrier() {
									Array      = item.Parent.ToObject<JArray>(),
									ArrayPath  = path,
									ArrayField = pathArr[pathArr.Length - 1],
									Carrier    = (JObject)item.Parent.Parent.Parent
								};
								links.Add(carrier);
							}
						}
					}
				} else {
					Trace.WriteLine($"unsupport json type {item.GetType().Name}");
				}
			}
		}

		enum SearchType {
			Key,
			Value
        }
	}
	public class JObjectCarrier {
		public JObjectCarrier() {
			Fields = new List<string>();
		}
		public JObject      Carrier    { get; set; }
		public List<string> Fields     { get; set; }
		public string       ArrayPath  { get; set; }
		public string       ArrayField { get; set; }
		public JArray       Array      { get; set; }
        public object       Value      { get; set; }
	}
}
