using System.Reflection;
using System.Reflection.Emit;
using Magicube.Core.Reflection.Emitters;

namespace Magicube.Core.Reflection {
    public static class ConstructorBuilderExtensionMethods {
        public static IConstructorBuilder Public(this IConstructorBuilder builder) {
            builder.MethodAttributes |= MethodAttributes.Public;
            return builder;
        }

        public static IConstructorBuilder Private(this IConstructorBuilder builder) {
            builder.MethodAttributes |= MethodAttributes.Private;
            return builder;
        }

        public static IConstructorBuilder HideBySig(this IConstructorBuilder builder) {
            builder.MethodAttributes |= MethodAttributes.HideBySig;
            return builder;
        }

        public static IConstructorBuilder Assembly(this IConstructorBuilder builder) {
            builder.MethodAttributes |= MethodAttributes.Assembly;
            return builder;
        }

        public static IConstructorBuilder FamANDAssem(this IConstructorBuilder builder) {
            builder.MethodAttributes |= MethodAttributes.FamANDAssem;
            return builder;
        }

        public static IConstructorBuilder Family(this IConstructorBuilder builder) {
            builder.MethodAttributes |= MethodAttributes.Family;
            return builder;
        }

        public static IConstructorBuilder FamORAssem(this IConstructorBuilder builder) {
            builder.MethodAttributes |= MethodAttributes.FamORAssem;
            return builder;
        }

        public static IConstructorBuilder SpecialName(this IConstructorBuilder builder) {
            builder.MethodAttributes |= MethodAttributes.SpecialName;
            return builder;
        }

        public static IConstructorBuilder RTSpecialName(this IConstructorBuilder builder) {
            builder.MethodAttributes |= MethodAttributes.RTSpecialName;
            return builder;
        }

        public static IConstructorBuilder Static(this IConstructorBuilder builder) {
            builder.CallingConvention(CallingConventions.Standard);
            return builder;
        }

        public static IConstructorBuilder Param<T>(this IConstructorBuilder builder, string parameterName) {
            builder.Param(typeof(T), parameterName);
            return builder;
        }

        public static IEmitter Body(this ConstructorBuilder constructorBuilder) {
            var emitter = new ILGeneratorEmitter(constructorBuilder.GetILGenerator());
            return emitter;

        }
    }
}