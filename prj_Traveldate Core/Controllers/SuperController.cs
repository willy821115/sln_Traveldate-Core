using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using prj_Traveldate_Core.Models.MyModels;
using System;

namespace prj_Traveldate_Core.Controllers
{
    public class SuperController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOGGEDIN_USER))
            {
                TempData[CDictionary.SK_BACK_TO_CONTROLLER] = ControllerContext.RouteData.Values["controller"];
                TempData[CDictionary.SK_BACK_TO_ACTION] = ControllerContext.RouteData.Values["action"];
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(
                    new
                    {
                        controller = "Login",
                        action = "Login"
                    }));
            }
        }
    }
}
