using System;
using System.Reflection;
using System.Reflection.Emit;
using Magicube.Core.Reflection.Builders;

namespace Magicube.Core.Reflection {
    public static class EmitterArrayExtensions {
        public static IEmitter NewArr<T>(this IEmitter emitter) {
            return emitter.NewArr(typeof(T));
        }

        public static IEmitter NewArr(this IEmitter emitter, Type arrayType) {
            return emitter.Emit(OpCodes.Newarr, arrayType);
        }

        public static IEmitter NewArr<T>(this IEmitter emitter, int length) {
            return emitter.NewArr(typeof(T), length);
        }

        public static IEmitter NewArr(this IEmitter emitter, Type arrayType, int length) {
            return emitter
                .LdcI4(length)
                .NewArr(arrayType);
        }

        public static IEmitter NewArr<T>(this IEmitter emitter, int length, ILocal localArray) {
            return emitter.NewArr(typeof(T), length, localArray);
        }

        public static IEmitter NewArr(this IEmitter emitter, Type arrayType, int length, ILocal localArray) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("The local array type is not an array");
            }

            return emitter
                .LdcI4(length)
                .NewArr(arrayType)
                .StLoc(localArray);
        }

        public static IEmitter LdLen(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldlen);
        }

        public static IEmitter LdLen(this IEmitter emitter, ILocal localArray) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("Local must be an array");
            }

            return emitter
                .LdLoc(localArray)
                .Emit(OpCodes.Ldlen);
        }

        public static IEmitter LdArrayElem<T>(this IEmitter emitter) {
            return emitter.LdArrayElem(typeof(T));
        }

        public static IEmitter LdArrayElem(this IEmitter emitter, Type elementType) {
            return emitter.Emit(OpCodes.Ldelem, elementType);
        }

        public static IEmitter LdArrayElem<T>(this IEmitter emitter, int index) {
            return emitter
                .LdArrayElem(typeof(T), index);
        }

        public static IEmitter LdArrayElem(this IEmitter emitter, Type elementType, int index) {
            return emitter
                .LdcI4(index)
                .LdArrayElem(elementType);
        }

        public static IEmitter LdArrayElem(this IEmitter emitter, ILocal localArray, int index) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("Local must be an array");
            }

            return emitter
                .LdLoc(localArray)
                .LdcI4(index)
                .LdArrayElem(localArray.LocalType);
        }

        public static IEmitter LdElemI(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldelem_I);
        }

        public static IEmitter LdElemI(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .LdElemI();
        }

        public static IEmitter LdElemI1(this IEmitter emitter) {
            emitter.Emit(OpCodes.Ldelem_I1);
            return emitter;
        }

        public static IEmitter LdElemI1(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .LdElemI1();
        }

        public static IEmitter LdElemI2(this IEmitter emitter) {
            emitter.Emit(OpCodes.Ldelem_I2);
            return emitter;
        }

        public static IEmitter LdElemI2(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .LdElemI2();
        }

        public static IEmitter LdElemI4(this IEmitter emitter) {
            emitter.Emit(OpCodes.Ldelem_I4);
            return emitter;
        }

        public static IEmitter LdElemI4(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .LdElemI4();
        }

        public static IEmitter LdElemI8(this IEmitter emitter) {
            emitter.Emit(OpCodes.Ldelem_I8);
            return emitter;
        }

        public static IEmitter LdElemI8(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .LdElemI8();
        }

        public static IEmitter LdElemR4(this IEmitter emitter) {
            emitter.Emit(OpCodes.Ldelem_R4);
            return emitter;
        }

        public static IEmitter LdElemR4(this IEmitter emitter, int index) {
            emitter.LdcI4(index);
            emitter.LdElemR4();
            return emitter;
        }

        public static IEmitter LdElemR8(this IEmitter emitter) {
            emitter.Emit(OpCodes.Ldelem_R8);
            return emitter;
        }

        public static IEmitter LdElemR8(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .LdElemR8();
        }

        public static IEmitter LdElemRef(this IEmitter emitter) {
            emitter.Emit(OpCodes.Ldelem_Ref);
            return emitter;
        }

        public static IEmitter LdElemRef(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .LdElemRef();
        }

        public static IEmitter LdElemU1(this IEmitter emitter) {
            emitter.Emit(OpCodes.Ldelem_U1);
            return emitter;
        }

        public static IEmitter LdElemU1(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .LdElemU1();
        }

        public static IEmitter LdElemU2(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldelem_U2);
        }

        public static IEmitter LdElemU2(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .LdElemU2();
        }

        public static IEmitter LdElemU4(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldelem_U4);
        }

        public static IEmitter LdElemU4(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .LdElemU4();
        }

        public static IEmitter LdElemAddr(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ldelema);
        }

        public static IEmitter LdElemAddr(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .LdElemAddr();
        }

        public static IEmitter StElem<T>(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stelem, typeof(T));
        }

        public static IEmitter StElem(this IEmitter emitter, Type elementType) {
            return emitter.Emit(OpCodes.Stelem, elementType);
        }

        public static IEmitter StElem(this IEmitter emitter, Type elementType, int index, ILocal localValue) {
            return emitter
                .LdcI4(index)
                .LdLoc(localValue)
                .StElem(elementType);
        }

        public static IEmitter StElem(this IEmitter emitter, ILocal localArray, int index, ILocal localValue) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("The local array type is not an array");
            }

            return emitter
                .LdLoc(localArray)
                .LdcI4(index)
                .LdLoc(localValue)
                .StElem(localArray.LocalType);
        }

        public static IEmitter StElemI(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stelem_I);
        }

        public static IEmitter StElemI(this IEmitter emitter, int value) {
            return emitter
                .LdcI4(value)
                .StElemI();
        }

        public static IEmitter StElemI(this IEmitter emitter, int index, int value) {
            return emitter
                .LdcI4(index)
                .LdcI4(value)
                .StElemI();
        }

        public static IEmitter StElemI(this IEmitter emitter, ILocal localArray, int index, int value) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("The local array type is not an array");
            }

            return emitter
                .LdLoc(localArray)
                .LdcI4(index)
                .LdcI4(value)
                .StElemI();
        }

        public static IEmitter StElemI1(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stelem_I1);
        }

        public static IEmitter StElemI1(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .StElemI1();
        }

        public static IEmitter StElemI1(this IEmitter emitter, ILocal localArray, int index) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("The local array type is not an array");
            }

            return emitter
                .LdLoc(localArray)
                .StElemI1(index);
        }

        public static IEmitter StElemI2(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stelem_I2);
        }

        public static IEmitter StElemI2(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .StElemI2();
        }

        public static IEmitter StElemI2(this IEmitter emitter, ILocal localArray, int index) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("The local array type is not an array");
            }

            return emitter
                .LdLoc(localArray)
                .StElemI2(index);
        }

        public static IEmitter StElemI4(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stelem_I4);
        }

        public static IEmitter StElemI4(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .StElemI4();
        }

        public static IEmitter StElemI4(this IEmitter emitter, ILocal localArray, int index) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("The local array type is not an array");
            }

            return emitter
                .LdLoc(localArray)
                .StElemI4(index);
        }

        public static IEmitter StElemI8(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stelem_I8);
        }

        public static IEmitter StElemI8(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .StElemI8();
        }

        public static IEmitter StElemI8(this IEmitter emitter, ILocal localArray, int index) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("The local array type is not an array");
            }

            return emitter
                .LdLoc(localArray)
                .StElemI8(index);
        }

        public static IEmitter StElemR4(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stelem_R4);
        }

        public static IEmitter StElemR4(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .StElemR4();
        }

        public static IEmitter StElemR4(this IEmitter emitter, ILocal localArray, int index) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("The local array type is not an array");
            }

            return emitter
                .LdLoc(localArray)
                .StElemR4(index);
        }

        public static IEmitter StElemR8(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stelem_R8);
        }

        public static IEmitter StElemR8(this IEmitter emitter, int index) {
            return emitter
                .LdcI4(index)
                .StElemR8();
        }

        public static IEmitter StElemR8(this IEmitter emitter, ILocal localArray, int index) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("The local array type is not an array");
            }

            return emitter
                .LdLoc(localArray)
                .StElemR8(index);
        }

        public static IEmitter StElemRef(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Stelem_Ref);
        }

        public static IEmitter Array(this IEmitter emitter, ILocal localArray, int length, Action<int> action) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("The local array type is not an array");
            }

            ArrayBuilder arrayBuilder = new ArrayBuilder(emitter, localArray.LocalType, length, localArray);
            for (int i = 0; i < length; i++) {
                arrayBuilder.Set(i, action);
            }

            return emitter;
        }

        public static IEmitter Array(this IEmitter emitter, Type arrayType, ILocal localArray, int length, Action<int> action) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("The local array type is not an array");
            }

            ArrayBuilder arrayBuilder = new ArrayBuilder(emitter, arrayType, length, localArray);
            for (int i = 0; i < length; i++) {
                arrayBuilder.Set(i, action);
            }

            return emitter;
        }

        public static IEmitter TypeArray(this IEmitter emitter, ILocal localArray, params ILocal[] localTypes) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("The local array type is not an array");
            }

            ArrayBuilder arrayBuilder = new ArrayBuilder(emitter, typeof(Type), localTypes.Length, localArray);
            for (int i = 0; i < localTypes.Length; i++) {
                arrayBuilder.Set(i, () => emitter.LdLoc(localTypes[i]));
            }

            return emitter;
        }

        public static IEmitter TypeArray(this IEmitter emitter, ILocal localArray, params Type[] types) {
            if (localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("The local array type is not an array");
            }

            ArrayBuilder arrayBuilder = new ArrayBuilder(emitter, typeof(Type), types.Length, localArray);
            for (int i = 0; i < types.Length; i++) {
                arrayBuilder.Set(i, () => emitter.EmitTypeOf(types[i]));
            }

            return emitter;
        }
    }
}