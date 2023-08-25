using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using System.Collections.Generic;
using System.Text.Json;

//TODO: 登入: 第三方登入 & 記住帳號密碼(cookie) & 確認Enable & 登入後跳轉
//TODO: 註冊: 欄位名稱 & 必填欄位 & 驗證 & 註冊成功跳轉

namespace prj_Traveldate_Core.Controllers
{
    public class LoginController : Controller
    {
        private IWebHostEnvironment _enviro = null;

        public LoginController(IWebHostEnvironment p)
        {
            _enviro = p;
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(CLoginMemberViewModel vm)
        {
            Member mem = (new TraveldateContext()).Members.FirstOrDefault(
                t => t.Phone.Equals(vm.mlUsername) && t.Password.Equals(vm.mlPassword));
            if (mem != null && mem.Password.Equals(vm.mlPassword))
            {
                if (!(bool)mem.Enable)
                {
                    ViewBag.Message = "您的帳號已被停權，如有疑異請洽客服人員。";
                    return View();
                }
                //string json = JsonSerializer.Serialize(mem);
                HttpContext.Session.SetString(CDictionary.SK_LOGGEDIN_USER, mem.MemberId.ToString());

                if (TempData.ContainsKey(CDictionary.SK_BACK_TO_CONTROLLER))
                {
                    string gocontroller = TempData[CDictionary.SK_BACK_TO_CONTROLLER].ToString();
                    string goaction = TempData[CDictionary.SK_BACK_TO_ACTION].ToString();

                    TempData.Remove(CDictionary.SK_BACK_TO_CONTROLLER);
                    TempData.Remove(CDictionary.SK_BACK_TO_ACTION);

                    return RedirectToAction(goaction, gocontroller);
                }

                return RedirectToAction("HomePage", "HomePage");
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
                HttpContext.Session.SetString(CDictionary.SK_LOGGEDIN_COMPANY, com.CompanyId.ToString());
                return RedirectToAction("List", "Dashboard");
            }
            ViewBag.Message = "帳號或密碼錯誤";
            return View();
        }

        public ActionResult PlatformLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PlatformLogin(CLoginCompanyViewModel vm)
        {
            Employee emp = (new TraveldateContext()).Employees.FirstOrDefault(
                t => t.EmployeeAccount.Equals(vm.clUsername) && t.EmployeePassword.Equals(vm.clPassword));
            if (emp != null && emp.EmployeePassword.Equals(vm.clPassword))
            {
                //string json = JsonSerializer.Serialize(mem);
                HttpContext.Session.SetString(CDictionary.SK_LOGGEDIN_EMPLOYEE, emp.EmployeeId.ToString());
                return RedirectToAction("Review", "Platform");
            }
            ViewBag.Message = "帳號或密碼錯誤";
            return View();
        }

        public ActionResult CompanySignUp()
        {
            TraveldateContext db = new TraveldateContext();
            List<SelectListItem> country = new List<SelectListItem>();
            foreach (var c in db.CountryLists)
            {
                SelectListItem cnty = new SelectListItem()
                {
                    Value = c.Country,
                    Text = c.Country
                };
                country.Add(cnty);
            }
            List<SelectListItem> cities = new List<SelectListItem>();
            foreach (var c in db.CityLists)
            {
                SelectListItem city = new SelectListItem()
                {
                    Value = c.City,
                    Text = c.City
                };
                cities.Add(city);
            }
            ViewBag.City = cities;
            ViewBag.Country = country;
            return View();
        }
        [HttpPost]
        public ActionResult CompanySignUp(Company c)
        {
            TraveldateContext db = new TraveldateContext();
            c.Enable = true;
            db.Companies.Add(c);
            db.SaveChanges();
            return RedirectToAction("CompanyLogin", "Login");
        }

        public ActionResult SignUp()
        {
            List<SelectListItem> cts = new List<SelectListItem>();
            TraveldateContext db = new TraveldateContext();
            foreach (var c in db.CityLists)
            {
                SelectListItem city = new SelectListItem()
                {
                    Value = c.CityId.ToString(),
                    Text = c.City
                };
                cts.Add(city);
            }
            ViewBag.CityId = cts;
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(CMemberWrap m)
        {
            TraveldateContext db = new TraveldateContext();

            //取得使用者上傳檔案的原始檔名
            var fileOriginName = Path.GetFileName(m.photo.FileName);
            //取原始檔名中的副檔名
            var fileExt = Path.GetExtension(fileOriginName);
            string photoName = Guid.NewGuid().ToString() + fileExt;
            m.ImagePath = photoName;
            m.photo.CopyTo(new FileStream(
                _enviro.WebRootPath + "/images/" + photoName, FileMode.Create));

            m.LevelId = 1;
            m.Discount = 0;
            m.Enable = true;
            db.Members.Add(m.member);
            db.SaveChanges();
            return RedirectToAction("Login", "Login");
        }



    }
}
