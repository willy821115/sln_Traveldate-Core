using Microsoft.AspNetCore.Mvc;

namespace prj_Traveldate_Core.Controllers
{
    public class AnalysisController : CompanySuperController
    {
        public IActionResult SellAnalysis()
        {
            return View();
        }
    }
}
