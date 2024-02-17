using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Magicube.Core.Reflection {
    public sealed class ReflectionContext {
        public ReflectionContext(Type type) {
            Parameters    = new List<IMemberExpressionModel>();
            Type          = type;
            TypeInfo      = type.GetTypeInfo();
            FullName      = type.FullName;
            Assembly      = type.Assembly;

            Attributes    = type.GetCustomAttributes();

            Fields        = type.GetFields().Where(x => !x.IsStatic).Select(member => new FieldInfoExplorer(member));
            Members       = type.GetMembers().Select(member=> new MemberInfoExplorer(member));
            Properties    = type.GetProperties().Select(member => new PropertyInfoExplorer(member));
            IsValueType   = TypeInfo.IsValueType;
            Constructors  = TypeInfo.GetConstructors().Select(x=>new ConstructorExplorer(x)).ToArray();

            foreach (var member in Members) {
                if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field) {
                    Parameters.Add(CreateExpressionModel(member.Member));
                }
            }
        }

        public Type                              Type          { get; }                              
        public Assembly                          Assembly      { get; }
        public string                            Name          { get; }
        public string                            FullName      { get; }
        public TypeInfo                          TypeInfo      { get; }
        public IEnumerable<Attribute>            Attributes    { get; }
        public bool                              IsValueType   { get; }
        public ConstructorExplorer[]             Constructors  { get; }

        public IEnumerable<FieldInfoExplorer>    Fields        { get; }
        public IEnumerable<MemberInfoExplorer>   Members       { get; }
        public IEnumerable<PropertyInfoExplorer> Properties    { get; }

        /// <summary>
        /// fields and properties
        /// </summary>
        public List<IMemberExpressionModel> Parameters { get; }

        private IMemberExpressionModel CreateExpressionModel(MemberInfo member) {
            return member.MemberType == MemberTypes.Property ? new PropertyExpressionModel((PropertyInfo)member) : new FieldExpressionModel((FieldInfo)member);
        }
    }

    public class ConstructorExplorer {
        public ConstructorExplorer(ConstructorInfo ctor) {
            Constructor = ctor;
            Parameters  = ctor.GetParameters();
            
            var parameters = Parameters.Select(x => Expression.Parameter(x.ParameterType, x.Name)).ToArray();
            Creator     = Expression.Lambda(Expression.New(ctor, parameters), parameters).Compile();
        }

        public Delegate        Creator     { get; }
        public ParameterInfo[] Parameters  { get; }
        public ConstructorInfo Constructor { get; }
    }

    public class TypeMemberExplorer<T> where T : MemberInfo {
        protected TypeMemberExplorer(T member) { 
            Member     = member;
            Attributes = member.GetCustomAttributes();
            MemberType = member.MemberType;
        }

        public TAttr GetAttribute<TAttr>() where TAttr : Attribute { 
            return Attributes.OfType<TAttr>().FirstOrDefault();
        }

        public IEnumerable<TAttr> GetAttributes<TAttr>() where TAttr : Attribute {
            return Attributes.OfType<TAttr>();
        }

        public IEnumerable<Attribute> Attributes { get; }
        public MemberTypes            MemberType { get; }
        public T                      Member     { get; }
    }

    public class MemberInfoExplorer : TypeMemberExplorer<MemberInfo>{
        public MemberInfoExplorer(MemberInfo member) : base(member) {
        }        
    }

    public class PropertyInfoExplorer : TypeMemberExplorer<PropertyInfo> {
        public PropertyInfoExplorer(PropertyInfo member) : base(member) {
            IsNullable = member.IsNullable();
        }

        public bool IsNullable { get; }
    }

    public class FieldInfoExplorer : TypeMemberExplorer<FieldInfo> {
        public FieldInfoExplorer(FieldInfo member) : base(member) {
            IsNullable = member.IsNullable();
        }

        public bool IsNullable { get; }
    }
}
