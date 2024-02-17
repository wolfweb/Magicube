using System.Security.Claims;
using System;
using Magicube.Core.Reflection;
using Magicube.Web;
using System.Linq;
using System.Collections.Generic;

namespace Magicube.Identity {
    public static class UserExtension {
        public static IUser ToUser(this ClaimsPrincipal principal) {
            var properties = TypeAccessor.Get<User>().Context.Properties;
            var result = new User();
            foreach (var claim in principal.Claims) {
                var p = properties.FirstOrDefault(x => x.Member.Name == claim.Type);
                if (p == null) continue;

                p.Member.SetValue(result, Convert.ChangeType(claim.Value, p.Member.PropertyType));
            }
            return result;
        }

        public static IUser ToUser(this IEnumerable<Claim> claims) {
            var properties = TypeAccessor.Get<User>().Context.Properties;
            var result = new User();
            foreach (var claim in claims) {
                var p = properties.FirstOrDefault(x => x.Member.Name == claim.Type);
                if (p == null) continue;

                p.Member.SetValue(result, Convert.ChangeType(claim.Value, p.Member.PropertyType));
            }
            return result;
        }

        public static ClaimsPrincipal ToPrincipal(this User user) {
            var typeAccessor = TypeAccessor.Get<User>(user);

            var claims = typeAccessor.Context.Properties.Select(x => {
                var value = x.Member.GetValue(user);
                if(value == null) {
                    return new Claim(x.Member.Name, "");
                }

                return new Claim(x.Member.Name, value.ToString());
            });

            var identity = new ClaimsIdentity(claims, "MagicubeUser");
            return new ClaimsPrincipal(identity);
        }
    }
}
