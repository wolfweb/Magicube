using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Magicube.Data.Abstractions {
    public static class DataDefaultValueMapping {
		public static readonly IDictionary<Type, object> ValueMappings = new Dictionary<Type, object> {
			[typeof(int)]      = 0,
			[typeof(bool)]     = false,
			[typeof(Guid)]     = Guid.Empty,
			[typeof(long)]     = 0,
			[typeof(float)]    = 0f,
			[typeof(short)]    = 0,
			[typeof(double)]   = 0d,
			[typeof(decimal)]  = 0m,
			[typeof(string)]   = string.Empty,
			[typeof(DateTime)] = DateTime.UtcNow,
			[typeof(JObject)]  = default(JObject)
		};        
    }
}
