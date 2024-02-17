using System.Reflection;

namespace Magicube.Core.Reflection {
    public static class FieldBuilderExtensionMethods {
        public static IFieldBuilder Private(this IFieldBuilder builder) {
            builder.FieldAttributes |= FieldAttributes.Private;
            return builder;
        }

        public static IFieldBuilder FamANDAssem(this IFieldBuilder builder) {
            builder.FieldAttributes |= FieldAttributes.FamANDAssem;
            return builder;
        }

        public static IFieldBuilder Assembly(this IFieldBuilder builder) {
            builder.FieldAttributes |= FieldAttributes.Assembly;
            return builder;
        }

        public static IFieldBuilder Family(this IFieldBuilder builder) {
            builder.FieldAttributes |= FieldAttributes.Family;
            return builder;
        }

        public static IFieldBuilder FamORAssem(this IFieldBuilder builder) {
            builder.FieldAttributes |= FieldAttributes.FamORAssem;
            return builder;
        }

        public static IFieldBuilder Public(this IFieldBuilder builder) {
            builder.FieldAttributes = (builder.FieldAttributes & ~FieldAttributes.Private) | FieldAttributes.Public;
            return builder;
        }

        public static IFieldBuilder Static(this IFieldBuilder builder) {
            builder.FieldAttributes |= FieldAttributes.Static;
            return builder;
        }

        public static IFieldBuilder InitOnly(this IFieldBuilder builder) {
            builder.FieldAttributes |= FieldAttributes.InitOnly;
            return builder;
        }

        public static IFieldBuilder Literal(this IFieldBuilder builder) {
            builder.FieldAttributes |= FieldAttributes.Literal;
            return builder;
        }

        public static IFieldBuilder NotSerialized(this IFieldBuilder builder) {
            builder.FieldAttributes |= FieldAttributes.NotSerialized;
            return builder;
        }

        public static IFieldBuilder HasFieldRVA(this IFieldBuilder builder) {
            builder.FieldAttributes |= FieldAttributes.HasFieldRVA;
            return builder;
        }

        public static IFieldBuilder SpecialName(this IFieldBuilder builder) {
            builder.FieldAttributes |= FieldAttributes.SpecialName;
            return builder;
        }

        public static IFieldBuilder RTSpecialName(this IFieldBuilder builder) {
            builder.FieldAttributes |= FieldAttributes.RTSpecialName;
            return builder;
        }

        public static IFieldBuilder HasFieldMarshal(this IFieldBuilder builder) {
            builder.FieldAttributes |= FieldAttributes.HasFieldMarshal;
            return builder;
        }

        public static IFieldBuilder HasDefault(this IFieldBuilder builder) {
            builder.FieldAttributes |= FieldAttributes.HasDefault;
            return builder;
        }
    }
}