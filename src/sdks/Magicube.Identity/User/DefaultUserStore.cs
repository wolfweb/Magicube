using Magicube.Data.Abstractions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Magicube.Core;
using Microsoft.EntityFrameworkCore;
using Magicube.Data.Abstractions.EfDbContext;

namespace Magicube.Identity {
    public class DefaultUserStore :
        IUserStore<User>,
        IUserRoleStore<User>,
        IUserLoginStore<User>,
        IUserClaimStore<User>,
        IUserEmailStore<User>,
        IUserLockoutStore<User>,
        IUserPasswordStore<User>,
        IUserTwoFactorStore<User>,
        IUserPhoneNumberStore<User>,
        IUserSecurityStampStore<User>,
        IUserAuthenticatorKeyStore<User>,
        IUserAuthenticationTokenStore<User>,
        IUserTwoFactorRecoveryCodeStore<User> {

        public const string InternalLoginProvider     = "[MagicubeUserStore]";
        public const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        public const string RecoveryCodeTokenName     = "RecoveryCodes";

        private readonly IRepository<User, long> _repository;
        private readonly IRepository<Role, int> _roleRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<UserLogin, long> _userLoginRepository;
        private readonly IRepository<UserToken, long> _userTokenRepository;
        private readonly IRepository<UserClaims, long> _userClaimsRepository;
        public DefaultUserStore(
            IRepository<User, long> repository,
            IRepository<Role, int> roleRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<UserLogin, long> userLoginRepository,
            IRepository<UserToken, long> userTokenRepository,
            IRepository<UserClaims, long> userClaimsRepository
            ) {
            _repository = repository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userTokenRepository = userTokenRepository;
            _userLoginRepository = userLoginRepository;
            _userClaimsRepository = userClaimsRepository;
        }

        public virtual async Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            foreach (var item in claims) {
                await _userClaimsRepository.InsertAsync(new UserClaims {
                    ClaimType  = item.Type,
                    ClaimValue = item.Value,
                    UserId     = user.Id
                }, cancellationToken);
            }
        }

        public virtual async Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            await _userLoginRepository.InsertAsync(new UserLogin {
                UserId        = user.Id,
                Ticket        = user.ConcurrencyStamp,
                CreateAt      = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                ProviderKey   = login.ProviderKey,
                LoginProvider = login.LoginProvider,
            }, cancellationToken);
        }

        public virtual async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken) {
            var role = _roleRepository.All.SingleOrDefault(x => x.Name == roleName);
            role.NotNull();
            await _userRoleRepository.InsertAsync(new UserRole {
                UserId = user.Id,
                RoleId = role.Id
            }, cancellationToken);
        }

        public virtual async Task<int> CountCodesAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
            if (mergedCodes.Length > 0) {
                return mergedCodes.Split(';').Length;
            }
            return 0;
        }

        public virtual async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            await _repository.InsertAsync(user, cancellationToken);

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            _repository.Delete(user);

            return await Task.FromResult(IdentityResult.Success);
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }

        public virtual async Task<User> FindByEmailAsync(string email, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();
            return await _repository.GetAsync(x => x.Email == email, x => x.UserRoles);
        }

        public virtual async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();
            if (long.TryParse(userId, out long uid)) {
                return await _repository.GetAsync(uid, x => x.UserRoles);
            }
            return await Task.FromResult(default(User));
        }

        public virtual async Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();
            var login = await _userLoginRepository.GetAsync(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey);
            if (login == null) return null;
            var user = await _repository.GetAsync(login.UserId, x => x.UserRoles);

            user.ConcurrencyStamp = login.Ticket;

            return user;
        }

        public virtual async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            return await _repository.GetAsync(x => x.UserName == normalizedUserName, x => x.UserRoles);
        }

        public virtual async Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            return await Task.FromResult(user.AccessFailedCount);
        }

        public virtual async Task<string> GetAuthenticatorKeyAsync(User user, CancellationToken cancellationToken = default) {
            return await GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
        }

        public virtual async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            var claims = await _userClaimsRepository.QueryAsync(x => x.UserId == user.Id);
            return await Task.FromResult(claims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList());
        }

        public virtual async Task<string> GetEmailAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            return await Task.FromResult(user.Email);
        }

        public virtual async Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            return await Task.FromResult(user.EmailConfirmed);
        }

        public virtual async Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            return await Task.FromResult(user.LockoutEnabled);
        }

        public virtual async Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            if (user.LockoutEnd.HasValue) return await Task.FromResult(DateTimeOffset.FromUnixTimeMilliseconds(user.LockoutEnd.Value));
            return null;
        }

        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            var userLogins = await _userLoginRepository.QueryAsync(x => x.UserId == user.Id);
            return userLogins.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.LoginProvider)).ToList();
        }

        public virtual async Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            return await Task.FromResult(user.Email);
        }

        public virtual async Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            return await Task.FromResult(user.DisplayName);
        }

        public virtual async Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            return await Task.FromResult(user.PasswordHash);
        }

        public virtual async Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            return await Task.FromResult(user.PhoneNumber);
        }

        public virtual async Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            return await Task.FromResult(user.PhoneNumberConfirmed);
        }

        public virtual async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken) {
            user.NotNull();
            var roleIds = user.UserRoles.Select(x => x.RoleId).ToArray();
            var roles = await _roleRepository.QueryAsync(x => roleIds.Contains(x.Id));
            return roles.Select(x => x.Name).ToList();
        }

        public virtual async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken) {
            var role = await _roleRepository.GetAsync(x => x.Name == roleName, x => x.UserRoles);
            role.NotNull();
            var userIds = role.UserRoles.Select(x => x.UserId).ToArray();
            return (await _repository.QueryAsync(x => userIds.Contains(x.Id))).ToList();
        }

        public virtual async Task<string> GetSecurityStampAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            return await Task.FromResult(user.SecurityStamp);
        }

        public virtual async Task<string> GetTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            var token = await _userTokenRepository.GetAsync(x => x.UserId == user.Id && x.LoginProvider == loginProvider && x.Name == name);
            return token?.Value;
        }

        public virtual async Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();
            return await Task.FromResult(user.TwoFactorEnabled);
        }

        public virtual async Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();
            return await Task.FromResult(user.Id.ToString());
        }

        public virtual async Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();
            return await Task.FromResult(user.UserName);
        }

        public virtual async Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            var userClaims = await _userClaimsRepository.QueryAsync(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            var userIds = userClaims.Select(x => x.UserId).ToArray();
            return (await _repository.QueryAsync(x => userIds.Contains(x.Id))).ToList();
        }

        public virtual async Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();
            user.NotNull();
            return await Task.FromResult(!user.PasswordHash.IsNullOrEmpty());
        }

        public virtual async Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();
            user.AccessFailedCount += 1;

            return await Task.FromResult(user.AccessFailedCount);
        }

        public virtual async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken) {
            user.NotNull();
            var role = await _roleRepository.GetAsync(x => x.Name == roleName);
            return role == null ? false : user.UserRoles.Any(x => x.RoleId == role.Id);
        }

        public virtual async Task<bool> RedeemCodeAsync(User user, string code, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
            var splitCodes = mergedCodes.Split(';');
            if (splitCodes.Contains(code)) {
                var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
                await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
                return true;
            }
            return false;
        }

        public virtual async Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();
            foreach (var claim in claims) {
                _userClaimsRepository.Delete(x => x.UserId == user.Id && x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            }

            await Task.CompletedTask;
        }

        public virtual async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken) {
            user.NotNull();
            var role = await _roleRepository.GetAsync(x => x.Name == roleName);
            if (role != null) {
                var userRole = await _userRoleRepository.GetAsync(x => x.UserId == user.Id && x.RoleId == role.Id);
                if(userRole!=null) {
                    await _userRoleRepository.DeleteAsync(userRole, cancellationToken);
                }
            }            
        }

        public virtual async Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();
            _userLoginRepository.Delete(x => x.UserId == user.Id && x.LoginProvider == loginProvider && x.ProviderKey == providerKey);
            await Task.CompletedTask;
        }

        public virtual async Task RemoveTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            _userTokenRepository.Delete(x => x.UserId == user.Id && x.LoginProvider == loginProvider && x.Name == name);
            await Task.CompletedTask;
        }

        public virtual async Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            var matchedClaims = await _userClaimsRepository.QueryAsync(x => x.UserId == user.Id && x.ClaimValue == claim.Value && x.ClaimType == claim.Type);
            foreach (var matchedClaim in matchedClaims) {
                matchedClaim.ClaimValue = newClaim.Value;
                matchedClaim.ClaimType = newClaim.Type;
            }
            await Task.CompletedTask;
        }

        public virtual async Task ReplaceCodesAsync(User user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken = default) {
            var mergedCodes = string.Join(";", recoveryCodes);
            await SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
        }

        public async Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            user.AccessFailedCount = 0;
            await Task.CompletedTask;
        }

        public virtual async Task SetAuthenticatorKeyAsync(User user, string key, CancellationToken cancellationToken = default) {
            await SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
        }

        public virtual async Task SetEmailAsync(User user, string email, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();
            user.Email = email;
            await Task.CompletedTask;
        }

        public virtual async Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            user.EmailConfirmed = confirmed;
            await Task.CompletedTask;
        }

        public virtual async Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            user.LockoutEnabled = enabled;
            await Task.CompletedTask;
        }

        public virtual async Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            if (lockoutEnd.HasValue) user.LockoutEnd = lockoutEnd.Value.ToUnixTimeMilliseconds();

            await Task.CompletedTask;
        }

        public virtual async Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();
            user.Email = normalizedEmail;
            await Task.CompletedTask;
        }

        public virtual async Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();
            if(user.DisplayName.IsNullOrEmpty()) user.DisplayName = normalizedName;
            await Task.CompletedTask;
        }

        public virtual async Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            user.PasswordHash = passwordHash;
            await Task.CompletedTask;
        }

        public virtual async Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            user.PhoneNumber = phoneNumber;
            await Task.CompletedTask;
        }

        public virtual async Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            user.PhoneNumberConfirmed = confirmed;
            await Task.CompletedTask;
        }

        public virtual async Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            user.SecurityStamp = stamp;
            await Task.CompletedTask;
        }

        public virtual async Task SetTokenAsync(User user, string loginProvider, string name, string value, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            var userToken = await _userTokenRepository.GetAsync(x=>x.UserId ==user.Id&& x.LoginProvider == loginProvider && x.Name == name);
            if (userToken == null) {
                await _userTokenRepository.InsertAsync(new UserToken {
                    Name          = name,
                    Value         = value,
                    UserId        = user.Id,
                    LoginProvider = loginProvider,
                }, cancellationToken);
            } else {
                userToken.Value = value;
                _userTokenRepository.Update(userToken);
                await Task.CompletedTask;
            }
        }

        public virtual async Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            user.TwoFactorEnabled = enabled;
            await Task.CompletedTask;
        }

        public virtual async Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            user.UserName = userName;
            await Task.CompletedTask;
        }

        public virtual async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            user.NotNull();

            _repository.Update(user);
            return await Task.FromResult(IdentityResult.Success);
        }
    }
}
