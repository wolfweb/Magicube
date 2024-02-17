using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection {
    public static class EmitterArithmeticExtensions {
        public static IEmitter CkFinite(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ckfinite);
        }

        public static IEmitter Neg(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Neg);
        }

        public static IEmitter Add(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Add);
        }

        public static IEmitter Add(this IEmitter emitter, ILocal localValue1, ILocal localValue2) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .Add();
        }

        public static IEmitter AddOvf(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Add_Ovf);
        }

        public static IEmitter AddOvf(this IEmitter emitter, ILocal localValue1, ILocal localValue2) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .AddOvf();
        }

        public static IEmitter AddOvfUn(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Add_Ovf_Un);
        }

        public static IEmitter AddOvfUn(this IEmitter emitter, ILocal localValue1, ILocal localValue2) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .AddOvfUn();
        }

        public static IEmitter Sub(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Sub);
        }

        public static IEmitter Sub(this IEmitter emitter, ILocal localValue1, ILocal localValue2) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .Sub();
        }

        public static IEmitter SubOvf(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Sub_Ovf);
        }

        public static IEmitter SubOvf(this IEmitter emitter, ILocal localValue1, ILocal localValue2) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .SubOvf();
        }

        public static IEmitter SubOvfUn(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Sub_Ovf_Un);
        }

        public static IEmitter SubOvfUn(this IEmitter emitter, ILocal localValue1, ILocal localValue2) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .SubOvfUn();
        }

        public static IEmitter Mul(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Mul);
        }

        public static IEmitter Mul(this IEmitter emitter, ILocal localValue1, ILocal localValue2) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .Mul();
        }

        public static IEmitter MulOvf(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Mul_Ovf);
        }

        public static IEmitter MulOvf(this IEmitter emitter, ILocal localValue1, ILocal localValue2) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .MulOvf();
        }

        public static IEmitter MulOvfUn(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Mul_Ovf_Un);
        }

        public static IEmitter MulOvfUn(this IEmitter emitter, ILocal localValue1, ILocal localValue2) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .MulOvfUn();
        }

        public static IEmitter Div(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Div);
        }

        public static IEmitter Div(this IEmitter emitter, ILocal localValue1, ILocal localValue2) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .Div();
        }

        public static IEmitter DivUn(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Div_Un);
        }

        public static IEmitter DivUn(this IEmitter emitter, ILocal localValue1, ILocal localValue2) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .DivUn();
        }

        public static IEmitter Rem(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Rem);
        }

        public static IEmitter Rem(this IEmitter emitter, ILocal localValue1, ILocal localValue2) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .Rem();
        }

        public static IEmitter RemUn(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Rem_Un);
        }

        public static IEmitter RemUn(this IEmitter emitter, ILocal localValue1, ILocal localValue2) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .RemUn();
        }
    }
}