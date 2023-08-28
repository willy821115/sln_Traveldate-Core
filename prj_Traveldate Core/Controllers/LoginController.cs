using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

//TODO: 登入: 第三方登入 & 記住帳號密碼(cookie) & 確認Enable & 登入後跳轉
//TODO: 註冊: 欄位名稱 & 必填欄位 & 驗證 & 註冊成功跳轉

namespace prj_Traveldate_Core.Controllers
{
    public class LoginController : Controller
    {
        private IWebHostEnvironment _enviro;
        private readonly IConfiguration _configuration;

        public LoginController(IWebHostEnvironment p, IConfiguration configuration)
        {
            _enviro = p;
            _configuration = configuration;
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
                if (!(bool)mem.Verified)
                {
                    return RedirectToAction("NotVerified");
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

            if (m.photo != null)
            {
                //取得使用者上傳檔案的原始檔名
                var fileOriginName = Path.GetFileName(m.photo.FileName);
                //取原始檔名中的副檔名
                var fileExt = Path.GetExtension(fileOriginName);
                string photoName = Guid.NewGuid().ToString() + fileExt;
                m.ImagePath = photoName;
                m.photo.CopyTo(new FileStream(
                    _enviro.WebRootPath + "/images/" + photoName, FileMode.Create));
            }

            m.LevelId = 1;
            m.Discount = 0;
            m.Enable = true;
            m.Verified = false;
            db.Members.Add(m.member);
            db.SaveChanges();

            CEmailVerify data = new CEmailVerify()
            {
                Email = m.Email,
                mailSubject = "Traveldate信箱驗證連結",
                mailContent = "點擊以下連結，即可回到Traveldate完成註冊驗證，連結將在 30 分鐘後失效。<br><br>",
                receivePage = "Login/VerifyMail",
                linkText = "點我啟用帳號",
                scheme = HttpContext.Request.Scheme,
                host = HttpContext.Request.Host.ToString(),
            };
            LoginApiController api = new LoginApiController(_configuration, HttpContext);
            api.SendMail(data);
            return RedirectToAction("VerifyMailSent");
        }

        public IActionResult ForgotPwd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPwd(CForgotPwdViewModel vm)
        {
            CEmailVerify data = new CEmailVerify()
            {
                Email = vm.email,
                mailSubject = "Traveldate重設密碼連結",
                mailContent = "點擊以下連結，即可回到Traveldate重新設定密碼，連結將在 30 分鐘後失效。<br><br>",
                receivePage = "Login/ResetPwd",
                linkText = "點我重設密碼",
                scheme = HttpContext.Request.Scheme,
                host = HttpContext.Request.Host.ToString(),
            };
            LoginApiController api = new LoginApiController(_configuration, HttpContext);
            api.SendMail(data);

            return RedirectToAction("SentResetMail");
        }


        public IActionResult SentResetMail()
        {
            return View();
        }

        public IActionResult ResetPwd(string verify)
        {
            LoginApiController api = new LoginApiController(_configuration, HttpContext);
            if (!api.VerifyCode(verify))
            {
                RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public IActionResult ResetPwd(CpasswordChangeViewModel vm)
        {
            TraveldateContext _context = new TraveldateContext();
            if(vm.txtNewPassword == vm.txtCheckPassword && HttpContext.Session.Keys.Contains(CDictionary.SK_RESET_PWD_EMAIL))
            {
                Member mem = _context.Members.Where(m => m.Email.Equals(HttpContext.Session.GetString(CDictionary.SK_RESET_PWD_EMAIL))).FirstOrDefault();
                if (mem != null)
                {
                    mem.Password = vm.txtNewPassword;
                    _context.SaveChanges();
                    TempData["resultMsg"] = "密碼更改成功！請重新登入一次";
                    TempData["destination"] = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + Url.Content("~/") + "Login/Login";
                    Thread.Sleep(3000);
                    return RedirectToAction("Login");
                }
            }

            return RedirectToAction("Login");
        }

        public IActionResult RedirectTo()
        {
            return View();
        }

        public IActionResult NotVerified()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NotVerified(CForgotPwdViewModel vm)
        {
            CEmailVerify data = new CEmailVerify()
            {
                Email = vm.email,
                mailSubject = "Traveldate信箱驗證連結",
                mailContent = "點擊以下連結，即可回到Traveldate完成註冊驗證，連結將在 30 分鐘後失效。<br><br>",
                receivePage = "Login/VerifyMail",
                linkText = "點我啟用帳號",
                scheme = HttpContext.Request.Scheme,
                host = HttpContext.Request.Host.ToString(),
            };
            LoginApiController api = new LoginApiController(_configuration, HttpContext);
            api.SendMail(data);
            return RedirectToAction("VerifyMailSent");
        }

        public IActionResult VerifyMailSent()
        {
            return View();
        }

        public IActionResult VerifyMail(string verify)
        {
            LoginApiController api = new LoginApiController(_configuration, HttpContext);
            if (!api.VerifyCode(verify))
            {
                RedirectToAction("Login");
            }
            TraveldateContext _context = new TraveldateContext();
            Member mem = _context.Members.Where(m => m.Email.Equals(HttpContext.Session.GetString(CDictionary.SK_RESET_PWD_EMAIL))).FirstOrDefault();
            if (mem != null)
            {
                mem.Verified = true;
                _context.SaveChanges();
            }

            return View();
        }
    }
}
