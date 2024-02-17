namespace Magicube.Core.Reflection {
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public interface IConstructorBuilder {
        MethodAttributes MethodAttributes { get; set; }

        IEmitter Body();

        IConstructorBuilder CallingConvention(CallingConventions callingConvention);

        IConstructorBuilder Param(Type parameterType, string parameterName, ParameterAttributes attrs = ParameterAttributes.None);

        IConstructorBuilder Param(IGenericParameterBuilder genericParameter, string parameterName, ParameterAttributes attrs = ParameterAttributes.None);

        IConstructorBuilder Param(Action<IParameterBuilder> action);

        IConstructorBuilder Params(params Type[] parameterTypes);

        IConstructorBuilder SetMethodAttributes(MethodAttributes attributes);

        IConstructorBuilder SetImplementationFlags(MethodImplAttributes attributes);

        ConstructorBuilder Define();
    }
}