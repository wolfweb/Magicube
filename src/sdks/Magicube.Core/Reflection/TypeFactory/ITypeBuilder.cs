namespace Magicube.Core.Reflection {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    public interface ITypeBuilder {
        string TypeName { get; }

        TypeAttributes TypeAttributes { get; set; }

        IEnumerable<Type> Interfaces { get; }

        ITypeBuilder Attributes(TypeAttributes attributes);

        ITypeBuilder InheritsFrom<T>();

        ITypeBuilder InheritsFrom(Type baseType);

        ITypeBuilder Implements<T>();

        ITypeBuilder Implements(Type interfaceType);

        IConstructorBuilder NewConstructor();

        ITypeBuilder NewConstructor(Action<IConstructorBuilder> constructorBuilder);

        ITypeBuilder NewDefaultConstructor(MethodAttributes constructorAttributes);

        IFieldBuilder NewField(string fieldName, Type fieldType);

        IFieldBuilder NewField(string fieldName, Type fieldType, params IGenericParameterBuilder[] genericParameters);

        IFieldBuilder NewField(string fieldName, IGenericParameterBuilder genericParameter);

        ITypeBuilder NewField(string fieldName, Type fieldType, Action<IFieldBuilder> fieldBuilder);

        ITypeBuilder NewMethod(string methodName, Action<IMethodBuilder> action);

        IMethodBuilder NewMethod(string methodName, MethodAttributes attributes, CallingConventions callingConvention, Type returnType);

        IMethodBuilder NewMethod(string methodName);

        IPropertyBuilder NewProperty(string propertyName, Type propertyType);

        ITypeBuilder NewProperty(string propertyName, Type propertyType, Action<IPropertyBuilder> propertyBuilder);

        IEventBuilder NewEvent(string eventName, Type eventType);

        ITypeBuilder NewEvent(string eventName, Type eventType, Action<IEventBuilder> eventBuilder);

        IGenericParameterBuilder NewGenericParameter(string parameterName);

        ITypeBuilder NewGenericParameter(string parameterName, Action<IGenericParameterBuilder> parameterBuilder);

        Type GetGenericParameterType(string parameterName);

        ITypeBuilder NewNestedType(string typeName);

        ITypeBuilder SetCustomAttribute(CustomAttributeBuilder customAttribute);

        TypeBuilder Define();

        Type CreateType();
    }
}