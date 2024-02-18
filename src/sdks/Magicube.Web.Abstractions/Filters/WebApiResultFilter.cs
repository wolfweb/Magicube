using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Magicube.Web.Filters {
    public class WebApiResultFilter : IActionFilter {
        public void OnActionExecuted(ActionExecutedContext context) {
            if(context.Result is ObjectResult objResult) {
                context.Result = new ObjectResult(new ServiceResult(200) { Data = objResult.Value });
            }else if(context.Result is JsonResult jsonResult) {
                context.Result = new ObjectResult(new ServiceResult(200) { Data = jsonResult.Value });
            }
        }

        public void OnActionExecuting(ActionExecutingContext context) {
            
        }
    }
}
