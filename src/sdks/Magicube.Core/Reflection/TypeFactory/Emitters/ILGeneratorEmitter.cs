using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection.Emitters {
    internal class ILGeneratorEmitter : EmitterBase {
        private ILGenerator _generator;

        public ILGeneratorEmitter(ILGenerator generator) {
            _generator = generator;
        }

        public override int ILOffset => _generator.ILOffset;

        public override IEmitter BeginCatchBlock(Type exceptionType) {
            _generator.BeginCatchBlock(exceptionType);
            return this;
        }

        public override IEmitter BeginExceptFilterBlock() {
            _generator.BeginExceptFilterBlock();
            return this;
        }

        public override IEmitter BeginExceptionBlock(out ILabel label) {
            var actualLabel = _generator.BeginExceptionBlock();
            label = new LabelAdapter(Guid.NewGuid().ToString(), actualLabel);
            return this;
        }

        public override IEmitter BeginExceptionBlock(ILabel label) {
            var actualLabel = _generator.BeginExceptionBlock();
            ((IAdaptedLabel)label).Label = actualLabel;
            return this;
        }

        public override IEmitter BeginFaultBlock() {
            _generator.BeginFaultBlock();
            return this;
        }

        public override IEmitter BeginFinallyBlock() {
            _generator.BeginFinallyBlock();
            return this;
        }

        public override IEmitter BeginScope() {
            _generator.BeginScope();
            return this;
        }

        public override IEmitter DeclareLocal(Type localType, out ILocal local) {
            return DeclareLocal(localType, Guid.NewGuid().ToString(), false, out local);
        }

        public override IEmitter DeclareLocal(Type localType, string localName, out ILocal local) {
            return DeclareLocal(localType, localName, false, out local);
        }

        public override IEmitter DeclareLocal(Type localType, bool pinned, out ILocal local) {
            return DeclareLocal(localType, Guid.NewGuid().ToString(), pinned, out local);
        }

        public override IEmitter DeclareLocal(Type localType, string localName, bool pinned, out ILocal local) {
            var actualLocal = _generator.DeclareLocal(localType, pinned);
            local = new LocalAdapter(localName, localType, actualLocal.LocalIndex, pinned, actualLocal);
            return this;
        }

        public override IEmitter DeclareLocal(IGenericParameterBuilder genericParameter, out ILocal local) {
            // var actualLocal = this.generator.DeclareLocal(localType, pinned);
            local = new LocalAdapter(Guid.NewGuid().ToString(), genericParameter);
            return this;
        }

        public override IEmitter DeclareLocal(Type localGenericTypeDefinition, IGenericParameterBuilder[] genericTypeArgs, out ILocal local) {
            // var actualLocal = this.generator.DeclareLocal(localType, pinned);
            local = new LocalAdapter(Guid.NewGuid().ToString(), localGenericTypeDefinition, genericTypeArgs);
            return this;
        }

        public override IEmitter DeclareLocal(ITypeBuilder typeBuilder, out ILocal local) {
            var actualLocal = _generator.DeclareLocal(typeBuilder.CreateType(), false);
            local = new LocalAdapter(Guid.NewGuid().ToString(), actualLocal.LocalType, actualLocal.LocalIndex, false, actualLocal);
            return this;
        }

        public override IEmitter DeclareLocal(ILocal local) {
            var actualLocal = _generator.DeclareLocal(local.LocalType, local.IsPinned);
            ((IAdaptedLocal)local).Local = actualLocal;
            return this;
        }

        public override IEmitter DefineLabel(out ILabel label) {
            return DefineLabel(Guid.NewGuid().ToString(), out label);
        }

        public override IEmitter DefineLabel(string labelName, out ILabel label) {
            var actualLabel = _generator.DefineLabel();
            label = new LabelAdapter(labelName, actualLabel);
            return this;
        }

        public override IEmitter DefineLabel(ILabel label) {
            ((IAdaptedLabel)label).Label = _generator.DefineLabel();
            return this;
        }

        public override IEmitter Emit(OpCode opcode, Type type) {
            _generator.Emit(opcode, type);

            if (type == null) {
                return this;
            }

            return this;
        }

        public override IEmitter Emit(OpCode opcode, string str) {
            _generator.Emit(opcode, str);
            return this;
        }

        public override IEmitter Emit(OpCode opcode, float arg) {
            _generator.Emit(opcode, arg);
            return this;
        }

        public override IEmitter Emit(OpCode opcode, sbyte arg) {
            _generator.Emit(opcode, arg);
            return this;
        }

        public override IEmitter Emit(OpCode opcode, MethodInfo methodInfo) {
            _generator.Emit(opcode, methodInfo);
            return this;
        }

        public override IEmitter Emit(OpCode opcode, FieldInfo field) {
            _generator.Emit(opcode, field);
            return this;
        }

        public override IEmitter Emit(OpCode opcode, IFieldBuilder field) {
            _generator.Emit(opcode, field.Define());
            return this;
        }

        public override IEmitter Emit(OpCode opcode, ILabel[] labels) {
            _generator.Emit(opcode, labels?.Select(l => ((Label)((IAdaptedLabel)l).Label)).ToArray());
            return this;
        }

        public override IEmitter Emit(OpCode opcode, SignatureHelper signature) {
            _generator.Emit(opcode, signature);
            return this;
        }

        public override IEmitter Emit(OpCode opcode, ILocal local) {
            _generator.Emit(opcode, ((IAdaptedLocal)local)?.Local as LocalBuilder);
            return this;
        }

        public override IEmitter Emit(OpCode opcode, ConstructorInfo con) {
            _generator.Emit(opcode, con);
            return this;
        }

        public override IEmitter Emit(OpCode opcode, long arg) {
            _generator.Emit(opcode, arg);
            return this;
        }

        public override IEmitter Emit(OpCode opcode, int arg) {
            _generator.Emit(opcode, arg);
            return this;
        }

        public override IEmitter Emit(OpCode opcode, short arg) {
            _generator.Emit(opcode, arg);
            return this;
        }

        public override IEmitter Emit(OpCode opcode, double arg) {
            _generator.Emit(opcode, arg);
            return this;
        }

        public override IEmitter Emit(OpCode opcode, byte arg) {
            _generator.Emit(opcode, arg);
            return this;
        }

        public override IEmitter Emit(OpCode opcode) {
            _generator.Emit(opcode);
            return this;
        }

        public override IEmitter Emit(OpCode opcode, ILabel label) {
            _generator.Emit(opcode, (Label)((IAdaptedLabel)label).Label);
            return this;
        }

        public override IEmitter EmitCall(OpCode opcode, Func<MethodInfo> action) {
            _generator.Emit(opcode, action());
            return this;
        }

        public override IEmitter EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes) {
            _generator.EmitCall(opcode, methodInfo, optionalParameterTypes);
            return this;
        }

        public override IEmitter EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes) {
            _generator.EmitCalli(opcode, callingConvention, returnType, parameterTypes, optionalParameterTypes);
            return this;
        }

        public override IEmitter EmitWriteLine(FieldInfo fld) {
            _generator.EmitWriteLine(fld);
            return this;
        }

        public override IEmitter EmitWriteLine(string value) {
            _generator.EmitWriteLine(value);
            return this;
        }

        public override IEmitter EmitWriteLine(ILocal local) {
            _generator.EmitWriteLine((LocalBuilder)((IAdaptedLocal)local).Local);
            return this;
        }

        public override IEmitter EndExceptionBlock() {
            _generator.EndExceptionBlock();
            return this;
        }

        public override IEmitter EndScope() {
            _generator.EndScope();
            return this;
        }

        public override IEmitter MarkLabel(ILabel label) {
            _generator.MarkLabel((Label)((IAdaptedLabel)label).Label);
            return this;
        }

        public override IEmitter ThrowException(Type excType) {
            _generator.ThrowException(excType);
            return this;
        }

        public override IEmitter UsingNamespace(string usingNamespace) {
            _generator.UsingNamespace(usingNamespace);
            return this;
        }

        public override IEmitter Comment(string comment) {
            return this;
        }

        public override IEmitter Defer(Action<IEmitter> action) {
            action(this);
            return this;
        }

        public override void EmitIL(ILGenerator generator) {
        }
    }
}