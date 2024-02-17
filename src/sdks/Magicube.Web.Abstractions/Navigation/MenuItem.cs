using Magicube.Web.Security;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;

namespace Magicube.Web.Navigation {
    public class MenuItem {
        public MenuItem() {
            Items       = new List<MenuItem>();
            Classes     = new List<string>();
            Permissions = new List<Permission>();
        }

        public LocalizedString      Text        { get; set; }

        public string               Href        { get; set; }

        public string               Icon        { get; set; }

        public int                  Sort        { get; set; }

        public List<MenuItem>       Items       { get; set; }

        public MenuItem             Parent      { get; internal set; }
                                    
        public bool                 IsActived   { get; set; }

        public bool                 IsOpened    { get; set; }

        public List<string>         Classes     { get; }

        public List<Permission>     Permissions { get; }

        public RouteValueDictionary RouteValues { get; set; }
    }
}
