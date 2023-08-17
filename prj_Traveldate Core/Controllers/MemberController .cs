using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualBasic;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.ViewModels;
using System.Globalization;

namespace prj_Traveldate_Core.Controllers
{
    public class MemberController : Controller
    {
        TraveldateContext context = new TraveldateContext();
        public IActionResult Index() //左側欄V
        {
            return View();
        }
        public IActionResult basicInfo() //基本資料設定V
        {
            var datas = from mm in context.Members where mm.MemberId == 1 select mm;

            return View(datas);
        }
        public IActionResult passwordChange() //密碼更改V
        {
                return View();
        }
        public IActionResult couponList(int? id = 1) //優惠券清單V
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

        /*public IActionResult addCompanion(Companion cm) *///新增旅伴資料
        public IActionResult addCompanion() //新增旅伴資料
        {
            //int MemberId = 1;
            //context.Companions.Add(cm);
            //context.SaveChanges();
            return View();
        }
        public IActionResult showCompanion(int? id = 1) //顯示常用旅客
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
        public IActionResult favoriteList(int? id = 1) //收藏清單V
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
                            //Description = pl.Description,
                            Outline = pl.Outline,

                        };
            return View(datas.Distinct());
        }
        public IActionResult orderList(int? id = 1) //會員訂單
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
        public IActionResult commentList(int? id = 1) //我的評論V
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

        public IActionResult addcomment() //添加評論V
        {
            return View();
        }
        public IActionResult forumList(int? id = 1) //我的揪團V
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

