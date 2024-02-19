using FluentMigrator.Builders;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.IfDatabase;
using FluentMigrator.Builders.Schema.Table;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Magicube.Core;
using Magicube.Core.Reflection;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using DataException = Magicube.Data.Abstractions.DataException;

namespace Magicube.Data.Migration {
    public class MigrationManager : IMigrationManager {
		#region 
		private static Dictionary<Type, Action<T>> BuildFieldMappings<T, TNext>() 
			where T : IColumnTypeSyntax<TNext>
			where TNext : IFluentSyntax {
			return new Dictionary<Type, Action<T>>() {
				[typeof(byte)]           = c => c.AsByte(),
				[typeof(byte[])]         = c => c.AsBinary(int.MaxValue),
				[typeof(bool)]           = c => c.AsBoolean(),
				[typeof(DateTime)]       = c => c.AsDateTime(),
				[typeof(decimal)]        = c => c.AsDecimal(18, 4),
				[typeof(double)]         = c => c.AsDouble(),
				[typeof(Guid)]           = c => c.AsGuid(),
				[typeof(short)]          = c => c.AsInt16(),
				[typeof(int)]            = c => c.AsInt32(),
				[typeof(long)]           = c => c.AsInt64(),
				[typeof(float)]          = c => c.AsFloat(),
				[typeof(string)]         = c => c.AsString(),
				[typeof(Type)]           = c => c.AsString(255),
				[typeof(JObject)]        = c => c.AsString(4000),
				[typeof(DateTimeOffset)] = c => c.AsDateTime(),
			};            
        }

		private static Dictionary<Type, Action<ICreateTableColumnAsTypeSyntax>> _typeMapping     = BuildFieldMappings<ICreateTableColumnAsTypeSyntax, ICreateTableColumnOptionOrWithColumnSyntax>();
		private static Dictionary<Type, Action<IAlterTableColumnAsTypeSyntax>> _typeAlterMapping = BuildFieldMappings<IAlterTableColumnAsTypeSyntax, IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax>();

        private static Dictionary<Type, DbType> PrimaryKeyTypeMapping = new() {            
            [typeof(Guid)]           = DbType.Guid,
            [typeof(int)]            = DbType.Int32,
            [typeof(long)]           = DbType.Int64,
            [typeof(string)]         = DbType.String
		};
        #endregion

        private readonly MigrationRunner _migrationRunner;
		private readonly IMigrationContext _migrationContext;
		private readonly IFilteringMigrationSource _filteringMigrationSource;
		private readonly SemaphoreSlim _semaphoreSlim = new(1);
        public MigrationManager(
			IMigrationRunner migrationRunner, 
			IMigrationContext migrationContext,
			IFilteringMigrationSource filteringMigrationSource
			) {
            _migrationRunner          = migrationRunner as MigrationRunner;
            _migrationContext         = migrationContext;
			_filteringMigrationSource = filteringMigrationSource;
			Database                  = new IfDatabaseExpressionRoot(_migrationContext, x => true);
		}

		public IIfDatabaseExpressionRoot Database { get; }

		public void BuildTable<T>() where T : Entity {
			var type = typeof(T);
			BuildTable(type, null);
        }

        public void BuildTable(Type type, IDbContext dbContext, bool useConf = false) {
			var tableName = GetTableName(type, dbContext, useConf);
            var schema = Database.Schema.Table(tableName);
			var fields = TypeAccessor.Get(type, null).Context.Properties;

			IEntityType entityType = null;
			if (useConf) {
                var dbCtx = dbContext as DbContext;
                entityType = dbCtx.Model.FindEntityType(type);
            }

            if (schema.Exists()) {
                var builder = Database.Alter.Table(tableName) as AlterTableExpressionBuilder;
                var keyProperties = GetPrimaryKey(type, dbContext, useConf);
                foreach (var prop in fields) {
                    var attr = prop.Member.GetCustomAttribute<NotMappedAttribute>();
                    if (attr != null) continue;
					if (keyProperties.Any(x => x.Name == prop.Member.Name)) continue;

					if (useConf) {
						var confProp = entityType.FindProperty(prop.Member);
                        DefineFields(tableName, prop, builder, schema, confProp);
                    } else {
						DefineFields(tableName, prop, builder, schema);
					}
                }
            } else {
                var builder = Database.Create.Table(tableName) as CreateTableExpressionBuilder;
                var expression = builder.Expression;
                var pks = BuildPrimaryKey(tableName, type);
                expression.Columns.AddRange(pks);

                foreach (var prop in fields) {
                    var attr = prop.Member.GetCustomAttribute<NotMappedAttribute>();
                    if (attr != null) continue;
                    if (pks.Any(x => x.Name == prop.Member.Name)) continue;

                    if (useConf) {
                        var confProp = entityType.FindProperty(prop.Member);
                        DefineFields(tableName, prop, builder, confProp);
                    }
					else {
                        DefineFields(tableName, prop, builder);
					}
                }
            }

            var tableAttrs = type.GetCustomAttributes<IndexAttribute>();
            if (tableAttrs.Any()) {
                foreach (var attr in tableAttrs) {
					var indexName = attr.Name;
					if (indexName.IsNullOrEmpty()) {
						indexName = $"{tableName}_{string.Join(",", attr.PropertyNames)}";
					}
					var indexBuilder = Database.Create.Index(indexName).OnTable(tableName);
                    foreach (var name in attr.PropertyNames) {
                        indexBuilder.OnColumn(name);
                    }
                }
            }

            Execute();
			_migrationRunner.CurrentScope.Complete();
		}

        public void BuildTable(IDbTable table) {
			var schema = Database.Schema.Table(table.Name);
			if (schema.Exists()) {
				var builder = Database.Alter.Table(table.Name) as AlterTableExpressionBuilder;
				foreach (var field in table.Fields) {
					if (schema.Column(field.Name).Exists()) {
						ParseColumn(field, builder, builder.AlterColumn(field.Name), schema);
					} else {
						ParseColumn(field, builder, builder.AddColumn(field.Name), schema);
					}
				}
			} else {
				var builder = Database.Create.Table(table.Name) as CreateTableExpressionBuilder;
				foreach (var field in table.Fields) {
					ParseColumn(field, builder);
				}
			}
			Execute();
			_migrationRunner.CurrentScope.Complete();
		}

		public void ParseColumn(DbField field, CreateTableExpressionBuilder builder) {
			var column = builder.WithColumn(field.Name);
			if (field.PrimaryKey || field.Name.Equals(Entity.IdKey, StringComparison.InvariantCultureIgnoreCase)) {
				if (field.BindType != null) {
					_typeMapping[field.BindType](column);
				}
				builder.PrimaryKey();
				if (field.AutoIncrement) builder.Identity();
			} else {
				if (field.Nullable) builder.Nullable();
				else builder.NotNullable();

				if (!field.DefaultValue.IsNullOrEmpty()) builder.WithDefaultValue(field.DefaultValue);
				if (field.Size > 0) {
					builder.AsString(field.Size);
                } else {
					if (!field.DbType.IsNullOrEmpty()) {
						var customDbType = field.DbType;
						if (field.Size > 0) customDbType += $"({field.Size})";
						column.AsCustom($"{customDbType}");
					} else if (field.BindType != null) {
						_typeMapping[field.BindType](column);
					} else {
						throw new DataException($"字段 {field.Name} 必须指定BindType或DbType类型");
					}
				}
				if (field.UniqueKey) {
					builder.Unique($"{field.Name}_Index");
				}
			}			
		}

		public void ParseColumn(DbField field, AlterTableExpressionBuilder builder, IAlterTableColumnAsTypeSyntax column, ISchemaTableSyntax schema) {
			if (field.PrimaryKey || field.Name.Equals(Entity.IdKey, StringComparison.InvariantCultureIgnoreCase)) {
				if (field.BindType != null) {
					_typeAlterMapping[field.BindType](column);
				}
				builder.PrimaryKey();
				if (field.AutoIncrement) builder.Identity();
			} else {
				if (field.Nullable) builder.Nullable();
				else builder.NotNullable();

				if (!field.DefaultValue.IsNullOrEmpty()) builder.WithDefaultValue(field.DefaultValue);
				if (field.Size > 0) {
					builder.AsString(field.Size);
                } else {
					if (!field.DbType.IsNullOrEmpty()) {
						var customDbType = field.DbType;
						if (field.Size > 0) customDbType += $"({field.Size})";
						column.AsCustom($"{customDbType}");
                    }
					else if( field.BindType != null) {
						_typeAlterMapping[field.BindType](column);
                    } else {
						throw new DataException($"字段 {field.Name} 必须指定BindType或DbType类型");
                    }
                }
				if (field.UniqueKey) {
					var key = $"{field.Name}_Index";
					if (!schema.Index(key).Exists()) {
						builder.Unique(key);
					}
				}
			}
		}

		public string GetTableName(Type type, IDbContext dbContext = null, bool useConf = false) {
			if(useConf && dbContext != null) {
                var dbCtx = dbContext as DbContext;
                var tbName = dbCtx.Model.FindEntityType(type).GetTableName();
				if (!tbName.IsNullOrEmpty()) return tbName;
            }

			var attr = type.GetCustomAttribute<TableAttribute>();
			if (attr != null) return attr.Name;
			return type.Name;
		}

		public bool TableExists(string tableName) {
			return _migrationContext.QuerySchema.TableExists("", tableName);
		}

		public void SchemaUp() {
			var migrations = _filteringMigrationSource.GetMigrations(t => t.Assembly == GetType().Assembly);

			foreach (var migrationInfo in migrations) {
				_migrationRunner.Up(migrationInfo);
			}
		}

		public void SchemaDown() {
			var migrations = _filteringMigrationSource.GetMigrations(t => t.Assembly == GetType().Assembly);

			foreach (var migrationInfo in migrations) {
				_migrationRunner.Down(migrationInfo);
			}
		}

		protected virtual void DefineFields<TNext>(Type propType, ColumnExtendAttribute columnAttr, IColumnTypeSyntax<TNext> column, IProperty confProperty = null) where TNext : IFluentSyntax {
            if (propType.IsEnum)
                column.AsInt16();
            else if (columnAttr != null && columnAttr.Size > 0) {
                column.AsString(columnAttr.Size);
            } else {
				if(column is ICreateTableColumnAsTypeSyntax _createCol) {
					_typeMapping[propType](_createCol);
				}else if(column is IAlterTableColumnAsTypeSyntax _alterCol) {
					_typeAlterMapping[propType](_alterCol);
				}
            }
            if (propType == typeof(string)) {
				if (confProperty != null) {
					column.AsString(confProperty.GetMaxLength() ?? 255);
                } else {
					var size = 255;
					if (columnAttr != null && columnAttr.Size > 0) {
						size = columnAttr.Size;
					}
					column.AsString(size);
				}
            }
        }

        protected virtual void DefineFields(string tableName, PropertyInfoExplorer propertyInfo, AlterTableExpressionBuilder builder, ISchemaTableSyntax schema, IProperty confProperty = null) {
            var propType = propertyInfo.Member.PropertyType;
            var (columnAttr, foreignKey) = GetColumnName(propertyInfo.Member, out string columnName);
			bool canBeNullable = false;
            if (Nullable.GetUnderlyingType(propType) is Type uType) {
                propType = uType;
                canBeNullable = true;
            }else if (confProperty != null) {
                columnName = confProperty.GetColumnName();
                canBeNullable = confProperty.IsColumnNullable();
			} else {
				if (propertyInfo.IsNullable) {
					canBeNullable = true;
				} else {
					canBeNullable = !propType.IsSimpleType();
				}
			}

            if (!_typeAlterMapping.ContainsKey(propType) && foreignKey == null && !propType.IsEnum && columnAttr == null)
                return;

            var column = schema.Column(columnName).Exists() ? builder.AlterColumn(columnName) : builder.AddColumn(columnName);

            if (foreignKey != null) {
                canBeNullable = true;
                builder.ForeignKey(GetTableName(propertyInfo.Member.PropertyType), $"{foreignKey.Name}");
                propType = propertyInfo.Member.PropertyType.GetProperty(foreignKey.Name).PropertyType;
            }

            var indexAttr = propertyInfo.Member.GetCustomAttribute<IndexFieldAttribute>();
            if (indexAttr != null && foreignKey == null) {
                string indexName = indexAttr.Name;
                if (indexAttr.Name.IsNullOrEmpty()) {
                    indexName = $"Idx_{tableName}_{columnName}";
                }
				if (!schema.Index(indexName).Exists()) {
					if (indexAttr.IsUnique)
						builder.Unique(indexName);
					else
						builder.Indexed(indexName);
				}
            }

            DefineFields(propType, columnAttr , column, confProperty);

            if (columnAttr != null && !columnAttr.Nullable) canBeNullable = false;

            if (canBeNullable)
                builder.Nullable();
            else {
				if (propType.Equals(typeof(string))) {
					builder.WithDefaultValue(string.Empty);
                } else if (propType.IsSimpleType()) {
                    builder.WithDefaultValue(propType.GetValue());
                }
            }
        }

        protected virtual void DefineFields(string tableName, PropertyInfoExplorer propertyInfo, CreateTableExpressionBuilder create, IProperty confProperty = null) {			
			var propType   = propertyInfo.Member.PropertyType;
			var (columnAttr, foreignKey) = GetColumnName(propertyInfo.Member, out string columnName);
			bool canBeNullable = false;

            if (Nullable.GetUnderlyingType(propType) is Type uType) {
                propType = uType;
                canBeNullable = true;
            } else if(confProperty != null) {
				columnName    = confProperty.GetColumnName();
				canBeNullable = confProperty.IsColumnNullable();
			} else {
				if (propertyInfo.IsNullable) {
					canBeNullable = true;
				} else {				
					canBeNullable = !propType.IsSimpleType();
				}
			}

			if (!_typeMapping.ContainsKey(propType) && foreignKey == null && !propType.IsEnum && columnAttr == null)
				return;

            var column = create.WithColumn(columnName);

			if (foreignKey != null) {
				canBeNullable = true;
				create.ForeignKey(GetTableName(propertyInfo.Member.PropertyType), $"{foreignKey.Name}");
				propType = propertyInfo.Member.PropertyType.GetProperty(foreignKey.Name).PropertyType;
			}

			var indexAttr = propertyInfo.Member.GetCustomAttribute<IndexFieldAttribute>();
            if (indexAttr != null && foreignKey == null) {
				string indexName = indexAttr.Name;
				if (indexAttr.Name.IsNullOrEmpty()) {
					indexName = $"Idx_{tableName}_{columnName}";
				}
                if (indexAttr.IsUnique)
                    create.Unique(indexName);
                else
                    create.Indexed(indexName);
            }

			DefineFields(propType, columnAttr, column, confProperty);

            if (columnAttr != null && !columnAttr.Nullable) canBeNullable = false;

			if (canBeNullable)
				create.Nullable();
			else {
				if (propType.Equals(typeof(string))) {
					create.WithDefaultValue(string.Empty);
                } else if (propType.IsSimpleType()) {
					create.WithDefaultValue(propType.GetValue());
				}
			}
		}

		private (ColumnExtendAttribute, ForeignKeyAttribute) GetColumnName(PropertyInfo propertyInfo, out string columnName) {
            var foreignKey = propertyInfo.GetCustomAttribute<ForeignKeyAttribute>();
            var columnAttr = propertyInfo.GetCustomAttribute<ColumnExtendAttribute>();

            columnName = columnAttr != null && !columnAttr.Name.IsNullOrEmpty() ? columnAttr.Name : propertyInfo.Name;
            columnName = foreignKey != null ? $"{propertyInfo.Name}{foreignKey.Name}" : columnName;
			return (columnAttr, foreignKey);
        }

		private IList<ColumnDefinition> BuildPrimaryKey(string tableName, Type type) {
			var keyProperties = GetPrimaryKey(type);

            var result = new List<ColumnDefinition>();
			foreach (var keyProperty in keyProperties) {
				if (!PrimaryKeyTypeMapping.TryGetValue(keyProperty.PropertyType, out DbType value)) throw new NotSupportedException($"not support db type with type {type.Name}");

				var pk = new ColumnDefinition {
					Name             = keyProperty.Name,
					Type             = value,
					TableName        = tableName,
					ModificationType = ColumnModificationType.Create,
					IsPrimaryKey     = true
                };

			    pk.IsIdentity = (pk.Type != DbType.String && pk.Type != DbType.Guid);
				result.Add(pk);
            }

			return result;
		}

		private List<PropertyInfo> GetPrimaryKey(Type type, IDbContext dbContext = null, bool useConf = false) {
			var typeAccessor = TypeAccessor.Get(type, null);

            if (useConf && dbContext != null) {
                var dbCtx = dbContext as DbContext;
				var primaryKey = dbCtx.Model.FindEntityType(type).FindPrimaryKey();
				return typeAccessor.Context.Properties.Where(x => primaryKey.Properties.Any(m => m.Name == x.Member.Name)).Select(x=>x.Member).ToList();
            }

            var keyProperties = typeAccessor.Context.Properties.Where(x => x.Attributes.Any(m => m is KeyAttribute)).Select(x => x.Member).ToList();
            if (keyProperties.Count == 0) {
                var idKeyProperty = typeAccessor.Context.Properties.FirstOrDefault(x => x.Member.Name == Entity.IdKey)?.Member;
                if (idKeyProperty == null) throw new InvalidDataException($"invalid type for migration with the type {type.Name}");

                keyProperties.Add(idKeyProperty);
            }
			return keyProperties;
        }

        private void Execute() {
			_semaphoreSlim.Wait();
			try {
				foreach (var expression in _migrationContext.Expressions) {
					try {
						expression.ExecuteWith(_migrationRunner.Processor);
					} catch (Exception e) {
						Trace.WriteLine($"execute migration exception: {(e.InnerException != null ? e.InnerException.Message : e.Message)}");
					}
				}				
			} finally {
				_semaphoreSlim.Release();
				_migrationContext.Expressions.Clear();
			}
		}
	}
}
