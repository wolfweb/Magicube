using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection {
    public static class EmitterLdExtensions {
        public static IEmitter LdStr(this IEmitter emitter, string value) {
            return emitter.Emit(OpCodes.Ldstr, value);
        }

        public static IEmitter LdLoc(this IEmitter emitter, ILocal local) {
            return emitter.Emit(OpCodes.Ldloc, local);
        }

        public static IEmitter LdLocS(this IEmitter emitter, ILocal local) {
            return emitter.Emit(OpCodes.Ldloc_S, local);
        }

        public static IEmitter LdLoc0(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldloc_0);
        }

        public static IEmitter LdLoc1(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldloc_1);
        }

        public static IEmitter LdLoc2(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldloc_2);
        }

        public static IEmitter LdLoc3(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldloc_3);
        }

        public static IEmitter LdLocA(this IEmitter emitter, ILocal local) {
            return emitter.LdLocA(local.LocalIndex);
        }

        public static IEmitter LdLocA(this IEmitter emitter, int index) {
            return emitter.Emit(OpCodes.Ldloca, Convert.ToInt16(index));
        }

        public static IEmitter LdLocAS(this IEmitter emitter, ILocal local) {
            return emitter.Defer(
                (e) => {
                    int index = local.LocalIndex;
                    if (index > 255) {
                        throw new InvalidProgramException("Local index greater than 255 so short form cannot be used");
                    }

                    e.LdLocAS(Convert.ToByte(index));
                });
        }

        public static IEmitter LdLocAS(this IEmitter emitter, byte index) {
            return emitter.Emit(OpCodes.Ldloca_S, index);
        }

        public static IEmitter LdFld(this IEmitter emitter, FieldInfo field) {
            return emitter.Emit(OpCodes.Ldfld, field);
        }

        public static IEmitter LdFld(this IEmitter emitter, IFieldBuilder field) {
            return emitter.Emit(OpCodes.Ldfld, field);
        }

        public static IEmitter LdFlda(this IEmitter emitter, FieldBuilder field) {
            return emitter.Emit(OpCodes.Ldflda, field);
        }

        public static IEmitter LdFlda(this IEmitter emitter, IFieldBuilder field) {
            return emitter.Emit(OpCodes.Ldflda, field);
        }

        public static IEmitter LdsFld(this IEmitter emitter, IFieldBuilder field) {
            return emitter.Emit(OpCodes.Ldsfld, field);
        }

        public static IEmitter LdsFld(this IEmitter emitter, FieldBuilder field) {
            return emitter.Emit(OpCodes.Ldsfld, field);
        }

        public static IEmitter LdsFlda(this IEmitter emitter, IFieldBuilder field) {
            return emitter.Emit(OpCodes.Ldsflda, field);
        }

        public static IEmitter LdsFlda(this IEmitter emitter, FieldBuilder field) {
            return emitter.Emit(OpCodes.Ldsflda, field);
        }

        public static IEmitter LdArg(this IEmitter emitter, int index) {
            return emitter.Emit(OpCodes.Ldarg, index);
        }

        public static IEmitter LdArg0(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldarg_0);
        }

        public static IEmitter LdArg1(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldarg_1);
        }

        public static IEmitter LdArg2(this IEmitter emitter) {
            emitter.Emit(OpCodes.Ldarg_2);
            return emitter;
        }

        public static IEmitter LdArg3(this IEmitter emitter) {
            emitter.Emit(OpCodes.Ldarg_3);
            return emitter;
        }

        public static IEmitter LdArgS(this IEmitter emitter, byte index) {
            emitter.Emit(OpCodes.Ldarg_S, index);
            return emitter;
        }

        public static IEmitter LdToken(this IEmitter emitter, Type type) {
            emitter.Emit(OpCodes.Ldtoken, type);
            return emitter;
        }

        public static IEmitter LdToken(this IEmitter emitter, MethodInfo method) {
            emitter.Emit(OpCodes.Ldtoken, method);
            return emitter;
        }

        public static IEmitter LdToken(this IEmitter emitter, FieldInfo field) {
            emitter.Emit(OpCodes.Ldtoken, field);
            return emitter;
        }

        public static IEmitter LdToken(this IEmitter emitter, IFieldBuilder field) {
            return emitter
                .LdToken(field.Define());
        }

        public static IEmitter LdNull(this IEmitter emitter) {
            emitter.Emit(OpCodes.Ldnull);
            return emitter;
        }

        public static IEmitter LdObj(this IEmitter emitter, Type type) {
            emitter.Emit(OpCodes.Ldobj, type);
            return emitter;
        }

        public static IEmitter LdIndI(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldind_I);
        }

        public static IEmitter LdIndI1(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldind_I1);
        }

        public static IEmitter LdIndI2(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldind_I2);
        }

        public static IEmitter LdIndI4(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldind_I4);
        }

        public static IEmitter LdIndI8(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldind_I8);
        }

        public static IEmitter LdIndR4(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldind_R4);
        }

        public static IEmitter LdIndR8(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldind_R8);
        }

        public static IEmitter LdIndRef(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldind_Ref);
        }

        public static IEmitter LdIndU1(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldind_U1);
        }

        public static IEmitter LdIndU2(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldind_U2);
        }

        public static IEmitter LdIndU4(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldind_U4);
        }

        public static IEmitter LdInd(this IEmitter emitter, Type type) {
#if NETSTANDARD1_6
            switch (Type.GetTypeCode(type))
#else
            switch (Type.GetTypeCode(type))
#endif
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                    return emitter.LdIndI1();

                case TypeCode.Char:
                case TypeCode.Int16:
                    return emitter.LdIndI2();

                case TypeCode.Byte:
                    return emitter.LdIndU1();

                case TypeCode.Int32:
                    return emitter.LdIndI4();

                case TypeCode.UInt32:
                    return emitter.LdIndU4();

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return emitter.LdIndI8();

                case TypeCode.Single:
                    return emitter.LdIndR4();

                case TypeCode.Double:
                    return emitter.LdIndR8();

                default:
                    return emitter.LdObj(type);
            }
        }
    }
}