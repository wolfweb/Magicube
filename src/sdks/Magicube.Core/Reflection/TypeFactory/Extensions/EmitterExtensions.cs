using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection {
    public static class EmitterExtensions {
        private static readonly MethodInfo DisposeMethodInfo = typeof(IDisposable).GetMethod("Dispose");

        private static readonly MethodInfo TypeGetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");

        private static readonly MethodInfo TypeFactoryGetType = typeof(TypeFactory).GetMethod("GetType", new Type[] { typeof(string), typeof(bool) });

        private static readonly MethodInfo MethodBaseGetMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) });

        private static readonly MethodInfo MethodBaseGetMethodFromHandleGeneric = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) });

        private static readonly MethodInfo ObjectGetType = typeof(object).GetMethod("GetType");

        private static readonly MethodInfo TypeGetType = typeof(Type).GetMethod("GetType", new[] { typeof(string), typeof(bool) });

        private static readonly MethodInfo TypeIsAssignableFrom = typeof(Type).GetMethod("IsAssignableFrom");

        public static IEmitter WriteLineLoc(this IEmitter emitter, ILocal local) {
            return emitter.EmitWriteLine(local);
        }

        public static IEmitter Newobj(this IEmitter emitter, ConstructorInfo ctor) {
            return emitter.Emit(OpCodes.Newobj, ctor);
        }

        public static IEmitter Newobj(this IEmitter emitter, IConstructorBuilder ctorBuilder) {
            return emitter.Emit(OpCodes.Newobj, ctorBuilder.Define());
        }

        public static IEmitter Nop(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Nop);
        }

        public static IEmitter Dup(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Dup);
        }

        public static IEmitter Not(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Not);
        }

        public static IEmitter And(this IEmitter emitter) {
            return emitter.Emit(OpCodes.And);
        }

        public static IEmitter Or(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Or);
        }

        public static IEmitter Xor(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Xor);
        }

        public static IEmitter Pop(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Pop);
        }

        public static IEmitter Ret(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Ret);
        }

        public static IEmitter SizeOf(this IEmitter emitter, Type valueType) {
#if NETSTANDARD1_6
            if (valueType.GetTypeInfo().IsValueType == false)
#else
            if (valueType.IsValueType == false)
#endif
            {
                throw new InvalidProgramException("SizeOf instruction must take a value type");
            }

            return emitter.Emit(OpCodes.Sizeof);
        }

        public static IEmitter Break(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Break);
        }

        public static IEmitter CastClass<T>(this IEmitter emitter) {
            return emitter.CastClass(typeof(T));
        }

        public static IEmitter CastClass(this IEmitter emitter, IGenericParameterBuilder genericParameter) {
            return emitter.Defer(
                e => {
                    e.CastClass(genericParameter.AsType());
                });
        }

        public static IEmitter CastClass(this IEmitter emitter, Type castToType) {
            return emitter.Emit(OpCodes.Castclass, castToType);
        }

        public static IEmitter Constrained<T>(this IEmitter emitter) {
            return emitter.Constrained(typeof(T));
        }

        public static IEmitter Constrained(this IEmitter emitter, Type constrainedType) {
            return emitter.Emit(OpCodes.Constrained, constrainedType);
        }

        public static IEmitter Switch(this IEmitter emitter, params ILabel[] labels) {
            return emitter.Emit(OpCodes.Switch, labels);
        }

        public static IEmitter TailCall(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Tailcall);
        }

        public static IEmitter Unaligned(this IEmitter emitter, ILabel label) {
            return emitter.Emit(OpCodes.Unaligned, label);
        }

        public static IEmitter Unaligned(this IEmitter emitter, byte alignment) {
            return emitter.Emit(OpCodes.Unaligned, alignment);
        }

        public static IEmitter LocalAlloc(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Localloc);
        }

        public static IEmitter Box<T>(this IEmitter emitter)
            where T : struct {
            return emitter.Box(typeof(T));
        }

        public static IEmitter Box(this IEmitter emitter, Type valueType) {
#if NETSTANDARD1_6
            if (valueType.GetTypeInfo().IsValueType == false)
#else
            if (valueType.IsValueType == false)
#endif
            {
                throw new InvalidProgramException("Box instruction must take a value type");
            }

            return emitter.Emit(OpCodes.Box, valueType);
        }

        public static IEmitter Box(this IEmitter emitter, Type refType, ILocal localValue) {
            return emitter
                .LdLoc(localValue)
                .Box(refType);
        }

        public static IEmitter Unbox<T>(this IEmitter emitter)
            where T : struct {
            return emitter.Unbox(typeof(T));
        }

        public static IEmitter Unbox(this IEmitter emitter, Type valueType) {
#if NETSTANDARD1_6
            if (valueType.GetTypeInfo().IsValueType == false)
#else
            if (valueType.IsValueType == false)
#endif
            {
                throw new InvalidProgramException("Unbox instruction must take a value type");
            }

            return emitter.Emit(OpCodes.Unbox, valueType);
        }

        public static IEmitter UnboxAny<T>(this IEmitter emitter)
            where T : struct {
            return emitter.UnboxAny(typeof(T));
        }

        public static IEmitter UnboxAny(this IEmitter emitter, Type valueType) {
#if NETSTANDARD1_6
            if (valueType.GetTypeInfo().IsValueType == false)
#else
            if (valueType.IsValueType == false)
#endif
            {
                throw new InvalidProgramException("Unbox instruction must take a value type");
            }

            return emitter.Emit(OpCodes.Unbox_Any, valueType);
        }

        public static IEmitter MkRefAny(this IEmitter emitter, Type refType) {
            return emitter.Emit(OpCodes.Mkrefany, refType);
        }

        public static IEmitter LocalAlloc(this IEmitter emitter, int length) {
            return emitter
                .LdcI4(length)
                .LocalAlloc();
        }

        public static IEmitter InitBlk(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Initblk);
        }

        public static IEmitter InitObj<T>(this IEmitter emitter)
            where T : struct {
            return emitter.InitObj(typeof(T));
        }

        public static IEmitter InitObj(this IEmitter emitter, IGenericParameterBuilder genericParameter) {
            return emitter.Defer(
                e => {
                    e.InitObj(genericParameter.AsType());
                });
        }

        public static IEmitter InitObj(this IEmitter emitter, Type valueType) {
#if NETSTANDARD1_6
            if (valueType.GetTypeInfo().IsValueType == false)
#else
            if (valueType.IsValueType == false)
#endif
            {
                throw new InvalidProgramException("InitObj instruction must take a value type");
            }

            return emitter.Emit(OpCodes.Initobj, valueType);
        }

        public static IEmitter IsInst(this IEmitter emitter, Type type) {
            return emitter.Emit(OpCodes.Isinst, type);
        }

        public static IEmitter ArgList(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Arglist);
        }

        public static IEmitter Leave(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Leave, target);
        }

        public static IEmitter LeaveS(this IEmitter emitter, ILabel target) {
            return emitter.Emit(OpCodes.Leave_S, target);
        }

        public static IEmitter LdFunc(this IEmitter emitter, MethodInfo methodInfo) {
            return emitter.Emit(OpCodes.Ldftn, methodInfo);
        }

        public static IEmitter LdFunc(this IEmitter emitter, IMethodBuilder methodBuilder) {
            return emitter.LdFunc(methodBuilder.Define());
        }

        public static IEmitter LdVirtFunc(this IEmitter emitter, MethodInfo methodInfo) {
            return emitter.LdVirtFunc(methodInfo);
        }

        public static IEmitter LdVirtFunc(this IEmitter emitter, IMethodBuilder methodBuilder) {
            return emitter.Emit(OpCodes.Ldvirtftn, methodBuilder.Define());
        }

        public static IEmitter Jmp(this IEmitter emitter, MethodInfo methodInfo) {
            return emitter.Emit(OpCodes.Jmp, methodInfo);
        }

        public static IEmitter Call(this IEmitter emitter, Func<MethodInfo> action) {
            return emitter.EmitCall(OpCodes.Call, action);
        }

        public static IEmitter Call(this IEmitter emitter, MethodInfo method) {
            return emitter.Emit(OpCodes.Call, method);
        }

        public static IEmitter Call(this IEmitter emitter, IMethodBuilder method) {
            return emitter.Call(method.Define());
        }

        public static IEmitter Call<T>(this IEmitter emitter, string methodName, params Type[] argumentTypes) {
            return emitter.Call(typeof(T), methodName, argumentTypes);
        }

        public static IEmitter Call(this IEmitter emitter, Type type, string methodName, params Type[] argumentTypes) {
            return emitter.Emit(OpCodes.Call, type.GetMethodWithOptionalTypes(methodName, argumentTypes));
        }

        public static IEmitter Call(this IEmitter emitter, Type type, string methodName, IEnumerable<IGenericParameterBuilder> genArgs, params Type[] argumentTypes) {
            var methodInfo = type.GetMethodWithOptionalTypes(methodName, argumentTypes).MakeGenericMethod(genArgs.Select(g => g.AsType()).ToArray());
            return emitter.Emit(OpCodes.Call, methodInfo);
        }

        public static IEmitter Call(this IEmitter emitter, IMethodBuilder method, params ILocal[] locals) {
            return emitter.Call(
                method.Define(),
                locals);
        }

        public static IEmitter Call(this IEmitter emitter, MethodInfo method, params ILocal[] locals) {
            foreach (var local in locals) {
                emitter.LdLoc(local);
            }

            return emitter.Call(method);
        }

        public static IEmitter Call(this IEmitter emitter, Type genericTypeDefinition, IGenericParameterBuilder[] genericTypeArgs, string methodName, Type[] argumentTypes = null) {
            return emitter.Call(
                () => {
                    var argTypes = genericTypeArgs.Select(t => t.AsType());
                    var genericType = genericTypeDefinition.MakeGenericType(argTypes.ToArray());

                    return genericTypeDefinition.GetMethodWithOptionalTypes(methodName, argumentTypes);
                });
        }

        public static IEmitter Call(this IEmitter emitter, IConstructorBuilder ctor) {
            return emitter
                .Call(ctor.Define());
        }

        public static IEmitter Call(this IEmitter emitter, ConstructorInfo ctor) {
            return emitter.Emit(OpCodes.Call, ctor);
        }

        public static IEmitter Call(this IEmitter emitter, IConstructorBuilder ctor, params ILocal[] locals) {
            return emitter
                .Call(ctor.Define(), locals);
        }

        public static IEmitter Call(this IEmitter emitter, ConstructorInfo ctor, params ILocal[] locals) {
            foreach (var local in locals) {
                emitter.LdLoc(local);
            }

            return emitter.Call(ctor);
        }

        public static IEmitter Calli(this IEmitter emitter, IMethodBuilder method) {
            return emitter.Calli(method);
        }

        public static IEmitter Calli(this IEmitter emitter, MethodInfo method) {
            return emitter.Emit(OpCodes.Calli, method);
        }

        public static IEmitter CallVirt(this IEmitter emitter, IMethodBuilder method) {
            return emitter.CallVirt(method.Define());
        }

        public static IEmitter CallVirt(this IEmitter emitter, MethodInfo method) {
            return emitter.Emit(OpCodes.Callvirt, method);
        }

        public static IEmitter Inc(this IEmitter emitter, ILocal local) {
            return emitter
                .LdLoc(local)
                .LdcI4_1()
                .Add()
                .StLoc(local);
        }

        public static IEmitter Dec(this IEmitter emitter, ILocal local) {
            return emitter
                .LdLoc(local)
                .LdcI4_1()
                .Sub()
                .StLoc(local);
        }

        public static IEmitter EmitTypeOf<T>(this IEmitter emitter) {
            return emitter.EmitTypeOf(typeof(T));
        }

        public static IEmitter EmitTypeOf(this IEmitter emitter, Type type) {
            return emitter
                .LdToken(type)
                .Call(TypeGetTypeFromHandle);
        }

        public static IEmitter EmitIsAssignableFrom<T>(this IEmitter emitter, ILocal local) {
            return emitter.EmitIsAssignableFrom(typeof(T), local);
        }

        public static IEmitter EmitIsAssignableFrom(this IEmitter emitter, Type from, ILocal local) {
            return emitter
                .EmitTypeOf(from)
                .LdLocA(local)
                .Constrained(local.LocalType)
                .CallVirt(ObjectGetType)
                .CallVirt(TypeIsAssignableFrom);
        }

        public static IEmitter EmitIsAssignableFrom(this IEmitter emitter, Type from, Type to) {
            return emitter
                .EmitTypeOf(from)
                .EmitTypeOf(to)
                .CallVirt(TypeIsAssignableFrom);
        }

        public static IEmitter EmitMethod(this IEmitter emitter, MethodInfo methodInfo) {
            return emitter
                .LdToken(methodInfo)
                .Call(MethodBaseGetMethodFromHandle);
        }

        public static IEmitter EmitMethod(this IEmitter emitter, MethodInfo methodInfo, Type declaringType) {
            return emitter
                .LdToken(methodInfo)
                .LdToken(declaringType)
                .Call(MethodBaseGetMethodFromHandleGeneric);
        }

        public static IEmitter Using(this IEmitter emitter, ILocal disposableObj, Action generateBlock) {
            // Try
            emitter.BeginExceptionBlock(out ILabel beginBlock);

            generateBlock();

            // Finally
            return emitter
                .BeginFinallyBlock()
                .DefineLabel(out ILabel endFinally)

                .LdLoc(disposableObj)
                .BrFalseS(endFinally)
                .LdLoc(disposableObj)
                .CallVirt(DisposeMethodInfo)
                .Nop()
                .MarkLabel(endFinally)
                .EndExceptionBlock();
        }

        public static IEmitter StringFormat(this IEmitter emitter, string format, params ILocal[] locals) {
            return emitter
                .DeclareLocal(typeof(object), out ILocal localArray)
                .Array(
                    localArray,
                    locals.Length,
                    (index) => {
                        emitter.LdLoc(locals[index]);
#if NETSTANDARD1_6
                        if (locals[index].LocalType.GetTypeInfo().IsValueType == true)
#else
                        if (locals[index].LocalType.IsValueType == true)
#endif
                        {
                            emitter.Emit(OpCodes.Box, locals[index].LocalType);
                        }
                    })
                .LdStr(format)
                .LdLoc(localArray)
                .Call(typeof(string)
                    .GetMethod("Format", new Type[] { typeof(string), typeof(object[]) }));
        }

        public static IEmitter For(
            this IEmitter emitter,
            ILocal localLength,
            Action<ILocal> action) {
            return emitter
                .For(
                    localLength,
                    (IEmitter il, ILocal index) => action(index));
        }

        public static IEmitter For(
            this IEmitter emitter,
            ILocal localLength,
            Action<IEmitter, ILocal> action) {
            emitter
                .DefineLabel(out ILabel beginLoop)
                .DefineLabel(out ILabel loopCheck)
                .DeclareLocal<int>("index", out ILocal index)

                .LdcI4_0()
                .StLoc(index)
                .Br(loopCheck)
                .MarkLabel(beginLoop);

            action(emitter, index);

            return emitter
                .Nop()
                .LdLoc(index)
                .LdcI4_1()
                .Add()
                .StLoc(index)

                .MarkLabel(loopCheck)
                .LdLoc(index)
                .LdLoc(localLength)
                .Blt(beginLoop);
        }

        public static IEmitter For(
            this IEmitter emitter,
            ILocal localArray,
            Action<ILocal, ILocal> action) {
            return emitter
                .For(
                    localArray,
                    (il, index, item) => action(index, item));
        }

        public static IEmitter For(
            this IEmitter emitter,
            ILocal localArray,
            Action<IEmitter, ILocal, ILocal> action) {
            return emitter
                .DeclareLocal(localArray.LocalType.GetElementType(), "item", out ILocal itemLocal)
                .DeclareLocal<int>("length", out ILocal lengthLocal)

                .LdLoc(localArray)
                .LdLen()
                .ConvI4()
                .StLocS(lengthLocal)

                .For(
                    lengthLocal,
                    (index) => {
                        emitter
                            .LdLoc(localArray)
                            .LdLoc(index)
                            .LdElemRef()
                            .StLoc(itemLocal)
                            .Nop();

                        action(emitter, index, itemLocal);
                    });
        }

        public static IEmitter ForEach(
            this IEmitter emitter,
            ILocal localEnumerable,
            Action<ILocal> action) {
            return emitter
                .ForEach(
                    localEnumerable,
                    (item, breakAction) => action(item));
        }

        public static IEmitter ForEach(
            this IEmitter emitter,
            ILocal localEnumerable,
            Action<IEmitter, ILocal> action) {
            return emitter
                .ForEach(
                    localEnumerable,
                    (il, item, breakAction) => action(il, item));
        }

        public static IEmitter ForEach(
            this IEmitter emitter,
            ILocal localEnumerable,
            Action<ILocal, Action> action) {
            return emitter
                .ForEach(
                    localEnumerable,
                    (il, item, breakLoop) => action(item, breakLoop));
        }

        public static IEmitter ForEach(
            this IEmitter emitter,
            ILocal localEnumerable,
            Action<IEmitter, ILocal, Action> action) {
            emitter
                .DefineLabel("loopEnd", out ILabel loopEnd);

            var localType = localEnumerable.LocalType;
            if (localType.IsArray == true) {
                emitter.For(
                    localEnumerable,
                    (item) => {
                        action(emitter, item, () => emitter.Br(loopEnd));
                    });
            }
#if NETSTANDARD1_6
            else if (localType.GetTypeInfo().IsGenericType == false ||
#else
            else if (localType.IsGenericType == false ||
#endif
                typeof(IEnumerable<>).MakeGenericType(localType.GetGenericArguments()).IsAssignableFrom(localEnumerable.LocalType) == false) {
                throw new InvalidOperationException("Not a enumerable type");
            } else {
                var enumerableType = localType.GetGenericArguments()[0];
                var enumeratorType = typeof(IEnumerator<>).MakeGenericType(enumerableType);

                var getEnumerator = typeof(IEnumerable<>).MakeGenericType(enumerableType).GetMethod("GetEnumerator");
                var getCurrent = enumeratorType.GetProperty("Current").GetGetMethod();
                var moveNext = typeof(IEnumerator).GetMethod("MoveNext");

                emitter
                    .DefineLabel("loopStart", out ILabel loopStart)
                    .DefineLabel("loopCheck", out ILabel loopCheck)
                    .DefineLabel("endFinally", out ILabel endFinally)

                    .DeclareLocal(enumeratorType, "localEnumerator", out ILocal localEnumerator)
                    .DeclareLocal(enumerableType, "localItem", out ILocal localItem)

                    .LdLocS(localEnumerable)
                    .CallVirt(getEnumerator)
                    .StLocS(localEnumerator)

                    .Try(out ILabel beginEx)

                    .Br(loopCheck)
                    .MarkLabel(loopStart)
                    .LdLoc(localEnumerator)
                    .CallVirt(getCurrent)
                    .StLocS(localItem)
                    .Nop();

                action(emitter, localItem, () => emitter.Leave(loopEnd));

                emitter
                    .Nop()
                    .MarkLabel(loopCheck)
                    .LdLoc(localEnumerator)
                    .CallVirt(moveNext)
                    .BrTrue(loopStart)

                    .Leave(loopEnd)

                    .Finally()

                    .LdLoc(localEnumerator)
                    .BrFalse(endFinally)

                    .LdLoc(localEnumerator)
                    .CallVirt(DisposeMethodInfo)

                    .Nop()
                    .MarkLabel(endFinally)

                    .EndExceptionBlock();
            }

            emitter
                .Nop()
                .MarkLabel(loopEnd);

            return emitter;
        }

        public static IEmitter DeclareLocal<T>(this IEmitter emitter, out ILocal local) {
            return emitter.DeclareLocal(typeof(T), out local);
        }

        public static IEmitter DeclareLocal<T>(this IEmitter emitter, string localName, out ILocal local) {
            return emitter.DeclareLocal(typeof(T), localName, out local);
        }

        public static IEmitter DeclareLocal<T>(this IEmitter emitter, string localName, bool pinned, out ILocal local) {
            return emitter.DeclareLocal(typeof(T), localName, pinned, out local);
        }

        public static IEmitter DeclareLocal<T>(this IEmitter emitter, bool pinned, out ILocal local) {
            return emitter.DeclareLocal(typeof(T), pinned, out local);
        }

        public static IEmitter Try(this IEmitter emitter) {
            return emitter.BeginExceptionBlock(out ILabel label);
        }

        public static IEmitter Try(this IEmitter emitter, out ILabel label) {
            return emitter.BeginExceptionBlock(out label);
        }

        public static IEmitter Catch<TException>(this IEmitter emitter)
            where TException : Exception {
            return emitter.Catch(typeof(TException));
        }

        public static IEmitter Catch(this IEmitter emitter, Type exceptionType) {
            return emitter.BeginCatchBlock(exceptionType);
        }

        public static IEmitter Catch(this IEmitter emitter, Type exceptionType, ILocal local) {
            if (typeof(Exception).IsAssignableFrom(local.LocalType) == false) {
                throw new InvalidOperationException("Local must be an exception type");
            }

            return emitter
                .Catch(exceptionType)
                .StLoc(local);
        }

        public static IEmitter Finally(this IEmitter emitter) {
            return emitter.BeginFinallyBlock();
        }

        public static IEmitter Finally(this IEmitter emitter, Action<IEmitter> block) {
            emitter.BeginFinallyBlock();
            block(emitter);
            return emitter.EndExceptionBlock();
        }

        public static IEmitter Fault(this IEmitter emitter) {
            return emitter.BeginFaultBlock();
        }

        public static IEmitter Filter(this IEmitter emitter) {
            return emitter.BeginExceptFilterBlock();
        }

        public static IEmitter Throw(this IEmitter emitter) {
            return emitter.Emit(OpCodes.Throw);
        }

        public static IEmitter ThrowException<TException>(
            this IEmitter emitter,
            string message)
            where TException : Exception {
            ConstructorInfo ctor =
                typeof(TException)
                .GetConstructor(new[] { typeof(string) });

            if (ctor == null) {
                throw new ArgumentException("Type TException does not have a public constructor that takes a string argument");
            }

            emitter
                .LdStr(message)
                .Newobj(ctor)
                .Throw();

            return emitter;
        }

        public static IEmitter GetType(
            this IEmitter emitter,
            ILocal typeNameLocal,
            bool dynamicOnly = false) {
            return emitter
                .LdLocS(typeNameLocal)
                .Emit(dynamicOnly == false ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1)
                .Call(TypeFactoryGetType);
        }

        public static IEmitter GetType(
            this IEmitter emitter,
            string typeName,
            bool dynamicOnly = false) {
            return emitter
                .LdStr(typeName)
                .Emit(dynamicOnly == false ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1)
                .Call(TypeFactoryGetType);
        }
    }
}