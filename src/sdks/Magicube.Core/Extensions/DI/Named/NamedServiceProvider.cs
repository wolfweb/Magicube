using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core {
    internal static class NamedServiceProvider {
		public static Type GenerateNamedServiceType<T>(string key) {
			var type = typeof(T);
			var namedType = NamedTypeBuilder.GetOrCreateNamedType($"{key}:{type.FullName}");
			return typeof(NamedService<,>).MakeGenericType(type, namedType);
		}

		public static Type GenerateNamedServiceType(string key, Type type) {
			var namedType = NamedTypeBuilder.GetOrCreateNamedType($"{key}:{type.FullName}");
			return typeof(NamedService<,>).MakeGenericType(type, namedType);
		}

		static class NamedTypeBuilder {
			private static string RootNamespace = "NamedType";

			private static readonly string AssemblyName = Guid.NewGuid().ToString();
			private static readonly AssemblyBuilder AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(AssemblyName), AssemblyBuilderAccess.Run);
			private static readonly ModuleBuilder   ModuleBuilder   = AssemblyBuilder.DefineDynamicModule("NamedTypeModule");

			private static readonly ConcurrentDictionary<string, Type> ExistingNamedTypes = new();

			private static readonly object dictLock = new();

			public static Type GetOrCreateNamedType(string key) {
				lock (dictLock) {
					return ExistingNamedTypes.GetOrAdd($"{RootNamespace}.{key}", CreateNamedType);
				}
			}

			private static Type CreateNamedType(string key) {
				var builder = ModuleBuilder.DefineType(key, TypeAttributes.Public | TypeAttributes.Sealed, typeof(ValueType));
				var objectTypeInfo = builder.CreateTypeInfo();
				return objectTypeInfo.AsType();
			}

			public static string GetFullName(Enum enumValue) {
				var enumType = enumValue.GetType();
				var enumStringValue = enumValue.ToString("F");

				return $"{enumType.FullName}.{enumStringValue}"; ;
			}
		}
	}
}
