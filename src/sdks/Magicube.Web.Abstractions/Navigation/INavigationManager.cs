using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Magicube.Web.Navigation {
    public interface INavigationManager {
        Task<MenuItem> GetCurrentMenuAsync(string name, ActionContext context);
        Task<IEnumerable<MenuItem>> BuildMenuAsync(string name, ActionContext context);
    }

    public class NavigationManager : INavigationManager {
        private readonly ILogger _logger;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEnumerable<INavigationProvider> _navigationProviders;

        private IUrlHelper _urlHelper;
        private IEnumerable<MenuItem> _menus;

        public MenuItem CurrentMenu { get; private set; }

        public NavigationManager(
            ILogger<NavigationManager> logger,
            IUrlHelperFactory urlHelperFactory,
            IAuthorizationService authorizationService,
            IEnumerable<INavigationProvider> navigationProviders
            ) {
            _logger               = logger;
            _urlHelperFactory     = urlHelperFactory;
            _navigationProviders  = navigationProviders;
            _authorizationService = authorizationService;
        }

        public async Task<MenuItem> GetCurrentMenuAsync(string name, ActionContext context) {
            if (CurrentMenu == null) {
                await BuildMenuAsync(name, context).ConfigureAwait(false);
            }

            return CurrentMenu;
        }

        public async Task<IEnumerable<MenuItem>> BuildMenuAsync(string name, ActionContext actionContext) {
            if (_menus != null) return _menus;

            var builder = new NavigationBuilder();

            foreach (var navigationProvider in _navigationProviders) {
                try {
                    await navigationProvider.BuildNavigationAsync(name, builder);
                } catch (Exception e) {
                    _logger.LogError(e, "An exception occurred while building the menu '{MenuName}'", name);
                }
            }

            var menuItems = builder.Build().OrderBy(x=>x.Sort).ToList();

            Merge(menuItems);

            menuItems = await AuthorizeAsync(menuItems, actionContext.HttpContext.User);

            menuItems = ComputeHref(menuItems, actionContext);

            menuItems = Reduce(menuItems);

            _menus = menuItems.OrderBy(x => x.Sort);
            return _menus;
        }

        private static void Merge(List<MenuItem> items) {
            for (var i = 0; i < items.Count; i++) {
                var source = items[i];
                var merged = false;
                for (var j = items.Count - 1; j > i; j--) {
                    var cursor = items[j];

                    if (string.Equals(cursor.Text.Name, source.Text.Name, StringComparison.OrdinalIgnoreCase)) {
                        merged = true;
                        foreach (var child in cursor.Items) {
                            source.Items.Add(child);
                        }

                        items.RemoveAt(j);

                        if (cursor.Sort > source.Sort) {
                            source.Href = cursor.Href;
                            source.RouteValues = cursor.RouteValues;
                            source.Text = cursor.Text;

                            source.Permissions.Clear();
                            source.Permissions.AddRange(cursor.Permissions);
                        }
                    }
                }

                if (merged) {
                    Merge(source.Items);
                }
            }
        }

        private List<MenuItem> ComputeHref(List<MenuItem> menuItems, ActionContext actionContext) {
            foreach (var menuItem in menuItems) {
                menuItem.Href = GetUrl(menuItem.Href, menuItem.RouteValues, actionContext);
                menuItem.Items = ComputeHref(menuItem.Items, actionContext);

                menuItem.IsActived = IsActived(menuItem.RouteValues, actionContext);

                if (menuItem.IsActived) {
                    CurrentMenu = menuItem;
                    var pm = menuItem;
                    while (pm.Parent!=null) {
                        pm.Parent.IsOpened = true;
                        pm = pm.Parent;
                    }
                }
            }

            return menuItems;
        }

        private string GetUrl(string menuItemUrl, RouteValueDictionary routeValueDictionary, ActionContext actionContext) {
            if (routeValueDictionary?.Count > 0) {
                if (_urlHelper == null) {
                    _urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);
                }

                return _urlHelper.RouteUrl(new UrlRouteContext { Values = routeValueDictionary });
            }

            if (string.IsNullOrEmpty(menuItemUrl)) {
                return "#";
            }

            if (menuItemUrl[0] == '/' || menuItemUrl.IndexOf("://", StringComparison.Ordinal) >= 0) {
                return menuItemUrl;
            }

            if (menuItemUrl.StartsWith("~/", StringComparison.Ordinal)) {
                menuItemUrl = menuItemUrl.Substring(2);
            }

            return actionContext.HttpContext.Request.PathBase.Add('/' + menuItemUrl).Value;
        }

        private async Task<List<MenuItem>> AuthorizeAsync(IEnumerable<MenuItem> items, ClaimsPrincipal user) {
            var filtered = new List<MenuItem>();

            foreach (var item in items) {
                if (user == null) {
                    filtered.Add(item);
                } else if (!item.Permissions.Any()) {
                    filtered.Add(item);
                } else {
                    var isAuthorized = true;
                    foreach (var permission in item.Permissions) {
                        if (!await _authorizationService.AuthorizeAsync(user, permission)) {
                            isAuthorized = false;
                            break;
                        }
                    }
                    if (isAuthorized) {
                        filtered.Add(item);
                    }
                }

                item.Items = await AuthorizeAsync(item.Items, user);
            }

            return filtered;
        }

        private List<MenuItem> Reduce(IEnumerable<MenuItem> items) {
            var filtered = items.ToList();

            foreach (var item in items) {
                if (!HasHrefOrChildHref(item)) {
                    filtered.Remove(item);
                }

                item.Items = Reduce(item.Items);
            }

            return filtered;
        }

        private static bool HasHrefOrChildHref(MenuItem item) {
            if (item.Href != "#") {
                return true;
            }

            return item.Items.Any(HasHrefOrChildHref);
        }

        private bool IsActived(RouteValueDictionary menu, ActionContext actionContext) {
            bool isActived = true;
            if (menu == null) return false;
            foreach (var key in menu.Keys) {
                if(!actionContext.RouteData.Values.ContainsKey(key) || !actionContext.RouteData.Values[key].Equals(menu[key])) {
                    isActived = false;
                    break;
                }
            }

            return isActived;
        }
    }
}
