using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using prj_Traveldate_Core.Models.MyModels;
using Microsoft.AspNetCore.Http;
using System;
using prj_Traveldate_Core.ViewModels;

namespace prj_Traveldate_Core.Controllers
{
    public class LoginApiController : Controller
    {
        TraveldateContext _context;
        private readonly IConfiguration _configuration;
        HttpContext _httpContext;


        public LoginApiController(IConfiguration configuration, HttpContext httpContext)
        {
            _context = new TraveldateContext();
            _configuration = configuration;
            _httpContext = httpContext;
        }


        public string CreateVerifyLink(CEmailVerify data)
        {
            // 取得系統自定密鑰，在 Web.config 設定
            string SecretKey = _configuration["SendMailSettings:SecretKey"];

            // 產生帳號+時間驗證碼
            string sVerify = data.Email + "|" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

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
            string webPath = data.scheme + "://" + data.host + "/";

            // 從信件連結回到重設密碼頁面
            string receivePage = data.receivePage;

            // 認證連結
            string link = webPath + receivePage + "?verify=" + sVerify;

            return link;
        }


        public void SimplySendMail(string mailSubject, string mailContent, List<string> UserEmail)
        {
            // mailSubject : 信件主旨
            // mailContent : 信件內文
            // UserEmail : 收件者的email(可多個)

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

            foreach (string email in UserEmail)
            {
                mms.To.Add(new MailAddress(email));
            }
            using (SmtpClient client = new SmtpClient(SmtpServer, SmtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(GoogleMailUserID, GoogleMailUserPwd);//寄信帳密 
                client.Send(mms); //寄出信件
            }
        }

        //有圖片的email
        public void SimplySendMail(string mailSubject, List<string> UserEmail, AlternateView alv)
        {
            // mailSubject : 信件主旨
            // mailContent : 信件內文
            // UserEmail : 收件者的email(可多個)

            // Google 發信帳號密碼
            string GoogleMailUserID = _configuration["SendMailSettings:GoogleMailUserID"];
            string GoogleMailUserPwd = _configuration["SendMailSettings:GoogleMailUserPwd"];

            // 使用 Google Mail Server 發信
            string SmtpServer = "smtp.gmail.com";
            int SmtpPort = 587;
            MailMessage mms = new MailMessage();
            mms.AlternateViews.Add(alv);
            mms.From = new MailAddress(GoogleMailUserID);
            mms.Subject = mailSubject;
            mms.IsBodyHtml = true;
            mms.SubjectEncoding = Encoding.UTF8;

            foreach (string email in UserEmail)
            {
                mms.To.Add(new MailAddress(email));
            }
            using (SmtpClient client = new SmtpClient(SmtpServer, SmtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(GoogleMailUserID, GoogleMailUserPwd);//寄信帳密 
                client.Send(mms); //寄出信件
            }
        }

        public bool VerifyCode(string verify)
        {

            // 由信件連結回來會帶參數 verify

            if (verify == null)
            {
                TempData["ErrorMsg"] = "未傳入驗證碼";
                return false;
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
                return false;
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
                return false;
            }

            // 驗證碼檢查成功，加入 Session
            _httpContext.Session.SetString(CDictionary.SK_RESET_PWD_EMAIL, UserID);
            return true;
        }



    }
}
