using Magicube.Data.Abstractions;
using Magicube.Web.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.Identity {
    public interface IRolePermissionStore<TRole> where TRole : Entity<int>{
        Task AddPermissionAsync(TRole role, Permission permission);
        Task RemovePermissionAsync(TRole role, Permission permission);
        Task<IList<Permission>> GetPermissionsAsync(TRole role);
        Task<IList<Permission>> GetPermissionsAsync(int roleId);
        Task<bool> HasPermissionAsync(TRole role, Permission permission);
        Task RemoveAllPermissionAsync(TRole role);
    }
}
