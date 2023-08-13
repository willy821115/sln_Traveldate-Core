using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.Controllers
{
    public class TripController : Controller
    {
        private TraveldateContext _db = null;

        public TripController() 
        {
        _db=new TraveldateContext();
            //HttpContext.Session.SetInt32(CDictionary.SK_COMPANYID, 1);
            //companyID=  (int)HttpContext.Session.GetInt32(CDictionary.SK_COMPANYID);
        }



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
