namespace Magicube.Core.Reflection {
    using System;
    using System.Reflection.Emit;

    public interface IParameterBuilder {
        IParameterBuilder Type<T>();

        IParameterBuilder Type(Type parameterType);

        IParameterBuilder Name(string parameterName);

        IParameterBuilder In();

        IParameterBuilder Out();

        IParameterBuilder Lcid();

        IParameterBuilder Retval();

        IParameterBuilder Optional();

        IParameterBuilder HasDefault();

        IParameterBuilder HasFieldMarshal();

        IParameterBuilder SetCustomAttribute(CustomAttributeBuilder attributeBuilder);
    }
}