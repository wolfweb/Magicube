using Magicube.Core;
using Magicube.Core.Reflection;
using Magicube.Data.Abstractions.Validation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Magicube.Data.Abstractions {
    public class DynamicEntity : DynamicObject, IEntity<long> {
		private static readonly TypeFactory _typeFactory = new("Magicube.Data", "DynamicEntity");
		private static ConcurrentDictionary<string, DbTableComponent> dbTableCache = new();

		private readonly ConcurrentDictionary<string, object> dataMeta = new();

		public  long               Id        {
			get {
				return dataMeta.TryGetValue(Entity.IdKey, out object value) ? value.ConvertTo<long>() : ulong.MinValue.ConvertTo<long>();
			}
			set {
				dataMeta.AddOrUpdate(Entity.IdKey, value, (_, x) => value);
			}
		}

		public  string             TableName { get; }
		private DbTableComponent   DataRaw   { get; }
		public  JObject            Parts     { get; set; } = new();

		public DynamicEntity(IDbTable table) {
			TableName = table.Name;
			if (table.Fields == null) throw new DataException("dbtable fields required!");

			DataRaw = dbTableCache.GetOrAdd(TableName.ToLower(), k => {
				var it = new DbTableComponent(TableName);
				foreach (var field in table.Fields) {
					it.Fields.Add(field.Name, new FieldComponent(field.IsSort, field.IsFilter, field.AutoIncrement, field.Desc, field.ValidatorProviders, field.BindType));
				}
				it.GenerateType();
				return it;
			});
		}

		private DynamicEntity(DataRow row) {
			if (!dbTableCache.TryGetValue(row.Table.TableName.ToLower(), out DbTableComponent v))
				throw new DataException("should use DbTable initialize DictEntity before");
			TableName = v.TableName;
			DataRaw   = v;
			foreach (var field in DataRaw.Fields) {
				if (row.Table.Columns.Contains(field.Key))
					dataMeta.TryAdd(field.Key, row[field.Key]);
			}
		}

		private DynamicEntity(Type type) {
			TableName = type.Name;
			var fields = TypeAccessor.Get(type, null).Context.Properties;

			DataRaw = dbTableCache.GetOrAdd(TableName.ToLower(), k => {
				var it = new DbTableComponent(TableName, type);
				foreach (var field in fields) {
					it.Fields.Add(field.Member.Name, new FieldComponent(
						field.Member.PropertyType.IsValueType,
						field.Member.PropertyType.IsValueType,
						field.Attributes.Any(x => x is KeyAttribute) || field.Member.Name == Entity.IdKey,
						null,
						field.Attributes.Where(x => typeof(ValidationAttribute).IsAssignableFrom(x.GetType())).Select(x => {
							var typeAccessor = TypeAccessor.Get(x.GetType(), x).Context;

							return new DbFieldValidator {
								Provider = typeAccessor.Type.Name.Replace("Attribute",""),
                                Args = typeAccessor.Properties.Select(y => new KeyValuePair<string, object>(y.Member.Name, y.Member.GetValue(x))).ToDictionary(k => k.Key, v => v.Value)
                            };
						}).ToArray(),
						field.Member.PropertyType
						));
				}
				it.GenerateType();
				return it;
			});
		}

		public DynamicEntity(string tableName) {
			if (!dbTableCache.TryGetValue(tableName.ToLower(), out DbTableComponent v))
				throw new DataException("should use DbTable initialize DynamicEntity before");
			TableName = v.TableName;
			DataRaw   = v;
		}

		public IDictionary<string, FieldComponent> Fields => DataRaw.Fields;

        public PropertyInfo[]                      ExportProperties => DataRaw.ExportProperties;

        public object this [string key] {
            get {
				var field = EnsureValidField(key);
				return field.Value.GetValue(dataMeta.TryGetValue(field.Key, out object value) ? value : DefaultValue(key));
			}
            set {
				var field = DataRaw.Fields.SingleOrDefault(x => x.Key.ToLower() == key.ToLower());
				if (field.Key.IsNullOrEmpty()) throw new DataException("unknow field for dynamic entity");
				dataMeta.AddOrUpdate(field.Key, value, (_, x) => value);
			}
        }
		public object DefaultValue(string key) {
			var field = EnsureValidField(key);
			if (DataDefaultValueMapping.ValueMappings.TryGetValue(field.Value.Type, out object value)) return value;
			throw new DataException("unknow field type for dynamic entity");
		}

		private KeyValuePair<string, FieldComponent> EnsureValidField(string key) {
			var field = DataRaw.Fields.SingleOrDefault(x => x.Key.ToLower() == key.ToLower());
            if (field.Key.IsNullOrEmpty()) throw new DataException($"数据载体 {TableName} 不包含字段 {key}");
            return field;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result) {
			if (DataRaw.Fields.TryGetValue(binder.Name, out FieldComponent value)) {
				result = value.GetValue(dataMeta.ContainsKey(binder.Name) ? dataMeta[binder.Name] : null);
				return true;
			}
			result = null;
			return false;
		}
		public override bool TrySetMember(SetMemberBinder binder, object value) {
			if (DataRaw.Fields.ContainsKey(binder.Name)) {
				dataMeta[binder.Name] = value;
				return true;
			}

			return false;
		}

        public static implicit operator DynamicEntity(DataRow row) {
			return new DynamicEntity(row);
		}

		public static implicit operator DynamicEntity(Type type) {
			return new DynamicEntity(type);
        }

		public static implicit operator DynamicEntity(string tableName) {
			return new DynamicEntity(tableName);
        }

		public static implicit operator Dictionary<string, object>(DynamicEntity value) {
			var dict = new Dictionary<string, object>();
			foreach (var field in value.DataRaw.Fields) {
				if (field.Key == Entity.IdKey && value.Id == default) continue;

				dict.Add(field.Key, field.Value.GetValue(value.dataMeta.ContainsKey(field.Key) ? value.dataMeta[field.Key] : null));
			}
			return dict;
		}

		public static void Initialize(IDbTable[] tables) {
			foreach(var table in tables) {
				if (table.Fields == null) throw new DataException("dbtable fields required!");
				var it = new DbTableComponent(table.Name);
				foreach (var field in table.Fields) {
					it.Fields.Add(field.Name, new FieldComponent(field.IsSort, field.IsFilter, field.AutoIncrement, field.Desc, field.ValidatorProviders, field.BindType));
				}
				it.GenerateType();
				dbTableCache.TryAdd(it.TableName.ToLower(), it);
			}
        }

		sealed class DbTableComponent {
			public DbTableComponent(string tableName, Type type = null) {
				TableName   = tableName;
				Fields      = new Dictionary<string, FieldComponent>();
				DbTableType = type;
			}
			public string                              TableName        { get; }
			public IDictionary<string, FieldComponent> Fields           { get; }
			public Type                                DbTableType      { get; private set; }
            internal PropertyInfo[]                    ExportProperties { get; private set; }

            internal Task GenerateType() {
				return Task.Factory.StartNew(() => {
					if(DbTableType == null) {
						var builder = _typeFactory.NewType(TableName);

						foreach (var field in Fields) {
							var propertyBuilder = builder.NewProperty(field.Key, field.Value.Type);
                            if( field.Value.Validators != null) {
								foreach (var validator in field.Value.Validators) {
									var attr = DynamicEntityValidationFactory.BuildValidAttribute(validator);
									if (attr != null) {
										propertyBuilder.SetCustomAttribute(attr);
									}
								}
                            }
						}

						DbTableType = builder.CreateType();
                    }
					ExportProperties = DbTableType.GetProperties();
				});
            }
		}

		public sealed class FieldComponent {
			public FieldComponent(bool isSort, bool isFilter, bool isAutoIncr, string remark, DbFieldValidator[] validators, Type type) {
				Type       = type;
				IsSort     = isSort;
				Remark     = remark;
				IsFilter   = isFilter;
				Validators = validators;
				IsAutoIncr = isAutoIncr;
			}
			public Type               Type        { get; }
			public bool               IsSort      { get; }
			public bool               IsFilter    { get; }
			public bool               IsAutoIncr  { get; }
			public string             Remark      { get; }
			public DbFieldValidator[] Validators  { get; }
			public object GetValue(object v) {
				if (v != null) return v.ConvertTo(Type);
				if (DataDefaultValueMapping.ValueMappings.TryGetValue(Type, out object value)) return value;
				return null;
			}
		}
	}

	public class DbFieldValidator {
		public string                     ErrorMessage { get; set; } = "字段 {0} 无效";
		public string                     Provider     { get; set; }
		public IDictionary<string,object> Args         { get; set; }
    }
}
