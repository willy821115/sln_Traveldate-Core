using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.Controllers
{
    public class CompanyController : Controller
    {
        public IActionResult Edit()
        {
            int companyID = 1;
            TraveldateContext db = new TraveldateContext();
            var datas = db.Companies.Where(c => c.CompanyId == companyID).FirstOrDefault();

            return View(datas);
        }

        public IActionResult Password() 
        {
        return View();
        }
    }
}
