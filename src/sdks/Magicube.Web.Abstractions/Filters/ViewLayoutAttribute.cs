using Magicube.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Magicube.Web.Filters {
    public class ViewLayoutAttribute : ResultFilterAttribute {
        public string Layout { get; set; }

        public override void OnResultExecuting(ResultExecutingContext context) {
            var filters = context.ActionDescriptor.FilterDescriptors.Select(x => x.Filter);
            var viewResult = context.Result as ViewResult;
            if (viewResult != null) {
                var viewLayout = filters.OfType<ViewLayoutAttribute>().FirstOrDefault();
                if (viewLayout != null) {
                    Layout = viewLayout.Layout;
                }

                if (Layout.IsNullOrEmpty() && filters.Any(x => x is IAllowAnonymousFilter)) return;
                
                if (Layout.IsNullOrEmpty() && filters.Any(x => x is IAsyncAuthorizationFilter || x is IAuthorizationFilter)) {
                    Layout = "_AdminLayout";
                }

                viewResult.ViewData["Layout"] = Layout;
            }
        }
    }

}
