namespace Magicube.Core.Reflection {
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public interface IGenericParameterBuilder {
        GenericParameterAttributes Attributes { get; set; }

        string ParameterName { get; }

        IGenericParameterBuilder BaseType<T>();

        IGenericParameterBuilder BaseType(Type baseType);

        IGenericParameterBuilder InterfaceType<T>();

        IGenericParameterBuilder InterfaceType(Type interfaceType);

        IGenericParameterBuilder Covariant();

        IGenericParameterBuilder Contravariant();

        IGenericParameterBuilder DefaultConstructor();

        IGenericParameterBuilder NotNullableValueType();

        IGenericParameterBuilder ReferenceType();

        Type AsType();
    }
}