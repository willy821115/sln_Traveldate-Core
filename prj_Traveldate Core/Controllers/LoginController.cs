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
            db.Members.Add(m.member);
            db.SaveChanges();

            //TempData["resultMsg"] = "註冊成功！登入後即可啟用會員功能";
            //TempData["destination"] = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + Url.Content("~/") + "Login/Login";

            return RedirectToAction("Login");
        }

        public IActionResult ForgotPwd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPwd(CForgotPwdViewModel vm)
        {
            TraveldateContext _context = new TraveldateContext();
            // 檢查資料庫是否有這個帳號
            var mailCheck = _context.Members.Where(m => m.Email.Equals(vm.email)).FirstOrDefault();
            if (mailCheck == null)
            {
                RedirectToAction("SentResetMail");
            }

            string UserEmail = mailCheck.Email;


            // 取得系統自定密鑰，在 Web.config 設定
            string SecretKey = _configuration["SendMailSettings:SecretKey"];

            // 產生帳號+時間驗證碼
            string sVerify = vm.email + "|" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            // 將驗證碼使用 3DES 加密
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] buf = Encoding.UTF8.GetBytes(SecretKey);
            byte[] result = md5.ComputeHash(buf);
            string md5Key = BitConverter.ToString(result).Replace("-", "").ToLower().Substring(0, 24);
            DES.Key = UTF8Encoding.UTF8.GetBytes(md5Key);
            DES.Mode = CipherMode.ECB;
            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            byte[] Buffer = UTF8Encoding.UTF8.GetBytes(sVerify);
            sVerify = Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length)); // 3DES 加密後驗證碼

            // 將加密後密碼使用網址編碼處理
            sVerify = HttpUtility.UrlEncode(sVerify);

            // 網站網址
            string webPath = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + Url.Content("~/");

            // 從信件連結回到重設密碼頁面
            string receivePage = "Login/ResetPwd";

            // 信件內容範本
            string mailContent = "點擊以下連結，即可回到Traveldate重新設定密碼，連結將在 30 分鐘後失效。<br><br>";
            mailContent = mailContent + "<a href='" + webPath + receivePage + "?verify=" + sVerify + "'  target='_blank'>點我重設密碼</a>";

            // 信件主題
            string mailSubject = "Traveldate重設密碼連結";

            // Google 發信帳號密碼
            string GoogleMailUserID = _configuration["SendMailSettings:GoogleMailUserID"];
            string GoogleMailUserPwd = _configuration["SendMailSettings:GoogleMailUserPwd"];

            // 使用 Google Mail Server 發信
            string SmtpServer = "smtp.gmail.com";
            int SmtpPort = 587;
            MailMessage mms = new MailMessage();
            mms.From = new MailAddress(GoogleMailUserID);
            mms.Subject = mailSubject;
            mms.Body = mailContent;
            mms.IsBodyHtml = true;
            mms.SubjectEncoding = Encoding.UTF8;
            mms.To.Add(new MailAddress(UserEmail));
            using (SmtpClient client = new SmtpClient(SmtpServer, SmtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(GoogleMailUserID, GoogleMailUserPwd);//寄信帳密 
                client.Send(mms); //寄出信件
            }
            
            return RedirectToAction("SentResetMail");
        }


        public IActionResult SentResetMail()
        {
            return View();
        }

        public IActionResult ResetPwd(string verify)
        {
            // 由信件連結回來會帶參數 verify

            if (verify==null)
            {
                TempData["ErrorMsg"] = "未傳入驗證碼";
                return RedirectToAction("Login");
            }

            // 取得系統自定密鑰，在 Web.config 設定
            string SecretKey = _configuration["SendMailSettings:SecretKey"];

            try
            {
                // 使用 3DES 解密驗證碼
                TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] buf = Encoding.UTF8.GetBytes(SecretKey);
                byte[] md5result = md5.ComputeHash(buf);
                string md5Key = BitConverter.ToString(md5result).Replace("-", "").ToLower().Substring(0, 24);
                DES.Key = UTF8Encoding.UTF8.GetBytes(md5Key);
                DES.Mode = CipherMode.ECB;
                DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                ICryptoTransform DESDecrypt = DES.CreateDecryptor();
                byte[] Buffer = Convert.FromBase64String(verify);
                string deCode = UTF8Encoding.UTF8.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));

                verify = deCode; //解密後還原資料
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "驗證碼錯誤";
                RedirectToAction("Login");
            }

            // 取出帳號
            string UserID = verify.Split('|')[0];

            // 取得重設時間
            string ResetTime = verify.Split('|')[1];

            // 檢查時間是否超過 30 分鐘
            DateTime dResetTime = Convert.ToDateTime(ResetTime);
            TimeSpan TS = new System.TimeSpan(DateTime.Now.Ticks - dResetTime.Ticks);
            double diff = Convert.ToDouble(TS.TotalMinutes);
            if (diff > 30)
            {
                TempData["ErrorMsg"] = "驗證碼已過期，請再試一次";
                RedirectToAction("Login");
            }

            // 驗證碼檢查成功，加入 Session
            HttpContext.Session.SetString(CDictionary.SK_RESET_PWD_EMAIL, UserID);

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

    }
}
