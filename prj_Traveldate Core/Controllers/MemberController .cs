using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.Controllers
{
    public class MemberController : Controller
    {
        TraveldateContext context = new TraveldateContext();
        public IActionResult Index() //左側欄
        {
            return View();
        }
        public IActionResult basicInfo() //基本資料設定
        {
            var datas = from mm in context.Members where mm.MemberId == 1 select mm;

            return View(datas);
        }
        public IActionResult passwordChange() //密碼更改
        {
            return View();
        }
        public IActionResult couponList() //優惠券清單
        {
            return View();
        }
        public IActionResult couponList2(int? id) //優惠券清單
        {
            
            var datas = from m in context.Members
                        join c in context.Coupons
                        on m.MemberId equals c.MemberId
                        join cl in context.CouponLists
                        on c.CouponListId equals cl.CouponListId
                        where m.MemberId == c.MemberId
                        select new { cl.CouponListId, cl.CouponName, cl.Discount, cl.Description, cl.DueDate };
            return View(datas.ToString());
        }
        public IActionResult addCompanion() //新增旅伴資料
        {
            return View();
        }
        public IActionResult favoriteList() //收藏清單
        {
            return View();
        }
        public IActionResult orderList() //會員訂單
        {
            return View();
        }
        public IActionResult commentList() //我的評論
        {
            return View();
        }
        public IActionResult addcomment() //添加評論
        {
            return View();
        }
        public IActionResult forumList() //我的揪團
        {
            return View();
        }
    }
}
