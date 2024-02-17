namespace Magicube.Core.Reflection {
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public interface IPropertyBuilder {
        PropertyAttributes PropertyAttributes { get; set; }

        IMethodBuilder SetMethod { get; set; }

        IMethodBuilder GetMethod { get; set; }

        IPropertyBuilder CallingConvention(CallingConventions callingConvention);

        IPropertyBuilder Attributes(PropertyAttributes attributes);

        IPropertyBuilder Getter(Action<IMethodBuilder> action = null);

        IMethodBuilder Getter();

        IPropertyBuilder Setter(Action<IMethodBuilder> action = null);

        IMethodBuilder Setter();

        PropertyBuilder Define();

        IPropertyBuilder SetCustomAttribute(CustomAttributeBuilder customAttribute);
    }
}