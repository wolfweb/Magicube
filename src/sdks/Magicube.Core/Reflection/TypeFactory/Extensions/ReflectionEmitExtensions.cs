namespace Magicube.Core.Reflection {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using Magicube.Core.Reflection.Builders;
    using Magicube.Core.Reflection.Expressions;

    public static class ReflectionEmitExtensions {
        public static IMethodBuilder DefineGlobalMethod(
            this ModuleBuilder moduleBuilder,
            string methodName,
            Type returnType) {
            return moduleBuilder
                .DefineGlobalMethod(methodName)
                .Returns(returnType);
        }

        public static IMethodBuilder DefineGlobalMethod<TReturn>(
            this ModuleBuilder moduleBuilder,
            string methodName) {
            return moduleBuilder
                .DefineGlobalMethod(methodName)
                .Returns(typeof(TReturn));
        }

        public static IMethodBuilder DefineGlobalMethod(
            this ModuleBuilder moduleBuilder,
            string methodName) {
            Func<string, MethodAttributes, CallingConventions, Type, Type[], Type[], Type[], Type[][], Type[][], MethodBuilder> defineFunc =
                (name,
                attributes,
                callingConvention,
                returnType,
                returnTypeRequiredCustomModifiers,
                returnTypeOptionalCustomModifiers,
                parameterTypes,
                parameterTypeRequiredCustomModifiers,
                parameterTypeOptionalCustomModifiers) => {
                    return moduleBuilder.DefineGlobalMethod(
                        name,
                        attributes |= MethodAttributes.Static,
                        callingConvention,
                        returnType,
                        returnTypeRequiredCustomModifiers,
                        returnTypeOptionalCustomModifiers,
                        parameterTypes,
                        parameterTypeRequiredCustomModifiers,
                        parameterTypeOptionalCustomModifiers);
                };

            return new FluentMethodBuilder(methodName, defineFunc)
                .Static();
        }

        internal static void SetCustomAttributes(
            this IEnumerable<CustomAttributeBuilder> attrs,
            Action<CustomAttributeBuilder> action) {
            if (attrs != null) {
                foreach (var attr in attrs) {
                    action(attr);
                }
            }
        }

        public static IEmitter IfNotNull(this IEmitter emitter, ILocal local, Action<IEmitter> emitBody, Action<IEmitter> emitElse = null) {
            return emitter
                .Emit(OpCodes.Ldloc, local)
                .IfNotNull(emitBody, emitElse);
        }

        public static IEmitter IfNotNull(this IEmitter emitter, Action<IEmitter> emitBody, Action<IEmitter> emitElse = null) {
            emitter.DefineLabel("endif", out ILabel endIf);

            if (emitElse != null) {
                emitter
                    .DefineLabel("else", out ILabel notNull)
                    .Emit(OpCodes.Brtrue, notNull)
                    .Emit(OpCodes.Nop);

                emitElse(emitter);

                emitter
                    .Emit(OpCodes.Br, endIf)
                    .MarkLabel(notNull);

                emitBody(emitter);
            } else {
                emitter
                    .Emit(OpCodes.Brfalse, endIf)
                    .Emit(OpCodes.Nop);

                emitBody(emitter);
            }

            return emitter
                .MarkLabel(endIf);
        }

        public static IEmitter If(this IEmitter emitter, Expression<Func<IExpression, bool>> expression, Action<IEmitter> action, Action<IEmitter> elseAction = null) {
            var builder = new ExpressionBuilder(emitter);
            builder.EmitIF(expression, action, elseAction);
            return emitter;
        }

        public static IEmitter While(this IEmitter emitter, Expression<Func<IExpression, bool>> expression, Action<IEmitter> action) {
            var builder = new ExpressionBuilder(emitter);
            builder.EmitWhile(expression, action);
            return emitter;
        }

        public static IEmitter Do(this IEmitter emitter, Expression<Func<IExpression, bool>> expression, Action<IEmitter> action) {
            var builder = new ExpressionBuilder(emitter);
            builder.EmitDoWhile(expression, action);
            return emitter;
        }

        public static IEmitter For(this IEmitter emitter, Expression<Action<IInitialiser>> initialiser, Expression<Func<ICondition, bool>> condition, Expression<Action<IIterator>> iterator, Action<IEmitter> action) {
            var builder = new ExpressionBuilder(emitter);
            builder.EmitFor(initialiser, condition, iterator, action);
            return emitter;
        }
    }
}