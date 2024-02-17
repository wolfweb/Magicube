using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection.Builders {
    internal class FluentFieldBuilder : IFieldBuilder {
        private readonly Func<string, Type, Type[], Type[], FieldAttributes, FieldBuilder> _defineFunc;

        private FieldBuilder _fieldBuilder;

        public FluentFieldBuilder(
            string fieldName,
            Type fieldType,
            Func<string, Type, Type[], Type[], FieldAttributes, FieldBuilder> defineFunc) {
            _defineFunc = defineFunc;

            FieldName = fieldName;
            FieldType = fieldType;
            FieldAttributes = FieldAttributes.Private;
        }

        public string FieldName { get; }

        public Type FieldType { get; }

        public FieldAttributes FieldAttributes { get; set; }

        public IFieldBuilder Attributes(FieldAttributes attributes) {
            FieldAttributes = attributes;
            return this;
        }

        public FieldBuilder Define() {
            if (_fieldBuilder == null) {
                _fieldBuilder = _defineFunc(
                    FieldName,
                    FieldType,
                    null,
                    null,
                    FieldAttributes);
            }

            return _fieldBuilder;
        }
    }
}