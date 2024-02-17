using Magicube.Cache.Abstractions;
using Magicube.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Magicube.Web.Security {
    public class PermissionOptions {
        private readonly IList<ModularPermissionItem> _modulars = new List<ModularPermissionItem>();
        private readonly IList<Permission> _permissions = new List<Permission>();

        public IList<ModularPermissionItem> ModularPermissions => _modulars;

        public IEnumerable<Permission>      Permissions        => _permissions;

        public PermissionOptions Add(string display, Assembly assembly, string area = null) {
            if (_modulars.Any(x => x.Assembly.Equals(assembly))) throw new InvalidOperationException($"已存在权限程序集:{area}");
            _modulars.Add(new ModularPermissionItem {
                Area     = area, 
                Display  = display,
                Assembly = assembly
            });
            return this;
        }

        public PermissionOptions Add(params Permission[] permissions) {
            foreach(var permission in permissions) {
                _permissions.Add(permission);
            }
            return this;
        }
    }

    public class ModularPermissionItem {
        public string   Area     { get; set; }
        public string   Display  { get; set; }
        public Assembly Assembly { get; set; }
    }

    public interface IPermissionProvider {
        IEnumerable<Permission> GetPermissions();
    }

    public class DefaultPermissionProvider : IPermissionProvider {
        private readonly ICacheProvider _cacheProvider;
        private readonly PermissionOptions _permissionOptions;
        public DefaultPermissionProvider(IOptions<PermissionOptions> options, IServiceProvider serviceProvider) {
            _permissionOptions = options.Value;
            _cacheProvider     = serviceProvider.GetService<ICacheProvider>(DefaultCacheProvider.Identity);
        }

        public IEnumerable<Permission> GetPermissions() {
            return _cacheProvider.GetOrAdd("permissions", () => _permissionOptions.ModularPermissions.SelectMany(ExtractPermissionsFromController).Concat(_permissionOptions.Permissions), TimeSpan.FromHours(2));
        }

        private IEnumerable<Permission> ExtractPermissionsFromController(ModularPermissionItem item) {
            var controllers = item.Assembly.GetTypes().Where(x => IsController(x));

            foreach (var controller in controllers) {
                if(controller.Name.StartsWith("admin", StringComparison.OrdinalIgnoreCase) || controller.IsDefined(typeof(AuthorizeAttribute))) {
                    var actions = GetActions(controller, x => !x.IsDefined(typeof(AllowAnonymousAttribute)));
                    foreach (var action in actions) {
                        var controllerName = controller.Name.Replace("Controller", "");
                        var permissionName = item.Area.IsNullOrEmpty() ? $"{controller.Namespace}:{controllerName}:{action}" : $"{item.Area}:{controller.Namespace}:{controllerName}:{action}";
                        yield return new Permission(permissionName) {
                            Category = item.Display
                        };
                    }
                }
                else {
                    var actions = GetActions(controller, x => x.IsDefined(typeof(AuthorizeAttribute)));
                    foreach (var action in actions) {
                        var controllerName = controller.Name.Replace("Controller", "");
                        var permissionName = item.Area.IsNullOrEmpty() ? $"{controller.Namespace}:{controllerName}:{action}" : $"{item.Area}:{controller.Namespace}:{controllerName}:{action}";
                        yield return new Permission(permissionName) {
                            Category = item.Display
                        };
                    }
                }
            }
        }

        private IList<string> GetActions(Type type, Func<MethodInfo,bool> filter) {
            var query = type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Where(x => x.ReturnType.IsAssignableFrom(typeof(IActionResult)) || x.ReturnType.IsAssignableFrom(typeof(Task<IActionResult>)))
                .Where(filter);
            
            return query.Select(x => {
                var actionName = x.GetCustomAttribute<ActionNameAttribute>();
                if (actionName != null) return actionName.Name;
                return x.Name;
            }).Distinct().ToList();
        }

        private bool IsController(Type x) {
            return x.IsSubclassOf(typeof(ControllerBase));
        }
    }
}
