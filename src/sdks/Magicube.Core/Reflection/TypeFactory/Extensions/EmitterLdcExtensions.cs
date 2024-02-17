using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection {
    public static class EmitterLdcExtensions {
        public static IEmitter LdcI4(this IEmitter emitter, int value) {
            return emitter.Emit(OpCodes.Ldc_I4, value);
        }

        public static IEmitter LdcI4_0(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldc_I4_0);
        }

        public static IEmitter LdcI4_1(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldc_I4_1);
        }

        public static IEmitter LdcI4_2(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldc_I4_2);
        }

        public static IEmitter LdcI4_3(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldc_I4_3);
        }

        public static IEmitter LdcI4_4(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldc_I4_4);
        }

        public static IEmitter LdcI4_5(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldc_I4_5);
        }

        public static IEmitter LdcI4_6(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldc_I4_6);
        }

        public static IEmitter LdcI4_7(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldc_I4_7);
        }

        public static IEmitter LdcI4_8(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldc_I4_8);
        }

        public static IEmitter LdcI4_M1(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldc_I4_M1);
        }

        public static IEmitter LdcI4_S(this IEmitter emitter, byte value) {
            return emitter.Emit(OpCodes.Ldc_I4_S, value);
        }

        public static IEmitter LdcI8(this IEmitter emitter, long value) {
            return emitter.Emit(OpCodes.Ldc_I8, value);
        }

        public static IEmitter LdcR4(this IEmitter emitter, float value) {
            return emitter.Emit(OpCodes.Ldc_R4, value);
        }

        public static IEmitter LdcR8(this IEmitter emitter, double value) {
            return emitter.Emit(OpCodes.Ldc_R8, value);
        }
    }
}