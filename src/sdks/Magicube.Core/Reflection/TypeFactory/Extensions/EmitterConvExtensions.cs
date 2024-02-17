using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection {
    public static class EmitterConvExtensions {
        public static IEmitter ConvI(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_I);
        }

        public static IEmitter ConvI1(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_I1);
        }

        public static IEmitter ConvI2(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_I2);
        }

        public static IEmitter ConvI4(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_I4);
        }

        public static IEmitter ConvI8(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_I8);
        }

        public static IEmitter ConvOvfI(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_Ovf_I);
        }

        public static IEmitter ConvOvfI1(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_Ovf_I1);
        }

        public static IEmitter ConvOvfI2(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_Ovf_I2);
        }

        public static IEmitter ConvOvfI4(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_Ovf_I4);
        }

        public static IEmitter ConvOvfI8(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_Ovf_I8);
        }

        public static IEmitter ConvOvfIUn(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_Ovf_I_Un);
        }

        public static IEmitter ConvOvfI1Un(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_Ovf_I1_Un);
        }

        public static IEmitter ConvOvfI2Un(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_Ovf_I2_Un);
        }

        public static IEmitter ConvOvfI4Un(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_Ovf_I4_Un);
        }

        public static IEmitter ConvOvfI8Un(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_Ovf_I8_Un);
        }

        public static IEmitter ConvRUn(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_R_Un);
        }

        public static IEmitter ConvR4(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_R4);
        }

        public static IEmitter ConvR8(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_R8);
        }

        public static IEmitter ConvU(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_U);
        }

        public static IEmitter ConvU1(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_U1);
        }

        public static IEmitter ConvU2(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_U2);
        }

        public static IEmitter ConvU4(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_U4);
        }

        public static IEmitter ConvU8(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Conv_U8);
        }

        public static IEmitter Conv(
            this IEmitter emitter,
            Type sourceType,
            Type targetType,
            bool isAddress) {
            if (sourceType != targetType) {
                if (sourceType.IsByRef == true) {
                    Type elementType = sourceType.GetElementType();
                    emitter.LdInd(elementType);
                    emitter.Conv(elementType, targetType, isAddress);
                }
#if NETSTANDARD1_6
                else if (targetType.GetTypeInfo().IsValueType == true)
                {
                    if (sourceType.GetTypeInfo().IsValueType == true)
#else
                else if (targetType.IsValueType == true) {
                    if (sourceType.IsValueType == true)
#endif
                    {
                        emitter.EmitConv(targetType);
                    } else {
                        emitter.Emit(OpCodes.Unbox, targetType);
                        if (isAddress == false) {
                            emitter.LdInd(targetType);
                        }
                    }
                } else if (targetType.IsAssignableFrom(sourceType) == true) {
#if NETSTANDARD1_6
                    if (sourceType.GetTypeInfo().IsValueType == true)
#else
                    if (sourceType.IsValueType == true)
#endif
                    {
                        if (isAddress == true) {
                            emitter.LdInd(sourceType);
                        }

                        emitter.Emit(OpCodes.Box, sourceType);
                    }
                } else if (targetType.IsGenericParameter == true) {
                    emitter.Emit(OpCodes.Unbox_Any, targetType);
                } else {
                    emitter.Emit(OpCodes.Castclass, targetType);
                }
            }

            return emitter;
        }

        public static IEmitter EmitConv(this IEmitter emitter, Type type) {
            switch (Type.GetTypeCode(type)) {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                    emitter.ConvI1();
                    break;

                case TypeCode.Char:
                case TypeCode.Int16:
                    emitter.ConvI2();
                    break;

                case TypeCode.Byte:
                    emitter.ConvU2();
                    break;

                case TypeCode.Int32:
                    emitter.ConvI4();
                    break;

                case TypeCode.UInt32:
                    emitter.ConvU4();
                    break;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    emitter.ConvI8();
                    break;

                case TypeCode.Single:
                    emitter.ConvR4();
                    break;

                case TypeCode.Double:
                    emitter.ConvR8();
                    break;

                default:
                    emitter.Nop();
                    break;
            }

            return emitter;
        }
    }
}