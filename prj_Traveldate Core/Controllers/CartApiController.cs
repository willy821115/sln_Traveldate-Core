using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;

namespace prj_Traveldate_Core.Controllers
{
    public class CartApiController : SuperController
    {
        int _memberID = 0;
        TraveldateContext _context;
        public CartApiController()
        {
            _context = new TraveldateContext();
        }

        //購物車
        //  加入最愛(修改最愛)
        public IActionResult AddToFavo(int id)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            Favorite favo = new Favorite()
            {
                MemberId = _memberID,
                ProductId = id
            };
            _context.Favorites.Add(favo);
            _context.SaveChanges();
            return RedirectToAction("ShoppingCart", "Cart");
        }
        public IActionResult DeleFromFavo(int id)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            var favo = _context.Favorites.Where(o=>o.MemberId==_memberID&&o.ProductId==id).FirstOrDefault();
            if (favo != null)
            {
                _context.Favorites.Remove(favo);
            }
            _context.SaveChanges();
            return RedirectToAction("ShoppingCart", "Cart");
        }
        //  自購物車刪除(刪除OD)
        public IActionResult DeleFromCart(int id)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            var od = _context.OrderDetails.Where(o => o.OrderDetailsId==id).FirstOrDefault();
            if (od != null)
            {
                _context.OrderDetails.Remove(od);
            }
            _context.SaveChanges();
            return RedirectToAction("ShoppingCart", "Cart");
        }
        //  刪除所有已選
        [HttpPost]
        public IActionResult DeleAll(int[] orderDetailID)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            for(int i = 0; i < orderDetailID.Length; i++)
            {
                var od = _context.OrderDetails.Where(o => o.OrderDetailsId == orderDetailID[i]).FirstOrDefault();
                if (od != null)
                {
                    _context.OrderDetails.Remove(od);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("ShoppingCart", "Cart");
        }

        // +-按鈕 修改數量
        public IActionResult ItemPlus(int id) //odid
        {
            OrderDetail od = _context.OrderDetails.Find(id);
            if (od != null)
            {
                od.Quantity += 1;
                _context.SaveChanges();
                return Content(od.Quantity.ToString());
            }
            return NoContent();
        }

        public IActionResult ItemMinus(int id) //odid
        {
            OrderDetail od = _context.OrderDetails.Find(id);
            if (od != null)
            {
                if (od.Quantity > 1)
                {
                    od.Quantity -= 1;
                    _context.SaveChanges();
                    return Content(od.Quantity.ToString());
                }
            }
            return NoContent();
        }

        //TODO  編輯訂購內容(修改OD)
        public IActionResult LoadTrips(int id) //tripid
        {
            var pid = _context.Trips.Find(id).ProductId;
            var trips = _context.Trips.Where(t => t.ProductId == pid && t.Date>DateTime.Now).OrderBy(t=>t.Date);
            foreach (var trip in trips)
            {
                if (trip.Discount == null)
                {
                    trip.Discount = 0;
                }
            }
            return Json(trips);
        }

        public IActionResult EditOrder(int odid, int num, int tripid)
        {



            return RedirectToAction("ShoppingCart");
        }


        //TODO  抓推薦
        //TODO  抓瀏覽紀錄


        //透過ID取得同行者資料
        public IActionResult GetCompanionByID(int id)
        {
            var companion = _context.Companions.Where(c=>c.CompanionId==id).Select(c=>c).FirstOrDefault();
            return Json(companion);
        }

        //取得購物車內商品數量
        [HttpGet]
        public IActionResult GetCartCount()
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            var count = _context.OrderDetails.Where(o => o.Order.IsCart == true && o.Order.MemberId == _memberID).Count();
            return Content(count.ToString());
        }

        //揪團用結帳
        //傳入ScheduleID + CouponListID + Discount (Quantity? SellingPrice?)


    }
}
