using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection.Emitters {
    internal class DeferredILGeneratorEmitter : EmitterBase {
        private int offset = 0;

        private List<Action<ILGenerator>> _actions = new List<Action<ILGenerator>>();

        public override int ILOffset => this.offset;

        public override IEmitter BeginCatchBlock(Type exceptionType) {
            _actions.Add(
                generator => {
                    generator.BeginCatchBlock(exceptionType);
                });

            return this;
        }

        public override IEmitter BeginExceptFilterBlock() {
           _actions.Add(
                generator => {
                    generator.BeginExceptFilterBlock();
                });

            return this;
        }

        public override IEmitter BeginExceptionBlock(out ILabel label) {
            var lbl = new LabelAdapter(Guid.NewGuid().ToString());

            _actions.Add(
                generator => {
                    var actualLabel = generator.BeginExceptionBlock();
                    lbl.Label = actualLabel;
                });

            label = lbl;
            return this;
        }

        public override IEmitter BeginExceptionBlock(ILabel label) {
            _actions.Add(
                generator => {
                    var actualLabel = generator.BeginExceptionBlock();
                    ((IAdaptedLabel)label).Label = actualLabel;
                });

            return this;
        }

        public override IEmitter BeginFaultBlock() {
            _actions.Add(
                generator => {
                    generator.BeginFaultBlock();
                });

            return this;
        }

        public override IEmitter BeginFinallyBlock() {
            _actions.Add(
                generator => {
                    generator.BeginFinallyBlock();
                });

            return this;
        }

        public override IEmitter BeginScope() {
            _actions.Add(
                generator => {
                    generator.BeginScope();
                });

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
            var loc = new LocalAdapter(localName, localType, 0, pinned, null);

            _actions.Add(
                generator => {
                    var actualLocal = generator.DeclareLocal(localType, pinned);
                    loc.LocalIndex = actualLocal.LocalIndex;
                    loc.Local = actualLocal;
                });

            local = loc;
            return this;
        }

        public override IEmitter DeclareLocal(IGenericParameterBuilder genericParameter, out ILocal local) {
            var loc = new LocalAdapter(Guid.NewGuid().ToString(), genericParameter);
            _actions.Add(
                generator => {
                    var type = genericParameter.AsType();
                    var actualLocal = generator.DeclareLocal(type, false);
                    loc.LocalIndex = actualLocal.LocalIndex;
                    loc.LocalType = type;
                    loc.Local = actualLocal;
                });

            local = loc;
            return this;
        }

        public override IEmitter DeclareLocal(Type localGenericTypeDefinition, IGenericParameterBuilder[] genericTypeArgs, out ILocal local) {
            var loc = new LocalAdapter(Guid.NewGuid().ToString(), localGenericTypeDefinition, genericTypeArgs);
            _actions.Add(
                generator => {
                    var type = localGenericTypeDefinition.MakeGenericType(genericTypeArgs.Select(t => t.AsType()).ToArray());
                    var actualLocal = generator.DeclareLocal(type, false);
                    loc.LocalIndex = actualLocal.LocalIndex;
                    loc.LocalType = type;
                    loc.Local = actualLocal;
                });

            local = loc;
            return this;
        }

        public override IEmitter DeclareLocal(ITypeBuilder typeBuilder, out ILocal local) {
            var loc = new LocalAdapter(Guid.NewGuid().ToString());

            _actions.Add(
                generator => {
                    var typeBuilderType = typeBuilder.Define().CreateTypeInfo().AsType();
                    var actualLocal = generator.DeclareLocal(typeBuilderType, false);
                    loc.LocalType = typeBuilderType;
                    loc.LocalIndex = actualLocal.LocalIndex;
                    loc.Local = actualLocal;
                });

            local = loc;
            return this;
        }

        public override IEmitter DeclareLocal(ILocal local) {
            _actions.Add(
                generator => {
                    var actualLocal = generator.DeclareLocal(local.LocalType, local.IsPinned);
                    ((IAdaptedLocal)local).Local = actualLocal;
                    ((LocalAdapter)local).LocalIndex = actualLocal.LocalIndex;
                });

            return this;
        }

        public override IEmitter DefineLabel(out ILabel label) {
            return DefineLabel(Guid.NewGuid().ToString(), out label);
        }

        public override IEmitter DefineLabel(string labelName, out ILabel label) {
            var lbl = new LabelAdapter(labelName, null);

            _actions.Add(
                generator => {
                    var actualLabel = generator.DefineLabel();
                    lbl.Label = actualLabel;
                });

            label = lbl;
            return this;
        }

        public override IEmitter DefineLabel(ILabel label) {
            _actions.Add(
                generator => {
                    ((IAdaptedLabel)label).Label = generator.DefineLabel();
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, Type type) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode, type);

                    if (type == null) {
                        return;
                    }
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, string str) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode, str);
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, float arg) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode, arg);
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, sbyte arg) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode, arg);
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, MethodInfo methodInfo) {
            methodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));

            _actions.Add(
                generator => {
                    generator.Emit(opcode, methodInfo);

                    if (methodInfo == null) {
                        return;
                    }
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, FieldInfo field) {
            field = field ?? throw new ArgumentNullException(nameof(field));

            _actions.Add(
                generator => {
                    generator.Emit(opcode, field);
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, IFieldBuilder field) {
            return this.Emit(opcode, field.Define());
        }

        public override IEmitter Emit(OpCode opcode, ILabel[] labels) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode, labels?.Select(l => ((Label)((IAdaptedLabel)l).Label)).ToArray());
                    if (labels == null) {
                        return;
                    }
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, SignatureHelper signature) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode, signature);

                    if (signature == null) {
                        return;
                    }
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, ILocal local) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode, ((IAdaptedLocal)local)?.Local as LocalBuilder);
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, ConstructorInfo con) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode, con);

                    if (con == null) {
                        return;
                    }
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, long arg) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode, arg);
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, int arg) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode, arg);
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, short arg) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode, arg);
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, double arg) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode, arg);
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, byte arg) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode, arg);
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode);
                });

            return this;
        }

        public override IEmitter Emit(OpCode opcode, ILabel label) {
            _actions.Add(
                generator => {
                    generator.Emit(opcode, (Label)((IAdaptedLabel)label).Label);
                });

            return this;
        }

        public override IEmitter EmitCall(OpCode opcode, Func<MethodInfo> action) {
            _actions.Add(
                generator => {
                    var methodInfo = action();
                    generator.Emit(opcode, methodInfo);
                });

            return this;
        }

        public override IEmitter EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes) {
            _actions.Add(
                generator => {
                    generator.EmitCall(opcode, methodInfo, optionalParameterTypes);

                    if (methodInfo == null) {
                        return;
                    }
                });

            return this;
        }

        public override IEmitter EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes) {
            _actions.Add(
                generator => {
                    generator.EmitCalli(opcode, callingConvention, returnType, parameterTypes, optionalParameterTypes);
                });

            return this;
        }

        public override IEmitter EmitWriteLine(FieldInfo fld) {
            _actions.Add(
                generator => {
                    generator.EmitWriteLine(fld);
                });

            return this;
        }

        public override IEmitter EmitWriteLine(string value) {
            _actions.Add(
                generator => {
                    generator.EmitWriteLine(value);
                });

            return this;
        }

        public override IEmitter EmitWriteLine(ILocal local) {
            _actions.Add(
                generator => {
                    generator.EmitWriteLine((LocalBuilder)((IAdaptedLocal)local).Local);
                });

            return this;
        }

        public override IEmitter EndExceptionBlock() {
            _actions.Add(
                generator => {
                    generator.EndExceptionBlock();
                });

            return this;
        }

        public override IEmitter EndScope() {
            _actions.Add(
                generator => {
                    generator.EndScope();
                });

            return this;
        }

        public override IEmitter MarkLabel(ILabel label) {
            _actions.Add(
                generator => {
                    generator.MarkLabel((Label)((IAdaptedLabel)label).Label);
                });

            return this;
        }

        public override IEmitter ThrowException(Type excType) {
            _actions.Add(
                generator => {
                    generator.ThrowException(excType);
                });

            return this;
        }

        public override IEmitter UsingNamespace(string usingNamespace) {
            _actions.Add(
                generator => {
                    generator.UsingNamespace(usingNamespace);                    
                });

            return this;
        }

        public override IEmitter Comment(string comment) {
            return this;
        }

        public override IEmitter Defer(Action<IEmitter> action) {
            _actions.Add(
                generator => {
                    var emitter = new ILGeneratorEmitter(generator);
                    action(emitter);
                });

            return this;
        }

        public override void EmitIL(ILGenerator generator) {
            foreach (var action in this._actions) {
                action(generator);
            }
        }
    }
}