using System.Diagnostics;
using Magicube.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Magicube.Identity.Web.Identity.Pages {
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel {
        public string RequestId { get; set; }

        public bool ShowRequestId => !RequestId.IsNullOrEmpty();

        public void OnGet() {
            Request.HttpContext.Response.StatusCode = int.Parse(Request.Query["code"]);
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
