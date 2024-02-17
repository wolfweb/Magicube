using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magicube.Web.Navigation {
    public class NavigationBuilder {
        private List<MenuItem> Contained { get; set; }

        public NavigationBuilder() {
            Contained = new List<MenuItem>();
        }

        public async Task<NavigationBuilder> AddAsync(LocalizedString display, Func<NavigationItemBuilder, Task> itemBuilder, IEnumerable<string> classes = null, int sort = 0) {
            var childBuilder = new NavigationItemBuilder();

            childBuilder.Sort(sort).Display(display);
            await itemBuilder(childBuilder);

            var menu = childBuilder.Complete();
            var existMenu = Recursive(Contained, x => x.Text.Name == display.Name);
            if (existMenu != null) {
                existMenu.Items.AddRange(menu.Items);
            }
            else {
                Contained.Add(menu);
            }
            
            if (classes != null) {
                foreach (var className in classes) {
                    childBuilder.AddClass(className);
                }
            }

            return this;
        }

        public NavigationBuilder Add(LocalizedString display, Action<NavigationItemBuilder> itemBuilder, IEnumerable<string> classes = null, int sort = 0) {
            var childBuilder = new NavigationItemBuilder();

            childBuilder.Sort(sort).Display(display);
            itemBuilder(childBuilder);

            var menu = childBuilder.Complete();
            var existMenu = Recursive(Contained, x => x.Text.Name == display.Name);
            if (existMenu != null) {
                menu.Items.ForEach(x => x.Parent = existMenu);
                existMenu.Items.AddRange(menu.Items);
            } else {
                Contained.Add(menu);
            }

            if (classes != null) {
                foreach (var className in classes) {
                    childBuilder.AddClass(className);
                }
            }

            return this;
        }

        public Task<NavigationBuilder> AddAsync(LocalizedString display, Func<NavigationItemBuilder, Task> itemBuilder, IEnumerable<string> classes = null) {
            return AddAsync(display, itemBuilder, classes, 0);
        }
        
        public NavigationBuilder Add(LocalizedString display, Action<NavigationItemBuilder> itemBuilder, IEnumerable<string> classes = null) {
            return Add(display, itemBuilder, classes, 0);
        }
        
        public NavigationBuilder Add(LocalizedString display, IEnumerable<string> classes = null) {
            return Add(display, x => { }, classes);
        }

        public NavigationBuilder Remove(MenuItem item) {
            Contained.Remove(item);
            return this;
        }

        public NavigationBuilder Remove(Predicate<MenuItem> match) {
            RemoveRecursive(Contained, match);
            return this;
        }

        public virtual List<MenuItem> Build(MenuItem parent = null) {
            if (parent != null) {
                foreach (var item in Contained) {
                    item.Parent = parent;
                }
            }
            return Contained;
        }

        private static void RemoveRecursive(List<MenuItem> menuItems, Predicate<MenuItem> match) {
            menuItems.RemoveAll(match);
            foreach (var menuItem in menuItems) {
                if (menuItem.Items?.Count > 0) {
                    RemoveRecursive(menuItem.Items, match);
                }
            }
        }

        private static MenuItem Recursive(List<MenuItem> menuItems, Predicate<MenuItem> match) {
            foreach (var menuItem in menuItems) {
                if (match(menuItem)) return menuItem;

                if (menuItem.Items?.Count > 0) {
                    var res = Recursive(menuItem.Items, match);
                    if (res != null) return res;
                }
            }

            return null;
        }
    }
}
