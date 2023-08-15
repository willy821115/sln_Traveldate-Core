using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualBasic;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.ViewModels;

namespace prj_Traveldate_Core.Controllers
{
    public class MemberController : Controller
    {
        TraveldateContext context = new TraveldateContext();
        public IActionResult Index() //左側欄
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
        //public IActionResult couponList3(int? id = 1) //優惠券清單
        //{
        //    var datas = from m in context.Members
        //                join c in context.Coupons
        //                on m.MemberId equals c.MemberId
        //                join cl in context.CouponLists
        //                on c.CouponListId equals cl.CouponListId
        //                where m.MemberId == id
        //                select new couponListViewModel
        //                { CouponListId=cl.CouponListId,
        //                    CouponName = cl.CouponName,
        //                    Discount = cl.Discount,
        //                    Description = cl.Description,
        //                    DueDate = cl.DueDate 
        //                };
        //    return View(datas);

        //}
        public IActionResult showCompanion(int? id = 1) //新增旅伴資料
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

        /*public IActionResult addCompanion(Companion cm) *///新增旅伴資料
       public IActionResult addCompanion()
        {
            //int MemberId = 1;
            //context.Companions.Add(cm);
            //context.SaveChanges();
            return View();
        }
        public IActionResult favoriteList(int? id = 1) //收藏清單V
        {
            var datas = from m in context.Members
                        join f in context.Favorites
                        on m.MemberId equals f.MemberId
                        join pl in context.ProductLists
                        on f.ProductId equals pl.ProductId
                        where m.MemberId == id
                        select new CfavoriteListViewModel
                        {
                            ProductName = pl.ProductName,
                            Description = pl.Description,
                            Outline = pl.Outline,
                        };
            return View(datas.Distinct());
        }
        public IActionResult orderList() //會員訂單
        {
            return View();
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

