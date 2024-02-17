using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.EfDbContext;
using Magicube.Web.Security;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Identity {
    public class DefaultRoleStore :
        IRoleStore<Role>,
        IRoleClaimStore<Role>,
        IRolePermissionStore<Role> {
        private readonly IRepository<Role, int> _roleRepository;
        private readonly IRepository<RoleClaim, int> _roleClaimRepository;

        public DefaultRoleStore(
            IRepository<Role, int> roleRepositoryService, 
            IRepository<RoleClaim, int> roleClaimRepository) {
            _roleRepository           = roleRepositoryService;
            _roleClaimRepository      = roleClaimRepository;
        }
        
        public virtual async Task AddClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();

            role.NotNull();

            await _roleClaimRepository.InsertAsync(new RoleClaim {
                RoleId     = role.Id,
                ClaimType  = claim.Type,
                ClaimValue = claim.Value
            }, cancellationToken);
        }

        public virtual async Task AddPermissionAsync(Role role, Permission permission) {
            role.NotNull();

            if (await HasPermissionAsync(role, permission)) {
                return;
            }

            role.Permissions.Add(permission.Name);
        }

        public virtual async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();

            role.NotNull();

            await _roleRepository.InsertAsync(role, cancellationToken);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();

            role.NotNull();

            _roleRepository.Delete(role);
            return await Task.FromResult(IdentityResult.Success);
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }

        public virtual async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();
            int id;
            if (int.TryParse(roleId, out id)) {
                return await _roleRepository.GetAsync(id, x => x.UserRoles);
            }
            return null;
        }

        public virtual async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();            
            var role = await _roleRepository.GetAsync(x => x.Name == normalizedRoleName, x => x.UserRoles);
            return role;
        }

        public virtual async Task<IList<Claim>> GetClaimsAsync(Role role, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();

            role.NotNull();

            var claims = await _roleClaimRepository.QueryAsync(x => x.RoleId == role.Id);
            return claims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();
        }

        public virtual async Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();
            role.NotNull();

            return await Task.FromResult(role.DisplayName);
        }

        public virtual async Task<IList<Permission>> GetPermissionsAsync(Role role) {
            role.NotNull();
            return await Task.FromResult(role.Permissions.Select(x => new Permission(x)).ToList());
        }

        public virtual async Task<IList<Permission>> GetPermissionsAsync(int roleId) {
            var role = await _roleRepository.GetAsync(roleId);
            return await GetPermissionsAsync(role);
        }

        public virtual async Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();

            role.NotNull();

            return await Task.FromResult(role.Id.ToString());
        }

        public virtual async Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();

            role.NotNull();

            return await Task.FromResult(role.DisplayName);
        }

        public virtual async Task<bool> HasPermissionAsync(Role role, Permission permission) {
            var result = role.Permissions.Any(x => x == permission.Name);
            return await Task.FromResult(result);
        }

        public virtual async Task RemoveAllPermissionAsync(Role role) {
            role.NotNull();
            role.Permissions.Clear();
            await Task.CompletedTask;
        }

        public virtual async Task RemoveClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();

            role.NotNull();

            var entities = _roleClaimRepository.All.Where(x => x.RoleId == role.Id && x.ClaimType == claim.Type && x.ClaimValue == claim.Value).ToArray();
            foreach (var it in entities) {
                _roleClaimRepository.Delete(it);
            }
            await Task.CompletedTask;
        }

        public virtual async Task RemovePermissionAsync(Role role, Permission permission) {
            role.NotNull();
            role.Permissions.RemoveAll(x => x == permission.Name);
            await Task.CompletedTask;
        }

        public virtual async Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();

            role.NotNull();

            role.DisplayName = normalizedName;
            await Task.CompletedTask;
        }

        public virtual async Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();

            role.NotNull();

            role.DisplayName = roleName;
            await Task.CompletedTask;
        }

        public virtual async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();

            role.NotNull();

            await _roleRepository.UpdateAsync(role, cancellationToken);
            return await Task.FromResult(IdentityResult.Success);
        }
    }
}
