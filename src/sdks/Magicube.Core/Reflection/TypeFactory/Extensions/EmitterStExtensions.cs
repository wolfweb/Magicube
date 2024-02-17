using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection {
    public static class EmitterStExtensions {
        public static IEmitter StArg(this IEmitter emitter, short index) {
            return emitter.Emit(OpCodes.Starg, index);
        }

        public static IEmitter StArgS(this IEmitter emitter, byte index) {
            return emitter.Emit(OpCodes.Starg_S, index);
        }

        public static IEmitter StArg(this IEmitter emitter, ILocal localValue, short index) {
            return emitter
                .LdLoc(localValue)
                .StArg(index);
        }

        public static IEmitter StArgS(this IEmitter emitter, ILocal localValue, byte index) {
            return emitter
                .LdLoc(localValue)
                .StArgS(index);
        }

        public static IEmitter StLoc(this IEmitter emitter, ILocal local) {
            return emitter.Emit(OpCodes.Stloc, local);
        }

        public static IEmitter StLocS(this IEmitter emitter, ILocal local) {
            return emitter.Emit(OpCodes.Stloc_S, local);
        }

        public static IEmitter StLoc0(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stloc_0);
        }

        public static IEmitter StLoc1(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stloc_1);
        }

        public static IEmitter StLoc2(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stloc_2);
        }

        public static IEmitter StLoc3(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stloc_3);
        }

        public static IEmitter StFld(this IEmitter emitter, IFieldBuilder field) {
            return emitter.Emit(OpCodes.Stfld, field);
        }

        public static IEmitter StFld(this IEmitter emitter, FieldInfo field) {
            return emitter.Emit(OpCodes.Stfld, field);
        }

        public static IEmitter StSFld(this IEmitter emitter, IFieldBuilder field) {
            return emitter.Emit(OpCodes.Stsfld, field);
        }

        public static IEmitter StSFld(this IEmitter emitter, FieldInfo field) {
            return emitter.Emit(OpCodes.Stsfld, field);
        }

        public static IEmitter StObj(this IEmitter emitter, Type type) {
            return emitter.Emit(OpCodes.Stobj, type);
        }

        public static IEmitter StIndI(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stind_I);
        }

        public static IEmitter StIndI1(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stind_I1);
        }

        public static IEmitter StIndI2(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stind_I2);
        }

        public static IEmitter StIndI4(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stind_I4);
        }

        public static IEmitter StIndI8(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stind_I8);
        }

        public static IEmitter StIndR4(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stind_R4);
        }

        public static IEmitter StIndR8(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stind_R8);
        }

        public static IEmitter StIndRef(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stind_Ref);
        }

        public static IEmitter StInd(this IEmitter emitter, Type type) {
#if NETSTANDARD1_6
            switch (Type.GetTypeCode(type))
#else
            switch (Type.GetTypeCode(type))
#endif
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                    return emitter.StIndI1();

                case TypeCode.Char:
                case TypeCode.Int16:
                    return emitter.StIndI2();

                case TypeCode.Int32:
                case TypeCode.UInt32:
                    return emitter.StIndI4();

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return emitter.StIndI8();

                case TypeCode.Single:
                    return emitter.StIndR4();

                case TypeCode.Double:
                    return emitter.StIndR8();

                default:
                    return emitter.StObj(type);
            }
        }
    }
}