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
        TraveldateContext _context;

        public LoginController(IWebHostEnvironment p, IConfiguration configuration)
        {
            _enviro = p;
            _configuration = configuration;
            _context = new TraveldateContext();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(CLoginMemberViewModel vm)
        {
            Member mem = _context.Members.FirstOrDefault(
                t => t.Email.Equals(vm.mlUsername) && t.Password.Equals(vm.mlPassword));
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
                HttpContext.Session.SetString(CDictionary.SK_LOGGEDIN_USER_NAME, mem.FirstName);
                if (TempData.ContainsKey(CDictionary.SK_BACK_TO_CONTROLLER) && !TempData.ContainsKey(CDictionary.SK_BACK_TO_PARAM))
                {
                    string gocontroller = TempData[CDictionary.SK_BACK_TO_CONTROLLER].ToString();
                    string goaction = TempData[CDictionary.SK_BACK_TO_ACTION].ToString();

                    TempData.Remove(CDictionary.SK_BACK_TO_CONTROLLER);
                    TempData.Remove(CDictionary.SK_BACK_TO_ACTION);

                    return RedirectToAction(goaction, gocontroller);
                }
                if (TempData.ContainsKey(CDictionary.SK_BACK_TO_PARAM))
                {
                    //可以帶入參數+值,下面是一組key+value的情況
                    string gocontroller = TempData[CDictionary.SK_BACK_TO_CONTROLLER].ToString();
                    string goaction = TempData[CDictionary.SK_BACK_TO_ACTION].ToString();
                    //這邊帶回來的假設是id=8
                    string param = TempData[CDictionary.SK_BACK_TO_PARAM].ToString();
                    TempData.Remove(CDictionary.SK_BACK_TO_CONTROLLER);
                    TempData.Remove(CDictionary.SK_BACK_TO_ACTION);
                    TempData.Remove(CDictionary.SK_BACK_TO_PARAM);
                    //把param切開
                    var keyValuePairs = param.Split('=');
                    var routeValues = new RouteValueDictionary
                        {
                            { keyValuePairs[0], keyValuePairs[1]}
                        };
                    return RedirectToAction(goaction, gocontroller, routeValues);
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
            Company com = _context.Companies.FirstOrDefault(
                t => t.TaxIdNumber.Equals(vm.clUsername) && t.Password.Equals(vm.clPassword));
            if (com != null && com.Password.Equals(vm.clPassword))
            {
                //string json = JsonSerializer.Serialize(mem);
                HttpContext.Session.SetString(CDictionary.SK_LOGGEDIN_COMPANY, com.CompanyId.ToString());
                HttpContext.Session.SetString(CDictionary.SK_LOGGEDIN_COMPANY_NAME, com.CompanyName.ToString());
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
            Employee emp = _context.Employees.FirstOrDefault(
                t => t.EmployeeAccount.Equals(vm.clUsername) && t.EmployeePassword.Equals(vm.clPassword));
            if (emp != null && emp.EmployeePassword.Equals(vm.clPassword))
            {
                //string json = JsonSerializer.Serialize(mem);
                HttpContext.Session.SetString(CDictionary.SK_LOGGEDIN_EMPLOYEE, emp.EmployeeId.ToString());
                HttpContext.Session.SetString(CDictionary.SK_LOGGEDIN_EMPLOYEE_NAME, "Admin");
                return RedirectToAction("Review", "Platform");
            }
            ViewBag.Message = "帳號或密碼錯誤";
            return View();
        }

        public ActionResult CompanySignUp()
        {
            List<SelectListItem> country = new List<SelectListItem>();
            foreach (var c in _context.CountryLists)
            {
                SelectListItem cnty = new SelectListItem()
                {
                    Value = c.Country,
                    Text = c.Country
                };
                country.Add(cnty);
            }
            List<SelectListItem> cities = new List<SelectListItem>();
            foreach (var c in _context.CityLists)
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
            c.Enable = true;
            _context.Companies.Add(c);
            _context.SaveChanges();
            return RedirectToAction("CompanyLogin", "Login");
        }

        public ActionResult SignUp()
        {
            List<SelectListItem> cts = new List<SelectListItem>();
            foreach (var c in _context.CityLists)
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
            _context.Members.Add(m.member);
            _context.SaveChanges();

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
            Member mem = _context.Members.Where(m => m.Email.Equals(HttpContext.Session.GetString(CDictionary.SK_RESET_PWD_EMAIL))).FirstOrDefault();
            if (mem != null)
            {
                mem.Verified = true;
                _context.SaveChanges();
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove(CDictionary.SK_LOGGEDIN_COMPANY);
            HttpContext.Session.Remove(CDictionary.SK_LOGGEDIN_COMPANY_NAME);
            HttpContext.Session.Remove(CDictionary.SK_LOGGEDIN_USER);
            HttpContext.Session.Remove(CDictionary.SK_LOGGEDIN_USER_NAME);
            HttpContext.Session.Remove(CDictionary.SK_LOGGEDIN_EMPLOYEE);
            HttpContext.Session.Remove(CDictionary.SK_LOGGEDIN_EMPLOYEE_NAME);

            return RedirectToAction("HomePage", "Homepage");
        }

        public bool CheckEmail(string Email)
        {
            return _context.Members.Any(m => m.Email == Email);
        }

        public bool CheckPhone(string Phone)
        {
            return _context.Members.Any(m => m.Phone == Phone);
        }
        public bool CheckTaxNo(string TaxNo)
        {
            return _context.Companies.Any(m => m.TaxIdNumber == TaxNo);
        }
        public bool CheckCEmail(string Email)
        {
            return _context.Companies.Any(m => m.Email == Email);
        }

    }
}
