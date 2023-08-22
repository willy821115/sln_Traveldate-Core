using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using System.Text.Json;

namespace prj_Traveldate_Core.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(CLoginMemberViewModel vm)
        {
            Member mem= (new TraveldateContext()).Members.FirstOrDefault(
                t => t.Phone.Equals(vm.mlUsername) && t.Password.Equals(vm.mlPassword));
            if ( mem != null && mem.Password.Equals(vm.mlPassword))
            {
                //string json = JsonSerializer.Serialize(mem);
                HttpContext.Session.SetString(CDictionary.SK_LOGGEDIN_USER, mem.MemberId.ToString());
                return RedirectToAction((string)TempData[CDictionary.SK_BACK_TO_ACTION], (string)TempData[CDictionary.SK_BACK_TO_CONTROLLER]);
            }
            ViewBag.Message = "帳號或密碼錯誤";
            return View();
        }


        public ActionResult CompanyLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CompanyLogin(CLoginCompanyViewModel vm)
        {
            Company com = (new TraveldateContext()).Companies.FirstOrDefault(
                t => t.TaxIdNumber.Equals(vm.clUsername) && t.Password.Equals(vm.clPassword));
            if (com != null && com.Password.Equals(vm.clPassword))
            {
                //string json = JsonSerializer.Serialize(mem);
                HttpContext.Session.SetString(CDictionary.SK_LOGGEDIN_COMPANY, com.TaxIdNumber.ToString());
                return RedirectToAction("List", "Dashboard");
            }
            ViewBag.Message = "帳號或密碼錯誤";
            return View();
        }

        public ActionResult CompanySignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CompanySignUp(Company c)
        {
            TraveldateContext db = new TraveldateContext();
            db.Companies.Add(c);
            db.SaveChanges();
            return RedirectToAction("CompanyLogin", "Login");
        }

        public ActionResult SignUp()
        {
            List<SelectListItem> cities = new List<SelectListItem>();
            TraveldateContext db = new TraveldateContext();
            foreach(var c in db.CityLists)
            {
                SelectListItem city = new SelectListItem()
                {
                    Value = c.CityId.ToString(),
                    Text = c.City
                };
                cities.Add(city);
            }
            ViewBag.CityId = cities;
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(Member m)
        {
            TraveldateContext db = new TraveldateContext();
            db.Members.Add(m);
            db.SaveChanges();
            return RedirectToAction("Login", "Login");
        }
    }
}
