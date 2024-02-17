namespace Magicube.Core.Reflection {
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public interface IFieldBuilder {
        string FieldName { get; }

        Type FieldType { get; }

        FieldAttributes FieldAttributes { get; set; }

        IFieldBuilder Attributes(FieldAttributes attributes);

        FieldBuilder Define();
    }
}