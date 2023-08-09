using Microsoft.AspNetCore.Mvc;

namespace prj_Traveldate_Core.Controllers
{
    public class ForumController : Controller
    {
        public IActionResult ForumList()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult ArticleView()
        {
            return View();
        }
    }
}
