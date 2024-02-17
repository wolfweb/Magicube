using Magicube.Core;
using Magicube.Core.Models;
using Magicube.Core.Reflection;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Magicube.Data.Abstractions.ViewModel {
    public interface IEntityViewModel {
        object this[string key]                     { get; set; }

        PropertyComponentContext[] ExportProperties { get; }
    }

    public class EntityViewModel<TEntity, TKey> : DynamicObject, IEntityViewModel where TEntity : IEntity {
        static Dictionary<DataType, Type> DataTypeMappings = new() {
            [DataType.Custom]        = typeof(string),
            [DataType.Date]          = typeof(DateTime),
            [DataType.Html]          = typeof(string),
            [DataType.Time]          = typeof(DateTime),
            [DataType.Text]          = typeof(string),
            [DataType.Url]           = typeof(string),
            [DataType.Upload]        = typeof(string),
            [DataType.Currency]      = typeof(decimal),
            [DataType.DateTime]      = typeof(DateTime),
            [DataType.Duration]      = typeof(int),
            [DataType.Password]      = typeof(string),
            [DataType.ImageUrl]      = typeof(string),
            [DataType.CreditCard]    = typeof(string),
            [DataType.PostalCode]    = typeof(string),
            [DataType.PhoneNumber]   = typeof(string),
            [DataType.EmailAddress]  = typeof(string),
            [DataType.MultilineText] = typeof(string),
        };

        private readonly IEntityContext<TEntity> _entityContext;
        private readonly IDictionary<string,object> _dataKeeper = new Dictionary<string, object>();

        public EntityViewModel(TEntity entity = default) {
            if (entity == null) {
                if (typeof(TEntity) == typeof(DynamicEntity)) {
                    throw new DataException("动态类型必须先初始化");
                } else {
                    entity = New<TEntity>.Instance();
                }
            }

            if (typeof(TEntity) != typeof(DynamicEntity)) {
                _entityContext = new EntityContext(this, entity);
            } else {
                _entityContext = new DynamicEntityContext(this, entity as DynamicEntity) as IEntityContext<TEntity>;
            }
        }

        [Key]
        public TKey Id { get; set; }

        [IgnoreDataMember]
        public object this[string key] {
            get {
                var component = _entityContext.Components.FirstOrDefault(x => x.Property.Name == key);

                if (component != null) {
                    if (component.Override != null) {
                        return component.Override.Property.Member.GetValue(component.Override.ViewModel);
                    }
                    else if (_entityContext.TryGetValue(component.Property.Name, out var v))
                        return v;
                }

                return default;
            } 
            set {
                var component = _entityContext.Components.FirstOrDefault(x => x.Property.Name == key);
                if (component != null) {
                    if (component.Override != null) {
                        if (component.Override.Property.Member.PropertyType == typeof(ValueObject)) {
                            var convertAttr = component.Override.Property.GetAttribute<DataTypeAttribute>();
                            var result = new ValueObject(value);
                            if (convertAttr != null) {
                                result = new ValueObject(result.ConvertTo(DataTypeMappings.ContainsKey(convertAttr.DataType) ? DataTypeMappings[convertAttr.DataType] : typeof(string)));
                            }
                            component.Override.Property.Member.SetValue(component.Override.ViewModel, result);
                        } else
                            component.Override.Property.Member.SetValue(component.Override.ViewModel, value.ConvertTo(component.Override.Property.Member.PropertyType));
                    } else {
                        _entityContext.TrySetValue(component.Property.Name, value.ConvertTo(component.Property.PropertyType));
                    }
                }
            } 
        }

        [IgnoreDataMember]
        public PropertyComponentContext[] ExportProperties => _entityContext.Components.OrderBy(x=>x.Order).ToArray();

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            if (_entityContext.TryGetValue(binder.Name, out result)) {
                _dataKeeper[binder.Name] = result;
                return true;
            }
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) {
            if( _entityContext.TrySetValue(binder.Name, value)) {
                _dataKeeper[binder.Name] = value;
                return true;
            }
            return false;
        }

        public TEntity Build() {
            return _entityContext.Build();
        }

        private void SetMember(string key, object value) {
            _dataKeeper[key] = value;
        }

        sealed class EntityContext : IEntityContext<TEntity> {
            private readonly TEntity _entity;
            public EntityContext(EntityViewModel<TEntity, TKey> viewModel, TEntity entity) {
                _entity    = entity;
                Components = TypeAccessor.Get<TEntity>().Context.Properties.Select(x => x.Member).Select(CreateComponentContext).ToList();
                CombineOverrideProperties(viewModel);

                if (entity != null) { 
                    foreach(var item in Components) {
                        if (item.Entity.Equals(_entity)) {
                            var value = item.Property.GetValue(item.Entity);
                            viewModel.SetMember(item.Property.Name, value);
                            if (item.Override != null) {
                                if (item.Override.Property.Member.PropertyType.UnwrapNullable() == typeof(ValueObject)) {
                                    var convertAttr = item.Override.Property.GetAttribute<DataTypeAttribute>();
                                    var result = new ValueObject(value);
                                    if (convertAttr != null) {
                                        result = new ValueObject(result.ConvertTo(DataTypeMappings.ContainsKey(convertAttr.DataType) ? DataTypeMappings[convertAttr.DataType] : typeof(string)));
                                    }
                                    item.Override.Property.Member.SetValue(item.Override.ViewModel, result);
                                } else {
                                    item.Override.Property.Member.SetValue(item.Override.ViewModel, value);
                                }
                            }
                        }
                    }
                }
            }

            public IList<PropertyComponentContext> Components { get; }

            public bool TrySetValue(string name, object value) {
                var component = Components.FirstOrDefault(x => x.Property.Name == name);
                if (component != null) {
                    ValidationContext ctx;
                    if (component.Override != null) {
                        ctx = new ValidationContext(component.Override.ViewModel, null, null);
                    } else {
                        ctx = new ValidationContext(component.Entity, null, null);
                    }
                    ctx.MemberName = name;
                    Validator.ValidateProperty(value, ctx);
                    component.Property.SetValue(component.Entity, value != null ? Convert.ChangeType(value, component.Property.PropertyType) : null);
                    return true;
                }
                return false;
            }

            public bool TryGetValue(string name, out object result) {
                var component = Components.FirstOrDefault(x => x.Property.Name == name);
                if (component != null) {
                    result = component.Property.GetValue(component.Entity);
                    return true;
                }
                result = default;
                return false;
            }

            public TEntity Build() {
                foreach (var component in Components) {
                    object value;
                    ValidationContext ctx;
                    if (component.Override != null) {
                        value = component.Override.Property.Member.GetValue(component.Override.ViewModel);
                        ctx = new ValidationContext(component.Override.ViewModel, null, null);
                    } else {
                        value = component.Property.GetValue(component.Entity);
                        ctx = new ValidationContext(component.Entity, null, null);
                    }

                    ctx.MemberName = component.Property.Name;
                    Validator.ValidateProperty(value, ctx);

                    if (component.Override != null) component.Property.SetValue(component.Entity, Convert.ChangeType(value, component.Property.PropertyType));
                }
                return _entity;
            }

            private void CombineOverrideProperties(object viewModel) {
                var properties = TypeAccessor.Get<object>(viewModel.GetType(), viewModel).Context.Properties;
                foreach (var property in properties) {
                    if (property.Attributes.Any(x => x is IgnoreDataMemberAttribute)) continue;
                    var overrideProperty = Components.FirstOrDefault(x => x.Property.Name == property.Member.Name);
                    if (overrideProperty != null) {
                        overrideProperty.Override = new ViewModelPropertyComponent {
                            Property  = property,
                            ViewModel = viewModel,
                        };
                    } else {
                        Components.Add(new PropertyComponentContext(viewModel, property.Member));
                    }
                }
            }

            private PropertyComponentContext CreateComponentContext(PropertyInfo property) {
                var ctx = new PropertyComponentContext(_entity, property);
                var attr = property.GetCustomAttribute<SortAttribute>();
                if (attr != null) {
                    ctx.Order = attr.Order;
                }
                return ctx;
            }
        }

        sealed class DynamicEntityContext : IEntityContext<DynamicEntity> {
            private readonly DynamicEntity _entity;

            public DynamicEntityContext(EntityViewModel<TEntity, TKey> viewModel, DynamicEntity entity) {
                _entity     = entity;
                Components = entity.ExportProperties.Select(property => new PropertyComponentContext (entity, property)).ToList();
                CombineOverrideProperties(viewModel);

                foreach (var item in Components) {
                    if (item.Entity.Equals(_entity)) {
                        var value = entity[item.Property.Name];
                        viewModel.SetMember(item.Property.Name, value);
                        if (item.Override != null) {
                            if (item.Override.Property.Member.PropertyType.UnwrapNullable() == typeof(ValueObject)) {
                                var convertAttr = item.Override.Property.GetAttribute<DataTypeAttribute>();
                                var result = new ValueObject(value);
                                if (convertAttr != null) {
                                    result = new ValueObject(result.ConvertTo(DataTypeMappings.ContainsKey(convertAttr.DataType) ? DataTypeMappings[convertAttr.DataType] : typeof(string)));
                                }
                                item.Override.Property.Member.SetValue(item.Override.ViewModel, result);
                            } else {
                                item.Override.Property.Member.SetValue(item.Override.ViewModel, value);
                            }
                        }
                    }
                }
            }

            public IList<PropertyComponentContext> Components { get; }

            public DynamicEntity Build() {
                foreach (var component in Components) {
                    object value;
                    if (component .Override!= null) {
                        value = component.Override.Property.Member.GetValue(component.Override.ViewModel);
                        var ctx = new ValidationContext(component.Override.ViewModel, null, null);
                        ctx.MemberName = component.Property.Name;
                        Validator.ValidateProperty(value, ctx);
                        _entity[component.Property.Name] = value;
                    } else {
                        var field = _entity.Fields.FirstOrDefault(x => x.Key == component.Property.Name);
                        if (field.Value.Validators != null && field.Value.Validators.Any()) {
                            DynamicEntityValidationFactory.Validator(field.Key, _entity[field.Key], field.Value.Validators);
                        }
                    }
                }

                return _entity;
            }

            private void CombineOverrideProperties(object viewModel) {
                var properties = TypeAccessor.Get<object>(viewModel.GetType(), viewModel).Context.Properties;
                foreach (var property in properties) {
                    if (property.Attributes.Any(x => x is IgnoreDataMemberAttribute)) continue;
                    var overrideProperty = Components.FirstOrDefault(x => x.Property.Name == property.Member.Name);
                    if (overrideProperty != null) {
                        overrideProperty.Override = new ViewModelPropertyComponent {
                            Property = property,
                            ViewModel = viewModel,
                        };
                    } else {
                        Components.Add(new PropertyComponentContext(viewModel, property.Member));
                    }
                }
            }

            public bool TryGetValue(string name, out object result) {
                result = _entity[name];
                return true;
            }

            public bool TrySetValue(string name, object value) {
                _entity[name] = value;
                return true;
            }
        }
    }
}
