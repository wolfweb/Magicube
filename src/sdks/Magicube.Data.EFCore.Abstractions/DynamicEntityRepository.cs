using Magicube.Core.Models;
using Magicube.Core.Signals;
using Magicube.Data.Abstractions.EfDbContext;
using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Abstractions.SqlBuilder.Operators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Magicube.Data.Abstractions {
	public class DynamicEntityRepository : IDynamicEntityRepository {
		private readonly DynamicDataSourceOptions _dynamicDataSourceOptions;
		private readonly IRepository<DbTable, int> _dbTableRepository;
		private readonly DefaultDbContext _defaultDbContext;
		private readonly ISqlBuilder _sqlBuilder;
		private readonly IServiceScope _scoped;
		private readonly ISignal _signal;

		public static ConcurrentBag<DbTable> Tables = new ConcurrentBag<DbTable>();

		public DynamicEntityRepository(
			IServiceScopeFactory serviceScopeFactory,
			IRepository<DbTable, int> dbTableRepository,
			IOptions<DynamicDataSourceOptions> options,
			ISignal signal
			) {
			_scoped = serviceScopeFactory.CreateScope();
			_dynamicDataSourceOptions = options.Value;
			_dbTableRepository = dbTableRepository;
			_signal = signal;
			_defaultDbContext = _scoped.ServiceProvider.GetService<IDbContext>() as DefaultDbContext;
			_sqlBuilder = _scoped.ServiceProvider.GetService<ISqlBuilder>();
		}

		public IServiceScope ServiceScope => _scoped;

		public IDbContext DbContext => _defaultDbContext;

		public IChangeToken ChangeToken { get; private set; }

		public virtual async Task<IEnumerable<DynamicEntityType>> GetAllTypes() {
			await EnsureInitialize();
			return Tables.Select(x => new DynamicEntityType {
				Name = x.Name,
				Title = x.Title,
				Desc = x.Description
			});
		}

		public virtual async Task<DynamicEntity> NewAsync(string name) {
			await EnsureInitialize();
			var table = Tables.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
			if (table == null) throw new DataException("unknow dynamic entity name");
			DynamicEntity entity = new DynamicEntity(table);
			return entity;
		}

		public virtual async Task DeleteAsync(DynamicEntity entity) {
			await EnsureInitialize();
			var rawSql = _sqlBuilder.Delete(entity.TableName).Where(Entity.IdKey, entity.Id).Build();
			_defaultDbContext.Execute(rawSql);
		}

		public virtual async Task<DynamicEntity> GetAsync(string name, int id) {
			return await GetAsync(name, null, id);
		}

		public virtual async Task<DynamicEntity> GetAsync(string name, string[] columns, int id) {
			await EnsureInitialize();

			var table = Tables.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
			if (table == null) throw new DataException("unknow dynamic entity name");

			SqlKata.Query queryCtx = _sqlBuilder.Query(name);
			if (columns != null && columns.Any()) {
				queryCtx = queryCtx.Select(columns);
			}

			var rawSql = queryCtx.Where(Entity.IdKey, id).Limit(1).Build();
			var dataTable = _defaultDbContext.SqlQuery(rawSql);
			var result = new List<DynamicEntity>();
			foreach (DataRow row in dataTable.Rows) {
				DynamicEntity entity = row;
				result.Add(entity);
			}
			return result.FirstOrDefault();
		}

		public virtual async Task<long> InsertAsync(DynamicEntity entity) {
			await EnsureInitialize();
			var rawSql = _sqlBuilder.Insert(entity.TableName).SetData(entity).Build();
			var result = _defaultDbContext.Execute<long>(rawSql);
			return result;
		}

		public virtual async Task UpdateAsync(DynamicEntity entity) {
			await EnsureInitialize();
			var operatorCtx = _sqlBuilder.Update(entity.TableName).Where(Entity.IdKey, entity.Id);
			foreach (var field in entity.Fields) {
				if (field.Key == Entity.IdKey && entity[field.Key] == entity.DefaultValue(Entity.IdKey)) continue;
				operatorCtx.Set(field.Key, entity[field.Key]);
			}

			_defaultDbContext.Execute(operatorCtx.Build());
		}

		public virtual async Task EnsureInitialize() {
			if (Tables.IsEmpty || ChangeToken == null || ChangeToken.HasChanged) {
				Tables.Clear();
				var tables = await _dbTableRepository.QueryAsync(x => x.Status == EntityStatus.Actived);
				foreach (var table in tables) {
					Tables.Add(table);
				}
				DynamicEntity.Initialize(tables.ToArray());
				(var _, ChangeToken) = _signal.GetToken(_dynamicDataSourceOptions.DbTableSignalKey);
			}
		}
	}
}
