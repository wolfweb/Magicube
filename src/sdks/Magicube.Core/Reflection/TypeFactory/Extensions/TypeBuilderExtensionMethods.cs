using System.Reflection;

namespace Magicube.Core.Reflection {
    public static class TypeBuilderExtensionMethods {
        public static ITypeBuilder Public(this ITypeBuilder typeBuilder) {
            typeBuilder.TypeAttributes |= TypeAttributes.Public;
            return typeBuilder;
        }

        public static ITypeBuilder NotPublic(this ITypeBuilder typeBuilder) {
            typeBuilder.TypeAttributes |= TypeAttributes.NotPublic;
            return typeBuilder;
        }

        public static ITypeBuilder Class(this ITypeBuilder typeBuilder) {
            typeBuilder.TypeAttributes |= TypeAttributes.Class;
            return typeBuilder;
        }

        public static ITypeBuilder Sealed(this ITypeBuilder typeBuilder) {
            typeBuilder.TypeAttributes |= TypeAttributes.Sealed;
            return typeBuilder;
        }

        public static ITypeBuilder Interface(this ITypeBuilder typeBuilder) {
            typeBuilder.TypeAttributes |= TypeAttributes.Interface;
            return typeBuilder;
        }

        public static ITypeBuilder Abstract(this ITypeBuilder typeBuilder) {
            typeBuilder.TypeAttributes |= TypeAttributes.Abstract;
            return typeBuilder;
        }

        public static ITypeBuilder BeforeFieldInit(this ITypeBuilder typeBuilder) {
            typeBuilder.TypeAttributes |= TypeAttributes.BeforeFieldInit;
            return typeBuilder;
        }

        public static IFieldBuilder NewField<T>(this ITypeBuilder typeBuilder, string fieldName) {
            return typeBuilder.NewField(fieldName, typeof(T));
        }

        public static IPropertyBuilder NewProperty<T>(this ITypeBuilder typeBuilder, string propertyName) {
            return typeBuilder.NewProperty(propertyName, typeof(T));
        }

        public static IMethodBuilder NewMethod<TReturn>(this ITypeBuilder typeBuilder, string name) {
            return typeBuilder.NewMethod(
                name,
                MethodAttributes.Public,
                CallingConventions.HasThis,
                typeof(TReturn));
        }
    }
}