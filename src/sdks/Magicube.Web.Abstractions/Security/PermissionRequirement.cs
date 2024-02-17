using Microsoft.AspNetCore.Authorization;
using System;

namespace Magicube.Web.Security {
    public class PermissionRequirement : IAuthorizationRequirement {
        public PermissionRequirement(Permission permission) {
            if (permission == null) {
                throw new ArgumentNullException(nameof(permission));
            }

            Permission = permission;
        }

        public Permission Permission { get; set; }
    }
}
