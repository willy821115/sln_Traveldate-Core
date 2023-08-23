//using AspNetCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualBasic;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.ViewModels;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace prj_Traveldate_Core.Controllers
{
    public class MemberController : Controller
    {
        TraveldateContext context = new TraveldateContext();
        public IActionResult Index() // 左側欄 先維持原版V
        {
            int MemberId = 1;
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
                ViewBag.level = "白金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View();
        }
        public IActionResult basicInfo() //基本資料設定 V
        {
            int MemberId = 1;
            Member mem = context.Members.FirstOrDefault(m => m.MemberId == MemberId);

            var levelvm = from m in context.Members
                          join l in context.LevelLists
                          on m.LevelId equals l.LevelId
                          where MemberId == m.MemberId
                          select m;
            if (mem.LevelId == 1)
                ViewBag.level = "一般會員";
            else if (mem.LevelId == 2)
                ViewBag.level = "白銀會員";
            else if (mem.LevelId == 3)
                ViewBag.level = "白金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (mem.FirstName == mem.FirstName)
                ViewBag.firstName = mem.FirstName;

            if (mem.LastName == mem.LastName)
                ViewBag.LastName = mem.LastName;

            return View(mem);
            #region 先註解掉的程式碼
            //int MemberId = 3;
            //Member mem = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            //// var datas =context.Members.Where(mm=>mm.MemberId == MemberId).FirstOrDefault();
            //return View(mem);
            //CMemberLevelViewModel viewModel = new CMemberLevelViewModel();
            //viewModel.Member = context.Members.Where(m => m.MemberId == MemberId).ToList();
            //return View(viewModel);
            //CMemberLevelViewModel vmml = new CMemberLevelViewModel();
            //Member mem =context.Members.FirstOrDefault(m=>m.MemberId == MemberId);
            //if (mem != null) {
            //    mem.FirstName=Views_Member_commentList.
            //return View(mem);

            //var datas = from m in context.Members
            //            join l in context.LevelLists
            //            on m.LevelId equals l.LevelId
            //            where m.MemberId == MemberId
            //            select new CMemberLevelViewModel
            //            {
            //                FirstName = m.FirstName,
            //                LastName = m.LastName,
            //                Gender = m.Gender,
            //                BirthDate = m.BirthDate,
            //                Phone = m.Phone,
            //                Email = m.Email,
            //                LevelId = m.LevelId,
            //            };
            //return View(datas);
            //var levelvm = from m in vmml.Member
            //              join l in vmml.LevelList
            //              on m.LevelId equals l.LevelId
            //              where MemberId == m.MemberId
            //              select m;
            //ViewBag.level = levelvm.ToString();
            #endregion
        }
        [HttpPost]
        public IActionResult basicInfo(Member edit) //基本資料設定edit V
        {
            int MemberId = 1;
            Member mDB = context.Members.FirstOrDefault(m => m.MemberId == edit.MemberId);
            if (mDB != null)
            {
                mDB.FirstName = edit.FirstName;
                mDB.LastName = edit.LastName;
                mDB.Gender = edit.Gender;
                mDB.BirthDate = edit.BirthDate;
                mDB.Phone = edit.Phone;
                mDB.Email = edit.Email;
                mDB.MemberId = edit.MemberId;

                context.SaveChanges();
            }
            return RedirectToAction("Index");
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
            int MemberId = 1;
            CpasswordChangeViewModel prd = new CpasswordChangeViewModel();

            prd.MemberId = MemberId;
            Member x = context.Members.FirstOrDefault(m => m.MemberId == prd.MemberId);

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
                ViewBag.level = "白金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View(prd);
            #region 先註解掉的程式碼
            //CpasswordChangeViewModel mem = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            //Member mem=context.Members.FirstOrDefault(m=>m.MemberId==MemberId);
            //Member m=context.Members.FirstOrDefault(m=>m.Password==prd.txtNewPassword);
            #endregion
        }
        [HttpPost]
        public IActionResult passwordChange(CpasswordChangeViewModel edit) //密碼更改 edit V
        {
            int memberId = 1;
            Member mDB = context.Members.FirstOrDefault(m => m.MemberId == memberId);

            if (mDB != null)
            {
                if (edit.txtNewPassword == edit.txtCheckPassword)
                {
                    mDB.Password = edit.txtNewPassword;
                    context.Entry(mDB).State = EntityState.Modified;
                    context.SaveChanges();

                  Thread.Sleep(2000);
                }
                else if(edit.txtNewPassword != edit.txtCheckPassword )
                {
                    Thread.Sleep(2000);
                    return RedirectToAction("passwordChange");
                }
            }
            Thread.Sleep(2000);
            return RedirectToAction("Index");
        }
    
        public IActionResult couponList() //優惠券清單 new V
        {
            int MemberId = 1;
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
                            Discount = cl.Discount,
                            Description = cl.Description,
                            DueDate = cl.DueDate
                        };

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
                ViewBag.level = "白金會員";
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
            int MemberId = 1;
            var datas = from m in context.Members
                        join cm in context.Companions
                        on m.MemberId equals cm.MemberId
                        where m.MemberId == MemberId
                        select new CCompanionViewModel
                        {
                            LastName = cm.LastName,
                            FirstName = cm.FirstName,
                            Phone = cm.Phone,
                            BirthDate = cm.BirthDate                           
                        };

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
                ViewBag.level = "白金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View(datas);
        }
        public IActionResult addCompanion() //新增旅伴資料
        {
            int MemberId = 1;
            CCompanionViewModel cm = new CCompanionViewModel();
            cm.MemberId = MemberId;

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
                ViewBag.level = "白金會員";
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
            if (
                (string.IsNullOrEmpty(vm.LastName)) ||
                (string.IsNullOrEmpty(vm.FirstName)) ||
                (string.IsNullOrEmpty(vm.Phone))
               )
                return RedirectToAction("showCompanion");
            else
            {
                Companion cpDB = new Companion();
                if (cpDB!= null)
                {
                    cpDB.LastName = vm.LastName;
                    cpDB.FirstName = vm.FirstName;
                    cpDB.Phone = vm.Phone;
                    cpDB.MemberId = vm.MemberId;
                    cpDB.BirthDate = vm.BirthDate;

                    context.Companions.Add(cpDB);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("showCompanion");
        }

        public IActionResult favoriteList() //收藏清單new V
        {
            int MemberId = 1;
            var datas = from pl in context.ProductLists
                        join f in context.Favorites
                        on pl.ProductId equals f.ProductId
                        join m in context.Members
                        on f.MemberId equals m.MemberId
                        where m.MemberId == MemberId
                        select new CfavoriteListViewModel
                        {
                            ProductName = pl.ProductName,
                            ProductId = pl.ProductId,
                            MemberId = m.MemberId,
                            //Description = pl.Description,
                            Outline = pl.Outline,
                        };

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
                ViewBag.level = "白金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View(datas.Distinct());
        }

        public IActionResult favoriteListDelete()  //收藏清單Delete V
        {
            int id = 1;
            var pid = from p in context.ProductLists
                      join f in context.Favorites
                      on p.ProductId equals f.ProductId
                      where f.MemberId == id
                      select p.ProductId;
            var mid = from f in context.Favorites
                      join m in context.Members
                      on f.MemberId equals m.MemberId
                      where f.MemberId == id
                      select f.MemberId;
            Member mm = context.Members.FirstOrDefault(p => p.MemberId == id);
            Favorite ff = context.Favorites.FirstOrDefault(p => p.MemberId == id);

            if (mm != null)
            {
                if (ff != null)
                    context.Favorites.Remove(ff);
                    context.SaveChanges();
            }
            return RedirectToAction("favoriteList");
        }
        public IActionResult orderList() //會員訂單new V
        {
            int MemberId = 1;
            var datas = from o in context.OrderDetails
                        where o.Order.Member.MemberId == MemberId
                        select new COrdersViewModel { Date = o.Trip.Date, Datetime = string.Format("{0:yyyy-MM-dd}",o.Order.Datetime ) , ProductName = o.Trip.Product.ProductName };

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
                ViewBag.level = "白金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View(datas.Distinct());
            #region 先註解掉的程式碼
            //var datas = from tripde in context.TripDetails
            //            join trip in context.Trips
            //            on tripde.ProductId equals trip.ProductId
            //            join orderde in context.OrderDetails
            //            on trip.TripId equals orderde.TripId

            //            join order in context.Orders
            //            on orderde.OrderId equals order.OrderId

            //            join m in context.Members
            //            on order.MemberId equals m.MemberId

            //            where m.MemberId == id
            //            select new COrdersViewModel
            //            {
            //                OrderId = orderde.OrderId,
            //                Date = trip.Date,
            //                Datetime = order.Datetime,
            //                //TripDetaill = tripde.TripDetaill,
            //            };
            #endregion
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
            int MemberId = 1;
            var datas = from m in context.Members
                        join cm in context.CommentLists
                        on m.MemberId equals cm.MemberId
                        join pl in context.ProductLists
                        on cm.ProductId equals pl.ProductId
                        where m.MemberId == MemberId
                        select new CcommentListViewModel
                        {
                            Title = cm.Title,
                            Content = cm.Content,
                            CommentScore = cm.CommentScore,
                            Date = cm.Date,
                            ProductName = pl.ProductName
                        };

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
                ViewBag.level = "白金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View(datas);
        }

        public IActionResult commentListDelete() //我的評論Delete 
        {
            int id = 1;
            var cid = from c in context.CommentLists
                      join m in context.Members
                      on c.MemberId equals m.MemberId
                      where m.MemberId == id
                      select c.MemberId;
            var pid = from c in context.CommentLists
                      join p in context.ProductLists
                      on c.ProductId equals p.ProductId
                      where c.ProductId == p.ProductId
                      select c.MemberId;
            Member mm = context.Members.FirstOrDefault(p => p.MemberId == id);
            CommentList ff = context.CommentLists.FirstOrDefault(p => p.MemberId == id);

                if(mm!= null)
            {
                if(ff!=null)
                {
                    context.CommentLists.Remove(ff);
                    context.SaveChanges();
                }
            }

            return RedirectToAction("index");
        }

        #region 添加評論view 暫時用不到2023.08.20
        public IActionResult addcomment() //添加評論 先維持舊版V
        {
            int MemberId = 1;
            var datas = from m in context.Members
                        join cm in context.CommentLists
                        on m.MemberId equals cm.MemberId
                        join pl in context.ProductLists
                        on cm.ProductId equals pl.ProductId
                        where m.MemberId == MemberId
                        select new CcommentListViewModel
                        {
                            Title = cm.Title,
                            Content = cm.Content,
                            CommentScore = cm.CommentScore,
                            Date = cm.Date,
                            ProductName = pl.ProductName
                        };

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
                ViewBag.level = "白金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View(datas);
        }
        #endregion
        [HttpPost]
        public IActionResult addcommentCreate()
        {
            //int MemberId = 1;
            //CcommentListViewModel vm = new CcommentListViewModel();
            //vm.MemberId = MemberId;
            //Member x = context.Members.FirstOrDefault(m => m.MemberId == MemberId);

            var commentList = new CommentList();
            
            CommentList cmDB = new CommentList();
            cmDB.MemberId = Convert.ToInt32(Request.Form["MemberId"]);
            cmDB.Title = Request.Form["Title"];
            cmDB.Content = Request.Form["Content"];
            cmDB.CommentScore = Convert.ToInt32(Request.Form["CommentScore"]);

            //cmDB.Date = Request.Form["txtTitle"];
            //cmDB.ProductId = Request.Form["txtTitle"];

            //if (cmDB != null)
            //{
            //    cmDB.Title = vm.Title;
            //    cmDB.Content = vm.Content;
            //    cmDB.CommentScore = vm.CommentScore;
            //    cmDB.MemberId = vm.MemberId;
            //    cmDB.Date = vm.Date;
            //    cmDB.ProductId = vm.ProductId;
            
                context.CommentLists.Add(cmDB);
                context.SaveChanges();
            //}
            return RedirectToAction("orderList");
        }
        public IActionResult forumList() //我的揪團new V
        {
            int MemberId =1;
            var datas = from m in context.Members
                        join fl in context.ForumLists
                        on m.MemberId equals fl.MemberId
                        where m.MemberId == MemberId
                        select new CForumListViewModel2
                        {
                            ForumListId = fl.ForumListId,
                            Title = fl.Title,
                            DueDate = fl.DueDate,
                            ReleaseDatetime = fl.ReleaseDatetime,
                            Likes = fl.Likes,
                            Watches = fl.Watches,
                            Content = fl.Content
                        };

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
                ViewBag.level = "白金會員";
            else
                ViewBag.level = "黑鑽會員";

            if (x.FirstName == x.FirstName)
                ViewBag.firstName = x.FirstName;

            if (x.LastName == x.LastName)
                ViewBag.LastName = x.LastName;

            return View(datas);
        }
    }
}

