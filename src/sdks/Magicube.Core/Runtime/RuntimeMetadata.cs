using Magicube.Core.Runtime.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Magicube.Core.Runtime {
    public class RuntimeMetadata {
		private readonly ConcurrentDictionary<string, MethodMetadata> _methodsCache = new ConcurrentDictionary<string, MethodMetadata>();
		public Type DeclaredType { get; }
		public Type ImplType     { get; }
		public bool IsStatic     { get; }
		public IEnumerable<RuntimeMethodProvider> Methods => _methodsCache.Select(x => new RuntimeMethodProvider {
			Tag             = x.Value.Attribute.Tag,
			Title           = x.Key,
			Method          = x.Value.Method,
			IsStatic        = IsStatic,
			Descript        = x.Value.Attribute.Descript,
			Parameters      = x.Value.Parameters,
			ReturnType      = x.Value.Method.ReturnType,
			DeclaredType    = DeclaredType,
			ImplType        = ImplType,
			RuntimeProvider = this
		});

		public RuntimeMetadata(Type declaredType, Type implType, bool isStatic = false) {
			DeclaredType = declaredType;
			ImplType     = implType;
			IsStatic     = isStatic;
			var methods = isStatic ? declaredType.GetExtensionMethods() : declaredType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(x => x.GetCustomAttribute<RuntimeMethodAttribute>() != null);

			if (methods.Count() == 0 && implType != null) { 
				methods = implType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(x => x.GetCustomAttribute<RuntimeMethodAttribute>() != null);
			}

			foreach (var method in methods) {
				var methodAttr = method.GetCustomAttribute<RuntimeMethodAttribute>();

				if (methodAttr == null) continue;

				var meta = new MethodMetadata {
					Method    = method,
					Attribute = methodAttr,
					Parameters = method.GetParameters().Select(x => {
						var attr = x.GetCustomAttribute<ParameterDescriptorAttribute>();
						var p = new MethodParameterMetadata {
							Name            = x.Name,
							Title           = attr == null ? x.Name : attr.Title,
							Descript        = attr == null ? x.Name : attr.Descript,
							Position        = x.Position,
							ParameterType   = x.ParameterType,
							DefaultValue    = x.DefaultValue,
							HasDefaultValue = x.HasDefaultValue
						};
						return p;
					})
				};

				if (methodAttr.Title.IsNullOrEmpty() || _methodsCache.ContainsKey(methodAttr.Title)) {
					methodAttr.Title = $"{(implType != null ? $"{implType.Name}" : declaredType.Name.Replace("Extension", ""))}:{method.Name}";
				}
				_methodsCache.TryAdd(methodAttr.Title, meta);
			}
		}

		public object Invoke(string methodTitle, object instance, params object[] parameters) {
			var meta = _methodsCache[methodTitle];
			var args = meta.Parameters.Select((x, i) => parameters.Length > i ? parameters[i] : null).ToArray();
			return meta.Method.Invoke(instance, args);
		}

		private sealed class MethodMetadata {
			public MethodInfo                           Method     { get; set; }
			public RuntimeMethodAttribute               Attribute  { get; set; }
			public IEnumerable<MethodParameterMetadata> Parameters { get; set; }
		}
	}
}
