using Microsoft.AspNetCore.Mvc;

namespace prj_Traveldate_Core.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult List()
        {
            return View();
        }
    }
}
