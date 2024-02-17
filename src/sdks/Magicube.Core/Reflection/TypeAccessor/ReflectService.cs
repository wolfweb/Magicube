using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Magicube.Core.Reflection {
    public class ReflectService<T> {
        private readonly ReflectionContext _reflectionContext;             
        private readonly object _instance;
        internal ReflectService(ReflectionContext ctx, object instance) {
            _instance = instance;
            _reflectionContext = ctx;
        }

        public ReflectionContext Context => _reflectionContext;

        public ConstructorExplorer GetConstructor(params Type[] types) {
            var ctors = Context.Constructors.Where(x => x.Parameters.Length == types.Length);
            foreach (var ctor in ctors) {
                var f = true;
                for (var i = 0; i < types.Length; i++) { 
                    if (ctor.Parameters[i].ParameterType != types[i]) {
                        f = false;
                        break;
                    }
                }
                if (f) return ctor;
            }
            return null;
        }

        public PropertyInfo      GetProperty(string name) => _reflectionContext.Properties.FirstOrDefault(x => x.Member.Name == name)?.Member;
                              
        public FieldInfo         GetField(string name)    => _reflectionContext.Fields.FirstOrDefault(x => x.Member.Name == name)?.Member;

        public IEnumerable<Type> ScanTypes<TTarget>() {
            return _reflectionContext.Assembly.ExportedTypes.Where(x => typeof(TTarget).IsAssignableFrom(x) && !x.IsAbstract);
        }

        public TTarget Copy<TTarget>() {
            if (_instance == null) return default;
            var instance = New<TTarget>.Instance();
            return Copy(instance);
        }

        public TTarget Copy<TTarget>(TTarget target) {
            var func = InternalCopy(target);
            func((T)_instance, target);
            return target;
        }

        private Action<T, TTarget> InternalCopy<TTarget>(TTarget target) {
            var targetType = target.GetType();
            ReflectionContext targetCtx;
            if (targetType != _reflectionContext.Type) {
                targetCtx = ReflectionContextCache.GetOrAdd(targetType);
            } else {
                targetCtx = _reflectionContext;
            }

            return new ObjectCopyer<T, TTarget>(_reflectionContext, targetCtx).Copy();
        }

        public void SetValue(FieldInfo property, object value, object instance = null) {
            var obj = instance ?? _instance;
            obj.NotNull();

            property.SetValue(obj, value);
        }

        public void SetValue(PropertyInfo property, object value ,object instance = null) {
            var obj = instance ?? _instance;
            obj.NotNull();

            if (property.CanWrite) {
                property.SetValue(obj, value);
            }
        }

        public object GetValue(PropertyInfo property, object instance = null) {
            var obj = instance ?? _instance;
            obj.NotNull();
            return property.GetValue(obj);
        }

        public object GetValue(FieldInfo field, object instance = null) {
            var obj = instance ?? _instance;
            obj.NotNull();
            return field.GetValue(obj);
        }

        sealed class ObjectCopyer<TSource,TTarget> {
            private static ConcurrentDictionary<TypeKey, Delegate> _delegateCache = new();

            private readonly ReflectionContext _source;
            private readonly ReflectionContext _target;

            public ObjectCopyer(ReflectionContext source, ReflectionContext target) {
                _source = source;
                _target = target;
            }

            public Action<TSource, TTarget> Copy() {
                var res = _delegateCache.GetOrAdd(new TypeKey(_source.Type, _target.Type), GenerateDelegate(_source, _target));
                return (Action<TSource, TTarget>)res;
            }

            private Delegate GenerateDelegate(ReflectionContext from, ReflectionContext to) {
                var source = Expression.Parameter(from.Type, "source");
                var target = Expression.Parameter(to.Type, "target");

                var blocks = new List<Expression>();
                foreach (var member in to.Parameters) {
                    var getter = PropertyOrFieldFn(source, member, from.Parameters);

                    if (getter != null) {
                        blocks.Add(member.SetExpression(target, getter));
                    }
                }

                var lambda = Expression.Lambda<Action<TSource, TTarget>>(Expression.Block(blocks), source, target);
                return lambda.Compile();
            }

            private Expression PropertyOrFieldFn(Expression source, IMemberExpressionModel destinationMember, IEnumerable<IMemberExpressionModel> members) {
                return members.Where(member => member.Name == destinationMember.Name && member.Type == destinationMember.Type)
                    .Select(member => member.GetExpression(source))
                    .FirstOrDefault();
            }

            readonly struct TypeKey : IEquatable<TypeKey> {
                public bool Equals(TypeKey other) {
                    return Source == other.Source && Destination == other.Destination;
                }

                public override bool Equals(object obj) {
                    if (obj is not TypeKey)
                        return false;
                    return Equals((TypeKey)obj);
                }

                public override int GetHashCode() {
                    return (Source.GetHashCode() << 16) ^ (Destination.GetHashCode() & 65535);
                }

                public static bool operator ==(TypeKey left, TypeKey right) {
                    return left.Equals(right);
                }

                public static bool operator !=(TypeKey left, TypeKey right) {
                    return !left.Equals(right);
                }

                public Type Source      { get; }
                public Type Destination { get; }

                public TypeKey(Type source, Type destination) {
                    Source      = source;
                    Destination = destination;
                }
            }
        }
    }
}
