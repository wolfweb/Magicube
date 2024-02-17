using Magicube.Web.Security;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;

namespace Magicube.Web.Navigation {
    public class NavigationItemBuilder : NavigationBuilder {
        private readonly MenuItem _item;

        public NavigationItemBuilder() {
            _item = new MenuItem();
        }

        public NavigationItemBuilder(MenuItem existingItem) {
            if (existingItem == null) {
                throw new ArgumentNullException(nameof(existingItem));
            }

            _item = existingItem;
        }

        public NavigationItemBuilder Display(LocalizedString display) {
            _item.Text = display;
            return this;
        }

        public NavigationItemBuilder Icon(string iconClass) {
            _item.Icon = iconClass;
            return this;
        }

        public NavigationItemBuilder Sort(int priority) {
            _item.Sort = priority;
            return this;
        }

        public NavigationItemBuilder Url(string url) {
            _item.Href = url;
            return this;
        }

        public NavigationItemBuilder AddClass(string className) {
            if (!_item.Classes.Contains(className))
                _item.Classes.Add(className);
            return this;
        }

        public NavigationItemBuilder RemoveClass(string className) {
            if (_item.Classes.Contains(className))
                _item.Classes.Remove(className);
            return this;
        }

        public NavigationItemBuilder Permission(Permission permission) {
            _item.Permissions.Add(permission);
            return this;
        }

        public NavigationItemBuilder Permissions(IEnumerable<Permission> permissions) {
            _item.Permissions.AddRange(permissions);
            return this;
        }

        internal MenuItem Complete() {
            _item.Items = base.Build(_item);
            return _item;
        }

        public NavigationItemBuilder Action(RouteValueDictionary values) {
            return values != null
                       ? Action(values["action"] as string, values["controller"] as string, values)
                       : Action(null, null, new RouteValueDictionary());
        }

        public NavigationItemBuilder Action(string actionName) {
            return Action(actionName, null, new RouteValueDictionary());
        }

        public NavigationItemBuilder Action(string actionName, string controllerName) {
            return Action(actionName, controllerName, new RouteValueDictionary());
        }

        public NavigationItemBuilder Action(string actionName, string controllerName, object values) {
            return Action(actionName, controllerName, new RouteValueDictionary(values));
        }

        public NavigationItemBuilder Action(string actionName, string controllerName, RouteValueDictionary values) {
            return Action(actionName, controllerName, null, values);
        }

        public NavigationItemBuilder Action(string actionName, string controllerName, string areaName) {
            return Action(actionName, controllerName, areaName, new RouteValueDictionary());
        }

        public NavigationItemBuilder Action(string actionName, string controllerName, string areaName, RouteValueDictionary values) {
            _item.RouteValues = new RouteValueDictionary(values);
            if (!string.IsNullOrEmpty(actionName))
                _item.RouteValues["action"] = actionName;
            if (!string.IsNullOrEmpty(controllerName))
                _item.RouteValues["controller"] = controllerName;
            if (!string.IsNullOrEmpty(areaName))
                _item.RouteValues["area"] = areaName;
            return this;
        }
    }
}
