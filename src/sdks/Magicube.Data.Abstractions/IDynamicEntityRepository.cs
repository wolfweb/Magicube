using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.Data.Abstractions {
	public interface IDynamicEntityRepository {
        Task<IEnumerable<DynamicEntityType>> GetAllTypes();
        Task<DynamicEntity>        NewAsync(string name);
        Task                       DeleteAsync(DynamicEntity entity);
        Task<DynamicEntity>        GetAsync(string name, int id);
        Task<DynamicEntity>        GetAsync(string name, string[] columns, int id);
        Task<long>                 InsertAsync(DynamicEntity entity);
        Task                       UpdateAsync(DynamicEntity entity);
        Task                       EnsureInitialize();

        IDbContext    DbContext    { get; }
                      
        IChangeToken  ChangeToken  { get; }

        IServiceScope ServiceScope { get; }
    }

    public class DynamicEntityType {
        public string Title  { get; set; }
        public string Name   { get; set; }
        public string Desc   { get; set; }
    }
}
