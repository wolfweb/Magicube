namespace Magicube.Core.Reflection {
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public interface IDynamicMethodBuilder {
        IEmitter Body();

        IDynamicMethodBuilder Body(Action<IEmitter> action);

        IDynamicMethodBuilder Returns<TReturn>();

        IDynamicMethodBuilder Returns(Type returnType);

        IDynamicMethodBuilder Param<TParam>(string parameterName, ParameterAttributes attrs = ParameterAttributes.None);

        IDynamicMethodBuilder Param(Type parameterType, string parameterName, ParameterAttributes attrs = ParameterAttributes.None);

        IDynamicMethodBuilder Param(Action<IParameterBuilder> action);

        IDynamicMethodBuilder Param(IParameterBuilder parameter);

        IDynamicMethodBuilder Params(params Type[] parameterTypes);

        IDynamicMethodBuilder Params(params IParameterBuilder[] parameters);

        IParameterBuilder CreateParam<TParam>(string parameterName, ParameterAttributes attrs = ParameterAttributes.None);

        IParameterBuilder CreateParam(Type parameterType, string parameterName, ParameterAttributes attrs = ParameterAttributes.None);

        DynamicMethod Define();
    }
}