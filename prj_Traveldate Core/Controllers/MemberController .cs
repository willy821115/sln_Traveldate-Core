using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.Controllers
{
    public class MemberController : Controller
    {
        public IActionResult Index() //左側欄
        {
            return View();
        }
        public IActionResult basicInfo() //基本資料設定
        {
            //Models.IndexController S ;
            return View();
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
            TraveldateContext db = new TraveldateContext();
            var datas = from m in db.Members
                        join c in db.Coupons
                        on m.MemberId equals c.MemberId
                        join cl in db.CouponLists
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
