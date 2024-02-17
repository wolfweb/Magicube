using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection {
    public static class EmitterCompareAndBranchExtensions {
        public static IEmitter Br(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Br, target);
        }

        public static IEmitter BrS(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Br_S, target);
        }

        public static IEmitter Beq(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Beq, target);
        }

        public static IEmitter Beq(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .Beq(target);
        }

        public static IEmitter BeqS(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Beq_S, target);
        }

        public static IEmitter BeqS(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BeqS(target);
        }

        public static IEmitter Bgt(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Bgt, target);
        }

        public static IEmitter Bgt(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .Bgt(target);
        }

        public static IEmitter BgtS(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Bgt_S, target);
        }

        public static IEmitter BgtS(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BgtS(target);
        }

        public static IEmitter BgtUn(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Bgt_Un, target);
        }

        public static IEmitter BgtUn(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BgtUn(target);
        }

        public static IEmitter BgtUnS(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Bgt_Un_S, target);
        }

        public static IEmitter BgtUnS(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BgtUnS(target);
        }

        public static IEmitter Bge(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Bge, target);
        }

        public static IEmitter Bge(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .Bge(target);
        }

        public static IEmitter BgeS(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Bge_S, target);
        }

        public static IEmitter BgeS(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BgeS(target);
        }

        public static IEmitter BgeUn(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Bge_Un, target);
        }

        public static IEmitter BgeUn(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BgeUn(target);
        }

        public static IEmitter BgeUnS(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Bge_Un_S, target);
        }

        public static IEmitter BgeUnS(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BgeUnS(target);
        }

        public static IEmitter Blt(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Blt, target);
        }

        public static IEmitter Blt(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .Blt(target);
        }

        public static IEmitter BltS(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Blt_S, target);
        }

        public static IEmitter BltS(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BltS(target);
        }

        public static IEmitter BltUn(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Blt_Un, target);
        }

        public static IEmitter BltUn(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BltUn(target);
        }

        public static IEmitter BltUnS(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Blt_Un_S, target);
        }

        public static IEmitter BltUnS(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BltUnS(target);
        }

        public static IEmitter Ble(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Ble, target);
        }

        public static IEmitter Ble(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .Ble(target);
        }

        public static IEmitter BleS(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Ble_S, target);
        }

        public static IEmitter BleS(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BleS(target);
        }

        public static IEmitter BleUn(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Ble_Un, target);
        }

        public static IEmitter BleUn(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BleUn(target);
        }

        public static IEmitter BleUnS(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Ble_Un_S, target);
        }

        public static IEmitter BleUnS(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BleUnS(target);
        }

        public static IEmitter BneUn(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Bne_Un, target);
        }

        public static IEmitter BneUn(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BneUn(target);
        }

        public static IEmitter BneUnS(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Bne_Un_S, target);
        }

        public static IEmitter BneUnS(this IEmitter emitter, ILocal localValue1, ILocal localValue2, ILabel target) {
            return emitter
                .LdLoc(localValue1)
                .LdLoc(localValue2)
                .BneUnS(target);
        }

        public static IEmitter BrTrue(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Brtrue, target);
        }

        public static IEmitter BrTrueS(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Brtrue_S, target);
        }

        public static IEmitter BrFalse(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Brfalse, target);
        }

        public static IEmitter BrFalseS(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Brfalse_S, target);
        }

        public static IEmitter Ceq(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ceq);
        }

        public static IEmitter Cgt(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Cgt);
        }

        public static IEmitter CgtUn(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Cgt_Un);
        }

        public static IEmitter Clt(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Clt);
        }

        public static IEmitter CltUn(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Clt_Un);
        }
    }
}