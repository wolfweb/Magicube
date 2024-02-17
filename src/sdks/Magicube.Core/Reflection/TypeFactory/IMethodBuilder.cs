using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection {
    public interface IMethodBuilder {
        MethodAttributes Attributes { get; set; }

        IEmitter Body();

        IMethodBuilder Body(Action<IEmitter> action);

        IMethodBuilder MethodAttributes(MethodAttributes attributes);

        IMethodBuilder CallingConvention(CallingConventions convention);

        IMethodBuilder Returns<TReturn>();

        IMethodBuilder Returns(Type returnType);

        IMethodBuilder Returns(IGenericParameterBuilder genericType);

        IMethodBuilder Returns(Type genericTypeDefinition, params IGenericParameterBuilder[] genericTypes);

        IMethodBuilder Param<TParam>(string parameterName, ParameterAttributes attrs = ParameterAttributes.None);

        IMethodBuilder Param(Type parameterType, string parameterName, ParameterAttributes attrs = ParameterAttributes.None);

        IMethodBuilder Param(Action<IParameterBuilder> action);

        IMethodBuilder Param(IParameterBuilder parameter);

        IMethodBuilder Params(params Type[] parameterTypes);

        IMethodBuilder Params(params IParameterBuilder[] parameters);

        IParameterBuilder CreateParam<TParam>(string parameterName, ParameterAttributes attrs = ParameterAttributes.None);

        IParameterBuilder CreateParam(Type parameterType, string parameterName, ParameterAttributes attrs = ParameterAttributes.None);

        bool HasParameter(string parameterName);

        IParameterBuilder GetParameter(string parameterName);

        IGenericParameterBuilder NewGenericParameter(string parameterName);

        IMethodBuilder NewGenericParameter(string parameterName, Action<IGenericParameterBuilder> parameterBuilder);

        IMethodBuilder NewGenericParameters(params string[] parameterNames);

        IMethodBuilder NewGenericParameters(string[] parameterNames, Action<IGenericParameterBuilder[]> action);

        GenericTypeParameterBuilder GetGenericParameter(string parameterName);

        IMethodBuilder SetCustomAttribute(CustomAttributeBuilder customAttribute);

        IMethodBuilder SetImplementationFlags(MethodImplAttributes attributes);

        MethodBuilder Define();
    }
}