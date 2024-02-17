using System;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection.Builders {
    public class ArrayBuilder {
        private readonly IEmitter _emitter;

        private ILocal _localArray;

        public ArrayBuilder(IEmitter emitter, Type arrayType, int length, ILocal localArray = null) {
            if (localArray != null &&
                localArray.LocalType.IsArray == false) {
                throw new InvalidProgramException("The local array type is not an array");
            }

            _emitter = emitter;
            _localArray = localArray;
            if (_localArray == null) {
                _emitter.DeclareLocal(arrayType.MakeArrayType(), out _localArray);
            }

            _emitter
                .LdcI4(length)
                .NewArr(arrayType)
                .StLoc(_localArray);
        }

        public void SetStart(int index) {
            _emitter
                .LdLoc(_localArray)
                .LdcI4(index);
        }

        public void SetEnd() {
            _emitter.Emit(OpCodes.Stelem_Ref);
        }

        public void Set(int index, Action action) {
            SetStart(index);

            if (action != null) {
                action();
            }

            SetEnd();
        }

        public void Set(int index, Action<int> action) {
            SetStart(index);

            if (action != null) {
                action(index);
            }

            SetEnd();
        }

        public void Get(int index) {
            _emitter
                .LdLoc(_localArray)
                .LdcI4(index)
                .Emit(OpCodes.Ldelem_Ref);
        }

        public void Load() {
            _emitter.LdLoc(_localArray);
        }
    }
}
