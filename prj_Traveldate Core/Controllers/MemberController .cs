using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualBasic;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.ViewModels;
using System.Globalization;
using System.Linq;

namespace prj_Traveldate_Core.Controllers
{
    public class MemberController : Controller
    {
        TraveldateContext context = new TraveldateContext();
        public IActionResult Index() // 左側欄 先維持原版V
        {
            return View();
        }
        public IActionResult basicInfo() //基本資料設定 V
        {
            int MemberId = 3;
            Member mem = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
           // var datas =context.Members.Where(mm=>mm.MemberId == MemberId).FirstOrDefault();

            return View(mem);
        }
        [HttpPost]
        public IActionResult basicInfo(Member edit) //基本資料設定edit V
        {
            int MemberId = 3;
            Member mDB = context.Members.FirstOrDefault(m=>m.MemberId == edit.MemberId);          
                if (mDB != null)
            {
                 
                    mDB.FirstName = edit.FirstName;
                    mDB.LastName = edit.LastName;
                    mDB.Gender = edit.Gender;
                    mDB.BirthDate = edit.BirthDate;
                    mDB.Photo = edit.Photo;
                    mDB.Email = edit.Email;

                    context.SaveChanges(); 
            }

            return RedirectToAction("Index");
        }
        
        public IActionResult passwordChange() //密碼更改 先維持原版V
        {
            //int MemberId = 3;
            //CpasswordChangeViewModel mem = context.Members.FirstOrDefault(m => m.MemberId == MemberId);
            int MemberId = 3;
            CpasswordChangeViewModel prd=new CpasswordChangeViewModel();
            //Member mem=context.Members.FirstOrDefault(m=>m.MemberId==MemberId);
            //Member m=context.Members.FirstOrDefault(m=>m.Password==prd.txtNewPassword);
            prd.MemberId = MemberId;
            return View(prd);
        }
        [HttpPost]
        public IActionResult passwordChange(CpasswordChangeViewModel edit) //密碼更改 edit V
        {
            if (string.IsNullOrEmpty(edit.txtNewPassword) || string.IsNullOrEmpty(edit.txtCheckPassword))
            {
                ModelState.AddModelError(string.Empty, "新密碼與確認新密碼不得為空白，請確認後再次提交");
                return View(edit);
            }

            int memberId = 3; 
            Member mDB = context.Members.FirstOrDefault(m => m.MemberId == memberId);

            if (mDB != null)
            {
                if (edit.txtNewPassword == edit.txtCheckPassword)
                {
                    mDB.Password = edit.txtNewPassword;
                    //context.Members.Add(mDB);
                    context.Entry(mDB).State=EntityState.Modified;
                    context.SaveChanges();
                    return RedirectToAction("passwordChange");
                }             
                else
                {
                    ModelState.AddModelError(string.Empty, "新密碼與確認新密碼不一致，請再次確認後提交");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "找不到對應的會員。");
            }

            return View("Index");
        }
        public IActionResult couponList(int? id = 1) //優惠券清單 new V
        {
            var datas = from m in context.Members
                        join c in context.Coupons
                        on m.MemberId equals c.MemberId
                        join cl in context.CouponLists
                        on c.CouponListId equals cl.CouponListId
                        where m.MemberId == id
                        select new couponListViewModel
                        {
                            CouponListId = cl.CouponListId,
                            CouponName = cl.CouponName,
                            Discount = cl.Discount,
                            Description = cl.Description,
                            DueDate = cl.DueDate
                        };
            return View(datas);
        }
        public IActionResult addCompanion() //新增旅伴資料
        {
            int MemberId = 3;
            CCompanionViewModel m =new CCompanionViewModel();
            m.MemberId = MemberId;
            return View(m);
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
                if (cpDB != null)
                {
                    cpDB.LastName = vm.LastName;
                    cpDB.FirstName = vm.FirstName;
                    cpDB.Phone = vm.Phone;

                    context.Companions.Add(cpDB);
                    context.SaveChanges();
                }               
            }
            return RedirectToAction("index");
        }
        public IActionResult showCompanion(int? id = 3) //顯示常用旅伴
        {
            var datas = from m in context.Members
                        join cm in context.Companions
                        on m.MemberId equals cm.MemberId
                        where m.MemberId == id
                        select new CCompanionViewModel
                        {
                            LastName = cm.LastName,
                            FirstName = cm.FirstName,
                            Phone = cm.Phone,
                            BirthDate = cm.BirthDate,
                        };
            return View(datas);
        }
        public IActionResult favoriteList(int? id=3) //收藏清單new V
        {
            
            var datas = from pl in context.ProductLists
                        join f in context.Favorites
                        on pl.ProductId equals f.ProductId
                        join m in context.Members
                        on f.MemberId equals m.MemberId
                        where m.MemberId == id
                        select new CfavoriteListViewModel
                        {
                            ProductName = pl.ProductName,
                            ProductId=pl.ProductId,
                            MemberId=m.MemberId,
                            //Description = pl.Description,
                            Outline = pl.Outline,
                        };
            return View(datas.Distinct());
        }
        //[HttpPost]
        //public IActionResult favoriteList(int ProductId, int MemberId) //收藏清單delete 
        //{
        //    ////Favorite fDB = new Favorite();
        //    //var fDB = from f in context.Favorites where f.FavoriteId == ProductId && f.MemberId == MemberId select
        //    ////Favorite fav = context.Favorites.FirstOrDefault(fa => fa.MemberId == vm.MemberId);
        //    //    if(fDB != null)
        //    //    {
        //    //        context.Favorites.Remove(fDB);
        //    //        context.SaveChanges();
        //    //    }
            
        //    //return RedirectToAction("favoriteList");
        //}
        public IActionResult orderList(int? id = 1) //會員訂單new V
        {
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

            var datas = from o in context.OrderDetails
                        where o.Order.Member.MemberId == id
                        select new COrdersViewModel { Date = o.Trip.Date, Datetime = string.Format("{0:yyyy-MM-dd}",o.Order.Datetime ) , ProductName = o.Trip.Product.ProductName };
            return View(datas.Distinct());
        }
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
        public IActionResult commentList(int? id = 1) //我的評論new V
        {
            var datas = from m in context.Members
                        join cm in context.CommentLists
                        on m.MemberId equals cm.MemberId
                        join pl in context.ProductLists
                        on cm.ProductId equals pl.ProductId
                        where m.MemberId == id
                        select new CcommentListViewModel
                        {
                            Title = cm.Title,
                            Content = cm.Content,
                            CommentScore = cm.CommentScore,
                            Date = cm.Date,
                            ProductName = pl.ProductName
                        };
            return View(datas);
        }

        public IActionResult addcomment() //添加評論 先維持舊版V
        {
            return View();
        }

        public IActionResult forumList(int? id = 1) //我的揪團new V
        {
            var datas = from m in context.Members
                        join fl in context.ForumLists
                        on m.MemberId equals fl.MemberId
                        where m.MemberId == id
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
            return View(datas);
        }
    }
}

