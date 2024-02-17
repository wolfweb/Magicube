using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection.Emitters {
    internal abstract class EmitterBase : IEmitter {
        public abstract int ILOffset { get; }

        public abstract IEmitter BeginCatchBlock(Type exceptionType);

        public abstract IEmitter BeginExceptFilterBlock();

        public abstract IEmitter BeginExceptionBlock(out ILabel label);

        public abstract IEmitter BeginExceptionBlock(ILabel label);

        public abstract IEmitter BeginFaultBlock();

        public abstract IEmitter BeginFinallyBlock();

        public abstract IEmitter BeginScope();

        public abstract IEmitter Comment(string comment);

        public abstract IEmitter DeclareLocal(Type localType, out ILocal local);

        public abstract IEmitter DeclareLocal(Type localType, string localName, out ILocal local);

        public abstract IEmitter DeclareLocal(Type localType, bool pinned, out ILocal local);

        public abstract IEmitter DeclareLocal(Type localType, string localName, bool pinned, out ILocal local);

        public abstract IEmitter DeclareLocal(IGenericParameterBuilder genericType, out ILocal local);

        public abstract IEmitter DeclareLocal(Type localGenericTypeDefinition, IGenericParameterBuilder[] genericTypeArgs, out ILocal local);

        public abstract IEmitter DeclareLocal(ITypeBuilder typeBuilder, out ILocal local);

        public abstract IEmitter DeclareLocal(ILocal local);

        public abstract IEmitter DefineLabel(out ILabel label);

        public abstract IEmitter DefineLabel(string labelName, out ILabel label);

        public abstract IEmitter DefineLabel(ILabel label);

        public abstract IEmitter Emit(OpCode opcode, Type type);

        public abstract IEmitter Emit(OpCode opcode, string str);

        public abstract IEmitter Emit(OpCode opcode, float arg);

        public abstract IEmitter Emit(OpCode opcode, sbyte arg);

        public abstract IEmitter Emit(OpCode opcode, MethodInfo meth);

        public abstract IEmitter Emit(OpCode opcode, FieldInfo field);

        public abstract IEmitter Emit(OpCode opcode, IFieldBuilder field);

        public abstract IEmitter Emit(OpCode opcode, ILabel[] labels);

        public abstract IEmitter Emit(OpCode opcode, SignatureHelper signature);

        public abstract IEmitter Emit(OpCode opcode, ILocal local);

        public abstract IEmitter Emit(OpCode opcode, ConstructorInfo con);

        public abstract IEmitter Emit(OpCode opcode, long arg);

        public abstract IEmitter Emit(OpCode opcode, int arg);

        public abstract IEmitter Emit(OpCode opcode, short arg);

        public abstract IEmitter Emit(OpCode opcode, double arg);

        public abstract IEmitter Emit(OpCode opcode, byte arg);

        public abstract IEmitter Emit(OpCode opcode);

        public abstract IEmitter Emit(OpCode opcode, ILabel label);

        public abstract IEmitter EmitCall(OpCode opcode, Func<MethodInfo> action);

        public abstract IEmitter EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes);

        public abstract IEmitter EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes);

        public abstract IEmitter EmitWriteLine(FieldInfo fld);

        public abstract IEmitter EmitWriteLine(string value);

        public abstract IEmitter EmitWriteLine(ILocal local);

        public abstract IEmitter EndExceptionBlock();

        public abstract IEmitter EndScope();

        public abstract IEmitter MarkLabel(ILabel label);

        public abstract IEmitter ThrowException(Type excType);

        public abstract IEmitter UsingNamespace(string usingNamespace);

        public abstract IEmitter Defer(Action<IEmitter> action);

        public abstract void EmitIL(ILGenerator generator);
    }
}