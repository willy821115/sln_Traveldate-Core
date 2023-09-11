//using AspNetCore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualBasic;
using NuGet.Versioning;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace prj_Traveldate_Core.Controllers
{
    public class MemberController : SuperController
    {
        TraveldateContext context = new TraveldateContext();

        private IWebHostEnvironment _enviro = null;//要找到照片串流的路徑需要IWebHostEnvironment
        public MemberController(IWebHostEnvironment p) //利用建構子將p注入全域的_enviro來使用，因為interface無法被new
        {
            _enviro = p;
        }
        public IActionResult Index() // 左側欄 先維持原版V
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            Member mem2 = (from m in context.Members where (m.MemberId == MemberId) select m).FirstOrDefault();
            #region 原本load二進位的頭像圖片用
            //原本load二進位的頭像圖片用
            //if (mem2 != null)
            //{
            //    byte[] photo = mem2.Photo;
            //    if (photo != null)
            //    {
            //        string base64String = Convert.ToBase64String(photo);
            //        ViewBag.PhotoBase64 = "data:image/jpeg;base64," + base64String;
            //    }
            //};
            //ViewBag.photo = mem2;
            #endregion
            if (mem2 != null)
            {
                ViewBag.image = mem2.ImagePath;
            }
            //===============================我是分隔線===================================//
            var countsForum = from m in context.Members
                              join fl in context.ForumLists
                              on m.MemberId equals fl.MemberId
                              where m.MemberId == MemberId
                              select new CForumListViewModel2
                              {
                                  Title = fl.Title
                              };
            ViewBag.ForumLists = countsForum.ToList().Count();

            var countscomment = from m in context.Members
                                join cm in context.CommentLists
                                on m.MemberId equals cm.MemberId
                                join pl in context.ProductLists
                                on cm.ProductId equals pl.ProductId
                                where m.MemberId == MemberId
                                select new CcommentListViewModel
                                {
                                    Title = cm.Title,
                                };
            ViewBag.countscomment = countscomment.ToList().Count();

            var countsorder = from o in context.OrderDetails
                              where o.Order.Member.MemberId == MemberId
                              select new COrdersViewModel { ProductName = o.Trip.Product.ProductName };

            ViewBag.countsorder = countsorder.ToList().Count();

            var countsfavorite = from pl in context.ProductLists
                                 join f in context.Favorites
                                 on pl.ProductId equals f.ProductId
                                 join m in context.Members
                                 on f.MemberId equals m.MemberId
                                 where m.MemberId == MemberId
                                 select new CfavoriteListViewModel
                                 {
                                     ProductName = pl.ProductName,
                                 };
            ViewBag.countsfavorite = countsfavorite.ToList().Count();

            var countscoupon = from m in context.Members
                               join c in context.Coupons
                               on m.MemberId equals c.MemberId
                               join cl in context.CouponLists
                               on c.CouponListId equals cl.CouponListId
                               where m.MemberId == MemberId
                               select new couponListViewModel
                               {
                                   CouponName = cl.CouponName,
                               };
            ViewBag.countscoupon = countscoupon.ToList().Count();

            //===============================我是分隔線===================================//

            Member x = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            var levelvm = from m in context.Members
                          join l in context.LevelLists
                          on m.LevelId equals l.LevelId
                          where MemberId == m.MemberId
                          select m.LevelId;
            if (x.LevelId == 1)
                ViewBag.level = "一般會員";
            else if (x.LevelId == 2)
                ViewBag.level = "白銀會員";
            else if (x.LevelId == 3)
                ViewBag.level = "黃金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View();
        }
        private string email; // 私有字段

        //public string Email
        //{
        //    get
        //    {
        //        return email; // 只提供 get 方法，不提供 set 方法
        //    }
        //}
        public IActionResult basicInfo() //基本資料設定 V
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            Member memshow = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            var cmvm =context.Members
                    .Where(m => m.MemberId == MemberId)
                    .Select(m => new CMemberForBasicInfoViewModel
                    {
                        LastName = m.LastName,
                        FirstName = m.FirstName,
                        Gender = m.Gender,
                        BirthDate = m.BirthDate,
                        Phone = m.Phone,
                        Email = m.Email,
                        Password=m.Password,
                        LevelId = m.LevelId,
                        ImagePath=m.ImagePath                       
                    })
                    .FirstOrDefault();

            Member mem2 =(from m in  context.Members where (m.MemberId == MemberId) select m).FirstOrDefault() ;
            #region 原本load二進位的頭像圖片用
            //原本load二進位的頭像圖片用
            //if (mem2 != null)
            //{
            //    byte[] photo = mem2.Photo;
            //    if (photo != null)
            //    {
            //        string base64String = Convert.ToBase64String(photo);
            //        ViewBag.PhotoBase64 = "data:image/jpeg;base64," + base64String;
            //    }
            //};
            //ViewBag.photo = mem2;
            #endregion
            if (mem2 != null)
            {
                ViewBag.image = mem2.ImagePath;
            }
            //===============================我是分隔線===================================//
            var countsForum = from m in context.Members
                              join fl in context.ForumLists
                              on m.MemberId equals fl.MemberId
                              where m.MemberId == MemberId
                              select new CForumListViewModel2
                              {
                                  Title = fl.Title
                              };
            ViewBag.ForumLists = countsForum.ToList().Count();

            var countscomment = from m in context.Members
                                join cm in context.CommentLists
                                on m.MemberId equals cm.MemberId
                                join pl in context.ProductLists
                                on cm.ProductId equals pl.ProductId
                                where m.MemberId == MemberId
                                select new CcommentListViewModel
                                {
                                    Title = cm.Title,
                                };
            ViewBag.countscomment = countscomment.ToList().Count();

            var countsorder = from o in context.OrderDetails
                              where o.Order.Member.MemberId == MemberId
                              select new COrdersViewModel { ProductName = o.Trip.Product.ProductName };

            ViewBag.countsorder = countsorder.ToList().Count();

            var countsfavorite = from pl in context.ProductLists
                                 join f in context.Favorites
                                 on pl.ProductId equals f.ProductId
                                 join m in context.Members
                                 on f.MemberId equals m.MemberId
                                 where m.MemberId == MemberId
                                 select new CfavoriteListViewModel
                                 {
                                     ProductName = pl.ProductName,
                                 };
            ViewBag.countsfavorite = countsfavorite.ToList().Count();

            var countscoupon = from m in context.Members
                               join c in context.Coupons
                               on m.MemberId equals c.MemberId
                               join cl in context.CouponLists
                               on c.CouponListId equals cl.CouponListId
                               where m.MemberId == MemberId
                               select new couponListViewModel
                               {
                                   CouponName = cl.CouponName,
                               };
            ViewBag.countscoupon = countscoupon.ToList().Count();

            //===============================我是分隔線===================================//
            var levelvm = from m in context.Members
                          join l in context.LevelLists
                          on m.LevelId equals l.LevelId
                          where MemberId == m.MemberId
                          select m;
            if (memshow.LevelId == 1)
                ViewBag.level = "一般會員";
            else if (memshow.LevelId == 2)
                ViewBag.level = "白銀會員";
            else if (memshow.LevelId == 3)
                ViewBag.level = "黃金會員";
            else
                ViewBag.level = "黑鑽會員";

            //if (cmvm.FirstName == cmvm.FirstName)
            //    ViewBag.firstName = cmvm.FirstName;

            //if (cmvm.LastName == cmvm.LastName)
            //    ViewBag.LastName = cmvm.LastName;

            ViewBag.firstName = memshow.FirstName;
            ViewBag.LastName = memshow.LastName;

            return View(cmvm);
        }
        [HttpPost]
        public IActionResult basicInfo(CMemberForBasicInfoViewModel edit, IFormFile photos) //基本資料設定edit V
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            Member mDB = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            try
            {
                if (mDB != null)
                {
                    mDB.FirstName = edit.FirstName;
                    mDB.LastName = edit.LastName;
                    mDB.Gender = edit.Gender;
                    mDB.BirthDate = edit.BirthDate;
                    mDB.Phone = edit.Phone;
                    mDB.Email = edit.Email;
                    //mDB.MemberId = MemberId;
                    //mDB.ImagePath= edit.ImagePath;

                    mDB.Password = edit.Password;

                    context.SaveChanges();
                };

                if (edit.photos != null)
                {
                    //foreach (IFormFile photo in edit.photos) 
                    //{
                    string photoName = Guid.NewGuid().ToString() + ".jpg";//用Guid產生一個系統上不會重複的代碼，重新命名圖片
                    mDB.ImagePath = photoName;
                    mDB.MemberId = MemberId;
                    photos.CopyTo(new FileStream(_enviro.WebRootPath + "/images/" + photoName, FileMode.Create));
                    //context.Members.Add(photoName);
                    context.SaveChanges();
                    //}              
                }
                Thread.Sleep(3000);
                return RedirectToAction("basicInfo");
            }
            catch(Exception ex)
            {
                return RedirectToAction("basicInfo");
            }
            #region 先註解掉的程式碼
            //context.Members.ToList();

            //CMemberLevelViewModel vmml=new CMemberLevelViewModel();

            //var levelvm=from m in vmml.Member
            //          join l in vmml.LevelList
            //          on m.LevelId equals l.LevelId
            //          where m.MemberId== MemberId
            //          select m;
            #endregion
        }
        public IActionResult passwordChange() //密碼更改 先維持原版V
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            //CpasswordChangeViewModel prd = new CpasswordChangeViewModel();
            var datas = context.Members.Where(m => m.MemberId == MemberId).Select(m=>new CpasswordChangeViewModel 
            { Password = m.Password}).FirstOrDefault();

            //prd.MemberId = MemberId;
            //Member x = context.Members.FirstOrDefault(m => m.MemberId ==MemberId);
            Member mem2 = (from m in context.Members where (m.MemberId == MemberId) select m).FirstOrDefault();
            #region 原本load二進位的頭像圖片用
            //原本load二進位的頭像圖片用
            //if (mem2 != null)
            //{
            //    byte[] photo = mem2.Photo;
            //    if (photo != null)
            //    {
            //        string base64String = Convert.ToBase64String(photo);
            //        ViewBag.PhotoBase64 = "data:image/jpeg;base64," + base64String;
            //    }
            //};
            //ViewBag.photo = mem2;
            #endregion
            if (mem2 != null)
            {
                ViewBag.image = mem2.ImagePath;
            }
            //===============================我是分隔線===================================//
            var countsForum = from m in context.Members
                              join fl in context.ForumLists
                              on m.MemberId equals fl.MemberId
                              where m.MemberId == MemberId
                              select new CForumListViewModel2
                              {
                                  Title = fl.Title
                              };
            ViewBag.ForumLists = countsForum.ToList().Count();

            var countscomment = from m in context.Members
                                join cm in context.CommentLists
                                on m.MemberId equals cm.MemberId
                                join pl in context.ProductLists
                                on cm.ProductId equals pl.ProductId
                                where m.MemberId == MemberId
                                select new CcommentListViewModel
                                {
                                    Title = cm.Title,
                                };
            ViewBag.countscomment = countscomment.ToList().Count();

            var countsorder = from o in context.OrderDetails
                              where o.Order.Member.MemberId == MemberId
                              select new COrdersViewModel { ProductName = o.Trip.Product.ProductName };

            ViewBag.countsorder = countsorder.ToList().Count();

            var countsfavorite = from pl in context.ProductLists
                                 join f in context.Favorites
                                 on pl.ProductId equals f.ProductId
                                 join m in context.Members
                                 on f.MemberId equals m.MemberId
                                 where m.MemberId == MemberId
                                 select new CfavoriteListViewModel
                                 {
                                     ProductName = pl.ProductName,
                                 };
            ViewBag.countsfavorite = countsfavorite.ToList().Count();

            var countscoupon = from m in context.Members
                               join c in context.Coupons
                               on m.MemberId equals c.MemberId
                               join cl in context.CouponLists
                               on c.CouponListId equals cl.CouponListId
                               where m.MemberId == MemberId
                               select new couponListViewModel
                               {
                                   CouponName = cl.CouponName,
                               };
            ViewBag.countscoupon = countscoupon.ToList().Count();

            //===============================我是分隔線===================================//
            var levelvm = from m in context.Members
                          join l in context.LevelLists
                          on m.LevelId equals l.LevelId
                          where MemberId == m.MemberId
                          select m.LevelId;
            if (mem2.LevelId == 1)
                ViewBag.level = "一般會員";
            else if (mem2.LevelId == 2)
                ViewBag.level = "白銀會員";
            else if (mem2.LevelId == 3)
                ViewBag.level = "黃金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (mem2.FirstName == mem2.FirstName)
                ViewBag.firstName = mem2.FirstName;

            if (mem2.LastName == mem2.LastName)
                ViewBag.LastName = mem2.LastName;

            return View(datas);
            #region 先註解掉的程式碼
            //CpasswordChangeViewModel mem = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            //Member mem=context.Members.FirstOrDefault(m=>m.MemberId==MemberId);
            //Member m=context.Members.FirstOrDefault(m=>m.Password==prd.txtNewPassword);
            #endregion
            if (mem2 != null)
            {
                ViewBag.image = mem2.ImagePath;
            }
        }
        [HttpPost]
        public IActionResult passwordChange(CpasswordChangeViewModel edit) //密碼更改 edit V
        {
            int memberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            Member mDB = context.Members.FirstOrDefault(m => m.MemberId == memberId);
            try
            {
                if (mDB != null)
                {
                    if (edit.txtNewPassword == edit.txtCheckPassword)
                    {
                        mDB.Password = edit.txtNewPassword;
                        context.Entry(mDB).State = EntityState.Modified;
                        context.SaveChanges();

                        Thread.Sleep(3000);
                    }
                    else if (edit.txtNewPassword != edit.txtCheckPassword)
                    {
                        Thread.Sleep(60000);
                        return RedirectToAction("passwordChange");
                    }
                }
                return RedirectToAction("passwordChange");
            }
            catch(Exception ex)
            {
                return RedirectToAction("passwordChange");
            }
        }
    
        public IActionResult couponList() //優惠券清單 new V
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            var datas = from m in context.Members
                        join c in context.Coupons
                        on m.MemberId equals c.MemberId
                        join cl in context.CouponLists
                        on c.CouponListId equals cl.CouponListId
                        where m.MemberId == MemberId
                        select new couponListViewModel
                        {
                            CouponListId = cl.CouponListId,
                            CouponName = cl.CouponName,
                            Discount =(cl.Discount),
                            Description = cl.Description,
                            DueDate = cl.DueDate,  
                            ImagePath= cl.ImagePath,
                        };
            Member mem2 = (from m in context.Members where (m.MemberId == MemberId) select m).FirstOrDefault();
            #region 原本load二進位的頭像圖片用
            //原本load二進位的頭像圖片用
            //if (mem2 != null)
            //{
            //    byte[] photo = mem2.Photo;
            //    if (photo != null)
            //    {
            //        string base64String = Convert.ToBase64String(photo);
            //        ViewBag.PhotoBase64 = "data:image/jpeg;base64," + base64String;
            //    }
            //};
            //ViewBag.photo = mem2;
            #endregion
            if (mem2 != null)
            {
                ViewBag.image = mem2.ImagePath;
            }
            //===============================我是分隔線===================================//
            var countsForum = from m in context.Members
                              join fl in context.ForumLists
                              on m.MemberId equals fl.MemberId
                              where m.MemberId == MemberId
                              select new CForumListViewModel2
                              {
                                  Title = fl.Title
                              };
            ViewBag.ForumLists = countsForum.ToList().Count();

            var countscomment = from m in context.Members
                                join cm in context.CommentLists
                                on m.MemberId equals cm.MemberId
                                join pl in context.ProductLists
                                on cm.ProductId equals pl.ProductId
                                where m.MemberId == MemberId
                                select new CcommentListViewModel
                                {
                                    Title = cm.Title,
                                };
            ViewBag.countscomment = countscomment.ToList().Count();

            var countsorder = from o in context.OrderDetails
                              where o.Order.Member.MemberId == MemberId
                              select new COrdersViewModel { ProductName = o.Trip.Product.ProductName };

            ViewBag.countsorder = countsorder.ToList().Count();

            var countsfavorite = from pl in context.ProductLists
                                 join f in context.Favorites
                                 on pl.ProductId equals f.ProductId
                                 join m in context.Members
                                 on f.MemberId equals m.MemberId
                                 where m.MemberId == MemberId
                                 select new CfavoriteListViewModel
                                 {
                                     ProductName = pl.ProductName,
                                 };
            ViewBag.countsfavorite = countsfavorite.ToList().Count();

            var countscoupon = from m in context.Members
                               join c in context.Coupons
                               on m.MemberId equals c.MemberId
                               join cl in context.CouponLists
                               on c.CouponListId equals cl.CouponListId
                               where m.MemberId == MemberId
                               select new couponListViewModel
                               {
                                   CouponName = cl.CouponName,
                               };
            ViewBag.countscoupon = countscoupon.ToList().Count();

            //===============================我是分隔線===================================//
            Member x = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            var levelvm = from m in context.Members
                          join l in context.LevelLists
                          on m.LevelId equals l.LevelId
                          where MemberId == m.MemberId
                          select m.LevelId;
            if (x.LevelId == 1)
                ViewBag.level = "一般會員";
            else if (x.LevelId == 2)
                ViewBag.level = "白銀會員";
            else if (x.LevelId == 3)
                ViewBag.level = "黃金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View(datas);
        }

        public IActionResult showCompanion() //顯示常用旅伴
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            var datas = from m in context.Members
                        join cm in context.Companions
                        on m.MemberId equals cm.MemberId
                        where m.MemberId == MemberId
                        select new CCompanionViewModel
                        {
                            LastName = cm.LastName,
                            FirstName = cm.FirstName,
                            Phone = cm.Phone,
                            BirthDate= cm.BirthDate,
                        };
            Member mem2 = (from m in context.Members where (m.MemberId == MemberId) select m).FirstOrDefault();
            #region 原本load二進位的頭像圖片用
            //原本load二進位的頭像圖片用
            //if (mem2 != null)
            //{
            //    byte[] photo = mem2.Photo;
            //    if (photo != null)
            //    {
            //        string base64String = Convert.ToBase64String(photo);
            //        ViewBag.PhotoBase64 = "data:image/jpeg;base64," + base64String;
            //    }
            //};
            //ViewBag.photo = mem2;
            #endregion
            if (mem2 != null)
            {
                ViewBag.image = mem2.ImagePath;
            }
            //===============================我是分隔線===================================//
            var countsForum = from m in context.Members
                              join fl in context.ForumLists
                              on m.MemberId equals fl.MemberId
                              where m.MemberId == MemberId
                              select new CForumListViewModel2
                              {
                                  Title = fl.Title
                              };
            ViewBag.ForumLists = countsForum.ToList().Count();

            var countscomment = from m in context.Members
                                join cm in context.CommentLists
                                on m.MemberId equals cm.MemberId
                                join pl in context.ProductLists
                                on cm.ProductId equals pl.ProductId
                                where m.MemberId == MemberId
                                select new CcommentListViewModel
                                {
                                    Title = cm.Title,
                                };
            ViewBag.countscomment = countscomment.ToList().Count();

            var countsorder = from o in context.OrderDetails
                              where o.Order.Member.MemberId == MemberId
                              select new COrdersViewModel { ProductName = o.Trip.Product.ProductName };

            ViewBag.countsorder = countsorder.ToList().Count();

            var countsfavorite = from pl in context.ProductLists
                                 join f in context.Favorites
                                 on pl.ProductId equals f.ProductId
                                 join m in context.Members
                                 on f.MemberId equals m.MemberId
                                 where m.MemberId == MemberId
                                 select new CfavoriteListViewModel
                                 {
                                     ProductName = pl.ProductName,
                                 };
            ViewBag.countsfavorite = countsfavorite.ToList().Count();

            var countscoupon = from m in context.Members
                               join c in context.Coupons
                               on m.MemberId equals c.MemberId
                               join cl in context.CouponLists
                               on c.CouponListId equals cl.CouponListId
                               where m.MemberId == MemberId
                               select new couponListViewModel
                               {
                                   CouponName = cl.CouponName,
                               };
            ViewBag.countscoupon = countscoupon.ToList().Count();

            //===============================我是分隔線===================================//
            Member x = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            var levelvm = from m in context.Members
                          join l in context.LevelLists
                          on m.LevelId equals l.LevelId
                          where MemberId == m.MemberId
                          select m.LevelId;
            if (x.LevelId == 1)
                ViewBag.level = "一般會員";
            else if (x.LevelId == 2)
                ViewBag.level = "白銀會員";
            else if (x.LevelId == 3)
                ViewBag.level = "黃金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View(datas.Distinct());
        }
        public IActionResult addCompanion() //新增旅伴資料 V
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            CCompanionViewModel cm = new CCompanionViewModel();
            cm.MemberId = MemberId;
            Member mem2 = (from m in context.Members where (m.MemberId == MemberId) select m).FirstOrDefault();
            #region 原本load二進位的頭像圖片用
            //原本load二進位的頭像圖片用
            //if (mem2 != null)
            //{
            //    byte[] photo = mem2.Photo;
            //    if (photo != null)
            //    {
            //        string base64String = Convert.ToBase64String(photo);
            //        ViewBag.PhotoBase64 = "data:image/jpeg;base64," + base64String;
            //    }
            //};
            //ViewBag.photo = mem2;
            #endregion
            if (mem2 != null)
            {
                ViewBag.image = mem2.ImagePath;
            }
            //===============================我是分隔線===================================//
            var countsForum = from m in context.Members
                              join fl in context.ForumLists
                              on m.MemberId equals fl.MemberId
                              where m.MemberId == MemberId
                              select new CForumListViewModel2
                              {
                                  Title = fl.Title
                              };
            ViewBag.ForumLists = countsForum.ToList().Count();

            var countscomment = from m in context.Members
                                join cmm in context.CommentLists
                                on m.MemberId equals cmm.MemberId
                                join pl in context.ProductLists
                                on cmm.ProductId equals pl.ProductId
                                where m.MemberId == MemberId
                                select new CcommentListViewModel
                                {
                                    Title = cmm.Title,
                                };
            ViewBag.countscomment = countscomment.ToList().Count();

            var countsorder = from o in context.OrderDetails
                              where o.Order.Member.MemberId == MemberId
                              select new COrdersViewModel { ProductName = o.Trip.Product.ProductName };

            ViewBag.countsorder = countsorder.ToList().Count();

            var countsfavorite = from pl in context.ProductLists
                                 join f in context.Favorites
                                 on pl.ProductId equals f.ProductId
                                 join m in context.Members
                                 on f.MemberId equals m.MemberId
                                 where m.MemberId == MemberId
                                 select new CfavoriteListViewModel
                                 {
                                     ProductName = pl.ProductName,
                                 };
            ViewBag.countsfavorite = countsfavorite.ToList().Count();

            var countscoupon = from m in context.Members
                               join c in context.Coupons
                               on m.MemberId equals c.MemberId
                               join cl in context.CouponLists
                               on c.CouponListId equals cl.CouponListId
                               where m.MemberId == MemberId
                               select new couponListViewModel
                               {
                                   CouponName = cl.CouponName,
                               };
            ViewBag.countscoupon = countscoupon.ToList().Count();

            //===============================我是分隔線===================================//

            Member x = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            var levelvm = from m in context.Members
                          join l in context.LevelLists
                          on m.LevelId equals l.LevelId
                          where MemberId == m.MemberId
                          select m.LevelId;
            if (x.LevelId == 1)
                ViewBag.level = "一般會員";
            else if (x.LevelId == 2)
                ViewBag.level = "白銀會員";
            else if (x.LevelId == 3)
                ViewBag.level = "黃金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View(cm);
        }
        [HttpPost]
        public IActionResult addCompanion(CCompanionViewModel vm) //新增旅伴資料Create V
        {
            try
            {
                if (
                     (string.IsNullOrEmpty(vm.LastName)) ||
                     (string.IsNullOrEmpty(vm.FirstName)) ||
                     (string.IsNullOrEmpty(vm.Phone))
                   )
                {
                    Thread.Sleep(60000);
                }
                else
                {
                    Companion cpDB = new Companion();
                    if (cpDB != null)
                    {
                        cpDB.LastName = vm.LastName;
                        cpDB.FirstName = vm.FirstName;
                        cpDB.Phone = vm.Phone;
                        cpDB.MemberId = vm.MemberId;
                        cpDB.BirthDate = vm.BirthDate;

                        context.Companions.Add(cpDB);
                        context.SaveChanges();
                        Thread.Sleep(3000);
                    }
                }
                return RedirectToAction("showCompanion");
            }
            catch(Exception ex)
            {
                return RedirectToAction("showCompanion");
            }
        }

        public IActionResult favoriteList() //收藏清單new V
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            var datas = from Members in context.Favorites
                        where (Members.MemberId == MemberId)
                        select new CfavoriteListViewModel
                        {
                            ProductName= Members.Product.ProductName,
                            Outline=Members.Product.Outline,
                            FavoriteId=Members.FavoriteId,

                        };
            Member mem2 = (from m in context.Members where (m.MemberId == MemberId) select m).FirstOrDefault();
            #region 原本load二進位的頭像圖片用
            //原本load二進位的頭像圖片用
            //if (mem2 != null)
            //{
            //    byte[] photo = mem2.Photo;
            //    if (photo != null)
            //    {
            //        string base64String = Convert.ToBase64String(photo);
            //        ViewBag.PhotoBase64 = "data:image/jpeg;base64," + base64String;
            //    }
            //};
            //ViewBag.photo = mem2;
            #endregion
            if (mem2 != null)
            {
                ViewBag.image = mem2.ImagePath;
            }
            //===============================我是分隔線===================================//
            var countsForum = from m in context.Members
                              join fl in context.ForumLists
                              on m.MemberId equals fl.MemberId
                              where m.MemberId == MemberId
                              select new CForumListViewModel2
                              {
                                  Title = fl.Title
                              };
            ViewBag.ForumLists = countsForum.ToList().Count();

            var countscomment = from m in context.Members
                                join cm in context.CommentLists
                                on m.MemberId equals cm.MemberId
                                join pl in context.ProductLists
                                on cm.ProductId equals pl.ProductId
                                where m.MemberId == MemberId
                                select new CcommentListViewModel
                                {
                                    Title = cm.Title,
                                };
            ViewBag.countscomment = countscomment.ToList().Count();

            var countsorder = from o in context.OrderDetails
                              where o.Order.Member.MemberId == MemberId
                              select new COrdersViewModel { ProductName = o.Trip.Product.ProductName };

            ViewBag.countsorder = countsorder.ToList().Count();

            var countsfavorite = from pl in context.ProductLists
                                 join f in context.Favorites
                                 on pl.ProductId equals f.ProductId
                                 join m in context.Members
                                 on f.MemberId equals m.MemberId
                                 where m.MemberId == MemberId
                                 select new CfavoriteListViewModel
                                 {
                                     ProductName = pl.ProductName,
                                 };
            ViewBag.countsfavorite = countsfavorite.ToList().Count();

            var countscoupon = from m in context.Members
                               join c in context.Coupons
                               on m.MemberId equals c.MemberId
                               join cl in context.CouponLists
                               on c.CouponListId equals cl.CouponListId
                               where m.MemberId == MemberId
                               select new couponListViewModel
                               {
                                   CouponName = cl.CouponName,
                               };
            ViewBag.countscoupon = countscoupon.ToList().Count();

            //===============================我是分隔線===================================//
            Member x = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            var levelvm = from m in context.Members
                          join l in context.LevelLists
                          on m.LevelId equals l.LevelId
                          where MemberId == m.MemberId
                          select m.LevelId;
            if (x.LevelId == 1)
                ViewBag.level = "一般會員";
            else if (x.LevelId == 2)
                ViewBag.level = "白銀會員";
            else if (x.LevelId == 3)
                ViewBag.level = "黃金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View(datas);
        }
        [HttpPost]
        public IActionResult favoriteList(int? id)  //收藏清單Delete V
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));

            //Member mm = context.Members.FirstOrDefault(p => p.MemberId == MemberId);
            Favorite ff = context.Favorites.Where(f => f.FavoriteId == id).FirstOrDefault();
            try
            {
                //if (mm !=null)
                //{
                if (ff != null)
                {
                    context.Favorites.Remove(ff);
                    context.SaveChanges();
                }
                Thread.Sleep(2000);
                return RedirectToAction("favoriteList");
            }
            catch (Exception ex)
            {
                return RedirectToAction("favoriteList");
            }
        }


        public IActionResult orderList(int? id, string? Title, string? Content, int? CommentScore, List<IFormFile> photos) //會員訂單new V 
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            //var datas = from o in context.OrderDetails//.GroupBy(i => i.Order.OrderId).Select(d => d.First()).ToList()
            //            from c in context.CommentLists
            //            where c.MemberId== MemberId  //&& o.Order.MemberId== MemberId //&& c.ProductId==o.Trip.Product.ProductId
            //            //where o.Order.Member.MemberId == MemberId
            //            group new { o, c } by o.Trip.Date into grouped
            //            select new COrdersViewModel { 
            //                Date =grouped.Key, //o.Trip.Date, 
            //                Datetime = string.Format("{0:yyyy-MM-dd}", grouped.First().o.Order.Datetime),//Datetime = string.Format("{0:yyyy-MM-dd}", o.Order.Datetime), 
            //                ProductName = grouped.First().o.Trip.Product.ProductName,//ProductName = o.Trip.Product.ProductName, 
            //                ProductId = grouped.First().o.Trip.Product.ProductId,//ProductId = o.Trip.Product. ProductId, 
            //                CommentScore = grouped.First().c.CommentScore,//CommentScore =c.CommentScore,
            //                Content = grouped.First().c.Content,//Content=c.Content,
            //                Title = grouped.First().c.Title,//Title=c.Title    
            //                //ImagePath = grouped.First().c.CommentPhotoLists.//ImagePath![0].ToString() //ImagePath
            //             };
            //08.31 調整
            var data5 = from orderdetails3 in context.OrderDetails
                        where orderdetails3.Order.Member.MemberId==MemberId
                        select new COrdersViewModel
                        {
                            Datetime = string.Format("{0:yyyy-MM-dd}", orderdetails3.Order.Datetime),
                            ProductName = orderdetails3.Trip.Product.ProductName,
                            ProductId = orderdetails3.Trip.ProductId,

                            //Date= orderdetails3.Trip.Date,
                            Date = string.Format("{0:yyyy-MM-dd}", orderdetails3.Trip.Date),
                            CommentScore =CommentScore,
                            Content = Content,
                            Title = Title,
                        };

            Member mem2 = (from m in context.Members where (m.MemberId == MemberId) select m).FirstOrDefault();
            #region 原本load二進位的頭像圖片用
            //原本load二進位的頭像圖片用
            //if (mem2 != null)
            //{
            //    byte[] photo = mem2.Photo;
            //    if (photo != null)
            //    {
            //        string base64String = Convert.ToBase64String(photo);
            //        ViewBag.PhotoBase64 = "data:image/jpeg;base64," + base64String;
            //    }
            //};
            //ViewBag.photo = mem2;
            #endregion
            if (mem2 != null)
            {
                ViewBag.image = mem2.ImagePath;
            }
            //===============================我是分隔線===================================//
            var countsForum = from m in context.Members
                              join fl in context.ForumLists
                              on m.MemberId equals fl.MemberId
                              where m.MemberId == MemberId
                              select new CForumListViewModel2
                              {
                                  Title = fl.Title
                              };
            ViewBag.ForumLists = countsForum.ToList().Count();

            var countscomment = from m in context.Members
                                join cm in context.CommentLists
                                on m.MemberId equals cm.MemberId
                                join pl in context.ProductLists
                                on cm.ProductId equals pl.ProductId
                                where m.MemberId == MemberId
                                select new CcommentListViewModel
                                {
                                    Title = cm.Title,
                                };
            ViewBag.countscomment = countscomment.ToList().Count();

            var countsorder = from o in context.OrderDetails
                              where o.Order.Member.MemberId == MemberId
                              select new COrdersViewModel { ProductName = o.Trip.Product.ProductName };

            ViewBag.countsorder = countsorder.ToList().Count();

            var countsfavorite = from pl in context.ProductLists
                                 join f in context.Favorites
                                 on pl.ProductId equals f.ProductId
                                 join m in context.Members
                                 on f.MemberId equals m.MemberId
                                 where m.MemberId == MemberId
                                 select new CfavoriteListViewModel
                                 {
                                     ProductName = pl.ProductName,
                                 };
            ViewBag.countsfavorite = countsfavorite.ToList().Count();

            
            var countscoupon = from m in context.Members
                               join c in context.Coupons
                               on m.MemberId equals c.MemberId
                               join cl in context.CouponLists
                               on c.CouponListId equals cl.CouponListId
                               where m.MemberId == MemberId
                               select new couponListViewModel
                               {
                                   CouponName = cl.CouponName,
                               };
            ViewBag.countscoupon = countscoupon.ToList().Count();

            //===============================我是分隔線===================================//
            Member x = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            var levelvm = from m in context.Members
                          join l in context.LevelLists
                          on m.LevelId equals l.LevelId
                          where MemberId == m.MemberId
                          select m.LevelId;
            if (x.LevelId == 1)
                ViewBag.level = "一般會員";
            else if (x.LevelId == 2)
                ViewBag.level = "白銀會員";
            else if (x.LevelId == 3)
                ViewBag.level = "黃金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View(data5);
            //return View(data2.Distinct());
        }

        #region 我的評論 0817(四)版 暫時用不到
        //public IActionResult commentList(int? id = 1) //我的評論V
        //{
        //    var datas = from m in context.Members
        //                join cm in context.CommentLists
        //                on m.MemberId equals cm.MemberId
        //                join pl in context.ProductLists
        //                on cm.ProductId equals pl.ProductId
        //                where m.MemberId == id
        //                select new CcommentListViewModel
        //                {
        //                    Title = cm.Title,
        //                    Content = cm.Content,
        //                    CommentScore = cm.CommentScore,
        //                    Date = cm.Date,
        //                    ProductName = pl.ProductName
        //                };
        //    return View(datas);
        //}
        #endregion

        public IActionResult commentList() //我的評論new V
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));

            var datas = from Member in context.CommentLists
                        where Member.MemberId == MemberId
                        select new CcommentListViewModel
                        {
                            Title = Member.Title,
                            Content = Member.Content,
                            CommentScore = Member.CommentScore,
                            Date = Member.Date,
                            ProductName = Member.Product.ProductName,
                            CommentId = Member.CommentId
                        };

            Member mem2 = (from m in context.Members where (m.MemberId == MemberId) select m).FirstOrDefault();
            #region 原本load二進位的頭像圖片用
            //原本load二進位的頭像圖片用
            //if (mem2 != null)
            //{
            //    byte[] photo = mem2.Photo;
            //    if (photo != null)
            //    {
            //        string base64String = Convert.ToBase64String(photo);
            //        ViewBag.PhotoBase64 = "data:image/jpeg;base64," + base64String;
            //    }
            //};
            //ViewBag.photo = mem2;
            #endregion
            if (mem2 != null)
            {
                ViewBag.image = mem2.ImagePath;
            }
            //===============================我是分隔線===================================//
            var countsForum = from m in context.Members
                              join fl in context.ForumLists
                              on m.MemberId equals fl.MemberId
                              where m.MemberId == MemberId
                              select new CForumListViewModel2
                              {
                                  Title = fl.Title
                              };
            ViewBag.ForumLists = countsForum.ToList().Count();

            var countscomment = from m in context.Members
                                join cm in context.CommentLists
                                on m.MemberId equals cm.MemberId
                                join pl in context.ProductLists
                                on cm.ProductId equals pl.ProductId
                                where m.MemberId == MemberId
                                select new CcommentListViewModel
                                {
                                    Title = cm.Title,
                                };
            ViewBag.countscomment = countscomment.ToList().Count();

            var countsorder = from o in context.OrderDetails
                              where o.Order.Member.MemberId == MemberId
                              select new COrdersViewModel { ProductName = o.Trip.Product.ProductName };

            ViewBag.countsorder = countsorder.ToList().Count();

            var countsfavorite = from pl in context.ProductLists
                                 join f in context.Favorites
                                 on pl.ProductId equals f.ProductId
                                 join m in context.Members
                                 on f.MemberId equals m.MemberId
                                 where m.MemberId == MemberId
                                 select new CfavoriteListViewModel
                                 {
                                     ProductName = pl.ProductName,
                                 };
            ViewBag.countsfavorite = countsfavorite.ToList().Count();

            var countscoupon = from m in context.Members
                               join c in context.Coupons
                               on m.MemberId equals c.MemberId
                               join cl in context.CouponLists
                               on c.CouponListId equals cl.CouponListId
                               where m.MemberId == MemberId
                               select new couponListViewModel
                               {
                                   CouponName = cl.CouponName,
                               };
            ViewBag.countscoupon = countscoupon.ToList().Count();

            //===============================我是分隔線===================================//
            Member x = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            var levelvm = from m in context.Members
                          join l in context.LevelLists
                          on m.LevelId equals l.LevelId
                          where MemberId == m.MemberId
                          select m.LevelId;
            if (x.LevelId == 1)
                ViewBag.level = "一般會員";
            else if (x.LevelId == 2)
                ViewBag.level = "白銀會員";
            else if (x.LevelId == 3)
                ViewBag.level = "黃金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View(datas.Distinct());
        }

        [HttpPost]
        public IActionResult commentList(int? id) //我的評論Delete V
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            try
            {
                if (id != null)
                {
                    // 使用 .Where() 來選擇所有匹配特定 CommentId 的紀錄
                    var recordsToDelete = context.CommentPhotoLists.Where(p => p.CommentId == id).ToList();

                    if (recordsToDelete.Count > 0)
                    {
                        // 使用 RemoveRange() 刪除所有匹配的紀錄
                        context.CommentPhotoLists.RemoveRange(recordsToDelete);
                        context.SaveChanges();
                    }
                }
                if (id != null)
                {
                    CommentList ff = context.CommentLists.Where(c => c.CommentId == id).FirstOrDefault();
                    //CommentList fff = context.CommentLists.Where(c => c.Date == c.Date).FirstOrDefault();
                    if (ff != null /*&& fff!=null*/)
                    {
                        context.CommentLists.Remove(ff);
                        context.SaveChanges();
                    }
                }
                Thread.Sleep(2000);
                return RedirectToAction("commentList");
            }
            catch(Exception ex)
            {
                return RedirectToAction("commentList");
            }
        }

        #region 添加評論view 暫時用不到2023.08.20
        public IActionResult addcomment(int? id) //添加評論 先維持舊版V
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            //var datas = from m in context.Members
            //            join cm in context.CommentLists
            //            on m.MemberId equals cm.MemberId
            //            join pl in context.ProductLists
            //            on cm.ProductId equals pl.ProductId
            //            where m.MemberId == MemberId
            //            select new CcommentListViewModel
            //            {
            //                Title = cm.Title,
            //                Content = cm.Content,
            //                CommentScore = cm.CommentScore,
            //                MemberId = m.MemberId,
            //                Date = cm.Date,
            //                ProductName = pl.ProductName,
            //                //ProductId = Convert.ToInt32(pl.ProductId),
            //                ProductId = id,
            //                CommentId= cm.CommentId,

            //            };
            CCommentListWrap z = new CCommentListWrap();
            z.MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            z.ProductId = id;


                Member mem2 = (from m in context.Members where (m.MemberId == MemberId) select m).FirstOrDefault();
            #region 原本load二進位的頭像圖片用
            //原本load二進位的頭像圖片用
            //if (mem2 != null)
            //{
            //    byte[] photo = mem2.Photo;
            //    if (photo != null)
            //    {
            //        string base64String = Convert.ToBase64String(photo);
            //        ViewBag.PhotoBase64 = "data:image/jpeg;base64," + base64String;
            //    }
            //};
            //ViewBag.photo = mem2;
            #endregion
            if (mem2 != null)
            {
                ViewBag.image = mem2.ImagePath;
            }
            //===============================我是分隔線===================================//
            var countsForum = from m in context.Members
                              join fl in context.ForumLists
                              on m.MemberId equals fl.MemberId
                              where m.MemberId == MemberId
                              select new CForumListViewModel2
                              {
                                  Title = fl.Title
                              };
            ViewBag.ForumLists = countsForum.ToList().Count();

            var countscomment = from m in context.Members
                                join cm in context.CommentLists
                                on m.MemberId equals cm.MemberId
                                join pl in context.ProductLists
                                on cm.ProductId equals pl.ProductId
                                where m.MemberId == MemberId
                                select new CcommentListViewModel
                                {
                                    Title = cm.Title,
                                };
            ViewBag.countscomment = countscomment.ToList().Count();

            var countsorder = from o in context.OrderDetails
                              where o.Order.Member.MemberId == MemberId
                              select new COrdersViewModel { ProductName = o.Trip.Product.ProductName };

            ViewBag.countsorder = countsorder.ToList().Count();

            var countsfavorite = from pl in context.ProductLists
                                 join f in context.Favorites
                                 on pl.ProductId equals f.ProductId
                                 join m in context.Members
                                 on f.MemberId equals m.MemberId
                                 where m.MemberId == MemberId
                                 select new CfavoriteListViewModel
                                 {
                                     ProductName = pl.ProductName,
                                 };
            ViewBag.countsfavorite = countsfavorite.ToList().Count();

            var countscoupon = from m in context.Members
                               join c in context.Coupons
                               on m.MemberId equals c.MemberId
                               join cl in context.CouponLists
                               on c.CouponListId equals cl.CouponListId
                               where m.MemberId == MemberId
                               select new couponListViewModel
                               {
                                   CouponName = cl.CouponName,
                               };
            ViewBag.countscoupon = countscoupon.ToList().Count();

            //===============================我是分隔線===================================//
            Member x = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            var levelvm = from m in context.Members
                          join l in context.LevelLists
                          on m.LevelId equals l.LevelId
                          where MemberId == m.MemberId
                          select m.LevelId;
            if (x.LevelId == 1)
                ViewBag.level = "一般會員";
            else if (x.LevelId == 2)
                ViewBag.level = "白銀會員";
            else if (x.LevelId == 3)
                ViewBag.level = "黃金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View(z);
        }
        #endregion

        [HttpPost]
        public IActionResult addcomment(CCommentListWrap vm) //添加評論Create V
        {
            //int? id, string? Title, string? Content, int? CommentScore, List<IFormFile> photos,string? ImagePath
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            //COrdersViewModel vm = new COrdersViewModel();
            //CCommentListWrap vm=new CCommentListWrap();
            //vm.ProductId= id;
            //vm.Title = Title;
            //vm.Content = Content;
            //vm.CommentScore = CommentScore;
            //vm.photos= photos;
            //vm.ImagePath = ImagePath;
            // vm.CommentId = comm;
            try
            {
                CommentList cmDB = new CommentList();
                if (cmDB != null)
                {
                    cmDB.Title = vm.Title;
                    cmDB.Content = vm.Content;
                    cmDB.CommentScore = vm.CommentScore;
                    cmDB.MemberId = vm.MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
                    cmDB.Date = DateTime.Now;
                    cmDB.ProductId = vm.ProductId;

                    context.CommentLists.Add(cmDB);
                    context.SaveChanges();
                }

                //CommentPhotoList photolist = context.CommentPhotoLists.FirstOrDefault(p => p.CommentId == vm.CommentId);
                int NewCommentID = context.CommentLists.Where(c => c.Title == vm.Title).Select(c => c.CommentId).FirstOrDefault();
                if (vm.photos != null)
                {
                    foreach (IFormFile photo in vm.photos)
                    {
                        CommentPhotoList photolistDB = new CommentPhotoList();
                        string photoName = Guid.NewGuid().ToString() + ".jpg";//用Guid產生一個系統上不會重複的代碼，重新命名圖片
                        photolistDB.ImagePath = photoName;
                        photolistDB.CommentId = NewCommentID;
                        photo.CopyTo(new FileStream(_enviro.WebRootPath + "/images/" + photoName, FileMode.Create));
                        context.CommentPhotoLists.Add(photolistDB);
                        context.SaveChanges();
                    }
                }
                Thread.Sleep(3000);
                return RedirectToAction("commentList");
            }
            catch(Exception ex)
            {
                return RedirectToAction("commentList");
            }
        }
        public IActionResult forumList() //我的揪團new V
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            //bool IsLike = context.LikeLists.Any(m => m.MemberId == MemberId && m.IsLike == true);
            //var IsLike2 = from isl in context.LikeLists where  isl.IsLike == true select new CForumListViewModel2 { IsLike = isl.IsLike };
            //var IsLike3 = context.LikeLists.Where(isl => isl.MemberId== MemberId && isl.IsLike == true);

            //ViewBag.countisLike3= IsLike3.ToList().Count();
            //bool IsLike = context.LikeLists.Where(m => m.MemberId == MemberId && m.IsLike == true).Count();//Count();
            //ViewBag.countisLike = Convert.ToInt32(IsLike).ToString().Count();//.ToString();//.Count();
            //int isLikeForumId = context.LikeLists.Where(m => m.MemberId == MemberId && m.ForumId == (m.IsLike==true);

            var datas = from m in context.Members
                                    from like in context.LikeLists
                        join fl in context.ForumLists
                        on m.MemberId equals fl.MemberId
                        where m.MemberId == MemberId
                        select new CForumListViewModel2
                        {
                            ForumListId = fl.ForumListId,
                            Title = fl.Title,
                            DueDate = fl.DueDate,
                            ReleaseDatetime = fl.ReleaseDatetime,
                            //IsLike = context.LikeLists.Any(like => like.ForumId == fl.ForumListId && like.MemberId == MemberId && like.IsLike == true),
                            //IsLike = like.IsLike.Value==true,
                            Likes =fl.Likes,
                            //IsLike=fl.MemberId,
                            Watches = fl.Watches,
                            Content = fl.Content,
                            IsPublish = fl.IsPublish
                        };
            Member mem2 = (from m in context.Members where (m.MemberId == MemberId) select m).FirstOrDefault();
            #region 原本load二進位的頭像圖片用
            //原本load二進位的頭像圖片用
            //if (mem2 != null)
            //{
            //    byte[] photo = mem2.Photo;
            //    if (photo != null)
            //    {
            //        string base64String = Convert.ToBase64String(photo);
            //        ViewBag.PhotoBase64 = "data:image/jpeg;base64," + base64String;
            //    }
            //};
            //ViewBag.photo = mem2;
            #endregion
            if (mem2 != null)
            {
                ViewBag.image = mem2.ImagePath;
            }
            //===============================我是分隔線===================================//
            var countsForum = from m in context.Members
                         join fl in context.ForumLists
                         on m.MemberId equals fl.MemberId
                         where m.MemberId == MemberId
                         select new CForumListViewModel2
                         {
                             Title = fl.Title
                        };
            ViewBag.ForumLists = countsForum.ToList().Count();

            var countscomment = from m in context.Members
                        join cm in context.CommentLists
                        on m.MemberId equals cm.MemberId
                        join pl in context.ProductLists
                        on cm.ProductId equals pl.ProductId
                        where m.MemberId == MemberId
                        select new CcommentListViewModel
                        {
                            Title = cm.Title,
                        };
            ViewBag.countscomment = countscomment.ToList().Count();

            var countsorder = from o in context.OrderDetails
                        where o.Order.Member.MemberId == MemberId
                        select new COrdersViewModel { ProductName = o.Trip.Product.ProductName };

            ViewBag.countsorder = countsorder.ToList().Count();

            var countsfavorite = from pl in context.ProductLists
                                 join f in context.Favorites
                                 on pl.ProductId equals f.ProductId
                                 join m in context.Members
                                 on f.MemberId equals m.MemberId
                                 where m.MemberId == MemberId
                                 select new CfavoriteListViewModel
                                 {
                                     ProductName = pl.ProductName,
                                 };
            ViewBag.countsfavorite = countsfavorite.ToList().Count();

            var countscoupon = from m in context.Members
                        join c in context.Coupons
                        on m.MemberId equals c.MemberId
                        join cl in context.CouponLists
                        on c.CouponListId equals cl.CouponListId
                        where m.MemberId == MemberId
                        select new couponListViewModel
                        {
                            CouponName = cl.CouponName,
                        };
            ViewBag.countscoupon = countscoupon.ToList().Count();

            //===============================我是分隔線===================================//
            Member x = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            var levelvm = from m in context.Members
                          join l in context.LevelLists
                          on m.LevelId equals l.LevelId
                          where MemberId == m.MemberId
                          select m.LevelId;
            if (x.LevelId == 1)
                ViewBag.level = "一般會員";
            else if (x.LevelId == 2)
                ViewBag.level = "白銀會員";
            else if (x.LevelId == 3)
                ViewBag.level = "黃金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;



            return View(datas.Distinct());
        }

        public IActionResult SignalR()
        {
            int MemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));

            Member mem2 = (from m in context.Members where (m.MemberId == MemberId) select m).FirstOrDefault();
            #region 原本load二進位的頭像圖片用
            //原本load二進位的頭像圖片用
            //if (mem2 != null)
            //{
            //    byte[] photo = mem2.Photo;
            //    if (photo != null)
            //    {
            //        string base64String = Convert.ToBase64String(photo);
            //        ViewBag.PhotoBase64 = "data:image/jpeg;base64," + base64String;
            //    }
            //};
            //ViewBag.photo = mem2;
            #endregion
            if (mem2 != null)
            {
                ViewBag.image = mem2.ImagePath;
            }
            //===============================我是分隔線===================================//
            var countsForum = from m in context.Members
                              join fl in context.ForumLists
                              on m.MemberId equals fl.MemberId
                              where m.MemberId == MemberId
                              select new CForumListViewModel2
                              {
                                  Title = fl.Title
                              };
            ViewBag.ForumLists = countsForum.ToList().Count();

            var countscomment = from m in context.Members
                                join cm in context.CommentLists
                                on m.MemberId equals cm.MemberId
                                join pl in context.ProductLists
                                on cm.ProductId equals pl.ProductId
                                where m.MemberId == MemberId
                                select new CcommentListViewModel
                                {
                                    Title = cm.Title,
                                };
            ViewBag.countscomment = countscomment.ToList().Count();

            var countsorder = from o in context.OrderDetails
                              where o.Order.Member.MemberId == MemberId
                              select new COrdersViewModel { ProductName = o.Trip.Product.ProductName };

            ViewBag.countsorder = countsorder.ToList().Count();

            var countsfavorite = from pl in context.ProductLists
                                 join f in context.Favorites
                                 on pl.ProductId equals f.ProductId
                                 join m in context.Members
                                 on f.MemberId equals m.MemberId
                                 where m.MemberId == MemberId
                                 select new CfavoriteListViewModel
                                 {
                                     ProductName = pl.ProductName,
                                 };
            ViewBag.countsfavorite = countsfavorite.ToList().Count();

            var countscoupon = from m in context.Members
                               join c in context.Coupons
                               on m.MemberId equals c.MemberId
                               join cl in context.CouponLists
                               on c.CouponListId equals cl.CouponListId
                               where m.MemberId == MemberId
                               select new couponListViewModel
                               {
                                   CouponName = cl.CouponName,
                               };
            ViewBag.countscoupon = countscoupon.ToList().Count();

            //===============================我是分隔線===================================//
            Member x = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            var levelvm = from m in context.Members
                          join l in context.LevelLists
                          on m.LevelId equals l.LevelId
                          where MemberId == m.MemberId
                          select m.LevelId;
            if (x.LevelId == 1)
                ViewBag.level = "一般會員";
            else if (x.LevelId == 2)
                ViewBag.level = "白銀會員";
            else if (x.LevelId == 3)
                ViewBag.level = "黃金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View();
        }


    }
}

