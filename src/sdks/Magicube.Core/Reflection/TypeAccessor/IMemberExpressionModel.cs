using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Magicube.Core.Reflection {
    public interface IMemberExpressionModel {
        string     Name { get; }
        Type       Type { get; }
        Expression GetExpression(Expression source);
        Expression SetExpression(Expression source, Expression value);
    }

    sealed class FieldExpressionModel : IMemberExpressionModel {
        private readonly FieldInfo _fieldInfo;

        public FieldExpressionModel(FieldInfo fieldInfo) {
            _fieldInfo = fieldInfo;
        }

        public string Name => _fieldInfo.Name;
        public Type   Type => _fieldInfo.FieldType;

        public Expression GetExpression(Expression source) {
            return Expression.Field(source, _fieldInfo);
        }
        public Expression SetExpression(Expression source, Expression value) {
            return Expression.Assign(GetExpression(source), value);
        }
    }

    sealed class PropertyExpressionModel : IMemberExpressionModel {
        private readonly PropertyInfo _propertyInfo;

        public PropertyExpressionModel(PropertyInfo propertyInfo) {
            _propertyInfo = propertyInfo;
        }

        public string Name => _propertyInfo.Name;

        public Type   Type => _propertyInfo.PropertyType;

        public Expression GetExpression(Expression source) {
            return Expression.Property(source, _propertyInfo);
        }
        public Expression SetExpression(Expression source, Expression value) {
            return Expression.Assign(GetExpression(source), value);
        }
    }
}
