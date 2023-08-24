using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using prj_Traveldate_Core.Models.MyModels;

namespace prj_Traveldate_Core.Controllers
{
    public class CompanySuperController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOGGEDIN_COMPANY))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(
                    new
                    {
                        controller = "Login",
                        action = "CompanyLogin"
                    }));
            }
        }
    }
}
