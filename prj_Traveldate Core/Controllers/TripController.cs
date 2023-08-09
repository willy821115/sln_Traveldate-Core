using Microsoft.AspNetCore.Mvc;

namespace prj_Traveldate_Core.Controllers
{
    public class TripController : Controller
    {
        public IActionResult List()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
    }
}
